This repository contains a [LINQPad](https://www.linqpad.net) project which downloads every Stardew
Valley mod from Nexus Mods, unpacks the downloads, and runs LINQ queries on them. Once downloaded,
the local cache can be updated incrementally with new/updated mods.

## Usage
1. [Install SMAPI](https://smapi.io/).
2. [Get a personal API key from Nexus Mods](https://www.nexusmods.com/users/myaccount?tab=api).
3. Edit `fetch mods.linq` and set `ApiKey`, `RootPath`, and `FetchMods` at the top.
4. Fix the references to `SMAPI.Toolkit.CoreInterfaces.dll` and `SMAPI.Toolkit.dll` in the Stardew Valley folder.
5. Run the script to begin downloading.

The script will handle API rate limits automatically. The first download may take a very long time.
