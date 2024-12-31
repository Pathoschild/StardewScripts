<Query Kind="Program" />

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
	// NOTE: we deliberate work with the JSON directly to avoid discarding comments & formatting
	string fullData = File.ReadAllText(CompatDataPath);

	// isolate mods list
	int startIndex;
	int endIndex;
	string data;
	{
		var startMatch = Regex.Match(fullData, $"^\t\"mods\": *\\[{Environment.NewLine}", RegexOptions.Multiline);
		var endMatch = Regex.Match(fullData, "^\t\\]", RegexOptions.Multiline);

		if (!startMatch.Success)
			throw new InvalidOperationException($"Can't find start of 'mods' field.");
		if (!endMatch.Success)
			throw new InvalidOperationException($"Can't find end of 'mods' field.");
		if (endMatch.Index < startMatch.Index)
			throw new InvalidOperationException($"Found end of 'mods' field before it's start (probably due to a format change).");

		startIndex = startMatch.Index + startMatch.Length;
		endIndex = endMatch.Index;
		data = fullData[startIndex..endIndex];
	}

	// extract entry blocks
	string[] entries;
	{
		string newline = Environment.NewLine;
		string startOfBlock = "\t\t{";
		string endOfBlock = "\t\t},";

		string wrappedData = (newline + endOfBlock + newline) + data.TrimEnd() + "," + (newline + startOfBlock + newline); // add empty 'entries' to start and end to get a consistent split
		entries = wrappedData.Split($"{newline}{endOfBlock}{newline}{startOfBlock}{newline}", StringSplitOptions.RemoveEmptyEntries);
	}

	// extract names
	var parsedEntries =
		(
			from entry in entries
			let comparableName = ToComparable(Regex.Match(entry, @"^\t\t\t""name"":\s*""(.+)""", RegexOptions.Multiline)?.Groups[1].Value ?? throw new InvalidOperationException($"Can't extract 'name' field from entry.\nRaw text: {entry}"))
			let nexusId = Regex.Match(entry, @"^\t\t\t""nexus"":\s*(\d+)", RegexOptions.Multiline)?.Groups[1].Value ?? throw new InvalidOperationException($"Can't extract 'nexus' field from entry.\nRaw text: {entry}")
			let comparableAuthor = ToComparable(Regex.Match(entry, @"^\t\t\t""author"":\s*""(.+)""", RegexOptions.Multiline)?.Groups[1].Value ?? throw new InvalidOperationException($"Can't extract 'author' field from entry.\nRaw text: {entry}"))
			select new { comparableName, comparableAuthor, nexusId, entry }
		).ToList();

	// sort
	parsedEntries.Sort((a, b) =>
	{
		string nameA = a.comparableName;
		string nameB = b.comparableName;

		// same name, sort by age and then author
		if (nameA == nameB)
		{
			if (int.TryParse(a.nexusId, out int nexusA) && int.TryParse(b.nexusId, out int nexusB))
				return nexusA.CompareTo(nexusB);

			return string.Compare(a.comparableAuthor, b.comparableAuthor);
		}

		// special case: if they have a common prefix, list shorter one first
		if (nameA.StartsWith(nameB))
			return 1;
		if (nameB.StartsWith(nameA))
			return -1;

		// else sort alphabetically
		return string.Compare(nameA, nameB);
	});

	// merge back into data
	string modifiedFullData =
		$$"""
		{{fullData.Substring(0, startIndex).TrimEnd()}}
		{{"\t\t{"}}
		{{string.Join("\n\t\t},\n\t\t{\n", parsedEntries.Select(p => p.entry))}}
		{{"\t\t}"}}
		{{fullData.Substring(endIndex)}}
		""";

	// save to file
	File.WriteAllText(CompatDataPath, modifiedFullData, new UTF8Encoding(false));
}

/// <summary>Get a comparable representation of a string value which ignores case, spaces, punctuation, and symbols.</summary>
/// <param name="value">The string value to represent.</param>
string ToComparable(string value)
{
	return new string(value.ToLower().Where(ch => ch is not (' ') && !Char.IsPunctuation(ch) && !Char.IsSymbol(ch)).ToArray());
}
