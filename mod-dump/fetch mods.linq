<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\Pathoschild.Http.Client.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.CoreInterfaces.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.dll</Reference>
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <NuGetReference>Pathoschild.FluentNexus</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Pathoschild.FluentNexus</Namespace>
  <Namespace>Pathoschild.FluentNexus.Models</Namespace>
  <Namespace>Pathoschild.Http.Client</Namespace>
  <Namespace>StardewModdingAPI</Namespace>
  <Namespace>StardewModdingAPI.Toolkit</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.CompatibilityRepo</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.CurseForgeExport</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.CurseForgeExport.ResponseModels</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.ModDropExport</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.ModDropExport.ResponseModels</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.NexusExport</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.NexusExport.ResponseModels</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModScanning</Namespace>
  <Namespace>System.Dynamic</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

/*

See documentation at https://github.com/Pathoschild/StardewScripts.

*/
#load "Utilities/ConsoleHelper.linq"
#load "Utilities/FileHelper.linq"
#load "Utilities/IncrementalProgressBar.linq"
#load "Utilities/ModCache.linq"
#load "Utilities/ModCacheUtilities.linq"

/*********
** Configuration
*********/
/// <summary>The user agent sent to the mod site APIs.</summary>
const string UserAgent = "PathoschildModDump/20240901 (+https://github.com/Pathoschild/StardewScripts)";

/// <summary>The mod site clients from which to fetch mods.</summary>
readonly IModSiteClient[] ModSites = new IModSiteClient[]
{
	new CurseForgeApiClient(
		exportApiUrl: null,
		userAgent: UserAgent
	),
	new ModDropApiClient(
		exportApiUrl: null,
		userAgent: UserAgent,
		username: null,
		password: null
	),
	new NexusApiClient(
		exportApiUrl: null,
		userAgent: UserAgent,
		apiKey: null,
		appName: "Pathoschild",
		appVersion: "1.0.0"
	)
};

/// <summary>If set, the full path to a local copy of the compatibility list repo to read directly instead of fetching it from the server.</summary>
const string LocalCompatListRepoPath = null;

/// <summary>The directory path in which to store cached mod data and downloads.</summary>
const string ModDumpPath = @"E:\dev\mod-dump";

/// <summary>The mods folder to which mods are copied when you click 'install mod'.</summary>
const string InstallModsToPath = @"C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods (test)";

/// <summary>Provides higher-level utilities for working with the underlying mod cache.</summary>
private readonly ModCacheUtilities ModCacheHelper = new(ModDumpPath, InstallModsToPath);

/// <summary>The path in which files are downloaded manually. This is only used when you need to download a file manually, and you click 'move download automatically'.</summary>
readonly string DownloadsPath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads");

/// <summary>Whether to fetch any updated mods from the remote mod sites. If false, the script will skip to analysis with the last cached mods.</summary>
readonly bool FetchMods = true;

/// <summary>Whether to delete mods which no longer exist on the mod sites. If false, they'll show warnings instead.</summary>
readonly bool DeleteRemovedMods = false; // NOTE: this can instantly delete many mods, which may take a long time to refetch. Consider only enabling it after you double-check the list it prints with it off.

/// <summary>The date from which to list updated mods.</summary>
readonly DateTimeOffset ListModsUpdatedSince = GetStartOfMonth().AddDays(-3);

/// <summary>The maximum age in hours for which a mod export is considered valid.</summary>
const int MaxExportAge = 5;

/// <summary>The number of mods to fetch from a mod site before the mod cache is written to disk to allow for incremental updates.</summary>
readonly int ModFetchesPerSave = 10;

/// <summary>The manual overrides for specific mods or source repos when analyzing them with this script.</summary>
/// <remarks>These are loaded from the adjacent <c>mod-overrides.jsonc</c> file.</remarks>
private ModOverridesData ModOverrides;

/// <summary>The <see cref="IgnoreForAnalysis"/> entries indexed by mod site/ID, like <c>"Nexus:2400"</c>.</summary>
private IDictionary<string, ModSearch[]> IgnoreForAnalysisBySiteId;


/*********
** Script
*********/
async Task Main()
{
	// dump CSS
	Util
		.RawHtml(
			"""
			<style>
				h1 { margin-top: 0.5em; }
			</style>
			"""
		)
		.Dump();

	// load mod data file
	this.ModOverrides = ModOverridesData.LoadFrom(
		Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "mod-overrides.jsonc")
	);

	// build optimized mod search lookup
	this.IgnoreForAnalysisBySiteId = this.ModOverrides.IgnoreForAnalysis
		.GroupBy(p => $"{p.Site}:{p.SiteId}")
		.ToDictionary(p => p.Key, p => p.ToArray());

	// fetch compatibility list
	Util.RawHtml("<h1>Init log</h1>").Dump();
	ConsoleHelper.Print("Fetching mod compatibility list...");
	ModCompatibilityEntry[] compatList = LocalCompatListRepoPath != null
		? await new ModToolkit().GetCompatibilityListFromLocalGitFolderAsync(LocalCompatListRepoPath)
		: await new ModToolkit().GetCompatibilityListAsync();

	// read cache
	ConsoleHelper.Print($"Reading mod cache...");
	ModCache modDump = this.ModCacheHelper.Cache;

	// sync cache to mod sites
	if (this.FetchMods)
	{
		ConsoleHelper.Print("Initializing clients...");
		foreach (var site in this.ModSites)
			await site.AuthenticateAsync();

		ConsoleHelper.Print("Syncing cache with mod sites...");

		foreach (IModSiteClient modSite in this.ModSites)
			await this.DownloadAndCacheSiteAsync(modDump, modSite);

		this.ModCacheHelper.ReloadFromModCache();
	}

	// read mod data
	ConsoleHelper.Print($"Reading mod folders...");
	ParsedMod[] mods = modDump.ReadUnpackedModFolders().ToArray();

	// add launch button
	Util.VerticalRun(
		new Hyperlinq(
			() => Process.Start(
				fileName: Path.Combine(InstallModsToPath, "..", "StardewModdingAPI.exe"),
				arguments: @$"--mods-path ""{Path.GetFileName(InstallModsToPath)}"""
			),
			"launch SMAPI"
		),
		Util.OnDemand(
			"install compatible mods on compatibility list",
			() => new object[] // returning an array allows collapsing the log in the LINQPad output
			{
				Util.WithStyle(
					Util.VerticalRun(this.InstallEveryCompatibleCSharpMod(compatList)),
					"font-style: monospace; font-size: 0.9em;"
				)
			}
		)
	).Dump("actions");

	// detect issues
	ConsoleHelper.Print($"Running analyses...");
	{
		Util.RawHtml("<h1>Detected issues</h1>").Dump();

		// compatibility list issues
		Util.RawHtml("<h3>Compatibility list issues</h3>").Dump();
		{
			var notOnCompatList = this.GetModsNotOnCompatibilityList(mods, compatList).ToArray();
			if (notOnCompatList.Length > 0)
			{
				notOnCompatList.Dump("SMAPI mods not on the compatibility list");
				new Lazy<dynamic>(() => Util.WithStyle(string.Join("\n", notOnCompatList.Select(p => ((Lazy<string>)p.CompatEntry).Value)), "font-family: monospace;")).Dump("SMAPI mods not on the compatibility list (JSON format)");
			}
			else
				"none".Dump("SMAPI mods not on the compatibility list");
		}
		this.GetCompatibilityListModsNotInCache(modDump, compatList).Dump("Mods on the compatibility list which weren't found on the modding sites");

		// mod issues
		Util.RawHtml("<h3>Mod issues</h3>").Dump();
		this.GetInvalidMods(mods).Dump("Mods marked invalid by SMAPI toolkit (except blacklist)");

		// script issues
		Util.RawHtml("<h3>Script issues</h3>").Dump();
		this.GetInvalidIgnoreModEntries(mods).Dump($"{nameof(ModOverridesData.IgnoreForAnalysis)} values which don't match any local mod");
	}

	// mod updates
	{
		Util.RawHtml("<h1>Mod updates</h1>").Dump();
		this.GetModsOnCompatibilityListUpdatedSince(mods, compatList, ListModsUpdatedSince).Dump($"Mod files on compatibility list uploaded since {ListModsUpdatedSince:yyyy-MM-dd HH:mm}");
	}

	// stats
	{
		Util.RawHtml("<h1>Stats</h1>").Dump();
		this.GetOpenSourceStats(compatList).Dump("open-source stats");
		this.GetModTypes(mods).Dump("mod types");
		DumpDictionaryToColumns(this.GetContentPatcherVersionUsage(mods).Dump("Content Patcher packs by format version"), "Content Patcher packs by format version (row)");
	}
}


/*********
** Common queries
*********/
/// <summary>Get SMAPI mods which aren't listed on the mod compatibility list.</summary>
/// <param name="mods">The mods to check.</param>
/// <param name="compatList">The mod data from the mod compatibility list.</param>
IEnumerable<dynamic> GetModsNotOnCompatibilityList(IEnumerable<ParsedMod> mods, ModCompatibilityEntry[] compatList)
{
	// fetch mods on the compatibility list
	ISet<string> manifestIDs = new HashSet<string>(compatList.SelectMany(p => p.ID), StringComparer.InvariantCultureIgnoreCase);
	IDictionary<ModSite, ISet<long>> siteIDs = new Dictionary<ModSite, ISet<long>>
	{
		[ModSite.CurseForge] = new HashSet<long>(compatList.Where(p => p.CurseForgeID.HasValue).Select(p => (long)p.CurseForgeID.Value)),
		[ModSite.ModDrop] = new HashSet<long>(compatList.Where(p => p.ModDropID.HasValue).Select(p => (long)p.ModDropID.Value)),
		[ModSite.Nexus] = new HashSet<long>(compatList.Where(p => p.NexusID.HasValue).Select(p => (long)p.NexusID.Value))
	};

	// fetch report
	return (
		from mod in mods
		from folder in mod.ModFolders
		orderby mod.Name

		where
			folder.ModType == ModType.Smapi
			&& !string.IsNullOrWhiteSpace(folder.ModID)
			&& !this.ShouldIgnoreForAnalysis(mod.Site, mod.ID, folder.ID, folder.ModID)

		let compatHasManifestId = manifestIDs.Contains(folder.ModID)
		let compatHasSiteId = siteIDs[mod.Site].Contains(mod.ID)

		where (!compatHasManifestId || !compatHasSiteId)

		let manifest = folder.RawFolder.Manifest
		let names = this.GetModNames(folder, mod)
		let authorNames = this.GetAuthorNames(manifest, mod)
		let githubRepo = this.GetGitHubRepo(manifest, mod)
		let customSourceUrl = githubRepo == null
			? this.GetCustomSourceUrl(mod)
			: null

		let isModInstalled = Directory.Exists(Path.Combine(InstallModsToPath, folder.RawFolder.Directory.Name))

		let missingLabels = (new[] { !compatHasManifestId ? "manifest ID" : null, !compatHasSiteId ? "site ID" : null }).Where(p => p is not null).ToArray()

		select new
		{
			SitePage = new Hyperlinq(mod.PageUrl, $"{mod.Site}:{mod.ID}"),
			SiteName = mod.Name,
			SiteAuthor = mod.AuthorLabel != null && mod.AuthorLabel != mod.Author
				? $"{mod.Author}\n({mod.AuthorLabel})"
				: mod.Author,
			SiteVersion = SemanticVersion.TryParse(mod.Version, out ISemanticVersion siteVersion) ? siteVersion.ToString() : mod.Version,
			FileName = folder.DisplayName,
			FileCategory = folder.Type,
			folder.ModID,
			folder.ModVersion,
			Missing = Util.WithStyle(
				string.Join(", ", missingLabels),
				missingLabels.Length == 1 ? "color: red" : "" // highlight mods that are partly missing, which usually means outdated info
			),
			Actions = isModInstalled
				? "installed"
				: (object)Util.OnDemand(
					"install mod",
					() => new object[] // returning an array allows collapsing the log in the LINQPad output
					{
						Util.WithStyle(
							Util.VerticalRun(this.ModCacheHelper.TryInstall(folder, deleteTargetFolder: false)),
							"font-style: monospace; font-size: 0.9em;"
						)
					}
				),
			Metadata = Util.OnDemand("expand", () => new
			{
				FileId = folder.ID,
				FileType = folder.ModType,
				UpdateKeys = Util.OnDemand("expand", () => manifest.UpdateKeys),
				Manifest = Util.OnDemand("expand", () => manifest),
				Mod = Util.OnDemand("expand", () => mod),
				Folder = Util.OnDemand("expand", () => folder)
			}),
			CompatEntry = new Lazy<string>(() => // can't be in Metadata since it's accessed by the main script
				BuildCompatibilityEntry(mod, manifest, names, authorNames, githubRepo, customSourceUrl)
			)
		}
	)
	.ToArray();
	
	static string BuildCompatibilityEntry(ParsedMod mod, IManifest manifest, string[] names, string[] authorNames, string githubRepo, string customSourceUrl)
	{
		// build JSON
		string json = JsonConvert.SerializeObject(
			new
			{
				name = string.Join(", ", names),
				author = string.Join(", ", authorNames),
				id = manifest?.UniqueID,
				curse = mod.Site == ModSite.CurseForge ? mod.ID : null as long?,
				moddrop = mod.Site == ModSite.ModDrop ? mod.ID : null as long?,
				nexus = mod.Site == ModSite.Nexus ? mod.ID : null as long?,
				github = githubRepo,
				source = customSourceUrl
			},
			Newtonsoft.Json.Formatting.Indented
		);

		// remove empty optional fields
		json = Regex.Replace(json, @"^\s*""(?:curse|moddrop|source)"": null,?" + Environment.NewLine, "", RegexOptions.Multiline);
		json = Regex.Replace(json, $",({Environment.NewLine}}})", "$1");

		return json + ",";
	}
}

/// <summary>Get SMAPI mods on the compatibility list which have been updated recently.</summary>
/// <param name="mods">The mods to check.</param>
/// <param name="compatList">The mod data from the compatibility list.</param>
/// <param name="updatedSince">The earliest update date for which to list mods.</param>
IEnumerable<dynamic> GetModsOnCompatibilityListUpdatedSince(IEnumerable<ParsedMod> mods, ModCompatibilityEntry[] compatList, DateTimeOffset updatedSince)
{
	// get mod IDs on the compatibility list
	var manifestIDs = new HashSet<string>(compatList.SelectMany(p => p.ID), StringComparer.InvariantCultureIgnoreCase);

	// build compatibility list lookup
	var compatEntries = new Dictionary<string, ModCompatibilityEntry>();
	foreach (var entry in compatList)
	{
		if (entry.CurseForgeID.HasValue)
			compatEntries[$"{ModSite.CurseForge}:{entry.CurseForgeID}"] = entry;
		if (entry.ModDropID.HasValue)
			compatEntries[$"{ModSite.ModDrop}:{entry.ModDropID}"] = entry;
		if (entry.NexusID.HasValue)
			compatEntries[$"{ModSite.Nexus}:{entry.NexusID}"] = entry;
	}

	// fetch report
	const string smallStyle = "font-size: 0.8em;";
	return (
		from mod in mods
		from folder in mod.ModFolders

		let compatEntry = compatEntries.GetValueOrDefault($"{mod.Site}:{mod.ID}")
		let compat = compatEntry?.Compatibility

		where
			compatEntry != null
			&& folder.Uploaded >= updatedSince
			&& !this.ShouldIgnoreForAnalysis(mod.Site, mod.ID, folder.ID, folder.ModID)

		let uploadedStr = folder.Uploaded.ToString("yyyy-MM-dd")
		let manifest = folder.RawFolder.Manifest
		let names = this.GetModNames(folder, mod)
		let authorNames = this.GetAuthorNames(manifest, mod)
		let githubRepo = this.GetGitHubRepo(manifest, mod)
		let customSourceUrl = githubRepo == null
			? this.GetCustomSourceUrl(mod)
			: null
		let isModInstalled = Directory.Exists(Path.Combine(InstallModsToPath, folder.RawFolder.Directory.Name))

		let highlightType = folder.ModType is not (ModType.Smapi or ModType.ContentPack)
		let highlightStatus = compat is null || compat.Status is not (ModCompatibilityStatus.Ok or ModCompatibilityStatus.Optional)

		orderby
			(highlightType || highlightStatus) descending, // mods with issues first
			uploadedStr descending, // then newest first
			mod.Name

		select new
		{
			Link = new Hyperlinq(mod.PageUrl, $"{mod.Site}:{mod.ID}"),
			Mod =
				$"{mod.Name}\n   by "
				+ (mod.AuthorLabel != null && mod.AuthorLabel != mod.Author
					? $"{mod.Author} ({mod.AuthorLabel})"
					: mod.Author
				),
			//SiteVersion = SemanticVersion.TryParse(mod.Version, out ISemanticVersion siteVersion) ? siteVersion.ToString() : mod.Version,
			FileUpdated = uploadedStr,
			File = $"{folder.DisplayName} {folder.Version}",
			FileCategory = folder.Type,
			ModType = Util.WithStyle(folder.ModType, highlightType ? ConsoleHelper.ErrorStyle : ""),
			Summary =
			compatEntry != null
				? Util.WithStyle($"{compat.Summary} {(!string.IsNullOrWhiteSpace(compat.BrokeIn) ? $"[broke in {compat.BrokeIn}]" : "")}".Trim(), $"{smallStyle} {(highlightStatus ? ConsoleHelper.ErrorStyle : "")}")
				: Util.WithStyle($"not found on compatibility list", ConsoleHelper.ErrorStyle),
			folder.ModID,
			folder.ModVersion,
			Actions = Util.HorizontalRun(true,
				isModInstalled
					? "installed"
					: (object)Util.OnDemand(
						"install",
						() => new object[] // returning an array allows collapsing the log in the LINQPad output
						{
							Util.WithStyle(
								Util.VerticalRun(this.ModCacheHelper.TryInstall(folder, deleteTargetFolder: false)),
								"font-style: monospace; font-size: 0.9em;"
							)
						}
					),
				new Hyperlinq(folder.RawFolder.DirectoryPath, "files")
			),
			Metadata = Util.OnDemand("expand", () => new
			{
				FileId = folder.ID,
				UpdateKeys = Util.OnDemand("expand", () => manifest.UpdateKeys),
				Manifest = Util.OnDemand("expand", () => manifest),
				Mod = Util.OnDemand("expand", () => mod),
				Folder = Util.OnDemand("expand", () => folder)
			})
		}
	)
	.ToArray();
}

/// <summary>Install every mod from the C# compatibility list that's marked compatible.</summary>
/// <param name="mods">The mods to install.</param>
IEnumerable<object> InstallEveryCompatibleCSharpMod(ModCompatibilityEntry[] mods)
{
	if (Directory.GetFileSystemEntries(InstallModsToPath).Any())
	{
		yield return Util.WithStyle($"Can't install all mods to folder '{InstallModsToPath}' because that folder isn't empty.", ConsoleHelper.ErrorStyle);
		yield break;
	}


	foreach (ModCompatibilityEntry mod in mods)
	{
		if (mod.Compatibility.Status is not (ModCompatibilityStatus.Ok or ModCompatibilityStatus.Optional or ModCompatibilityStatus.Unofficial))
			continue;

		string modId = mod.ID.FirstOrDefault();
		if (modId is null)
			continue;

		bool installed = this.ModCacheHelper.TryInstall(modId, out List<object> log);

		var _mod = mod;
		yield return Util.HorizontalRun(true,
			installed
				? Util.WithStyle($"Installed {mod.Name.FirstOrDefault()}.", ConsoleHelper.SuccessStyle)
				: Util.WithStyle($"Error installing {mod.Name.FirstOrDefault()}. Compatibility status: {mod.Compatibility.Summary}{(!string.IsNullOrWhiteSpace(mod.Compatibility.BrokeIn) ? $"[broke in {mod.Compatibility.BrokeIn}]" : "")}.", ConsoleHelper.ErrorStyle),
			new Lazy<object>(() => new { log, _mod })
		);
	}
}

/// <summary>Get SMAPI mods listed on the mod compatibility list which don't exist in the mod dump, so they were probably hidden or deleted. This excludes mods marked abandoned on the compatibility list.</summary>
/// <param name="modDump">The mod dump to search.</param>
/// <param name="compatList">The mod data from the mod compatibility list.</param>
IEnumerable<dynamic> GetCompatibilityListModsNotInCache(ModCache modDump, ModCompatibilityEntry[] mods)
{
	ModToolkit toolkit = new();

	HashSet<string> missingPages = new(StringComparer.OrdinalIgnoreCase);
	foreach (ModCompatibilityEntry mod in mods)
	{
		if (mod.Compatibility.Status is ModCompatibilityStatus.Abandoned or ModCompatibilityStatus.Obsolete)
			continue;

		missingPages.Clear();
		if (mod.CurseForgeID.HasValue && modDump.GetMod(ModSite.CurseForge, mod.CurseForgeID.Value) is null)
			missingPages.Add($"{ModSite.CurseForge}:{mod.CurseForgeID}");
		if (mod.ModDropID.HasValue && modDump.GetMod(ModSite.ModDrop, mod.ModDropID.Value) is null)
			missingPages.Add($"{ModSite.ModDrop}:{mod.ModDropID}");
		if (mod.NexusID.HasValue && modDump.GetMod(ModSite.Nexus, mod.NexusID.Value) is null)
			missingPages.Add($"{ModSite.Nexus}:{mod.NexusID}");

		if (missingPages.Count > 0)
		{
			yield return new
			{
				Name = mod.Name.FirstOrDefault(),
				ID = mod.ID.FirstOrDefault(),
				InvalidPages = Util.HorizontalRun(
					true,
					missingPages.Select(page =>
					{
						string url = toolkit.GetUpdateUrl(page);
						return url != null
							? (object)new Hyperlinq(url, page)
							: page;
					})
				)
			};
		}
	}
}

/// <summary>Get mods which the SMAPI toolkit marked as invalid or unparseable.</summary>
/// <param name="mods">The mods to check.</param>
IEnumerable<dynamic> GetInvalidMods(IEnumerable<ParsedMod> mods)
{
	return (
		from mod in mods

		let invalid = mod.ModFolders
			.Where(folder =>
				folder.ModType == ModType.Invalid
				&& folder.ModError != ModParseError.ManifestMissing // ignore non-mod files
				&& folder.ModError != ModParseError.EmptyFolder // contains only non-mod files (e.g. replacement PNG assets)
				&& !this.ShouldIgnoreForAnalysis(mod.Site, mod.ID, folder.ID, folder.ModID)
			)
			.ToArray()

		where invalid.Any()
		select new
		{
			mod.Name,
			mod.Author,
			mod.Version,
			mod.Updated,
			SitePage = new Hyperlinq(mod.PageUrl, $"{mod.Site}:{mod.ID}"),
			Data = new Lazy<object>(() => mod),
			InvalidFile = invalid.Select(parsedFile => new
			{
				parsedFile.ID,
				parsedFile.Type,
				parsedFile.DisplayName,
				parsedFile.Version,
				parsedFile.ModType,
				parsedFile.ModError,
				Data = new Lazy<object>(() => parsedFile),
				Manifest = new Lazy<string>(() =>
				{
					FileInfo file = new FileInfo(Path.Combine(parsedFile.RawFolder.Directory.FullName, "manifest.json"));
					return file.Exists
						? File.ReadAllText(file.FullName)
						: "<file not found>";
				}),
				ManifestError = new Lazy<string>(() => $"{parsedFile.RawFolder.ManifestParseError}\n{parsedFile.RawFolder.ManifestParseErrorText}"),
				FileList = new Lazy<string>(() => this.BuildFileList(parsedFile.RawFolder.Directory))
			})
		}
	)
	.ToArray();
}

/// <summary>Get entries in <see cref="IgnoreModsForValidation" /> which don't match any of the given mods.</summary>
/// <param name="mods">The mods to check.</param>
IEnumerable<dynamic> GetInvalidIgnoreModEntries(IEnumerable<ParsedMod> mods)
{
	// index known mods
	IDictionary<string, ParsedMod> modsByKey = mods.ToDictionary(mod => $"{mod.Site}:{mod.ID}", StringComparer.OrdinalIgnoreCase);

	// show unknown entries
	var invalid = new List<(ModSearch Entry, string Reason, ParsedMod Mod)>();
	foreach (var pair in this.IgnoreForAnalysisBySiteId)
	{
		(string key, ModSearch[] entries) = pair;

		// fetch mod
		if (!modsByKey.TryGetValue(key, out ParsedMod mod))
		{
			foreach (var entry in entries)
				invalid.Add((entry, "Site ID not found", mod));
			continue;
		}

		// match against mod folders
		HashSet<long> fileIds = new(mod.Files.Select(p => p.ID));
		foreach (var entry in entries)
		{
			if (entry.FileId.HasValue && !fileIds.Contains(entry.FileId.Value))
				invalid.Add((entry, "File ID not found", mod));
			else if (!mod.ModFolders.Any(folder => entry.Matches(site: mod.Site, siteId: mod.ID, fileId: folder.ID, manifestId: folder.ModID)))
				invalid.Add((entry, "Mod folder data not matched", mod));
		}
	}

	return invalid
		.Select(p => new { p.Entry.Site, p.Entry.SiteId, p.Entry.FileId, p.Entry.ManifestId, Reason = p.Reason, Mod = new Lazy<ParsedMod>(() => p.Mod), Entry = new Lazy<ModSearch>(() => p.Entry) })
		.OrderBy(p => p.Site)
		.ThenBy(p => p.SiteId)
		.ThenBy(p => p.FileId);
}

/// <summary>Get stats about open-source C# mods on the mod compatibility list.</summary>
/// <param name="compatList">The mod data from the mod compatibility list.</param>
string[] GetOpenSourceStats(ModCompatibilityEntry[] compatList)
{
	// get C# mod count by repo
	Dictionary<string, int> modsByRepo = new(StringComparer.OrdinalIgnoreCase);
	int totalMods = 0;
	foreach (ModCompatibilityEntry mod in compatList)
	{
		if (mod.ContentPackFor != null)
			continue;

		string repo = null;
		if (!string.IsNullOrWhiteSpace(mod.CustomSourceUrl))
			repo = mod.CustomSourceUrl.Trim();
		else if (!string.IsNullOrWhiteSpace(mod.GitHubRepo))
			repo = mod.GitHubRepo.Trim();

		totalMods++;
		if (repo != null)
			modsByRepo[repo] = modsByRepo.GetValueOrDefault(repo) + 1;
	}

	// get stats
	int modsWithCode = 0;
	int modsWithSharedRepo = 0;
	foreach (int count in modsByRepo.Values)
	{
		modsWithCode += count;
		if (count > 1)
			modsWithSharedRepo += count;
	}

	// return stats
	return [
		$"- We have {totalMods:#,###} tracked C# mods, of which",
		$"- {modsWithCode:#,###} mods ({GetPercentage(modsWithCode, totalMods)}) have a source code repo, with",
		$"- {modsWithSharedRepo:#,###} ({GetPercentage(modsWithSharedRepo, modsWithCode)}) in a multi-mod repo and {modsWithCode - modsWithSharedRepo:#,###} ({GetPercentage(modsWithCode - modsWithSharedRepo, modsWithCode)}) in a single-mod repo."
	];

	static string GetPercentage(int amount, int total)
	{
		return $"{Math.Round(amount / (total * 1m) * 100)}%";
	}
}

/// <summary>Get the number of mods by type.</summary>
/// <param name="mods">The mods to check.</param>
IDictionary<string, int> GetModTypes(IEnumerable<ParsedMod> mods)
{
	const int minPerGroup = 100;

	// get mod id => name lookup
	IDictionary<string, string> namesById = mods
		.SelectMany(p => p.ModFolders)
		.Select(p => new { Id = p.ModID?.Trim(), Name = p.ModDisplayName })
		.Where(p => !string.IsNullOrWhiteSpace(p.Id) && !string.IsNullOrWhiteSpace(p.Name))
		.GroupBy(p => p.Id, StringComparer.OrdinalIgnoreCase)
		.ToDictionary(p => p.Key, p => p.First().Name, StringComparer.OrdinalIgnoreCase);
	if (namesById.ContainsKey("Paritee.BetterFarmAnimalVariety"))
		namesById["Paritee.BetterFarmAnimalVariety"] = "Better Farm Animal Variety"; // match format used in stats without the "Paritee's" prefix

	// get type priority for ID conflicts
	static int GetPriority(string type)
	{
		return type switch
		{
			"SMAPI" => 4,
			"content pack (Content Patcher)" => 3,
			_ when (type?.StartsWith("content pack") == true) => 2,
			"XNB" => 1,
			_ => -1
		};
	}

	// get count by type key
	var typesByKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	foreach (ParsedMod mod in mods)
	{
		// get all the mods available to download from this page (including downloads which contain multiple submods)
		var submods =
			(
				from folder in mod.ModFolders
				where folder.ModType is not (ModType.Ignored or ModType.Invalid)

				let manifest = folder.RawFolder.Manifest
				let contentPackFor = manifest?.ContentPackFor?.UniqueID
				let type = folder.ModType switch
				{
					ModType.Smapi => "SMAPI",
					ModType.ContentPack => !string.IsNullOrWhiteSpace(contentPackFor) && namesById.TryGetValue(contentPackFor.Trim(), out string name)
						? $"content pack ({name})"
						: $"content pack ({contentPackFor?.Trim()})",
					ModType.Xnb => "XNB",
					_ => $"other ({folder.ModType})"
				}

				orderby GetPriority(type)
				select (folder, manifest, type)
			);
		if (!submods.Any())
			continue;

		// count submods by type
		bool hasNonXnb = submods.Any(p => p.type != "XNB");
		foreach ((ParsedFile folder, IManifest manifest, string type) in submods)
		{
			// special case: if the mod has both XNB and non-XNB components, ignore the XNB ones (they're generally old/alternative versions)
			if (type == "XNB" && hasNonXnb)
				continue;

			// get tracking key
			string key = !string.IsNullOrWhiteSpace(manifest?.UniqueID)
				? manifest.UniqueID.Trim()
				: $"{type}:{mod.Site}:{mod.ID}";

			// ignore duplicates by ID
			// (Each player can only install one mod with a given ID. If two mods have the same ID, we assume they're equivalent and count them once in priority order.)
			if (typesByKey.TryGetValue(key, out string prevType) && GetPriority(type) <= GetPriority(prevType))
				continue;

			// set type
			typesByKey[key] = type;
		}
	}

	// get counts
	var counts = typesByKey
		.GroupBy(p => p.Value, StringComparer.OrdinalIgnoreCase)
		.OrderByDescending(p => p.Key == "SMAPI")
		.ThenByDescending(p => p.Key == "content pack (Content Patcher)")
		.ThenByDescending(p => p.Key == "XNB")
		.ThenBy(p => p.Key)
		.ToDictionary(p => p.Key, p => p.Count(), StringComparer.OrdinalIgnoreCase);

	// merge content packs with < min usages
	{
		int mergedSum = 0;

		foreach (var pair in counts.Where(p => p.Value < minPerGroup).ToArray())
		{
			if (pair.Key.StartsWith("content pack ("))
			{
				mergedSum += pair.Value;
				counts.Remove(pair.Key);
			}
		}

		if (mergedSum > 0)
			counts[$"content pack (<{minPerGroup} usages)"] = mergedSum;
	}

	return counts;
}

/// <summary>Get the number of unique content packs by Content Patcher version.</summary>
/// <param name="mods">The mods to check.</param>
IDictionary<string, int> GetContentPatcherVersionUsage(IEnumerable<ParsedMod> mods)
{
	// get unique versions by content pack ID
	var modVersions = new Dictionary<string, ISemanticVersion>(StringComparer.OrdinalIgnoreCase);
	foreach (ParsedMod mod in mods)
	{
		foreach (ParsedFile folder in mod.ModFolders)
		{
			// parse manifest
			IManifest manifest = folder.RawFolder.Manifest;
			string id = manifest?.UniqueID?.Trim();
			string contentPackFor = manifest?.ContentPackFor?.UniqueID?.Trim();
			if (string.IsNullOrWhiteSpace(id) || !string.Equals(contentPackFor, "Pathoschild.ContentPatcher", StringComparison.OrdinalIgnoreCase))
				continue;

			// skip if content.json doesn't exist
			FileInfo contentFile = new FileInfo(Path.Combine(folder.RawFolder.Directory.FullName, "content.json"));
			if (!contentFile.Exists)
				continue;

			// extract format version
			ISemanticVersion format = null;
			try
			{
				var template = new { Format = "" };
				var rawContent = JsonConvert.DeserializeAnonymousType(File.ReadAllText(contentFile.FullName), template);
				if (!SemanticVersion.TryParse(rawContent?.Format, out format))
					continue;

				format = new SemanticVersion(format.MajorVersion, format.MinorVersion, 0);
			}
			catch (JsonException)
			{
				continue; // ignore invalid content.json
			}

			// track latest version
			if (!modVersions.TryGetValue(id, out ISemanticVersion prevVersion) || format.IsNewerThan(prevVersion))
				modVersions[id] = format;
		}
	}

	// get counts
	var counts = modVersions
		.OrderBy(p => p.Value.MajorVersion)
		.ThenBy(p => p.Value.MinorVersion)
		.ThenBy(p => p.Value.PatchVersion)
		.GroupBy(p => p.Value.ToString())
		.ToDictionary(p => p.Key.ToString(), p => p.Count());

	// ignore invalid values
	counts.Remove("11.1.0");
	counts.Remove("3.0.0");

	return counts;
}

/// <summary>Get all mods which depend on the given mod.</summary>
/// <param name="parsedMods">The mods to check.</param>
/// <param name="modID">The dependency mod ID.</param>
IEnumerable<ModFolder> GetModsDependentOn(IEnumerable<ParsedMod> parsedMods, string modID)
{
	foreach (ParsedMod mod in parsedMods)
	{
		foreach (ModFolder folder in mod.ModFolders.Select(p => p.RawFolder))
		{
			bool dependency =
				folder.Manifest?.Dependencies?.Any(p => p.UniqueID?.Equals(modID, StringComparison.InvariantCultureIgnoreCase) == true) == true
				|| folder.Manifest?.ContentPackFor?.UniqueID?.Equals(modID, StringComparison.InvariantCultureIgnoreCase) == true;
			if (dependency)
				yield return folder;
		}
	}
}


/*********
** Implementation
*********/
/// <summary>Dump a dictionary to the console with each key formatted as a table column.</summary>
/// <param name="dict">The dictionary data to dump.</param>
/// <param name="label">The dump label, if any.</param>
private void DumpDictionaryToColumns<TKey, TValue>(IDictionary<TKey, TValue> dict, string label = null)
{
	var result = new ExpandoObject();
	foreach (var pair in dict)
		result.TryAdd(pair.Key.ToString(), pair.Value);

	new[] { result }.Dump(label);
}

/// <summary>Get the start of the preceding month.</summary>
/// <param name="dayOffset">The day offset to apply to the date.</param>
private static DateTimeOffset GetStartOfMonth(int fuzzyDays = 5)
{
	DateTimeOffset now = DateTimeOffset.Now;

	return new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, 0, now.Offset)
		.AddMonths(now.Day <= fuzzyDays ? -1 : 0);
}

/// <summary>Fetch and cache all mods and mod files from a mod site.</summary>
/// <param name="modDump">The mod dump to update.</param>
/// <param name="modSite">The mod site from which to fetch mods.</param>
async Task DownloadAndCacheSiteAsync(ModCache modDump, IModSiteClient modSite)
{
	// fetch all mods
	ConsoleHelper.Print($"   Fetching mods from {modSite.SiteKey}...");
	Dictionary<long, GenericMod> remoteMods;
	while (true)
	{
		try
		{
			remoteMods = (await modSite.GetAllModsAsync()).ToDictionary(p => p.ID);
			break;
		}
		catch (Exception ex)
		{
			ex.Dump();
			string choice = ConsoleHelper.GetChoice("Do you want to [r]etry or [s]kip this site?", ["r", "s"]);
			if (choice == "s")
				return;
		}
	}

	// remove deleted/hidden mods
	bool removedAny = false;
	foreach (GenericMod mod in modDump.GetModsFor(modSite.SiteKey))
	{
		if (!remoteMods.ContainsKey(mod.ID))
		{
			if (DeleteRemovedMods)
			{
				removedAny = true;
				modDump.DeleteMod(modSite.SiteKey, mod.ID);
				ConsoleHelper.Print($"      Deleted mod {mod.ID} ('{mod.Name}' by {mod.Author}) which is no longer accessible.");
			}
			else
				ConsoleHelper.Print($"      Ignored mod {mod.ID} ('{mod.Name}' by {mod.Author}) which is no longer accessible. Enable {nameof(DeleteRemovedMods)} to delete it.");
		}
	}
	if (removedAny)
		modDump.SaveCache();

	// get new/updated mods
	List<(GenericMod mod, bool isNew)> queue = new();
	foreach ((long modId, GenericMod mod) in remoteMods)
	{
		GenericMod cachedMod = modDump.GetMod(mod.Site, mod.ID);

		bool isNew = true;
		if (cachedMod != null)
		{
			if (this.IsCacheUpToDate(cachedMod, mod))
				continue;

			isNew = false;
		}

		queue.Add((mod, isNew));
	}

	// save updated mods
	if (queue.Count > 0)
	{
		var progress = new IncrementalProgressBar(queue.Count).Dump();
		int fetchesSinceSave = 0;
		int i = 0;
		foreach ((GenericMod mod, bool isNew) in queue)
		{
			// update progress
			progress.Increment();
			progress.Caption = $"Fetching {(isNew ? "new" : "updated")} mod {mod.ID} \"{mod.Name}\" ({++i} of {queue.Count})...";

			// fetch & save
			while (true)
			{
				try
				{
					await this.DownloadAndCacheModDataAsync(modDump, mod, getDownloadLinks: async file => await modSite.GetDownloadUrlsAsync(mod, file));
					break;
				}
				catch (RateLimitedException ex)
				{
					if (fetchesSinceSave > 0)
					{
						modDump.SaveCache();
						fetchesSinceSave = 0;
					}

					this.LogAndAwaitRateLimit(ex, modSite.SiteKey);
					continue;
				}
			}
			fetchesSinceSave++;

			// update cache
			if (fetchesSinceSave >= this.ModFetchesPerSave)
			{
				progress.Caption = $"Saving mod cache...";
				modDump.SaveCache();
				fetchesSinceSave = 0;
			}
		}

		if (fetchesSinceSave > 0)
			modDump.SaveCache();
	}
}

/// <summary>Get whether a cached mod is already up-to-date with the latest fetched data.</summary>
/// <param name="cached">The cached mod data.</param>
/// <param name="fetched">The fetched mod data.</param>
private bool IsCacheUpToDate(GenericMod cached, GenericMod fetched)
{
	// basic mod info
	if (
		cached.Name != fetched.Name
		|| cached.Author != fetched.Author
		|| cached.AuthorLabel != fetched.AuthorLabel
		|| cached.Updated != fetched.Updated
		|| cached.Version != fetched.Version
		|| cached.Files.Length != fetched.Files.Length
	)
		return false;

	// basic file info
	Dictionary<long, GenericFile> fetchedFiles = fetched.Files.ToDictionary(p => p.ID);
	foreach (GenericFile cachedFile in cached.Files)
	{
		if (
			!fetchedFiles.TryGetValue(cachedFile.ID, out GenericFile fetchedFile)
			|| cachedFile.DisplayName != fetchedFile.DisplayName
			|| cachedFile.FileName != fetchedFile.FileName
			|| cachedFile.Version != fetchedFile.Version
		)
			return false;
	}

	return true;
}

/// <summary>Write mod data to the cache directory and download the available files.</summary>
/// <param name="modDump">The mod dump to update.</param>
/// <param name="mod">The mod whose data to save.</param>
/// <param name="getDownloadLinks">Get the download URLs for a specific file. If this returns multiple URLs, the first working one will be used.</param>
async Task DownloadAndCacheModDataAsync(ModCache modDump, GenericMod mod, Func<GenericFile, Task<Uri[]>> getDownloadLinks)
{
	// reset cache folder
	modDump.DeleteMod(mod.Site, mod.ID);
	modDump.AddMod(mod);

	// download files
	using (WebClient downloader = new WebClient())
	{
		foreach (GenericFile file in mod.Files)
		{
			// download file from first working CDN
			FileInfo localFile = new FileInfo(modDump.GetModFilePath(mod.Site, mod.ID, file));
			Queue<Uri> sources = new Queue<Uri>(await getDownloadLinks(file));
			bool skipped = false;
			while (true)
			{
				if (!sources.Any())
				{
					ConsoleHelper.Print($"File {mod.ID} > {file.ID} has no download sources available.", Severity.Error);
					ConsoleHelper.Print("You can optionally download it yourself:", Severity.Error);

					Util.HorizontalRun(
						true,
						ConsoleHelper.FormatMessage($"   download from:", Severity.Error),
						new Hyperlinq(mod.Site switch
						{
							ModSite.CurseForge => $"{mod.PageUrl}/download/{file.ID}",
							_ => mod.PageUrl
						})
					).Dump();

					CancellationTokenSource inputCancelToken = new();

					Util.HorizontalRun(
						true,
						ConsoleHelper.FormatMessage($"   download to:  {localFile.FullName}", Severity.Error),
						new Hyperlinq(
							() =>
							{
								if (this.MoveDownloadedFile(file, localFile.FullName))
									inputCancelToken.Cancel();
							},
							"[move download automatically]"
						)
					).Dump();

					while (true)
					{
						try
						{
							if (await ConsoleHelper.GetChoiceAsync("Do you want to [u]se a manually downloaded file or [s]kip this file?", ["u", "s"], inputCancelToken.Token) == "u")
							{
								if (!File.Exists(localFile.FullName))
								{
									ConsoleHelper.Print($"No file found at {localFile.FullName}.", Severity.Error);
									continue;
								}

								ConsoleHelper.Print($"Using manually downloaded file.", Severity.Info);
							}
							else
							{
								ConsoleHelper.Print($"Skipped file.", Severity.Info);
								skipped = true;
							}
						}
						catch (OperationCanceledException) // input cancelled due to [move download automatically]
						{
							if (!File.Exists(localFile.FullName))
							{
								ConsoleHelper.Print($"No file found at {localFile.FullName}.", Severity.Error);
								continue;
							}
						}
						break;
					}
					break;
				}

				Uri url = sources.Dequeue();
				try
				{
					downloader.DownloadFile(url, localFile.FullName);
					break;
				}
				catch (Exception ex)
				{
					string errorPhrase = $"Failed downloading mod {mod.ID} > file {file.ID} from {url}";
					if (sources.Any())
						ConsoleHelper.Print($"{errorPhrase}. Trying next CDN...\n{ex}", Severity.Warning);
					else
						ConsoleHelper.Print($"{errorPhrase}.\n{ex}", Severity.Error);
					Console.WriteLine();
				}
			}

			// unpack file
			if (!skipped && !modDump.TryUnpackFile(mod.Site, mod.ID, file, out string error))
				ConsoleHelper.Print($"Failed unpacking mod {mod.ID} > file {file.ID}: {error}.", Severity.Warning);
		}
	}
}

/// <summary>If a file was downloaded manually, move the downloaded file to the target path automatically.</summary>
/// <param name="file">The file info from the mod site.</param>
/// <param name="targetPath">The absolute file path to which to move it.</param>
/// <returns>Returns whether it was moved successfully.</returns>
private bool MoveDownloadedFile(GenericFile file, string targetPath)
{
	FileInfo fromFile = new(Path.Combine(DownloadsPath, file.FileName));
	if (!fromFile.Exists)
	{
		ConsoleHelper.Print($"   No file found at {fromFile.FullName}.");
		return false;
	}

	fromFile.MoveTo(targetPath);
	ConsoleHelper.Print($"  Download moved to {targetPath}.");
	return true;
}

/// <summary>Get the human-readable mod names for the compatibility list.</summary>
/// <param name="folder">The downloaded mod folder.</param>
/// <param name="mod">The mod metadata.</param>
private string[] GetModNames(ParsedFile folder, ParsedMod mod)
{
	// get possible names
	string[] names = new[] { folder.ModDisplayName?.Trim(), mod.Name?.Trim() }
		.Where(name => !string.IsNullOrWhiteSpace(name))
		.OrderBy(name => name)
		.Distinct(StringComparer.InvariantCultureIgnoreCase)
		.ToArray();

	// if both names are equivalent except for spacing and capitalization (e.g. SomeModName vs Some Mod Name), use the longer version
	if (names.Length == 2 && names[0].Replace(" ", "").ToLower() == names[1].Replace(" ", "").ToLower())
	{
		names = names
			.OrderByDescending(p => p.Length)
			.Take(1)
			.ToArray();
	}

	return names;
}

/// <summary>Get the human-readable mod author names for the compatibility list.</summary>
/// <param name="manifest">The downloaded mod manifest file.</param>
/// <param name="mod">The mod metadata.</param>
private string[] GetAuthorNames(IManifest manifest, ParsedMod mod)
{
	return new[] { manifest?.Author?.Trim(), mod.AuthorLabel?.Trim() ?? mod.Author?.Trim() }
		.SelectMany(field => field?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
		.OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
		.Distinct(StringComparer.OrdinalIgnoreCase)
		.ToArray();
}

/// <summary>Get the GitHub repository name for a mod, if available.</summary>
/// <param name="manifest">The downloaded mod manifest file.</param>
/// <param name="mod">The mod metadata.</param>
private string GetGitHubRepo(IManifest manifest, ParsedMod mod)
{
	// from update key
	foreach (string rawUpdateKey in manifest?.UpdateKeys ?? Array.Empty<string>())
	{
		string updateKey = rawUpdateKey?.Trim();
		if (updateKey?.StartsWith("GitHub", StringComparison.OrdinalIgnoreCase) == true)
		{
			Match match = Regex.Match(updateKey, @"^GitHub\s*:\s*(.+)");
			if (match.Success)
				return match.Groups[1].Value;
		}
	}

	// from mod description
	string description = this.TryGetModDescription(mod);
	if (!string.IsNullOrWhiteSpace(description))
	{
		MatchCollection matches = Regex.Matches(description, @"github\.com/([a-z0-9_\-\.]+/[a-z0-9_\-\.]+)", RegexOptions.IgnoreCase);
		foreach (Match match in matches)
		{
			string repo = match.Groups[1].Value;

			if (repo.StartsWith("Pathoschild/", StringComparison.OrdinalIgnoreCase))
				continue; // this is usually not the mod's source link (e.g. links to the Content Patcher docs, or copy/pasted without changing the link)

			return repo;
		}
	}

	// none found
	return null;
}

/// <summary>Get the custom source code URL for a mod, if available.</summary>
/// <param name="mod">The mod metadata.</param>
private string GetCustomSourceUrl(ParsedMod mod)
{
	// from mod description
	string description = this.TryGetModDescription(mod);
	if (!string.IsNullOrWhiteSpace(description))
	{
		Match match = Regex.Match(description, @"(gitlab\.com/[a-z0-9_\-\.]+/[a-z0-9_\-\.]+|sourceforge\.net/p/[a-z0-9_\-\.]+)", RegexOptions.IgnoreCase);
		if (match.Success)
			return $"https://{match.Groups[1].Value}";
	}

	// none found
	return null;
}

/// <summary>Get the raw mod page description, if available.</summary>
/// <param name="mod">The mod metadata.</param>
private string TryGetModDescription(ParsedMod mod)
{
	// ModDrop
	{
		if (mod.RawData.TryGetValue("desc", out object rawValue) && rawValue is string description)
			return description;
	}

	// Nexus
	{
		if (mod.RawData.TryGetValue("Description", out object rawValue) && rawValue is string description)
			return description;
	}

	return null;
}

/// <summary>Build a human-readable file list for a directory path.</summary>
/// <param name="root">The directory for which to build a file list.</param>
public string BuildFileList(DirectoryInfo root)
{
	static IEnumerable<string> BuildEntries(FileSystemInfo entry, string indent = "")
	{
		// yield current
		string icon = entry is DirectoryInfo ? "📁" : "🗎";
		yield return $"{indent}{icon} {entry.Name}";

		// yield children
		if (entry is DirectoryInfo dir)
		{
			foreach (FileSystemInfo child in dir.EnumerateFileSystemInfos().OrderByDescending(p => p is FileInfo))
			{
				foreach (var subEntry in BuildEntries(child, $"{indent}    "))
					yield return subEntry;
			}
		}
	}

	return string.Join("\n", BuildEntries(root));
}

/// <summary>Get whether a given mod and file ID should be ignored when validating mods.</summary>
/// <param name="site">The mod site.</param>
/// <param name="siteId">The mod ID on the mod site.</param>
/// <param name="fileId">The file ID on the mod site.</param>
/// <param name="manifestId">The mod's manifest ID, if available.</param>
private bool ShouldIgnoreForAnalysis(ModSite site, long siteId, long fileId, string manifestId)
{
	return
		this.IgnoreForAnalysisBySiteId.TryGetValue($"{site}:{siteId}", out ModSearch[] entries)
		&& entries.Any(search => search.Matches(site: site, siteId: siteId, fileId: fileId, manifestId: manifestId));
}

/// <summary>Log a human-readable summary for a rate limit exception, and pause the thread until the rate limit is refreshed.</summary>
/// <param name="ex">The rate limit exception.</param>
/// <param name="site">The mod site whose rate limit was exceeded.</param>
private void LogAndAwaitRateLimit(RateLimitedException ex, ModSite site)
{
	TimeSpan resumeDelay = ex.TimeUntilRetry;
	DateTime resumeTime = DateTime.Now + resumeDelay;

	ConsoleHelper.Print($"{site} rate limit exhausted: {ex.RateLimitSummary}; resuming in {ConsoleHelper.GetFormattedTime(resumeDelay)} ({resumeTime:HH:mm:ss} local time).");
	Thread.Sleep(resumeDelay);
}

/// <summary>The manual overrides for specific mods or source repos when analyzing them with this script.</summary>
private class ModOverridesData
{
	/*********
	** Accessors
	*********/
	/// <summary>Mods to ignore when validating mods or compiling statistics.</summary>
	public ModSearch[] IgnoreForAnalysis { get; init; }


	/*********
	** Public methods
	*********/
	/// <summary>Load the data from a file path.</summary>
	/// <param name="filePath">The file path from which to read the JSON data.</param>
	public static ModOverridesData LoadFrom(string filePath)
	{
		// load raw data
		if (!File.Exists(filePath))
			throw new FileNotFoundException($"Can't load mod overrides data because no file was found at path '{filePath}'.");
		string json = File.ReadAllText(filePath);
		var rawData = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);

		// read ignore for analysis
		List<ModSearch> ignoreForAnalysis = new();
		foreach ((string rawSiteKey, string[] entries) in rawData["IgnoreForAnalysis"].ToObject<Dictionary<string, string[]>>())
		{
			if (!Enum.TryParse(rawSiteKey, out ModSite siteKey))
				throw new InvalidOperationException($"Can't load mod overrides data from '{filePath}': invalid site key '{rawSiteKey}'.");

			foreach (string entry in entries)
			{
				ignoreForAnalysis.Add(
					ModSearch.Parse(siteKey, entry)
				);
			}
		}

		// build model
		return new ModOverridesData
		{
			IgnoreForAnalysis = ignoreForAnalysis.ToArray()
		};
	}
}

/// <summary>Matches a mod which should be ignored when validating mod data or compiling statistics.</summary>
class ModSearch
{
	/// <summary>The site which hosts the mod.</summary>
	public ModSite Site { get; }

	/// <summary>The mod's page ID in the site.</summary>
	public long SiteId { get; }

	/// <summary>The uploaded file ID, or <c>null</c> for any value.</summary>
	public long? FileId { get; }

	/// <summary>The mod's manifest ID, or <c>null</c> for any value.</summary>
	public string ManifestId { get; }

	/// <summary>Construct an instance.</summary>
	/// <param name="site">The site which hosts the mod.</param>
	/// <param name="siteId">The mod's page ID in the site.</param>
	/// <param name="fileId">The uploaded file ID, or <c>null</c> for any value.</param>
	/// <param name="manifestId">The mod's manifest ID, or <c>null</c> for any value.</param>
	public ModSearch(ModSite site, long siteId, long? fileId = null, string manifestId = null)
	{
		this.Site = site;
		this.SiteId = siteId;
		this.FileId = fileId;
		this.ManifestId = manifestId;
	}

	/// <summary>Get whether a given mod and file ID should be ignored when validating mods.</summary>
	/// <param name="site">The mod site.</param>
	/// <param name="siteId">The mod ID on the mod site.</param>
	/// <param name="fileId">The file ID on the mod site.</param>
	/// <param name="manifestId">The mod's manifest ID, if available.</param>
	public bool Matches(ModSite site, long siteId, long fileId, string manifestId)
	{
		return
			this.Site == site
			&& this.SiteId == siteId
			&& (this.FileId == null || this.FileId == fileId)
			&& (this.ManifestId == null || this.ManifestId == manifestId);
	}

	/// <summary>Get a set of mod search models for the same site.</summary>
	/// <param name="site">The mod site.</param>
	/// <param name="siteIds">The mod IDs on the mod site.</param>
	public static IEnumerable<ModSearch> ForSiteIds(ModSite site, params long[] siteIds)
	{
		return siteIds.Select(id => new ModSearch(site, id));
	}

	/// <summary>Parse a string representation of a mod search.</summary>
	/// <param name="site">The mod site.</param>
	/// <param name="entry">The mod entry to match, in the form <c>{mod page ID} [file ID] [@{manifest ID}]</c>.</param>
	public static ModSearch Parse(ModSite site, string entry)
	{
		string[] mainParts = entry.Split('@', StringSplitOptions.TrimEntries);
		string[] idParts = mainParts[0].Split(' ', 2);

		if (!long.TryParse(idParts[0], out long siteId))
			throw new InvalidOperationException($"Can't parse {site} mod override entry '{entry}': invalid mod page ID '{idParts[0]}'");

		long? fileId = null;
		if (idParts.Length > 1)
		{
			if (!long.TryParse(idParts[1], out long rawFileId))
				throw new InvalidOperationException($"Can't parse {site} mod override entry '{entry}': invalid file ID '{idParts[1]}'");
			fileId = rawFileId;
		}

		return new ModSearch(
			site: site,
			siteId: siteId,
			fileId: fileId,
			manifestId: mainParts.Length > 1 ? mainParts[1] : null
		);
	}
}

/// <summary>An exception raised when API client exceeds the rate limits for an API.</summary>
class RateLimitedException : Exception
{
	/*********
	** Accessors
	*********/
	/// <summary>The amount of time to wait until it's safe to retry the request.</summary>
	public TimeSpan TimeUntilRetry { get; }

	/// <summary>A human-readable of current rate limit values, if available.</summary>
	public string RateLimitSummary { get; }


	/*********
	** Accessors
	*********/
	public RateLimitedException(TimeSpan timeUntilRetry, string rateLimitSummary)
		: base("Rate limits have been exceeded for this API.")
	{
		this.TimeUntilRetry = timeUntilRetry;
		this.RateLimitSummary = rateLimitSummary;
	}
}

/// <summary>A client which fetches mods from a particular mod site.</summary>
interface IModSiteClient
{
	/*********
	** Accessors
	*********/
	/// <summary>The identifier for this mod site used in update keys.</summary>
	ModSite SiteKey { get; }


	/*********
	** Methods
	*********/
	/// <summary>Authenticate with the mod site if needed.</summary>
	Task AuthenticateAsync();

	/// <summary>Get every mod currently available on this site.</summary>
	Task<GenericMod[]> GetAllModsAsync();

	/// <summary>Get the download URLs for a file. If this returns multiple URLs, they're assumed to be mirrors and the first working URL will be used.</summary>
	/// <param name="mod">The mod for which to get download URLs.</param>
	/// <param name="file">The file for which to get download URLs.</param>
	/// <exception cref="RateLimitedException">The API client has exceeded the API's rate limits.</exception>
	Task<Uri[]> GetDownloadUrlsAsync(GenericMod mod, GenericFile file);
}

/// <summary>A client which fetches mods from the CurseForge export API.</summary>
class CurseForgeApiClient : IModSiteClient
{
	/*********
	** Fields
	*********/
	/// <summary>A regex pattern which matches a version number in a CurseForge mod file name.</summary>
	private readonly Regex VersionInNamePattern = new Regex(@"^(?:.+? | *)v?(\d+\.\d+(?:\.\d+)?(?:-.+?)?) *(?:\.(?:zip|rar|7z))?$", RegexOptions.Compiled);

	/// <summary>The CurseForge export API client.</summary>
	private readonly CurseForgeExportApiClient CurseForge;


	/*********
	** Accessors
	*********/
	/// <inheritdoc />
	public ModSite SiteKey { get; } = ModSite.CurseForge;


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="exportApiUrl">The base URL for the CurseForge export API.</param>
	/// <param name="userAgent">The user agent sent to the mod site API.</param>
	public CurseForgeApiClient(string exportApiUrl, string userAgent)
	{
		this.CurseForge = new CurseForgeExportApiClient(userAgent, exportApiUrl);
	}

	/// <inheritdoc/>
	public Task AuthenticateAsync()
	{
		return Task.CompletedTask;
	}

	/// <inheritdoc/>
	public async Task<GenericMod[]> GetAllModsAsync()
	{
		CurseForgeFullExport export = await this.CurseForge.FetchExportAsync();

		if (export.CacheHeaders.LastModified < DateTimeOffset.UtcNow.AddHours(-MaxExportAge))
			throw new InvalidOperationException($"Can't fetch export from CurseForge export API: the export was last updated {Math.Round((DateTimeOffset.UtcNow - export.CacheHeaders.LastModified).TotalHours, 2)} hours ago.");

		return export.Mods.Values
			.Select(this.Parse)
			.ToArray();
	}

	/// <inheritdoc />
	public Task<Uri[]> GetDownloadUrlsAsync(GenericMod mod, GenericFile file)
	{
		return Task.FromResult(
			GetDownloadUrls(mod, file).Select(url => new Uri(url)).ToArray()
		);
	}


	/*********
	** Private methods
	*********/
	/// <summary>Get the download URLs for a CurseForge file.</summary>
	/// <param name="mod">The mod which has the file to download.</param>
	/// <param name="file">The file for which to get download URLs.</param>
	private IEnumerable<string> GetDownloadUrls(GenericMod mod, GenericFile file)
	{
		// API download URL
		if (file.RawData.GetValueOrDefault("downloadUrl") is string apiDownloadUrl)
			yield return apiDownloadUrl;

		// build CDN URL manually
		// The API doesn't always return a download URL.
		yield return $"https://www.curseforge.com/api/v1/mods/{mod.ID}/files/{file.ID}/download";
	}

	/// <summary>Parse raw mod data from the CurseForge API.</summary>
	/// <param name="rawMod">The raw mod data.</param>
	private GenericMod Parse(CurseForgeModExport mod)
	{
		// get author names
		string[] authorNames = mod.Authors.Select(p => p.Name).ToArray();

		// get last updated
		DateTimeOffset lastUpdated;
		{
			lastUpdated = mod.DateCreated;
			if (mod.DateModified > lastUpdated)
				lastUpdated = mod.DateModified;
			if (mod.DateReleased > lastUpdated)
				lastUpdated = mod.DateReleased;
		}

		// get files
		List<GenericFile> files = new List<GenericFile>();
		foreach (CurseForgeFileExport file in mod.Files)
		{
			files.Add(new GenericFile(
				id: file.Id,
				type: file.ReleaseType == 1 ? GenericFileType.Main : GenericFileType.Optional, // FileReleaseType: 1=release, 2=beta, 3=alpha
				displayName: file.DisplayName,
				fileName: file.FileName,
				version: this.GetFileVersion(file.DisplayName, file.FileName),
				uploaded: file.FileDate,
				rawData: file
			));

			if (file.FileDate > lastUpdated)
				lastUpdated = file.FileDate;
		}

		// get model
		CurseForgeModExport rawModWithoutFiles = JsonConvert.DeserializeObject<CurseForgeModExport>(JsonConvert.SerializeObject(mod));
		rawModWithoutFiles.Files = Array.Empty<CurseForgeFileExport>();

		return new GenericMod(
			site: ModSite.CurseForge,
			id: mod.Id,
			name: mod.Name,
			author: authorNames.FirstOrDefault(),
			authorLabel: authorNames.Length > 1 ? string.Join(", ", authorNames) : null,
			pageUrl: mod.ModPageUrl,
			version: null,
			updated: lastUpdated,
			rawData: rawModWithoutFiles,
			files: files.ToArray()
		);
	}

	/// <summary>Get a raw version string for a mod file, if available.</summary>
	/// <param name="displayName">The file's display name.</param>
	/// <param name="fileName">The filename.</param>
	private string GetFileVersion(string displayName, string fileName)
	{
		Match match = this.VersionInNamePattern.Match(displayName);
		if (!match.Success)
			match = this.VersionInNamePattern.Match(fileName);

		return match.Success
			? match.Groups[1].Value
			: null;
	}
}

/// <summary>A client which fetches mods from the ModDrop export API.</summary>
class ModDropApiClient : IModSiteClient
{
	/*********
	** Fields
	*********/
	/// <summary>The username with which to log in to the main API, if any.</summary>
	private readonly string Username;

	/// <summary>The password with which to log in to the main API, if any.</summary>
	private readonly string Password;

	/// <summary>The ModDrop API client.</summary>
	private IClient MainApi = new FluentClient("https://www.moddrop.com/api");

	/// <summary>The ModDrop export API client.</summary>
	private readonly ModDropExportApiClient ExportApi;


	/*********
	** Accessors
	*********/
	/// <inheritdoc />
	public ModSite SiteKey { get; } = ModSite.ModDrop;


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="exportApiUrl">The base URL for the ModDrop export API.</param>
	/// <param name="userAgent">The user agent sent to the mod site API.</param>
	/// <param name="username">The username with which to log in, if any.</param>
	/// <param name="password">The password with which to log in, if any.</param>
	public ModDropApiClient(string exportApiUrl, string userAgent, string username, string password)
	{
		this.ExportApi = new ModDropExportApiClient(userAgent, exportApiUrl);
		this.Username = username;
		this.Password = password;
	}

	/// <summary>Authenticate with the mod site if needed.</summary>
	public async Task AuthenticateAsync()
	{
		if (this.Username == null || this.Password == null)
			return;

		var response = await this.MainApi
			.PostAsync("v1/auth/login")
			.WithBasicAuthentication(this.Username, this.Password)
			.AsRawJsonObject();

		string apiToken = response["apiToken"].Value<string>();
		if (string.IsNullOrEmpty(apiToken))
			throw new InvalidOperationException($"Authentication with the ModDrop API failed:\n{response.ToString()}");

		this.MainApi.AddDefault(p => p.WithHeader("Authorization", apiToken));
	}

	/// <inheritdoc/>
	public async Task<GenericMod[]> GetAllModsAsync()
	{
		ModDropFullExport export = await this.ExportApi.FetchExportAsync();

		if (export.CacheHeaders.LastModified < DateTimeOffset.UtcNow.AddHours(-MaxExportAge))
			throw new InvalidOperationException($"Can't fetch export from ModDrop export API: the export was last updated {Math.Round((DateTimeOffset.UtcNow - export.CacheHeaders.LastModified).TotalHours, 2)} hours ago.");

		return export.Mods.Values
			.Where(p => !p.IsDeleted && p.IsPublished)
			.Select(this.Parse)
			.ToArray();
	}

	/// <summary>Get the download URLs for a file. If this returns multiple URLs, they're assumed to be mirrors and the first working URL will be used.</summary>
	/// <param name="mod">The mod for which to get download URLs.</param>
	/// <param name="file">The file for which to get download URLs.</param>
	/// <exception cref="RateLimitedException">The API client has exceeded the API's rate limits.</exception>
	public async Task<Uri[]> GetDownloadUrlsAsync(GenericMod mod, GenericFile file)
	{
		try
		{
			var response = await this.MainApi
				.PostAsync($"v1/mod-{mod.ID}/file-{file.ID}/download")
				.AsRawJsonObject();

			return new[]
			{
				new Uri(response["url"].Value<string>())
			};
		}
		catch (Exception ex)
		{
			string error = $"Can't fetch download URL for \"{mod.Name}\" (#{mod.ID}) > file \"{file.DisplayName} {file.Version}\" (#{file.ID}).";
			if (ex is ApiException apiEx)
				error += $"\n\nHTTP {apiEx.Response.Status}: {await apiEx.Response.AsString()}";
			error += $"\n\n{ex.ToString()}";

			ConsoleHelper.Print(error, Severity.Error);
			return new Uri[0];
		}
	}


	/*********
	** Private methods
	*********/
	/// <summary>Parse raw mod data from the ModDrop export API.</summary>
	/// <param name="rawMod">The raw mod data.</param>
	private GenericMod Parse(ModDropModExport rawMod)
	{
		// get author names
		string author = rawMod.UserName?.Trim();
		string authorLabel = rawMod.AuthorName?.Trim();
		if (author.Equals(authorLabel, StringComparison.InvariantCultureIgnoreCase))
			authorLabel = null;

		// get last updated
		DateTimeOffset lastUpdated = DateTimeOffset.FromUnixTimeMilliseconds(rawMod.DateUpdated);
		{
			DateTimeOffset published = DateTimeOffset.FromUnixTimeMilliseconds(rawMod.DatePublished);
			if (published > lastUpdated)
				lastUpdated = published;
		}

		// get files
		List<GenericFile> files = new List<GenericFile>();
		foreach (ModDropFileExport file in rawMod.Files)
		{
			try
			{
				if (file.IsOld || file.IsDeleted || file.IsHidden)
					continue;

				DateTimeOffset dateCreated = DateTimeOffset.FromUnixTimeMilliseconds(file.DateCreated);
				if (dateCreated > lastUpdated)
					lastUpdated = dateCreated;

				files.Add(new GenericFile(
					id: file.Id,
					type: !file.IsPreRelease && !file.IsAlternative
						? GenericFileType.Main
						: GenericFileType.Optional,
					displayName: file.Name,
					fileName: file.FileName,
					version: file.Version,
					uploaded: dateCreated,
					rawData: file
				));
			}
			catch
			{
				new { mod = new Lazy<ModDropModExport>(() => rawMod), file = new Lazy<ModDropFileExport>(() => file) }.Dump();
				throw;
			}
		}

		// get model
		return new GenericMod(
			site: ModSite.ModDrop,
			id: rawMod.Id,
			name: rawMod.Title,
			author: author,
			authorLabel: authorLabel,
			pageUrl: rawMod.PageUrl,
			version: null,
			updated: lastUpdated,
			rawData: rawMod,
			files: files.ToArray()
		);
	}
}

/// <summary>A client which fetches mods from the Nexus Mods API.</summary>
class NexusApiClient : IModSiteClient
{
	/*********
	** Fields
	*********/
	/// <summary>The Nexus Mods game key for Stardew Valley.</summary>
	private readonly string GameKey = "stardewvalley";

	/// <summary>The API client for the main Nexus API.</summary>
	private NexusClient MainApi;

	/// <summary>The Nexus export API client.</summary>
	private readonly NexusExportApiClient ExportApi;


	/*********
	** Accessors
	*********/
	/// <inheritdoc/>
	public ModSite SiteKey { get; } = ModSite.Nexus;


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="exportApiUrl">The base URL for the ModDrop export API.</param>
	/// <param name="userAgent">The user agent sent to the mod site API.</param>
	/// <param name="apiKey">The Nexus API key with which to authenticate.</param>
	/// <param name="appName">An arbitrary name for the app/script using the client, reported to the Nexus Mods API and used in the user agent.</param>
	/// <param name="appVersion">An arbitrary version number for the <paramref name="appName" /> (ideally a semantic version).</param>
	public NexusApiClient(string exportApiUrl, string userAgent, string apiKey, string appName, string appVersion)
	{
		this.MainApi = new NexusClient(apiKey, appName, appVersion);
		this.ExportApi = new NexusExportApiClient(userAgent, exportApiUrl);
	}

	/// <inheritdoc/>
	public Task AuthenticateAsync()
	{
		return Task.CompletedTask;
	}

	/// <inheritdoc/>
	public async Task<GenericMod[]> GetAllModsAsync()
	{
		NexusFullExport export = await this.ExportApi.FetchExportAsync();

		if (export.CacheHeaders.LastModified < DateTimeOffset.UtcNow.AddHours(-MaxExportAge))
			throw new InvalidOperationException($"Can't fetch export from Nexus export API: the export was last updated {Math.Round((DateTimeOffset.UtcNow - export.CacheHeaders.LastModified).TotalHours, 2)} hours ago.");

		return export.Data
			.Where(p => p.Value.Published && p.Value.AllowView && !p.Value.Moderated)
			.Select(p => this.Parse(p.Key, p.Value))
			.ToArray();
	}

	/// <inheritdoc/>
	public async Task<Uri[]> GetDownloadUrlsAsync(GenericMod mod, GenericFile file)
	{
		try
		{
			ModFileDownloadLink[] downloadLinks = await this.MainApi.ModFiles.GetDownloadLinks(this.GameKey, (int)mod.ID, (int)file.ID);
			return downloadLinks.Select(p => p.Uri).ToArray();
		}
		catch (ApiException ex) when (ex.Status == (HttpStatusCode)429)
		{
			throw await this.GetRateLimitExceptionAsync();
		}
	}


	/*********
	** Private methods
	*********/
	/// <summary>Parse raw mod data from the Nexus export API.</summary>
	/// <param name="modId">The mod ID.</param>
	/// <param name="rawMod">The raw mod data.</param>
	private GenericMod Parse(uint modId, NexusModExport mod)
	{
		// get author
		string author = mod.Uploader ?? mod.Author;
		string authorLabel = mod.Author != null && !mod.Author.Equals(author, StringComparison.InvariantCultureIgnoreCase)
			? mod.Author
			: null;

		// get files
		DateTimeOffset lastUpdated = DateTimeOffset.MinValue;
		List<GenericFile> files = new List<GenericFile>();
		foreach ((uint fileId, NexusFileExport file) in mod.Files)
		{
			// get file type
			GenericFileType type;
			if (file.CategoryId is (int)FileCategory.Main)
				type = GenericFileType.Main;
			else if (file.CategoryId is (int)FileCategory.Optional)
				type = GenericFileType.Optional;
			else
				continue;

			// track last update
			DateTimeOffset uploadedAt = DateTimeOffset.FromUnixTimeSeconds(file.UploadedAt);
			if (uploadedAt > lastUpdated)
				lastUpdated = uploadedAt;

			// add file
			files.Add(new GenericFile(id: fileId, type: type, displayName: file.Name, fileName: file.FileName, version: file.Version, uploaded: uploadedAt, rawData: file));
		}

		// special case: if a mod has zero main/optional files, get files from any non-archived/deleted/old category
		if (files.Count == 0)
		{
			foreach ((uint fileId, NexusFileExport file) in mod.Files)
			{
				if (file.CategoryId is (int)FileCategory.Archived or (int)FileCategory.Deleted or (int)FileCategory.Old)
					continue;

				// track last update
				DateTimeOffset uploadedAt = DateTimeOffset.FromUnixTimeSeconds(file.UploadedAt);
				if (uploadedAt > lastUpdated)
					lastUpdated = uploadedAt;

				files.Add(new GenericFile(id: fileId, type: GenericFileType.Optional, displayName: file.Name, fileName: file.FileName, version: file.Version, uploaded: uploadedAt, rawData: file));
			}
		}

		// get model
		return new GenericMod(
			site: ModSite.Nexus,
			id: modId,
			name: mod.Name,
			author: author,
			authorLabel: authorLabel,
			pageUrl: $"https://www.nexusmods.com/stardewvalley/mods/{modId}",
			version: null,
			updated: lastUpdated,
			rawData: mod,
			files: files.ToArray()
		);
	}

	/// <summary>Get an exception indicating that rate limits have been exceeded.</summary>
	private async Task<RateLimitedException> GetRateLimitExceptionAsync()
	{
		IRateLimitManager rateLimits = await this.MainApi.GetRateLimits();
		TimeSpan unblockTime = rateLimits.GetTimeUntilRenewal();
		throw new RateLimitedException(unblockTime, this.GetRateLimitSummary(rateLimits));
	}

	/// <summary>Get a human-readable summary for the current rate limits.</summary>
	/// <param name="meta">The current rate limits.</param>
	private string GetRateLimitSummary(IRateLimitManager meta)
	{
		return $"{meta.DailyRemaining}/{meta.DailyLimit} daily resetting in {ConsoleHelper.GetFormattedTime(meta.DailyReset - DateTimeOffset.UtcNow)}, {meta.HourlyRemaining}/{meta.HourlyLimit} hourly resetting in {ConsoleHelper.GetFormattedTime(meta.HourlyReset - DateTimeOffset.UtcNow)}";
	}

	/// <summary>Get a human-readable formatted time span.</summary>
	/// <param name="span">The time span to format.</param>
	private string GetFormattedTime(TimeSpan span)
	{
		int hours = (int)span.TotalHours;
		int minutes = (int)span.TotalMinutes - (hours * 60);
		return $"{hours:00}:{minutes:00}";
	}
}
