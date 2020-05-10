<Query Kind="Program">
  <Reference>C:\source\_Stardew\SMAPI\bin\Debug\SMAPI.Toolkit\netstandard2.0\SMAPI.Toolkit.CoreInterfaces.dll</Reference>
  <Reference>C:\source\_Stardew\SMAPI\bin\Debug\SMAPI.Toolkit\netstandard2.0\SMAPI.Toolkit.dll</Reference>
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <NuGetReference>Pathoschild.Http.FluentClient</NuGetReference>
  <Namespace>StardewModdingAPI</Namespace>
  <Namespace>StardewModdingAPI.Toolkit</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.WebApi</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.Wiki</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModData</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModScanning</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.UpdateData</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Serialization</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Serialization.Models</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Utilities</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

/*

  This script...
     1. fetches the list of SMAPI mods from the wiki;
     2. downloads every Git repository for those mods to a local folder.

*/

/*********
** Configuration
*********/
/// <summary>The absolute path to the folder which contains the Git repositories.</summary>
private readonly string RootPath = @"C:\source\_Stardew\_smapi-mod-dump\source";

/// <summary>Patterns matching valid file or folder names that are legitimately part of the Git repository, but should be removed from the cloned repositories.</summary>
private readonly Regex[] IgnoreLegitNames =
{
	// folders
	new Regex(@"^\.git$", RegexOptions.Compiled),

	// files
	new Regex(@"^\.gitattributes$", RegexOptions.Compiled),
	new Regex(@"^\.gitignore$", RegexOptions.Compiled)
};

/// <summary>Patterns matching valid file or folder names that shouldn't be in Git.</summary>
private readonly Regex[] IgnoreIncorrectNames =
{
	// folders
	new Regex(@"^_releases$", RegexOptions.Compiled),
	new Regex(@"^\.vs$", RegexOptions.Compiled),
	new Regex(@"^bin$", RegexOptions.Compiled),
	new Regex(@"^obj$", RegexOptions.Compiled),
	new Regex(@"^packages$", RegexOptions.Compiled),

	// files
	new Regex(@"\.csproj\.user$", RegexOptions.Compiled),
	new Regex(@"\.DotSettings\.user$", RegexOptions.Compiled),
	new Regex(@"\.psd$", RegexOptions.Compiled),
	new Regex(@"\.userprefs$", RegexOptions.Compiled),
	new Regex(@"\.xcf$", RegexOptions.Compiled),
	new Regex(@"\.zip$", RegexOptions.Compiled),
	new Regex(@"_(?:BACKUP|BASE|LOCAL)_\d+\.[a-z]+", RegexOptions.Compiled) // merge backups
};

/// <summary>Patterns matching valid file or folder names that shouldn't be in Git for a specific repo folder.</summary>
private readonly IDictionary<string, Regex> IgnoreFilesByRepo = new Dictionary<string, Regex>
{
	["~JessebotX"] = new Regex(@"^oldversions$", RegexOptions.Compiled),
	["Birthday Mail"] = new Regex(@"^Release$", RegexOptions.Compiled),
	["Chest Label System"] = new Regex(@"^zip\.exe$", RegexOptions.Compiled),
	["Faster Run"] = new Regex(@"^Release$", RegexOptions.Compiled),
	["HD Sprites"] = new Regex(@"^tools$", RegexOptions.Compiled), // dependencies, including exe over 10MB
	["StackSplitX"] = new Regex(@"^Demo\.gif$", RegexOptions.Compiled), // 10MB file
};

/// <summary>The source URLs to skip when cloning repositories. This should match the GitHub repository name or custom URL specified on the wiki.</summary>
private readonly HashSet<string> IgnoreSourceUrls = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
{
	// SMAPI
	"Pathoschild/SMAPI"
};

/// <summary>Maps GitHub URLs to the folder name to use, overriding the generated name.</summary>
public IDictionary<string, string> OverrideFolderNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
{
	["https://github.com/TheThor59/StardewMods.git"] = "Mods.TheThor59"
};


/*********
** Script
*********/
async Task Main()
{
	/****
	** Initialise
	****/
	var toolkit = new ModToolkit();
	var rootDir = new DirectoryInfo(this.RootPath);
	rootDir.Create();


	/****
	** Fetch Git URLs
	****/
	List<ModRepository> repos = new List<ModRepository>();
	Helper.Print("Fetching Git repository URLs...");
	{
		// fetch mods
		WikiModEntry[] mods = (await toolkit.GetWikiCompatibilityListAsync()).Mods.Where(p => p.ContentPackFor == null).ToArray();
		int totalMods = mods.Length;
		mods = mods
			.Where(mod => !this.IgnoreSourceUrls.Contains(mod.GitHubRepo) && !this.IgnoreSourceUrls.Contains(mod.CustomSourceUrl))
			.ToArray();

		// fetch repositories
		repos.AddRange(
			mods
				.Select(mod => new { mod, gitUrl = ModRepository.GetGitUrl(mod) })
				.Where(p => p.gitUrl != null)
				.GroupBy(p => p.gitUrl, p => p.mod, StringComparer.InvariantCultureIgnoreCase)
				.Select(group => new ModRepository(group.Key, group))
		);

		// find invalid custom source URLs
		string[] invalidUrls = mods
			.Except(repos.SelectMany(p => p.Mods))
			.Where(mod => !string.IsNullOrWhiteSpace(mod.CustomSourceUrl))
			.Select(mod => mod.CustomSourceUrl)
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.ToArray();

		// print stats
		int uniqueRepos = repos.Count;
		int haveCode = repos.SelectMany(repo => repo.Mods).Count();
		int haveSharedRepo = haveCode - uniqueRepos;

		Helper.Print($"   {totalMods} mods in the SMAPI compatibility list.");
		Helper.Print($"   {haveCode} mods ({this.GetPercentage(haveCode, totalMods)}) have a Git repository.");
		Helper.Print($"   {haveSharedRepo} repositories ({this.GetPercentage(haveSharedRepo, haveCode)}) contain multiple mods.");
		if (invalidUrls.Any())
		{
			Helper.Print($"   Found {invalidUrls.Length} unsupported source URLs on the wiki:", Severity.Trace);
			foreach (string url in invalidUrls.OrderBy(p => p))
				Helper.Print($"      {url}", Severity.Trace);
		}
	}
	Console.WriteLine();

	/****
	** Generate folder names
	****/
	IDictionary<string, ModRepository> repoFolders = new Dictionary<string, ModRepository>();
	foreach (ModRepository repo in repos)
	{
		if (!this.OverrideFolderNames.TryGetValue(repo.GitUrl, out string folderName))
			folderName = repo.GetRecommendedFolderName();

		if (repoFolders.ContainsKey(folderName))
			throw new InvalidOperationException($"Folder name conflict: can't add {folderName}, it matches both [{repo.GitUrl}] and [{repoFolders[folderName].GitUrl}].");

		repoFolders[folderName] = repo;
	}

	/****
	** Clear old repos
	****/
	if (rootDir.EnumerateFileSystemInfos().Any())
	{
		Helper.Print($"Deleting old Git repositories...");
		foreach (FileSystemInfo entry in rootDir.EnumerateFileSystemInfos())
			this.Delete(entry);
		Console.WriteLine();
	}

	/****
	** Clone repos
	****/
	Helper.Print("Fetching Git repositories...");
	foreach (var entry in repoFolders.OrderBy(p => p.Key))
	{
		// collect info
		DirectoryInfo dir = new DirectoryInfo(Path.Combine(this.RootPath, entry.Key));
		ModRepository repo = entry.Value;
		Helper.Print($"   {dir.Name} → {repo.GitUrl}...");

		// validate
		if (dir.Exists)
		{
			Helper.Print($"   ERROR: directory already exists.", Severity.Error);
			continue;
		}

		// clone repo
		bool cloned = false;
		while (true)
		{
			try
			{
				await this.ExecuteShellAsync(
					filename: "git",
					arguments: $"clone -q {repo.GitUrl} \"{dir.Name}\"", // only get the latest version
					workingDir: rootDir.FullName,
					ignoreErrorOut: errorOut => Regex.IsMatch(errorOut, "^Filtering content:.+/s, done.$") // git LFS logs progress output to stderr
				);
				cloned = true;
				break;
			}
			catch (Exception ex)
			{
				ex.Dump();
				string choice = Helper.GetChoice("Cloning the Git repository failed! [r]etry [s]kip?", "r", "s");
				if (choice == "s")
					break;
			}
		}
		if (!cloned)
			continue;

		// write latest commit
		string lastCommit = await this.ExecuteShellAsync("git", "log -1", workingDir: dir.FullName);
		File.WriteAllText(
			Path.Combine(dir.FullName, "_metadata.txt"),
			$"url:\n   {repo.GitUrl}\n\n"
			+ $"mods:\n   {string.Join("\n   ", repo.Mods.Select(p => p.Name.FirstOrDefault()).OrderBy(p => p))}\n\n"
			+ $"latest commit:\n   {string.Join("\n   ", lastCommit.Replace("\r", "").Split('\n'))}"
		);

		// get patterns to ignore
		IEnumerable<Regex> ignorePatterns = this.IgnoreLegitNames.Concat(this.IgnoreIncorrectNames);
		{
			if (this.IgnoreFilesByRepo.TryGetValue(entry.Key, out Regex pattern))
				ignorePatterns = ignorePatterns.Concat(new[] { pattern });
		}

		// clean up
		var logDeletedEntries = this
			.RecursivelyDeleteMatches(dir, ignorePatterns.ToArray())
			.Where(deleted => !this.IgnoreLegitNames.Any(pattern => pattern.IsMatch(deleted.Name)))
			.ToArray();
		if (logDeletedEntries.Any())
			Helper.Print($"      deleted: {string.Join(", ", logDeletedEntries.Select(p => p.FullName.Substring(dir.FullName.Length + 1)))}.", Severity.Warning);
	}
	Console.WriteLine();

	Helper.Print("Done!");
}

/*********
** Private methods
*********/
/// <summary>Get a percentage string for display.</summary>
/// <param name="amount">The actual amount.</param>
/// <param name="total">The total possible amount.</param>
private string GetPercentage(int amount, int total)
{
	return $"{Math.Round(amount / (total * 1m) * 100)}%";
}

/// <summary>Execute an arbitrary shell command.</summary>
/// <param name="filename">The command filename to execute.</param>
/// <param name="arguments">The command arguments to execute.</summary>
/// <param name="workingDir">The working directory in which to execute the command.</param>
/// <param name="ignoreErrorOut">Whether to ignore a given error output.</param>
private async Task<string> ExecuteShellAsync(string filename, string arguments, string workingDir, Func<string, bool> ignoreErrorOut = null)
{
	string stdOut = null;
	string errorOut = null;
	try
	{
		var command = new ProcessStartInfo
		{
			FileName = filename,
			Arguments = arguments,
			WorkingDirectory = workingDir,
			CreateNoWindow = true,
			RedirectStandardError = true,
			RedirectStandardOutput = true,
			UseShellExecute = false
		};

		using (Process process = new Process { StartInfo = command })
		{
			process.Start();
			stdOut = await process.StandardOutput.ReadToEndAsync();
			errorOut = await process.StandardError.ReadToEndAsync();
			process.WaitForExit();

			if (!string.IsNullOrWhiteSpace(errorOut))
			{
				if (ignoreErrorOut == null || !ignoreErrorOut(errorOut))
					throw new Exception($"The shell returned an error message.\nCommand: {filename} {arguments}\nError: {errorOut}");
			}
			return stdOut;
		}
	}
	catch
	{
		new { command = $"{filename} {arguments}", workingDir, stdOut, errorOut }.Dump("shell error");
		throw;
	}
}

/// <summary>Recursively delete any files or folders which match one of the provided patterns.</summary>
/// <param name="entry">The file or root directory to check.</param>
/// <param name="namePatterns">The regex patterns matching file or directory names to delete.</param>
/// <returns>Returns the deleted files and directories.</returns>
private IEnumerable<FileSystemInfo> RecursivelyDeleteMatches(FileSystemInfo entry, Regex[] namePatterns)
{
	// delete if matched
	if (namePatterns.Any(p => p.IsMatch(entry.Name)))
	{
		this.Delete(entry);
		yield return entry;
	}

	// check subentries
	else if (entry is DirectoryInfo dir)
	{
		foreach (FileSystemInfo child in dir.GetFileSystemInfos())
		{
			foreach (FileSystemInfo deletedEntry in this.RecursivelyDeleteMatches(child, namePatterns))
				yield return deletedEntry;
		}
	}
}

/// <summary>Delete the given subfolder, handling permission issues along the way.</summary>
/// <param name="entry">The directory or file to delete.</param>
public void Delete(FileSystemInfo entry)
{
	if (!entry.Exists)
		return;

	while (true)
	{
		try
		{
			// delete subentries
			if (entry is DirectoryInfo dir)
			{
				foreach (FileSystemInfo child in dir.GetFileSystemInfos())
					this.Delete(child);
			}

			// delete current
			entry.Attributes = FileAttributes.Normal; // clear readonly flag
			entry.Delete();
			break;
		}
		catch (Exception ex)
		{
			ex.Dump();
			string choice = Helper.GetChoice($"Deleting {entry.FullName} failed! [r]etry [s]kip?", "r", "s");
			if (choice == "s")
				break;
		}
	}
}

/// <summary>Metadata about a mod repository.</summary>
class ModRepository
{
	/*********
	** Accessors
	*********/
	/// <summary>The repository's Git URL.</summary>
	public string GitUrl { get; }

	/// <summary>The mod's wiki metadata.</summary>
	public WikiModEntry[] Mods { get; }

	/// <summary>The mods' custom source URLs, if specified.</summary>
	public string[] CustomSourceUrls { get; }

	/// <summary>The repository owner name.</summary>
	public string RepositoryOwner { get; }

	/// <summary>The repository name.</summary>
	public string RepositoryName { get; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="gitUrl">The git URL.</param>
	/// <param name="mods">The mods in the repository.</param>
	public ModRepository(string gitUrl, IEnumerable<WikiModEntry> mods)
	{
		this.GitUrl = gitUrl;
		this.Mods = mods.ToArray();
		this.CustomSourceUrls = (from mod in mods where !string.IsNullOrWhiteSpace(mod.CustomSourceUrl) select mod.CustomSourceUrl).ToArray();

		if (this.TryParseRepositoryUrl(gitUrl, out string owner, out string name))
		{
			this.RepositoryOwner = owner;
			this.RepositoryName = name;
		}
	}

	/// <summary>Get the recommended folder name for the repository.</summary>
	public string GetRecommendedFolderName()
	{
		string name = this.Mods.Length == 1
			? this.Mods.Single().Name.FirstOrDefault()
			: $"~{this.RepositoryOwner}";

		foreach (char invalidCh in Path.GetInvalidFileNameChars())
			name = name.Replace(invalidCh, '_');

		return name;
	}


	/*********
	** Private methods
	*********/
	/// <summary>Get the Git URL for a mod entry, if any.</summary>
	/// <param name="mod">The mod's wiki metadata.</param>
	public static string GetGitUrl(WikiModEntry mod)
	{
		if (!string.IsNullOrWhiteSpace(mod.GitHubRepo))
			return $"https://github.com/{mod.GitHubRepo.Trim('/')}.git";

		if (mod.CustomSourceUrl != null)
		{
			if (mod.CustomSourceUrl.Contains("gitlab.com"))
				return $"{mod.CustomSourceUrl}.git";
		}

		return null;
	}

	/// <summary>Parse a Git URL.</param>
	/// <param name="url">The Git URL to parse.</summary>
	/// <param name="owner">The parsed repository owner.</param>
	/// <param name="name">The parsed repository name.</param>
	private bool TryParseRepositoryUrl(string url, out string owner, out string name)
	{
		if (!string.IsNullOrWhiteSpace(url))
		{
			var match = Regex.Match(url.Trim(), "^https://git(?:hub|lab).com/([^/]+)/([^/]+).git");
			if (match.Success)
			{
				owner = match.Groups[1].Value;
				name = match.Groups[2].Value;
				return true;
			}
		}

		// invalid
		owner = null;
		name = null;
		return false;
	}
}