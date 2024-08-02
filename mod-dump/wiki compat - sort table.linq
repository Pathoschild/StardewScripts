<Query Kind="Program" />

/*

See documentation at https://github.com/Pathoschild/StardewScripts.

*/
void Main()
{
	// normalize
	string modified = Regex.Replace(data, @"{{\s*#invoke\s*:\s*SMAPI compatibility\s*\|\s*entry", "{{#invoke:SMAPI compatibility|entry");

	// extract templates
	string[] templates;
	{
		List<string> extractedTemplates = new();
		modified = Regex.Replace(modified, @"{{#invoke:SMAPI compatibility\|entry[\s\S]+?\n}}", match =>
		{
			if (match.Value.IndexOf("{{#invoke", 1) != -1)
				throw new InvalidOperationException($"Mismatched templates. Found #invoke inside #invoke call.\nMatched text: {match.Value}.");

			extractedTemplates.Add(match.Value);

			return string.Empty;
		});

		var parsedTemplates =
			(
				from template in extractedTemplates
				let comparableName = ToComparable(Regex.Match(template, @"\|\s*name\s*=\s*(.+)")?.Groups[1].Value ?? throw new InvalidOperationException($"Can't extract name field from template.\nRaw text: {template}"))
				let nexusId = Regex.Match(template, @"\|\s*nexus\s*=\s*(.*)")?.Groups[1].Value ?? throw new InvalidOperationException($"Can't extract nexus field from template.\nRaw text: {template}")
				let comparableAuthor = ToComparable(Regex.Match(template, @"\|\s*author\s*=\s*(.+)")?.Groups[1].Value ?? throw new InvalidOperationException($"Can't extract author field from template.\nRaw text: {template}"))
				select new { comparableName, comparableAuthor, nexusId, template }
			).ToList();

		parsedTemplates.Sort((a, b) =>
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
				return -1;
			if (nameB.StartsWith(nameA))
				return 1;

			// else sort alphabetically
			return string.Compare(nameA, nameB);
		});

		templates = parsedTemplates.Select(p => p.template).ToArray();
	}

	// merge back into table
	modified = Regex.Replace(modified, @"({{#invoke:SMAPI compatibility\|header}})([\s\S]+)({{#invoke:SMAPI compatibility\|footer}})", match =>
	{
		string header = match.Groups[1].Value;
		string remainingContent = match.Groups[2].Value;
		string footer = match.Groups[3].Value;

		if (!string.IsNullOrWhiteSpace(remainingContent))
			throw new InvalidOperationException($"Some templates could not be extracted.\nRemaining text: {match.Value.Trim()}.");

		return
			header
			+ Environment.NewLine
			+ string.Join(Environment.NewLine, templates)
			+ Environment.NewLine
			+ footer;
	});

	// sort
	modified.Dump();
}

/// <summary>Get a comparable representation of a string value which ignores case, spaces, punctuation, and symbols.</summary>
/// <param name="value">The string value to represent.</param>
string ToComparable(string value)
{
	return new string(value.ToLower().Where(ch => ch is not (' ') && !Char.IsPunctuation(ch) && !Char.IsSymbol(ch)).ToArray());
}

#region data
const string data =
"""
===C# mods===
This includes every known C# SMAPI mod. It's updated for new/updated mods on CurseForge/ModDrop/Nexus periodically with the help of {{github|Pathoschild/StardewScripts/tree/main/mod-dump|semi-automated scripts}}, but feel free to make corrections as needed!

<!--

Tokens used in the markup:
 - "@retest-after Mod Name" means the mod is broken due to the named mod being broken, so it should be retested when that mod is updated.
 - "@retest-linked" means one or more mods should be retested when this mod is updated.

-->
{{#invoke:SMAPI compatibility|header}}
...
{{#invoke:SMAPI compatibility|footer}}
<div class="game-stable-version" style="display:none;">{{version|link=0}}</div><div class="game-beta-version" style="display:none;">{{version|link=0}}</div>
""";
#endregion