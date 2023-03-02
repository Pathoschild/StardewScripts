<Query Kind="Program" />

/*
Overview
-------------------------------------------------
This script prepares a C# mod repo to migrate to nullable reference type annotations:
https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references.

Specifically, the script...
   1. Adds `<Nullable>enable</Nullable>` to each project file.
   2. For each C# code file:
      a. If it has `#nullable enable`, remove it and continue to the next file.
      b. If it only has an enum, continue to the next file.
      c. Else add `#nullable disable` to the file.

Once that's done, you can go through code (starting from the lowest level) and remove the
`#nullable disable` as each file is migrated.


Usage
-------------------------------------------------
To use this script:

1. Open this file in LINQPad. (It's a small app: https://www.linqpad.net/.)
2. Change the settings under 'configure' below.
3. Click the execute (▶) button to generate the translated event.
*/

/*********
** Configure
*********/
/****
** Repo
****/
/// <summary>The absolute path to the code repo to update.</summary>
readonly string RepoPath = @"E:\source\_Stardew\Mods.Pathoschild";

/****
** Add <Nullable>enable</Nullable> to project files
****/
/// <summary>Whether to enable nullable annotations in the project files.</summary>
readonly bool EditProjects = false;

/// <summary>When adding the nullable property in a project file, whether to add a blank line between it and the previous property.</summary>
readonly bool AddLineBeforeProjectProperty = false;

/// <summary>The encoding to use when reading and writing project files.</summary>
readonly Encoding ProjectFileEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

/****
** Add #nullable disable to code files
****/
/// <summary>Whether to disable nullable annotations in the code files.</summary>
readonly bool EditCodeFiles = true;

/// <summary>When adding <c>#nullable disable</c> to a code file, whether to add a blank line between it and the next line.</summary>
readonly bool AddLineAfterCodeTag = true;

/// <summary>The encoding to use when reading and writing project files.</summary>
readonly Encoding CodeFileEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);


/*********
** Script
*********/
/// <summary>Apply the changes.</summary>
void Main()
{
	DirectoryInfo repoDir = new DirectoryInfo(this.RepoPath);

	if (this.EditProjects)
		this.ApplyProjectChanges(repoDir);

	if (this.EditCodeFiles)
		this.ApplyCodeChanges(repoDir);
}

/// <summary>Enable nullable annotations in the project files.</summary>
/// <param name="repoDir">The code repo folder to update.</param>
void ApplyProjectChanges(DirectoryInfo repoDir)
{
	// get file list
	FileInfo[] files = repoDir.GetFiles("*.csproj", SearchOption.AllDirectories);
	$"Enabling nullable annotations in {files.Length} project files...".Dump();

	// edit each file
	var progress = new Util.ProgressBar().Dump();
	for (int i = 0; i < files.Length; i++)
	{
		var file = files[i];

		// log progress
		progress.Caption = $"Processing {Path.GetRelativePath(repoDir.FullName, file.FullName)}...";
		progress.Fraction = i / (files.Length * 1d);

		// load file
		string content = File.ReadAllText(file.FullName, this.ProjectFileEncoding);

		// use existing <Nullable>
		bool added = false;
		content = Regex.Replace(
			content,
			@"([\r\n\s]+< *Nullable *>) *(enable|disable) *(< */ *Nullable *>[\r\n]+)",
			match => {
				string before = match.Groups[1].Value;
				string value = match.Groups[2].Value;
				string after = match.Groups[3].Value;

				if (!added && string.Equals(value, "enable", StringComparison.InvariantCultureIgnoreCase))
				{
					added = true;
					return match.Value;
				}

				return Environment.NewLine;
			},
			RegexOptions.IgnoreCase
		);

		// else add to first property group
		if (!added)
		{
			content = Regex.Replace(
				content,
				@"^([ \t]+)(<[^>\r\n]+>[^<\r\n]+<[^<\r\n]+> *[\r\n\s]+?)([ \t]*< */ *PropertyGroup *>)",
				match =>
				{
					string propIndent = match.Groups[1].Value;
					string previousLine = match.Groups[2].Value;
					string nextLine = match.Groups[3].Value;

					added = true;

					return string.Concat(
						propIndent, previousLine.TrimEnd(),
						(this.AddLineBeforeProjectProperty ? Environment.NewLine : string.Empty),
						Environment.NewLine, propIndent, "<Nullable>enable</Nullable>",
						Environment.NewLine, nextLine
					);
				},
				RegexOptions.IgnoreCase | RegexOptions.Multiline
			);
		}

		if (!added)
		{
			Util.WithStyle($"Can't apply change to {Path.GetRelativePath(repoDir.FullName, file.FullName)}: no <PropertyGroup> found.", "color: red; font-weight: bold;").Dump();
			continue;
		}

		// save changes
		File.WriteAllText(file.FullName, content, this.ProjectFileEncoding);
	}
	progress.Visible = false;
}

/// <summary>Disable nullable annotations in code files.</summary>
/// <param name="repoDir">The code repo folder to update.</param>
private void ApplyCodeChanges(DirectoryInfo repoDir)
{
	// get repo file list
	FileInfo[] files = repoDir.GetFiles("*.cs", SearchOption.AllDirectories);
	$"Processing {files.Length} files in {this.RepoPath}...".Dump();

	// process each file
	var progress = new Util.ProgressBar().Dump();
	for (int i = 0; i < files.Length; i++)
	{
		var file = files[i];

		// log progress
		progress.Caption = $"Processing {Path.GetRelativePath(repoDir.FullName, file.Directory.FullName)}...";
		progress.Fraction = i / (files.Length * 1d);

		// load file
		string content = File.ReadAllText(file.FullName, this.CodeFileEncoding);
		bool changed = false;

		// remove annotation if needed
		bool shouldAddAnnotation = true;
		content = Regex.Replace(content, @"[\r\n\s]*#nullable\s*(enable|disable)[\r\n\s]*", match =>
		{
			shouldAddAnnotation = false;
			
			if (string.Equals(match.Groups[1].Value, "enable", StringComparison.OrdinalIgnoreCase))
			{
				changed = true;
				return match.Index == 0
					? string.Empty
					: Environment.NewLine;
			}
			else
				return match.Value;
		});

		// skip if null definitely not possible
		if (shouldAddAnnotation)
			shouldAddAnnotation = Regex.IsMatch(content, @"\b(?:class|delegate|event|interface|struct)\b"); // any data structure besides an enum

		// add #nullable disable
		if (shouldAddAnnotation)
		{
			content = string.Concat(
				"#nullable disable",
				Environment.NewLine,
				(this.AddLineAfterCodeTag ? Environment.NewLine : string.Empty),
				content
			);
			changed = true;
		}

		// save changes
		if (changed)
			File.WriteAllText(file.FullName, content, this.CodeFileEncoding);
	}

	progress.Visible = false;
}