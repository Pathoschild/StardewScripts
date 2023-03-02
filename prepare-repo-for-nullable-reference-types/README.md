This script prepares a C# mod repo to migrate to [nullable reference type annotations]
(https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references).

Specifically, the script...
1. Adds `<Nullable>enable</Nullable>` to each project file.
2. For each C# code file:
   1. If it has `#nullable enable`, remove it and continue to the next file.
   2. If it only has an enum, continue to the next file.
   3. Else add `#nullable disable` to the file.

Once that's done, you can go through code (starting from the lowest level) and remove the
`#nullable disable` as each file is migrated.

# Usage
To use this script:

1. Open the script file in [LINQPad](https://www.linqpad.net).
2. Change the settings under 'configure' near the top.
3. Click the execute (▶) button to generate the translated event.
