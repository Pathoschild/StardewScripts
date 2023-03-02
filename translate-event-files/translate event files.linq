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
** Common settings
*********/
/// <summary>The full path to the event script to parse.</summary>
readonly string EventFile = @"C:\Users\patho\Downloads\example_event.json";

/// <summary>The internal name for the NPC, if this is a single-NPC content pack. This allows more readable translation keys like "4hearts" instead of using the event ID.</summary>
readonly string ForNpc = null;

/// <summary>The absolute folder path in which to save the translated event and i18n <c>.json</c> files for easier copying, or <c>null</c> to output directly to the console.</summary>
readonly string OutputFolderPath = null; // for example: @"C:\Users\patho\Downloads\i18n";


/*********
** Override settings (rarely need to edit these)
*********/
/// <summary>For a fork event, whether to prefix its translation keys with the key for the last full event.</summary>
readonly bool AddForksToPrevious = true;

/// <summary>The character encoding for the event file. This should usually be UTF-8, but you can override it for files with unusual encodings.</summary>
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

	// generate scripts/i18n for each event and output to console
	StringBuilder log = new StringBuilder();
	try
	{
		Dictionary<string, string> allI18n = new();
		Dictionary<string, string> allEvents = new();
		List<object> consoleOutput = new();
		int eventsParsed = 0;
		string lastEventPrefix = null;
		foreach (var @event in eventFile)
		{
			log.AppendLine($@"Parsing event {++eventsParsed} of {eventFile.Count} ({@event.Key})...");

			// get event info
			string[] commands = @event.Value.Split('/');
			string translationPrefix = this.GetTranslationPrefix(@event.Key, this.ForNpc);

			// extract unique event key (either event ID or fork key)
			string uniqueKey = @event.Key.Split('/')[0];

			// link forks to previous event
			if (this.AddForksToPrevious)
			{
				if (long.TryParse(uniqueKey, out long eventId))
					lastEventPrefix = translationPrefix;
				else if (lastEventPrefix != null)
					translationPrefix = $"{lastEventPrefix}.{uniqueKey}";
			}

			// build translated script
			var i18n = new TranslationDictionary(translationPrefix);
			string newScript;
			{
				StringBuilder scriptBuilder = new StringBuilder();
				for (int i = 0, last = commands.Length - 1; i <= last; i++)
				{
					// translate command
					string command = commands[i];
					if (this.TryTranslateCommand(command, i18n, out string newCommand))
						command = newCommand;

					// append to new script
					scriptBuilder.Append(command);
					if (i != last)
						scriptBuilder.Append('/');
				}
				newScript = scriptBuilder.ToString();
			}

			// add to full output files
			foreach (var entry in i18n.Values)
				allI18n[entry.Key] = entry.Value;
			allEvents[@event.Key] = newScript;

			// get report entry
			consoleOutput.Add(new
			{
				key = @event.Key,
				hearts = this.GetFriendshipRequirement(@event.Key, this.ForNpc) / 250m,
				i18n = this.ToJson(i18n.Values),
				newScript = this.ToJson(new Dictionary<string, string> { [@event.Key] = newScript }),
				oldScript = new Lazy<string>(() => @event.Value)
			});
		}

		// output
		if (this.OutputFolderPath != null)
		{
			Directory.CreateDirectory(this.OutputFolderPath);
			File.WriteAllText(Path.Combine(this.OutputFolderPath, "events.json"), this.ToJson(allEvents));
			File.WriteAllText(Path.Combine(this.OutputFolderPath, "i18n.json"), this.ToJson(allI18n));

			new
			{
				SavedToFolder = new Hyperlinq(this.OutputFolderPath),
				Log = new Lazy<string>(() => log.ToString()),
				RawEvents = new Lazy<object>(() => consoleOutput)
			}.Dump("translated events");
		}
		else
		{
			new
			{
				AllEvents = new Lazy<string>(() => this.ToJson(allEvents)),
				AllI18n = new Lazy<string>(() => this.ToJson(allI18n)),
				RawEvents = new Lazy<object>(() => consoleOutput),
				Log = new Lazy<string>(() => log.ToString())
			}.Dump("translated events");
		}
	}
	catch (Exception ex)
	{
		new
		{
			Log = log.ToString(),
			Exception = ex
		}.Dump("An unhandled exception occurred.");
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
	newCommand = commandName switch
	{
		"end" => this.TryTranslateEnd(command, translations),
		"message" => this.TryTranslateMessage(command, translations),
		"question" => this.TryTranslateQuestion(command, translations),
		"quickquestion" => this.TryTranslateQuickQuestion(command, translations),
		"speak" => this.TryTranslateSpeak(command, translations),
		"textabovehead" => this.TryTranslateTextAboveHead(command, translations),
		_ => command
	};
	
	return translations.Values.Count > initialCount;
}

/// <summary>Translate an <c>end</c> event command if needed.</summary>
/// <param name="command">The event command to translate.</param>
/// <param name="translations">The translations for this event.</param>
private string TryTranslateEnd(string command, TranslationDictionary translations)
{
	return Regex.Replace(
		command,
		@"^(end dialogue(?:WarpOut)? [a-z0-9]+) ""(.+)""",
		match => $@"{match.Groups[1].Value} ""{translations.Add(match.Groups[2].Value)}""",
		RegexOptions.IgnoreCase
	);
}

/// <summary>Translate a <c>message</c> event command if needed.</summary>
/// <param name="command">The event command to translate.</param>
/// <param name="translations">The translations for this event.</param>
private string TryTranslateMessage(string command, TranslationDictionary translations)
{
	return Regex.Replace(
		command,
		@"^message ""(.+)""$",
		match => $@"message ""{translations.Add(match.Groups[1].Value)}"""
	);
}

/// <summary>Translate a <c>question</c> event command if needed.</summary>
/// <param name="command">The event command to translate.</param>
/// <param name="translations">The translations for this event.</param>
private string TryTranslateQuestion(string command, TranslationDictionary translations)
{
	return Regex.Replace(
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
}

/// <summary>Translate a <c>quickQuestion</c> event command if needed.</summary>
/// <param name="command">The event command to translate.</param>
/// <param name="translations">The translations for this event.</param>
private string TryTranslateQuickQuestion(string command, TranslationDictionary translations)
{
	// translate a subscript embedded within a quickQuestion command
	string TranslateSubscript(string script)
	{
		IEnumerable<string> subcommands = script
			.Split('\\')
			.Select(subcommand =>
			{
				this.TryTranslateCommand(subcommand, translations, out string newSubcommand);
				return newSubcommand;
			});
		return string.Join('\\', subcommands);
	}

	// translate command
	return Regex.Replace(
		command,
		// quickQuestion #It's probably because you talk too much.#That might be, but I'm sure you can all be friends.#That's silly, I'm sure he likes you well enough.#You're just kids. You'll get over it.(break)speak Eloise \"Oh... He does say I chatter like a parrot...$3\"(break)speak Eloise \"Of course we're all friends. You don't understand...$2\"(break)emote Eloise 12\\
		@"^quickQuestion (?:#(?<questions>[^#]+?))+?\(break\)(?<commands>.+?)(?:\(break\)(?<scripts>.+?))+$",
		match =>
		{
			StringBuilder result = new StringBuilder("quickQuestion ");

			// questions
			foreach (string question in match.Groups["questions"].Captures.Select(p => p.Value))
			{
				result.Append("#");
				result.Append(translations.Add(question));
			}

			// commands
			result.Append("(break)");
			result.Append(TranslateSubscript(match.Groups["commands"].Captures.Single().Value));

			// answer scripts
			foreach (string answerScript in match.Groups["scripts"].Captures.Select(p => p.Value))
			{
				result.Append("(break)");
				result.Append(TranslateSubscript(answerScript));
			}

			new { before = match.Value, after = result.ToString() }.Dump();
			return result.ToString();
		}
	);
}

/// <summary>Translate a <c>speak</c> event command if needed.</summary>
/// <param name="command">The event command to translate.</param>
/// <param name="translations">The translations for this event.</param>
private string TryTranslateSpeak(string command, TranslationDictionary translations)
{
	// question fork
	if (command.Contains("$q"))
	{
		return Regex.Replace(
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
	return Regex.Replace(
		command,
		@"^(speak [a-z0-9]+) ""(.+)""$",
		match => $@"{match.Groups[1].Value} ""{translations.Add(match.Groups[2].Value)}""",
		RegexOptions.IgnoreCase
	);
}

/// <summary>Translate a <c>textAboveHead</c> event command if needed.</summary>
/// <param name="command">The event command to translate.</param>
/// <param name="translations">The translations for this event.</param>
private string TryTranslateTextAboveHead(string command, TranslationDictionary translations)
{
	return Regex.Replace(
		command,
		@"^(textAboveHead [a-z0-9]+) ""(.+)""",
		match => $@"{match.Groups[1].Value} ""{translations.Add(match.Groups[2].Value)}""",
		RegexOptions.IgnoreCase
	);
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
		foreach (var condition in preconditions.Split('/'))
		{
			// parse friendship condition format (`f NameA 250 NameB 250 ...`)
			if (!condition.StartsWith("f "))
				continue;
			
			string[] parts = condition.Split(' ').Skip(1).ToArray();
			for (int i = 0; i + 1 < parts.Length; i += 2)
			{
				if (parts[i] == forNpc && int.TryParse(parts[i + 1], out int points))
					return points;
			}
		}
	}

	return null;
}

/// <summary>Get a formatted JSON representation of the given value.</summary>
/// <param name="value">The raw value to serialize.</param>
private string ToJson(object value)
{
	return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented);
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
