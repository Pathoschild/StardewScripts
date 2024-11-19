<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.CompatibilityRepo.RawDataModels</Namespace>
  <Namespace>StardewModdingAPI.Toolkit.Framework.Clients.CompatibilityRepo</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

/*

See documentation at https://github.com/Pathoschild/StardewScripts.

*/
/*********
** Configuration
*********/
/// <summary>The absolute path for the compatibility list data file.</summary>
private const string CompatDataPath = @"E:\source\_Stardew\_SmapiCompatibilityList\data\data.jsonc";


/*********
** Script
*********/
void Main()
{
	// load mods
	ModCompatibilityEntry[] mods;
	{
		string json = File.ReadAllText(CompatDataPath);
		using var repoClient = new CompatibilityRepoClient("Pathoschild/StardewScripts 1.0.0");
		var data = JsonConvert.DeserializeObject<RawCompatibilityList>(json);
		mods = data.Mods.Select(repoClient.ParseRawModEntry).ToArray();
	}
	var modsByName = mods.ToLookup(p => p.Name.First(), StringComparer.OrdinalIgnoreCase);

	// check for invalid references
	List<LinkError> errors = new();
	foreach (var mod in mods)
	{
		string summary = mod.Compatibility.Summary;

		foreach (Match match in Regex.Matches(summary, @"\[([^\]]+)\]\(#([^\)]*)\)"))
		{
			string modName = match.Groups[1].Value;
			string customAnchor = match.Groups[2].Value;

			if (!string.IsNullOrEmpty(customAnchor))
			{
				AddLinkError("The URL should be just '#' to auto-generate the anchor based on the link text.");
				continue;
			}

			ModCompatibilityEntry[] targetMods = modsByName[modName].ToArray();
			if (targetMods.Length == 0)
			{
				AddLinkError($"No mod with name '{modName}' found.");
				continue;
			}
			if (!targetMods.Any(p => p.Compatibility.Status is ModCompatibilityStatus.Ok or ModCompatibilityStatus.Optional or ModCompatibilityStatus.Unofficial))
			{
				AddLinkError($"The linked mod '{modName}' has status {string.Join(", ", targetMods.Select(p => p.Compatibility.Status).Distinct())}.");
				continue;
			}
			

			void AddLinkError(string errorPhrase)
			{
				errors.Add(
					new LinkError(mod.Name.FirstOrDefault(), mod.ID.FirstOrDefault(), summary, errorPhrase)
				);
			}
		}
	}

	// dump results
	errors
		.OrderBy(p => p.Error.Substring(0, 10))
		.ThenBy(p => p.ModName)
		.Dump("link errors");
}

/// <summary>An error related to a mod link in a summary.</summary>
/// <param name="ModName">The name of the mod whose summary has an issue.</param>
/// <param name="ModId">The unique ID of the mod whose summary has an issue.</param>
/// <param name="Summary">The summary text which has an issue.</param>
/// <param name="Error">The human-readable error indicating what the issue is.</param>
record LinkError(string ModName, string ModId, string Summary, string Error);
