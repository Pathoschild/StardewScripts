<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.CoreInterfaces.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.dll</Reference>
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>Pathoschild.Http.FluentClient</NuGetReference>
  <Namespace>HtmlAgilityPack</Namespace>
  <Namespace>Pathoschild.Http.Client</Namespace>
  <Namespace>StardewModdingAPI</Namespace>
  <Namespace>StardewModdingAPI.Toolkit</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.CompatibilityRepo</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.WebApi</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModData</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModScanning</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.UpdateData</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Serialization</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Serialization.Models</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Utilities</Namespace>
  <Namespace>System.Globalization</Namespace>
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
/****
** Environment
****/
/// <summary>The absolute path for the folder containing mods.</summary>
private const string GameFolderPath = @"C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley";

/// <summary>The absolute path for the folder containing installed mods.</summary>
private static string InstalledModsPath => Path.Combine(GameFolderPath, "Mods (test)");

/// <summary>Provides higher-level utilities for working with the underlying mod cache.</summary>
private readonly ModCacheUtilities ModCacheHelper = new(@"E:\dev\mod-dump", InstalledModsPath);

/// <summary>If set, the full path to a local copy of the compatibility list repo to read directly instead of fetching it from the server.</summary>
const string LocalCompatListRepoPath = null;


/****
** Common settings
****/
/// <summary>The mod compatibility statuses to highlight as errors. Mainly useful when you have a set of mods you know work or don't work, and want to find errors in the compatibility list.</summary>
private readonly HashSet<ModCompatibilityStatus> HighlightStatuses = new HashSet<ModCompatibilityStatus>(
	// all statuses
	new[]
	{
		ModCompatibilityStatus.Ok, ModCompatibilityStatus.Optional, ModCompatibilityStatus.Unofficial, // OK
		ModCompatibilityStatus.Broken, ModCompatibilityStatus.Workaround, // broken
		ModCompatibilityStatus.Abandoned, ModCompatibilityStatus.Obsolete // abandoned
	}
	//.Except(new[] { ModCompatibilityStatus.Abandoned, ModCompatibilityStatus.Obsolete }) // if abandoned
	//.Except(new[] { ModCompatibilityStatus.Broken, ModCompatibilityStatus.Workaround }) // if broken
	.Except(new[] { ModCompatibilityStatus.Ok, ModCompatibilityStatus.Optional, ModCompatibilityStatus.Unofficial }) // if OK
);

/// <summary>Whether to perform update checks for mods installed locally.</summary>
public bool UpdateCheckLocal = true;

/// <summary>Whether to show installed mods not found on the compatibility list.</summary>
public bool ShowMissingCompatMods = true;

/// <summary>Whether to show mods which are required to load the current mods but which aren't installed locally.</summary>
public bool ShowMissingDependencies = true;

/// <summary>Whether to show mods on the compatibility list that aren't installed locally. This should be false in most cases.</summary>
public bool ShowMissingLocalMods = false;

/// <summary>Whether to show potential errors in the compatibility list.</summary>
public bool ShowCompatListErrors = true;

/****
** Mod exception lists
****/
/// <summary>Mod IDs, update keys, custom URLs, or entry DLLs to ignore when checking if a local mod is on the mod compatibility list.</summary>
/// <remarks>This should only be used when a mod can't be cross-referenced because it has no ID and isn't released anywhere valid that can be used as an update key.</summary>
public string[] IgnoreMissingCompatListMods = new[]
{
	// no ID
	"Nexus:450", // XmlSerializerRetool

	// Farm Automation
	"GitHub:oranisagu/SDV-FarmAutomation",
	"FarmAutomation.BarnDoorAutomation.dll",
	"FarmAutomation.ItemCollector.dll",

	// EvilPdor's mods
	"https://community.playstarbound.com/threads/111526",
	"RainRandomizer.dll",
	"StaminaRegen.dll",
	"WakeUp.dll",
	"WeatherController.dll",

	// local mods
	"Pathoschild.TestContentMod"
};

/// <summary>Mod IDs to ignore when checking if a compatibility list mod is installed locally.</summary>
public string[] IgnoreMissingLocalMods = new[]
{
	// bundled with SMAPI
	"SMAPI.ConsoleCommands",
	"SMAPI.SaveBackup",

	// no longer available for download
	"BALANCEMOD_AntiExhaustion", // Less Strict Over-Exertion by Permamiss: Nexus page hidden
	"ConfigurableShippingDates", // Configurable Shipping Dates by Nishtra: Nexus page hidden
	"Pathoschild.NoDebugMode"    // No Debug Mode by Pathoschild: Nexus page deleted
};

/// <summary>The mod versions to consider equivalent in update checks (indexed by mod ID to local/server versions).</summmary>
public IDictionary<string, Tuple<string, string>> EquivalentModVersions = new Dictionary<string, Tuple<string, string>>(StringComparer.InvariantCultureIgnoreCase)
{
	// broke in 1.2
	["439"] = Tuple.Create("1.2.1", "1.21.0"), // Almighty Tool
	["FileLoading"] = Tuple.Create("1.1.0", "1.12.0"), // File Loading
	["stephansstardewcrops"] = Tuple.Create("1.4.1", "1.41.0"), // Stephen's Stardew Crops

	// okay
	["alphablackwolf.skillPrestige"] = Tuple.Create("1.0.9-unofficial.1-huancz", "1.2.3"), // Skill Prestige (unofficial update is for an older version)
	["Jotser.AutoGrabberMod"] = Tuple.Create("1.0.12-beta.1", "1.0.12"),
	["skuldomg.freeDusty"] = Tuple.Create("1.0-beta.7", "1.0.5"), // Free Dusty
	["ElectroCrumpet.PelicanPostalService"] = Tuple.Create("1.0.5-beta", "1.0.6") // Pelican Postal Service
};

/****
** State
****/
/// <summary>The SMAPI mod toolkit.</summary>
readonly ModToolkit ModToolkit = new();


/*********
** Script
*********/
async Task Main()
{
	/****
	** Add launch button
	****/
	new Hyperlinq(
		() => Process.Start(
			fileName: Path.Combine(GameFolderPath, "StardewModdingAPI.exe"),
			arguments: @$"--mods-path ""{InstalledModsPath}"""
		),
		"launch SMAPI"
	).Dump("actions");

	/****
	** Initialize
	****/
	Console.WriteLine("Initialising...");

	// data
	var toolkit = this.ModToolkit;
	var mods = new List<ModData>();

	/****
	** Read local mod data
	****/
	Console.WriteLine("Reading local data...");
	foreach (ModFolder folder in toolkit.GetModFolders(InstalledModsPath, useCaseInsensitiveFilePaths: true))
	{
		if (folder.Manifest == null)
		{
			Util.WithStyle($"   Ignored invalid mod: {folder.DisplayName} (manifest error: {folder.ManifestParseError})", "color: red;").Dump();
			continue;
		}
		mods.Add(new ModData(folder));
	}

	/****
	** Match to API data
	****/
	Console.WriteLine("Fetching API data...");
	{
		// get valid mods by ID
		IDictionary<string, ModData> fetchQueue = new Dictionary<string, ModData>(StringComparer.InvariantCultureIgnoreCase);
		{
			IDictionary<ModData, string> issues = new Dictionary<ModData, string>();
			int fakeIDs = 0;
			foreach (ModData mod in mods.Where(p => p.IsInstalled).OrderBy(p => p.GetDisplayName()))
			{
				// get unique ID
				string id = mod.Folder.Manifest.UniqueID;
				if (string.IsNullOrWhiteSpace(id))
				{
					id = $"SMAPI.FAKE.{fakeIDs++}";
					issues[mod] = $"has no ID (will use '{id}')";
				}

				// validate
				if (fetchQueue.ContainsKey(id))
				{
					issues[mod] = $"can't fetch API data: same ID as {fetchQueue[id].GetDisplayName()} (in '{fetchQueue[id].Folder.Directory.Name}' folder)";
					continue;
				}

				// can check for updates
				fetchQueue.Add(id, mod);
			}

			// show update-check issues
			if (issues.Any())
			{
				(
					from pair in issues
					let mod = pair.Key
					let issue = pair.Value
					let name = mod.GetDisplayName()
					orderby name
					select new
					{
						DisplayName = name,
						Folder = mod.Folder.DisplayName,
						Issue = issue
					}
				).Dump("found update-check issues for some mods");
			}
		}

		// fetch API data
		if (fetchQueue.Any())
		{
			IncrementalProgressBar progress = new IncrementalProgressBar(fetchQueue.Count) { HideWhenCompleted = true }.Dump();

			ISemanticVersion apiVersion = new SemanticVersion("4.0.0");
			WebApiClient client = new WebApiClient("https://smapi.io/api/", apiVersion);
			foreach (var pair in fetchQueue)
			{
				string id = pair.Key;
				var mod = pair.Value;

				progress.Increment();

				// fetch data
				Platform platform = EnvironmentUtility.DetectPlatform();
				ModSearchEntryModel searchModel = new ModSearchEntryModel(id, apiVersion, mod.UpdateKeys, isBroken: true); // isBroken ensures unofficial updates always listed
				ModEntryModel result = (await client.GetModInfoAsync(new[] { searchModel }, apiVersion, gameVersion: null, platform, includeExtendedMetadata: true)).Select(p => p.Value).FirstOrDefault();

				// select latest version
				mod.ApiRecord = result;
			}
		}
	}

	/****
	** Highlight potential compat list errors
	****/
	if (this.ShowCompatListErrors)
	{
		Console.WriteLine("Checking for potential compatibility list errors...");
		bool EqualsInvariant(string actual, string expected)
		{
			if (actual == null)
				return expected == null;
			return actual.Equals(expected, StringComparison.InvariantCultureIgnoreCase);
		}

		object Format(string actual, string expected)
		{
			return Util.WithStyle(
				actual ?? "null",
				EqualsInvariant(actual, expected) ? "color: green;" : "color: red; font-weight: bold;"
			);
		}

		var result = (
			from mod in mods
			let metadata = mod?.ApiRecord?.Metadata

			where mod.Folder.Type is ModType.Smapi
				? metadata != null // C# mods should be added to compat list
				: metadata?.Name != null // other mods should only be validated if they're already listed (e.g. in 'broken content packs' section)

			orderby (metadata.Name ?? mod.Folder.DisplayName).Replace(" ", "").Replace("'", "")
			let compatHasID = metadata.ID.Any() == true
			let ids = mod.IDs.ToArray()
			let modHasID = ids.Any()
			let commonID = metadata.ID.Intersect(ids).FirstOrDefault()
			where
				((modHasID || compatHasID) && commonID == null)
				|| !EqualsInvariant(metadata.NexusID?.ToString(), mod.GetModID("Nexus", mustBeInt: true))
				|| !EqualsInvariant(metadata.ChucklefishID?.ToString(), mod.GetModID("Chucklefish", mustBeInt: true))
				|| !EqualsInvariant(metadata.GitHubRepo, mod.GetModID("GitHub"))

			select new
			{
				ID = ids.FirstOrDefault(),
				CompatList = metadata?.Name != null
					? new
					{
						Name = metadata.Name,
						ID = Format(commonID ?? (compatHasID ? string.Join(", ", metadata.ID) : null), commonID ?? (modHasID ? string.Join(", ", ids) : null)),
						NexusID = Format(metadata.NexusID?.ToString(), mod.GetModID("Nexus", mustBeInt: true)),
						ChucklefishID = Format(metadata.ChucklefishID?.ToString(), mod.GetModID("Chucklefish", mustBeInt: true)),
						GitHub = Format(metadata.GitHubRepo, mod.GetModID("GitHub"))
					}
					: (object)Format("not on compat list", null),
				UpdateKeys = string.Join(", ", mod.UpdateKeys),
				Raw = Util.OnDemand("raw data", () => mod)
			}
		);
		if (result.Any())
			result.Dump("Potential compatibility list errors");
	}

	/****
	** Report mods missing from the compatibility list
	****/
	if (this.ShowMissingCompatMods)
	{
		ModData[] missing =
			(
				from mod in mods
				where
					mod.ApiRecord?.Metadata == null
					&& mod.IsInstalled
					&& mod.Folder.Manifest.ContentPackFor == null
					&& !this.IgnoreMissingCompatListMods.Intersect(mod.UpdateKeys).Any()
					&& !this.IgnoreMissingCompatListMods.Intersect(mod.IDs).Any()
					&& !this.IgnoreMissingCompatListMods.Contains(mod.Folder.Manifest.EntryDll)
				select mod
			)
			.Where(p => p.IDs.Count() > 1 || !p.IDs.FirstOrDefault()?.Contains("FAKE.") == true)
			.ToArray();
		if (missing.Any())
		{
			missing
				.Select(mod => new
				{
					FolderName = mod.Folder.Directory.Name,
					Manifest = Util.OnDemand("expand", () => mod.Folder.Manifest),
					RawManifest = Util.OnDemand("expand", () => File.ReadAllText(Path.Combine(mod.Folder.Directory.FullName, "manifest.json"))),
					ApiRecord = Util.OnDemand("expand", () => mod.ApiRecord),
					IDs = mod.IDs,
					UpdateKeys = mod.UpdateKeys,
					Installed = mod.InstalledVersion.ToString(),
					Source = mod.GetSourceUrl() != null ? new Hyperlinq(mod.GetSourceUrl(), "source") : null
				})
				.Dump("Installed mods not on compatibility list");
		}
	}

	/****
	** Report mods on the compatibility list not installed locally
	****/
	Lazy<Task<ModCompatibilityEntry[]>> compatListAsync = new(
		() => LocalCompatListRepoPath != null
			? toolkit.GetCompatibilityListFromLocalGitFolderAsync(LocalCompatListRepoPath)
			: toolkit.GetCompatibilityListAsync()
	);
	if (this.ShowMissingLocalMods)
	{
		// get mods installed locally
		ISet<string> localIds = new HashSet<string>(
			mods.Select(p => p.Folder.Manifest.UniqueID),
			StringComparer.InvariantCultureIgnoreCase
		);

		// fetch mods on the compatibility list that aren't installed
		ModCompatibilityEntry[] compatList = await compatListAsync.Value;
		var missing =
			(
				from mod in compatList
				where
					// has an ID
					mod.ID.Any()
					&& !mod.ID.All(p => p == "none")

					// isn't ignored
					&& !this.IgnoreMissingLocalMods.Intersect(mod.ID).Any()

					// isn't a content pack
					&& mod.ContentPackFor == null

					// isn't installed locally
					&& !mod.ID.Any(id => localIds.Contains(id))
				
				let links = this.GetReportLinks(mod)

				select new
				{
					Name = mod.Name.FirstOrDefault(),
					Author = mod.Author.FirstOrDefault(),
					Nexus = links.Nexus,
					ModDrop = links.ModDrop,
					CurseForge = links.CurseForge,
					Chucklefish = links.Chucklefish,
					GitHub = links.GitHub,
					Custom = links.Custom,
					ID = mod.ID
				}
			)
			.OrderBy(p => p.Name, StringComparer.InvariantCultureIgnoreCase)
			.ToArray();
		
		if (missing.Any())
			missing.Dump("Mods on compatibility list not installed locally");
	}

	/****
	** Show missing dependencies
	****/
	if (this.ShowMissingDependencies)
	{
		ModCompatibilityEntry[] compatList = await compatListAsync.Value;
		Lazy<Dictionary<string, ModCompatibilityEntry>> compatModsById = new(() =>
		{
			Dictionary<string, ModCompatibilityEntry> values = new(StringComparer.OrdinalIgnoreCase);
			foreach (ModCompatibilityEntry entry in compatList)
			{
				foreach (string id in entry.ID)
					values.TryAdd(id, entry);
			}
			return values;
		});

		var dependenciesById =
			(
				from mod in mods
				from dependency in mod.GetRequiredDependencies()
				group mod by dependency into modGroup

				let requiredId = modGroup.Key
				let installed = mods.Any(p => p.IDs.Contains(requiredId))

				where !installed

				let requiredName = compatModsById.Value.GetValueOrDefault(requiredId)?.Name.FirstOrDefault() ?? "???"
				orderby requiredName

				select new
				{
					MissingFramework = Util.VerticalRun(requiredName, Util.WithStyle(requiredId, "color: gray; font-size: 0.9em;")),
					NeededForMods = "- " + string.Join("\n- ", modGroup.Select(p => p.GetDisplayName()).Order()),
					Actions = (object)Util.OnDemand(
						"install from mod dump",
						() => new object[] // returning an array allows collapsing the log in the LINQPad output
						{
							Util.WithStyle(
								Util.VerticalRun(this.ModCacheHelper.TryInstall(requiredId, folderNamePrefix: ModCacheUtilities.TemporaryFolderPrefix)),
								"font-style: monospace; font-size: 0.9em;"
							)
						}
					)
				}
			)
			.ToArray();
		if (dependenciesById.Length > 0)
			dependenciesById.Dump("Missing dependencies needed to run these mods");
	}

	/****
	** Show final report
	****/
	this
		.GetReport(mods.Where(p => p.IsInstalled))
		.OrderBy(mod =>
		{
			if (mod.HasUpdate)
				return 1;
			if (mod.Errors.Any())
				return 2;
			if (mod.Warnings.Any())
				return 3;
			return 4;
		})
		.ThenBy(mod => mod.Name)
		.Select(mod =>
		{
			if (!mod.IsValid)
				return (dynamic)new { NormalizedFolder = Util.WithStyle(mod.NormalizedFolder, "color: red;") };

			const string smallStyle = "font-size: 0.8em;";
			const string errorStyle = "color: red; font-weight: bold;";
			const string warnStyle = "color: yellow; font-weight: bold;";
			const string fadedStyle = "color: gray;";

			// get mod info
			bool highlightStatus = mod.Compatibility != null && this.HighlightStatuses.Contains(mod.Compatibility.Value);
			var apiMetadata = mod.ModData.ApiRecord?.Metadata;

			// parse overrides
			ChangeDescriptor changeLocalVersion = ChangeDescriptor.Parse(apiMetadata?.ChangeLocalVersions, out var changeLocalVersionErrors);
			ChangeDescriptor changeRemoteVersion = ChangeDescriptor.Parse(apiMetadata?.ChangeRemoteVersions, out var changeRemoteVersionErrors);
			ChangeDescriptor changeUpdateKeys = ChangeDescriptor.Parse(apiMetadata?.ChangeUpdateKeys, out var changeUpdateKeysErrors);

			// format version
			bool hasUpdate = false;
			string versionHtml;
			if (mod.Latest == null)
				versionHtml = $"<span style='{smallStyle} {errorStyle}'>not found</span>";
			else if (mod.HasUpdate)
			{
				hasUpdate = true;
				versionHtml = $"<a href='{mod.DownloadUrl}' style='{smallStyle}'>{mod.Latest}</a>";
			}
			else
				versionHtml = $"<span style='{smallStyle} {fadedStyle}'>{mod.Latest}</span>";

			// build issues
			XElement issues = new XElement("div");
			foreach (var error in mod.Errors.Concat(changeLocalVersionErrors).Concat(changeRemoteVersionErrors).Concat(changeUpdateKeysErrors))
				issues.Add(new XElement("div", new XAttribute("style", $"{smallStyle} {errorStyle}"), $"⚠ {error}"));
			foreach (var warning in mod.Warnings)
				issues.Add(new XElement("div", new XAttribute("style", $"{smallStyle} {warnStyle}"), $"⚠ {warning}"));
			foreach (var issue in mod.MinorIssues)
				issues.Add(new XElement("div", new XAttribute("style", $"{smallStyle} {fadedStyle}"), $"⚠ {issue}"));

			// build name column
			object nameCol = Util.WithStyle(mod.Name, smallStyle);
			if (!string.Equals(mod.NormalizedFolder.Replace(" ", ""), mod.Name.Replace(" ", "", StringComparison.OrdinalIgnoreCase)))
				nameCol = Util.VerticalRun(nameCol, Util.WithStyle("  in " + mod.NormalizedFolder, smallStyle + "opacity:0.7;"));
			if (mod.Author != null)
				nameCol = Util.VerticalRun(nameCol, Util.WithStyle($"  by {mod.Author}", smallStyle));

			// get actions
			object actions = null;
			if (hasUpdate)
			{
				var data = mod.ModData;

				actions = Util.OnDemand(
					"install from dump",
					() => new object[] // returning an array allows collapsing the log in the LINQPad output
					{
						Util.WithStyle(
							Util.VerticalRun(this.ModCacheHelper.TryUpdateFromModCache(data.Folder.Directory, data.IDs, data.UpdateKeys, data.InstalledVersion)),
							"font-style: monospace; font-size: 0.9em;"
						)
					}
				);
			}

			// get report
			return new
			{
				Name = nameCol,
				Installed = Util.WithStyle(mod.Installed, smallStyle),
				Latest = Util.RawHtml(versionHtml),
				Status = Util.WithStyle(mod.Compatibility, $"{smallStyle} {(highlightStatus ? errorStyle : "")}"),
				Summary = Util.WithStyle($"{mod.CompatSummary} {(!string.IsNullOrWhiteSpace(mod.CompatBrokeIn) ? $"[broke in {mod.CompatBrokeIn}]" : "")}".Trim(), $"{smallStyle} {(highlightStatus ? errorStyle : "")}"),
				Issues = Util.RawHtml(issues),
				Type = mod.ModData.Folder.Type,
				Source = mod.SourceUrl != null ? new Hyperlinq(mod.SourceUrl, "source") : null,
				Actions = actions,
				Metadata = Util.OnDemand("expand", () => new
				{
					UpdateKeys = Util.OnDemand("expand", () => mod.UpdateKeys),
					ManifestUpdateKeys = Util.OnDemand("expand", () => mod.ManifestUpdateKeys != null ? mod.ManifestUpdateKeys : Util.WithStyle("none", errorStyle)),
					NormalizedFolder = Util.OnDemand("expand", () => mod.NormalizedFolder),
					Manifest = Util.OnDemand("expand", () => mod.Manifest),
					Nexus = mod.Links.Nexus,
					OtherLinks = Util.OnDemand("expand", () => mod.Links),
				}),
				Overrides = changeLocalVersion.HasChanges || changeRemoteVersion.HasChanges || changeUpdateKeys.HasChanges
					? Util.OnDemand("expand", () => new
					{
						LocalVersion = changeLocalVersion?.ToString(),
						RemoteVersion = changeRemoteVersion?.ToString(),
						UpdateKeys = changeUpdateKeys?.ToString()
					})
					: null
			};
		})
		.Dump("mods");
}

/*********
** Helpers
*********/
/// <summary>Get links for a mod.</summary>
/// <param name="mod">The compatibility entry for the mod.</param>
private dynamic GetReportLinks(ModCompatibilityEntry mod)
{
	return mod is not null
		? GetReportLinks(nexusId: mod.NexusID, modDropId: mod.ModDropID, curseForgeId: mod.CurseForgeID, chucklefishId: mod.ChucklefishID, gitHubRepo: mod.GitHubRepo, customUrl: mod.CustomUrl)
		: null;
}

/// <summary>Get links for a mod.</summary>
/// <param name="mod">The API metadata for the mod.</param>
private dynamic GetReportLinks(ModEntryModel mod)
{
	ModExtendedMetadataModel meta = mod?.Metadata;
	return meta is not null
		? GetReportLinks(nexusId: meta.NexusID, modDropId: meta.ModDropID, curseForgeId: meta.CurseForgeID, chucklefishId: meta.ChucklefishID, gitHubRepo: meta.GitHubRepo, customUrl: meta.CustomUrl)
		: null;
}

/// <summary>Get links for a mod.</summary>
/// <param name="mod">The API metadata for the mod.</param>
private dynamic GetReportLinks(int? nexusId, int? modDropId, int? curseForgeId, int? chucklefishId, string gitHubRepo, string customUrl)
{
	return new
	{
		Nexus = BuildLink(ModSiteKey.Nexus, nexusId),
		ModDrop = BuildLink(ModSiteKey.ModDrop, modDropId),
		CurseForge = BuildLink(ModSiteKey.CurseForge, curseForgeId),
		Chucklefish = BuildLink(ModSiteKey.Chucklefish, chucklefishId),
		GitHub = BuildLink(ModSiteKey.GitHub, gitHubRepo),
		Custom = customUrl != null ? new Hyperlinq(customUrl, "custom") : null
	};

	Hyperlinq BuildLink(ModSiteKey site, object id)
	{
		if (id is not null)
		{
			string url = this.ModToolkit.GetUpdateUrl(site, id.ToString());
			return new Hyperlinq(url, $"{site}:{id}");
		}

		return null;
	}
}

/// <summary>Get a flattened view of the mod data.</summary>
/// <param name="mods">The mods to represent.</param>
IEnumerable<ReportEntry> GetReport(IEnumerable<ModData> mods)
{
	foreach (ModData mod in mods)
	{
		// not installed locally
		if (mod.Folder == null)
			continue;

		// yield info
		{
			// get latest version + URL
			var downloads = new[]
			{
				new { Version = mod.ApiRecord?.Metadata?.Main?.Version, Url = mod.ApiRecord?.Metadata?.Main?.Url },
				new { Version = mod.ApiRecord?.Metadata.Optional?.Version, Url = mod.ApiRecord?.Metadata?.Optional?.Url },
				new { Version = mod.GetUnofficialVersion(), Url = $"https://smapi.io/mods#{this.GetAnchor(mod.ApiRecord?.Metadata?.Name ?? mod.Folder.DisplayName)}" }
			};
			ISemanticVersion latestVersion = null;
			string downloadUrl = null;
			foreach (var download in downloads)
			{
				if (download.Version == null)
					continue;

				if (latestVersion == null || download.Version.IsNewerThan(latestVersion))
				{
					latestVersion = download.Version;
					downloadUrl = download.Url;
				}
			}

			// override update check
			bool ignoreUpdate = false;
			if (latestVersion != null && latestVersion != mod.InstalledVersion)
			{
				foreach (string id in mod.IDs)
				{
					if (this.EquivalentModVersions.TryGetValue(id, out Tuple<string, string> pair))
					{
						if (mod.InstalledVersion.ToString() == pair.Item1 && latestVersion.ToString() == pair.Item2)
						{
							ignoreUpdate = true;
							break;
						}
					}
				}
			}

			// build model
			yield return new ReportEntry(mod, latestVersion, downloadUrl, ignoreUpdate, this.GetReportLinks(mod.ApiRecord), (Func<string, string>)(p => this.TryFormatVersion(p)));
		}
	}
}

/// <summary>Get a normalized representation of a version if it's parseable.</summary>
/// <param name="version">The raw version to parse.</param>
private string TryFormatVersion(string version)
{
	return SemanticVersion.TryParse(version, allowNonStandard: true, out ISemanticVersion parsed)
		? parsed.ToString()
		: version?.Trim();
}

/// <summary>The aggregated data for a mod.</summary>
class ModData
{
	/*********
	** Properties
	*********/
	/// <summary>The record from SMAPI's web API.</summary>
	private ModEntryModel ApiRecordImpl;


	/*********
	** Accessors
	*********/
	/// <summary>The mod metadata read from its folder.</summary>
	public ModFolder Folder { get; }

	/// <summary>The record from SMAPI's web API.</summary>
	public ModEntryModel ApiRecord
	{
		get { return this.ApiRecordImpl; }
		set
		{
			this.ApiRecordImpl = value;
			this.Update();
		}
	}

	/// <summary>The unique mod IDs.</summary>
	public string[] IDs { get; private set; }

	/// <summary>The aggregate update keys.</summary>
	public string[] UpdateKeys { get; private set; }

	/// <summary>Whether the mod is installed (regardless of whether it's compatible).</summary>
	public bool IsInstalled => this.Folder?.Manifest != null;

	/// <summary>The installed mod version.</summary>
	public ISemanticVersion InstalledVersion { get; private set; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="folder">The mod metadata read from its folder.</param>
	public ModData(ModFolder folder)
	{
		this.Folder = folder;
		this.Update();
	}

	/// <summary>Get the mod ID in a repository from the mod's update keys, if available.</summary>
	/// <param name="repositoryKey">The case-insensitive repository key (like Nexus or Chucklefish) to match.</summary>
	/// <param name="mustBeInt">Whether the mod must be a positive integer value.</param>
	/// <returns>Retudns the mod ID, or <c>null</c> if not found.</returns>
	public string GetModID(string repositoryKey, bool mustBeInt = false)
	{
		foreach (string key in this.UpdateKeys.Where(p => p != null && p.StartsWith($"{repositoryKey}:", StringComparison.InvariantCultureIgnoreCase)))
		{
			string[] parts = key.Split(new[] { ':' }, 2);
			if (parts[1].Length <= 0)
				continue;

			if (mustBeInt)
			{
				if (int.TryParse(parts[1], out int id) && id >= 0)
					return id.ToString(CultureInfo.InvariantCulture);
				continue;
			}

			return parts[1];
		}

		return null;
	}

	/// <summary>Get a display name for this mod.</summary>
	public string GetDisplayName()
	{
		return
			this.Folder?.DisplayName
			?? this.ApiRecord?.Metadata?.Name
			?? this.IDs.FirstOrDefault();
	}

	/// <summary>Get the unofficial update for this mod, if any.</summary>
	public ISemanticVersion GetUnofficialVersion()
	{
		return this.ApiRecord?.Metadata.Unofficial?.Version;
	}

	/// <summary>Get the URL to the mod's code repository, if any.</summary>
	public string GetSourceUrl()
	{
		// GitHub
		string repo = this.GetModID("GitHub");
		if (repo != null)
			return $"https://github.com/{repo}";

		// custom source
		if (!string.IsNullOrWhiteSpace(this.ApiRecord?.Metadata?.CustomSourceUrl))
			return this.ApiRecord.Metadata.CustomSourceUrl;

		return null;
	}

	/// <summary>Get the mod IDs that must be installed to use this mod.</summary>
	public HashSet<string> GetRequiredDependencies()
	{
		var dependencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		if (this.Folder.ManifestParseError is ModParseError.None)
		{
			IManifest manifest = this.Folder.Manifest;

			if (!string.IsNullOrWhiteSpace(manifest.ContentPackFor?.UniqueID))
				dependencies.Add(manifest.ContentPackFor.UniqueID.Trim());

			if (manifest.Dependencies?.Length > 0)
			{
				foreach (ManifestDependency dependency in manifest.Dependencies)
				{
					if (dependency?.IsRequired is true && !string.IsNullOrWhiteSpace(dependency.UniqueID))
						dependencies.Add(dependency.UniqueID.Trim());
				}
			}
		}

		return dependencies;
	}


	/*********
	** Private methods
	*********/
	/// <summary>Update the aggregated data.</summary>
	private void Update()
	{
		this.IDs = this.GetIDs().ToArray();
		this.UpdateKeys = this.GetUpdateKeys().ToArray();
		this.InstalledVersion = this.Folder?.Manifest?.Version;

		// map installed version
		if (this.InstalledVersion != null && this.ApiRecord?.Metadata.ChangeLocalVersions != null)
		{
			var versions = new List<string>() { this.Folder.Manifest.Version.ToString() };
			var changes = ChangeDescriptor.Parse(this.ApiRecord.Metadata.ChangeLocalVersions, out _);
			changes.Apply(versions);
			if (SemanticVersion.TryParse(versions.FirstOrDefault(), out ISemanticVersion version))
				this.InstalledVersion = version;
		}
	}

	/// <summary>Get the possible mod IDs.</summary>
	private IEnumerable<string> GetIDs()
	{
		IEnumerable<string> GetAll()
		{
			yield return this.Folder?.Manifest?.UniqueID;
			foreach (string cur in this.ApiRecord?.Metadata?.ID ?? new string[0])
				yield return cur.Trim();
		}

		return GetAll()
			.Where(p => !string.IsNullOrWhiteSpace(p))
			.Select(p => p.Trim())
			.Distinct();
	}

	/// <summary>Get the known update keys.</summary>
	private IEnumerable<string> GetUpdateKeys()
	{
		// get defined
		IEnumerable<string> GetDefined()
		{
			// mod folder
			var manifestKeys = this.Folder?.Manifest?.UpdateKeys;
			if (manifestKeys != null)
			{
				foreach (string key in manifestKeys)
				{
					if (!string.IsNullOrWhiteSpace(key))
						yield return key;
				}
			}

			// API record
			var compat = this.ApiRecord?.Metadata;
			if (compat != null)
			{
				foreach (string key in compat.GetUpdateKeys())
					yield return key;
			}
		}

		// yield uniques or default
		HashSet<string> seen = new HashSet<string>();
		foreach (string key in GetDefined())
		{
			UpdateKey parsed = UpdateKey.Parse(key);
			if (!parsed.LooksValid)
				continue;

			string parsedStr = parsed.ToString();
			if (seen.Add(parsedStr))
				yield return parsedStr;
		}
	}
}

/// <summary>An entry in the generated report.</summary>
class ReportEntry
{
	/********
	** Accessors
	********/
	/// <summary>The underlying mod data.</summary>
	public ModData ModData { get; }

	/// <summary>Whether the mod is correctly installed.</summary>
	public bool IsValid { get; }

	/// <summary>The mod manifest.</summary>
	public Manifest Manifest { get; }

	/// <summary>The mod folder name.</summary>
	public string NormalizedFolder { get; }

	/// <summary>The mod name.</summary>
	public string Name { get; }

	/// <summary>The mod author's name.</summary>
	public string Author { get; }

	/// <summary>The installed mod version.</summary>
	public string Installed { get; }

	/// <summary>The latest available mod version.</summary>
	public string Latest { get; }

	/// <summary>A comma-delimited list of update keys in the manifest file.</summary>
	public string ManifestUpdateKeys { get; }

	/// <summary>A comma-delimited list of update keys from all sources.</summary>
	public string UpdateKeys { get; }

	/// <summary>Whether a newer version than the one installed is available.</summary>
	public bool HasUpdate { get; }

	/// <summary>The compatibility status from the compatibility list.</summary>
	public ModCompatibilityStatus? Compatibility { get; }

	/// <summary>The compatibility 'broke in' field from the compatibility list.</summary>
	public string CompatBrokeIn { get; }

	/// <summary>The compatibility summary from the compatibility list.</summary>
	public string CompatSummary { get; }

	/// <summary>The unofficial version from the compatibility list, if any.</summary>
	public ISemanticVersion CompatUnofficialVersion { get; }

	/// <summary>The URL to download the latest version.</summary>
	public string DownloadUrl { get; set; }

	/// <summary>Any errors that occurred while checking for updates.</summary>
	public string[] UpdateCheckErrors { get; set; }

	/// <summary>The code repository URL, if any.</summary>
	public string SourceUrl { get; }
	
	/// <summary>An exportable list of links.</summary>
	public dynamic Links { get; }

	/// <summary>The critical mod issues.</summary>
	public IList<string> Errors { get; } = new List<string>();
	
	/// <summary>The non-critical mod issues.</summary>
	public IList<string> Warnings { get; } = new List<string>();

	/// <summary>The minor issues.</summary>
	public IList<string> MinorIssues { get; } = new List<string>();


	/********
	** Public methods
	********/
	/// <summary>Construct an instance.</summary>
	public ReportEntry(ModData mod, ISemanticVersion latestVersion, string downloadUrl, bool ignoreUpdate, dynamic links, Func<string, string> tryFormatVersion)
	{
		var manifest = mod.Folder.Manifest;
		var apiMetadata = mod.ApiRecord?.Metadata;

		this.ModData = mod;
		this.IsValid = true;
		this.Manifest = manifest;
		this.NormalizedFolder = mod.Folder.Directory.Name;
		this.Name = manifest.Name;
		this.Author = manifest.Author;
		this.Installed = mod.InstalledVersion.ToString();
		this.Latest = latestVersion?.ToString();
		this.ManifestUpdateKeys = manifest.UpdateKeys != null ? string.Join(", ", manifest.UpdateKeys) : null;
		this.UpdateKeys = string.Join(", ", mod.UpdateKeys);
		this.HasUpdate = !ignoreUpdate && latestVersion != null && latestVersion.IsNewerThan(mod.InstalledVersion);
		this.DownloadUrl = downloadUrl;
		this.UpdateCheckErrors = mod.ApiRecord?.Errors ?? new string[0];
		this.SourceUrl = mod.GetSourceUrl();
		this.Links = links;

		this.Compatibility = apiMetadata?.CompatibilityStatus;
		this.CompatSummary = apiMetadata?.CompatibilitySummary;
		this.CompatBrokeIn = apiMetadata?.BrokeIn;
		this.CompatUnofficialVersion = apiMetadata?.Unofficial?.Version;

		this.PopulateModIssues(tryFormatVersion);
	}


	/********
	** Private methods
	********/
	/// <summary>Populate the issue fields.</summary>
	/// <param name="tryFormatVersion">Get a normalized representation of a version if it's parseable.</param>
	private void PopulateModIssues(Func<string, string> tryFormatVersion)
	{
		if (!this.IsValid)
			return;

		// get mod info
		var apiMetadata = this.ModData.ApiRecord?.Metadata;

		// issues to highlight
		if (this.Compatibility == null && this.ModData.Folder.Type is ModType.Smapi)
			this.Errors.Add("not on compat list");
		if (this.Installed != null && this.Latest != null && new SemanticVersion(this.Latest).IsOlderThan(this.Installed))
			this.Warnings.Add("official version is older");
		if (this.Installed != null && this.CompatUnofficialVersion != null && this.CompatUnofficialVersion.IsOlderThan(this.Installed))
			this.Warnings.Add("unofficial version on compat list is older");
		if (string.IsNullOrWhiteSpace(this.UpdateKeys))
			this.Warnings.Add("no valid update keys in manifest or compat list");

		// update check errors
		foreach (string message in this.UpdateCheckErrors)
		{
			if (message.Contains("Exception") || (message.StartsWith("Found no") && !message.StartsWith("Found no GitHub")))
				this.Errors.Add(message);
			else if (message.Contains("has no valid versions"))
				this.Warnings.Add(message);
			else
				this.MinorIssues.Add(message);
		}
	}
}

/// <summary>Get the unique anchor for the mod on the compatibility list, excluding the '#' symbol.</summary>
/// <param name="name">The standardized mod name.</param>
private string GetAnchor(string name)
{
	return PathUtilities.CreateSlug(name)?.ToLower();
}
