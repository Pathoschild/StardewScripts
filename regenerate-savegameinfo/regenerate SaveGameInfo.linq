<Query Kind="Program">
  <Namespace>System.Xml.Serialization</Namespace>
</Query>

/*

This script generates a SaveGameInfo for a save file which doesn't have one.

Usage
-------------------------------------------------
To use this script:

1. Open this file in LINQPad. (It's a small app: https://www.linqpad.net/.)
2. Change the 'SaveFilePath' value below to the full path to the save file.
3. Click the execute (▶) button to generate the SaveGameInfo in the same folder.

*/

string SaveFilePath = @"%appdata%\StardewValley\Saves\Example_117455303\Example_117455303";

void Main()
{
    // open save file
    SaveFilePath = Environment.ExpandEnvironmentVariables(SaveFilePath);
    FileInfo saveFile = new FileInfo(SaveFilePath);

    // extract player XML
    var player = XElement.Load(saveFile.FullName).XPathSelectElement("//player");
    player.Name = "Farmer";
    player.Add(
        new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
        new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance")
    );
    
    // build document
    XDocument document = new(
        new XDeclaration("1.0", "utf-8", null),
        player
    );

    // write to SaveFileInfo
    FileInfo infoFile = new FileInfo(Path.Combine(saveFile.Directory.FullName, "SaveGameInfo"));
    document.Save(infoFile.FullName);

    // output details
    Console.WriteLine("Done!");
    GetSaveDetails(player, infoFile).Dump("save details");
}

/// <summary>Get human-readable details about the save file.</summary>
/// <param name="player">The player data saved to the <c>SaveGameInfo</c> file.</param>
/// <param name="generatedFile">The generated <c>SaveGameInfo</c> file.</param>
public IDictionary<string, object> GetSaveDetails(XElement player, FileInfo generatedFile)
{
    // extract save date
    // derived from SaveGame.getSaveEnumerator
    string lastSaved;
    {
        string rawSaveTime = player.XPathSelectElement("//saveTime")?.Value;
        if (!int.TryParse(rawSaveTime, out int saveTime))
            lastSaved = $"Can't parse '{rawSaveTime ?? "<null>"}' as an integer save time.";
        else
        {
            var saveDate = new DateTimeOffset(2012, 6, 22, 0, 0, 0, TimeSpan.Zero).AddMinutes(saveTime);
            lastSaved = saveDate.ToLocalTime().ToString("f");
        }
    }
    
    // extract in-game date
    // note: the 'dateStringForSaveGame' field isn't reliable
    string date;
    {
        string day = player.XPathSelectElement("//dayOfMonthForSaveGame")?.Value;
        string season = player.XPathSelectElement("//seasonForSaveGame")?.Value;
        string year = player.XPathSelectElement("//yearForSaveGame")?.Value;

        season = season switch // derived from Utility.getSeasonNumber
        {
            "0" => "spring",
            "1" => "summer",
            "2" => "fall",
            "3" => "winter",
            _ => season
        };

        date = $"{season} {day}, year {year}";
    }
    
    // base save info
    return new Dictionary<string, object>
    {
        ["player name"] = player.XPathSelectElement("//name")?.Value,
        ["farm name"] = player.XPathSelectElement("//farmName")?.Value,
        ["in-game date"] = date,
        ["last saved"] = lastSaved,
        ["raw file"] = new Lazy<string>(() => File.ReadAllText(generatedFile.FullName)),
        ["open folder"] = new Hyperlinq(generatedFile.Directory.FullName, "open folder")
    };
}