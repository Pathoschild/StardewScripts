<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.CoreInterfaces.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.dll</Reference>
  <NuGetReference>Squid-Box.SevenZipSharp</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.ModScanning</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Serialization</Namespace>
  <Namespace>StardewModdingAPI.Toolkit</Namespace>
  <Namespace>SevenZip</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

#load "ConsoleHelper.linq"
#load "FileHelper.linq"
#load "IncrementalProgressBar.linq"

/// <summary>Manages the mod dump folder containing downloaded versions of every mod.</summary>
public class ModDumpManager
{
	/*********
	** Fields
	*********/
	/// <summary>The path in which to store cached data.</summary>
	private readonly string RootPath;

	/// <summary>Whether to delete the entire unpacked folder and unpack all files from the export path. If this is false, only updated mods will be re-unpacked.</summary>
	private readonly bool ResetUnpacked;

	/// <summary>The settings to use when writing JSON files.</summary>
	private readonly JsonSerializerSettings JsonSettings = JsonHelper.CreateDefaultSettings();


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="rootPath">The path in which to store cached data.</param>
	/// <param name="resetUnpacked">Whether to delete the entire unpacked folder and unpack all files from the export path. If this is false, only updated mods will be re-unpacked.</param>
	public ModDumpManager(string rootPath, bool resetUnpacked)
	{
		this.RootPath = rootPath;
		this.ResetUnpacked = resetUnpacked;
	}

	/// <summary>Get the absolute file path for the JSON file containing a cached representation of the <see cref="ReadMods" /> result.</summary>
	private string GetCacheFilePath()
	{
		return Path.Combine(this.RootPath, "cache.json");
	}

	/// <summary>Delete the cache of mod file data, if it exists.</summary>
	private void DeleteCache()
	{
		File.Delete(this.GetCacheFilePath());
	}

	/// <summary>Unpack all mod downloads in the cache which haven't been unpacked yet.</summary>
	/// <param name="unpackMods">The mod folder names to unpack.</param>
	public void UnpackModDownloads(HashSet<string> unpackMods = null)
	{
		HashSet<string> modFoldersToUnpack = new HashSet<string>(this.GetModFoldersWithFilesNotUnpacked(this.RootPath), StringComparer.InvariantCultureIgnoreCase);
		if (modFoldersToUnpack.Any())
		{
			this.DeleteCache();
			this.UnpackMods(rootPath: this.RootPath, filter: folder => this.ResetUnpacked || unpackMods?.Any(p => folder.FullName.EndsWith(p)) is true || modFoldersToUnpack.Any(p => folder.FullName.EndsWith(p)));
		}
	}

	/// <summary>Remove all files for a mod from the dump.</summary>
	/// <param name="site">The mod site.</param>
	/// <param name="modId">The mod ID within the site.</param>
	public void DeleteMod(ModSite site, string modId)
	{
		string path = Path.Combine(this.RootPath, site.ToString(), modId.ToString());
		if (Directory.Exists(path))
			FileHelper.ForceDelete(new DirectoryInfo(path));
	}

	/// <summary>Get the folder which contains the downloads and extracted files for a mod, creating it if needed. (This is *not* the mod folder; it may contain multiple mod folder which can be downloaded from the same mod page.)</summary>
	/// <param name="site">The mod site.</param>
	/// <param name="modId">The mod ID within the site.</param>
	public DirectoryInfo GetModFolder(ModSite site, string modId)
	{
		DirectoryInfo folder = new DirectoryInfo(Path.Combine(this.RootPath, site.ToString(), modId));

		if (!folder.Exists)
		{
			folder.Create();
			folder.Refresh();
		}

		return folder;
	}

	/// <summary>Get the absolute path to which to save a mod download (e.g. a zip file, not the individual files within the download).</summary>
	/// <param name="site">The mod site.</param>
	/// <param name="modId">The mod ID within the site.</param>
	/// <param name="file">The mod file that will be downloaded.</param>
	public string GetModDownloadPath(ModSite site, string modId, GenericFile file)
	{
		DirectoryInfo modFolder = this.GetModFolder(site, modId);

		string filesPath = Path.Combine(modFolder.FullName, "files");
		Directory.CreateDirectory(filesPath);

		return Path.Combine(filesPath, $"{file.ID}{Path.GetExtension(file.FileName)}");
	}

	/// <summary>Save the mod metadata to its dump folder.</summary>
	/// <param name="mod">The mod metadata to save.</param>
	public void SaveModData(GenericMod mod)
	{
		DirectoryInfo modDir = this.GetModFolder(mod.Site, mod.ID.ToString());
		File.WriteAllText(Path.Combine(modDir.FullName, "mod.json"), JsonConvert.SerializeObject(mod, this.JsonSettings));
	}

	/// <summary>Parse unpacked mod data in the given folder.</summary>
	public IEnumerable<ParsedMod> ReadMods()
	{
		// get from cache
		string cacheFilePath = this.GetCacheFilePath();
		if (File.Exists(cacheFilePath))
		{
			Stopwatch timer = Stopwatch.StartNew();
			string cachedJson = File.ReadAllText(cacheFilePath);
			ParsedMod[] mods = JsonConvert.DeserializeObject<ParsedMod[]>(cachedJson, this.JsonSettings);
			timer.Stop();

			ConsoleHelper.Print($"Read {mods.Length} mods from cache in {ConsoleHelper.GetFormattedTime(timer.Elapsed)}.");

			return mods;
		}

		// read data from each mod's folder
		List<ParsedMod> parsedMods = new();
		ModToolkit toolkit = new ModToolkit();
		foreach (DirectoryInfo siteFolder in this.GetSortedSubfolders(new DirectoryInfo(this.RootPath)))
		{
			Stopwatch timer = Stopwatch.StartNew();

			var modFolders = this.GetSortedSubfolders(siteFolder).ToArray();
			var progress = new IncrementalProgressBar(modFolders.Length).Dump();

			foreach (DirectoryInfo modFolder in modFolders)
			{
				progress.Increment();
				progress.Caption = $"Reading {siteFolder.Name} > {modFolder.Name}...";

				// read metadata files
				GenericMod metadata = JsonConvert.DeserializeObject<GenericMod>(File.ReadAllText(Path.Combine(modFolder.FullName, "mod.json")));
				IDictionary<int, GenericFile> fileMap = metadata.Files.ToDictionary(p => p.ID);

				// load mod folders
				IDictionary<GenericFile, ModFolder[]> unpackedFileFolders = new Dictionary<GenericFile, ModFolder[]>();
				DirectoryInfo unpackedFolder = new DirectoryInfo(Path.Combine(modFolder.FullName, "unpacked"));
				if (unpackedFolder.Exists)
				{
					foreach (DirectoryInfo fileDir in this.GetSortedSubfolders(unpackedFolder))
					{
						if (fileDir.Name == "_tmp")
							continue;

						progress.Caption = $"Reading {siteFolder.Name} > {modFolder.Name} > {fileDir.Name}...";

						// get file data
						GenericFile fileData = fileMap[int.Parse(fileDir.Name)];

						// get mod folders from toolkit
						ModFolder[] mods = toolkit.GetModFolders(rootPath: unpackedFolder.FullName, modPath: fileDir.FullName, useCaseInsensitiveFilePaths: true).ToArray();
						if (mods.Length == 0)
						{
							ConsoleHelper.Print($"   Ignored {fileDir.FullName}, folder is empty?");
							continue;
						}

						// store metadata
						unpackedFileFolders[fileData] = mods;
					}
				}

				// yield mod
				parsedMods.Add(new ParsedMod(metadata, unpackedFileFolders));
			}

			timer.Stop();
			progress.Caption = $"Read {progress.Total} mods from {siteFolder.Name} (100%) in {ConsoleHelper.GetFormattedTime(timer.Elapsed)}";
		}

		// write cache file
		{
			Stopwatch timer = Stopwatch.StartNew();
			string json = JsonConvert.SerializeObject(parsedMods, this.JsonSettings);
			File.WriteAllText(cacheFilePath, json);
			timer.Stop();

			ConsoleHelper.Print($"Created cache with {parsedMods.Count} mods in {ConsoleHelper.GetFormattedTime(timer.Elapsed)}.");
		}

		return parsedMods.ToArray();
	}


	/*********
	** Private methods
	*********/
	/// <summary>Get all mod folders which have files that haven't been unpacked.</summary>
	/// <param name="rootPath">The path containing mod folders.</param>
	private IEnumerable<string> GetModFoldersWithFilesNotUnpacked(string rootPath)
	{
		// unpack files
		foreach (DirectoryInfo siteDir in this.GetSortedSubfolders(new DirectoryInfo(rootPath)))
		{
			foreach (DirectoryInfo modDir in this.GetSortedSubfolders(siteDir))
			{
				// get packed folder
				string filesPath = Path.Combine(modDir.FullName, "files");
				if (!Directory.Exists(filesPath))
					continue;

				// check for files that need unpacking
				string unpackedDirPath = Path.Combine(modDir.FullName, "unpacked");
				foreach (string archiveFilePath in Directory.GetFiles(filesPath))
				{
					string extension = Path.GetExtension(archiveFilePath);
					if (extension == ".exe")
						continue;

					string id = Path.GetFileNameWithoutExtension(archiveFilePath);
					string targetDirPath = Path.Combine(unpackedDirPath, id);
					if (!Directory.Exists(targetDirPath))
					{
						yield return Path.Combine(siteDir.Name, modDir.Name);
						break;
					}
				}
			}
		}
	}

	/// <summary>Unpack all mods in the given folder.</summary>
	/// <param name="rootPath">The path in which to store cached data.</param>
	/// <param name="filter">A filter which indicates whether a mod folder should be unpacked.</param>
	private void UnpackMods(string rootPath, Func<DirectoryInfo, bool> filter)
	{
		SevenZipBase.SetLibraryPath(Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess
			? @"C:\Program Files (x86)\7-Zip\7z.dll"
			: @"C:\Program Files\7-Zip\7z.dll"
		);

		foreach (DirectoryInfo siteDir in new DirectoryInfo(rootPath).EnumerateDirectories())
		{
			// get folders to unpack
			DirectoryInfo[] modDirs = this
				.GetSortedSubfolders(siteDir)
				.Where(filter)
				.ToArray();
			if (!modDirs.Any())
				continue;

			// unpack files
			Stopwatch timer = Stopwatch.StartNew();
			var progress = new IncrementalProgressBar(modDirs.Count()).Dump();
			foreach (DirectoryInfo modDir in modDirs)
			{
				progress.Increment();
				progress.Caption = $"Unpacking {siteDir.Name} > {modDir.Name} ({progress.Percent}%)...";

				// get packed folder
				DirectoryInfo packedDir = new DirectoryInfo(Path.Combine(modDir.FullName, "files"));
				if (!packedDir.Exists)
					continue;

				// create/reset unpacked folder
				DirectoryInfo unpackedDir = new DirectoryInfo(Path.Combine(modDir.FullName, "unpacked"));
				if (unpackedDir.Exists)
				{
					FileHelper.ForceDelete(unpackedDir);
					unpackedDir.Refresh();
				}
				unpackedDir.Create();

				// unzip each download
				foreach (FileInfo archiveFile in packedDir.GetFiles())
				{
					ConsoleHelper.AutoRetry(() =>
					{
						progress.Caption = $"Unpacking {siteDir.Name} > {modDir.Name} > {archiveFile.Name} ({progress.Percent}%)...";

						// validate
						if (archiveFile.Extension == ".exe")
						{
							ConsoleHelper.Print($"  Skipped {archiveFile.FullName} (not an archive).", Severity.Error);
							return;
						}

						// unzip into temporary folder
						string id = Path.GetFileNameWithoutExtension(archiveFile.Name);
						DirectoryInfo tempDir = new DirectoryInfo(Path.Combine(unpackedDir.FullName, "_tmp", $"{archiveFile.Name}"));
						if (tempDir.Exists)
							FileHelper.ForceDelete(tempDir);
						tempDir.Create();
						tempDir.Refresh();

						try
						{
							this.ExtractFile(archiveFile, tempDir);
						}
						catch (Exception ex)
						{
							ConsoleHelper.Print($"  Could not unpack {archiveFile.FullName}:\n{(ex is SevenZipArchiveException ? ex.Message : ex.ToString())}", Severity.Error);
							Console.WriteLine();
							FileHelper.ForceDelete(tempDir);
							return;
						}

						// move into final location
						if (tempDir.EnumerateFiles().Any() || tempDir.EnumerateDirectories().Count() > 1) // no root folder in zip
							tempDir.Parent.MoveTo(Path.Combine(unpackedDir.FullName, id));
						else
						{
							tempDir.MoveTo(Path.Combine(unpackedDir.FullName, id));
							FileHelper.ForceDelete(new DirectoryInfo(Path.Combine(unpackedDir.FullName, "_tmp")));
						}
					});
				}
			}
			timer.Stop();
			progress.Caption = $"Unpacked {progress.Total} mods from {siteDir.Name} in {ConsoleHelper.GetFormattedTime(timer.Elapsed)} (100%)";
		}
	}

	/// <summary>Extract an archive file to the given folder.</summary>
	/// <param name="file">The archive file to extract.</param>
	/// <param name="extractTo">The directory to extract into.</param>
	private void ExtractFile(FileInfo file, DirectoryInfo extractTo)
	{
		try
		{
			CancellationTokenSource cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(60));
			Task
				.Run(() =>
				{
					using (SevenZipExtractor unpacker = new SevenZipExtractor(file.FullName))
						unpacker.ExtractArchive(extractTo.FullName);
				}, cancellation.Token)
				.Wait();
		}
		catch (AggregateException outerEx)
		{
			throw outerEx.InnerException;
		}
	}

	/// <summary>Get the subfolders of a given folder sorted by numerical or alphabetical order.</summary>
	/// <param name="root">The folder whose subfolders to get.</param>
	private IEnumerable<DirectoryInfo> GetSortedSubfolders(DirectoryInfo root)
	{
		return
			(
				from subfolder in root.GetDirectories()
				let isNumeric = int.TryParse(subfolder.Name, out int _)
				let numericName = isNumeric ? int.Parse(subfolder.Name) : int.MaxValue
				orderby numericName, subfolder.Name
				select subfolder
			);
	}
}


/// <summary>Get a clone of the input as a raw data dictionary.</summary>
/// <param name="data">The input data to clone.</param>
private static Dictionary<string, object> CloneToDictionary(object data)
{
	switch (data)
	{
		case null:
			return new Dictionary<string, object>();

		case JObject obj:
			return obj.DeepClone().ToObject<Dictionary<string, object>>();

		default:
			return JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(data));
	}
}

/// <summary>The identifier for a mod site used in update keys.</summary>
public enum ModSite
{
	/// <summary>The CurseForge site.</summary>
	CurseForge,

	/// <summary>The CurseForge site.</summary>
	ModDrop,

	/// <summary>The Nexus Mods site.</summary>
	Nexus
}

/// <summary>Metadata for a mod from any mod site.</summary>
public class GenericMod
{
	/*********
	** Accessors
	*********/
	/// <summary>The mod site which has the mod.</summary>
	public ModSite Site { get; }

	/// <summary>The mod ID within the site.</summary>
	public int ID { get; }

	/// <summary>The mod display name.</summary>
	public string Name { get; }

	/// <summary>The mod author name.</summary>
	public string Author { get; }

	/// <summary>Custom author text, if different from <see cref="Author" />.</summary>
	public string AuthorLabel { get; }

	/// <summary>The URL to the user-facing mod page.</summary>
	public string PageUrl { get; }

	/// <summary>The main mod version, if applicable.</summary>
	public string Version { get; }

	/// <summary>When the mod metadata or files were last updated.</summary>
	public DateTimeOffset Updated { get; }

	/// <summary>The original data from the mod site.</summary>
	public Dictionary<string, object> RawData { get; }

	/// <summary>The available mod downloads.</summary>
	public GenericFile[] Files { get; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="site">The mod site which has the mod.</param>
	/// <param name="id">The mod ID within the site.</param>
	/// <param name="name">The mod display name.</param>
	/// <param name="author">The mod author name.</param>
	/// <param name="authorLabel">Custom author text, if different from <paramref name="author" />.</param>
	/// <param name="pageUrl">The URL to the user-facing mod page.</param>
	/// <param name="version">The main mod version, if applicable.</param>
	/// <param name="updated">When the mod metadata or files were last updated.</param>
	/// <param name="rawData">The original data from the mod site.</param>
	/// <param name="files">The available mod downloads.</param>
	public GenericMod(ModSite site, int id, string name, string author, string authorLabel, string pageUrl, string version, DateTimeOffset updated, object rawData, GenericFile[] files)
	{
		this.Site = site;
		this.ID = id;
		this.Name = name;
		this.Author = author;
		this.AuthorLabel = authorLabel;
		this.PageUrl = pageUrl;
		this.Version = version;
		this.Updated = updated;
		this.RawData = UserQuery.CloneToDictionary(rawData);
		this.Files = files;
	}
}

/// <summary>Parsed data about a mod page.</summary>
public class ParsedMod : GenericMod
{
	/*********
	** Accessors
	*********/
	/// <summary>The parsed mod folders.</summary>
	public ParsedFile[] ModFolders { get; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="mod">The raw mod metadata.</param>
	/// <param name="downloads">The raw mod download data.</param>
	public ParsedMod(GenericMod mod, IDictionary<GenericFile, ModFolder[]> downloads)
		: base(site: mod.Site, id: mod.ID, name: mod.Name, author: mod.Author, authorLabel: mod.AuthorLabel, pageUrl: mod.PageUrl, version: mod.Version, updated: mod.Updated, rawData: mod.RawData, files: mod.Files)
	{
		try
		{
			// set mod folders
			this.ModFolders =
				(
					from entry in downloads
					from folder in entry.Value
					select new ParsedFile(entry.Key, folder)
				)
				.ToArray();
		}
		catch (Exception)
		{
			new { mod, downloads }.Dump("failed parsing mod data");
			throw;
		}
	}

	/// <inheritdoc />
	/// <param name="modFolders">The parsed mod folders.</param>
	[JsonConstructor]
	public ParsedMod(ModSite site, int id, string name, string author, string authorLabel, string pageUrl, string version, DateTimeOffset updated, object rawData, GenericFile[] files, ParsedFile[] modFolders)
		: base(site, id, name, author, authorLabel, pageUrl, version, updated, rawData, files)
	{
		this.ModFolders = modFolders;
	}
}

/// <summary>A file category on a mod site.</summary>
public enum GenericFileType
{
	/// <summary>The primary download.</summary>
	Main,

	/// <summary>A secondary download, often for preview or beta versions.</summary>
	Optional
}

/// <summary>Metadata for a mod download on any mod site.</summary>
public class GenericFile
{
	/*********
	** Accessors
	*********/
	/// <summary>The file ID.</summary>
	public int ID { get; set; }

	/// <summary>The file type.</summary.
	public GenericFileType Type { get; set; }

	/// <summary>The display name on the mod site.</summary>
	public string DisplayName { get; set; }

	/// <summary>The file name on the mod site.</summary>
	public string FileName { get; set; }

	/// <summary>The file version on the mod site.</summary>
	public string Version { get; set; }

	/// <summary>The original file data from the mod site.</summary>
	public Dictionary<string, object> RawData { get; set; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	public GenericFile() { }

	/// <summary>Construct an instance.</summary>
	/// <param name="id">The file ID.</param>
	/// <param name="type">The file type.</param>
	/// <param name="displayName">The display name on the mod site.</param>
	/// <param name="fileName">The filename on the mod site.</param>
	/// <param name="version">The file version on the mod site.</param>
	/// <param name="rawData">The original file data from the mod site.</param>
	public GenericFile(int id, GenericFileType type, string displayName, string fileName, string version, object rawData)
	{
		this.ID = id;
		this.Type = type;
		this.DisplayName = displayName;
		this.FileName = fileName;
		this.Version = version;
		this.RawData = UserQuery.CloneToDictionary(rawData);
	}
}

/// <summary>Parsed data about a mod download.</summary>
public class ParsedFile : GenericFile
{
	/*********
	** Accessors
	*********/
	/// <summary>The mod display name based on the manifest.</summary>
	public string ModDisplayName { get; }

	/// <summary>The mod type.</summary>
	public ModType ModType { get; }

	/// <summary>The mod parse error, if it could not be parsed.</summary>
	public ModParseError? ModError { get; }

	/// <summary>The mod ID from the manifest.</summary>
	public string ModID { get; }

	/// <summary>The mod version from the manifest.</summary>
	public string ModVersion { get; }

	/// <summary>The raw parsed mod folder.</summary>
	public ModFolder RawFolder { get; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="download">The raw mod file.</param>
	/// <param name="folder">The raw parsed mod folder.</param>
	public ParsedFile(GenericFile download, ModFolder folder)
		: base(id: download.ID, type: download.Type, displayName: download.DisplayName, fileName: download.FileName, version: download.Version, rawData: download.RawData)
	{
		this.RawFolder = folder;

		this.ModDisplayName = folder.DisplayName;
		this.ModType = folder.Type;
		this.ModError = folder.ManifestParseError == ModParseError.None ? (ModParseError?)null : folder.ManifestParseError;
		this.ModID = folder.Manifest?.UniqueID;
		this.ModVersion = folder.Manifest?.Version?.ToString();
	}

	/// <inheritdoc />
	/// <param name="modDisplayName">The mod display name based on the manifest.</param>
	/// <param name="modType">The mod type.</param>
	/// <param name="modError">The mod parse error, if it could not be parsed.</param>
	/// <param name="modId">The mod ID from the manifest.</param>
	/// <param name="modVersion">The mod version from the manifest.</param>
	/// <param name="rawFolder">The raw parsed mod folder.</param>
	[JsonConstructor]
	public ParsedFile(int id, GenericFileType type, string displayName, string fileName, string version, object rawData, string modDisplayName, ModType modType, ModParseError? modError, string modId, string modVersion, ModFolder rawFolder)
		: base(id, type, displayName, fileName, version, rawData)
	{
		this.ModDisplayName = modDisplayName;
		this.ModType = modType;
		this.ModError = modError;
		this.ModID = modId;
		this.ModVersion = modVersion;
		this.RawFolder = rawFolder;
	}
}