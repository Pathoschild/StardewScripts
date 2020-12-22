<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

/*

Overview
-------------------------------------------------
This script pulls text strings out of Stardew Valley events and generates translatable versions
using Content Patcher's {{i18n}} token. See https://stardewvalleywiki.com/Modding:Index for more
info.


Usage
-------------------------------------------------
To use this script:

1. Open this file in LINQPad. (It's a small app: https://www.linqpad.net/.)
2. Change the settings under 'configuration' below.
3. Click the execute (▶) button to generate the translated event.


Event file format
-------------------------------------------------
The script automatically recognizes the three common formats:

1. Raw event files loaded using "Action": "Load":
    {
       "event A": "script",
       "event B": "script",
       ...
    }

2. A file loaded via "Action": "Include":
    {
        "Changes": [
            {
	        "Action": "EditData",
	        "Target": "Data/Events/Town",
		"Entries": {
		    "event A": "script",
		    "event B": "script",
		    ...
		}
	    },
	    ...
        ]
    }

3. A Legacy file loaded using "Action": "EditData" with FromFile:
    {
       "Entries": {
          "event A": "script",
          "event B": "script",
          ...
       }
    }

If you have events in a different format, you can copy & paste them into the raw event file format
to use this script.

*/
/*********
** Configuration
*********/
/***
** Common settings
***/
/// <summary>The full path to the event script to parse.</summary>
readonly string EventFile = @"C:\Users\patho\Downloads\example_event.json";

/// <summary>The internal name for the NPC, if this is a single-NPC content pack. This allows more readable translation keys like "4hearts" instead of using the event ID.</summary>
readonly string ForNpc = null;

/***
** Overrides (rarely need to change these)
***/
/// <summary>For a fork event, whether to prefix its translation keys with the key for the last full event.</summary>
readonly bool AddForksToPrevious = true;

/// <summary>The character encoding for the file. This should usually be UTF-8, but you can override it for files with unusual encodings.</summary>
readonly Encoding FileEncoding = Encoding.UTF8;

/// <summary>Get the translation key prefix for an event.</summary>
/// <param name="hearts">The NPC hearts needed to see the event. This only applies if <see cref="ForNpc"/> has a value, the event has a friendship requirement with that NPC, and a round number of hearts is needed (e.g. 2 hearts, not 2.5 hearts).</param>
/// <param name="points">The NPC friendship points needed to see the event. This only applies if <see cref="ForNpc"/> has a value, and the event has a friendship requirement with that NPC.</param>
/// <param name="eventId">The unique event ID.</param>
/// <param name="preconditions">The full event precondition key, including the ID.</param>
string GetTranslationPrefix(int? hearts, int? points, string eventId, string preconditions)
{
	if (hearts.HasValue)
		return hearts == 1 ? "1heart" : $"{hearts}hearts";
	return $"event-{eventId}";
}


/*********
** Script
*********/
public void Main()
{
	Console.WriteLine("Reading file...");
	var eventFile = this.ParseRawFile(this.EventFile, this.FileEncoding);

	int eventsParsed = 0;
	string lastEventPrefix = null;
	foreach (var @event in eventFile)
	{
		Console.WriteLine($@"Parsing event {++eventsParsed} of {eventFile.Count} ({@event.Key})...");
		
		// get event info
		string[] commands = @event.Value.Split('/');
		string translationPrefix = this.GetTranslationPrefix(@event.Key, this.ForNpc);

		// link forks to previous event
		if (this.AddForksToPrevious)
		{
			string rawId = @event.Key.Split('/')[0];
			if (long.TryParse(rawId, out long eventId))
				lastEventPrefix = translationPrefix;
			else if (lastEventPrefix != null)
				translationPrefix = $"{lastEventPrefix}.{rawId}";
		}

		// build translated script
		var i18n = new TranslationDictionary(translationPrefix);
		StringBuilder newScript = new StringBuilder();
		for (int i = 0, last = commands.Length - 1; i <= last; i++)
		{
			// translate command
			string command = commands[i];
			if (this.TryTranslateCommand(command, i18n, out string newCommand))
				command = newCommand;

			// append to new script
			newScript.Append(command);
			if (i != last)
				newScript.Append('/');
		}

		new
		{
			key = @event.Key,
			hearts = this.GetFriendshipRequirement(@event.Key, this.ForNpc) / 250m,
			i18n = string.Join("\n", i18n.Values.Select(p => $@"    ""{p.Key}"": ""{p.Value}"",")),
			newScript = JsonConvert.SerializeObject(new Dictionary<string, string> { [@event.Key] = newScript.ToString() }, Newtonsoft.Json.Formatting.Indented),
			oldScript = @event.Value
		}.Dump();
	}
}

/// <summary>Translate an event command if needed.</summary>
/// <param name="command">The event command to translate.</param>
/// <param name="translations">The translations for this event.</param>
/// <param name="newCommand">The command with translation tokens injected.</param>
/// <returns>Returns whether the command was translated.</returns>
private bool TryTranslateCommand(string command, TranslationDictionary translations, out string newCommand)
{
	int initialCount = translations.Values.Count;
	
	string commandName = command.Split(' ').FirstOrDefault()?.ToLower();
	switch (commandName)
	{
		case "end":
			newCommand = Regex.Replace(
				command,
				@"^(end dialogue(?:WarpOut)? [a-z0-9]+) ""(.+)""",
				match => $@"{match.Groups[1].Value} ""{translations.Add(match.Groups[2].Value)}""",
				RegexOptions.IgnoreCase
			);
		
			break;
	
		case "message":
			newCommand = Regex.Replace(
				command,
				@"^message ""(.+)""$",
				match => $@"message ""{translations.Add(match.Groups[1].Value)}"""
			);
			break;

		case "question":
			newCommand = Regex.Replace(
				command,
				@"^(question [a-z0-9]+) ""([^#]+)(?:#([^#]+))+(#?)""",
				match =>
				{
					StringBuilder newStr = new StringBuilder();

					newStr.Append($@"{match.Groups[1].Value} ""{translations.Add(match.Groups[2].Value)}");

					foreach (Capture capture in match.Groups[3].Captures)
					{
						newStr.Append('#');
						newStr.Append(translations.Add(capture.Value));
					}

					if (match.Groups[4].Success)
						newStr.Append('#');
					newStr.Append('"');
					return newStr.ToString();
				}
			);
			break;

		case "speak":
			// question fork
			if (command.Contains("$q"))
			{
				newCommand = Regex.Replace(
					command,
					@"^(speak [a-z0-9]+ ""\$q [^#]+)#([^#]+)(?:#(\$r [^#]+#[^#]+))+""$",
					match =>
					{
						StringBuilder newStr = new StringBuilder();
						newStr.Append($@"{match.Groups[1].Value} """);

						// question
						newStr.Append('#');
						newStr.Append(translations.Add(match.Groups[2].Value));

						// answers
						foreach (Capture capture in match.Groups[3].Captures)
						{
							string[] parts = capture.Value.Split('#');
							
							newStr.Append('#');
							newStr.Append(parts[0]);
							newStr.Append('#');
							newStr.Append(translations.Add(parts[1]));
						}

						newStr.Append(@"""");
						return newStr.ToString();
					},
					RegexOptions.IgnoreCase
				);
			}

			// normal message
			else
			{
				newCommand = Regex.Replace(
					command,
					@"^(speak [a-z0-9]+) ""(.+)""$",
					match => $@"{match.Groups[1].Value} ""{translations.Add(match.Groups[2].Value)}""",
					RegexOptions.IgnoreCase
				);
			}
			break;
		
		case "textabovehead":
			newCommand = Regex.Replace(
				command,
				@"^(textAboveHead [a-z0-9]+) ""(.+)""",
				match => $@"{match.Groups[1].Value} ""{translations.Add(match.Groups[2].Value)}""",
				RegexOptions.IgnoreCase
			);
			break;

		default:
			newCommand = command;
			break;
	}
	
	return translations.Values.Count > initialCount;
}

/// <summary>Extract the event entries from a raw file.</summary>
/// <param name="path">The full path to the file to parse.</param>
/// <param name="fileEncoding">The character encoding for the file.</param>
private IDictionary<string, string> ParseRawFile(string path, Encoding fileEncoding)
{
	JObject json = JObject.Parse(File.ReadAllText(path, fileEncoding));
	
	// legacy EditData with FromFile format
	if (json.Count == 1 && json.Property("Entries") != null)
	{
		JProperty entries = json.Property("Entries");
		return entries.Value.ToObject<Dictionary<string, string>>();
	}
	
	// Include patch format
	if (json.Count == 1 && json.Property("Changes")?.Value is JArray patches)
	{
		IDictionary<string, string> events = new Dictionary<string, string>();
		foreach (JObject patch in patches.Values<JObject>())
		{
			var entries = patch.Property("Entries")?.Value.ToObject<Dictionary<string, string>>();
			if (entries != null)
			{
				foreach (var entry in entries)
					events[entry.Key] = entry.Value;
			}
		}
		return events;
	}

	// Load format
	try
	{
		return json.ToObject<Dictionary<string, string>>();
	}
	catch (JsonReaderException ex)
	{
		throw new InvalidOperationException("Can't parse this file as an event dictionary. The file must contain either a flat JSON object (i.e. Load format), or an object with only an Entries property containing a flat JSON object (i.e. EditData with FromFile format).", ex);
	}
}

/// <summary>Get a unique key for an arbitrary event.</summary>
/// <param name="preconditions">The event preconditions.</param>
/// <param name="forNpc">The internal name for the NPC, if this is a single-NPC content pack. This allows more readable event keys like "4hearts" instead of using the event ID.</param>
private string GetTranslationPrefix(string preconditions, string forNpc = null)
{
	// extract info
	string[] parts = preconditions.Split('/');
	string eventId = parts[0];
	int? hearts = null;
	int? points = null;
	if (forNpc != null)
	{
		points = this.GetFriendshipRequirement(preconditions, forNpc);
		if (points % 250 == 0)
			hearts = points / 250;
	}
	
	// get key
	return this.GetTranslationPrefix(
		hearts: hearts,
		points: points,
		eventId: eventId,
		preconditions: preconditions
	);
}

/// <summary>Get an event's friendship requirement with an NPC, if applicable.</summary>
/// <param name="preconditions">The event preconditions key to parse.</param>
/// <param name="forNpc">The NPC whose friendship to check.</param>
private int? GetFriendshipRequirement(string preconditions, string forNpc)
{
	if (forNpc != null)
	{
		string friendshipPrefix = $"f {forNpc} ";
		foreach (var part in preconditions.Split('/'))
		{
			if (part.StartsWith(friendshipPrefix) && int.TryParse(part.Substring(friendshipPrefix.Length), out int points))
				return points;
		}
	}

	return null;
}

/// <summary>Manages translations for an event.</summary>
private class TranslationDictionary
{
	/*********
	** Accessors
	*********/
	/// <summary>A key which uniquely identifies the event in the global translations.</summary>
	public string TranslationPrefix { get; }

	/// <summary>The underlying translations.</summary>
	public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="translationPrefix">A key which uniquely identifies the event in the global translations.</param>
	public TranslationDictionary(string translationPrefix)
	{
		this.TranslationPrefix = translationPrefix;
	}

	/// <summary>Add a translation and get a placeholder token for it.</summary>
	/// <param name="value">The translation text to add.</param>
	public string Add(string text)
	{
		string key = $"{this.TranslationPrefix}.{(this.Values.Count + 1).ToString().PadLeft(2, '0')}";
		this.Values.Add(key, text);
		return "{{i18n:" + key + "}}";
	}
}