<Query Kind="Program" />

/// <summary>An implementation of <see cref="Util.ProgressBar" /> which is simpler to use when incrementing towards a known value.</summary>
public class IncrementalProgressBar : Util.ProgressBar
{
	/*********
	** Accessors
	*********/
	/// <summary>The total value.</summary>
	public int Total { get; }
	
	/// <summary>The current value relative to the total.</summary>
	public int Current { get; private set; }

	/// <summary>The template for the progress bar (if any). Valid tokens: {{current}}, {{total}}, {{percent}}.</summary>
	public string CaptionTemplate { get; }


	/*********
	** Public methods
	*********/
	/// <summary>Construct an instance.</summary>
	/// <param name="total">The total value.</param>
	/// <param name="captionTemplate">The template for the progress bar (if any). Valid tokens: {{current}}, {{total}}, {{percent}}.</param>
	public IncrementalProgressBar(int total, string captionTemplate = null)
	{
		this.Total = total;
		this.Fraction = 0;
		this.CaptionTemplate = captionTemplate;
	}
	
	/// <summary>Set the current value.</summary>
	/// <param name="current">The current value.</param>
	public void Set(int current)
	{
		this.Current = current;
		this.Fraction = this.Current / (this.Total * 1d);
		this.Caption = this.CaptionTemplate != null
			? this.CaptionTemplate.Replace("{{current}}", this.Current.ToString()).Replace("{{total}}", this.Total.ToString()).Replace("{{percent}}", Math.Round(this.Fraction * 100).ToString())
			: null;
	}

	/// <summary>Increment the current value.</summary>
	public void Increment()
	{
		this.Set(this.Current + 1);
	}
}