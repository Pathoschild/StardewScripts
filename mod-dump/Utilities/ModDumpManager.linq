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

/// <summary>Manages the mod dump folder containing metadata and downloads for every mod.</summary>
public class ModCache
{
	/*********
	** Fields
	*********/
	/// <summary>The path in which cached data is stored.</summary>
	private readonly string CachePath;

	/// <summary>The settings to use when writing JSON files.</summary>
	private readonly JsonSerializerSettings JsonSettings = JsonHelper.CreateDefaultSettings();


	/*********
	** Accessors
	*********/
	/// <summary>The cached mods.</summary>
	public Dictionary<ModSite, Dictionary<long, GenericMod>> Mods { get; }
	

	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="rootPath">The path in which cached data is stored.</param>
	public ModCache(string cachePath)
	{
		this.CachePath = cachePath;

		string cacheFilePath = this.GetModsCacheFilePath();
		this.Mods = File.Exists(cacheFilePath)
			? JsonConvert.DeserializeObject<Dictionary<ModSite, Dictionary<long, GenericMod>>>(File.ReadAllText(cacheFilePath))
			: new();

		SevenZipBase.SetLibraryPath(Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess
			? @"C:\Program Files (x86)\7-Zip\7z.dll"
			: @"C:\Program Files\7-Zip\7z.dll"
		);
	}

	/// <summary>Get the mods cached for a site, if any.</summary>
	/// <param name="site">The mod site key.</param>
	public IEnumerable<GenericMod> GetModsFor(ModSite site)
	{
		return this.Mods.TryGetValue(site, out Dictionary<long, GenericMod> mods)
			? mods.Values
			: Array.Empty<GenericMod>();
	}

	/// <summary>Get a mod cached for a site, if any.</summary>
	/// <param name="site">The mod site key.</param>
	/// <param name="modId">The mod ID within the site.</param>
	public GenericMod GetMod(ModSite site, long modId)
	{
		return this.Mods.TryGetValue(site, out Dictionary<long, GenericMod> mods) && mods.TryGetValue(modId, out GenericMod mod)
			? mod
			: null;
	}

	/// <summary>Add a mod to the cache.</summary>
	/// <param name="mod">The mod to cache.</param>
	public void AddMod(GenericMod mod)
	{
		if (!this.Mods.TryGetValue(mod.Site, out Dictionary<long, GenericMod> mods))
			this.Mods[mod.Site] = mods = new();

		mods[mod.ID] = mod;
	}

	/// <summary>Fully delete a mod from the cache.</summary>
	/// <param name="site">The mod site.</param>
	/// <param name="modId">The mod ID within the site.</param>
	public void DeleteMod(ModSite site, long modId)
	{
		if (this.Mods.TryGetValue(site, out Dictionary<long, GenericMod> mods))
			mods.Remove(modId);

		string dirPath = Path.Combine(this.CachePath, site.ToString(), modId.ToString());
		if (Directory.Exists(dirPath))
			FileHelper.ForceDelete(new DirectoryInfo(dirPath));
	}

	/// <summary>Get the absolute path for the folder containing a mod's downloaded files.</summary>
	/// <param name="site">The mod site.</param>
	/// <param name="modId">The mod ID within the site.</param>
	public string GetModFolderPath(ModSite site, long modId)
	{
		return Path.Combine(this.CachePath, site.ToString(), modId.ToString());
	}

	/// <summary>Get the absolute path for the folder containing a mod's downloaded files.</summary>
	/// <param name="site">The mod site.</param>
	/// <param name="modId">The mod ID within the site.</param>
	public string GetModFilePath(ModSite site, long modId, GenericFile file)
	{
		string dirPath = Path.Combine(this.CachePath, site.ToString(), modId.ToString());

		Directory.CreateDirectory(dirPath);
		return Path.Combine(dirPath, $"temp_{file.ID}{Path.GetExtension(file.FileName)}");
	}

	/// <summary>Unpack a downloaded mod file.</summary>
	/// <param name="site">The mod site.</param>
	/// <param name="modId">The mod ID within the site.</param>
	/// <param name="error">An error indicating that unpacking failed, if applicable.</param>
	public bool TryUnpackFile(ModSite site, long modId, GenericFile file, out string error)
	{
		FileInfo download = new FileInfo(this.GetModFilePath(site, modId, file));
		if (!download.Exists)
			throw new InvalidOperationException($"Can't unpack file {site}:{modId} > {file.ID} because there's no file at {download.FullName}.");

		string finalPath = Path.Combine(download.Directory.FullName, file.ID.ToString());

		string setError = null;
		ConsoleHelper.AutoRetry(() =>
		{
			// skip if not an archive
			if (download.Extension == ".exe")
			{
				download.MoveTo(Path.Combine($"{finalPath}.exe"), overwrite: true);
				setError = null;
				return;
			}

			// unzip into temporary folder
			DirectoryInfo finalDir = new DirectoryInfo(finalPath);
			if (finalDir.Exists)
				FileHelper.ForceDelete(finalDir);
			finalDir.Create();
			finalDir.Refresh();

			try
			{
				this.ExtractFile(download, finalDir);
			}
			catch (Exception ex)
			{
				setError = ex is SevenZipArchiveException ? ex.Message : ex.ToString();
				FileHelper.ForceDelete(finalDir);
				return;
			}
		});

		error = setError;

		if (error is null)
			FileHelper.ForceDelete(download);

		return error is null;
	}

	/// <summary>Parse all unpacked mod folders.</summary>
	public IEnumerable<ParsedMod> ReadUnpackedModFolders()
	{
		// get from cache
		string cacheFilePath = this.GetFoldersCacheFilePath();
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
		ModToolkit toolkit = new();
		foreach (DirectoryInfo siteFolder in this.GetSortedSubfolders(new DirectoryInfo(this.CachePath)))
		{
			if (!Enum.TryParse(siteFolder.Name, out ModSite site))
			{
				ConsoleHelper.Print($"   Ignored {siteFolder.FullName}: folder name isn't a valid site key.", Severity.Warning);
				continue;
			}

			Stopwatch timer = Stopwatch.StartNew();
			var modFolders = this.GetSortedSubfolders(siteFolder).ToArray();
			var progress = new IncrementalProgressBar(modFolders.Length).Dump();

			foreach (DirectoryInfo modFolder in modFolders)
			{
				if (!long.TryParse(modFolder.Name, out long modId))
				{
					ConsoleHelper.Print($"   Ignored {modFolder.FullName}: folder name isn't a valid mod ID.", Severity.Warning);
					continue;
				}

				progress.Increment();
				progress.Caption = $"Reading {site} > {modId}...";

				// get mod info
				GenericMod metadata = this.Mods.GetValueOrDefault(site)?.GetValueOrDefault(modId);
				if (metadata is null)
				{
					ConsoleHelper.Print($"   Ignored {modFolder.FullName}: there's no matching mod in the cached data from the modding site.", Severity.Warning);
					continue;
				}

				// load mod folders
				IDictionary<long, GenericFile> fileMap = metadata.Files.ToDictionary(p => p.ID);
				IDictionary<GenericFile, ModFolder[]> unpackedFolders = new Dictionary<GenericFile, ModFolder[]>();
				foreach (DirectoryInfo fileDir in this.GetSortedSubfolders(modFolder))
				{
					progress.Caption = $"Reading {siteFolder.Name} > {modFolder.Name} > {fileDir.Name}...";

					// get file data
					GenericFile fileData = fileMap[int.Parse(fileDir.Name)];

					// get mod folders from toolkit
					ModFolder[] mods = toolkit.GetModFolders(rootPath: modFolder.FullName, modPath: fileDir.FullName, useCaseInsensitiveFilePaths: true).ToArray();
					if (mods.Length == 0)
					{
						ConsoleHelper.Print($"   Ignored {fileDir.FullName}: folder is empty?", Severity.Warning);
						continue;
					}

					// store metadata
					unpackedFolders[fileData] = mods;
				}

				// yield mod
				parsedMods.Add(new ParsedMod(metadata, unpackedFolders));
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

	/// <summary>Save the metadata to the cache file.</summary>
	public void SaveCache()
	{
		// update mods cache
		string json = JsonConvert.SerializeObject(this.Mods, this.JsonSettings);
		File.WriteAllText(this.GetModsCacheFilePath(), json);

		// clear folder cache
		string foldersCachePath = this.GetFoldersCacheFilePath();
		if (File.Exists(foldersCachePath))
			File.Delete(foldersCachePath);
	}


	/*********
	** Private methods
	*********/
	/// <summary>Get the absolute path for the JSON file containing the cached mod data.</summary>
	private string GetModsCacheFilePath()
	{
		return Path.Combine(this.CachePath, "mods.json");
	}

	/// <summary>Get the absolute path for the JSON file containing the cached folder data.</summary>
	private string GetFoldersCacheFilePath()
	{
		return Path.Combine(this.CachePath, "folders.json");
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
	public long ID { get; }

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
	public GenericMod(ModSite site, long id, string name, string author, string authorLabel, string pageUrl, string version, DateTimeOffset updated, object rawData, GenericFile[] files)
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
	public long ID { get; set; }

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
	public GenericFile(long id, GenericFileType type, string displayName, string fileName, string version, object rawData)
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