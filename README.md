This repository contains [LINQPad](https://www.linqpad.net) scripts used to download and analyze
all Stardew Valley mods.

## Scripts
### Check mod updates
`src/check mod updates.linq` scans the given `Mods` folder to find mods, crossreferences them to
the wiki and SMAPI update-check API, and displays the metadata for review.

Usage:
1. [Install SMAPI](https://smapi.io/).
2. Fix the references to `SMAPI.Toolkit.CoreInterfaces.dll` and `SMAPI.Toolkit.dll` in the Stardew
   Valley folder.
3. Optionally edit the script to set `GameFolderPath`, `ModFolderPath`, and other options.
4. Run the script to fetch & display metadata.

### Fetch mods
`src/fetch mods.linq` downloads every Stardew Valley mod from Nexus Mods, unpacks the downloads,
and runs LINQ queries on them. Once downloaded, the local cache can be updated incrementally with
new/updated mods.

Usage:
1. [Install SMAPI](https://smapi.io/).
2. [Get a personal API key from Nexus Mods](https://www.nexusmods.com/users/myaccount?tab=api).
3. Fix the references to `SMAPI.Toolkit.CoreInterfaces.dll` and `SMAPI.Toolkit.dll` in the Stardew
   Valley folder.
4. Edit the script to set `ModSites` credentials, `RootPath`, and `FetchMods` at the top.
5. Run the script to begin downloading.

The script will handle API rate limits automatically. The first download may take a very long time.

### Fetch mod repositories
`src/fetch mod repositories.linq` fetches the list of SMAPI mods from the wiki, and downloads every
Git repository for those mods into a local folder. This is used to populate the [smapi-mod-dump](https://github.com/Pathoschild/smapi-mod-dump)
repository.

Usage:
1. [Install SMAPI](https://smapi.io/).
2. Fix the references to `SMAPI.Toolkit.CoreInterfaces.dll` and `SMAPI.Toolkit.dll` in the Stardew
   Valley folder.
3. Optionally edit the script to set `RootPath` and other options.
4. Run the script to begin downloading.
