<Query Kind="Program">
  <Namespace>StardewModdingAPI</Namespace>
  <Namespace>StardewModdingAPI.Toolkit</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.UpdateData</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModScanning</Namespace>
</Query>

#load "ConsoleHelper.linq"
#load "FileHelper.linq"
#load "IncrementalProgressBar.linq"
#load "ModCache.linq"

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
	/// <summary>The absolute path for the folder containing the mod cache.</summary>
	private readonly string ModCachePath;

	/// <summary>The absolute path for the folder containing installed mods.</summary>
	private readonly string InstalledModsPath;

	/// <summary>The backing field for <see cref="Cache"/>.</summary>
	private readonly Lazy<ModCache> ModCacheImpl;

	/// <summary>The mod folders in the underlying mod cache, indexed by site and mod ID (like "Nexus:2400" for SMAPI's Nexus page).</summary>
	private Lazy<ILookup<string, ParsedMod>> ModFoldersBySiteId;


	/*********
	** Properties
	*********/
	/// <summary>The underlying mod cache.</summary>
	public ModCache Cache => this.ModCacheImpl.Value;


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="modCachePath">The absolute path for the folder containing the mod cache.</param>
	/// <param name="installedModsPath">The absolute path for the folder containing installed mods.</param>
	public ModCacheUtilities(string modCachePath, string installedModsPath)
	{
		this.ModCachePath = modCachePath;
		this.InstalledModsPath = installedModsPath;
		this.ModCacheImpl = new(() => new ModCache(modCachePath));

		this.ReloadFromModCache();
	}

	/// <summary>Reset higher-level caches if the underlying mod cache changed.</summary>
	public void ReloadFromModCache()
	{
		this.ModFoldersBySiteId = new(() => this.ModCacheImpl.Value.ReadUnpackedModFolders().ToLookup(mod => $"{mod.Site}:{mod.ID}"));
	}

	/// <summary>Install a mod from the mod dump.</summary>
	/// <param name="mod">The mod ID to install.</param>
	/// <param name="folderNamePrefix">A string to prepend to the original folder name when it's added to the installed-mods folder, if any.</param>
	public IEnumerable<object> TryInstall(string id, string folderNamePrefix = null)
	{
		// get latest version from mod dump
		ParsedFile selectedMod = null;
		{
			if (!this.ModCacheImpl.IsValueCreated)
				yield return Util.WithStyle("Reading mod dump...", ConsoleHelper.TraceStyle);
			ILookup<string, ParsedMod> modFoldersBySiteId = this.ModFoldersBySiteId.Value;

			// find latest version of the target mod
			yield return Util.WithStyle($"Scanning for ID '{id}'...", ConsoleHelper.TraceStyle);
			ISemanticVersion latestVersion = null;
			foreach (ParsedMod modPage in modFoldersBySiteId.SelectMany(p => p))
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
				yield return Util.WithStyle($"No matching mod found in the mod dump.", ConsoleHelper.ErrorStyle);
				yield break;
			}
		}

		foreach (object entry in TryInstall(selectedMod, folderNamePrefix))
			yield return entry;
	}

	/// <summary>Install a mod from the mod dump.</summary>
	/// <param name="folder">The mod folder to install.</param>
	/// <param name="folderNamePrefix">A string to prepend to the original folder name when it's added to the installed-mods folder, if any.</param>
	/// <param name="deleteTargetFolder">Whether to delete the target folder if it already exists.</param>
	public IEnumerable<object> TryInstall(ParsedFile folder, string folderNamePrefix = null, bool deleteTargetFolder = true)
	{
		// get paths
		DirectoryInfo fromDir = folder.RawFolder.Directory;
		DirectoryInfo toDir = new DirectoryInfo(Path.Combine(this.InstalledModsPath, folderNamePrefix + fromDir.Name));
		yield return Util.WithStyle($"Installing '{folder.DisplayName}' version {folder.Version}:\n  - from: {fromDir.FullName};\n  - to: {toDir.FullName}.", ConsoleHelper.TraceStyle);
		if (toDir.Exists)
		{
			if (!deleteTargetFolder)
			{
				yield return Util.WithStyle($"Target mod folder already exists.", ConsoleHelper.ErrorStyle);
				yield break;
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

		yield return Util.WithStyle("Done!", ConsoleHelper.SuccessStyle);
	}

	/// <summary>If a newer version of a mod exists in the mod dump folder, replace the installed version with those newer files.</summary>
	/// <param name="mod">The mod to update if possible.</param>
	public IEnumerable<object> TryUpdateFromModCache(DirectoryInfo targetFolder, string[] modIds, string[] updateKeys, ISemanticVersion installedVersion)
	{
		// validate
		if (installedVersion is null)
		{
			yield return Util.WithStyle("Can't auto-update because the installed version is unknown.", ConsoleHelper.ErrorStyle);
			yield break;
		}

		// get latest version from mod dump
		ParsedFile latestUpdate = null;
		{
			if (!this.ModFoldersBySiteId.IsValueCreated)
				yield return Util.WithStyle("Reading mod dump...", ConsoleHelper.TraceStyle);
			ILookup<string, ParsedMod> modFoldersBySiteId = this.ModFoldersBySiteId.Value;

			HashSet<string> modIdsSet = new(modIds, StringComparer.OrdinalIgnoreCase);
			ISemanticVersion latestVersion = installedVersion;

			yield return Util.WithStyle($"Checking update keys:", ConsoleHelper.TraceStyle);
			foreach (string rawUpdateKey in updateKeys)
			{
				if (!UpdateKey.TryParse(rawUpdateKey, out UpdateKey updateKey))
				{
					yield return Util.WithStyle($"   {rawUpdateKey} — skipped (invalid update key).", ConsoleHelper.TraceStyle);
					continue;
				}

				string lookupKey = $"{updateKey.Site}:{updateKey.ID}";
				ParsedMod[] candidates = modFoldersBySiteId[lookupKey].ToArray();
				if (candidates.Length == 0)
				{
					yield return Util.WithStyle($"   {lookupKey} — skipped (no match found in the mod dump).", ConsoleHelper.TraceStyle);
					continue;
				}

				foreach (ParsedMod candidate in candidates)
				{
					foreach (ParsedFile modFolder in candidate.ModFolders)
					{
						string logPrefix = candidate.ModFolders.Length > 1
							? $"'{lookupKey}' > file {modFolder.ID}"
							: $"'{lookupKey}'";

						if (!modIdsSet.Contains(modFolder.ModID))
						{
							yield return Util.WithStyle($"   {logPrefix} — skipped (different mod ID '{modFolder.ModID}').", ConsoleHelper.TraceStyle);
							continue;
						}

						if (!SemanticVersion.TryParse(modFolder.Version, out ISemanticVersion candidateVersion))
						{
							yield return Util.WithStyle($"    {logPrefix} — skipped (its version '{modFolder.Version}' couldn't be parsed).", ConsoleHelper.TraceStyle);
							continue;
						}

						if (!latestVersion.IsOlderThan(candidateVersion))
						{
							yield return Util.WithStyle($"   {logPrefix} — skipped (its version '{candidate.Version}' is older than {latestVersion}).", ConsoleHelper.TraceStyle);
							continue;
						}

						yield return Util.WithStyle($"   {logPrefix} — matched for newer version '{candidate.Version}'.", ConsoleHelper.TraceStyle);
						latestUpdate = modFolder;
						latestVersion = candidateVersion;
					}
				}
			}
		}
		if (latestUpdate is null)
		{
			yield return Util.WithStyle("Can't auto-update because no newer version was found in the mod dump.", ConsoleHelper.ErrorStyle);
			yield break;
		}

		// get paths
		DirectoryInfo fromDir = latestUpdate.RawFolder.Directory;
		DirectoryInfo toDir = targetFolder;
		yield return Util.WithStyle($"Updating to version {latestUpdate.Version}:\n  - from: {fromDir.FullName};\n  - to: {toDir.FullName}.", ConsoleHelper.TraceStyle);
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

		yield return Util.WithStyle("Done!", ConsoleHelper.SuccessStyle);
	}
}