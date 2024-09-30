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
/// <summary>The source URLs to skip when cloning repositories. This should match the GitHub repository name or custom URL specified on the wiki.</summary>
private readonly HashSet<string> IgnoreSourceUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
	// SMAPI
	"Pathoschild/SMAPI"
};

/// <summary>Maps source URLs to the folder name to use, overriding the generated name.</summary>
public IDictionary<string, string> OverrideFolderNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
	// simplify names to avoid `~author` format for main mod + sub-mods
	["https://github.com/6135/StardewValley.ProfitCalculator.git"] = "ProfitCalculator",
	["https://github.com/Alphablackwolf/SkillPrestige.git"] = "SkillPrestige",
	["https://github.com/ApryllForever/PolyamorySweetLove.git"] = "PolyamorySweet",
	["https://github.com/b-b-blueberry/BlueberryMushroomMachine.git"] = "MushroomPropagator",
	["https://github.com/b-b-blueberry/CustomCommunityCentre.git"] = "CustomCommunityCentre",
	["https://github.com/Dawilly/SAAT.git"] = "SAAT",
	["https://github.com/Floogen/Archery.git"] = "Archery",
	["https://github.com/Floogen/FishingTrawler.git"] = "FishingTrawler",
	["https://github.com/Floogen/GreenhouseGatherers.git"] = "GreenhouseGatherers",
	["https://github.com/Floogen/IslandGatherers.git"] = "IslandGatherers",
	["https://github.com/Floogen/SolidFoundations.git"] = "SolidFoundations",
	["https://github.com/jltaylor-us/StardewJsonProcessor.git"] = "Json Processor",
	["https://github.com/slothsoft/stardew-challenger.git"] = "Challenger"
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

		// list invalid custom source URLs
		{
			var validUrls = new HashSet<string>(repos.SelectMany(repo => repo.CustomSourceUrls), StringComparer.OrdinalIgnoreCase);
			string[] invalidUrls = mods
				.Select(mod => mod.CustomSourceUrl)
				.Where(url => !string.IsNullOrWhiteSpace(url) && !validUrls.Contains(url))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToArray();

			if (invalidUrls.Any())
			{
				ConsoleHelper.Print($"   Found {invalidUrls.Length} unsupported source URLs on the wiki:", Severity.Trace);
				foreach (string url in invalidUrls.OrderBy(p => p))
					ConsoleHelper.Print($"      {url}", Severity.Trace);
			}
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
	** Clone repos
	****/
	ConsoleHelper.Print("Printing new commands...");
	int reposLeft = repoFolders.Count;

	StringBuilder report = new();
	foreach ((string folderName, ModRepository repo) in repoFolders.OrderBy(p => p.Value.SourceUrl, StringComparer.OrdinalIgnoreCase))
	{
		// add command
		switch (repo.SourceType)
		{
			case SourceType.Git:
				report.Append($"   git clone -q {repo.SourceUrl} \"{folderName}\"");
				break;

			case SourceType.Mercurial:
				report.Append($"   hg clone {repo.SourceUrl} \"{folderName}\"");
				break;

			default:
				report.Append($"   # ⚠ invalid source type '{repo.SourceType}'.");
				break;
		}

		// add mod list
		report
			.Append(" # ")
			.Append(repo.Mods.Length)
			.Append(repo.Mods.Length == 1 ? " mod: " : " mods: ")
			.Append(string.Join(", ", repo.Mods.Select(p => p.Mod.Name[0]).Order(StringComparer.OrdinalIgnoreCase)))
			.AppendLine();
	}
	Console.WriteLine();

	report.Dump("instructions");
	ConsoleHelper.Print("Done!");
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
