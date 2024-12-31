<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Steam\steamapps\common\Stardew Valley\smapi-internal\SMAPI.Toolkit.CoreInterfaces.dll</Reference>
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
private const string CompatDataPath = @"E:\source\_Stardew\_SmapiCompatibilityList\data\mods.jsonc";


/*********
** Script
*********/
void Main()
{
	// load data
	string json = File.ReadAllText(CompatDataPath);
	var data = JsonConvert.DeserializeObject<RawCompatibilityList>(json);

	// collect stats
	int ok = 0;
	int workaround = 0;
	int soon = 0;
	int broken = 0;
	int total = 0;
	foreach (RawModEntry mod in data.Mods)
	{
		var status = mod.GetStatus();

		switch (status)
		{
			case ModCompatibilityStatus.Ok:
			case ModCompatibilityStatus.Optional:
				ok++;
				total++;
				break;

			case ModCompatibilityStatus.Unofficial:
			case ModCompatibilityStatus.Workaround:
				workaround++;
				total++;
				break;

			case ModCompatibilityStatus.Broken:
				if (mod.GitHub != null || mod.Source != null)
					soon++;
				else
					broken++;
				total++;
				break;

			case ModCompatibilityStatus.Abandoned:
			case ModCompatibilityStatus.Obsolete:
				break; // abandoned/obsolete mods don't count towards stats

			default:
				mod.Dump($"ignored mod with invalid status '{status}'.");
				break;
		}
	}

	// dump status
	new
	{
		Overall = $"{(ok + workaround) / (total * 1f):P1} compatible",

		Ok = ok,
		Workaround = workaround,
		Soon = soon,
		Broken = broken,
		Total = total
	}.Dump("stats");
}
