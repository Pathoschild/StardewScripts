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
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.WebApi</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.Wiki</Namespace>
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
#load "Utilities/ModDumpManager.linq"

/*********
** Configuration
*********/
/****
** Environment
****/
/// <summary>The absolute path for the folder containing mods.</summary>
private readonly string GameFolderPath = @"C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley";

/// <summary>The folder to which mods should be moved which you click 'move to mod dump'.</summary>
private readonly string ModDumpFolderPath = @"E:\source\_Stardew\_smapi-mod-dump\compiled\okay";

/// <summary>The absolute path for the folder containing mods.</summary>
private string ModFolderPath => Path.Combine(this.GameFolderPath, "Mods (test)");

/// <summary>The absolute path for the file which, if present, indicates mod folders should not be normalized.</summary>
private string ModFolderPathDoNotNormalizeToken => Path.Combine(this.ModFolderPath, "DO_NOT_NORMALIZE.txt");

/// <summary>The mod dump from which to install mod updates, if they're fetched.</summary>
private Lazy<ILookup<string, ParsedMod>> ModDump = new(() =>
{
	ModDumpManager modDump = new(rootPath: @"C:\dev\mod-dump", resetUnpacked: false);
	return modDump.ReadMods().ToLookup(mod => $"{mod.Site}:{mod.ID}");
});

/****
** Common settings
****/
/// <summary>The wiki compatibility statuses to highlight as errors. Mainly useful when you have a set of mods you know work or don't work, and want to find errors in the compatibility list.</summary>
private readonly HashSet<WikiCompatibilityStatus> HighlightStatuses = new HashSet<WikiCompatibilityStatus>(
	// all statuses
	new[]
	{
		WikiCompatibilityStatus.Ok, WikiCompatibilityStatus.Optional, WikiCompatibilityStatus.Unofficial, // OK
		WikiCompatibilityStatus.Broken, WikiCompatibilityStatus.Workaround, // broken
		WikiCompatibilityStatus.Abandoned, WikiCompatibilityStatus.Obsolete // abandoned
	}
	//.Except(new[] { WikiCompatibilityStatus.Abandoned, WikiCompatibilityStatus.Obsolete }) // if abandoned
	//.Except(new[] { WikiCompatibilityStatus.Broken, WikiCompatibilityStatus.Workaround }) // if broken
	.Except(new[] { WikiCompatibilityStatus.Ok, WikiCompatibilityStatus.Optional, WikiCompatibilityStatus.Unofficial }) // if OK
);

/// <summary>Whether to show data for the latest version of the game, even if it's a beta.</summary>
public bool ForBeta = true;

/// <summary>Whether to normalize mod folders.</summary>
public bool NormalizeFolders = true;

/// <summary>Whether to allow normalizing a mod folder if it contains multiple mods. This is usually not intended (e.g. a download containing a SMAPI mod along with supporting content packs).</summary>
public bool AllowNormalizingFoldersContainingMultipleMods = false;

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
/// <summary>Mod IDs, update keys, custom URLs, or entry DLLs to ignore when checking if a local mod is on the wiki.</summary>
/// <remarks>This should only be used when a mod can't be cross-referenced because it has no ID and isn't released anywhere valid that can be used as an update key.</summary>
public string[] IgnoreMissingWikiMods = new[]
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

/// <summary>Mod IDs to ignore when checking if a wiki mod is installed locally.</summary>
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

/// <summary>Maps mod IDs to the equivalent mod page URLs, in cases where that can't be determined from the mod data.</summary>
public IDictionary<string, string> OverrideModPageUrls = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
{
	// mods with only a forum thread ID
	["ANDR55.AdvMachines"] = "https://community.playstarbound.com/threads/137132", // Advanced Machines
	["AutoWater"] = "https://community.playstarbound.com/threads/129355",
	["FAKE.AshleyMod"] = "https://community.playstarbound.com/threads/112077",
	["FAKE.EvilPdor.WakeUp"] = "https://community.playstarbound.com/threads/111526",
	["FAKE.EvilPdor.StaminaRegen"] = "https://community.playstarbound.com/threads/111526",
	["FAKE.EvilPdor.WeatherController"] = "https://community.playstarbound.com/threads/111526",
	["FAKE.FarmAutomation.ItemCollector"] = "https://community.playstarbound.com/threads/111931",
	["FAKE.FarmAutomation.BarnDoorAutomation"] = "https://community.playstarbound.com/threads/111931",
	["FAKE.RainRandomizer"] = "https://community.playstarbound.com/threads/111526",
	["HappyAnimals"] = "https://community.playstarbound.com/threads/126655",
	["HorseWhistle_SMAPI"] = "https://community.playstarbound.com/threads/111550", // Horse Whistle (Nabuma)
	["KuroBear.SmartMod"] = "https://community.playstarbound.com/threads/108104",
	["RoyLi.Fireballs"] = "https://community.playstarbound.com/threads/129346",
	["RuyiLi.AutoCrop"] = "https://community.playstarbound.com/threads/129152",
	["RuyiLi.BloodTrail"] = "https://community.playstarbound.com/threads/129308",
	["RuyiLi.Emotes"] = "https://community.playstarbound.com/threads/129159",
	["RuyiLi.InstantFishing"] = "https://community.playstarbound.com/threads/129163",
	["RuyiLi.Kamikaze"] = "https://community.playstarbound.com/threads/129126",
	["RuyiLi.SlimeSpawner"] = "https://community.playstarbound.com/threads/129326",
	["Spouseroom"] = "https://community.playstarbound.com/threads/111636" // Spouse's Room Mod
};

/// <summary>Maps mod IDs to the folder name to use, overriding the name from the wiki.</summary>
public IDictionary<string, string> OverrideFolderNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
{
	// prefixes for testing convenience
	["aedenthorn.MobilePhone"] = "@Mobile Phone",
	["bcmpinc.StardewHack"] = "@@Stardew Hack",
	["bwdyworks"] = "@Bwdyworks",
	["cat.Pong"] = "(1) Pong",
	["Cherry.ExpandedPreconditionsUtility"] = "@Expanded Preconditions Utility",
	["DIGUS.MailFrameworkMod"] = "@@Mail Framework Mod",
	["Entoarox.AdvancedLocationLoader"] = "@Advanced Location Loader",
	["Ilyaki.BattleRoyale"] = "(2) Battle Royalley",
	["Platonymous.ArcadePong"] = "(1) Arcade Pong",
	["Platonymous.PlatoTK"] = "@@PlatoTK",
	["Platonymous.Toolkit"] = "@@PyTK",
	["spacechase0.DynamicGameAssets"] = "@@Dynamic Game Assets",
	["spacechase0.JsonAssets"] = "@@Json Assets",
	["spacechase0.SpaceCore"] = "@@SpaceCore",
	["TehPers.CoreMod"] = "@@TehCore",
	["tylergibbs2.BattleRoyalleyYear2"] = "(2) Battle Royalley - Year 2",

	// names for mods without a real ID
	["FAKE.RainRandomizer"] = "Rain Randomizer",

	// fix invalid names
	["jahangmar.CompostPestsCultivation"] = "Compost, Pests, and Cultivation", // commas stripped by wiki
	["leclair.bcbuildings"] = "Better Crafting - Buildings", // : replaced with _
	["minervamaga.FeelingLucky"] = "Feeling Lucky", // ? replaced with _, just strip it instead

	// fix duplicate IDs (Slime Minigame)
	["Tofu.SlimeMinigame"] = "Slime Mods - Slime Minigame",
	["Tofu.SlimeQOL"] = "Slime Mods - SlimeQoL Alt",

	// fix ambiguous names
	["pempi.addMoney"] = "Add Money (Pempi)",
	["StephHoel.AddMoney"] = "Add Money (StephHoel)",

	["cgifox.AutoAttack"] = "Auto-Attack (cgifox)",
	["X3n0n182.AutoAttack"] = "Auto-Attack (X3n0n182)",
	
	["pickle.autowater"] = "Auto Water (Pickle)",
	["StephHoel.AutoWater"] = "Auto Water (StephHoel)",
	
	["leclair.bettercrafting"] = "Better Crafting (Khloe Leclair)",
	["RedstoneBoy.BetterCrafting"] = "Better Crafting (RedstoneBoy)",
	
	["ceruleandeep.bwdyworks"] = "Bwdyworks (ceruleandeep)",

	["BayesianBandit.ConfigureMachineSpeed"] = "Configure Machine Speed (BayesianBandit)",
	["StephHoel.ConfigureMachineSpeed"] = "Configure Machine Speed (StephHoel)",

	["Vrakyas.CurrentLocation"] = "Current Location (Vrakyas)",
	["CurrentLocation102120161203"] = "Current Location (Omegasis)",

	["Thor.EnemyHealthBars"] = "Enemy Health Bars (TheThor59)",

	["BlaDe.EventTester"] = "Event Tester (BlaDe)",
	["sinZandAtravita.SinZsEventTester"] = "Event Tester (SinZ)",

	["HappyBirthday"] = "Happy Birthday (Oxyligen)",
	["Omegasis.HappyBirthday"] = "Happy Birthday (Omegasis)",

	["HorseWhistle_SMAPI"] = "Horse Whistle (Nabuma)",
	["icepuente.HorseWhistle"] = "Horse Whistle (Icepuente)",
	
	["6135.ProfitCalculator"] = "Profit Calculator (6135)",
	["6135.ProfitCalculatorDGA"] = "Profit Calculator (6135) → DGA Support",
	["spacechase0.ProfitCalculator"] = "Profit Calculator (spacechase0)",
	
	["stokastic.SmartCursor"] = "Smart Cursor (Stokastic)",
	["DecidedlyHuman.SmartCursor"] = "Smart Cursor (DecidedlyHuman)",

	// prefix sub-mods
	["SilentOak.AutoQualityPatch"] = "Quality Products - Auto Quality Patch"
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
** Internal constants
****/
const string TraceStyle = "opacity: 0.5";
const string ErrorStyle = "color: red; font-weight: bold;";
const string SuccessStyle = "color: green;";


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
			fileName: Path.Combine(this.GameFolderPath, "StardewModdingAPI.exe"),
			arguments: @$"--mods-path ""{this.ModFolderPath}"""
		),
		"launch SMAPI"
	).Dump("actions");

	/****
	** Initialize
	****/
	Console.WriteLine("Initialising...");

	// data
	var toolkit = new ModToolkit();
	var mods = new List<ModData>();

	// check tokens
	if (this.NormalizeFolders && File.Exists(this.ModFolderPathDoNotNormalizeToken))
	{
		Console.WriteLine("   WARNING: detected 'do not normalize' file, disabling folder normalising.");
		this.NormalizeFolders = false;
	}

	/****
	** Read local mod data
	****/
	Console.WriteLine("Reading local data...");
	foreach (ModFolder folder in toolkit.GetModFolders(this.ModFolderPath, useCaseInsensitiveFilePaths: true))
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
	** Normalize mod folders
	****/
	if (this.NormalizeFolders)
	{
		Console.WriteLine("Normalising mod folders...");

		// validate: don't allow normalizing multiple subfolders as separate mods (usually not intended)
		if (!this.AllowNormalizingFoldersContainingMultipleMods)
		{
			var modsByTopFolder = new Dictionary<string, List<UserQuery.ModData>>();
			foreach (ModData mod in mods)
			{
				string relativePath = PathUtilities.GetRelativePath(this.ModFolderPath, mod.Folder.Directory.FullName);
				string mainFolderName = PathUtilities.GetSegments(relativePath, 2).First();

				if (!modsByTopFolder.TryGetValue(mainFolderName, out List<UserQuery.ModData> modsInFolder))
					modsByTopFolder[mainFolderName] = modsInFolder = new List<UserQuery.ModData>();

				modsInFolder.Add(mod);
			}
			
			string[] foldersWithMultipleMods = modsByTopFolder.Where(p => p.Value.Count > 1).Select(p => p.Key).ToArray();
			if (foldersWithMultipleMods.Any())
			{
				foldersWithMultipleMods.Dump("Found folders containing multiple mods, which can't be normalized per current settings.");
				return;
			}
		}

		// normalize
		foreach (ModData mod in mods)
		{
			// get mod info
			if (!mod.IsInstalled)
				continue;
			ModFolder folder = mod.Folder;
			DirectoryInfo actualDir = folder.Directory;
			string searchFolderName = PathUtilities
				.GetSegments(folder.Directory.FullName)
				.Skip(PathUtilities.GetSegments(this.ModFolderPath).Length)
				.First(); // the name of the folder immediately under Mods containing this mod
			DirectoryInfo searchDir = new DirectoryInfo(Path.Combine(this.ModFolderPath, searchFolderName));
			string relativePath = PathUtilities.GetRelativePath(this.ModFolderPath, actualDir.FullName);

			// get page url
			string url = mod.GetModPageUrl();
			foreach (string id in mod.IDs)
			{
				if (this.OverrideModPageUrls.TryGetValue(id, out string @override))
				{
					url = @override;
					break;
				}
			}

			// create metadata file
			File.WriteAllText(
				Path.Combine(actualDir.FullName, "_metadata.txt"),
				$"page URL: {url}\n"
				+ $"IDs: {string.Join(", ", mod.IDs)}\n"
				+ $"update keys: {string.Join(", ", mod.UpdateKeys)}\n"
			);

			// normalize
			if (!relativePath.StartsWith('%')) // convention for temporary folders (usually dependencies needed to load the mods being tested)
			{
				string newName = null;
				try
				{
					// get preferred name
					newName = mod.GetRecommendedFolderName();
					foreach (string id in mod.IDs)
					{
						if (this.OverrideFolderNames.TryGetValue(id, out string @override))
						{
							newName = @override;
							break;
						}
					}

					// mark unofficial versions
					if (mod.InstalledVersion.IsPrerelease() && (mod.InstalledVersion.PrereleaseTag.Contains("unofficial") || mod.InstalledVersion.PrereleaseTag.Contains("update")))
						newName += $" [unofficial]";

					// sanitize name
					foreach (char ch in Path.GetInvalidFileNameChars())
					{
						char replacementChar = ch is '"' && !newName.Contains('\'')
							? '\'' // special case: change " to ' for readability
							: '_';

						newName = newName.Replace(ch, replacementChar);
					}

					// move to new name
					DirectoryInfo newDir = new DirectoryInfo(Path.Combine(this.ModFolderPath, newName));
					newDir.Parent.Create();
					if (actualDir.FullName != newDir.FullName)
					{
						string newRelativePath = PathUtilities.GetRelativePath(this.ModFolderPath, newDir.FullName);

						Console.WriteLine($"   Moving {relativePath} to {newRelativePath}...");
						if (newDir.Exists)
						{
							actualDir.MoveTo(newDir.FullName + "__TEMP");
							FileUtilities.ForceDelete(newDir);
						}
						actualDir.MoveTo(newDir.FullName);

						relativePath = newRelativePath;
					}
				}
				catch (Exception error)
				{
					new { error, newName, mod }.Dump("error normalising mod folder");
				}
			}
		}

		// delete empty folders
		foreach (DirectoryInfo dir in new DirectoryInfo(this.ModFolderPath).EnumerateDirectories())
		{
			if (this.IsEmptyFolder(dir))
				FileUtilities.ForceDelete(dir);
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
			where metadata != null

			orderby (metadata?.Name ?? mod.Folder.DisplayName).Replace(" ", "").Replace("'", "")
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
				Wiki = metadata?.Name != null
					? new
					{
						Name = metadata.Name,
						ID = Format(commonID ?? (compatHasID ? string.Join(", ", metadata.ID) : null), commonID ?? (modHasID ? string.Join(", ", ids) : null)),
						NexusID = Format(metadata.NexusID?.ToString(), mod.GetModID("Nexus", mustBeInt: true)),
						ChucklefishID = Format(metadata.ChucklefishID?.ToString(), mod.GetModID("Chucklefish", mustBeInt: true)),
						GitHub = Format(metadata.GitHubRepo, mod.GetModID("GitHub"))
					}
					: (object)Format("not on wiki", null),
				UpdateKeys = string.Join(", ", mod.UpdateKeys),
				Raw = Util.OnDemand("raw data", () => mod)
			}
		);
		if (result.Any())
			result.Dump("Potential compatibility list errors");
	}

	/****
	** Report mods missing from the wiki
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
					&& !this.IgnoreMissingWikiMods.Intersect(mod.UpdateKeys).Any()
					&& !this.IgnoreMissingWikiMods.Intersect(mod.IDs).Any()
					&& !this.IgnoreMissingWikiMods.Contains(mod.Folder.Manifest.EntryDll)
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
	** Report mods on the wiki not installed locally
	****/
	Lazy<Task<WikiModList>> compatListAsync = new(() => toolkit.GetWikiCompatibilityListAsync());
	if (this.ShowMissingLocalMods)
	{
		// get mods installed locally
		ISet<string> localIds = new HashSet<string>(
			mods.Select(p => p.Folder.Manifest.UniqueID),
			StringComparer.InvariantCultureIgnoreCase
		);

		// fetch mods on the wiki that aren't installed
		WikiModList compatList = await compatListAsync.Value;
		var missing =
			(
				from mod in compatList.Mods
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
		WikiModList compatList = await compatListAsync.Value;
		Lazy<Dictionary<string, WikiModEntry>> wikiModsById = new(() =>
		{
			Dictionary<string, WikiModEntry> values = new(StringComparer.OrdinalIgnoreCase);
			foreach (WikiModEntry entry in compatList.Mods)
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

				let requiredName = wikiModsById.Value.GetValueOrDefault(requiredId)?.Name.FirstOrDefault() ?? "???"
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
								Util.VerticalRun(this.TryInstallDependency(requiredId)),
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
		.GetReport(mods.Where(p => p.IsInstalled), this.ForBeta)
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
			bool highlightStatus = mod.WikiStatus != null && this.HighlightStatuses.Contains(mod.WikiStatus.Value);
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
			if (mod.Manifest.ExtraFields.TryGetValue("@modDump", out object rawNotes) && rawNotes is string notes)
				issues.Add(new XElement("div", $"ⓘ {notes}"));

			// build name column
			object nameCol = Util.WithStyle(mod.Name, smallStyle);
			if (!string.Equals(mod.NormalizedFolder.Replace(" ", ""), mod.Name.Replace(" ", "", StringComparison.OrdinalIgnoreCase)))
				nameCol = Util.VerticalRun(nameCol, Util.WithStyle("  in " + mod.NormalizedFolder, smallStyle + "opacity:0.7;"));
			if (mod.Author != null)
				nameCol = Util.VerticalRun(nameCol, Util.WithStyle($"  by {mod.Author}", smallStyle));

			// get actions
			object actions = Util.OnDemand(
				"→dump",
				() => new object[] // returning an array allows collapsing the log in the LINQPad output
				{
					Util.WithStyle(
						Util.VerticalRun(this.TryMoveToModDump(mod.ModData, toolkit)),
						"font-style: monospace; font-size: 0.9em;"
					)
				}
			);
			if (hasUpdate)
			{
				actions = Util.HorizontalRun(true,
					actions,
					"•",
					Util.OnDemand(
						"install from dump",
						() => new object[] // returning an array allows collapsing the log in the LINQPad output
						{
							Util.WithStyle(
								Util.VerticalRun(this.TryUpdateFromModDump(mod.ModData)),
								"font-style: monospace; font-size: 0.9em;"
							)
						}
					)
				);
			}

			// get report
			return new
			{
				Name = nameCol,
				Installed = Util.WithStyle(mod.Installed, smallStyle),
				Latest = Util.RawHtml(versionHtml),
				Status = Util.WithStyle(mod.WikiStatus, $"{smallStyle} {(highlightStatus ? errorStyle : "")}"),
				Summary = Util.WithStyle($"{mod.WikiSummary} {(!string.IsNullOrWhiteSpace(mod.WikiBrokeIn) ? $"[broke in {mod.WikiBrokeIn}]" : "")}".Trim(), $"{smallStyle} {(highlightStatus ? errorStyle : "")}"),
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
/// <summary>If a newer version of a mod exists in the mod dump folder, replace the installed version with those newer files.</summary>
/// <param name="mod">The mod to update if possible.</param>
private IEnumerable<object> TryUpdateFromModDump(ModData mod)
{
	// validate
	if (mod.InstalledVersion is null)
	{
		yield return Util.WithStyle("Can't auto-update because the installed version is unknown.", ErrorStyle);
		yield break;
	}

	// get latest version from mod dump
	ParsedFile latestUpdate = null;
	{
		if (!this.ModDump.IsValueCreated)
			yield return Util.WithStyle("Reading mod dump...", TraceStyle);
		ILookup<string, ParsedMod> modDump = this.ModDump.Value;

		HashSet<string> modIds = new(mod.IDs, StringComparer.OrdinalIgnoreCase);
		ISemanticVersion latestVersion = mod.InstalledVersion;

		yield return Util.WithStyle($"Checking update keys:", TraceStyle);
		foreach (string rawUpdateKey in mod.UpdateKeys)
		{
			if (!UpdateKey.TryParse(rawUpdateKey, out UpdateKey updateKey))
			{
				yield return Util.WithStyle($"   {rawUpdateKey} — skipped (invalid update key).", TraceStyle);
				continue;
			}

			string lookupKey = $"{updateKey.Site}:{updateKey.ID}";
			ParsedMod[] candidates = modDump[lookupKey].ToArray();
			if (candidates.Length == 0)
			{
				yield return Util.WithStyle($"   {lookupKey} — skipped (no match found in the mod dump).", TraceStyle);
				continue;
			}

			foreach (ParsedMod candidate in candidates)
			{
				foreach (ParsedFile modFolder in candidate.ModFolders)
				{
					string logPrefix = candidate.ModFolders.Length > 1
						? $"'{lookupKey}' > file {modFolder.ID}"
						: $"'{lookupKey}'";

					if (!modIds.Contains(modFolder.ModID))
					{
						yield return Util.WithStyle($"   {logPrefix} — skipped (different mod ID '{modFolder.ModID}').", TraceStyle);
						continue;
					}

					if (!SemanticVersion.TryParse(modFolder.Version, out ISemanticVersion candidateVersion))
					{
						yield return Util.WithStyle($"    {logPrefix} — skipped (its version '{modFolder.Version}' couldn't be parsed).", TraceStyle);
						continue;
					}

					if (!latestVersion.IsOlderThan(candidateVersion))
					{
						yield return Util.WithStyle($"   {logPrefix} — skipped (its version '{candidate.Version}' is older than {latestVersion}).", TraceStyle);
						continue;
					}

					yield return Util.WithStyle($"   {logPrefix} — matched for newer version '{candidate.Version}'.", TraceStyle);
					latestUpdate = modFolder;
					latestVersion = candidateVersion;
				}
			}
		}
	}
	if (latestUpdate is null)
	{
		yield return Util.WithStyle("Can't auto-update because no newer version was found in the mod dump.", ErrorStyle);
		yield break;
	}

	// get paths
	DirectoryInfo fromDir = latestUpdate.RawFolder.Directory;
	DirectoryInfo toDir = mod.Folder.Directory;
	yield return Util.WithStyle($"Updating to version {latestUpdate.Version}:\n  - from: {fromDir.FullName};\n  - to: {toDir.FullName}.", TraceStyle);
	if (toDir.Exists)
	{
		FileHelper.ForceDelete(toDir);
		toDir.Create();
	}

	// copy mod
	foreach (FileInfo file in fromDir.GetFiles("*", SearchOption.AllDirectories))
	{
		string relativePath = Path.GetRelativePath(fromDir.FullName, file.FullName);
		string toPath = Path.Combine(toDir.FullName, relativePath);

		Directory.CreateDirectory(Path.GetDirectoryName(toPath));
		File.Copy(file.FullName, toPath);
	}

	yield return Util.WithStyle("Done!", SuccessStyle);
}

/// <summary>Move the folder into the mod dump repo specified by <see cref="ModDumpFolderPath" />, deleting the previous folder if found.</summary>
/// <param name="mod">The mod whose folder to move.</param>
/// <param name="toolkit">The toolkit with which to scan for a matching mod folder, if any.</param>
private IEnumerable<object> TryMoveToModDump(ModData mod, ModToolkit toolkit)
{
	// validation
	if (mod.Folder.Directory.Name.StartsWith('%'))
	{
		yield return Util.WithStyle("This seems to be a temporary folder (per the '%' prefix in the folder name).", ErrorStyle);
		yield break;
	}

	// get mod ID
	string uniqueId = mod.Folder.Manifest?.UniqueID;
	if (string.IsNullOrWhiteSpace(uniqueId))
	{
		yield return Util.WithStyle("Can't move mod to mod dump because it has no manifest ID.", ErrorStyle);
		yield break;
	}

	// delete mod if it already exists
	// note: the mod may be installed with a different folder name (e.g. with an "[unofficial]" suffix)
	var scanner = new ModScanner(toolkit.JsonHelper);
	var targetDir = new DirectoryInfo(Path.Combine(ModDumpFolderPath, mod.Folder.Directory.Name));
	yield return Util.WithStyle("Removing previous copies...", TraceStyle);
	if (targetDir.Exists)
	{
		ModFolder targetFolder = scanner.ReadFolder(targetDir.Parent, targetDir, true);

		if (!string.Equals(targetFolder.Manifest?.UniqueID, uniqueId, StringComparison.OrdinalIgnoreCase))
		{
			yield return Util.WithStyle($"Can't move mod to mod dump because the '{targetDir.Name}' folder contains a different mod ('{targetFolder.DisplayName}' with unique ID '{targetFolder.Manifest.UniqueID}').", ErrorStyle);
			yield break;
		}

		FileHelper.ForceDelete(targetDir);
		yield return Util.WithStyle($"   Deleted {targetDir.Name}.", TraceStyle);
	}
	else
	{
		foreach (ModFolder folder in scanner.GetModFolders(targetDir.Parent.FullName, true))
		{
			if (string.Equals(folder.Manifest?.UniqueID, uniqueId, StringComparison.OrdinalIgnoreCase))
			{
				FileHelper.ForceDelete(folder.Directory);
				yield return Util.WithStyle($"   Deleted {folder.Directory.Name}.", TraceStyle);
			}
		}
	}

	// move folder
	FileHelper.RecursiveCopy(mod.Folder.Directory, targetDir.Parent);
	FileHelper.ForceDelete(mod.Folder.Directory);
	yield return Util.WithStyle("Done!", SuccessStyle);
}

/// <summary>Install a dependency referenced by another mod.</summary>
/// <param name="mod">The mod ID to install.</param>
private IEnumerable<object> TryInstallDependency(string id)
{
	// get latest version from mod dump
	ParsedFile selectedMod = null;
	{
		if (!this.ModDump.IsValueCreated)
			yield return Util.WithStyle("Reading mod dump...", TraceStyle);
		ILookup<string, ParsedMod> modDump = this.ModDump.Value;

		// find latest version of the target mod
		yield return Util.WithStyle($"Scanning for ID '{id}'...", TraceStyle);
		ISemanticVersion latestVersion = null;
		foreach (ParsedMod modPage in modDump.SelectMany(p => p))
		{
			foreach (ParsedFile modFolder in modPage.ModFolders)
			{
				if (!string.Equals(modFolder.ModID, id, StringComparison.OrdinalIgnoreCase) || !SemanticVersion.TryParse(modFolder.Version, out ISemanticVersion curVersion))
					continue;

				if (latestVersion?.IsOlderThan(curVersion) is false)
					continue;

				latestVersion = curVersion;
				selectedMod = modFolder;
			}
		}
		if (selectedMod is null)
		{
			yield return Util.WithStyle($"No matching mod found in the mod dump.", ErrorStyle);
			yield break;
		}
	}

	// get paths
	DirectoryInfo fromDir = selectedMod.RawFolder.Directory;
	DirectoryInfo toDir = new DirectoryInfo(Path.Combine(this.ModFolderPath, '%' + selectedMod.RawFolder.Directory.Name));
	yield return Util.WithStyle($"Installing '{selectedMod.DisplayName}' version {selectedMod.Version}:\n  - from: {fromDir.FullName};\n  - to: {toDir.FullName}.", TraceStyle);
	if (toDir.Exists)
	{
		FileHelper.ForceDelete(toDir);
		toDir.Create();
	}

	// copy mod
	foreach (FileInfo file in fromDir.GetFiles("*", SearchOption.AllDirectories))
	{
		string relativePath = Path.GetRelativePath(fromDir.FullName, file.FullName);
		string toPath = Path.Combine(toDir.FullName, relativePath);

		Directory.CreateDirectory(Path.GetDirectoryName(toPath));
		File.Copy(file.FullName, toPath);
	}

	yield return Util.WithStyle("Done!", SuccessStyle);
}

/// <summary>Get links for a mod.</summary>
/// <param name="mod">The wiki entry for the mod.</param>
private dynamic GetReportLinks(WikiModEntry mod)
{
	if (mod == null)
		return null;
	
	return new
	{
		Nexus = mod.NexusID.HasValue ? new Hyperlinq($"https://www.nexusmods.com/stardewvalley/mods/{mod.NexusID}", $"Nexus:{mod.NexusID}") : null,
		ModDrop = mod.ModDropID.HasValue ? new Hyperlinq($"https://www.moddrop.com/sdv/mod/{mod.ModDropID}", $"ModDrop:{mod.ModDropID}") : null,
		CurseForge = mod.CurseForgeID.HasValue ? new Hyperlinq($"https://curseforge.com/stardewvalley/mods/{mod.CurseForgeKey}", $"CurseForge:{mod.CurseForgeKey}") : null,
		Chucklefish = mod.ChucklefishID.HasValue ? new Hyperlinq($"https://community.playstarbound.com/resources/{mod.ChucklefishID}", $"Chucklefish:{mod.ChucklefishID}") : null,
		GitHub = mod.GitHubRepo != null ? new Hyperlinq($"https://github.com/{mod.GitHubRepo}", $"GitHub:{mod.GitHubRepo}") : null,
		Custom = mod.CustomUrl != null ? new Hyperlinq(mod.CustomUrl, "custom") : null,
	};
}

/// <summary>Get links for a mod.</summary>
/// <param name="mod">The API metadata for the mod.</param>
private dynamic GetReportLinks(ModEntryModel mod)
{
	if (mod?.Metadata == null)
		return null;

	var meta = mod.Metadata;
	return new
	{
		Nexus = meta.NexusID.HasValue ? new Hyperlinq($"https://www.nexusmods.com/stardewvalley/mods/{meta.NexusID}", $"Nexus:{meta.NexusID}") : null,
		ModDrop = meta.ModDropID.HasValue ? new Hyperlinq($"https://www.moddrop.com/sdv/mod/{meta.ModDropID}", $"ModDrop:{meta.ModDropID}") : null,
		CurseForge = meta.CurseForgeID.HasValue ? new Hyperlinq($"https://stardewvalley.curseforge.com/projects/{meta.CurseForgeKey}", $"CurseForge:{meta.CurseForgeKey}") : null,
		Chucklefish = meta.ChucklefishID.HasValue ? new Hyperlinq($"https://community.playstarbound.com/resources/{meta.ChucklefishID}", $"Chucklefish:{meta.ChucklefishID}") : null,
		GitHub = meta.GitHubRepo != null ? new Hyperlinq($"https://github.com/{meta.GitHubRepo}", $"GitHub:{meta.GitHubRepo}") : null,
		Custom = meta.CustomUrl != null ? new Hyperlinq(meta.CustomUrl, "custom") : null,
	};
}

/// <summary>Get a flattened view of the mod data.</summary>
/// <param name="mods">The mods to represent.</param>
/// <param name="forBeta">Whether to render data for the beta version of Stardew Valley (if any).</param>
IEnumerable<ReportEntry> GetReport(IEnumerable<ModData> mods, bool forBeta)
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
				new { Version = mod.GetUnofficialVersion(forBeta), Url = $"https://stardewvalleywiki.com/Modding:SMAPI_compatibility#{this.GetAnchor(mod.ApiRecord?.Metadata?.Name ?? mod.Folder.DisplayName)}" }
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
			yield return new ReportEntry(mod, latestVersion, downloadUrl, forBeta, ignoreUpdate, this.GetReportLinks(mod.ApiRecord), (Func<string, string>)(p => this.TryFormatVersion(p)));
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

/// <summary>Get whether a folder is empty, ignoring metadata files like the __MACOSX folder.</summary>
/// <param name="directory">The directory to check.</param>
private bool IsEmptyFolder(DirectoryInfo directory)
{
	foreach (FileSystemInfo info in directory.EnumerateFileSystemInfos())
	{
		if (info is DirectoryInfo subdir)
		{
			if (subdir.Name == "__MACOSX")
				continue;

			if (!this.IsEmptyFolder(subdir))
				return false;
		}
		else
			return false;
	}
	
	return true;
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
	/// <param name="forBeta">If there's an ongoing Stardew Valley or SMAPI beta which affects compatibility, whether to return the unofficial update for that beta version instead of the one for the stable version.</param>
	public ISemanticVersion GetUnofficialVersion(bool forBeta)
	{
		return forBeta
			? this.ApiRecord?.Metadata?.UnofficialForBeta?.Version ?? this.ApiRecord?.Metadata?.Unofficial?.Version
			: this.ApiRecord?.Metadata.Unofficial?.Version;
	}

	/// <summary>Get the URL to this mod's web page.</summary>
	public string GetModPageUrl()
	{
		// Nexus ID
		string nexusID = this.GetModID("Nexus", mustBeInt: true);
		if (nexusID != null)
			return $"https://www.nexusmods.com/stardewvalley/mods/{nexusID}";

		// ModDrop ID
		string modDropId = this.GetModID("ModDrop", mustBeInt: true);
		if (modDropId != null)
			return $"https://www.moddrop.com/stardew-valley/mod/{modDropId}";

		// CurseForge key
		if (this.ApiRecord?.Metadata?.CurseForgeKey != null)
			return $"https://stardewvalley.curseforge.com/projects/{this.ApiRecord.Metadata.CurseForgeKey}";

		// Chucklefish ID
		string chucklefishID = this.GetModID("Chucklefish", mustBeInt: true);
		if (chucklefishID != null)
			return $"https://community.playstarbound.com/resources/{chucklefishID}";

		// GitHub key
		string repo = this.GetModID("GitHub");
		if (repo != null)
			return $"https://github.com/{repo}";

		return null;
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

	/// <summary>Get a recommended folder name based on the mod data.</summary>
	public string GetRecommendedFolderName()
	{
		return (this.ApiRecord?.Metadata?.Name ?? this.Folder.DisplayName)
			?.Replace("&#44;", ",");
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

	/// <summary>The compatibility status from the wiki.</summary>
	public WikiCompatibilityStatus? WikiStatus { get; }

	/// <summary>The compatibility 'broke in' field from the wiki.</summary>
	public string WikiBrokeIn { get; }

	/// <summary>The compatibility summary from the wiki.</summary>
	public string WikiSummary { get; }

	/// <summary>The unofficial version from the wiki, if any.</summary>
	public ISemanticVersion WikiUnofficialVersion { get; }

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
	public ReportEntry(ModData mod, ISemanticVersion latestVersion, string downloadUrl, bool forBeta, bool ignoreUpdate, dynamic links, Func<string, string> tryFormatVersion)
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
		if (forBeta)
		{
			this.WikiStatus = apiMetadata?.BetaCompatibilityStatus ?? apiMetadata?.CompatibilityStatus;
			this.WikiSummary = apiMetadata?.BetaCompatibilitySummary ?? apiMetadata?.CompatibilitySummary;
			this.WikiBrokeIn = apiMetadata?.BetaBrokeIn ?? apiMetadata?.BrokeIn;
			this.WikiUnofficialVersion = apiMetadata?.UnofficialForBeta?.Version ?? apiMetadata?.Unofficial?.Version;
		}
		else
		{
			this.WikiStatus = apiMetadata?.CompatibilityStatus;
			this.WikiSummary = apiMetadata?.CompatibilitySummary;
			this.WikiBrokeIn = apiMetadata?.BrokeIn;
		}
		
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
		if (this.WikiStatus == null)
			this.Errors.Add("not on wiki");
		if (this.Installed != null && this.Latest != null && new SemanticVersion(this.Latest).IsOlderThan(this.Installed))
			this.Warnings.Add("official version is older");
		if (this.Installed != null && this.WikiUnofficialVersion != null && this.WikiUnofficialVersion.IsOlderThan(this.Installed))
			this.Warnings.Add("unofficial version on wiki is older");
		if (string.IsNullOrWhiteSpace(this.UpdateKeys))
			this.Warnings.Add("no valid update keys in manifest or wiki");

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
	name = name.Replace(' ', '_');
	return WebUtility
		.UrlEncode(name)
		?.Replace('%', '.')
		.Replace("(", ".28")
		.Replace(")", ".29");
}
