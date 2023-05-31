<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.CoreInterfaces.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.dll</Reference>
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

See documentation at https://github.com/Pathoschild/StardewScripts.

*/
#load "Utilities\ConsoleHelper"

/*********
** Configuration
*********/
/// <summary>The absolute path to the folder which contains the source repositories.</summary>
private readonly string RootPath = @"E:\source\_Stardew\_smapi-mod-dump\source";

/// <summary>Patterns matching valid file or folder names that are legitimately part of the source repository, but should be removed from the cloned repositories.</summary>
private readonly Regex[] IgnoreLegitNames =
{
	// Git/Mercurial metadata
	new Regex(@"^\.git(?:attributes|ignore|modules)?$", RegexOptions.Compiled),
	new Regex(@"^\.hg(?:ignore|sub|substate|tags)?$", RegexOptions.Compiled),

	// large non-code files
	new Regex(@"\.gif$", RegexOptions.Compiled),
	new Regex(@"\.ogg$", RegexOptions.Compiled),
	new Regex(@"\.psd$", RegexOptions.Compiled),
	new Regex(@"\.wav$", RegexOptions.Compiled),
	new Regex(@"\.xcf$", RegexOptions.Compiled)
};

/// <summary>Patterns matching valid file or folder names that shouldn't be in source control.</summary>
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
	new Regex(@"\.userprefs$", RegexOptions.Compiled),
	new Regex(@"\.zip$", RegexOptions.Compiled),
	new Regex(@"_(?:BACKUP|BASE|LOCAL)_\d+\.[a-z]+", RegexOptions.Compiled) // merge backups
};

/// <summary>Patterns matching valid file or folder names that shouldn't be in source control for a specific repo folder.</summary>
private readonly IDictionary<string, Regex> IgnoreFilesByRepo = new Dictionary<string, Regex>
{
	["~JessebotX"] = new Regex(@"^oldversions$", RegexOptions.Compiled),
	["Battle Royalley - Year 2"] = new Regex("^Saves$", RegexOptions.Compiled),
	["Birthday Mail"] = new Regex(@"^Release$", RegexOptions.Compiled),
	["Chest Label System"] = new Regex(@"^zip\.exe$", RegexOptions.Compiled),
	["Faster Run"] = new Regex(@"^Release$", RegexOptions.Compiled),
	["HD Sprites"] = new Regex(@"^tools$", RegexOptions.Compiled), // dependencies, including exe over 10MB
	["Raised Garden Beds"] = new Regex("^(?:Media|Work)$", RegexOptions.Compiled),
	["Ridgeside Village"] = new Regex("^Versions$", RegexOptions.Compiled), // releases
	["StackSplitX"] = new Regex(@"^Demo\.gif$", RegexOptions.Compiled), // 10MB file
};

/// <summary>The source URLs to skip when cloning repositories. This should match the GitHub repository name or custom URL specified on the wiki.</summary>
private readonly HashSet<string> IgnoreSourceUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
	// SMAPI
	"Pathoschild/SMAPI"
};

/// <summary>Maps source URLs to the folder name to use, overriding the generated name.</summary>
public IDictionary<string, string> OverrideFolderNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
	// ~b-b-blueberry conflict
	["https://github.com/b-b-blueberry/BlueberryMushroomMachine.git"] = "MushroomPropagator",
	["https://github.com/b-b-blueberry/CustomCommunityCentre.git"] = "CustomCommunityCentre",

	// ~Floogen conflict
	["https://github.com/Floogen/GreenhouseGatherers.git"] = "GreenhouseGatherers",
	["https://github.com/Floogen/IslandGatherers.git"] = "IslandGatherers",

	// generic name
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
	** Fetch repo URLs
	****/
	List<ModRepository> repos = new List<ModRepository>();
	ConsoleHelper.Print("Fetching repository URLs...");
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
				.Select(mod => new ModWithSource(mod))
				.Where(p => p.HasSource)
				.GroupBy(p => p.SourceUrl, p => p, StringComparer.OrdinalIgnoreCase)
				.Select(group => new ModRepository(group))
		);

		// find invalid custom source URLs
		string[] invalidUrls;
		{
			var validUrls = new HashSet<string>(repos.SelectMany(repo => repo.CustomSourceUrls), StringComparer.OrdinalIgnoreCase);
			invalidUrls = mods
				.Select(mod => mod.CustomSourceUrl)
				.Where(url => !string.IsNullOrWhiteSpace(url) && !validUrls.Contains(url))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToArray();
		}

		// print stats
		int uniqueRepos = repos.Count;
		int haveCode = repos.SelectMany(repo => repo.Mods).Count();
		int haveSharedRepo = haveCode - uniqueRepos;

		ConsoleHelper.Print($"   {totalMods} mods in the SMAPI compatibility list.");
		ConsoleHelper.Print($"   {haveCode} mods ({this.GetPercentage(haveCode, totalMods)}) have a source repository.");
		ConsoleHelper.Print($"   {haveSharedRepo} repositories ({this.GetPercentage(haveSharedRepo, haveCode)}) contain multiple mods.");
		if (invalidUrls.Any())
		{
			ConsoleHelper.Print($"   Found {invalidUrls.Length} unsupported source URLs on the wiki:", Severity.Trace);
			foreach (string url in invalidUrls.OrderBy(p => p))
				ConsoleHelper.Print($"      {url}", Severity.Trace);
		}
	}
	Console.WriteLine();

	/****
	** Generate folder names
	****/
	IDictionary<string, ModRepository> repoFolders = new Dictionary<string, ModRepository>();
	foreach (ModRepository repo in repos)
	{
		if (!this.OverrideFolderNames.TryGetValue(repo.SourceUrl, out string folderName))
			folderName = repo.GetRecommendedFolderName();

		if (repoFolders.ContainsKey(folderName))
		{
			int discriminator = 2;
			string newName = folderName;
			do
			{
				newName = $"{folderName} ({discriminator})";
				discriminator++;
			}
			while (repoFolders.ContainsKey(newName));

			ConsoleHelper.Print($"Multiple repos would have name '{folderName}'; renamed one to '{newName}' to avoid a collision.", Severity.Warning);
			folderName = newName;
		}

		repoFolders[folderName] = repo;
	}

	/****
	** Clear old repos
	****/
	if (rootDir.EnumerateFileSystemInfos().Any())
	{
		ConsoleHelper.Print($"Deleting old source repositories...");
		foreach (FileSystemInfo entry in rootDir.EnumerateFileSystemInfos())
			this.Delete(entry);
		Console.WriteLine();
	}

	/****
	** Clone repos
	****/
	ConsoleHelper.Print("Fetching source repositories...");
	int reposLeft = repoFolders.Count;
	foreach (var entry in repoFolders.OrderBy(p => p.Key))
	{
		// collect info
		DirectoryInfo dir = new DirectoryInfo(Path.Combine(this.RootPath, entry.Key));
		ModRepository repo = entry.Value;
		ConsoleHelper.Print($"   [{reposLeft--}] {dir.Name} → {repo.SourceUrl}...");

		// validate
		if (dir.Exists)
		{
			ConsoleHelper.Print($"   ERROR: directory already exists.", Severity.Error);
			continue;
		}

		// clone repo
		bool cloned = false;
		string sourceUrl = repo.SourceUrl;
		while (true)
		{
			try
			{
				switch (repo.SourceType)
				{
					case SourceType.Git:
						await this.ExecuteShellAsync(
							filename: "git",
							arguments: $"clone -q {sourceUrl} \"{dir.Name}\" --depth 1 --no-tags", // only get the latest version
							workingDir: rootDir.FullName,
							ignoreErrorOut: errorOut => Regex.IsMatch(errorOut, "^Filtering content:.+/s, done.$") // git LFS logs progress output to stderr
						);
						cloned = true;
						break;

					case SourceType.Mercurial:
						await this.ExecuteShellAsync(
							filename: "hg",
							arguments: $"clone {sourceUrl} \"{dir.Name}\"",
							workingDir: rootDir.FullName
						);
						cloned = true;
						break;

					default:
						throw new NotSupportedException($"Invalid source type '{repo.SourceType}'.");
				}

				if (cloned)
					break;
			}
			catch (Exception ex)
			{
				ex.Dump();
				string choice = ConsoleHelper.GetChoice($"Cloning the {repo.SourceType} repository failed! [r]etry, [s]kip as failed, [c]ontinue as cloned, or change repo [U]RL?", "r", "s", "c", "u");
				if (choice == "c")
				{
					cloned = true;
					break;
				}
				if (choice == "s")
					break;
				if (choice == "u")
					sourceUrl = Util.ReadLine("Enter new source URL:", sourceUrl).Trim();
			}
		}
		if (!cloned)
			continue;

		// get last commit info
		string lastCommit = null;
		while (true)
		{
			try
			{
				bool succeeded = false;
				switch (repo.SourceType)
				{
					case SourceType.Git:
						lastCommit = await this.ExecuteShellAsync("git", "log -1", workingDir: dir.FullName);
						succeeded = true;
						break;

					case SourceType.Mercurial:
						lastCommit = await this.ExecuteShellAsync("hg", "log --limit 1", workingDir: dir.FullName);
						succeeded = true;
						break;

					default:
						throw new NotSupportedException($"Invalid source type '{repo.SourceType}'.");
				}

				if (succeeded)
					break;
			}
			catch (Exception ex)
			{
				ex.Dump();
				string choice = ConsoleHelper.GetChoice($"Reading the latest commit for the {repo.SourceType} repository failed! [r]etry or or [c]ontinue without commit info?", "r", "c");
				if (choice == "c")
					break;
			}
		}
		if (!cloned)
			continue;

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
			ConsoleHelper.Print($"      deleted: {string.Join(", ", logDeletedEntries.Select(p => p.FullName.Substring(dir.FullName.Length + 1)))}.", Severity.Warning);

		// add file headers to avoid confusion
		foreach (FileInfo file in dir.GetFiles("*", SearchOption.AllDirectories))
		{
			string header = this.GetFileHeader(file.Extension?.ToLower(), repo);
			if (header != null)
				File.WriteAllText(file.FullName, string.Concat(header, "\n\n", File.ReadAllText(file.FullName)));
		}

		// write metadata file
		File.WriteAllText(
			Path.Combine(dir.FullName, "_metadata.txt"),
			$"urls:\n   web: {repo.WebUrl}\n   {repo.SourceType}: {repo.SourceUrl}\n\n"
			+ $"mods:\n   {string.Join("\n   ", repo.Mods.Select(p => p.Mod.Name.FirstOrDefault()).OrderBy(p => p))}\n\n"
			+ $"latest commit:\n   {string.Join("\n   ", lastCommit.Replace("\r", "").Split('\n'))}"
		);
	}
	Console.WriteLine();

	ConsoleHelper.Print("Done!");
}

/*********
** Private methods
*********/
/// <summary>Get the file header to prepend to a file, if any.</summary>
/// <param name="extension">The file extension for which to get a header.</param>
/// <param name="repo">The source repository.</param>
private string GetFileHeader(string extension, ModRepository repo)
{
	switch (extension)
	{
		case ".cs":
		case ".js":
		case ".json":
		case ".txt":
		case ".xml":
			return string.Join("\n",
				$"/*************************************************",
				$"**",
				$"** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod",
				$"** for queries and analysis.",
				$"**",
				$"** This is *not* the original file, and not necessarily the latest version.",
				$"** Source repository: {repo.WebUrl}",
				$"**",
				$"*************************************************/"
			);

		case ".md":
			return string.Join("\n",
				$"**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod",
				$"for queries and analysis.**",
				$"",
				$"**This is _not_ the original file, and not necessarily the latest version.**  ",
				$"**Source repository: {repo.WebUrl}**",
				$"",
				$"----"
			);

		case ".yaml":
		case ".yml":
			return string.Join("\n",
				$"##################################################",
				$"##",
				$"## You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod",
				$"## for queries and analysis.",
				$"##",
				$"## This is *not* the original file, and not necessarily the latest version.",
				$"## Source repository: {repo.WebUrl}",
				$"##",
				$"##################################################"
			);

		default:
			return null;
	}
}

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
			string choice = ConsoleHelper.GetChoice($"Deleting {entry.FullName} failed! [r]etry [s]kip?", "r", "s");
			if (choice == "s")
				break;
		}
	}
}

/// <summary>A wiki mod entry with source info.</summary>
class ModWithSource
{
	/*********
	** Accessors
	*********/
	/// <summary>The wiki mod entry.</summary>
	public WikiModEntry Mod { get; }

	/// <summary>The repository's web URL.</summary>
	public string WebUrl { get; }

	/// <summary>The repository's Git or Mercurial URL.</summary>
	public string SourceUrl { get; }

	/// <summary>The name of the user who owns the repo, if available in the URL.</summary>
	public string OwnerName { get; }

	/// <summary>The name of the repository project.</summary>
	public string RepoName { get; }

	/// <summary>The source control system for this mod.</summary>
	public SourceType SourceType { get; }

	/// <summary>Whether the mod has a source repo URL.</summary>
	public bool HasSource => this.SourceUrl != null;


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="mod">The wiki mod entry.</param>
	public ModWithSource(WikiModEntry mod)
	{
		this.Mod = mod;

		if (ModRepository.TryGetSourceInfo(mod, out string webUrl, out string sourceUrl, out string ownerName, out string RepoName, out SourceType type))
		{
			this.WebUrl = webUrl;
			this.SourceUrl = sourceUrl;
			this.SourceType = type;
			this.OwnerName = ownerName;
			this.RepoName = RepoName;
		}
	}
}

/// <summary>Metadata about a mod repository.</summary>
class ModRepository
{
	/*********
	** Accessors
	*********/
	/// <summary>The repository's web URL.</summary>
	public string WebUrl { get; }

	/// <summary>The repository's Git or Mercurial URL.</summary>
	public string SourceUrl { get; }

	/// <summary>The name of the user who owns the repo, if available in the URL.</summary>
	public string OwnerName { get; }

	/// <summary>The name of the repository project.</summary>
	public string RepoName { get; }

	/// <summary></summary>
	public SourceType SourceType { get; }

	/// <summary>The mods in this repo.</summary>
	public ModWithSource[] Mods { get; }

	/// <summary>The mods' custom source URLs, if specified.</summary>
	public string[] CustomSourceUrls { get; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="mods">The mods in the repository.</param>
	public ModRepository(IEnumerable<ModWithSource> mods)
	{
		// save base values
		this.Mods = mods.ToArray();
		this.CustomSourceUrls = (from mod in mods where !string.IsNullOrWhiteSpace(mod.Mod.CustomSourceUrl) select mod.Mod.CustomSourceUrl).ToArray();
		if (!this.Mods.Any())
			throw new ArgumentException("Can't create a mod repository with zero mods.");

		// extract source details
		HashSet<string> webUrls = new(StringComparer.OrdinalIgnoreCase);
		HashSet<string> sourceUrls = new(StringComparer.OrdinalIgnoreCase);
		HashSet<SourceType> sourceTypes = new();
		HashSet<string> repoOwners = new(StringComparer.OrdinalIgnoreCase);
		HashSet<string> repoNames = new(StringComparer.OrdinalIgnoreCase);
		foreach (var mod in this.Mods)
		{
			webUrls.Add(mod.WebUrl);
			sourceUrls.Add(mod.SourceUrl);
			sourceTypes.Add(mod.SourceType);
			repoOwners.Add(mod.OwnerName);
			repoNames.Add(mod.RepoName);
		}

		// save single source details
		if (webUrls.Count > 1 || sourceUrls.Count > 1 || sourceTypes.Count > 1 || repoOwners.Count > 1 || repoNames.Count > 1)
			throw new ArgumentException($"Can't create a mod repository with inconsistent mod repo info. Found conflicting info for one or more fields:\n   web URLs: '{string.Join("', '", webUrls)}'\n   source URLs: '{string.Join("', '", sourceUrls)}'\n   repo types: '{string.Join("', '", sourceTypes)}'\n   repo owners: '{string.Join("', '", repoOwners)}'\n   repo names: '{string.Join("', '", repoNames)}'");
		this.WebUrl = webUrls.FirstOrDefault();
		this.SourceUrl = sourceUrls.FirstOrDefault();
		this.SourceType = sourceTypes.FirstOrDefault();
		this.OwnerName = repoOwners.FirstOrDefault();
		this.RepoName = repoNames.FirstOrDefault();
	}

	/// <summary>Get the recommended folder name for the repository.</summary>
	public string GetRecommendedFolderName()
	{
		string name = this.GetRawRecommendedFolderName();

		foreach (char invalidCh in Path.GetInvalidFileNameChars())
			name = name.Replace(invalidCh, '_');

		return name;
	}

	/// <summary>Extract the source control info for a mod entry, if valid.</summary>
	/// <param name="mod">The mod's wiki metadata.</param>
	/// <param name="webUrl">The repo's web URL.</param>
	/// <param name="sourceUrl">The repo's Git or Mercurial URL.</param>
	/// <param name="ownerName">The name of the user who owns the repo, if available in the URL.</param>
	/// <param name="repoName">The name of the repository project.</param>
	/// <param name="type">The repo's source control type.</param>
	public static bool TryGetSourceInfo(WikiModEntry mod, out string webUrl, out string sourceUrl, out string ownerName, out string repoName, out SourceType type)
	{
		// GitHub
		if (!string.IsNullOrWhiteSpace(mod.GitHubRepo))
		{
			webUrl = $"https://github.com/{mod.GitHubRepo.Trim('/')}";
			sourceUrl = $"{webUrl}.git";
			type = SourceType.Git;

			string[] parts = mod.GitHubRepo.Split('/', 2);
			if (parts.Length == 2)
			{
				ownerName = parts[0];
				repoName = parts[1];
			}
			else
			{
				// GitHub repo is invalid, but that will be validated elsewhere
				ownerName = null;
				repoName = parts[0];
			}
			return true;
		}

		// GitLab
		if (mod.CustomSourceUrl?.Contains("gitlab.com") == true)
		{
			webUrl = mod.CustomSourceUrl;
			sourceUrl = $"{mod.CustomSourceUrl}.git";
			type = SourceType.Git;

			var match = Regex.Match(mod.CustomSourceUrl, "gitlab.com/([^/]+)/([^/]+)");
			if (match.Success)
			{
				ownerName = match.Groups[1].Value;
				repoName = match.Groups[2].Value;
			}
			else
			{
				ownerName = null;
				repoName = null;
			}
			return true;
		}

		// SourceForge
		// web URL format: https://sourceforge.net/p/PROJECT_ID/
		// Mercurial URL format: http://hg.code.sf.net/p/PROJECT_ID/code
		if (mod.CustomSourceUrl?.Contains("sourceforge.net") == true)
		{
			var match = Regex.Match(mod.CustomSourceUrl, "sourceforge.net/p(?:rojects)?/([^/]+)", RegexOptions.IgnoreCase);
			if (match.Success)
			{
				ownerName = null;
				repoName = match.Groups[1].Value;

				webUrl = mod.CustomSourceUrl;
				sourceUrl = $"http://hg.code.sf.net/p/{repoName}/code";
				type = SourceType.Mercurial;
				return true;
			}
		}

		ownerName = null;
		repoName = null;
		webUrl = default;
		sourceUrl = default;
		type = default;
		return false;
	}


	/*********
	** Private methods
	*********/
	/// <summary>Get the raw recommended folder name for the repository, without sanitization.</summary>
	private string GetRawRecommendedFolderName()
	{
		if (this.Mods.Length == 1)
			return this.Mods.SingleOrDefault()?.Mod.Name.FirstOrDefault() ?? $"{this.OwnerName}_{this.RepoName}";

		return this.OwnerName != null
			? $"~{this.OwnerName}"
			: this.RepoName;
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

/// <summary>A source control type.</summary>
public enum SourceType
{
	/// <summary>The Git source control system.</summary>
	Git,

	/// <summary>The Mercurial source control system.</summary>
	Mercurial
}
