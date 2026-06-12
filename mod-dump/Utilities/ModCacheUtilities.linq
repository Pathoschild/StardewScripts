<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.CoreInterfaces.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.dll</Reference>
  <Namespace>StardewModdingAPI</Namespace>
  <Namespace>StardewModdingAPI.Toolkit</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.CompatibilityRepo</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModDataset</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModScanning</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.UpdateData</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>

#load "ConsoleHelper.linq"
#load "FileHelper.linq"

/// <summary>Provides higher-level utilities for syncing mods between the mod cache (containing mods downloaded automatically mod sites) and installed mods (for the installed version of Stardew Valley).</summary>
public class ModCacheUtilities
{
	/*********
	** Constants
	*********/
	/// <summary>A prefix in a folder name which indicates that it's a temporary folder installed as part of testing another mod (e.g. a dependency), so it be ignored by normalization and analysis as needed.</summary>
	public const string TemporaryFolderPrefix = "%";


	/*********
	** Fields
	*********/
	/// <summary>The absolute path for the folder containing the mod data repo.</summary>
	private readonly string ModDataRepoPath;

	/// <summary>The absolute path for the folder containing installed mods.</summary>
	private readonly string InstalledModsPath;


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="modDataRepoPath">The absolute path for the folder containing the mod data repo.</param>
	/// <param name="installedModsPath">The absolute path for the folder containing installed mods.</param>
	public ModCacheUtilities(string modDataRepoPath, string installedModsPath)
	{
		this.ModDataRepoPath = modDataRepoPath;
		this.InstalledModsPath = installedModsPath;
	}

	/// <summary>Get the full path to a mod's downloaded files in the mod dump.</summary>
	/// <param name="modPage">The mod page record.</param>
	/// <param name="download">The download record.</param>
	public string GetModDumpFolder(ModPageRecord modPage, ModPageDownloadRecord download)
	{
		string relativePath = this.GetModDataRelativePath(modPage.Site, modPage.Id, download.Id);
		return Path.Combine(this.ModDataRepoPath, "mod-dump", relativePath);
	}

	/// <summary>Get the full path to a mod's downloaded files in the mod dump.</summary>
	/// <param name="modPage">The mod page record.</param>
	/// <param name="download">The download record.</param>
	/// <param name="mod">The mod within the download.</param>
	public string GetModDumpFolder(ModPageRecord modPage, ModPageDownloadRecord download, ModFolderRecord mod)
	{
		return Path.Combine(
			this.GetModDumpFolder(modPage, download),
			mod.RelativePath ?? ""
		);
	}

	/// <summary>Get the relative path to a mod's folder within the mod data or mod dump.</summary>
	/// <param name="modSite">The mod site.</param>
	/// <param name="modPageId">The mod page ID within the site.</param>
	/// <param name="downloadId">The download ID, or <c>null</c> to get the folder for the mod page.</param>
	public string GetModDataRelativePath(ModSite modSite, long modPageId, long? downloadId)
	{
		long bucket = modSite switch
		{
			ModSite.CurseForge or ModSite.ModDrop => modPageId / 10_000,
			_ => modPageId / 1_000
		};

		return Path.Combine(
			modSite.ToString(),
			bucket.ToString(),
			modPageId.ToString(),
			downloadId?.ToString() ?? ""
		);
	}

	/// <summary>Install a mod from the mod dump.</summary>
	/// <param name="mod">The mod ID to install.</param>
	/// <param name="folderNamePrefix">A string to prepend to the original folder name when it's added to the installed-mods folder, if any.</param>
	/// <param name="compatibilityEntry">If set, the mod data from the compatibility entry to use to help select the correct mod.</param>
	/// <param name="deleteTargetFolder">Whether to delete the target folder if it already exists.</param>
	public List<object> TryInstallByModId(string id, string folderNamePrefix = null, ModCompatibilityEntry compatibilityEntry = null, bool deleteTargetFolder = true)
	{
		List<object> log = new();

		// get candidates
		List<(ModPageRecord ModPage, ModPageDownloadRecord Download, ModFolderRecord Mod, ISemanticVersion Version)> candidates = [];
		{
			// read mod ID index
			log.Add(Util.WithStyle($"Loading mods-by-ID index...", ConsoleHelper.TraceStyle));
			string indexPath = Path.Combine(this.ModDataRepoPath, "data-indexes", "mod IDs.json");
			if (!File.Exists(indexPath))
				throw new FileNotFoundException($"Could not find mods-by-ID index at {indexPath}.");
			using FileStream indexStream = File.OpenRead(indexPath);
			var index = JsonSerializer.Deserialize<Dictionary<ModSite, Dictionary<string, long[]>>>(indexStream);

			// collect candidates
			log.Add(Util.WithStyle($"Searching for candidates...", ConsoleHelper.TraceStyle));
			foreach ((ModSite modSite, Dictionary<string, long[]> pagesByModId) in index)
			{
				foreach ((string modId, long[] pageIds) in pagesByModId)
				{
					if (!modId.Equals(id, StringComparison.OrdinalIgnoreCase))
						continue;

					foreach (long pageId in pageIds)
					{
						string relativePath = this.GetModDataRelativePath(modSite, pageId, null);
						string jsonPath = Path.Combine(this.ModDataRepoPath, "data", $"{relativePath}.json");
						if (!File.Exists(jsonPath))
							throw new FileNotFoundException($"Couldn't find mod metadata at path {jsonPath}.");

						using FileStream fileStream = File.OpenRead(jsonPath);
						var modPage = JsonSerializer.Deserialize<ModPageRecord>(fileStream);

						foreach (var download in modPage.Downloads)
						{
							foreach (var mod in download.Mods)
							{
								if (id.Equals(mod.Id, StringComparison.OrdinalIgnoreCase) && SemanticVersion.TryParse(mod.Manifest?.Version, out ISemanticVersion version))
									candidates.Add((modPage, download, mod, version));
							}
						}
					}
				}
			}

			if (candidates.Count == 0)
			{
				log.Add(Util.WithStyle($"No matching mod found in the open mod dataset.", ConsoleHelper.ErrorStyle));
				return log;
			}
		}

		// sort by priority order
		candidates.Sort((a, b) =>
		{
			// official pages first
			if (compatibilityEntry != null)
			{
				bool left = compatibilityEntry.HasSiteId((ModSiteKey)a.ModPage.Site, a.ModPage.Id);
				bool right = compatibilityEntry.HasSiteId((ModSiteKey)b.ModPage.Site, b.ModPage.Id);

				if (left != right)
					return right.CompareTo(left);
			}

			// then by version number
			if (a.Version != null || b.Version != null)
			{
				if (a.Version is null)
					return 1;
				if (b.Version is null)
					return -1;

				int comparison = b.Version.CompareTo(a.Version);
				if (comparison != 0)
					return comparison;
			}

			// then by site ID (lowest first)
			if (a.ModPage.Site == b.ModPage.Site)
				return a.ModPage.Id.CompareTo(b.ModPage.Id);

			return 0;
		});

		// select best match
		var match = candidates[0];
		log.Add(
			Util.WithStyle(
				$"Found {candidates.Count} matches. In priority order:\n" + string.Join("\n", candidates.Select((p, index) => $"  {index + 1}. {p.ModPage.Site}:{p.ModPage.Id} {p.Mod.DisplayName} -- {p.Download.DisplayName} -- {p.Version}")),
				ConsoleHelper.TraceStyle
			)
		);

		// install match
		log.AddRange(
			this.TryInstall(match.ModPage, match.Download, match.Mod, out _, folderNamePrefix, deleteTargetFolder)
		);
		return log;
	}

	/// <summary>Install a mod from the mod dump.</summary>
	/// <param name="mod">The mod ID to install.</param>
	/// <param name="folderNamePrefix">A string to prepend to the original folder name when it's added to the installed-mods folder, if any.</param>
	/// <param name="compatibilityEntry">If set, the mod data from the compatibility entry to use to help select the correct mod.</param>
	/// <param name="success">Whether the mod was successfully installed.</param>
	/// <param name="folderNamePrefix">A string to prepend to the original folder name when it's added to the installed-mods folder, if any.</param>
	/// <param name="deleteTargetFolder">Whether to delete the target folder if it already exists.</param>
	public List<object> TryInstall(ModPageRecord modPage, ModPageDownloadRecord download, ModFolderRecord mod, out bool success, string folderNamePrefix = null, bool deleteTargetFolder = true)
	{
		string modDirPath = this.GetModDumpFolder(modPage, download, mod);
		success = TryInstall(modDirPath, mod.Id, mod.DisplayName, mod.Manifest?.Version, out List<object> log, folderNamePrefix, deleteTargetFolder);
		return log;
	}

	/// <summary>Install a mod from the mod dump.</summary>
	/// <param name="fromDirPath">The mod directory path to install.</param>
	/// <param name="modId">The unique mod ID.</param>
	/// <param name="modDisplayName">The mod's display name.</param>
	/// <param name="modVersion">The mod's display version.</param>
	/// <param name="log">A formatted list of log messages to display.</param>
	/// <param name="folderNamePrefix">A string to prepend to the original folder name when it's added to the installed-mods folder, if any.</param>
	/// <param name="deleteTargetFolder">Whether to delete the target folder if it already exists.</param>
	/// <returns>Returns whether the mod was successfully installed (or was already installed).</returns>
	public bool TryInstall(string fromDirPath, string modId, string modDisplayName, string modVersion, out List<object> log, string folderNamePrefix = null, bool deleteTargetFolder = true)
	{
		log = new();

		// get paths
		DirectoryInfo fromDir = new DirectoryInfo(fromDirPath);
		DirectoryInfo toDir = new DirectoryInfo(Path.Combine(this.InstalledModsPath, folderNamePrefix + modId));
		log.Add(Util.WithStyle($"Installing '{modDisplayName}' version {modVersion}:\n  - from: {fromDir.FullName};\n  - to: {toDir.FullName}.", ConsoleHelper.TraceStyle));
		if (toDir.Exists)
		{
			if (!deleteTargetFolder)
			{
				log.Add(Util.WithStyle($"Target mod folder already exists.", ConsoleHelper.ErrorStyle));
				return true;
			}

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

		log.Add(Util.WithStyle("Done!", ConsoleHelper.SuccessStyle));
		return true;
	}
}
