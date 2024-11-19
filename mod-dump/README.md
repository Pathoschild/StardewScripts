This folder has scripts used to download and analyze all Stardew Valley mods. This is used to help
keep the [mod compatibility list](https://github.com/Pathoschild/SmapiCompatibilityList) up-to-date,
prepare monthly stats [for Patreon](https://www.patreon.com/pathoschild), and perform analyses
across all mods (e.g. to see which mods are using a particular feature).

# Scripts
## Fetch mods
> [!NOTE]
> This script uses special API endpoints that aren't publicly accessible. You'll need special
> permission from CurseForge, ModDrop, and Nexus before you can use this script.

`fetch mods.linq` downloads every Stardew Valley mod from CurseForge + ModDrop + Nexus, unpacks the
downloads, and runs some automated analyses and stats on them. Once downloaded, the local cache is
updated incrementally with new and updated mods.

Usage:
1. [Install SMAPI](https://smapi.io/).
2. Open the script file in [LINQPad](https://www.linqpad.net).
3. Edit the "Configuration" options at the top as needed.
4. Fix the references to `SMAPI.Toolkit.CoreInterfaces.dll` and `SMAPI.Toolkit.dll` in the Stardew
   Valley folder.
5. Run the script to begin downloading.

The script will handle API rate limits automatically, and will automatically continue from the last
fetch when run again. The first run may take 2-3 days due to Nexus rate limits.

## Check mod updates
> [!NOTE]
> This script uses special API endpoints that aren't publicly accessible. You'll need special
> permission from CurseForge, ModDrop, and Nexus before you can use this script.

`check mod updates.linq` scans the given `Mods` folder to find mods, crossreferences them to
the wiki and SMAPI update-check API, and displays the metadata for review.

This assumes you've already run `fetch-mods.linq`.

Usage:
1. [Install SMAPI](https://smapi.io/).
2. Open the script file in [LINQPad](https://www.linqpad.net).
3. Edit the "Configuration" options at the top as needed.
4. Fix the references to `SMAPI.Toolkit.CoreInterfaces.dll` and `SMAPI.Toolkit.dll` in the Stardew
   Valley folder.
5. Run the script to fetch & display metadata.

## Fetch mod repositories
`fetch mod repositories.linq` fetches the list of SMAPI mods from the wiki, and downloads every
Git repository for those mods into a local folder so you.

Usage:
1. [Install SMAPI](https://smapi.io/).
2. Install [Git](https://git-scm.com) and [Mercurial](https://www.mercurial-scm.org).
3. Open the script file in [LINQPad](https://www.linqpad.net).
4. Fix the references to `SMAPI.Toolkit.CoreInterfaces.dll` and `SMAPI.Toolkit.dll` in the Stardew
   Valley folder.
5. Optionally edit the script to set `RootPath` and other options.
6. Run the script to begin downloading.

## Compatibility list scripts
These are utilities to help maintain the SMAPI mod compatibility list:
* `compat repo - get stats.linq` logs some high-level mod compatibility stats.
* `compat repo - detect incorrect mod links in summaries.js` reports 'use X instead' links which
  point to a mod which is marked broken, abandoned, or obsolete.
* `wiki compat - sort list.linq` sorts the compatibility list.

To use the scripts:
1. Clone the [mod compatibility repo](https://github.com/Pathoschild/SmapiCompatibilityList).
2. Open a script file in [LINQPad](https://www.linqpad.net).
3. Update the path to the compatibility list file at the top if needed.
4. Run the script.
