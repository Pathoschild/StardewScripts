This little script decompiles every `.dll` and `.exe` file found in a given folder into individual
.NET projects, and creates a solution containing all of the projects.

The projects will be grouped by the original folder name in the `Mods` folder.

This lets you quickly skim through all their decompiled code (e.g. to find the mod which is doing a
particular thing, search for malicious code patterns, etc).

# Usage
To use this script on Windows:

1. Install...
   - [ILSpy](https://github.com/icsharpcode/ILSpy);
   - [VS Code](https://code.visualstudio.com);
   - [PowerShell 7+](https://learn.microsoft.com/en-us/powershell/scripting/whats-new/migrating-from-windows-powershell-51-to-powershell-7).
2. Open the `.ps1` file in VS Code.  
   _If prompted, install the PowerShell extension._
3. Edit the settings at the top of the file if needed.
4. Press `F5` to run the script, and check the _Terminal_ pane for output.

# Caveats
This is a quick tool for developers, it's not meant to handle every edge case.

In particular:

- It assumes that each folder under `$modsPath` represents a mod. It'll still work if you manually
  organized the folder into subdirectories, but the grouping by mod name will be incorrect.
- It decompiles every assembly found. It doesn't parse the `manifest.json` files and try to walk
  dependencies, which would be unreliable due to dynamic assembly loading.