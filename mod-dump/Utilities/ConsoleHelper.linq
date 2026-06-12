<Query Kind="Program">
</Query>

/// <summary>Represents a log severity.</summary>
public enum Severity { Trace, Info, Warning, Error }

/// <summary>Provides common methods for writing LINQPad scripts.</summary>
public static class ConsoleHelper
{
	/****
	** Constants
	****/
	public const string TraceStyle = "opacity: 0.5";
	public const string ErrorStyle = "color: red; font-weight: bold;";
	public const string SuccessStyle = "color: green;";


	/*********
	** Public methods
	*********/
	/// <summary>Print a formatted message to the console.</summary>
	/// <param name="message">The message to print.</param>
	/// <param name="severity">The message severity.</param>
	public static void Print(string message, Severity severity = Severity.Info)
	{
		object formatted = severity switch
		{
			Severity.Trace => Util.WithStyle(message, "color: gray"),
			Severity.Warning => Util.WithStyle(message, "color: orange"),
			Severity.Error => Util.WithStyle(message, "color: red"),
			_ => message
		};
		Console.WriteLine(formatted);
	}
}