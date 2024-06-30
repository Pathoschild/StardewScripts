<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

/// <summary>Represents a log severity.</summary>
public enum Severity { Trace, Info, Warning, Error }

/// <summary>Provides common methods for writing LINQPad scripts.</summary>
public static class ConsoleHelper
{
	/*********
	** Public methods
	*********/
	/// <summary>Get a human-readable formatted time span.</summary>
	/// <param name="span">The time span to format.</param>
	public static string GetFormattedTime(TimeSpan time)
	{
		if (time.TotalSeconds < 1)
			return $"{Math.Round(time.TotalMilliseconds, 0)} ms";

		StringBuilder formatted = new();
		void Format(int amount, string label)
		{
			if (amount > 0)
			{
				formatted.Append(" ");
				formatted.Append(amount);
				formatted.Append(" ");
				formatted.Append(label);
				if (amount != 1)
					formatted.Append('s');
			}
		}
		Format((int)time.TotalHours, "hour");
		Format(time.Minutes, "minute");
		Format(time.Seconds, "second");

		return formatted.ToString().TrimStart();
	}

	/// <summary>Get input from the user until the specify a valid option.</summary>
	/// <param name="options">The valid inputs (case insensitive).</summary>
	public static string GetChoice(string prompt, params string[] options)
	{
		while (true)
		{
			string input = Util.ReadLine(prompt, "", options).Trim();
			if (options.Contains(input))
				return input;
		}
	}

	/// <summary>Get input from the user until the specify a valid option.</summary>
	/// <param name="options">The valid inputs (case insensitive).</summary>
	/// <param name="cancelToken">A cancellation token which can end the input.</param>
	public static async Task<string> GetChoiceAsync(string prompt, string[] options, CancellationToken cancelToken)
	{
		while (true)
		{
			string input = (await Util.ReadLineAsync(prompt, "", options, cancelToken)).Trim();
			if (options.Contains(input))
				return input;
		}
	}

	/// <summary>Print a formatted message to the console.</summary>
	/// <param name="message">The message to print.</param>
	/// <param name="severity">The message severity.</param>
	public static void Print(string message, Severity severity = Severity.Info)
	{
		object formatted = ConsoleHelper.FormatMessage(message, severity);
		Console.WriteLine(formatted);
	}

	/// <summary>Format a message before it's printed to the console.</summary>
	/// <param name="message">The message to print.</param>
	/// <param name="severity">The message severity.</param>
	public static object FormatMessage(string message, Severity severity)
	{
		return severity switch
		{
			Severity.Trace => Util.WithStyle(message, "color: gray"),
			Severity.Warning => Util.WithStyle(message, "color: orange"),
			Severity.Error => Util.WithStyle(message, "color: red"),
			_ => message
		};
	}

	/// <summary>Try to run a block of code, and let the user retry if it fails.</summary>
	/// <param name="action">The action to try perfoming.</param>
	/// <param name="formatDump">Create a custom object to dump to the console instead of the exception.</param>
	public static bool AutoRetry(Action action, Func<Exception, object> formatDump = null)
	{
		return ConsoleHelper.AutoRetry<bool>(
			() => { action(); return true; },
			out bool _,
			formatDump
		);
	}

	/// <summary>Try to run a block of code, and let the user retry if it fails.</summary>
	/// <param name="action">The action to try perfoming.</param>
	/// <param name="result">The action return value.</param>
	/// <param name="formatDump">Create a custom object to dump to the console instead of the exception.</param>
	public static bool AutoRetry<TResult>(Func<TResult> action, out TResult result, Func<Exception, object> formatDump = null)
	{
		while (true)
		{
			try
			{
				result = action();
				return true;
			}
			catch (Exception ex)
			{
				(formatDump != null ? formatDump(ex) : ex).Dump("error occurred");
				string choice = ConsoleHelper.GetChoice("Do you want to [r]etry, [s]kip, or [a]bort?", "r", "s", "a");
				if (choice == "r")
					continue; // retry
				else if (choice == "s")
				{
					result = default;
					return false; // skip
				}
				else if (choice == "a")
					throw; // abort
				else
					throw new NotSupportedException($"Invalid choice: '{choice}'", ex);
			}
		}
	}
}