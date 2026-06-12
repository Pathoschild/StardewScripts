<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.CoreInterfaces.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>StardewModdingAPI</Namespace>
  <Namespace>StardewModdingAPI.Toolkit</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.CompatibilityRepo</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModDataset</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModScanning</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.UpdateData</Namespace>
  <Namespace>System.Dynamic</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Text.Encodings.Web</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Web</Namespace>
  <RuntimeVersion>10.0</RuntimeVersion>
</Query>

/*

See documentation at https://github.com/Pathoschild/StardewScripts.

*/
#load "Utilities/ConsoleHelper.linq"
#load "Utilities/FileHelper.linq"
#load "Utilities/ModCacheUtilities.linq"

/*********
** Configuration
*********/
/*****
** Behavior
*****/
/// <summary>The date from which to list updated mods.</summary>
readonly DateTimeOffset ListModsUpdatedSince = GetStartOfMonth().AddDays(-6);

/*****
** Folder paths
*****/
/// <summary>The mods folder to which mods are copied when you click 'install mod'.</summary>
const string InstallModsToPath = @"C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods (test)";

/// <summary>The full path to the <a href="https://github.com/Pathoschild/StardewModData">open mod dataset</a> repo.</summary>
const string ModDataRepoPath = @"E:\source\_Stardew\_ModData";

/// <summary>If set, the full path to a local copy of the compatibility list repo to read directly instead of fetching it from the server.</summary>
const string LocalCompatListRepoPath = @"E:\source\_Stardew\_SmapiCompatibilityList";

/// <summary>The full path to the file containing mod data analysis overrides.</summary>
/// <remarks>This should be the <c>scripts/metadata/mod-analysis-overrides.jsonc</c> file from the <a href="https://github.com/Pathoschild/SmapiCompatibilityList">Pathoschild/SmapiCompatibilityList repo</a>.</remarks>
const string ModDataOverridesFilePath = @"E:\source\_Stardew\_SmapiCompatibilityList\scripts\metadata\mod-analysis-overrides.jsonc";

/*****
** Internal
*****/
/// <summary>The full path to the directory which contains downloaded mod files.</summary>
const string ModDumpPath = $@"{ModDataRepoPath}\mod-dump";

/// <summary>Provides higher-level utilities for working with the underlying mod cache.</summary>
private readonly ModCacheUtilities ModCacheHelper = new(ModDataRepoPath, InstallModsToPath);

/// <summary>The manual overrides for specific mods or source repos when analyzing them with this script.</summary>
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
	this.ModOverrides = ModOverridesData.LoadFrom(ModDataOverridesFilePath);

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

	// load mods from repo
	ConsoleHelper.Print("Loading mods from repo...");
	ModPageRecord[] modPages = LoadModsFromRepo(ModDataRepoPath).ToArray();
	ConsoleHelper.Print($"Loaded {modPages.Length:#,###} mods.");

	// add launch button
	Util.VerticalRun(
		new Hyperlinq(
			() => Process.Start(
				fileName: Path.Combine(InstallModsToPath, "..", "StardewModdingAPI.exe"),
				arguments: @$"--mods-path ""{Path.GetFileName(InstallModsToPath)}"""
			),
			"launch SMAPI"
		)
	).Dump("actions");

	// detect issues
	ConsoleHelper.Print("Running analyses...");
	{
		Util.RawHtml("<h1>Detected issues</h1>").Dump();

		// compatibility list issues
		Util.RawHtml("<h3>Compatibility list issues</h3>").Dump();
		{
			var notOnCompatList = this.GetModsNotOnCompatibilityList(modPages, compatList).ToArray();
			if (notOnCompatList.Length > 0)
			{
				notOnCompatList.Dump("SMAPI mods not on the compatibility list");
				new Lazy<object>(() => Util.RawHtml("<pre>" + HttpUtility.HtmlEncode(string.Join("\n", notOnCompatList.Select(p => ((Lazy<string>)p.CompatEntry).Value))) + "</pre>")).Dump("SMAPI mods not on the compatibility list (JSON format)");
			}
			else
				"none".Dump("SMAPI mods not on the compatibility list");
		}
		this.GetCompatibilityListModsNotInRepo(modPages, compatList).Dump("Mods on the compatibility list which weren't found on the modding sites");
		this.GetModsWithSourceNotOnCompatList(modPages, compatList).Dump("Mods on the compatibility list whose source repo doesn't match cached data");
		this.GetModsMarkedHiddenWhichAreNot(modPages, compatList).Dump("Mods on the compatibility list marked deleted/hidden which were found on a mod site");
		//this.GetModsWhichAreContentPacks(modPages, compatList).Dump("Mods on the compatibility list which are actually content packs (may have false positives with multiple versions)");

		// mod issues
		Util.RawHtml("<h3>Mod issues</h3>").Dump();
		this.GetInvalidMods(modPages).Dump("Mods marked invalid by SMAPI toolkit (except blacklist)");

		// script issues
		Util.RawHtml("<h3>Script issues</h3>").Dump();
		this.GetInvalidIgnoreModEntries(modPages).Dump($"{nameof(ModOverridesData.IgnoreForAnalysis)} values which don't match any local mod");
	}

	// mod updates
	{
		Util.RawHtml("<h1>Mod updates</h1>").Dump();
		this.GetModsOnCompatibilityListUpdatedSince(modPages, compatList, ListModsUpdatedSince).Dump($"Mod files on compatibility list uploaded since {ListModsUpdatedSince:yyyy-MM-dd HH:mm}");
	}

	// stats
	{
		Util.RawHtml("<h1>Stats</h1>").Dump();
		this.GetOpenSourceStats(compatList).Dump("open-source stats");
		this.GetModTypes(modPages).Dump("mod types");
		DumpDictionaryToColumns(this.GetContentPatcherVersionUsage(modPages).Dump("Content Patcher packs by format version"), "Content Patcher packs by format version (row)");
	}
}


/*********
** Common queries
*********/
/// <summary>Get SMAPI mods which aren't listed on the mod compatibility list.</summary>
/// <param name="mods">The mods to check.</param>
/// <param name="compatList">The mod data from the mod compatibility list.</param>
IEnumerable<dynamic> GetModsNotOnCompatibilityList(IEnumerable<ModPageRecord> mods, ModCompatibilityEntry[] compatList)
{
	// fetch mods on the compatibility list
	ISet<string> manifestIDs = new HashSet<string>(compatList.SelectMany(p => p.ID), StringComparer.InvariantCultureIgnoreCase);
	Dictionary<ModSite, ISet<long>> siteIDs = new()
	{
		[ModSite.CurseForge] = new HashSet<long>(compatList.Where(p => p.CurseForgeID.HasValue).Select(p => p.CurseForgeID.Value)),
		[ModSite.ModDrop] = new HashSet<long>(compatList.Where(p => p.ModDropID.HasValue).Select(p => p.ModDropID.Value)),
		[ModSite.Nexus] = new HashSet<long>(compatList.Where(p => p.NexusID.HasValue).Select(p => p.NexusID.Value))
	};

	// fetch report
	return (
		from modPage in mods
		from download in modPage.Downloads
		from mod in download.Mods
		orderby modPage.Name

		where
			mod.Type == ModType.Smapi
			&& !string.IsNullOrWhiteSpace(mod.Id)
			&& !this.ShouldIgnoreForAnalysis(modPage.Site, modPage.Id, download.Id, mod.Id)

		let compatHasManifestId = manifestIDs.Contains(mod.Id)
		let compatHasSiteId = siteIDs.GetValueOrDefault(modPage.Site)?.Contains(modPage.Id) is true

		where (!compatHasManifestId || !compatHasSiteId)

		let manifest = mod.Manifest
		let names = this.GetModNames(mod, modPage)
		let authorNames = this.GetAuthorNames(manifest, modPage)
		let githubRepos = this.GetGitHubRepos(manifest, modPage)
		let customSourceUrls = this.GetCustomSourceUrls(manifest, modPage)

		let isModInstalled = Directory.Exists(Path.Combine(InstallModsToPath, mod.Id))

		let missingLabels = (new[] { !compatHasManifestId ? "manifest ID" : null, !compatHasSiteId ? "site ID" : null }).Where(p => p is not null).ToArray()

		select new
		{
			SitePage = new Hyperlinq(modPage.PageUrl, $"{modPage.Site}:{modPage.Id}"),
			SiteName = modPage.Name,
			SiteAuthor = modPage.AuthorLabel != null && modPage.AuthorLabel != modPage.Author
				? $"{modPage.Author}\n({modPage.AuthorLabel})"
				: modPage.Author,
			SiteVersion = SemanticVersion.TryParse(modPage.Version, out ISemanticVersion siteVersion) ? siteVersion.ToString() : modPage.Version,
			FileName = mod.DisplayName,
			FileCategory = mod.Type,
			ModId = mod.Id,
			ModVersion = mod.Manifest?.Version,
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
							Util.VerticalRun(this.ModCacheHelper.TryInstall(modPage, download, mod, out _, deleteTargetFolder: false)),
							"font-style: monospace; font-size: 0.9em;"
						)
					}
				),
			Metadata = Util.OnDemand("expand", () => new
			{
				FileId = download.Id,
				FileType = download.Type,
				UpdateKeys = Util.OnDemand("expand", () => this.GetUpdateKeys(manifest)),
				Manifest = Util.OnDemand("expand", () => manifest),
				Mod = Util.OnDemand("expand", () => modPage),
				Folder = Util.OnDemand("expand", () => mod)
			}),
			CompatEntry = new Lazy<string>(() => // can't be in Metadata since it's accessed by the main script
				BuildCompatibilityEntry(modPage, manifest, names, authorNames, githubRepos, customSourceUrls)
			)
		}
	)
	.ToArray();

	static string BuildCompatibilityEntry(ModPageRecord mod, ModManifestRecord? manifest, string[] names, string[] authorNames, HashSet<string> githubRepos, HashSet<string> customSourceUrls)
	{
		// build JSON
		bool hasMultipleSourceUrls = (githubRepos.Count + customSourceUrls.Count) > 1;
		string json = JsonSerializer.Serialize(
			new
			{
				name = string.Join(", ", names),
				author = string.Join(", ", authorNames),
				id = manifest?.UniqueId,
				curse = mod.Site == ModSite.CurseForge ? mod.Id : null as long?,
				moddrop = mod.Site == ModSite.ModDrop ? mod.Id : null as long?,
				nexus = mod.Site == ModSite.Nexus ? mod.Id : null as long?,
				github = FormatSourceField(githubRepos),
				source = FormatSourceField(customSourceUrls)
			},
			new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }
		);

		// remove empty optional fields
		json = Regex.Replace(json, @"^\s*""(?:curse|moddrop|source)"": null,?" + Environment.NewLine, "", RegexOptions.Multiline);
		json = Regex.Replace(json, $",({Environment.NewLine}}})", "$1");

		// indent to match JSON file
		json = Regex.Replace(json, @"^([\{\}])", "\t\t$1", RegexOptions.Multiline);
		json = Regex.Replace(json, @"^  """, "\t\t\t\"", RegexOptions.Multiline);

		return json + ",";

		string FormatSourceField(HashSet<string> entries)
		{
			string field = entries.Count > 0
				? string.Join(", ", entries)
				: null;

			if (hasMultipleSourceUrls && entries.Count > 0)
				field += " # TODO: choose one";

			return field;
		}
	}
}

/// <summary>Get SMAPI mods with a source URL which isn't on the mod compatibility list.</summary>
/// <param name="modPages">The mod pages to check.</param>
/// <param name="compatList">The mod data from the mod compatibility list.</param>
IEnumerable<dynamic> GetModsWithSourceNotOnCompatList(IEnumerable<ModPageRecord> modPages, ModCompatibilityEntry[] compatList)
{
	Dictionary<string, ModCompatibilityEntry> compatById = this.GetCompatibilityEntriesByModId(compatList);

	const string oldStyle = "text-decoration: line-through;";
	const string newStyle = "font-weight: bold;";
	object FormatLink(string url, string text, string format)
	{
		return url != null
			? Util.WithStyle(new Hyperlinq(url, text), format)
			: "";
	}
	return (
		from modPage in modPages
		from download in modPage.Downloads
		from mod in download.Mods
		orderby modPage.Name

		where
			mod.Type == ModType.Smapi
			&& !string.IsNullOrWhiteSpace(mod.Id)
			&& !this.ShouldIgnoreForAnalysis(modPage.Site, modPage.Id, download.Id, mod.Id)

		let compatEntry = compatById.GetValueOrDefault(mod.Id)
		where compatEntry is not null

		let manifest = mod.Manifest
		let githubRepos = this.GetGitHubRepos(manifest, modPage)
		let customSourceUrls = this.GetCustomSourceUrls(manifest, modPage)

		let githubRepoMismatch =
			githubRepos.Count > 0
			&& (
				string.IsNullOrWhiteSpace(compatEntry.GitHubRepo?.Trim())
				|| !githubRepos.Contains(compatEntry.GitHubRepo.Trim())
			)
		let customSourceMismatch =
			customSourceUrls.Count > 0
			&& (
				string.IsNullOrWhiteSpace(compatEntry.CustomSourceUrl?.Trim())
				|| !customSourceUrls.Contains(compatEntry.CustomSourceUrl.Trim())
			)

		where githubRepoMismatch || customSourceMismatch

		select new
		{
			ModId = mod.Id,
			SitePage = new Hyperlinq(modPage.PageUrl, $"{modPage.Site}:{modPage.Id}"),
			SiteName = modPage.Name,
			SiteAuthor = modPage.AuthorLabel != null && modPage.AuthorLabel != modPage.Author
				? $"{modPage.Author}\n({modPage.AuthorLabel})"
				: modPage.Author,
			FileName = mod.DisplayName,
			FileCategory = mod.Type,
			GithubRepo = githubRepoMismatch
				? Util.VerticalRun([
					FormatLink(compatEntry.GitHubRepo != null ? $"https://github.com/{compatEntry.GitHubRepo}" : null, compatEntry.GitHubRepo, oldStyle),
					..githubRepos.Select(repo => FormatLink($"https://github.com/{repo}", repo, newStyle))
				])
				: "",
			CustomSourceUrl = customSourceMismatch
				? Util.VerticalRun([
					FormatLink(compatEntry.CustomSourceUrl, compatEntry.CustomSourceUrl, oldStyle),
					..customSourceUrls.Select(sourceUrl => FormatLink(sourceUrl, sourceUrl, newStyle))
				])
				: "",
			Metadata = Util.OnDemand("expand", () => new
			{
				FileId = download.Id,
				FileType = mod.Type,
				UpdateKeys = Util.OnDemand("expand", () => this.GetUpdateKeys(manifest)),
				Manifest = Util.OnDemand("expand", () => manifest),
				SitePage = Util.OnDemand("expand", () => modPage),
				Mod = Util.OnDemand("expand", () => mod)
			}),
		}
	)
	.ToArray();
}

/// <summary>Get SMAPI mods which are marked deleted or hidden on the mod compatibility list, but which were found on a mod site.</summary>
/// <param name="modPages">The mod pages to check.</param>
/// <param name="compatList">The mod data from the mod compatibility list.</param>
IEnumerable<dynamic> GetModsMarkedHiddenWhichAreNot(IEnumerable<ModPageRecord> modPages, ModCompatibilityEntry[] compatList)
{
	Dictionary<string, ModCompatibilityEntry> compatById = this.GetCompatibilityEntriesByModId(compatList);

	return (
		from modPage in modPages
		from download in modPage.Downloads
		from mod in download.Mods
		orderby modPage.Name

		where
			!string.IsNullOrWhiteSpace(mod.Id)
			&& !this.ShouldIgnoreForAnalysis(modPage.Site, modPage.Id, download.Id, mod.Id)
			&& compatById.TryGetValue(mod.Id, out ModCompatibilityEntry compatEntry)
			&& compatEntry.Compatibility is { Status: ModCompatibilityStatus.Abandoned, AbandonedReason: ModCompatibilityReasonAbandoned.Deleted or ModCompatibilityReasonAbandoned.Hidden }

		let manifest = mod.Manifest

		select new
		{
			ModId = mod.Id,
			SitePage = new Hyperlinq(modPage.PageUrl, $"{modPage.Site}:{modPage.Id}"),
			SiteName = modPage.Name,
			SiteAuthor = modPage.AuthorLabel != null && modPage.AuthorLabel != modPage.Author
				? $"{modPage.Author}\n({modPage.AuthorLabel})"
				: modPage.Author,
			FileName = mod.DisplayName,
			FileCategory = mod.Type,
			Metadata = Util.OnDemand("expand", () => new
			{
				FileId = download.Id,
				FileType = mod.Type,
				UpdateKeys = Util.OnDemand("expand", () => this.GetUpdateKeys(manifest)),
				Manifest = Util.OnDemand("expand", () => manifest),
				SitePage = Util.OnDemand("expand", () => modPage),
				Mod = Util.OnDemand("expand", () => mod)
			}),
		}
	)
	.ToArray();
}

/// <summary>Get SMAPI mods on the compatibility list which are actually content packs.</summary>
/// <param name="modPages">The mod pages to check.</param>
/// <param name="compatList">The mod data from the compatibility list.</param>
IEnumerable<dynamic> GetModsWhichAreContentPacks(IEnumerable<ModPageRecord> modPages, ModCompatibilityEntry[] compatList)
{
	// get lookup of mods by ID
	Dictionary<string, (ModPageRecord Mod, ModFolderRecord Folder)> modsById = new(StringComparer.OrdinalIgnoreCase);
	foreach (ModPageRecord modPage in modPages)
	{
		foreach (ModPageDownloadRecord download in modPage.Downloads)
		{
			foreach (ModFolderRecord mod in download.Mods)
			{
				if (string.IsNullOrWhiteSpace(mod.Id))
					continue;

				modsById[mod.Id] = (modPage, mod);
			}
		}
	}

	// list mods
	foreach (ModCompatibilityEntry entry in compatList)
	{
		// skip: deliberate content pack
		if (entry.ContentPackFor != null)
			continue;

		// skip: mod not found (handled separately)
		(ModPageRecord modPage, ModFolderRecord mod) = entry.ID.Select(p => modsById.GetValueOrDefault(p)).FirstOrDefault(p => p.Mod != null);
		if (modPage is null)
			continue;

		// skip: has a C# mod
		if (mod.Type is ModType.Smapi)
			continue;

		// return match
		string url = entry.GetModPageUrls().FirstOrDefault().Value;
		string linkText = $"{modPage.Site}:{modPage.Id}";
		yield return new
		{
			Link = url != null
				? new Hyperlinq(url, linkText)
				: (object)linkText,
			Mod =
				$"{modPage.Name}\n   by "
				+ (modPage.AuthorLabel != null && modPage.AuthorLabel != modPage.Author
					? $"{modPage.Author} ({modPage.AuthorLabel})"
					: modPage.Author
				),
			Metadata = Util.OnDemand("expand", () => new
			{
				Mod = modPage,
				Folder = mod,
				Entry = entry
			})
		};
	}

	yield break;
}

/// <summary>Get SMAPI mods on the compatibility list which have been updated recently.</summary>
/// <param name="modPages">The mod pages to check.</param>
/// <param name="compatList">The mod data from the compatibility list.</param>
/// <param name="updatedSince">The earliest update date for which to list mods.</param>
IEnumerable<dynamic> GetModsOnCompatibilityListUpdatedSince(IEnumerable<ModPageRecord> modPages, ModCompatibilityEntry[] compatList, DateTimeOffset updatedSince)
{
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
		from modPage in modPages
		from download in modPage.Downloads
		from mod in download.Mods

		let compatEntry = compatEntries.GetValueOrDefault($"{modPage.Site}:{modPage.Id}")
		let compat = compatEntry?.Compatibility

		where
			compatEntry != null
			&& download.Uploaded >= updatedSince
			&& !this.ShouldIgnoreForAnalysis(modPage.Site, modPage.Id, download.Id, mod.Id)

		let manifest = mod.Manifest
		let isModInstalled = mod.Id != null && Directory.Exists(Path.Combine(InstallModsToPath, mod.Id))

		let highlightType = mod.Type is not (ModType.Smapi or ModType.ContentPack)
		let highlightStatus = compat is null || compat.Status is not (ModCompatibilityStatus.Ok or ModCompatibilityStatus.Optional)

		orderby
			(highlightType || highlightStatus) descending, // mods with issues first
			modPage.Name

		select new
		{
			Link = new Hyperlinq(modPage.PageUrl, $"{modPage.Site}:{modPage.Id}"),
			Mod =
				$"{modPage.Name}\n   by "
				+ (modPage.AuthorLabel != null && modPage.AuthorLabel != modPage.Author
					? $"{modPage.Author} ({modPage.AuthorLabel})"
					: modPage.Author
				),
			FileUpdated = download.Uploaded.ToString("yyyy-MM-dd"),
			File = $"{download.DisplayName} {download.Version}",
			FileCategory = mod.Type,
			ModType = Util.WithStyle(mod.Type, highlightType ? ConsoleHelper.ErrorStyle : ""),
			Summary =
			compatEntry != null
				? Util.WithStyle($"{compat.Summary} {(!string.IsNullOrWhiteSpace(compat.BrokeIn) ? $"[broke in {compat.BrokeIn}]" : "")}".Trim(), $"{smallStyle} {(highlightStatus ? ConsoleHelper.ErrorStyle : "")}")
				: Util.WithStyle($"not found on compatibility list", ConsoleHelper.ErrorStyle),
			mod.Id,
			ModVersion = mod.Manifest?.Version,
			Actions = mod.Id != null
				? Util.HorizontalRun(true,
					isModInstalled
						? "installed"
						: (object)Util.OnDemand(
							"install",
							() => new object[] // returning an array allows collapsing the log in the LINQPad output
							{
								Util.WithStyle(
									Util.VerticalRun(this.ModCacheHelper.TryInstall(modPage, download, mod, out _, deleteTargetFolder: false)),
									"font-style: monospace; font-size: 0.9em;"
								)
							}
						),
					new Hyperlinq(this.ModCacheHelper.GetModDumpFolder(modPage, download, mod), "files")
				)
				: null,
			Metadata = Util.OnDemand("expand", () => new
			{
				FileId = download.Id,
				UpdateKeys = Util.OnDemand("expand", () => this.GetUpdateKeys(manifest)),
				Manifest = Util.OnDemand("expand", () => manifest),
				Mod = Util.OnDemand("expand", () => modPage),
				Folder = Util.OnDemand("expand", () => mod)
			})
		}
	)
	.ToArray();
}

/// <summary>Get SMAPI mods listed on the mod compatibility list which don't exist in the open mod dataset, so they were probably hidden or deleted. This excludes mods marked abandoned on the compatibility list.</summary>
/// <param name="modPages">The mod pages to check.</param>
/// <param name="mods">The mod data from the mod compatibility list.</param>
IEnumerable<dynamic> GetCompatibilityListModsNotInRepo(ModPageRecord[] modPages, ModCompatibilityEntry[] compatList)
{
	ModToolkit toolkit = new();

	Dictionary<ModSite, HashSet<long>> knownIds = modPages
		.GroupBy(page => page.Site)
		.ToDictionary(
			pages => pages.Key,
			pages => new HashSet<long>(pages.Select(page => page.Id))
		);

	HashSet<string> missingPages = new(StringComparer.OrdinalIgnoreCase);
	foreach (ModCompatibilityEntry mod in compatList)
	{
		if (mod.Compatibility.Status is ModCompatibilityStatus.Abandoned or ModCompatibilityStatus.Obsolete)
			continue;

		missingPages.Clear();
		if (mod.CurseForgeID.HasValue && knownIds.GetValueOrDefault(ModSite.CurseForge)?.Contains(mod.CurseForgeID.Value) is not true)
			missingPages.Add($"{ModSite.CurseForge}:{mod.CurseForgeID}");
		if (mod.ModDropID.HasValue && knownIds.GetValueOrDefault(ModSite.ModDrop)?.Contains(mod.ModDropID.Value) is not true)
			missingPages.Add($"{ModSite.ModDrop}:{mod.ModDropID}");
		if (mod.NexusID.HasValue && knownIds.GetValueOrDefault(ModSite.Nexus)?.Contains(mod.NexusID.Value) is not true)
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
/// <param name="modPages">The mod pages to check.</param>
IEnumerable<dynamic> GetInvalidMods(IEnumerable<ModPageRecord> modPages)
{
	return (
		from modPage in modPages

		let invalid =
			(
				from download in modPage.Downloads
				from mod in download.Mods
				where
					mod.Type == ModType.Invalid
				&& mod.ManifestParseError != ModParseError.ManifestMissing // ignore non-mod files
				&& mod.ManifestParseError != ModParseError.EmptyFolder // contains only non-mod files (e.g. replacement PNG assets)
				&& !this.ShouldIgnoreForAnalysis(modPage.Site, modPage.Id, download.Id, mod.Id)
				select (Download: download, Mod: mod)
			)
			.ToArray()

		where invalid.Any()
		select new
		{
			modPage.Name,
			modPage.Author,
			modPage.Version,
			modPage.Updated,
			SitePage = new Hyperlinq(modPage.PageUrl, $"{modPage.Site}:{modPage.Id}"),
			Data = new Lazy<object>(() => modPage),
			InvalidFile = invalid.Select(entry => new
			{
				FileId = entry.Download.Id,
				FileType = entry.Mod.Type,
				entry.Mod.DisplayName,
				entry.Download.Version,
				entry.Mod.Type,
				entry.Mod.ManifestParseError,
				Data = new Lazy<object>(() => entry),
				Manifest = new Lazy<string>(() =>
				{
					string modPath = this.ModCacheHelper.GetModDumpFolder(modPage, entry.Download, entry.Mod);
					FileInfo file = new FileInfo(Path.Combine(modPath, "manifest.json"));
					return file.Exists
						? File.ReadAllText(file.FullName)
						: "<file not found>";
				}),
				ManifestError = new Lazy<string>(() => $"{entry.Mod.ManifestParseError}\n{entry.Mod.ManifestParseErrorText}"),
				FileList = new Lazy<string>(() =>
				{
					string modPath = this.ModCacheHelper.GetModDumpFolder(modPage, entry.Download, entry.Mod);
					return this.BuildFileList(new DirectoryInfo(modPath));
				})
			})
		}
	)
	.ToArray();
}

/// <summary>Get entries in <see cref="IgnoreForAnalysis" /> which don't match any of the given mods.</summary>
/// <param name="modPages">The mod pages to check.</param>
IEnumerable<dynamic> GetInvalidIgnoreModEntries(IEnumerable<ModPageRecord> modPages)
{
	// index known mods
	IDictionary<string, ModPageRecord> modPagesByKey = modPages.ToDictionary(mod => $"{mod.Site}:{mod.Id}", StringComparer.OrdinalIgnoreCase);

	// show unknown entries
	var invalid = new List<(ModSearch Entry, string Reason, ModPageRecord Mod)>();
	foreach (var pair in this.IgnoreForAnalysisBySiteId)
	{
		(string key, ModSearch[] entries) = pair;

		// fetch mod
		if (!modPagesByKey.TryGetValue(key, out ModPageRecord modPage))
		{
			foreach (var entry in entries)
				invalid.Add((entry, "Site ID not found", modPage));
			continue;
		}

		// match against mod folders
		HashSet<long> fileIds = new(modPage.Downloads.Select(p => p.Id));
		foreach (var entry in entries)
		{
			if (entry.FileId.HasValue && !fileIds.Contains(entry.FileId.Value))
				invalid.Add((entry, "File ID not found", modPage));
			else
			{
				bool found = false;
				foreach (var download in modPage.Downloads)
				{
					foreach (var mod in download.Mods)
					{
						found = entry.Matches(site: modPage.Site, siteId: modPage.Id, fileId: download.Id, manifestId: mod.Id);
						if (found)
							break;
					}
				}

				if (!found)
					invalid.Add((entry, "Mod folder data not matched", modPage));
			}
		}
	}

	return invalid
		.Select(p => new { p.Entry.Site, p.Entry.SiteId, p.Entry.FileId, p.Entry.ManifestId, Reason = p.Reason, Mod = new Lazy<ModPageRecord>(() => p.Mod), Entry = new Lazy<ModSearch>(() => p.Entry) })
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
/// <param name="modPages">The mod pages to check.</param>
IDictionary<string, int> GetModTypes(IEnumerable<ModPageRecord> modPages)
{
	const int minPerGroup = 100;

	// get mod id => name lookup
	IDictionary<string, string> namesById = modPages
		.SelectMany(p => p.Downloads)
		.SelectMany(p => p.Mods)
		.Select(p => new { Id = p.Id?.Trim(), Name = p.DisplayName })
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
	foreach (ModPageRecord modPage in modPages)
	{
		// get all the mods available to download from this page (including downloads which contain multiple submods)
		var submods =
			(
				from download in modPage.Downloads
				from mod in download.Mods
				where mod.Type is not (ModType.Ignored or ModType.Invalid)
	
				let contentPackFor = mod.Manifest?.ContentPackFor?.UniqueId
				let type = mod.Type switch
				{
					ModType.Smapi => "SMAPI",
					ModType.ContentPack => !string.IsNullOrWhiteSpace(contentPackFor) && namesById.TryGetValue(contentPackFor.Trim(), out string name)
						? $"content pack ({name})"
						: $"content pack ({contentPackFor?.Trim()})",
					ModType.Xnb => "XNB",
					_ => $"other ({mod.Type})"
				}

				orderby GetPriority(type)
				select (mod, type)
			)
			.ToArray();
		if (!submods.Any())
			continue;

		// count submods by type
		bool hasNonXnb = submods.Any(p => p.type != "XNB");
		foreach ((ModFolderRecord mod, string type) in submods)
		{
			// special case: if the mod has both XNB and non-XNB components, ignore the XNB ones (they're generally old/alternative versions)
			if (type == "XNB" && hasNonXnb)
				continue;

			// get tracking key
			string key = !string.IsNullOrWhiteSpace(mod.Id)
				? mod.Id.Trim()
				: $"{type}:{modPage.Site}:{modPage.Id}";

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
/// <param name="modPages">The mod pages to check.</param>
IDictionary<string, int> GetContentPatcherVersionUsage(IEnumerable<ModPageRecord> modPages)
{
	// get unique versions by content pack ID
	var contentTemplate = new { Format = "" };
	var modVersions = new Dictionary<string, ISemanticVersion>(StringComparer.OrdinalIgnoreCase);
	foreach (ModPageRecord modPage in modPages)
	{
		foreach (ModPageDownloadRecord download in modPage.Downloads)
		{
			foreach (ModFolderRecord mod in download.Mods)
			{
				// parse manifest
				ModManifestRecord manifest = mod.Manifest;
				string id = manifest?.UniqueId?.Trim();
				string contentPackFor = manifest?.ContentPackFor?.UniqueId?.Trim();
				if (string.IsNullOrWhiteSpace(id) || !string.Equals(contentPackFor, "Pathoschild.ContentPatcher", StringComparison.OrdinalIgnoreCase))
					continue;

				// skip if content.json doesn't exist
				FileInfo contentFile = new FileInfo(
					Path.Combine(
						this.ModCacheHelper.GetModDumpFolder(modPage, download, mod),
						"content.json"
					)
				);
				if (!contentFile.Exists)
					continue;

				// extract format version
				ISemanticVersion format = null;
				try
				{
					var rawContent = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(File.ReadAllText(contentFile.FullName), contentTemplate);
					if (!SemanticVersion.TryParse(rawContent?.Format, out format))
						continue;

					format = new SemanticVersion(format.MajorVersion, format.MinorVersion, 0);
				}
				catch (Newtonsoft.Json.JsonException)
				{
					continue; // ignore invalid content.json
				}

				// track latest version
				if (!modVersions.TryGetValue(id, out ISemanticVersion prevVersion) || format.IsNewerThan(prevVersion))
					modVersions[id] = format;
			}
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
/// <param name="fuzzyDays">The day offset to apply to the date.</param>
private static DateTimeOffset GetStartOfMonth(int fuzzyDays = 5)
{
	DateTimeOffset now = DateTimeOffset.Now;

	return new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, 0, now.Offset)
		.AddMonths(now.Day <= fuzzyDays ? -1 : 0);
}

/// <summary>Load all mod page records from the open mod dataset.</summary>
/// <param name="repoPath">The full path to the open mod dataset repo.</param>
private static IEnumerable<ModPageRecord> LoadModsFromRepo(string repoPath)
{
	string dataPath = Path.Combine(repoPath, "data");

	var progress = new Util.ProgressBar().Dump();

	foreach (string siteDir in Directory.EnumerateDirectories(dataPath))
	{
		if (!Enum.TryParse(Path.GetFileName(siteDir), out ModSite site))
		{
			Console.WriteLine($"  Ignored site dir at '{siteDir}': folder name isn't a known site.");
			continue;
		}

		string[] jsonFiles = Directory.EnumerateFiles(siteDir, "*.json", SearchOption.AllDirectories).ToArray();

		int i = 0;
		foreach (string jsonFile in jsonFiles)
		{
			string relativePath = Path.GetRelativePath(repoPath, jsonFile);

			progress.Fraction = i++ / ((float)jsonFiles.Length);
			progress.Caption = $"Reading {relativePath}...";

			ModPageRecord modPage;
			try
			{
				using Stream fileStream = File.OpenRead(jsonFile);
				modPage = JsonSerializer.Deserialize<ModPageRecord>(fileStream);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"  Ignored mod file at '{relativePath}': can't deserialize data.\n{ex}");
				continue;
			}

			yield return modPage;
		}
	}

	progress.HideWhenCompleted = true;
	progress.Fraction = 1;
}

/// <summary>Get the human-readable mod names for the compatibility list.</summary>
/// <param name="folder">The downloaded mod folder.</param>
/// <param name="mod">The mod metadata.</param>
private string[] GetModNames(ModFolderRecord folder, ModPageRecord mod)
{
	// get possible names
	string[] names = new[] { folder.DisplayName?.Trim(), mod.Name?.Trim() }
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
private string[] GetAuthorNames(ModManifestRecord? manifest, ModPageRecord mod)
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
private HashSet<string> GetGitHubRepos(ModManifestRecord? manifest, ModPageRecord mod)
{
	HashSet<string> repos = new(StringComparer.OrdinalIgnoreCase);

	// from update key
	foreach (string rawUpdateKey in this.GetUpdateKeys(manifest))
	{
		string updateKey = rawUpdateKey?.Trim();
		if (updateKey?.StartsWith("GitHub", StringComparison.OrdinalIgnoreCase) == true)
		{
			Match match = Regex.Match(updateKey, @"^GitHub\s*:\s*(.+)");
			if (match.Success)
			{
				string repo = this.MapSourceLink(manifest, match.Groups[1].Value);
				if (repo is not null)
					repos.Add(repo);
			}
		}
	}

	// from mod description
	if (!string.IsNullOrWhiteSpace(mod.Description))
	{
		MatchCollection matches = Regex.Matches(mod.Description, @"(?<!help\.|gist\.)github\.com/([a-z0-9_\-\.]+/[a-z0-9_\-\.]+)", RegexOptions.IgnoreCase);
		foreach (Match match in matches)
		{
			string repo = this.MapSourceLink(manifest, match.Groups[1].Value);
			if (repo is not null)
				repos.Add(repo);
		}
	}

	return repos;
}

/// <summary>Get the custom source code URL for a mod, if available.</summary>
/// <param name="manifest">The downloaded mod manifest file.</param>
/// <param name="mod">The mod metadata.</param>
private HashSet<string> GetCustomSourceUrls(ModManifestRecord? manifest, ModPageRecord mod)
{
	HashSet<string> sourceUrls = new(StringComparer.OrdinalIgnoreCase);

	// from mod description
	if (!string.IsNullOrWhiteSpace(mod.Description))
	{
		MatchCollection matches = Regex.Matches(mod.Description, @"(gitlab\.com/[a-z0-9_\-\.]+/[a-z0-9_\-\.]+|sourceforge\.net/p/[a-z0-9_\-\.]+)", RegexOptions.IgnoreCase);
		foreach (Match match in matches)
		{
			string url = this.MapSourceLink(manifest, $"https://{match.Groups[1].Value}");
			if (url is not null)
				sourceUrls.Add(url);
		}
	}

	return sourceUrls;
}

/// <summary>Get the source link for a mod after applying the <see cref="ModOverridesData.IgnoreSourceLinks"/>, <see cref="ModOverridesData.IgnoreSourceLinksForSpecificMods"/>, and <see cref="ModOverridesData.MapSourceLinks"/> patterns.</summary>
/// <param name="manifest">The mod manifest.</param>
/// <param name="repoOrUrl">The GitHub repo name (like 'Pathoschild/SMAPI') or custom source URL to map.</param>
/// <returns>Returns the source link to use, or <c>null</c> if it should be ignored.</returns>
private string? MapSourceLink(ModManifestRecord? manifest, string repoOrUrl)
{
	if (string.IsNullOrWhiteSpace(repoOrUrl))
		return null;

	repoOrUrl = repoOrUrl.Trim().TrimEnd('/').TrimEnd();

	// strip .git suffix
	if (repoOrUrl.EndsWith(".git", true, null))
		repoOrUrl = repoOrUrl.Substring(0, repoOrUrl.Length - 4).TrimEnd();

	// apply overrides
	repoOrUrl = this.ModOverrides.MapSourceLinks.GetValueOrDefault(repoOrUrl) ?? repoOrUrl;
	if (this.ModOverrides.IgnoreSourceLinks.Contains(repoOrUrl))
		return null;
	if (manifest?.UniqueId != null && this.ModOverrides.IgnoreSourceLinksForSpecificMods.GetValueOrDefault(manifest.UniqueId.Trim())?.Contains(repoOrUrl) is true)
		return null;

	return repoOrUrl;
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

/// <summary>Get a lookup of mod compatibility entries by mod ID.</summary>
/// <param name="compatList">The compatibility entries to index.</param>
private Dictionary<string, ModCompatibilityEntry> GetCompatibilityEntriesByModId(ModCompatibilityEntry[] compatList)
{
	Dictionary<string, ModCompatibilityEntry> entriesById = new(StringComparer.OrdinalIgnoreCase);
	foreach (ModCompatibilityEntry entry in compatList)
	{
		foreach (string id in entry.ID)
			entriesById[id] = entry;
	}
	return entriesById;
}

/// <summary>Get the update keys from a mod manifest.</summary>
/// <param name="manifest">The mod manifest.</param>
private IEnumerable<string> GetUpdateKeys(ModManifestRecord? manifest)
{
	return manifest?.UpdateKeys?.Distinct().Where(p => p is not null) ?? [];
}


/*********
** Override types
*********/
/// <summary>The manual overrides for specific mods or source repos when analyzing them with this script.</summary>
private class ModOverridesData
{
	/*********
	** Accessors
	*********/
	/// <summary>Mods to ignore when validating mods or compiling statistics.</summary>
	public ModSearch[] IgnoreForAnalysis { get; init; }

	/// <summary>The GitHub or custom source URLs to ignore globally when auto-detecting the source link for a mod.</summary>
	/// <remarks>Entries can be a GitHub repo name (like 'Pathoschild/StardewMods') or custom source URL.</remarks>
	public HashSet<string> IgnoreSourceLinks { get; init; }

	/// <summary>The GitHub or custom source URLs to ignore for specific mod IDs when auto-detecting the source link for a mod.</summary>
	/// <remarks>Entries can be a GitHub repo name (like 'Pathoschild/StardewMods') or custom source URL.</remarks>
	public Dictionary<string, HashSet<string>> IgnoreSourceLinksForSpecificMods { get; init; }

	/// <summary>The GitHub or custom source URLs which redirect to a new name.</summary>
	public Dictionary<string, string> MapSourceLinks { get; init; }


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
		RawDataModel rawData = Newtonsoft.Json.JsonConvert.DeserializeObject<RawDataModel>(json);

		// read ignore for analysis
		var ignoreForAnalysis = new List<ModSearch>();
		foreach ((string rawSiteKey, string[] entries) in rawData.IgnoreForAnalysis)
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

		// read 'ignore source links for specific mods'
		var ignoreSourceLinksForSpecificMods = new Dictionary<string, HashSet<string>>();
		foreach ((string modId, string rawRepos) in rawData.IgnoreSourceLinksForSpecificMods)
			ignoreSourceLinksForSpecificMods[modId] = new HashSet<string>(rawRepos.Split(',', StringSplitOptions.TrimEntries), StringComparer.OrdinalIgnoreCase);

		// read other fields
		var ignoreSourceLinks = new HashSet<string>(rawData.IgnoreSourceLinks, StringComparer.OrdinalIgnoreCase);
		var mapSourceLinks = new Dictionary<string, string>(rawData.MapSourceLinks, StringComparer.OrdinalIgnoreCase);

		// build model
		return new ModOverridesData
		{
			IgnoreForAnalysis = ignoreForAnalysis.ToArray(),
			IgnoreSourceLinks = ignoreSourceLinks,
			IgnoreSourceLinksForSpecificMods = ignoreSourceLinksForSpecificMods,
			MapSourceLinks = mapSourceLinks
		};
	}

	/// <summary>The raw data model for the overrides file.</summary>
	private class RawDataModel
	{
		public Dictionary<string, string[]> IgnoreForAnalysis;
		public Dictionary<string, string> IgnoreSourceLinksForSpecificMods;
		public string[] IgnoreSourceLinks;
		public Dictionary<string, string> MapSourceLinks;
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
