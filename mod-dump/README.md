This folder has scripts used to download and analyze all Stardew Valley mods. This is used to help
keep the [mod compatibility list](https://stardewvalleywiki.com/Modding:Mod_compatibility#SMAPI_mods)
up-to-date, prepare monthly stats [for Patreon](https://www.patreon.com/pathoschild), and perform
analyses across all mods (e.g. to see which mods are using a particular feature).

# Scripts
## Check mod updates
`check mod updates.linq` scans the given `Mods` folder to find mods, crossreferences them to
the wiki and SMAPI update-check API, and displays the metadata for review.

Usage:
1. [Install SMAPI](https://smapi.io/).
2. Open the script file in [LINQPad](https://www.linqpad.net).
3. Fix the references to `SMAPI.Toolkit.CoreInterfaces.dll` and `SMAPI.Toolkit.dll` in the Stardew
   Valley folder.
4. Optionally edit the script to set `GameFolderPath`, `ModFolderPath`, and other options.
5. Run the script to fetch & display metadata.

## Fetch mods
`fetch mods.linq` downloads every Stardew Valley mod from CurseForge + ModDrop + Nexus, unpacks the
downloads, and runs LINQ queries on them. Once downloaded, the local cache can be updated
incrementally with new/updated mods.

Usage:
1. [Install SMAPI](https://smapi.io/).
2. [Get a personal API key from Nexus Mods](https://www.nexusmods.com/users/myaccount?tab=api).
3. Open the script file in [LINQPad](https://www.linqpad.net).
4. Fix the references to `SMAPI.Toolkit.CoreInterfaces.dll` and `SMAPI.Toolkit.dll` in the Stardew
   Valley folder.
5. Edit the script to set `ModSites` credentials, `RootPath`, and `FetchMods` near the top.
6. Run the script to begin downloading.

The script will handle API rate limits automatically. The first download may take a very long time.

## Fetch mod repositories
`fetch mod repositories.linq` fetches the list of SMAPI mods from the wiki, and downloads every
Git repository for those mods into a local folder. This is used to populate the [smapi-mod-dump](https://github.com/Pathoschild/smapi-mod-dump)
repository.

Usage:
1. [Install SMAPI](https://smapi.io/).
2. Install [Git](https://git-scm.com) and [Mercurial](https://www.mercurial-scm.org).
3. Open the script file in [LINQPad](https://www.linqpad.net).
4. Fix the references to `SMAPI.Toolkit.CoreInterfaces.dll` and `SMAPI.Toolkit.dll` in the Stardew
   Valley folder.
5. Optionally edit the script to set `RootPath` and other options.
6. Run the script to begin downloading.

## Wiki compatibility page scripts
* `wiki compat - get stats.js` can be run while [editing the mod compatibility page]
  (https://stardewvalleywiki.com/Modding:Mod_compatibility?action=edit&section=3)
  to log some high-level compatibility stats about the listed mods.
* `wiki compat - highlight workaround links to a broken mod.js` can be run while editing [the mod
  compatibility page](https://stardewvalleywiki.com/Modding:Mod_compatibility)
  to highlight any 'use X instead' links which point to a mod which is marked broken, abandoned, or bsolete.
* `wiki compat - get stats.linq` lets you paste the [mod compatibility section](https://stardewvalleywiki.com/Modding:Mod_compatibility?action=edit&section=3)
  into the script (at the bottom), then run it to output the sorted table.
