These instruction let you download the source code for every open-source C# mod from the [SMAPI mod
compatibility list](https://stardewvalleywiki.com/Modding:SMAPI_compatibility), so you can search
through mod code and perform analyses.

## Fetch mod source code
1. Install [Git](https://git-scm.com/) and [Mercurial](https://www.mercurial-scm.org/).
2. Create a new folder to contain the fetched repos.
3. Open a terminal in your new folder.
4. Run these commands:
   ```sh
   # last updated 2025-11-30.

   hg clone http://hg.code.sf.net/p/sdvmod-24h-harmony/code "24-Hour Clock Patcher" # 1 mod: 24-Hour Clock Patcher
   hg clone http://hg.code.sf.net/p/sdvmod-damage-overlay/code "Bloody Damage Overlay" # 1 mod: Bloody Damage Overlay
   hg clone http://hg.code.sf.net/p/sdvmod-even-better-rng/code "Even Better RNG" # 1 mod: Even Better RNG
   hg clone http://hg.code.sf.net/p/sdvmod-grandfather-s-gift-re/code "Grandfather's Gift Remade" # 1 mod: Grandfather's Gift Remade
   hg clone http://hg.code.sf.net/p/sdvmod-remind-to-exit/code "Remind to Exit" # 1 mod: Remind to Exit
   hg clone http://hg.code.sf.net/p/sdvmod-silo-size/code "Silo Size" # 1 mod: Silo Size
   hg clone http://hg.code.sf.net/p/sdvmod-tapper-report/code "Tapper Report" # 1 mod: Tapper Report
   hg clone http://hg.code.sf.net/p/sdvmods-artifact-fix-redux/code "Artifact System Drop Framework & Geodes Handler" # 1 mod: Artifact System Drop Framework & Geodes Handler
   git clone -q https://github.com/1Avalon/Avas-Stardew-Mods.git "~1Avalon" # 5 mods: Custom NPC Paintings, Friendship Bars, Friendship Streaks, Improved Fall Debris, Prismatic Quality
   git clone -q https://github.com/1Avalon/Love-Festival.git "Love Festival" # 1 mod: Love Festival
   git clone -q https://github.com/1Avalon/Ore-Detector.git "Ore Detector" # 1 mod: Ore Detector
   git clone -q https://github.com/1F1XDRAGO/Radio-Stardew-valley-Mod-.git "Radio (Audio Play)" # 1 mod: Radio (Audio Play)
   git clone -q https://github.com/24v/BuffNotifications.git "Buff Notifications" # 1 mod: Buff Notifications
   git clone -q https://github.com/24v/SproutSight.git "SproutSight Pro™" # 1 mod: SproutSight Pro™
   git clone -q https://github.com/2Retr0/FarmhandFinder.git "Farmhand Finder" # 1 mod: Farmhand Finder
   git clone -q https://github.com/2Retr0/PlacementPlus.git "Placement Plus" # 1 mod: Placement Plus
   git clone -q https://github.com/40022808/SMAPI_MOD_IamFishingMaster.git "I Am Fishing Master" # 1 mod: I Am Fishing Master
   git clone -q https://github.com/40022808/SMAPI_MOD_IamSportsStudents.git "I Am Sports Students" # 1 mod: I Am Sports Students
   git clone -q https://github.com/435201823/CookingShowdown.git "Cook Showdown" # 1 mod: Cook Showdown
   git clone -q https://github.com/4ppleCracker/NewGameOnLaunch.git "New Game on Launch" # 1 mod: New Game on Launch
   git clone -q https://github.com/6135/StardewValley.ProfitCalculator.git "ProfitCalculator" # 2 mods: Profit Calculator, Profit Calculator → DGA Support
   git clone -q https://github.com/6480k/colorful-fishing-rods.git "Colorful Fishing Rods" # 1 mod: Colorful Fishing Rods
   git clone -q https://github.com/9Rifleman/JustSleepIn.git "Just Sleep In" # 1 mod: Just Sleep In
   git clone -q https://github.com/AairTheGreat/StardewValleyMods.git "~AairTheGreat" # 4 mods: Better Garbage Cans, Better Panning, Better Train Loot, Fish Finder
   git clone -q https://github.com/Abagaianye/AbaStardewMods.git "Apollo's Trick or Treat" # 1 mod: Apollo's Trick or Treat
   git clone -q https://github.com/AbbyNode/StardewValley-CoalRegen.git "Coal Regen" # 1 mod: Coal Regen
   git clone -q https://github.com/aceynk/AdvancedFishingTreasure.git "Advanced Fishing Treasure" # 1 mod: Advanced Fishing Treasure
   git clone -q https://github.com/aceynk/Buffs-Last.git "Buffs Last" # 1 mod: Buffs Last
   git clone -q https://github.com/aceynk/CustomMoss.git "Custom Moss" # 1 mod: Custom Moss
   git clone -q https://github.com/aceynk/PersistentBuffs.git "Persistent Buffs" # 1 mod: Persistent Buffs
   git clone -q https://github.com/Achtuur/StardewMods.git "~Achtuur" # 8 mods: AchtuurCore, Better Planting, Better Rods, Hover Labels, Junimo Beacon, Multiplayer Exp Share, Prismatic Statue, Watering Can Give Exp
   git clone -q https://github.com/Achtuur/StardewTravelSkill.git "Travelling Skill" # 1 mod: Travelling Skill
   git clone -q https://github.com/AcidicNic/StardewValleyMods.git "Special Orders Anywhere" # 1 mod: Special Orders Anywhere
   git clone -q https://github.com/AdeelTariq/ObjectProgressBars.git "Object Progress Bars" # 1 mod: Object Progress Bars
   git clone -q https://github.com/Adham084/FPSViewer.git "Frame Rate Viewer" # 1 mod: Frame Rate Viewer
   git clone -q https://github.com/ADoby/SimpleSprinkler.git "Simple Sprinkler" # 1 mod: Simple Sprinkler
   git clone -q https://github.com/Adradis/StardewMods.git "Limited Regrowable Crop Harvests" # 1 mod: Limited Regrowable Crop Harvests
   git clone -q https://github.com/adroslice/sdv-relativity-mirror.git "Relativity" # 1 mod: Relativity
   git clone -q https://github.com/adverserath/QuickGlance.git "Quick Glance" # 1 mod: Quick Glance
   git clone -q https://github.com/adverserath/StardewValley-CasualLifeMod.git "Casual Life" # 1 mod: Casual Life
   git clone -q https://github.com/AdvizeGH/FarmExpansion.git "Farm Expansion" # 1 mod: Farm Expansion
   git clone -q https://github.com/AdvizeGH/GetDressed.git "Get Dressed" # 1 mod: Get Dressed
   git clone -q https://github.com/AdvizeGH/LovedLabels.git "Loved Labels" # 1 mod: Loved Labels
   git clone -q https://github.com/aedenthorn/StardewValleyMods.git "~aedenthorn" # 243 mods: Additional Mine Maps, Advanced Cooking, Advanced Flute Blocks, Advanced Loot Framework, Advanced Melee Framework, Advanced Menu Positioning, AFK Pause, Alarms, All Chests Menu, Animal Dialogue Framework, Animated Parrot and Perch, Another Jump Mod, April Bug Fix Suite, Bat Form, Bed Tweaks, Bee Paths, Better Elevator, Better Lightning Rods, Birthday Buff, Birthday Friendship, Boss Creatures, Buff Framework, Building Shift, Bulk Animal Purchase, Catalogue Filter, Chess Boards, Coin Collector, Connected Garden Pots, Content Patcher Editor, Cooperative Egg Hunt, Craft and Build from Containers, Craftable Butterfly Hutches, Craftable Terrarium, Crop Growth Info, Crop Harvest Bubbles, Crop Hat, Crop Markers, Crop Variation, Crop Watering Bubbles, Crops Survive Season Change, Custom Achievements, Custom Backpack Framework, Custom Chest Types, Custom Decoration Areas, Custom Dungeon Floors, Custom Fixed Dialogue, Custom Gift Limits, Custom Hay, Custom Locks, Custom Mine Cart Steam, Custom Mounts, Custom Object Production, Custom Ore Nodes, Custom Pet Beds, Custom Picture Frames, Custom Renovations, Custom Resource Clumps, Custom Signs, Custom Spouse Patio, Custom Spouse Patio Redux, Custom Spouse Patio Wizard, Custom Spouse Rooms, Custom Starter Furniture, Custom Starter Package, Custom Toolbar, Custom Tree Tweaks, Custom Wallpaper Framework, Custom Warp Points, Death Tweaks, Dialogue Display Framework, Dialogue New Line, Dialogue Trees, Dino Form, Dreamscape, Dungeon Merchants, Dynamic Flooring, Dynamic Map Tiles, Dynamic Map Tiles Extended, Email App, Emily for Stardew Impact, Enhanced Loot Magnet, Expert Sitting, Extra Map Layers, Familiars, Farm Animal Harvest Helper, Farm Cave Framework, Farmer Helper, Farmer Portraits, Farmer Portraits for 1.6, Field Harvest, Fire Breath, Fish Spot Bait, Fishing Chests Expanded, Floating Garden Pots, Flowing Mine Rocks, Food on the Table, Free Love, Friendly Divorce, Friendship Tweaks, Fruit Tree Shaker, Fruit Tree Tweaks, Furniture Adjustment, Furniture Display Framework, Furniture Placement Tweaks, Furniture Recolor, Galaxy Weapon Choice, Garden Pot Tweaks, Gem Isles, Genie Lamp, Gift Rejection, Groundhog Day, Harvest Seeds, Help Wanted, Here Fishy, Hugs and Kisses, Immersive Scarecrows, Immersive Sprinklers, In-Game SMAPI Log Uploader, Infallible Fishing, Instant Building Construction and Upgrade, Instant Growth Powder, Inventory Indicators, Krobus Roommate Shop, Light Mod, Like a Duck to Water, Livestock Choices, Log Spam Filter, Longer Seasons, Loose Audio Files, Mailbox Menu, Map Editor Extended, Map Teleport, Mayo Mart, Meteor Defence, Mobile Arcade, Mobile Audio Player, Mobile Calendar, Mobile Catalogues, Mobile Phone, Mobile Television, Modify This, Moolah Money Mod, Move Greenhouse Plot, Move It, Moveable Mailbox, Moveable Pet Bowl, Movie Theatre Tweaks, Multi Save, Multiple Floor Farmhouse, Multiple Spouses, Murdercrows, Musical Paths, Napalm Mummies, Night Event Chance Tweak, Non-Random Prairie King, NPC Clothing Framework, Object Import Map, Object Product Display, OK Night Check, Omni Tools, Outfit Sets, Overworld Chests, Pacifist Valley, Painting Display, Partial Hearts, Particle Effects, Pelican xVASynth, Persistent Grange Display, Personal Traveling Cart, Pet Hats, Pipe Irrigation, Placeable Mine Shafts, Planned Parenthood, Plant and Fertilize All, Planter Trees, Play Prairie King With NPCs, Playground Mod, Poop Framework, Portable Furnace, Prismatic Fire, Prismatic Furniture, Purist Mod, Quest Time Limits, Quick Chest Color, Quick Load, Quick Responses, Quick Responses (1.6 Updated), Quotes, Rainbow Trail, Random NPCs, Realistic Random Names, Resource Storage, Restauranteer, Roasting Marshmallows, Robin Work Hours, Secret Hedge Maze, Seed Info, Seed Maker Tweaks, Self Serve, Sewer Slimes, Show Players Behind Buildings, Simple Screenshots, Sixty-Nine Shirt, Skateboard, Slime Hutch Water Spots, Social Page Order Button, Social Page Order Menu, Sound Effect Replacement, Sound Tweaker, Spoilage, Sprinkler Mod, Stacked Item Icons, Stardew Impact, Stardew Open World, Stardew RPG, Statue Shorts, Submerged Crab Pots, Swim, Take All, Tappable Palm Trees, Tile Transparency, Tool Smart Switch, Train Tracks, Trampoline, Transparent Objects, Trash Can Reactions, Trash Cans On Horse, Treasure Chests Expanded, Underground Secrets, Unique Valley, Utility Grid, Video Player, Wall Planters, Wall Televisions, Watering Can Tweaks, Weapons Ignore Grass, Weather Totem, Wedding Tweaks, Week Starts Sunday, Wiki Links, Wild Flowers, Witcher Mod, Zombie Outbreak
   git clone -q https://github.com/aEnigmatic/ConvenientChests.git "Convenient Chests" # 1 mod: Convenient Chests
   git clone -q https://github.com/aeremyns/AdoptWithKrobus.git "Adopt With Krobus" # 1 mod: Adopt With Krobus
   git clone -q https://github.com/aeremyns/GNMTokensMod.git "Gender Neutrality Mod Tokens" # 1 mod: Gender Neutrality Mod Tokens
   git clone -q https://github.com/Aflojack/BetterWateringCanAndHoe.git "Better Watering Can and Hoe" # 1 mod: Better Watering Can and Hoe
   git clone -q https://github.com/agilbert1412/ArchipelagoStandaloneMods.git "MultiSleep" # 1 mod: MultiSleep
   git clone -q https://github.com/agilbert1412/StardewArchipelago.git "Stardew Archipelago" # 1 mod: Stardew Archipelago
   git clone -q https://github.com/AguirreMoy/Stardew-Multiplayer-Server-Mod.git "Stardew Multiplayer Server" # 1 mod: Stardew Multiplayer Server
   git clone -q https://github.com/AHilyard/BerrySeasonReminder.git "Berry Season Reminder" # 1 mod: Berry Season Reminder
   git clone -q https://github.com/AHilyard/DwarvishMattock.git "Dwarvish Mattock" # 1 mod: Dwarvish Mattock
   git clone -q https://github.com/AHilyard/ImprovedTracker.git "Improved Tracker" # 1 mod: Improved Tracker
   git clone -q https://github.com/AHilyard/UpgradeablePan.git "Upgradable Pan" # 1 mod: Upgradable Pan
   git clone -q https://github.com/AHilyard/WeaponsOnDisplay.git "Weapons on Display" # 1 mod: Weapons on Display
   git clone -q https://github.com/AlanBF1992/BigFridge.git "Big Fridges" # 1 mod: Big Fridges
   git clone -q https://github.com/AlanBF1992/BlinkFix.git "Blink Fix" # 1 mod: Blink Fix
   git clone -q https://github.com/AlanBF1992/FixHoverShadows.git "Fix Hover Shadows" # 1 mod: Fix Hover Shadows
   git clone -q https://github.com/AlanBF1992/MasteryExtended.git "~AlanBF1992" # 3 mods: Mastery Extended, Mastery Extended → VPP Compat, Mastery Extended → WoL Compat
   git clone -q https://github.com/AlanBF1992/WorldAtlas.git "World Atlas" # 1 mod: World Atlas
   git clone -q https://github.com/AlanDavison/StardewValleyMods.git "~AlanDavison" # 13 mods: Better Crystalariums, Better Return Scepter, Buff Crops, Carry Your Pet, Default Window Size, Dialogue Tester, Give Me My Cursor Back, Into the Game (Save Loaded Notifier), Mapping Extensions and Extra Properties (MEEP), Player Co-ordinate HUD, Smart Building, Smart Cursor, Zoom Per Map
   git clone -q https://github.com/alanperrow/StardewModding.git "~alanperrow" # 3 mods: Convenient Inventory, Faster Path Speed, Splitscreen Improved
   git clone -q https://github.com/alcmoe/FreezeTimeMultiplayer.git "Freeze Time Multiplayer" # 1 mod: Freeze Time Multiplayer
   git clone -q https://github.com/alcmoe/SVMods.git "Weird Sounds" # 1 mod: Weird Sounds
   git clone -q https://github.com/AlejandroAkbal/Stardew-Valley-OmniSwing-Mod.git "Omni Swing" # 1 mod: Omni Swing
   git clone -q https://github.com/AlejandroAkbal/Stardew-Valley-Quick-Sell-Mod.git "Quick Sell" # 1 mod: Quick Sell
   git clone -q https://github.com/Alhifar/WaitAroundSMAPI.git "Wait Around" # 1 mod: Wait Around
   git clone -q https://github.com/Alison-Li/auto-bait-and-tackles-mod.git "Auto Bait and Tackles" # 1 mod: Auto Bait and Tackles
   git clone -q https://github.com/AllanWang/Stardew-Mods.git "Quick Swap" # 1 mod: Quick Swap
   git clone -q https://github.com/alpeerkaraca/SDV-Show-HP-Bar.git "Mob Health Bars" # 1 mod: Mob Health Bars
   git clone -q https://github.com/Alphablackwolf/SkillPrestige.git "SkillPrestige" # 5 mods: Skill Prestige, Skill Prestige for All SpaceCore Skill Mods, Skill Prestige for Cooking Skill, Skill Prestige for Luck Skill, Skill Prestige for Magic
   git clone -q https://github.com/AlphaMeece/StardewMods.git "~AlphaMeece" # 2 mods: Skill Rings, This Round's On Me
   git clone -q https://github.com/amazingalek/Stardew-ChangeProfessions.git "Change Professions" # 1 mod: Change Professions
   git clone -q https://github.com/amconners/ReorientAfterEating.git "Reorient After Eating" # 1 mod: Reorient After Eating
   git clone -q https://github.com/ameisen/SV-SpriteMaster.git "SpriteMaster" # 1 mod: SpriteMaster
   git clone -q https://github.com/andraemon/SlimeProduce.git "Slime Produce" # 1 mod: Slime Produce
   git clone -q https://github.com/AndrewGraber/Skull-Cavern-Drill-Redux.git "Skull Cavern Drill Redux" # 1 mod: Skull Cavern Drill Redux
   git clone -q https://github.com/andyruwruw/stardew-valley-pet-bowl-sprinklers.git "Pet Bowl Sprinklers" # 1 mod: Pet Bowl Sprinklers
   git clone -q https://github.com/andyruwruw/stardew-valley-water-bot.git "Water Bot" # 1 mod: Water Bot
   git clone -q https://github.com/AngelaRanna/StardewMods.git "~AngelaRanna" # 3 mods: Angel's Existing Weapon Updater, Angel's Lead Rod Drop Rate Fixer, Angel's Weapon Rebalance
   git clone -q https://github.com/AngeloC3/StardewMods.git "Stardew Media Keys" # 1 mod: Stardew Media Keys
   git clone -q https://github.com/Annosz/StardewValleyModding.git "~Annosz" # 5 mods: Cheaper Beach Bridge Repair, Highlighted Jars, Krobus Sells Larger Stacks, Remember Faced Direction, Sleepless Fisherman
   git clone -q https://github.com/Annosz/UiInfoSuite2.git "Ui Info Suite 2" # 1 mod: Ui Info Suite 2
   git clone -q https://github.com/AnotherPillow/ModifiableFruitRegion.git "Modifiable Fruit Region" # 1 mod: Modifiable Fruit Region
   git clone -q https://github.com/AnotherPillow/MushroomBoxLocationFramework.git "Mushroom Box Location Framework" # 1 mod: Mushroom Box Location Framework
   git clone -q https://github.com/AnotherPillow/No-Meteors.git "No Meteors" # 1 mod: No Meteors
   git clone -q https://github.com/AnotherPillow/NoGreenRain.git "No Green Rain" # 1 mod: No Green Rain
   git clone -q https://github.com/AnotherPillow/RelocateSaves.git "Relocate Saves" # 1 mod: Relocate Saves
   git clone -q https://github.com/AnotherPillow/SummitPatches.git "Summit Patches" # 1 mod: Summit Patches
   git clone -q https://github.com/AnotherPillow/WarpParsnip.git "Warp Parsnip" # 1 mod: Warp Parsnip
   git clone -q https://github.com/ApryllForever/ArmorBuffs.git "Clothes Give Buffs" # 1 mod: Clothes Give Buffs
   git clone -q https://github.com/ApryllForever/NuclearBombLocations.git "Nuclear Valley - PG" # 1 mod: Nuclear Valley - PG
   git clone -q https://github.com/ApryllForever/PolyamorySweetBed.git "Polyamory Sweet → Polyamory Sweet Bed" # 1 mod: Polyamory Sweet → Polyamory Sweet Bed
   git clone -q https://github.com/ApryllForever/PolyamorySweetKiss.git "Polyamory Sweet → Polyamory Kiss" # 1 mod: Polyamory Sweet → Polyamory Kiss
   git clone -q https://github.com/ApryllForever/PolyamorySweetLove.git "Polyamory Sweet → Polyamory Sweet Love" # 1 mod: Polyamory Sweet → Polyamory Sweet Love
   git clone -q https://github.com/ApryllForever/PolyamorySweetRooms.git "Polyamory Sweet → Polyamory Sweet Rooms" # 1 mod: Polyamory Sweet → Polyamory Sweet Rooms
   git clone -q https://github.com/ApryllForever/PolyamorySweetWedding.git "Polyamory Sweet → Polyamory Sweet Wedding" # 1 mod: Polyamory Sweet → Polyamory Sweet Wedding
   git clone -q https://github.com/ApryllForever/Rummage.git "Rummage - The Thievery Mod" # 1 mod: Rummage - The Thievery Mod
   git clone -q https://github.com/arannya/BoomerangMod.git "Boomerangs" # 1 mod: Boomerangs
   git clone -q https://github.com/Arborsm/PortraiturePlus.git "Portraiture Plus" # 1 mod: Portraiture Plus
   git clone -q https://github.com/Arborsm/ScaleUpUnofficial.git "Scale Up Unofficial" # 1 mod: Scale Up Unofficial
   git clone -q https://github.com/ArenKDesai/Murder-Mod.git "Murder Mod" # 1 mod: Murder Mod
   git clone -q https://github.com/ArjanSeijs/SprinklerMod.git "Increased Sprinkler Range" # 1 mod: Increased Sprinkler Range
   git clone -q https://github.com/Armitxes/StardewValley_UnlimitedPlayers.git "Unlimited Players" # 1 mod: Unlimited Players
   git clone -q https://github.com/aRooooooba/SortingChests.git "Sorting Chests" # 1 mod: Sorting Chests
   git clone -q https://github.com/arphox/StardewValleyMods.git "Show Daily Luck" # 1 mod: Show Daily Luck
   git clone -q https://github.com/arruda/BalancedCombineManyRings.git "Balanced Combine Many Rings" # 1 mod: Balanced Combine Many Rings
   git clone -q https://github.com/Artinity/CleanBeach.git "Clean Beach" # 1 mod: Clean Beach
   git clone -q https://github.com/Artinity/SaloonWarpTotem.git "Saloon Warp Totem" # 1 mod: Saloon Warp Totem
   git clone -q https://github.com/Ash-shroom/Elliott_Piano_Improved.git "Elliott's Piano Improved" # 1 mod: Elliott's Piano Improved
   git clone -q https://github.com/Ash-shroom/Playable_Piano.git "Playable Piano" # 1 mod: Playable Piano
   git clone -q https://github.com/atravita-mods/StardewMods.git "~atravita-mods" # 31 mods: Atra's Interaction Tweaks, AtraCore, Avoid Losing Scepter, Camera Pan, Critter Rings, Draw Fish Ponds Over Grass, Easier Cave Puzzle, Easier Dart Puzzle, Event Tester, Farm Cave Spawns More Tree Fruit, Farm Combat Grants XP Too, Fix Pig Random, Frame Rate Logger, Giant Crop Fertilizer, Ginger Island Mainland Adjustments, Growable Bushes, Growable Giant Crops, Highlight Empty Machines, Holiday Sales, Identifiable Combined Rings, Last Day to Plant Redux, Less Mini Shipping Bin, More Fertilizers, Museum Store, Pick Your Enchantment, Prismatic Clothing, Revive Dead Crops, Sleep In Wedding, Special Orders Tags Extended, Tap Giant Crops, Trash Does Not Consume Bait
   git clone -q https://github.com/aurpine/Stardew-SpriteMaster.git "Clear Glasses" # 1 mod: Clear Glasses
   git clone -q https://github.com/AustinYQM/ImprovedMill.git "Improved Mill" # 1 mod: Improved Mill
   git clone -q https://github.com/AvaKliment/stardew-valley-readersdigest.git "Readers Digest for 1.6" # 1 mod: Readers Digest for 1.6
   git clone -q https://github.com/Aviroen/RoenCore.git "CustomField Chaos" # 1 mod: CustomField Chaos
   git clone -q https://github.com/AxesOfEvil/SV_DeliveryService.git "Delivery Service" # 1 mod: Delivery Service
   git clone -q https://github.com/azah/AutomatedDoors.git "Automated Doors" # 1 mod: Automated Doors
   git clone -q https://github.com/b-b-blueberry/BlueberryMushroomMachine.git "MushroomPropagator" # 2 mods: Mushroom Propagator, Mushroom Propagator [Automate]
   git clone -q https://github.com/b-b-blueberry/CooksAssistant.git "The Love of Cooking" # 1 mod: The Love of Cooking
   git clone -q https://github.com/b-b-blueberry/CustomCommunityCentre.git "CustomCommunityCentre" # 2 mods: Community Kitchen, Custom Community Centre
   git clone -q https://github.com/b-b-blueberry/DesertBus.git "Calico Desert Bus" # 1 mod: Calico Desert Bus
   git clone -q https://github.com/b-b-blueberry/GiftWrapper.git "Gift Wrapper" # 1 mod: Gift Wrapper
   git clone -q https://github.com/b-b-blueberry/GreenhouseEntryPatch.git "Greenhouse Entry Begone!" # 1 mod: Greenhouse Entry Begone!
   git clone -q https://github.com/b-b-blueberry/Halloween2024.git "Halloween Puzzle Dreams" # 1 mod: Halloween Puzzle Dreams
   git clone -q https://github.com/b-b-blueberry/HideBuildingShadows.git "Hide Building Shadows" # 1 mod: Hide Building Shadows
   git clone -q https://github.com/b-b-blueberry/InstantPets.git "Instant Pets" # 1 mod: Instant Pets
   git clone -q https://github.com/b-b-blueberry/RaisedGardenBeds.git "Raised Garden Beds" # 1 mod: Raised Garden Beds
   git clone -q https://github.com/b-b-blueberry/SeedsUnboxed.git "Seeds Unpacked" # 1 mod: Seeds Unpacked
   git clone -q https://github.com/b-b-blueberry/WaterFlow.git "Water Flow" # 1 mod: Water Flow
   git clone -q https://github.com/BadScientist/ConfirmGifts.git "Confirm Gifts" # 1 mod: Confirm Gifts
   git clone -q https://github.com/BananaFruit1492/MonsterMuskInVolcano.git "Monster Musk in Volcano" # 1 mod: Monster Musk in Volcano
   git clone -q https://github.com/BananaFruit476/BulkStaircases.git "Bulk Staircases" # 1 mod: Bulk Staircases
   git clone -q https://github.com/barteke22/StardewMods.git "~barteke22" # 4 mods: Custom Spouse Location, Fishing Info Overlays, Fishing Minigames, Fishing Minigames
   git clone -q https://github.com/BayesianBandit/ConfigureMachineSpeed.git "Configure Machine Speed" # 1 mod: Configure Machine Speed
   git clone -q https://github.com/bcmpinc/StardewHack.git "~bcmpinc" # 12 mods: Always Scroll Map, Craft Counter, Fix Animal Tools, Fix Scythe Exp, Flexible Arms, Grass Growth, Movement Speed, Stardew Hack, Tilled Soil Decay, Tree Spread, Wear More Rings, Yet Another Harvest With Scythe Mod
   git clone -q https://github.com/Becks723/StardewMods.git "~Becks723" # 3 mods: Font Settings, Phone Traveling Cart, Staircase Placement Fix
   git clone -q https://github.com/benrpatterson/InfestedLevels.git "Infested Levels" # 1 mod: Infested Levels
   git clone -q https://github.com/benrpatterson/MushroomLevels.git "Mushroom Levels" # 1 mod: Mushroom Levels
   git clone -q https://github.com/Berisan/SpawnMonsters.git "Spawn Monsters" # 1 mod: Spawn Monsters
   git clone -q https://github.com/berkayylmao/StardewValleyMods.git "~berkayylmao" # 3 mods: ChestEx, Infinite Zoom, Ship Anything
   git clone -q https://github.com/Bifibi/SDVEscapeRope.git "Escape Rope" # 1 mod: Escape Rope
   git clone -q https://github.com/bikinavisho/RainCoatWhenRaining.git "Raincoat When Raining" # 1 mod: Raincoat When Raining
   git clone -q https://github.com/BinaryLip/ScheduleViewer.git "Schedule Viewer" # 1 mod: Schedule Viewer
   git clone -q https://github.com/BinaryLip/TrainInfo.git "Train Info" # 1 mod: Train Info
   git clone -q https://github.com/Binarynova/MyStardewMods.git "~Binarynova" # 2 mods: Instant Tool Upgrades, More Level Up Notifications
   git clone -q https://github.com/bitwisejon/StardewValleyMods.git "~bitwisejon" # 2 mods: Instant Buildings From Farm, One Click Shed Reloader
   git clone -q https://github.com/BladeOfGrass445/another-tax-mod.git "Another Tax Mod" # 1 mod: Another Tax Mod
   git clone -q https://github.com/blamsart/StardewMods.git "~blamsart" # 2 mods: More for Leo, Post Wedding Dates
   git clone -q https://github.com/Blaxsmith/SprinklerActivation.git "Sprinkler Activation" # 1 mod: Sprinkler Activation
   git clone -q https://github.com/BlckHawker/Instant-Community-Center-Cheat.git "Instant Community Center Cheat" # 1 mod: Instant Community Center Cheat
   git clone -q https://github.com/BleakCodex/SimpleSounds.git "Simple Sounds" # 1 mod: Simple Sounds
   git clone -q https://github.com/BleakCodex/SpritesInDetail.git "Sprites in Detail" # 1 mod: Sprites in Detail
   git clone -q https://github.com/bmarquismarkail/SV_BuildersList.git "Builder's List" # 1 mod: Builder's List
   git clone -q https://github.com/bmarquismarkail/SV_PotC.git "Part of the Community" # 1 mod: Part of the Community
   git clone -q https://github.com/bmarquismarkail/SV_VerticalToolMenu.git "Vertical Toolbar" # 1 mod: Vertical Toolbar
   git clone -q https://github.com/bogie5464/HuggableScarecrows.git "Huggable Scarecrows" # 1 mod: Huggable Scarecrows
   git clone -q https://github.com/bogie5464/SiloReport.git "Silo Report" # 1 mod: Silo Report
   git clone -q https://github.com/Bouhm/stardew-valley-mods.git "~Bouhm" # 2 mods: Location Compass, NPC Map Locations
   git clone -q https://github.com/BPavol/stardew-valley-mods.git "Hopper Extractor" # 1 mod: Hopper Extractor
   git clone -q https://github.com/Bpendragon/Best-of-Queen-of-Sauce.git "The Best of _The Queen of Sauce_" # 1 mod: The Best of "The Queen of Sauce"
   git clone -q https://github.com/Bpendragon/DynamicHorses.git "Dynamic Horses" # 1 mod: Dynamic Horses
   git clone -q https://github.com/Bpendragon/ForagePointers.git "Forage Pointers" # 1 mod: Forage Pointers
   git clone -q https://github.com/Bpendragon/GreenhouseSprinklers.git "Greenhouse Sprinklers" # 1 mod: Greenhouse Sprinklers
   git clone -q https://github.com/Brandon22Adams/ToolPouch.git "Tool Pouch" # 1 mod: Tool Pouch
   git clone -q https://github.com/brandonwamboldt/FriendshipTotems.git "Friendship Totems" # 1 mod: Friendship Totems
   git clone -q https://github.com/brandonwamboldt/LockableThings.git "Lockable Things" # 1 mod: Lockable Things
   git clone -q https://github.com/brandonwamboldt/SearchableChests.git "Searchable Chests" # 1 mod: Searchable Chests
   git clone -q https://github.com/Brian-Wuest/Instacrops.git "InstaCrops" # 1 mod: InstaCrops
   git clone -q https://github.com/Brian-Wuest/Quick_Start.git "Quick Start" # 1 mod: Quick Start
   git clone -q https://github.com/briankendall/stardew-switch-controls.git "Switch Controls" # 1 mod: Switch Controls
   git clone -q https://github.com/brog-io/AutoPetter.git "AutoPetter" # 1 mod: AutoPetter
   git clone -q https://github.com/BuildABuddha/StardewDailyPlanner.git "Daily Planner" # 1 mod: Daily Planner
   git clone -q https://github.com/BussinBungus/BungusSDVMods.git "~BussinBungus" # 4 mods: Default Animal Skin Replacer, Heart Event Helper, Holes and Falling, Lost Book Menu
   git clone -q https://github.com/calebstein1/CountingSheep.git "Counting Sheep" # 1 mod: Counting Sheep
   git clone -q https://github.com/calebstein1/StardewVariableSeasons.git "Variable Seasons" # 1 mod: Variable Seasons
   git clone -q https://github.com/camcamcamcam/murderdrone.git "Murder Drone" # 1 mod: Murder Drone
   git clone -q https://github.com/cameronhimself/DeviceSpecificZoom.git "Device Specific Zoom" # 1 mod: Device Specific Zoom
   git clone -q https://github.com/camiska/PregnancyOdds.git "Pregnancy Odds" # 1 mod: Pregnancy Odds
   git clone -q https://github.com/cantorsdust/StardewMods.git "~cantorsdust" # 6 mods: All Crops All Seasons, All Professions, Instant Grow Trees, Recatch Legendary Fish, Skull Cave Saver, TimeSpeed
   git clone -q https://github.com/CaptainSully/StardewMods.git "~CaptainSully" # 3 mods: Better Tappers, Infinite Watering Cans, Start With Greenhouse
   git clone -q https://github.com/captncraig/StardewMods.git "DVR" # 1 mod: DVR
   git clone -q https://github.com/Caraxi/StardewValleyMods.git "~Caraxi" # 3 mods: Auto Load Game, Longer Lasting Lures, Scroll To Blank
   git clone -q https://github.com/casualcozy/SDVMods.git "Location-Specific Experience Gain Mod" # 1 mod: Location-Specific Experience Gain Mod
   git clone -q https://github.com/Caua-Oliveira/StardewMods.git "~Caua-Oliveira" # 4 mods: Automate Tool Swap, Instant Shipping, Machine Input Return, Only When It Counts
   git clone -q https://github.com/cdaragorn/Ui-Info-Suite.git "Ui Info Suite" # 1 mod: Ui Info Suite
   git clone -q https://github.com/certifiableGrimalkin/HCCA.git "Hot Chocolate Coffee Alternative" # 1 mod: Hot Chocolate Coffee Alternative
   git clone -q https://github.com/ceruleandeep/CeruleanStardewMods.git "~ceruleandeep" # 7 mods: Better Junimos Forestry, Junimo Dialog, Market Day, Multiple Spouse Dialogs, No Cooties, Personal Effects Redux, Warp Snitch
   git clone -q https://github.com/chawolbaka/HaltEventTime.git "Halt Event Time" # 1 mod: Halt Event Time
   git clone -q https://github.com/cheesysteak/stardew-steak.git "More Multiplayer Info" # 1 mod: More Multiplayer Info
   git clone -q https://github.com/chencrstu/TeleportNPCLocation.git "Teleport NPC Location" # 1 mod: Teleport NPC Location
   git clone -q https://github.com/chiccendev/StardewValleyMods.git "~chiccendev" # 6 mods: Fruit Tree Tweaks for 1.6, Garbage Can Tweaks, Map Teleport For 1.6 (SVE Compatible), Skateboard For 1.6, Water Your Crops, Wild Tree Tweaks
   git clone -q https://github.com/Chikakoo/stardew-valley-randomizer.git "More Random Edition" # 1 mod: More Random Edition
   git clone -q https://github.com/chippokiddo/sdvmods.git "~chippokiddo" # 1 mod: Custom Winter Star Gifts
   git clone -q https://github.com/ChrisMzz/StardewValleyMods.git "~ChrisMzz" # 3 mods: Legendary Fish Bait, Random Slimes, Slime Minerals
   git clone -q https://github.com/ChristinaEd/ChefHelperAddToFridges.git "Chef Helper - Add to Fridges" # 1 mod: Chef Helper - Add to Fridges
   git clone -q https://github.com/ChroniclerCherry/stardew-valley-mods.git "~ChroniclerCherry" # 17 mods: Catalogues Anywhere, Change Slime Hutch Limit, Custom Crafting Stations, Customize Anywhere, Expanded Preconditions Utility, Farm Rearranger, Hay Bales as Silos, Limited Campfire Cooking, Multi Yield Crops, Multiplayer Mod Checker, Platonic Relationships, Profit Margins, Shop Tile Framework, Snack Anything, Tool Upgrade Costs, Train Station, Upgrade Empty Cabins
   git clone -q https://github.com/chronohart/RentedToolsRefresh.git "Rented Tools Refresh" # 1 mod: Rented Tools Refresh
   git clone -q https://github.com/chsiao58/EvenBetterArtisanGoodIcons.git "Even Better Artisan Good Icons" # 1 mod: Even Better Artisan Good Icons
   git clone -q https://github.com/chudders1231/SDV-FishingProgression.git "Fishing Progression" # 1 mod: Fishing Progression
   git clone -q https://github.com/chyro/SV-InventoryNextPrev.git "Inventory Next-Prev" # 1 mod: Inventory Next-Prev
   git clone -q https://github.com/cilekli-link/SDVMods.git "~cilekli-link" # 2 mods: Dark User Interface, The Chest Dimension
   git clone -q https://github.com/CJBok/SDV-Mods.git "~CJBok" # 4 mods: CJB Automation, CJB Cheats Menu, CJB Item Spawner, CJB Show Item Sell Price
   git clone -q https://github.com/cl4r3/FastForward.git "Fast Forward" # 1 mod: Fast Forward
   git clone -q https://github.com/cl4r3/Halloween-Mod-Jam-2023.git "Tricks and Treats" # 1 mod: Tricks and Treats
   git clone -q https://github.com/clockworkhound/SDV-ChildBedConfig.git "Child Bed Config" # 1 mod: Child Bed Config
   git clone -q https://github.com/cloudyluna/StardewValleyMods.git "~cloudyluna" # 4 mods: Crab Pot Collect Jellies, Drop Seeds After Eating, Maintain Glow Rings Indoors Radius, Selective Eating
   git clone -q https://github.com/Club559/AshleyMod.git "Ashley NPC" # 1 mod: Ashley NPC
   git clone -q https://github.com/clumsyjackdaw/StardewValley_NatureStrikesBack.git "Nature Strikes Back" # 1 mod: Nature Strikes Back
   git clone -q https://github.com/Codeize/MineTooltips.git "Mine Tooltips" # 1 mod: Mine Tooltips
   git clone -q https://github.com/Coldopa/Mini-Bars.git "Mini Bars" # 1 mod: Mini Bars
   git clone -q https://github.com/CompSciLauren/stardew-valley-cozy-clothing-mod.git "Cozy Clothing" # 1 mod: Cozy Clothing
   git clone -q https://github.com/CompSciLauren/stardew-valley-daily-screenshot-mod.git "Daily Screenshot" # 1 mod: Daily Screenshot
   git clone -q https://github.com/concentrate7/Hibernation-Redux.git "Hibernation Redux" # 1 mod: Hibernation Redux
   git clone -q https://github.com/congha22/foodstore.git "Market Town" # 1 mod: Market Town
   git clone -q https://github.com/CosmicKillsSouls/GreenhouseUpgrades.git "Greenhouse Upgrades" # 1 mod: Greenhouse Upgrades
   git clone -q https://github.com/Crimson-is-stupid/Stardew-Valley-Mods.git "Item Every Second" # 1 mod: Item Every Second
   git clone -q https://github.com/Croutonic-Creations/SDV-WastelessWatering.git "Wasteless Watering" # 1 mod: Wasteless Watering
   git clone -q https://github.com/CrunchyDuck/OwO-Stawdew-Vawwey.git "OwO Stawdew Vawwey" # 1 mod: OwO Stawdew Vawwey
   git clone -q https://github.com/CrunchyDuck/Stardew-Skip-Text.git "Skip Text" # 1 mod: Skip Text
   git clone -q https://github.com/Crystallyne/EnergyCount.git "Energy Count" # 1 mod: Energy Count
   git clone -q https://github.com/csandwith/StardewValleyMods.git "~csandwith" # 2 mods: Custom Fixed Dialogue - Continued, Custom Spouse Rooms - Continued
   git clone -q https://github.com/cTooshi/stardew-valley-randomizer.git "Stardew Valley Randomizer" # 1 mod: Stardew Valley Randomizer
   git clone -q https://github.com/CuttingPledge/DialogueConfirmation.git "Dialogue Confirmation" # 1 mod: Dialogue Confirmation
   git clone -q https://github.com/CyanFireUK/StardewValleyMods.git "~CyanFireUK" # 3 mods: Change Horse Sounds, Multiple Horses, Permanent Cellar
   git clone -q https://github.com/Cyrillya/InputMethodFix.git "Input Method Fix" # 1 mod: Input Method Fix
   git clone -q https://github.com/dailyxcz/StardewValleyMods.git "Voice Chat" # 1 mod: Voice Chat
   git clone -q https://github.com/daleao/bagi-meads.git "Better Artisan Good Icons For Meads" # 1 mod: Better Artisan Good Icons For Meads
   git clone -q https://github.com/daleao/sdv.git "~daleao" # 8 mods: Aquarism, Chargeable Resource Tools, DaLionheart, Gemstone Harmonics, Serfdom, Springmyst, Walk Of Life - Rebirth, Wildcat
   git clone -q https://github.com/dandm1/ValleyTalk.git "Valley Talk" # 1 mod: Valley Talk
   git clone -q https://github.com/dantheman999301/StardewMods.git "Variable Grass" # 1 mod: Variable Grass
   git clone -q https://github.com/danvolchek/StardewMods.git "~danvolchek" # 30 mods: Auto Stacker, Better Artisan Good Icons, Better Doors, Better Fruit Trees, Better Garden Pots, Better Hay, Better Slingshots, Better Workbenches, Cacti Hurt You, Casks Everywhere, Chat Commands, Copy Invite Code, Custom Transparency, Custom Warp Locations, Customizable Death Penalty, Desert Obelisk, Flowery Tools, Geode Info Menu, Giant Crop Ring, Hold to Break Geodes, Mod Update Menu, No Crows, No Fence Decay, Pong, Range Display, Removable Horse Hats, Safe Lightning, Stack Everything, Wind Effects, Winter Grass
   git clone -q https://github.com/davicr/StardewHypnos.git "Stardew Hypnos - Sleeping Overhaul" # 1 mod: Stardew Hypnos - Sleeping Overhaul
   git clone -q https://github.com/Dawilly/SAAT.git "SAAT" # 2 mods: SAAT.API, SAAT.Mod
   git clone -q https://github.com/DeadRobotDev/StardewMods.git "~DeadRobotDev" # 2 mods: Jack Be Thicc, Skip Start
   git clone -q https://github.com/deathbygin/Ferngill-Arcadium.git "Ferngill Arcadium" # 1 mod: Ferngill Arcadium
   git clone -q https://github.com/DeathGameDev/SDV-FastTravel.git "Fast Travel" # 1 mod: Fast Travel
   git clone -q https://github.com/DefenTheNation/StardewMod-BackpackResizer.git "Backpack Resizer" # 1 mod: Backpack Resizer
   git clone -q https://github.com/defenthenation/StardewMod-CustomSlayerChallenges.git "Custom Adventure Guild Challenges" # 1 mod: Custom Adventure Guild Challenges
   git clone -q https://github.com/DefenTheNation/StardewMod-TimeMultiplier.git "Time Multiplier" # 1 mod: Time Multiplier
   git clone -q https://github.com/Deflaktor/KeySuppressor.git "Key Suppressor" # 1 mod: Key Suppressor
   git clone -q https://github.com/deLaDude/ChefsCloset.git "Chefs Closet" # 1 mod: Chefs Closet
   git clone -q https://github.com/DelphinWave/BabyPets.git "Baby Pets" # 1 mod: Baby Pets
   git clone -q https://github.com/DelphinWave/CatsAndDogsMod.git "Cats and Dogs" # 1 mod: Cats and Dogs
   git clone -q https://github.com/DelphinWave/MultiplayerHorseReskin.git "Multiplayer Horse Reskin" # 1 mod: Multiplayer Horse Reskin
   git clone -q https://github.com/demiacle/QualityOfLifeMods.git "Improved Quality of Life" # 1 mod: Improved Quality of Life
   git clone -q https://github.com/demiacle/UiModSuite.git "UI Mod Suite" # 1 mod: UI Mod Suite
   git clone -q https://github.com/Denifia/StardewMods.git "~Denifia" # 2 mods: Buy Cooking Recipes, Send Items to Other Farmers
   git clone -q https://github.com/DenisSilent/Eleanor.git "Eleanor" # 1 mod: Eleanor
   git clone -q https://github.com/derslayr10/GreenhouseConstruction.git "Greenhouse Construction" # 1 mod: Greenhouse Construction
   git clone -q https://github.com/DespairScent-SDV/AlwaysSpecialTitle.git "Always Special Title" # 1 mod: Always Special Title
   git clone -q https://github.com/DespairScent-SDV/Autonomals.git "Autonomals" # 1 mod: Autonomals
   git clone -q https://github.com/DespairScent-SDV/PrecisionWheel.git "PrecisionWheel" # 1 mod: PrecisionWheel
   git clone -q https://github.com/DespairScent-SDV/UnlockedBundles.git "Unlocked Bundles" # 1 mod: Unlocked Bundles
   git clone -q https://github.com/desto-git/sdv-mods.git "~desto-git" # 3 mods: Billboard Profit Margin, Gift Decline, Regular Quality
   git clone -q https://github.com/DevWithMaj/Stardew-CHAOS.git "CHAOS mod for Stardew Valley" # 1 mod: CHAOS mod for Stardew Valley
   git clone -q https://github.com/DhiaReza/Progressive_Tax.git "Progressive Tax" # 1 mod: Progressive Tax
   git clone -q https://github.com/Digus/StardewValleyMods.git "~Digus" # 15 mods: Advanced Weather Forecast Mod, Animal Husbandry Mod, Borderless Wood Floor, Crop Transplant, Custom Cask Mod, Custom Crystalarium Mod, Custom Crystalarium Mod → CCRMAutomate, Custom Kissing, Everlasting Baits and Unbreakable Tackles, Mail Framework, Mail Services Mod, PPJA Producer Converter, Producer Framework Mod, Producer Framework Mod → PFMAutomate, Water Retaining Field
   git clone -q https://github.com/DiscipleOfEris/HardyGrass.git "Hardy Grass" # 1 mod: Hardy Grass
   git clone -q https://github.com/DJ-STLN/CombatControls.git "Combat Controls" # 1 mod: Combat Controls
   git clone -q https://github.com/dlrdlrdlr/DialogToClipboard.git "Dialog to Clipboard" # 1 mod: Dialog to Clipboard
   git clone -q https://github.com/dmarcoux/DisplayEnergy.git "Display Energy" # 1 mod: Display Energy
   git clone -q https://github.com/dmcrider/LastDayToPlant.git "Last Day to Plant" # 1 mod: Last Day to Plant
   git clone -q https://github.com/domsim1/stardew-valley-deluxe-hats-mod.git "Deluxe Hats" # 1 mod: Deluxe Hats
   git clone -q https://github.com/doncollins/StardewValleyMods.git "~doncollins" # 2 mods: Categorize Chests, Love Bubbles
   git clone -q https://github.com/doodlum/nexusmods-widgets.git "Certain NPCs Snore" # 1 mod: Certain NPCs Snore
   git clone -q https://github.com/DotSharkTeeth/StardewValleyMods.git "~DotSharkTeeth" # 3 mods: Faster Transition, No Hat Treasure in Skull Cave, Right Click to Untill Soil
   git clone -q https://github.com/dphile/SDVBetterIridiumTools.git "Better Iridium Tool Area" # 1 mod: Better Iridium Tool Area
   git clone -q https://github.com/Drachenkaetzchen/AdvancedKeyBindings.git "Advanced Key Bindings" # 1 mod: Advanced Key Bindings
   git clone -q https://github.com/DraconisLeonidas/PressToQuack.git "Press to Quack" # 1 mod: Press to Quack
   git clone -q https://github.com/drbirbdev/StardewValley.git "~drbirbdev" # 12 mods: Better Festival Notifications, Binning Skill, Birb Core, Gameboy Arcade, Junimo Kart Global Rankings, Leaderboard Library, Look to the Sky, Panning Upgrades, Ranching Tool Upgrades, Realtime Framework, Socializing Skill, Winter Star Spouse
   git clone -q https://github.com/drewhoener/FroggyFilter.git "Froggy Filter" # 1 mod: Froggy Filter
   git clone -q https://github.com/DryIcedTea/Buttplug-Valley.git "Buttplug Valley" # 1 mod: Buttplug Valley
   git clone -q https://github.com/Drynwynn/StardewValleyMods.git "~Drynwynn" # 2 mods: Fishing Automaton, No More Random Mine Flyers
   git clone -q https://github.com/dtomlinson-ga/EarlyCommunityUpgrades.git "Early Community Upgrades" # 1 mod: Early Community Upgrades
   git clone -q https://github.com/dtomlinson-ga/StatsAsTokens.git "Stats as Tokens" # 1 mod: Stats as Tokens
   git clone -q https://github.com/Dunc4nNT/StardewMods.git "~Dunc4nNT" # 2 mods: Self Serve for 1.6, Yet Another Fishing Mod
   git clone -q https://github.com/Dwayneten/AutoFarmScreenshot.git "Auto Farm Screenshot" # 1 mod: Auto Farm Screenshot
   git clone -q https://github.com/eduanttunes/sv_fighting_with_npcs.git "Fighting With NPCs" # 1 mod: Fighting With NPCs
   git clone -q https://github.com/eideehi/sdv-easyfishing.git "Eidee Easy Fishing" # 1 mod: Eidee Easy Fishing
   git clone -q https://github.com/EKomperud/StardewMods.git "~EKomperud" # 3 mods: Companion NPCs, Iridium Tools Patch, Multiple Gifts for Spouses
   git clone -q https://github.com/eleanoot/stardew-to-do-mod.git "To Do List" # 1 mod: To Do List
   git clone -q https://github.com/elfuun1/FlowerDanceFix.git "Flower Dance Fix" # 1 mod: Flower Dance Fix
   git clone -q https://github.com/elizabethcd/CustomizeWeddingAttire.git "Customize Wedding Attire" # 1 mod: Customize Wedding Attire
   git clone -q https://github.com/elizabethcd/DragonPearlLure.git "Dragon Pearl Lure" # 1 mod: Dragon Pearl Lure
   git clone -q https://github.com/elizabethcd/FireworksFestival.git "Fireworks Festival" # 1 mod: Fireworks Festival
   git clone -q https://github.com/elizabethcd/MigrateDGAItems.git "Migrate DGA Items" # 1 mod: Migrate DGA Items
   git clone -q https://github.com/elizabethcd/MoreConversationTopics.git "More Conversation Topics" # 1 mod: More Conversation Topics
   git clone -q https://github.com/elizabethcd/PreventFurniturePickup.git "Glue Your Furniture Down" # 1 mod: Glue Your Furniture Down
   git clone -q https://github.com/elizabethcd/SeaMonsterAlert.git "Crocus Crew Sea Monster Alert" # 1 mod: Crocus Crew Sea Monster Alert
   git clone -q https://github.com/embolden/Heartbeat.git "Heartbeat" # 1 mod: Heartbeat
   git clone -q https://github.com/emperorzuul/SMAPI_MODS.git "Idle Timer" # 1 mod: Idle Timer
   git clone -q https://github.com/emuengine/StardewValleyMayorMod.git "Mayor Mod" # 1 mod: Mayor Mod
   git clone -q https://github.com/emurphy42/AllYourPowersRevealed.git "All Your Powers Revealed" # 1 mod: All Your Powers Revealed
   git clone -q https://github.com/emurphy42/AnimalNametags.git "Animal Nametags" # 1 mod: Animal Nametags
   git clone -q https://github.com/emurphy42/AutoGrabberVacuum.git "Auto-Grabber Vacuum" # 1 mod: Auto-Grabber Vacuum
   git clone -q https://github.com/emurphy42/CalendarBuffs.git "Calendar Buffs" # 1 mod: Calendar Buffs
   git clone -q https://github.com/emurphy42/ChickenStatueDoesSomething.git "Chicken Statue Does Something" # 1 mod: Chicken Statue Does Something
   git clone -q https://github.com/emurphy42/ConfirmMineLadder.git "Confirm Mine Ladder" # 1 mod: Confirm Mine Ladder
   git clone -q https://github.com/emurphy42/ConfirmSleep.git "Confirm Sleep" # 1 mod: Confirm Sleep
   git clone -q https://github.com/emurphy42/ConfirmTotem.git "Confirm Totem" # 1 mod: Confirm Totem
   git clone -q https://github.com/emurphy42/CraftableDiagonalFloors.git "Craftable Diagonal Floors" # 1 mod: Craftable Diagonal Floors
   git clone -q https://github.com/emurphy42/FairiesIsCrows.git "Fairies Is Crows" # 1 mod: Fairies Is Crows
   git clone -q https://github.com/emurphy42/GrapeDay.git "Grape Day" # 1 mod: Grape Day
   git clone -q https://github.com/emurphy42/GuntherTellsYou.git "Gunther Tells You" # 1 mod: Gunther Tells You
   git clone -q https://github.com/emurphy42/ICaughtThis.git "I Caught This" # 1 mod: I Caught This
   git clone -q https://github.com/emurphy42/NowPlaying.git "Now Playing" # 1 mod: Now Playing
   git clone -q https://github.com/emurphy42/NPCNametags.git "NPC Nametags" # 1 mod: NPC Nametags
   git clone -q https://github.com/emurphy42/PendingSecretNotes.git "Pending Secret Notes" # 1 mod: Pending Secret Notes
   git clone -q https://github.com/emurphy42/PredictiveMods.git "Public Access TV (1.6)" # 1 mod: Public Access TV (1.6)
   git clone -q https://github.com/emurphy42/RedGreenRecolor.git "Red Green Recolor" # 1 mod: Red Green Recolor
   git clone -q https://github.com/emurphy42/snow-crab.git "Snow Crab" # 1 mod: Snow Crab
   git clone -q https://github.com/emurphy42/SpeedBobber.git "Speed Bobber" # 1 mod: Speed Bobber
   git clone -q https://github.com/emurphy42/SwissArmyKnife.git "Swiss Army Knife" # 1 mod: Swiss Army Knife
   git clone -q https://github.com/emurphy42/trash-bear-abides.git "Trash Bear Abides" # 1 mod: Trash Bear Abides
   git clone -q https://github.com/emurphy42/WhatDoesTrashBearWant.git "What Does Trash Bear Want" # 1 mod: What Does Trash Bear Want
   git clone -q https://github.com/emurphy42/WhatDoYouWant.git "What Do You Want" # 1 mod: What Do You Want
   git clone -q https://github.com/Enaium-StardewValleyMods/EnaiumToolKit.git "Enaium ToolKit" # 1 mod: Enaium ToolKit
   git clone -q https://github.com/Enaium-StardewValleyMods/KeyBindUI.git "KeyBind UI" # 1 mod: KeyBind UI
   git clone -q https://github.com/Enaium-StardewValleyMods/Labeling.git "Labeling" # 1 mod: Labeling
   git clone -q https://github.com/Enaium-StardewValleyMods/ModMenu.git "Mod Menu" # 1 mod: Mod Menu
   git clone -q https://github.com/Enaium-StardewValleyMods/NameTags.git "Name Tags" # 1 mod: Name Tags
   git clone -q https://github.com/Enaium-StardewValleyMods/QuickMinigame.git "Quick Minigame" # 1 mod: Quick Minigame
   git clone -q https://github.com/Enaium-StardewValleyMods/QuickShop.git "Quick Shop" # 1 mod: Quick Shop
   git clone -q https://github.com/Enaium-StardewValleyMods/SimpleHUD.git "Simple HUD" # 1 mod: Simple HUD
   git clone -q https://github.com/Enaium-StardewValleyMods/TeleportNpc.git "Teleport NPC" # 1 mod: Teleport NPC
   git clone -q https://github.com/Enaium-StardewValleyMods/TeleportPoint.git "Teleport Point" # 1 mod: Teleport Point
   git clone -q https://github.com/EnderTedi/ConfusedTortilla.git "Confused Tortillas" # 1 mod: Confused Tortillas
   git clone -q https://github.com/EnderTedi/DefaultOnCheats.git "Default On Cheats" # 1 mod: Default On Cheats
   git clone -q https://github.com/EnderTedi/No-Fence-Decay-Redux.git "No Fence Decay Redux" # 1 mod: No Fence Decay Redux
   git clone -q https://github.com/EnderTedi/Tree-Size-Framework.git "Tree Size Framework" # 1 mod: Tree Size Framework
   git clone -q https://github.com/EnderTedi/Untimed-Special-Orders.git "Untimed Special Orders" # 1 mod: Untimed Special Orders
   git clone -q https://github.com/Enteligenz/StardewMods.git "~Enteligenz" # 2 mods: Food Cravings, Twitch Chat Integration
   git clone -q https://github.com/Entoarox/StardewMods.git "~Entoarox" # 13 mods: Advanced Location Loader, Custom Paths, Entoarox Framework, Entoarox Modding Utilities, Extended Minecart, Faster Paths, Furniture Anywhere, Magic Junimo Pet, More Animals, Seasonal Immersion, Shop Expander, Sit For Stamina, XNB Loader
   git clone -q https://github.com/Esca-MMC/BuildOnAnyTile.git "Build On Any Tile" # 1 mod: Build On Any Tile
   git clone -q https://github.com/Esca-MMC/CustomNPCExclusions.git "Custom NPC Exclusions" # 1 mod: Custom NPC Exclusions
   git clone -q https://github.com/Esca-MMC/CustomTracker.git "Custom Tracker" # 1 mod: Custom Tracker
   git clone -q https://github.com/Esca-MMC/DestroyableBushes.git "Destroyable Bushes" # 1 mod: Destroyable Bushes
   git clone -q https://github.com/Esca-MMC/EscasModdingPlugins.git "Esca's Modding Plugins" # 1 mod: Esca's Modding Plugins
   git clone -q https://github.com/Esca-MMC/FarmTypeManager.git "Farm Type Manager" # 1 mod: Farm Type Manager
   git clone -q https://github.com/Esca-MMC/PollenSprites.git "Pollen Sprites" # 1 mod: Pollen Sprites
   git clone -q https://github.com/Esca-MMC/TransparencySettings.git "Transparency Settings" # 1 mod: Transparency Settings
   git clone -q https://github.com/Esca-MMC/WaterproofItems.git "Waterproof Items" # 1 mod: Waterproof Items
   git clone -q https://github.com/Esper89/StardewValley-FruitTreeSeasons.git "Fruit Tree Seasons" # 1 mod: Fruit Tree Seasons
   git clone -q https://github.com/Eureka-dot-net/NPCInfo.git "NPC Info" # 1 mod: NPC Info
   git clone -q https://github.com/evfredericksen/StardewSpeak.git "Stardew Speak" # 1 mod: Stardew Speak
   git clone -q https://github.com/Exblosis/StardewValleyMods.git "Let's Move It" # 1 mod: Let's Move It
   git clone -q https://github.com/explosivetortellini/bluechickensaregreen.git "Blue Chickens Are Green" # 1 mod: Blue Chickens Are Green
   git clone -q https://github.com/explosivetortellini/StardewValleyDRP.git "DewCord Rich Presence" # 1 mod: DewCord Rich Presence
   git clone -q https://github.com/F1r3w477/level-extender.git "Level Extender" # 1 mod: Level Extender
   git clone -q https://github.com/F1r3w477/TheseModsAintLoyal.git "Moody Planet" # 1 mod: Moody Planet
   git clone -q https://github.com/f4iTh/StardewValleyModding.git "~f4iTh" # 7 mods: Activate Sprinklers, Adjust Baby Chance, Breed Like Rabbits 2: Procreation Boogaloo, Custom Warps, Parsnips Absolutely Everywhere But It's Garlic, Show Catch Quality, Where's My Items?
   git clone -q https://github.com/facufierro/RuneMagic.git "Rune Magic" # 1 mod: Rune Magic
   git clone -q https://github.com/FairfieldBW/CropCheck.git "Crop Check" # 1 mod: Crop Check
   git clone -q https://github.com/FairfieldBW/MachineCheck.git "Machine Check" # 1 mod: Machine Check
   git clone -q https://github.com/FawazTakahji/CloudSync.git "~FawazTakahji" # 3 mods: CloudSync, CloudSync → CloudSync Dropbox, CloudSync → Google Drive
   git clone -q https://github.com/FawazTakahji/GlobalConfigSettingsRewrite.git "Global Config Settings Rewrite" # 1 mod: Global Config Settings Rewrite
   git clone -q https://github.com/FawazTakahji/StardewTileCounter.git "Tile Counter" # 1 mod: Tile Counter
   git clone -q https://github.com/Felix-Dev/StardewMods.git "~Felix-Dev" # 3 mods: Archaeology House Content Management Helper, FeTK - Stardew-Valley Toolkit, Tool Upgrade Delivery Service
   git clone -q https://github.com/ferdaber/sdv-mods.git "Deluxe Grabber Redux" # 1 mod: Deluxe Grabber Redux
   git clone -q https://github.com/FerMod/StardewMods.git "Multiplayer Emotes" # 1 mod: Multiplayer Emotes
   git clone -q https://github.com/FlameHorizon/ProductionStats.git "Production Stats" # 1 mod: Production Stats
   git clone -q https://github.com/FlashShifter/StardewValleyExpanded.git "Stardew Valley Expanded" # 1 mod: Stardew Valley Expanded
   git clone -q https://github.com/FleemUmbleem/DisplayPlayerStats.git "Display Player Stats" # 1 mod: Display Player Stats
   git clone -q https://github.com/Floogen/AlternativeTextures.git "Alternative Textures" # 1 mod: Alternative Textures
   git clone -q https://github.com/Floogen/Archery.git "Archery" # 2 mods: Archery, Archery - Starter Pack
   git clone -q https://github.com/Floogen/CombatDummy.git "Combat Dummies" # 1 mod: Combat Dummies
   git clone -q https://github.com/Floogen/CosmeticRings.git "Cosmetic Rings" # 1 mod: Cosmetic Rings
   git clone -q https://github.com/Floogen/CustomCompanions.git "Custom Companions" # 1 mod: Custom Companions
   git clone -q https://github.com/Floogen/DynamicReflections.git "Dynamic Reflections" # 1 mod: Dynamic Reflections
   git clone -q https://github.com/Floogen/FashionSense.git "Fashion Sense" # 1 mod: Fashion Sense
   git clone -q https://github.com/Floogen/FishingTrawler.git "FishingTrawler" # 2 mods: Fishing Trawler, Fishing Trawler - New Horizons
   git clone -q https://github.com/Floogen/GreenhouseGatherers.git "GreenhouseGatherers" # 2 mods: Greenhouse Gatherers, Greenhouse Gatherers Automate
   git clone -q https://github.com/Floogen/HandyHeadphones.git "Handy Headphones" # 1 mod: Handy Headphones
   git clone -q https://github.com/Floogen/IslandGatherers.git "IslandGatherers" # 2 mods: Island Gatherers, Island Gatherers Automate
   git clone -q https://github.com/Floogen/JojaOnline.git "Joja Online" # 1 mod: Joja Online
   git clone -q https://github.com/Floogen/MultipleMiniObelisks.git "Multiple Mini-Obelisks" # 1 mod: Multiple Mini-Obelisks
   git clone -q https://github.com/Floogen/MysticalBuildings.git "~Floogen" # 2 mods: Cave of Memories, Mystical Buildings
   git clone -q https://github.com/Floogen/SereneGreenhouse.git "Serene Greenhouse" # 1 mod: Serene Greenhouse
   git clone -q https://github.com/Floogen/SolidFoundations.git "SolidFoundations" # 2 mods: Solid Foundations, Solid Foundations → Automate Integration
   git clone -q https://github.com/Floogen/StardewSandbox.git "Hat Shop Restoration" # 1 mod: Hat Shop Restoration
   git clone -q https://github.com/FlyingTNT/StardewValleyMods.git "~FlyingTNT" # 13 mods: Better Elevator - Continued, Catalogue Filter - Continued, Custom Gift Limits - Continued, Longer Seasons - Continued, Multiple Floor Farmhouse - Continued, Pacifist Valley - Continued, Painting Display - Continued, Quest Time Limits - Continued, Resource Storage - Continued, Seed Info - Continued, Social Page Order Redux, Swim - Continued, Wall Televisions - Continued
   git clone -q https://github.com/focustense/StardewAutoTrash.git "Garbage In Garbage Can" # 1 mod: Garbage In Garbage Can
   git clone -q https://github.com/focustense/StardewBulkBuy.git "Bulk Buy" # 1 mod: Bulk Buy
   git clone -q https://github.com/focustense/StardewControllers.git "Star Control" # 1 mod: Star Control
   git clone -q https://github.com/focustense/StardewFishingSea.git "A Fishing Sea" # 1 mod: A Fishing Sea
   git clone -q https://github.com/focustense/StardewForgeMenuChoice.git "Pick Your Enchantment Restored" # 1 mod: Pick Your Enchantment Restored
   git clone -q https://github.com/focustense/StardewPenPals.git "Pen Pals" # 1 mod: Pen Pals
   git clone -q https://github.com/focustense/StardewUI.git "Stardew UI" # 1 mod: Stardew UI
   git clone -q https://github.com/foxwhite25/Stardew-Ultimate-Fertilizer.git "Ultimate Fertilizer" # 1 mod: Ultimate Fertilizer
   git clone -q https://github.com/Freaksaus/DefaultToolSlots.git "Default Tool Slots" # 1 mod: Default Tool Slots
   git clone -q https://github.com/Freaksaus/PreventEnergyLoss.git "Prevent Energy Loss" # 1 mod: Prevent Energy Loss
   git clone -q https://github.com/Freaksaus/RecipesInMail.git "Recipes In Mail" # 1 mod: Recipes In Mail
   git clone -q https://github.com/Freaksaus/Tileman-Redux.git "Tileman Redux" # 1 mod: Tileman Redux
   git clone -q https://github.com/FricativeMelon/GroupableChests.git "Groupable Chests" # 1 mod: Groupable Chests
   git clone -q https://github.com/FricativeMelon/SkipDialogue.git "Skip Dialogue" # 1 mod: Skip Dialogue
   git clone -q https://github.com/funny-snek/Always-On-Server-for-Multiplayer.git "Always On Server for Multiplayer" # 1 mod: Always On Server for Multiplayer
   git clone -q https://github.com/funny-snek/anticheat-and-servercode.git "Anti-Cheat Server" # 1 mod: Anti-Cheat Server
   git clone -q https://github.com/futroo/Stardew-Valley-Mods.git "Shipping Bin Summary" # 1 mod: Shipping Bin Summary
   git clone -q https://github.com/Gaiadin/Stardew-Screenshot-Everywhere.git "Screenshot Everywhere" # 1 mod: Screenshot Everywhere
   git clone -q https://github.com/Gaphodil/BetterJukebox.git "Better Jukebox" # 1 mod: Better Jukebox
   git clone -q https://github.com/Gaphodil/GlobalConfigSettings.git "Global Config Settings" # 1 mod: Global Config Settings
   git clone -q https://github.com/Gathouria/Adopt-Skin.git "Adopt 'n Skin" # 1 mod: Adopt 'n Skin
   git clone -q https://github.com/gellingly/stardewmods.git "~gellingly" # 6 mods: Adjust Bomb Speed, Crop Encyclopedia, Keep Spawning Ladders, Less Secret Logo Screen Secret, Stack More Things, Stackable Monster Musk Effects
   git clone -q https://github.com/gembree/HelpWantedQuestFixes.git "Help Wanted Quest Fixes" # 1 mod: Help Wanted Quest Fixes
   git clone -q https://github.com/GenDeathrow/SDV_BlessingsAndCurses.git "Blessings and Curses" # 1 mod: Blessings and Curses
   git clone -q https://github.com/GeorgeTR1/BetterMovement.git "Better Movement" # 1 mod: Better Movement
   git clone -q https://github.com/GhostGoesBinted/KeyboardOnlyMod.git "Keyboard Only" # 1 mod: Keyboard Only
   git clone -q https://github.com/GhostUnderBlanket/Stardew-SMAPI-Mods.git "Fishing Assistant 2" # 1 mod: Fishing Assistant 2
   git clone -q https://github.com/GilarF/SVM.git "Mod Settings Tab" # 1 mod: Mod Settings Tab
   git clone -q https://github.com/gingajamie/smapi-better-sprinklers-plus-encore.git "Better Sprinklers Plus (1.6)" # 1 mod: Better Sprinklers Plus (1.6)
   git clone -q https://github.com/GlimmerDev/StardewValleyMod_SleepWarning.git "Sleep Warning" # 1 mod: Sleep Warning
   git clone -q https://github.com/GlitchedDeveloper/StardewValley-TerrariaBosses.git "Terraria Bosses" # 1 mod: Terraria Bosses
   git clone -q https://github.com/Goldenrevolver/Stardew-Valley-Mods.git "~Goldenrevolver" # 16 mods: A Tapper's Dream, Animals Die, Crab Pot Loot Has Quality and Bait Effects, Drop It - Drop Item Hotkey, Enchantable Scythes and Golden Scythe Respawns, Enchanted Adventurer's Guild Rewards, Enchanted Cowboy Boots, Forage Fantasy, Harvest Moon FoMT-like Watering Can and Hoe Area, Horse Overhaul, Just Relax, Maritime Secrets, Mushroom Rancher, Permanent Cookout Kit And Better Charcoal Kiln, Ring Overhaul, Watering Grants Experience and Crops Can Wither
   git clone -q https://github.com/Gorzalt/StardewValleyMods.git "Kawakami - Healer NPC" # 1 mod: Kawakami - Healer NPC
   git clone -q https://github.com/gottyduke/stardew-informant.git "Informant - The Tooltip Labels" # 1 mod: Informant - The Tooltip Labels
   git clone -q https://github.com/gr3ger/Stardew_JJB.git "Jiggly Junimo Bundles" # 1 mod: Jiggly Junimo Bundles
   git clone -q https://github.com/Graham-Nelson73/LoonCallsMod.git "Loon Calls" # 1 mod: Loon Calls
   git clone -q https://github.com/Greem3/LVC-StardewValleyMod.git "Location Voice Chat" # 1 mod: Location Voice Chat
   git clone -q https://github.com/GreenUserBlue/AnimationCancelKey.git "Animation-Cancel Keybinding" # 1 mod: Animation-Cancel Keybinding
   git clone -q https://github.com/gregoirechauvet/StardewValleyMods.git "~gregoirechauvet" # 2 mods: 24 Hour Format, Multiplayer Shaft Fix
   git clone -q https://github.com/greyivy/OrnithologistsGuild.git "Ornithologist's Guild" # 1 mod: Ornithologist's Guild
   git clone -q https://github.com/GrumpyCrouton/SDV-AccessibleTiles.git "Accessible Tiles" # 1 mod: Accessible Tiles
   git clone -q https://github.com/GStefanowich/SDV-Forecaster.git "Forecaster Text" # 1 mod: Forecaster Text
   git clone -q https://github.com/GStefanowich/SDV-NFFTT.git "Not Far From The Tree" # 1 mod: Not Far From The Tree
   git clone -q https://github.com/GuiNoya/SVMods.git "~GuiNoya" # 3 mods: Daily Tasks Report, Self Service Shops, Zoom Level Keybinding
   git clone -q https://github.com/Guniism/GuniismUnicycle.git "Guniism's Unicycle" # 1 mod: Guniism's Unicycle
   git clone -q https://github.com/gunnargolf/DynamicChecklist.git "Dynamic Checklist" # 1 mod: Dynamic Checklist
   git clone -q https://github.com/gzhynko/stardew-mods.git "Crop Growth Adjustments" # 1 mod: Crop Growth Adjustments
   git clone -q https://github.com/gzhynko/StardewMods.git "~gzhynko" # 6 mods: Animals Need Water, Dialogue Box Redesign, Event Black Bars, Fish Exclusions, Incubate Pufferfish Eggs, Scythe Harvests More
   git clone -q https://github.com/ha1fdaew/iTile.git "iTile" # 1 mod: iTile
   git clone -q https://github.com/ha1fdaew/PlayerIncomeStats.git "Player Income Stats" # 1 mod: Player Income Stats
   git clone -q https://github.com/Hackswell/GloryOfEfficiency.git "Glory Of Efficiency" # 1 mod: Glory Of Efficiency
   git clone -q https://github.com/Hakej/Animal-Pet-Status.git "Animal Pet Status" # 1 mod: Animal Pet Status
   git clone -q https://github.com/hankan1918/QuickEat1.git "Quick Eat" # 1 mod: Quick Eat
   git clone -q https://github.com/Haphestia/SDV-Lockpicks.git "Lockpicks" # 1 mod: Lockpicks
   git clone -q https://github.com/Haphestia/SDVModding.git "Minecart Patcher" # 1 mod: Minecart Patcher
   git clone -q https://github.com/HarrisonGrey/StardewCapybara.git "Capybara Friendship" # 1 mod: Capybara Friendship
   git clone -q https://github.com/harshlele/CutTheCord.git "Cut the Cord" # 1 mod: Cut the Cord
   git clone -q https://github.com/HaulinOats/StardewMods.git "~HaulinOats" # 2 mods: Dynamic Crops, Water Balloon
   git clone -q https://github.com/HauntedPineapple/Stendew-Valley.git "Stendew Valley" # 1 mod: Stendew Valley
   git clone -q https://github.com/hawkfalcon/Stardew-Mods.git "~hawkfalcon" # 3 mods: Better Junimos, Custom Quest Expiration, Tillable Ground
   git clone -q https://github.com/hcoona/StardewValleyMods.git "~hcoona" # 2 mods: Fishing Adjust, Move Faster
   git clone -q https://github.com/HeartoLazor/StardewValleyBobbingDisabler.git "Bobbing Disabler" # 1 mod: Bobbing Disabler
   git clone -q https://github.com/Hedgehog-Technologies/StardewMods.git "~Hedgehog-Technologies" # 4 mods: Allow Beach Sprinklers, Auto Trasher, AutoShaker, Full Fishing Bar
   git clone -q https://github.com/HelriaVellon/ScriptCommandPlus.git "~HelriaVellon" # 2 mods: Script Command Plus, Script Command Plus → Polyamory Compatible Version
   git clone -q https://github.com/HeroOfTheWinds/BlueChickensInMultiplayer.git "Blue Chickens in Multiplayer" # 1 mod: Blue Chickens in Multiplayer
   git clone -q https://github.com/HeyImAmethyst/Ars-Venefici.git "Ars Venefici" # 1 mod: Ars Venefici
   git clone -q https://github.com/HeyImAmethyst/SkinToneLoader.git "Skin Tone Loader" # 1 mod: Skin Tone Loader
   git clone -q https://github.com/heyseth/SDVMods.git "~heyseth" # 2 mods: Pause While Sitting, Remember Your Umbrella
   git clone -q https://github.com/Hikari-BS/StardewMods.git "~Hikari-BS" # 2 mods: Empty Your Hands, Fixed Weapons Damage
   git clone -q https://github.com/holy-the-sea/LightSwitch.git "Light Switch" # 1 mod: Light Switch
   git clone -q https://github.com/holy-the-sea/SortShippingCollection.git "Shipping Collection Sorted By Mods" # 1 mod: Shipping Collection Sorted By Mods
   git clone -q https://github.com/holybananapants/UltimateStorageSystem.git "Ultimate Storage System" # 1 mod: Ultimate Storage System
   git clone -q https://github.com/hootless/StardewMods.git "~hootless" # 2 mods: Bus Locations, Line Sprinklers
   git clone -q https://github.com/HowXu/StardewValley_DragonSword.git "Dragon Sword" # 1 mod: Dragon Sword
   git clone -q https://github.com/huancz/SDV-TidyOrchard.git "Tidy Orchard - Move Fruit Trees" # 1 mod: Tidy Orchard - Move Fruit Trees
   git clone -q https://github.com/Hunter-Chambers/StardewValleyMods.git "~Hunter-Chambers" # 3 mods: Auto Grab Truffles, Deluxe Auto-Petter, Easy Fishin'
   git clone -q https://github.com/Husky110/BatteryWarningMod.git "Battery Warning Mod" # 1 mod: Battery Warning Mod
   git clone -q https://github.com/Husky110/ChillInYourFarmHouse.git "Chill in Your Farmhouse" # 1 mod: Chill in Your Farmhouse
   git clone -q https://github.com/i-saac-b/PostBoxMod.git "Postbox Building" # 1 mod: Postbox Building
   git clone -q https://github.com/icepuente/StardewValleyMods.git "Horse Whistle" # 1 mod: Horse Whistle
   git clone -q https://github.com/ichortower/FontSmasher.git "Font Smasher" # 1 mod: Font Smasher
   git clone -q https://github.com/ichortower/GrandpasStardrop.git "Grandpa's Stardrop" # 1 mod: Grandpa's Stardrop
   git clone -q https://github.com/ichortower/HatMouseLacey.git "Hat Mouse Lacey" # 1 mod: Hat Mouse Lacey
   git clone -q https://github.com/ichortower/Nightshade.git "Nightshade" # 1 mod: Nightshade
   git clone -q https://github.com/ichortower/NPCGeometry.git "NPC Geometry" # 1 mod: NPC Geometry
   git clone -q https://github.com/ichortower/PortableHole.git "Portable Hole" # 1 mod: Portable Hole
   git clone -q https://github.com/ichortower/PositionalAudio.git "Positional Audio" # 1 mod: Positional Audio
   git clone -q https://github.com/ichortower/SecretNoteFramework.git "Secret Note Framework" # 1 mod: Secret Note Framework
   git clone -q https://github.com/ichortower/SecretWoodsSnorlax.git "Secret Woods Snorlax" # 1 mod: Secret Woods Snorlax
   git clone -q https://github.com/ichortower/TaterToss.git "Tater Toss - Throw Your Toddlers" # 1 mod: Tater Toss - Throw Your Toddlers
   git clone -q https://github.com/ichortower/TheFishDimension.git "The Fish Dimension" # 1 mod: The Fish Dimension
   git clone -q https://github.com/ichortower/TheJClub.git "The J Club" # 1 mod: The J Club
   git clone -q https://github.com/idermailer/DresserSorter.git "Dresser Sorter" # 1 mod: Dresser Sorter
   git clone -q https://github.com/idermailer/iDontUseThatWater.git "I Don't Use That Water" # 1 mod: I Don't Use That Water
   git clone -q https://github.com/idermailer/InstantDialogues.git "Instant Dialogues" # 1 mod: Instant Dialogues
   git clone -q https://github.com/idermailer/NoGreenhouseEntranceTiles.git "No Greenhouse Entrance Tiles" # 1 mod: No Greenhouse Entrance Tiles
   git clone -q https://github.com/idermailer/OutfitDisable.git "Outfit Disable" # 1 mod: Outfit Disable
   git clone -q https://github.com/idermailer/RandomStartDay.git "Random Start Day" # 1 mod: Random Start Day
   git clone -q https://github.com/Igorious/StardevValleyNewMachinesMod.git "New Machines" # 1 mod: New Machines
   git clone -q https://github.com/Igorious/Stardew_Valley_Showcase_Mod.git "Showcase" # 1 mod: Showcase
   git clone -q https://github.com/igotnobugs/PrepareNewDay.git "Prepare for New Day" # 1 mod: Prepare for New Day
   git clone -q https://github.com/ihasTaco/ValleyCast.git "Valley Cast" # 1 mod: Valley Cast
   git clone -q https://github.com/Ilyaki/ArtifactSystemFixed.git "Artifact System Fixed" # 1 mod: Artifact System Fixed
   git clone -q https://github.com/Ilyaki/BattleRoyalley.git "Battle Royalley" # 1 mod: Battle Royalley
   git clone -q https://github.com/Ilyaki/CapitalistSplitMoney.git "Capitalist Split Money" # 1 mod: Capitalist Split Money
   git clone -q https://github.com/Ilyaki/Elevator.git "Elevator" # 1 mod: Elevator
   git clone -q https://github.com/Ilyaki/NeatAdditions.git "Neat Additions" # 1 mod: Neat Additions
   git clone -q https://github.com/Ilyaki/NetworkOptimizer.git "Network Optimizer" # 1 mod: Network Optimizer
   git clone -q https://github.com/Ilyaki/Server-Browser.git "Server Browser" # 1 mod: Server Browser
   git clone -q https://github.com/Ilyaki/ServerBookmarker.git "Server Bookmarker" # 1 mod: Server Bookmarker
   git clone -q https://github.com/Ilyaki/SplitScreen.git "Split Screen" # 1 mod: Split Screen
   git clone -q https://github.com/Ilyaki/StackToNearbyChests.git "Stack to Nearby Chests" # 1 mod: Stack to Nearby Chests
   git clone -q https://github.com/Imogen599/StackableBuffs.git "Stackable Buff Durations" # 1 mod: Stackable Buff Durations
   git clone -q https://github.com/Incognito357/PrairieKingUIEnhancements.git "Prairie King UI Enhancements" # 1 mod: Prairie King UI Enhancements
   git clone -q https://github.com/instafluff/SleeplessInStardew.git "Sleepless in Stardew" # 1 mod: Sleepless in Stardew
   git clone -q https://github.com/instafluff/SpelldewValley.git "Spelldew Valley" # 1 mod: Spelldew Valley
   git clone -q https://github.com/irocendar/LightRadiusMod.git "Light Radius" # 1 mod: Light Radius
   git clone -q https://github.com/irocendar/MapEventMarkersMod.git "Map Event Markers" # 1 mod: Map Event Markers
   git clone -q https://github.com/irocendar/PetRenameMod.git "Pet Rename" # 1 mod: Pet Rename
   git clone -q https://github.com/IrregularPorygon/SMAPIGenericShopMod.git "Generic Shop Extender" # 1 mod: Generic Shop Extender
   git clone -q https://github.com/isaacs-dev/Minidew-Mods.git "Friends Forever" # 1 mod: Friends Forever
   git clone -q https://github.com/iSkLz/DecraftingMod.git "Decrafting Mod" # 1 mod: Decrafting Mod
   git clone -q https://github.com/iSkLz/ExperienceMultiplier.git "Experience Rate Multiplier" # 1 mod: Experience Rate Multiplier
   git clone -q https://github.com/itsbenter/DialogueDisplayTweaks.git "Dialogue Display Tweaks" # 1 mod: Dialogue Display Tweaks
   git clone -q https://github.com/itsbenter/ResourcefulFriends.git "Resourceful Friends" # 1 mod: Resourceful Friends
   git clone -q https://github.com/jacksonkimball/AVerySpecialBlueChicken.git "A Very Special Blue Chicken" # 1 mod: A Very Special Blue Chicken
   git clone -q https://github.com/jacob-keller/CustomBackpackFramework.git "The Return of Custom Backpack Framework" # 1 mod: The Return of Custom Backpack Framework
   git clone -q https://github.com/JacquePott/StardewValleyMods.git "Mass Production" # 1 mod: Mass Production
   git clone -q https://github.com/JadeTheavas/MoreMineLadders.git "More Mine Ladders" # 1 mod: More Mine Ladders
   git clone -q https://github.com/Jadiael1/PerFarmhandWoods.git "Per Farmhand Woods" # 1 mod: Per Farmhand Woods
   git clone -q https://github.com/Jadiael1/PerPlayerFarm.git "Per Player Farm" # 1 mod: Per Player Farm
   git clone -q https://github.com/jahangmar/StardewValleyMods.git "~jahangmar" # 9 mods: Compost&#44; Pests&#44; and Cultivation, Dismantle Craftables, Don't Eat That, Fall To Autumn, Interaction Tweaks, Leveling Adjustment, Pet Interaction, Seeds are Rare, Working Fireplace
   git clone -q https://github.com/jakee417/Fog-Mod.git "Foggy Grouse" # 1 mod: Foggy Grouse
   git clone -q https://github.com/Jaksha6472/ChallengingCommunityCenterBundles.git "~Jaksha6472" # 2 mods: Challenging Community Center Bundles, Custom Community Center Bundles
   git clone -q https://github.com/Jaksha6472/MiningShack.git "Alvadea's Farm Maps - Goldrush" # 1 mod: Alvadea's Farm Maps - Goldrush
   git clone -q https://github.com/Jaksha6472/WitchTower.git "Alvadea's Farm Maps - Walpurgisnacht" # 1 mod: Alvadea's Farm Maps - Walpurgisnacht
   git clone -q https://github.com/jamescodesthings/smapi-better-sprinklers.git "Better Sprinklers Plus" # 1 mod: Better Sprinklers Plus
   git clone -q https://github.com/jamespfluger/Stardew-EqualMoneySplit.git "~jamespfluger" # 2 mods: Auto Crop Watering, Equal Money Split
   git clone -q https://github.com/jamespfluger/Stardew-MisophoniaAccessibility.git "Misophonia Accessibility" # 1 mod: Misophonia Accessibility
   git clone -q https://github.com/jamespfluger/Stardew-ModCollection.git "Relaxing Weekends" # 1 mod: Relaxing Weekends
   git clone -q https://github.com/jamespfluger/Stardew-NoFriendshipDecayReborn.git "No Friendship Decay - Reborn" # 1 mod: No Friendship Decay - Reborn
   git clone -q https://github.com/jamespfluger/Stardew-TimeOfThePrairieKing.git "Time of the Prairie King" # 1 mod: Time of the Prairie King
   git clone -q https://github.com/janavarro95/Stardew_Valley_Mods.git "~janavarro95" # 20 mods: Advanced Save Backup, Auto Speed, Billboard Anywhere, Build Endurance, Build Health, Buy Back Collectables, Current Location, Custom Asset Modifier, Custom Shops Redux, Daily Quest Anywhere, Fall 28 Snow Day, Happy Birthday, More Rain, Museum Rearranger, Night Owl, No More Pets, Simple Sound Manager, Stardew Symphony Remastered, Stardust Core, Time Freeze
   git clone -q https://github.com/Janglinator/Secretariat.git "Secretariat (Faster Horse)" # 1 mod: Secretariat (Faster Horse)
   git clone -q https://github.com/jangofett4/FishingTreasureSpawner.git "Fishing Treasure Spawner" # 1 mod: Fishing Treasure Spawner
   git clone -q https://github.com/jaredlll08/HotbarHotswap.git "Hotbar Hotswap" # 1 mod: Hotbar Hotswap
   git clone -q https://github.com/jaredlll08/Item-Tooltips.git "Item Tooltips" # 1 mod: Item Tooltips
   git clone -q https://github.com/jaredlll08/Trash-Trash.git "Trash Trash" # 1 mod: Trash Trash
   git clone -q https://github.com/jaredtjahjadi/LogMenu.git "Log Menu" # 1 mod: Log Menu
   git clone -q https://github.com/Jarvie8176/StardewMods.git "~Jarvie8176" # 2 mods: Rented Tools, Self Service
   git clone -q https://github.com/jasisco5/UncannyValleyMod.git "Uncanny Valley" # 1 mod: Uncanny Valley
   git clone -q https://github.com/JaVonox/Stardew-Spellbook.git "Runescape Spellbook" # 1 mod: Runescape Spellbook
   git clone -q https://github.com/jbossjaslow/Stardew_Mods.git "~jbossjaslow" # 3 mods: Chatter, Villager Compass, Windowed Borderless
   git clone -q https://github.com/jbtheshadow/StardewValleyMods.git "Time Freezes at Midnight" # 1 mod: Time Freezes at Midnight
   git clone -q https://github.com/jdusbabek/stardewvalley.git "~jdusbabek" # 6 mods: Animal Sitter, Crab Net, Mail Order Pigs, Pelican Fiber, Point and Plant, Replanter
   git clone -q https://github.com/JDvickery/Sauvignon-in-Stardew.git "Sauvignon in Stardew" # 1 mod: Sauvignon in Stardew
   git clone -q https://github.com/JeanSebGwak/CustomFenceDecay.git "Custom Fence Decay" # 1 mod: Custom Fence Decay
   git clone -q https://github.com/jeffgillean/StardewValleyMods.git "Frugal Farm Menu" # 1 mod: Frugal Farm Menu
   git clone -q https://github.com/jennisimone/Expanded-Witch-Mod.git "Full Romanceable Witch NPC" # 1 mod: Full Romanceable Witch NPC
   git clone -q https://github.com/JessebotX/StardewValleyMods.git "~JessebotX" # 6 mods: Bigger Riverlands Farm, Goodbye SMAPI, Health and Stamina Regeneration, Hello SMAPI, Please Fix Error, Sprint Sprint
   git clone -q https://github.com/jessevang/BreakGeodesInBulk.git "Break Geodes in Bulk" # 1 mod: Break Geodes in Bulk
   git clone -q https://github.com/jessevang/BuildModeForTilesAndCraftables.git "Build Mode For Tiles and Big Craftables" # 1 mod: Build Mode For Tiles and Big Craftables
   git clone -q https://github.com/jessevang/BulkEatingAndDrinking.git "Bulk Eating and Drinking" # 1 mod: Bulk Eating and Drinking
   git clone -q https://github.com/jessevang/FlyingWeaponMount.git "Flying Weapon Mount" # 1 mod: Flying Weapon Mount
   git clone -q https://github.com/jessevang/HammerHeavyAttackAlwaysCrits.git "Hammer or Club Heavy Attack Always Crits" # 1 mod: Hammer or Club Heavy Attack Always Crits
   git clone -q https://github.com/jessevang/LootFIlter.git "Loot Filter Item Pickup Filter" # 1 mod: Loot Filter Item Pickup Filter
   git clone -q https://github.com/jessevang/MineforMore.git "Level For More" # 1 mod: Level For More
   git clone -q https://github.com/jessevang/MineOnDay1.git "Mine On Day 1" # 1 mod: Mine On Day 1
   git clone -q https://github.com/jessevang/QuickCastHotkeys.git "Quick Cast Hotkeys" # 1 mod: Quick Cast Hotkeys
   git clone -q https://github.com/jessevang/SpinningWeaponAndToolMod.git "Spinning Weapon and Tool Mod" # 1 mod: Spinning Weapon and Tool Mod
   git clone -q https://github.com/jessevang/StuckAtLevel1ButOverPowerDropRates.git "More Monster Drops" # 1 mod: More Monster Drops
   git clone -q https://github.com/jessevang/UnifiedExperienceSystem.git "Unified Experience System" # 1 mod: Unified Experience System
   git clone -q https://github.com/jessevang/VoiceOverFrameworkMod.git "Voice Over Framework Mod" # 1 mod: Voice Over Framework Mod
   git clone -q https://github.com/jessica0x73/StardewValleyMods.git "Extra Fish Information" # 1 mod: Extra Fish Information
   git clone -q https://github.com/jevonlipsey/stardew-inventory-while-fishing.git "Inventory While Fishing" # 1 mod: Inventory While Fishing
   git clone -q https://github.com/jevonlipsey/stardew-valley-sprinter.git "Sprinter" # 1 mod: Sprinter
   git clone -q https://github.com/JhonnieRandler/TVBrasileira.git "TV Brasileira" # 1 mod: TV Brasileira
   git clone -q https://github.com/Jibblestein/StardewMods.git "Integrated Minecarts" # 1 mod: Integrated Minecarts
   git clone -q https://github.com/jingshenSN2/ReplaceFertilizer.git "Replace Fertilizer" # 1 mod: Replace Fertilizer
   git clone -q https://github.com/Jinxiewinxie/StardewValleyMods.git "~Jinxiewinxie" # 3 mods: Stone Bridge Over Pond, Tainted Cellar, Wonderful Farm Life
   git clone -q https://github.com/Jishuna/RunningLate.git "Running Late" # 1 mod: Running Late
   git clone -q https://github.com/jlfulmer/PetEnhancementMod.git "Pet Enhancement Mod" # 1 mod: Pet Enhancement Mod
   git clone -q https://github.com/jltaylor-us/StardewGMCMOptions.git "GMCM Options" # 1 mod: GMCM Options
   git clone -q https://github.com/jltaylor-us/StardewHoldToProcessGeodes.git "Hold to Process Geodes" # 1 mod: Hold to Process Geodes
   git clone -q https://github.com/jltaylor-us/StardewJsonProcessor.git "Json Processor" # 2 mods: Content Patcher Json Processor, Json Processor
   git clone -q https://github.com/jltaylor-us/StardewRainbowCursor.git "Rainbow Cursor" # 1 mod: Rainbow Cursor
   git clone -q https://github.com/jltaylor-us/StardewRangeHighlight.git "Range Highlight" # 1 mod: Range Highlight
   git clone -q https://github.com/jltaylor-us/StardewToDew.git "To-Dew" # 1 mod: To-Dew
   git clone -q https://github.com/jn84/QualitySmash.git "Quality Smash" # 1 mod: Quality Smash
   git clone -q https://github.com/joaniedavis/RandomHairMod.git "Random Hair Color" # 1 mod: Random Hair Color
   git clone -q https://github.com/joelra/SDV-PassableLadders.git "Passable Ladders" # 1 mod: Passable Ladders
   git clone -q https://github.com/JoeStrout/Farmtronics.git "Farmtronics" # 1 mod: Farmtronics
   git clone -q https://github.com/JohnsonNicholas/SDVMods.git "~JohnsonNicholas" # 2 mods: Solar Eclipse Event, Summit Reborn
   git clone -q https://github.com/joisse1101/InstantAnimals.git "Instant Animals" # 1 mod: Instant Animals
   git clone -q https://github.com/jokthefoo/StardewMods.git "Bigger Machines" # 1 mod: Bigger Machines
   git clone -q https://github.com/Jolly-Alpaca/PrismaticDinosaur.git "Prismatic Dinosaur" # 1 mod: Prismatic Dinosaur
   git clone -q https://github.com/Jolly-Alpaca/PrismaticValleyFramework.git "Prismatic Valley Framework" # 1 mod: Prismatic Valley Framework
   git clone -q https://github.com/JonathanFeenstra/Auto-StackBaitAndAmmo.git "Auto-Stack Bait & Ammo" # 1 mod: Auto-Stack Bait & Ammo
   git clone -q https://github.com/JonathanFeenstra/SmartSpin.git "Smart Spin - Pre-fill Optimal Wager" # 1 mod: Smart Spin - Pre-fill Optimal Wager
   git clone -q https://github.com/Jonqora/StardewMods.git "~Jonqora" # 4 mods: Angry Grandpa, Fish Preview, Show Item Quality, UV Index (Sunscreen Mod)
   git clone -q https://github.com/jorgamun/EventMusicVolume.git "Event Music Volume" # 1 mod: Event Music Volume
   git clone -q https://github.com/jorgamun/HorseSqueeze.git "Horse Squeeze" # 1 mod: Horse Squeeze
   git clone -q https://github.com/jorgamun/PauseInMultiplayer.git "Pause in Multiplayer" # 1 mod: Pause in Multiplayer
   git clone -q https://github.com/joshjke/MultiplayerPortalGuns.git "Portal Guns" # 1 mod: Portal Guns
   git clone -q https://github.com/JoXW100/GiftTasteHelper.git "Gift Taste Helper Continued x2" # 1 mod: Gift Taste Helper Continued x2
   git clone -q https://github.com/JoXW100/StardewValleyMods.git "~JoXW100" # 7 mods: Hugs and Kisses Continued, Mobile Phone Continued, Mobile Phone Continued → Arcade Mobile Apps, Mobile Phone Continued → Calendar Mobile App, Mobile Phone Continued → Email, Mobile Phone Continued → Mobile Catalogues, Mobile Phone Continued → Television Mobile App
   git clone -q https://github.com/JoXW100/SW_Predictor.git "Predictor" # 15 mods: Predictor, Predictor →  Dig Spot Patch, Predictor → Breakable Container Patch, Predictor → Bush Patch, Predictor → Crab Pot Patch, Predictor → Crop Patch, Predictor → Fishing Patch, Predictor → Garbage Can Patch, Predictor → Geode Patch, Predictor → Mineable Patch, Predictor → Minigames Patch, Predictor → Panning Patch, Predictor → Seed Maker Patch, Predictor → Spawned Patch, Predictor → Tree Patch
   git clone -q https://github.com/JPANv2/Stardew-Valley-Mine-Changes.git "Mine Changes" # 1 mod: Mine Changes
   git clone -q https://github.com/JPANv2/Stardew-Valley-Tree-Changes.git "Tree Changes" # 1 mod: Tree Changes
   git clone -q https://github.com/jpparajeles/StardewValleyMods.git "Wild Flowers Reimagined" # 1 mod: Wild Flowers Reimagined
   git clone -q https://github.com/jslattery26/stardew_mods.git "~jslattery26" # 2 mods: Chest Pooling V2, Shared Recipes
   git clone -q https://github.com/JudeRV/SDV_WikiLinker.git "Character Wiki Linker" # 1 mod: Character Wiki Linker
   git clone -q https://github.com/JudeRV/Stardew-ActivatingSprinklers.git "Activated Sprinklers" # 1 mod: Activated Sprinklers
   git clone -q https://github.com/JudeRV/Stardew-EasyPathDestruction.git "Easy Path Destruction" # 1 mod: Easy Path Destruction
   git clone -q https://github.com/JudeRV/Stardew-HeartLevelNotifier.git "Heart Level Notifications" # 1 mod: Heart Level Notifications
   git clone -q https://github.com/JudeRV/Stardew-MaxCastStamina.git "Max Cost Stamina" # 1 mod: Max Cost Stamina
   git clone -q https://github.com/JudeRV/Stardew-PassiveFriendship.git "Passive Friendship" # 1 mod: Passive Friendship
   git clone -q https://github.com/JudeRV/Stardew-RainRefillsWateringCan.git "Rain Refills Watering Can" # 1 mod: Rain Refills Watering Can
   git clone -q https://github.com/JudeRV/Stardew-ReportMobCounts.git "Mob Count Reports" # 1 mod: Mob Count Reports
   git clone -q https://github.com/JulianoSFA/WallpaperSwitcher.git "Wallpaper Switcher" # 1 mod: Wallpaper Switcher
   git clone -q https://github.com/juminos/StardewValleyMods.git "~juminos" # 5 mods: Agrivoltaics, Critter Fixes, Dino Form 2, Frenship Rings, Monster Hutch Framework
   git clone -q https://github.com/Just-Chaldea/GiftTasteHelper.git "Gift Taste Helper Continued" # 1 mod: Gift Taste Helper Continued
   git clone -q https://github.com/justastranger/ArtisanProductsCopyQuality.git "Artisan Products Copy Quality" # 1 mod: Artisan Products Copy Quality
   git clone -q https://github.com/justastranger/MushroomLogAdditions.git "Mushroom Log Additions" # 1 mod: Mushroom Log Additions
   git clone -q https://github.com/JustCylon/stardew-brewery.git "Stardew Brewery" # 1 mod: Stardew Brewery
   git clone -q https://github.com/Jwaad/Stardew_Player_Arrows.git "Player Arrows" # 1 mod: Player Arrows
   git clone -q https://github.com/K4rakara/Stardew-Mods.git "JoJa84+" # 1 mod: JoJa84+
   git clone -q https://github.com/KabigonFirst/MineAssist.git "Mine Assist" # 1 mod: Mine Assist
   git clone -q https://github.com/KabigonFirst/StardewValleyMods.git "~KabigonFirst" # 2 mods: Campfire Cooking, Kisekae
   git clone -q https://github.com/kakashigr/stardew-radioactivetools.git "Radioactive Tools" # 1 mod: Radioactive Tools
   git clone -q https://github.com/KankuroGB/NightLight.git "Night Light" # 1 mod: Night Light
   git clone -q https://github.com/Kantrip-Mods/SDV.git "~Kantrip-Mods" # 3 mods: Reverse Proposals, Special Spouse Dialogue Framework, Wedding Anniversaries
   git clone -q https://github.com/KathrynHazuka/StardewValley_BirthdayMail.git "Birthday Mail" # 1 mod: Birthday Mail
   git clone -q https://github.com/KathrynHazuka/StardewValley_FasterRun.git "Faster Run" # 1 mod: Faster Run
   git clone -q https://github.com/KDERazorback/SvFishingMod.git "SV Fishing Mod" # 1 mod: SV Fishing Mod
   git clone -q https://github.com/KediDili/Creaturebook.git "Creaturebook" # 1 mod: Creaturebook
   git clone -q https://github.com/KediDili/FurnitureTweaks.git "Kedi's Furniture Tweaks" # 1 mod: Kedi's Furniture Tweaks
   git clone -q https://github.com/KediDili/NPCUtilities.git "Kedi's NPC Utilities" # 1 mod: Kedi's NPC Utilities
   git clone -q https://github.com/KediDili/SMAPicross.git "Picross Arcade Mod" # 1 mod: Picross Arcade Mod
   git clone -q https://github.com/KediDili/VanillaPlusProfessions.git "Vanilla Plus Professions" # 1 mod: Vanilla Plus Professions
   git clone -q https://github.com/KeelanRosa/StardewMods.git "Demetrius Visits Cave" # 1 mod: Demetrius Visits Cave
   git clone -q https://github.com/kenny2892/StardewValleyMods.git "~kenny2892" # 2 mods: Custom NPC Festival Additions, Flower Dancing
   git clone -q https://github.com/kesesek/StardewValleyMapTeleport.git "Map Teleport For Expansions" # 1 mod: Map Teleport For Expansions
   git clone -q https://github.com/kevinforrestconnors/ProfessionAdjustments.git "Profession Adjustments" # 1 mod: Profession Adjustments
   git clone -q https://github.com/kevinforrestconnors/RealisticFishing.git "Realistic Fishing" # 1 mod: Realistic Fishing
   git clone -q https://github.com/KhloeLeclair/StardewMods.git "~KhloeLeclair" # 10 mods: Almanac, Almanac → DGA Support, Better Crafting, Better Crafting: Buildings, Better Game Menu, Cloudy Skies, Giant Crop Tweaks, More Nightly Events, See Me Rollin', Theme Manager
   git clone -q https://github.com/kiranmurmu/StardewValleyMods.git "Lucky Reclamation Trash Can" # 1 mod: Lucky Reclamation Trash Can
   git clone -q https://github.com/KirbyLink/BeeHouseFix.git "Bee House Flower Range Fix" # 1 mod: Bee House Flower Range Fix
   git clone -q https://github.com/KirbyLink/FestivalEndTimeTweak.git "Festival End Time Tweak" # 1 mod: Festival End Time Tweak
   git clone -q https://github.com/KirbyLink/NoSeedsFromTreesFix.git "No Seeds From Trees Fix" # 1 mod: No Seeds From Trees Fix
   git clone -q https://github.com/KirbyLink/PumpkinKing.git "The Pumpkin King" # 1 mod: The Pumpkin King
   git clone -q https://github.com/KirbyLink/UnderdarkKrobus.git "Elven Krobus" # 1 mod: Elven Krobus
   git clone -q https://github.com/KirbyLink/UnderdarkSewer.git "Underdark Sewer" # 1 mod: Underdark Sewer
   git clone -q https://github.com/Kodfod/BetterHitboxStardew.git "Better Hitbox" # 1 mod: Better Hitbox
   git clone -q https://github.com/kqrse/StardewValleyMods-aedenthorn.git "~kqrse (aedenthorn mods)" # 3 mods: Crop Watering Bubbles Continued, Like a Duck to Water Continued, Mailbox Menu Continued
   git clone -q https://github.com/kqrse/StardewValleyMods.git "~kqrse" # 7 mods: Better Truffles, Brighter Building Paint, Empty Jar Bubbles, Fertilizer Bubbles, Place Floor on Tilled Dirt, Skinny Animals, Where's My Horse
   git clone -q https://github.com/kristian-skistad/automators.git "Automators" # 1 mod: Automators
   git clone -q https://github.com/kronosta/Stardew-Mods.git "~kronosta" # 2 mods: Forage Crops, Typable Books
   git clone -q https://github.com/Kryspur/HypnoValley.git "Hypno Valley" # 1 mod: Hypno Valley
   git clone -q https://github.com/KyuubiRan/TimeWatch.git "Time Watch" # 1 mod: Time Watch
   git clone -q https://github.com/L3thal1ty/No-Kids-Ever.git "No Kids Ever" # 1 mod: No Kids Ever
   git clone -q https://github.com/LajnaLegenden/Stardew_Valley_Mods.git "24h Clock" # 1 mod: 24h Clock
   git clone -q https://github.com/Lake1059/StardewAutoGC.git "Stardew Auto GC" # 1 mod: Stardew Auto GC
   git clone -q https://github.com/Lake1059/StardewFPS.git "Stardew FPS (Animation Frame Speed)" # 1 mod: Stardew FPS (Animation Frame Speed)
   git clone -q https://github.com/lambui/StardewValleyMod_OmniFarm.git "OmniFarm" # 1 mod: OmniFarm
   git clone -q https://github.com/lambui/StardewValleyMod_StashItemsToChest.git "Stash Items to Chest" # 1 mod: Stash Items to Chest
   git clone -q https://github.com/leejohnkt11/WSF.git "WSF's Mod" # 1 mod: WSF's Mod
   git clone -q https://github.com/LeFauxMatt/CarryChests.git "Carry Chests" # 1 mod: Carry Chests
   git clone -q https://github.com/LeFauxMatt/ColorfulChests.git "Colorful Chests (LeFauxMatt)" # 1 mod: Colorful Chests
   git clone -q https://github.com/LeFauxMatt/CrystallineJunimoChests.git "Crystalline Junimo Chests" # 1 mod: Crystalline Junimo Chests
   git clone -q https://github.com/LeFauxMatt/CustomBush.git "Custom Bush" # 2 mods: Custom Bush, Custom Bush Automate
   git clone -q https://github.com/LeFauxMatt/EasyAccess.git "Easy Access" # 1 mod: Easy Access
   git clone -q https://github.com/LeFauxMatt/ExpandedStorage.git "Expanded Storage" # 1 mod: Expanded Storage
   git clone -q https://github.com/LeFauxMatt/FindAnything.git "Find Anything" # 1 mod: Find Anything
   git clone -q https://github.com/LeFauxMatt/GarbageDay.git "Garbage Day" # 1 mod: Garbage Day
   git clone -q https://github.com/LeFauxMatt/IconicFramework.git "Iconic Framework" # 1 mod: Iconic Framework
   git clone -q https://github.com/LeFauxMatt/Sandwich.git "Sandwich" # 1 mod: Sandwich
   git clone -q https://github.com/LeFauxMatt/SelfCheckout.git "Self Checkout" # 1 mod: Self Checkout
   git clone -q https://github.com/LeFauxMatt/StardewMods.git "~LeFauxMatt" # 10 mods: Better Chests, Cycle Tools, FauxCore, Is It Cake, Ordinary Capsule, Portable Holes, Shopping Cart, Smack Dat Scarecrow, Stack Quality, Too Many Animals
   git clone -q https://github.com/LeFauxMatt/UltraOrganizedChests.git "Ultra Organized Chests" # 1 mod: Ultra Organized Chests
   git clone -q https://github.com/LeFauxMatt/UnlimitedStorage.git "Unlimited Storage" # 1 mod: Unlimited Storage
   git clone -q https://github.com/legovader09/SDVLoanMod.git "Loan Mod" # 1 mod: Loan Mod
   git clone -q https://github.com/LeonBlade/CasksAnywhere.git "Casks Anywhere" # 1 mod: Casks Anywhere
   git clone -q https://github.com/LeonBlade/TreeTransplant.git "Tree Transplant" # 1 mod: Tree Transplant
   git clone -q https://github.com/Leroymilo/Catalogue-Framework.git "Catalogue Framework" # 1 mod: Catalogue Framework
   git clone -q https://github.com/Leroymilo/FurnitureFramework.git "Furniture Framework" # 1 mod: Furniture Framework
   git clone -q https://github.com/Leroymilo/MZG.git "Modular Zen Garden" # 1 mod: Modular Zen Garden
   git clone -q https://github.com/LetsTussleBoiz/MatrixFishingUI.git "Fish Helper UI" # 1 mod: Fish Helper UI
   git clone -q https://github.com/LilietB/HappyHunting.git "Happy Hunting" # 1 mod: Happy Hunting
   git clone -q https://github.com/linfanqian/CraftCookTracker.git "CraftCook Tracker" # 1 mod: CraftCook Tracker
   git clone -q https://github.com/LinHuiGD/StardewValleyMods.git "Water Depth Overlay" # 1 mod: Water Depth Overlay
   git clone -q https://github.com/linkoid/Stardew.YetAnother.ContentPatcher.git "Yet Another Content Patcher" # 1 mod: Yet Another Content Patcher
   git clone -q https://github.com/lisamreynolds/MusicalCellar.git "Musical Cellar" # 1 mod: Musical Cellar
   git clone -q https://github.com/lisyce/StardewValleyMods.git "~lisyce" # 12 mods: BarleyZP's Allergies, Conversation Topic Utilities, Craftable and Functional Wishing Well, Failed Quests Lose Friendship, Fishing Tackle Tooltips, Items Have Weight, Mushroom Log Framework, Run In The Bath House (Spa), Shareable Mod List Generator, Stardew Audio Captions, Stop Offering My Stuff, Volcano Button Tracker
   git clone -q https://github.com/littleraskol/Basic-Sprinkler-Improved.git "Basic Sprinkler Improved" # 1 mod: Basic Sprinkler Improved
   git clone -q https://github.com/littleraskol/ReRegeneration.git "ReRegeneration" # 1 mod: ReRegeneration
   git clone -q https://github.com/littleraskol/Sprint-And-Dash-Redux.git "Sprint and Dash Redux" # 1 mod: Sprint and Dash Redux
   git clone -q https://github.com/Lixeer/ValleyServer.git "Command Web UI" # 1 mod: Command Web UI
   git clone -q https://github.com/LodGvedeon/DroneWarehouseMod.git "Drone Warehouse" # 1 mod: Drone Warehouse
   git clone -q https://github.com/loe2run/ChildToNPC.git "Child to NPC" # 1 mod: Child to NPC
   git clone -q https://github.com/loe2run/FamilyPlanningMod.git "Family Planning" # 1 mod: Family Planning
   git clone -q https://github.com/Logoddy/Odd-Quality-of-Life.git "Odd Quality of Life" # 1 mod: Odd Quality of Life
   git clone -q https://github.com/LonerAxl/Stardew_HarvestCalendar.git "Harvest Calendar" # 1 mod: Harvest Calendar
   git clone -q https://github.com/LostRuins/koboldcpp.git "Cobold AI Valley" # 1 mod: Cobold AI Valley
   git clone -q https://github.com/lshtech/AnimalProduceExpansion.git "Animal Produce Expansion" # 1 mod: Animal Produce Expansion
   git clone -q https://github.com/lshtech/AnimalShopConditions.git "Animal Shop Conditions" # 1 mod: Animal Shop Conditions
   git clone -q https://github.com/lshtech/DialogueExtension.git "Dialogue Extension" # 1 mod: Dialogue Extension
   git clone -q https://github.com/lshtech/StardewValleyMods.git "Barn Incubator Support" # 1 mod: Barn Incubator Support
   git clone -q https://github.com/lucaskfreitas/ImmersiveScarecrows.git "The Return of Immersive Scarecrows" # 1 mod: The Return of Immersive Scarecrows
   git clone -q https://github.com/lucaskfreitas/ImmersiveSprinklers.git "The Return of Immersive Sprinklers" # 1 mod: The Return of Immersive Sprinklers
   git clone -q https://github.com/LucasOCastro/StardewValley-BetterFrogControl.git "Better Frog Control" # 1 mod: Better Frog Control
   git clone -q https://github.com/LucasOCastro/StardewValley-PanHat.git "Pan Hats" # 1 mod: Pan Hats
   git clone -q https://github.com/LukeSeewald/PublicStardewValleyMods.git "What Are You Missing" # 1 mod: What Are You Missing
   git clone -q https://github.com/LunarDiver/StardewValley.ItemRecoveryPlus.git "Item Recovery Plus" # 1 mod: Item Recovery Plus
   git clone -q https://github.com/LunaticShade/StardewValley.SafeReading.git "Safe Reading" # 1 mod: Safe Reading
   git clone -q https://github.com/LunaticShade/StardewValley.SkillfulClothes.git "Skillful Clothes" # 1 mod: Skillful Clothes
   git clone -q https://github.com/M3ales/ChestNaming.git "Chest Naming" # 1 mod: Chest Naming
   git clone -q https://github.com/M3ales/FishingLogbook.git "Fishing Logbook" # 1 mod: Fishing Logbook
   git clone -q https://github.com/M3ales/RelationshipTooltips.git "Relationship Tooltips" # 1 mod: Relationship Tooltips
   git clone -q https://github.com/MaciejMarkuszewski/StardewValleyAutoToolSelect.git "Auto Tool Select" # 1 mod: Auto Tool Select
   git clone -q https://github.com/MaciejMarkuszewski/StardewValleyMultiplayerTime.git "Multiplayer Time" # 1 mod: Multiplayer Time
   git clone -q https://github.com/MaciejMarkuszewski/StardewValleySeparateGreenhouse.git "Separate Greenhouse" # 1 mod: Separate Greenhouse
   git clone -q https://github.com/maciel310/sdv-give-anywhere.git "Give Anywhere" # 1 mod: Give Anywhere
   git clone -q https://github.com/maciel310/sdv-iridium-horse-flute.git "Iridium Horse Flute" # 1 mod: Iridium Horse Flute
   git clone -q https://github.com/MadaraUchiha/NonDestructiveNPCs.git "Non Destructive NPCs" # 1 mod: Non Destructive NPCs
   git clone -q https://github.com/maephie/StardewMods.git "Autofill Pet Bowl" # 1 mod: Autofill Pet Bowl
   git clone -q https://github.com/maguc00/explodableBushes.git "Explodable Bushes" # 1 mod: Explodable Bushes
   git clone -q https://github.com/ManApart/RetroActiveAchievements.git "Retro Active Achievements" # 1 mod: Retro Active Achievements
   git clone -q https://github.com/ManApart/shipment-tracker.git "Shipment Tracker" # 1 mod: Shipment Tracker
   git clone -q https://github.com/MangusuPixel/DialogueDisplayFrameworkContinued.git "Dialogue Display Framework Continued" # 1 mod: Dialogue Display Framework Continued
   git clone -q https://github.com/MangusuPixel/MonsterFurnitureDrop.git "Monster Furniture Drop" # 1 mod: Monster Furniture Drop
   git clone -q https://github.com/MangusuPixel/PikminJunimos.git "Pikmin Junimos" # 1 mod: Pikmin Junimos
   git clone -q https://github.com/MangusuPixel/RunningCostsStamina.git "Running Costs Stamina" # 1 mod: Running Costs Stamina
   git clone -q https://github.com/marinsgabriel1997/DetailedDescriptions.git "Better Detailed Descriptions" # 1 mod: Better Detailed Descriptions
   git clone -q https://github.com/MartyrPher/GetGlam.git "Get Glam" # 1 mod: Get Glam
   git clone -q https://github.com/MateusAquino/stardewids.git "Farmhouse Upgrade Configurable" # 1 mod: Farmhouse Upgrade Configurable
   git clone -q https://github.com/maxmakesmods/AudioDevices.git "Audio Devices" # 1 mod: Audio Devices
   git clone -q https://github.com/maxmakesmods/BetterSkullCavernFalling.git "Better Skull Cavern Falling" # 1 mod: Better Skull Cavern Falling
   git clone -q https://github.com/maxmakesmods/DeepWoodsMod.git "Deep Woods" # 1 mod: Deep Woods
   git clone -q https://github.com/maxmakesmods/LeftyMode.git "Lefty Mode" # 1 mod: Lefty Mode
   git clone -q https://github.com/maxmakesmods/pickupclutter.git "Pick Up Clutter" # 1 mod: Pick Up Clutter
   git clone -q https://github.com/Mekadrom/IFramesInMines.git "IFrames in Mines" # 1 mod: IFrames in Mines
   git clone -q https://github.com/MercuriusXeno/EquivalentExchange.git "Equivalent Exchange" # 1 mod: Equivalent Exchange
   git clone -q https://github.com/MercuriusXeno/RegenPercent.git "Percent of Max Stamina and Health Regeneration" # 1 mod: Percent of Max Stamina and Health Regeneration
   git clone -q https://github.com/MercuryVN/AutoCatch.git "Auto Catch" # 1 mod: Auto Catch
   git clone -q https://github.com/MercuryVN/BaitsDoTheirJobs.git "Baits Do Their Jobs" # 1 mod: Baits Do Their Jobs
   git clone -q https://github.com/MercuryVN/GrowThatGiantCrop.git "Grow That Giant Crop" # 1 mod: Grow That Giant Crop
   git clone -q https://github.com/MercuryVN/HorseFluteAutoMount.git "Horse Flute Auto Mount" # 1 mod: Horse Flute Auto Mount
   git clone -q https://github.com/MercuryVN/OnlyFishConsumeBait.git "Only Fish Consume Bait" # 1 mod: Only Fish Consume Bait
   git clone -q https://github.com/MiguelLucas/StardewValleyMods.git "Adjustable Building Costs" # 1 mod: Adjustable Building Costs
   git clone -q https://github.com/MiihauLOL/CommandsForTwitchChatMod.git "Commands For Twitch Chat Mod" # 1 mod: Commands For Twitch Chat Mod
   git clone -q https://github.com/MiihauLOL/TwitchChatMod.git "Twitch Chat Mod" # 1 mod: Twitch Chat Mod
   git clone -q https://github.com/mikael-uhl/SimplyShirtless.git "Simply Shirtless" # 1 mod: Simply Shirtless
   git clone -q https://github.com/Mikesnorth/StardewNewsFeed.git "Stardew News Feed" # 1 mod: Stardew News Feed
   git clone -q https://github.com/miketweaver/BashNinja_SDV_Mods.git "Dynamic NPC Sprites" # 1 mod: Dynamic NPC Sprites
   git clone -q https://github.com/miketweaver/DailyNews.git "Daily News" # 1 mod: Daily News
   git clone -q https://github.com/millerscout/StardewMillerMods.git "~millerscout" # 2 mods: Economy Mod, Retroactive Stardew
   git clone -q https://github.com/MindMeltMax/SAML.git "SAML" # 1 mod: SAML
   git clone -q https://github.com/MindMeltMax/Stardew-Valley-Mods.git "~MindMeltMax" # 29 mods: Better Shipping Bin, Buildings Included, Change Farm Caves, Chest Displays, Craft Anything Framework, Crop Walker, Custom Names, Easy Island Puzzle, Fairy Fix, Fishing Nets, Fishing Nets → Automate, Fishing Nets → Better Crafting, Geode Preview, Glow Buff, Hay Collection, Joja's Hauntings, Let Me Shop, Locked Chests, Multi-User Chests, Multiplayer Info, Parrot Perch, Piggy Bank Mod, Quality Bait, Shared Community Center Rewards, Single-Player Regen, Sounds Patcher, Trading, Unbreakable Tackles, Winter Pigs
   git clone -q https://github.com/minervamaga/FeelingLucky.git "Feeling Lucky_" # 1 mod: Feeling Lucky?
   git clone -q https://github.com/minervamaga/NoMineFlyersRedux.git "No Mine Flyers Redux" # 1 mod: No Mine Flyers Redux
   git clone -q https://github.com/minervamaga/StandardizedKrobus.git "Standardized Krobus" # 1 mod: Standardized Krobus
   git clone -q https://github.com/miome/MultitoolMod.git "Multitool" # 1 mod: Multitool
   git clone -q https://github.com/miome/OneDayAtATime.git "One Day at a Time" # 1 mod: One Day at a Time
   git clone -q https://github.com/mishagp/PauseInMultiplayer.git "Pause Time in Multiplayer Revived" # 1 mod: Pause Time in Multiplayer Revived
   git clone -q https://github.com/MissCoriel/Customize-Health-and-Stamina.git "Customize Starting Health and Stamina" # 1 mod: Customize Starting Health and Stamina
   git clone -q https://github.com/MissCoriel/Dear-Diary.git "Dear Diary" # 1 mod: Dear Diary
   git clone -q https://github.com/MissCoriel/Event-Repeater.git "Event Repeater" # 1 mod: Event Repeater
   git clone -q https://github.com/MissCoriel/Investment.git "Investment" # 1 mod: Investment
   git clone -q https://github.com/MissCoriel/InvestmentMaximized.git "Investment Maximized" # 1 mod: Investment Maximized
   git clone -q https://github.com/MissCoriel/NPCUtilities.git "Miss Coriel's NPC Tool Kit" # 1 mod: Miss Coriel's NPC Tool Kit
   git clone -q https://github.com/MissCoriel/Position-Tool.git "Miss Coriel's Position Tool" # 1 mod: Miss Coriel's Position Tool
   git clone -q https://github.com/MissCoriel/Sit-n-Relax.git "Sit n' Relax_ Rest Your Butt" # 1 mod: Sit n' Relax: Rest Your Butt
   git clone -q https://github.com/MissCoriel/Tractor-Sound.git "Tractor Sounds" # 1 mod: Tractor Sounds
   git clone -q https://github.com/MissCoriel/UniqueCourtshipResponseCore.git "Miss Coriel's Unique Courtship Response Core" # 1 mod: Miss Coriel's Unique Courtship Response Core
   git clone -q https://github.com/misty-spring/StardewMods.git "~misty-spring" # 10 mods: Audio Descriptions, Dynamic Dialogues Framework, Farmhouse Visits, Frog Drops Loot, Ginger Island Extra Locations, Immersive Grandpa, Item Extensions, Krobus Sleeps, Prismatic Butterfly Notifier, Spouses in Ginger Island
   git clone -q https://github.com/Mizzion/MyStardewMods.git "~Mizzion" # 13 mods: Artifact Digger, Bank of Ferngill, Configure Machine Outputs, Enhanced Relationships, Fish Reminder, Increase Animal House Max Population, Increased Artifact Spots, One Sprinkler One Scarecrow, Qi Exchanger, Stardew Locations, Wallpaper Recycler, Water Can Refiller, Water Pet Bowl
   git clone -q https://github.com/mjSurber/FarmHouseRedone.git "Farmhouse Redone" # 1 mod: Farmhouse Redone
   git clone -q https://github.com/mjSurber/Map-Utilities.git "Map Utilities" # 1 mod: Map Utilities
   git clone -q https://github.com/MoisesGodoy17/bGames-Stardew-Valley-Mod.git "bGames" # 1 mod: bGames
   git clone -q https://github.com/MolsonCAD/DeluxeJournal.git "Deluxe Journal" # 1 mod: Deluxe Journal
   git clone -q https://github.com/mopquill/Mopsys-Ranch-Livin.git "Mopsy's Ranch Livin'" # 1 mod: Mopsy's Ranch Livin'
   git clone -q https://github.com/mouahrara/BuildableGingerIslandFarm.git "Buildable Ginger Island Farm" # 1 mod: Buildable Ginger Island Farm
   git clone -q https://github.com/mouahrara/FlipBuildings.git "Flip Buildings" # 1 mod: Flip Buildings
   git clone -q https://github.com/mouahrara/QOLEssentials.git "QOL Essentials" # 1 mod: QOL Essentials
   git clone -q https://github.com/mouahrara/RelocateBuildingsAndFarmAnimals.git "Relocate Buildings And Farm Animals" # 1 mod: Relocate Buildings And Farm Animals
   git clone -q https://github.com/mouahrara/TrainStation.git "Train Station (Iterum)" # 1 mod: Train Station (Iterum)
   git clone -q https://github.com/mouahrara/TransportFramework.git "Transport Framework" # 1 mod: Transport Framework
   git clone -q https://github.com/MouseyPounds/stardew-mods.git "~MouseyPounds" # 10 mods: Anything Ponds, Crane Man Begone, Crop Color Combiner, Dish of the Day Display, Festival of the Mundane, Floor Shadow Switcher, Home Sewing Kit, Monster Log Anywhere, Plantable Palm Trees, Pond Painter
   git clone -q https://github.com/mpcomplete/StardewMods.git "Intravenous Coffee" # 1 mod: Intravenous Coffee
   git clone -q https://github.com/mpomirski/StardewMods.git "Indoor Sprinklers" # 1 mod: Indoor Sprinklers
   git clone -q https://github.com/mralbobo/stardew-chest-pooling.git "Chest Pooling" # 1 mod: Chest Pooling
   git clone -q https://github.com/mralbobo/stardew-gate-opener.git "Gate Opener" # 1 mod: Gate Opener
   git clone -q https://github.com/mralbobo/stardew-tool-charging.git "Tool Charging" # 1 mod: Tool Charging
   git clone -q https://github.com/mrveress/SDVMods.git "Seed Machines" # 1 mod: Seed Machines
   git clone -q https://github.com/mttdr/sdv-streamlined-quality.git "Streamlined Item Quality" # 1 mod: Streamlined Item Quality
   git clone -q https://github.com/mucchan/sv-mod-prairie-king.git "Prairie King Made Easy" # 1 mod: Prairie King Made Easy
   git clone -q https://github.com/mus-candidus/CellarAvailable.git "Cellar Available" # 1 mod: Cellar Available
   git clone -q https://github.com/mus-candidus/GoToBed.git "Go To Bed" # 1 mod: Go To Bed
   git clone -q https://github.com/mus-candidus/ImagEd.git "ImagEd" # 1 mod: ImagEd
   git clone -q https://github.com/mus-candidus/LittleNPCs.git "Little NPCs" # 1 mod: Little NPCs
   git clone -q https://github.com/mus-candidus/MiceInTheValley.git "Mice in the Valley" # 1 mod: Mice in the Valley
   git clone -q https://github.com/mus-candidus/TrendyHaley.git "Trendy Haley" # 1 mod: Trendy Haley
   git clone -q https://github.com/musbah/StardewValleyMods.git "Community Bundle Item Tooltip" # 1 mod: Community Bundle Item Tooltip
   git clone -q https://github.com/Mushymato/CompactSearchableShopMenu.git "Compact Searchable Shop Menu" # 1 mod: Compact Searchable Shop Menu
   git clone -q https://github.com/Mushymato/CueSwap.git "Cue Swap" # 1 mod: Cue Swap
   git clone -q https://github.com/Mushymato/ESBarberShop.git "East Scarp → Barber Shop" # 1 mod: East Scarp → Barber Shop
   git clone -q https://github.com/Mushymato/FarmComputerNetwork.git "Farm Computer Network" # 1 mod: Farm Computer Network
   git clone -q https://github.com/Mushymato/FishPondering.git "Fish Pondering" # 1 mod: Fish Pondering
   git clone -q https://github.com/Mushymato/GrassVariety.git "Grass Variety" # 1 mod: Grass Variety
   git clone -q https://github.com/Mushymato/LivestockBazaar.git "Livestock Bazaar" # 1 mod: Livestock Bazaar
   git clone -q https://github.com/Mushymato/MachineControlPanel.git "Machine Control Panel" # 1 mod: Machine Control Panel
   git clone -q https://github.com/Mushymato/MiscMapActionsProperties.git "Misc Map Actions & Properties" # 1 mod: Misc Map Actions & Properties
   git clone -q https://github.com/Mushymato/MonsterVariety.git "Monster Variety" # 1 mod: Monster Variety
   git clone -q https://github.com/Mushymato/PeliQ.git "PeliQ" # 1 mod: PeliQ
   git clone -q https://github.com/Mushymato/PortraitsForAll.git "They Deserve It Too" # 1 mod: They Deserve It Too
   git clone -q https://github.com/Mushymato/StardewMods.git "~Mushymato" # 6 mods: Full Inventory Toolbar (and Invisible Toolbar), More Visible Cask Quality, Mothman, Scythe Tool Enchantments, Special Order Notifications, Sprinkler Attachments
   git clone -q https://github.com/Mushymato/TrinketTinker.git "Trinket Tinker" # 1 mod: Trinket Tinker
   git clone -q https://github.com/mustafa-git/StopSoundsWhenAltTabbed.git "Stop Sounds When in Background" # 1 mod: Stop Sounds When in Background
   git clone -q https://github.com/Mystra007/ExtendedFridgeSMAPI.git "Extended Fridge" # 1 mod: Extended Fridge
   git clone -q https://github.com/mytigio/CustomHarvestFix.git "Custom Harvest Fix" # 1 mod: Custom Harvest Fix
   git clone -q https://github.com/mytigio/SDVMods.git "~mytigio" # 2 mods: Exhaustion Tweaks, Subterranian Overhaul
   git clone -q https://github.com/myuusubi/SteamDew.git "SteamDew" # 1 mod: SteamDew
   git clone -q https://github.com/namelessto/EnchantedGalaxyWeapons.git "Enchanted Galaxy and Infinity Weapons" # 1 mod: Enchanted Galaxy and Infinity Weapons
   git clone -q https://github.com/namelessto/SmithYourself.git "Smith yourself" # 1 mod: Smith yourself
   git clone -q https://github.com/natfoth/StardewValleyMods.git "~natfoth" # 2 mods: StaminaRegen, Weather Controller
   git clone -q https://github.com/ncarigon/StardewValleyMods.git "~ncarigon" # 14 mods: Adjustable Sprinklers, Better Fish Ponds, Better Honey Mead, Bundles Open Sooner, Bush Bloom Mod, Copper Still, Garden Pot - Automate, Garden Pot Options, More Sensible Juices, Passable Crops, Rabbits Have Babies, Relaxed Mastery, Tree Shake Mod, Usable Hay Hoppers
   git clone -q https://github.com/Neosinf/StardewDruid.git "Stardew Druid" # 1 mod: Stardew Druid
   git clone -q https://github.com/NermNermNerm/JojaFinancial.git "Joja Financial" # 1 mod: Joja Financial
   git clone -q https://github.com/NermNermNerm/Junimatic.git "Junimatic" # 1 mod: Junimatic
   git clone -q https://github.com/NermNermNerm/QuestableTractor.git "Questable Tractor" # 1 mod: Questable Tractor
   git clone -q https://github.com/NeroYuki/StardewSurvivalProject.git "Stardew Survival Project" # 1 mod: Stardew Survival Project
   git clone -q https://github.com/nickmartin1ee7/RefundStaircases.git "Refund Staircases" # 1 mod: Refund Staircases
   git clone -q https://github.com/Nicoconot/NoSeedSpots_For-Stardew-Valley.git "No Seed Spots" # 1 mod: No Seed Spots
   git clone -q https://github.com/Nicoconot/StaircaseFound-for-StardewValley.git "Found Ladder and Shaft Notification Sound" # 1 mod: Found Ladder and Shaft Notification Sound
   git clone -q https://github.com/Nicoconot/WishingWell-For-Stardew-Valley.git "Wishing Well" # 1 mod: Wishing Well
   git clone -q https://github.com/nihilistzsche/FashionSenseOutfits.git "Fashion Sense Outfits" # 1 mod: Fashion Sense Outfits
   git clone -q https://github.com/ninthworld/HDSprites.git "HD Sprites" # 1 mod: HD Sprites
   git clone -q https://github.com/nithssh/SDVMods.git "~nithssh" # 2 mods: Custom Reminders, Recurring Reminders
   git clone -q https://github.com/nman130/Stardew-Mods.git "Menus Everywhere" # 1 mod: Menus Everywhere
   git clone -q https://github.com/nofilenamed/AnimalObserver.git "Animal Observer" # 1 mod: Animal Observer
   git clone -q https://github.com/nofilenamed/XPMultiplier.git "XPMultiplier" # 1 mod: XPMultiplier
   git clone -q https://github.com/nofilenamed/XPMultiplier.Space.git "XPMultiplier.Space" # 1 mod: XPMultiplier.Space
   git clone -q https://github.com/Nook001/StardewValley-BetterBuildingUpgrades.git "Better Building Upgrades" # 1 mod: Better Building Upgrades
   git clone -q https://github.com/Noomklaw/SV_BetterQualityMoreSeeds.git "Seed Maker Crop Quality" # 1 mod: Seed Maker Crop Quality
   git clone -q https://github.com/noriteway/StardewMods.git "Dwarf Scroll Prices" # 1 mod: Dwarf Scroll Prices
   git clone -q https://github.com/NormanPCN/StardewValleyMods.git "~NormanPCN" # 4 mods: Better Butterfly Hutch, Combat Controls Redux, Easier Monster Eradication, Longer Fence Life
   git clone -q https://github.com/notevenamouse/StardewMods.git "Fix TVs on Custom Farm Types" # 1 mod: Fix TVs on Custom Farm Types
   git clone -q https://github.com/notsemisu/VibrantPastoral.C.git "Vibrant Pastoral Redrawn → Vibrant Pastoral C#" # 1 mod: Vibrant Pastoral Redrawn → Vibrant Pastoral C#
   git clone -q https://github.com/NoxChimaera/StardewValleyTODO.git "Recipe Tracker" # 1 mod: Recipe Tracker
   git clone -q https://github.com/nrobinson12/StardewValleyMods.git "Static Inventory" # 1 mod: Static Inventory
   git clone -q https://github.com/NTaylor1993/FarmerHelper.git "Farmer Helper (Updated)" # 1 mod: Farmer Helper (Updated)
   git clone -q https://github.com/NTaylor1993/ToolSmartSwitch.git "Tool Smart Switch (Updated)" # 1 mod: Tool Smart Switch (Updated)
   git clone -q https://github.com/Nyayurin/DontStarve.git "Don't Starve" # 1 mod: Don't Starve
   git clone -q https://github.com/Nyayurin/MinuteTimeHelper.git "Minute Time Helper" # 1 mod: Minute Time Helper
   git clone -q https://github.com/Nykal145/StardewValleyMods.git "Luck Skill" # 1 mod: Luck Skill
   git clone -q https://github.com/nymvaline/StardewValley-AnkiStudyBreak.git "Anki Study Break" # 1 mod: Anki Study Break
   git clone -q https://github.com/oatgh2/StardewValley_UnlockBffHousesDoor.git "Unlock Best Friend's Door" # 1 mod: Unlock Best Friend's Door
   git clone -q https://github.com/OfficialRenny/InfiniteJunimoCartLives.git "Infinite Junimo Cart Lives" # 1 mod: Infinite Junimo Cart Lives
   git clone -q https://github.com/OfficialRenny/PrairieKingPrizes.git "Prairie King Prizes (Lootboxes)" # 1 mod: Prairie King Prizes (Lootboxes)
   git clone -q https://github.com/OfficialRenny/StardewVoidEffects.git "Void Effects" # 1 mod: Void Effects
   git clone -q https://github.com/OfficialTab/walk-through-trellis.git "Walk Through Trellis" # 1 mod: Walk Through Trellis
   git clone -q https://github.com/ofts-cqm/SDV_JojaExpress.git "Joja Express" # 1 mod: Joja Express
   git clone -q https://github.com/ofts-cqm/StardewValley-Agenda.git "Agenda" # 1 mod: Agenda
   git clone -q https://github.com/ofts-cqm/ToolAssembly.git "Tool Assembly" # 1 mod: Tool Assembly
   git clone -q https://github.com/OhWellMikell/Starksouls.git "Starksouls" # 1 mod: Starksouls
   git clone -q https://github.com/oliverpl/SprintingMod.git "Sprinting (oliverpl)" # 1 mod: Sprinting
   git clone -q https://github.com/OMEGAlinc/BuildingMenuAnywhere.git "Building Menu Anywhere" # 1 mod: Building Menu Anywhere
   git clone -q https://github.com/OMEGAlinc/FreezeTimeinEvents.git "Freeze Time in Events" # 1 mod: Freeze Time in Events
   git clone -q https://github.com/OMEGAlinc/OMC.git "OMC - OMEGAlinc's Commands" # 1 mod: OMC - OMEGAlinc's Commands
   git clone -q https://github.com/OMEGAlinc/QuickEatKeybinds.git "Quick Eat Keybinds" # 1 mod: Quick Eat Keybinds
   git clone -q https://github.com/Opalie/Wedding.git "Wedding Mod" # 1 mod: Wedding Mod
   git clone -q https://github.com/Ophaneom/Abilities-Experience-Bars.git "Abilities - Experience Bars" # 1 mod: Abilities - Experience Bars
   git clone -q https://github.com/Ophaneom/Let-Me-Rest.git "Let Me Rest!" # 1 mod: Let Me Rest!
   git clone -q https://github.com/Ophaneom/Survivalistic.git "Survivalistic" # 1 mod: Survivalistic
   git clone -q https://github.com/oranisagu/SDV-FarmAutomation.git "~oranisagu" # 2 mods: Farm Automation: Barn Door Automation, Farm Automation: Item Collector
   git clone -q https://github.com/otterbagel/sleepyhost.git "Sleepy Host" # 1 mod: Sleepy Host
   git clone -q https://github.com/OWODevelopers/OWO_StardewValley.git "OWO Integration - Stardew Valley" # 1 mod: OWO Integration - Stardew Valley
   git clone -q https://github.com/p-holodynski/StardewValleyMods.git "More Clocks" # 1 mod: More Clocks
   git clone -q https://github.com/pa4op/stardew-teleport.git "Cursor Teleport" # 1 mod: Cursor Teleport
   git clone -q https://github.com/Paradox355/SDVMods.git "Relationship Tracker" # 1 mod: Relationship Tracker
   git clone -q https://github.com/paritee/Paritee.StardewValley.Frameworks.git "~paritee" # 3 mods: Animal Birth Every Night, Paritee's Better Farm Animal Variety, Paritee's Treat Your Animals
   git clone -q https://github.com/Pathoschild/StardewMods.git "~Pathoschild" # 17 mods: Automate, Central Station, Chests Anywhere, Content Patcher, Crops Anytime Anywhere, Data Layers, Debug Mode, Fast Animations, Horse Flute Anywhere, Lookup Anything, No Debug Mode, Noclip Mode, Rotate Toolbar, Skip Intro, Small Beach Farm, The Long Night, Tractor Mod
   git clone -q https://github.com/paulsteele/Ferngill-Supply-And-Demand.git "Ferngill Simple Economy" # 1 mod: Ferngill Simple Economy
   git clone -q https://github.com/paulwrubel/stardew-mods.git "Automatic To-Do List" # 1 mod: Automatic To-Do List
   git clone -q https://github.com/PedroLucasMiguel/PredoSDVMods.git "Edit Stable Ownership" # 1 mod: Edit Stable Ownership
   git clone -q https://github.com/pepoluan/StackSplitRedux.git "Stack Split Redux" # 1 mod: Stack Split Redux
   git clone -q https://github.com/perkmi/Always-On-Server-for-Multiplayer.git "Unlimited Community Center" # 1 mod: Unlimited Community Center
   git clone -q https://github.com/Permamiss/Auto-Eat.git "Auto-Eat" # 1 mod: Auto-Eat
   git clone -q https://github.com/Pet-Slime/StardewValley.git "~Pet-Slime" # 11 mods: Archaeology Skill for 1.6, Archaeology Skill for Stardew 1.5, Athletics, Exp Control, Generic Mana Bar API, Scaring - Thieving Skill, Shovel Tool Upgrades, Spacecore Luck Skill, Total Level Affected by Spacecore Skill, Wizardry, Yet Another Cooking Skill
   git clone -q https://github.com/PhantomGamers/StardewPortChanger.git "Server Port Changer" # 1 mod: Server Port Changer
   git clone -q https://github.com/pheaux-prometheus/customcatalogues.git "Custom Catalogues and Dressers" # 1 mod: Custom Catalogues and Dressers
   git clone -q https://github.com/pheaux-prometheus/SDV-Mods.git "Mannequin Roulette" # 1 mod: Mannequin Roulette
   git clone -q https://github.com/PhillZitt/BetterBombs.git "Better Bombs" # 1 mod: Better Bombs
   git clone -q https://github.com/philosquare/EatInBackpack.git "Eat Food In Backpack Instantly" # 1 mod: Eat Food In Backpack Instantly
   git clone -q https://github.com/philosquare/SDVSkipFishingMinigame2.git "Skip Fishing Minigame 2" # 1 mod: Skip Fishing Minigame 2
   git clone -q https://github.com/phrasefable/StardewMods.git "Aggressive Acorns" # 1 mod: Aggressive Acorns
   git clone -q https://github.com/Platonymous/PlatoTK.git "PlatoTK" # 1 mod: PlatoTK
   git clone -q https://github.com/Platonymous/Stardew-Valley-Mods.git "~Platonymous" # 43 mods: Arcade 2048, Arcade Pong, Arcade Snake, ATM, Chess, Comics, Crop Extension, Custom Element Handler, Custom Farming Redux, Custom Farming Redux → CFAutomate, Custom Furniture, Custom Movies, Custom Music, Custom NPC, Custom Shirts, Custom TV, Custom Walls and Floors, Farm Hub, Ghost Town, Harp of Yoba Redux, HD Portraiture Plus, JoJaBan - Arcade Sokoban, Junimo Farm, Mod Updater, More Map Layers, No Soil Decay Redux, Notes, Pelican TTS, Plan Importer, Plato Warp Menu, Portraiture, PyTK, Ring of Fire, Scale Up, Seed Bag, Ship From Inventory, Sleep Worker, Speedster, Split Money, Swim (Almost) Anywhere, TMXL Map Toolkit, Ultiplayer, Visualize
   git clone -q https://github.com/pneuma163/StardewMods.git "~pneuma163" # 2 mods: Custom Elevator Sound, Where The Spreading Weeds Are
   git clone -q https://github.com/Pokachi/Stardew.git "~Pokachi" # 2 mods: Daily Boons and Banes, Experience Config
   git clone -q https://github.com/pomepome/AdjustableStaminaHealing.git "Adjustable Stamina Healing" # 1 mod: Adjustable Stamina Healing
   git clone -q https://github.com/pomepome/CasksOnGround.git "Casks on Ground" # 1 mod: Casks on Ground
   git clone -q https://github.com/pomepome/JoysOfEfficiency.git "Joys of Efficiency" # 1 mod: Joys of Efficiency
   git clone -q https://github.com/pomepome/PassableObjects.git "Passable Objects" # 1 mod: Passable Objects
   git clone -q https://github.com/pomepome/StackCheckFix.git "Stack Check Fix" # 1 mod: Stack Check Fix
   git clone -q https://github.com/pomepome/UsefulBombs.git "Useful Bombs" # 1 mod: Useful Bombs
   git clone -q https://github.com/poohnhi/PoohCore.git "PoohCore" # 1 mod: PoohCore
   git clone -q https://github.com/PrimmR/TrinketHotswap.git "Trinket Hotswap" # 1 mod: Trinket Hotswap
   git clone -q https://github.com/PrimmR/Turbo.git "Turbo" # 1 mod: Turbo
   git clone -q https://github.com/Prism-99/Aedenthorn-StardewValleyMods.git "~Prism-99" # 6 mods: Custom Picture Frames Redux, Farm Animal Harvest Helper Redux, Field Harvest Redux, Light Mod REDUX, Realistic Random Names Redux, Underground Secrets Redux
   git clone -q https://github.com/Prism-99/DailyTaskReportPlus.git "Daily Tasks Report Plus" # 1 mod: Daily Tasks Report Plus
   git clone -q https://github.com/Prism-99/FarmExpansion.git "Stardew Realty Framework" # 1 mod: Stardew Realty Framework
   git clone -q https://github.com/Prism-99/Hungry-Hungry-Animals.git "Hungry Hungry Animals" # 1 mod: Hungry Hungry Animals
   git clone -q https://github.com/Prism-99/murderdrone.git "Personal Combat Drone Redux" # 1 mod: Personal Combat Drone Redux
   git clone -q https://github.com/Prism-99/SDV-Location-Tuner.git "Location Tuner" # 1 mod: Location Tuner
   git clone -q https://github.com/Prism-99/stardewvalley-jwdred.git "Crab Net Redux" # 1 mod: Crab Net Redux
   git clone -q https://github.com/Prism-99/Think-n-Talk.git "Think-n-Talk" # 1 mod: Think-n-Talk
   git clone -q https://github.com/Prism-99/WhoLivesHere.git "Who Lives Here" # 1 mod: Who Lives Here
   git clone -q https://github.com/ProfeJavix/StardewValleyMods.git "~ProfeJavix" # 3 mods: OST Player, Tree Organizer, UI Helper
   git clone -q https://github.com/purrplingcat/CustomEmotes.git "Custom Emotes" # 1 mod: Custom Emotes
   git clone -q https://github.com/purrplingcat/DialogueEmotes.git "Dialogue Emotes" # 1 mod: Dialogue Emotes
   git clone -q https://github.com/purrplingcat/PurrplingMod.git "NPC Adventures" # 1 mod: NPC Adventures
   git clone -q https://github.com/purrplingcat/QuestFramework.git "Quest Framework" # 1 mod: Quest Framework
   git clone -q https://github.com/purrplingcat/StardewMods.git "~purrplingcat" # 2 mods: Custom Gift Dialogue, Quest Essentials
   git clone -q https://github.com/qixing-jk/QiXingAutoGrabTruffles.git "Auto-Grab Truffles" # 1 mod: Auto-Grab Truffles
   git clone -q https://github.com/QuangBM138/Find-Item.git "Find Item" # 1 mod: Find Item
   git clone -q https://github.com/QuangBM138/LadderOutline.git "Ladder Outline" # 1 mod: Ladder Outline
   git clone -q https://github.com/QuangBM138/Stack-Everything-Redux-Unofficial.git "Stack Everything Redux (Unofficial)" # 1 mod: Stack Everything Redux (Unofficial)
   git clone -q https://github.com/quicksilverfox/StardewMods.git "Empty Hands" # 1 mod: Empty Hands
   git clone -q https://github.com/Quipex/ExperienceUpdates.git "Experience Updates" # 1 mod: Experience Updates
   git clone -q https://github.com/Rafseazz/MovementOverhaul-Mod.git "Movement Overhaul" # 1 mod: Movement Overhaul
   git clone -q https://github.com/Rafseazz/Ridgeside-Village-Mod.git "Ridgeside Village" # 1 mod: Ridgeside Village
   git clone -q https://github.com/ragdollKB/stardew-magic-atlas.git "Magic Atlas" # 1 mod: Magic Atlas
   git clone -q https://github.com/RealSweetPanda/SaveAnywhereRedux.git "Save Anywhere Redux" # 1 mod: Save Anywhere Redux
   git clone -q https://github.com/recon88/Multi-Save-Continued.git "Multi Save - Continued" # 1 mod: Multi Save - Continued
   git clone -q https://github.com/rei2hu/stardewvalley-esp.git "Stardew Valley ESP" # 1 mod: Stardew Valley ESP
   git clone -q https://github.com/remybach/stardew-valley-forecaster.git "Forecaster" # 1 mod: Forecaster
   git clone -q https://github.com/remybach/stardew-valley-readersdigest.git "Readers Digest" # 1 mod: Readers Digest
   git clone -q https://github.com/rfht/MOTDMod.git "MOTD Mod" # 1 mod: MOTD Mod
   git clone -q https://github.com/ribeena/BusLocations.git "Ribeena's Vehicles" # 1 mod: Ribeena's Vehicles
   git clone -q https://github.com/ribeena/StardewValleyMods.git "Dynamic Bodies" # 1 mod: Dynamic Bodies
   git clone -q https://github.com/Richard2091/MapTeleport.git "Map Teleport" # 1 mod: Map Teleport
   git clone -q https://github.com/RichardJCai/stardew-valley-mods.git "Upgraded Horse" # 1 mod: Upgraded Horse
   git clone -q https://github.com/rikai/ABonafideSpecialBlueChicken.git "A Bonafide Special Blue Chicken" # 1 mod: A Bonafide Special Blue Chicken
   git clone -q https://github.com/rikai/AnimalSelector.git "Animal Selector" # 1 mod: Animal Selector
   git clone -q https://github.com/rikai/Grandfathers-Heirlooms.git "Grandfather's Heirlooms" # 1 mod: Grandfather's Heirlooms
   git clone -q https://github.com/robbiev/SilentScarecrows.git "Silent Scarecrows" # 1 mod: Silent Scarecrows
   git clone -q https://github.com/rockinrolla/ZoomMod.git "Zoom Mod _ Zoom Out Extreme" # 1 mod: Zoom Mod / Zoom Out Extreme
   git clone -q https://github.com/rokugin/BetterStardrops.git "Better Stardrops" # 1 mod: Better Stardrops
   git clone -q https://github.com/rokugin/ClintsBackstock.git "Clint's Backstock" # 1 mod: Clint's Backstock
   git clone -q https://github.com/rokugin/Collector.git "Collector" # 1 mod: Collector
   git clone -q https://github.com/rokugin/ColorfulFishPonds.git "Colorful Fish Ponds" # 1 mod: Colorful Fish Ponds
   git clone -q https://github.com/rokugin/CustomLocksUpdated.git "Custom Locks Updated" # 1 mod: Custom Locks Updated
   git clone -q https://github.com/rokugin/ExtraMapActions.git "Extra Map Actions" # 1 mod: Extra Map Actions
   git clone -q https://github.com/rokugin/PassableDescents.git "Passable Descents" # 1 mod: Passable Descents
   git clone -q https://github.com/rokugin/PerfectionExclusions.git "Perfection Exclusions" # 1 mod: Perfection Exclusions
   git clone -q https://github.com/rokugin/PetStore.git "Pet Store" # 1 mod: Pet Store
   git clone -q https://github.com/rokugin/RangedMachineInteractions.git "Ranged Machine Interactions" # 1 mod: Ranged Machine Interactions
   git clone -q https://github.com/rokugin/RobinBuildPosition.git "Robin Build Position" # 1 mod: Robin Build Position
   git clone -q https://github.com/rokugin/SafeLightningUpdated.git "Safe Lightning Updated" # 1 mod: Safe Lightning Updated
   git clone -q https://github.com/rokugin/StandaloneCraneGame.git "Standalone Crane Game" # 1 mod: Standalone Crane Game
   git clone -q https://github.com/RPGGO-AI/stardewAIRPG.git "RPGGO Stardew AI Mod" # 1 mod: RPGGO Stardew AI Mod
   git clone -q https://github.com/rtrox/LineSprinklersRedux.git "Line Sprinklers Redux (1.6 Compatible)" # 1 mod: Line Sprinklers Redux (1.6 Compatible)
   git clone -q https://github.com/ruggerbuns/MouseFishing.git "Mouse Fishing" # 1 mod: Mouse Fishing
   git clone -q https://github.com/RuiNtD/SVRichPresence.git "Discord Rich Presence" # 1 mod: Discord Rich Presence
   git clone -q https://github.com/rupak0577/FishDex.git "FishDex" # 1 mod: FishDex
   git clone -q https://github.com/rusunu/Enchantment.git "Better Enchantments" # 1 mod: Better Enchantments
   git clone -q https://github.com/RyanJesky/IncreaseCropGrowthPhase.git "Instantly Increase Crop Growth Phase" # 1 mod: Instantly Increase Crop Growth Phase
   git clone -q https://github.com/S1mmyy/StardewMods.git "~S1mmyy" # 2 mods: Item AutoTrasher, Skull Cavern Drill
   git clone -q https://github.com/sagittaeri/StardewValleyMods.git "Input Tools" # 1 mod: Input Tools
   git clone -q https://github.com/saikanyas/InteractiveEmotes.git "Interactive Emotes" # 1 mod: Interactive Emotes
   git clone -q https://github.com/Saitoue/Orchard.git "Orchard - Better Fruit Trees" # 1 mod: Orchard - Better Fruit Trees
   git clone -q https://github.com/Sakorona/SDVMods.git "~Sakorona" # 11 mods: Climates of Ferngill, Customizable Cart Redux, Dynamic Night Time, Ferngill Dynamic Rain, Happy Fish Jump, Lunar Disturbances, Reset Skull Caverns, Stardew Notifications, Time Reminder, Wash Away Forage, Weather Illnesses
   git clone -q https://github.com/Sandman534/Abilities-Experience-Bars.git "Abilities - Experience Bars 1.6" # 1 mod: Abilities - Experience Bars 1.6
   git clone -q https://github.com/sarahvloos/StardewMods.git "Always Show Bar Values" # 1 mod: Always Show Bar Values
   git clone -q https://github.com/Sasara2201/More-Weapons-SDV-Mod-.git "More Weapons" # 1 mod: More Weapons
   git clone -q https://github.com/Sasara2201/WitchPrincess.git "Harvest Moon Witch Princess Mod" # 1 mod: Harvest Moon Witch Princess Mod
   git clone -q https://github.com/scayze/MoreCollections.git "More Collections - Armory and Attire" # 1 mod: More Collections - Armory and Attire
   git clone -q https://github.com/scayze/multiprairie.git "Multiplayer Journey Of The Prairie King" # 1 mod: Multiplayer Journey Of The Prairie King
   git clone -q https://github.com/scriptsforweirdos/FestivalFeedback.git "Festival Feedback" # 1 mod: Festival Feedback
   git clone -q https://github.com/Seawolf87/DailyFarmPhoto.git "Daily Farm Photo" # 1 mod: Daily Farm Photo
   git clone -q https://github.com/seferoni/EnergyRework.git "Energy Rework" # 1 mod: Energy Rework
   git clone -q https://github.com/seferoni/FoodPoisoning.git "Food Poisoning" # 1 mod: Food Poisoning
   git clone -q https://github.com/seferoni/HealthRework.git "Health Rework" # 1 mod: Health Rework
   git clone -q https://github.com/seichen/Stardew-Stop-Flower-Harvests.git "Stop Flower Harvests" # 1 mod: Stop Flower Harvests
   git clone -q https://github.com/Seren-L/ShopTabs.git "Shop Tabs" # 1 mod: Shop Tabs
   git clone -q https://github.com/sergiomadd/StardewValleyMods.git "~sergiomadd" # 2 mods: Chest Preview, Connected Fences
   git clone -q https://github.com/setsevireon/LazyFarmer.git "Lazy Farmer" # 1 mod: Lazy Farmer
   git clone -q https://github.com/SevenDespised/WriteDownYourPlan.git "Write Down Your Plan" # 1 mod: Write Down Your Plan
   git clone -q https://github.com/shailalias/NoRumbleHorse.git "Updated No Rumble Horse" # 1 mod: Updated No Rumble Horse
   git clone -q https://github.com/Shalankwa/SDV_Mods.git "Warp to Friends" # 1 mod: Warp to Friends
   git clone -q https://github.com/shankencedric/StardewValleyMods-stardewsteak.git "More Multiplayer Info (1.6 Updated)" # 1 mod: More Multiplayer Info (1.6 Updated)
   git clone -q https://github.com/shekurika/DailySpecialOrders.git "Daily Special Orders" # 1 mod: Daily Special Orders
   git clone -q https://github.com/shen02/StardewValley-Harvest-Day-Calendar.git "Harvest Day Calendar" # 1 mod: Harvest Day Calendar
   git clone -q https://github.com/shivaGuy/StardewMods.git "~shivaGuy" # 3 mods: Better Trash Can, Custom Trash Can, Farm Animal Choices
   git clone -q https://github.com/Shivion/StardewValleyMods.git "Filtered Chest Hopper" # 1 mod: Filtered Chest Hopper
   git clone -q https://github.com/Shockah/Stardew-Valley-Mods.git "~Shockah" # 18 mods: Don't Stop Me Now, Early Ginger Island, Flexible Sprinklers, Hats on Hats, Hibernation, In a Heartbeat, Junimo Warp, Kokoro, Machine Status, Maximize Fix, Mine Tweaks, Please Gift Me In Person, Predictable Retaining Soil, Project Fluent, Safe Lightning Redux, Season Affixes, Stack Size Changer, XP Display
   git clone -q https://github.com/sikker/SpouseStuff.git "Spouse Stuff" # 1 mod: Spouse Stuff
   git clone -q https://github.com/silentoak/StardewMods.git "~silentoak" # 2 mods: Quality Products, Quality Products → Auto Quality Patch
   git clone -q https://github.com/silicon-git/GVChildrenSMAPI.git "Growing Valley - Child NPCs" # 1 mod: Growing Valley - Child NPCs
   git clone -q https://github.com/silicon-git/NPCTokens.git "NPC Tokens" # 1 mod: NPC Tokens
   git clone -q https://github.com/simonbru/WarpAnimals.git "Warp Animals" # 1 mod: Warp Animals
   git clone -q https://github.com/SimonK1122/More-Lively-Sewer-Overhaul-code-patches.git "More Lively Sewer Overhaul" # 1 mod: More Lively Sewer Overhaul
   git clone -q https://github.com/SimonK1122/WitchSwampOverhaulPatches.git "Distant Lands - A Small Witch Swamp Expansion" # 1 mod: Distant Lands - A Small Witch Swamp Expansion
   git clone -q https://github.com/SinZ163/StardewMods.git "~SinZ163" # 6 mods: Automate Chests, Profiler, SinZational Several Spouse Spots, SinZational Shared Spaces, SinZational Speedy Solutions, SinZational Spending Services
   git clone -q https://github.com/siweipancc/InstantAnimals.git "Instant Animals 1.6" # 1 mod: Instant Animals 1.6
   git clone -q https://github.com/siweipancc/StardewMods.git "No Crows Revisited" # 1 mod: No Crows Revisited
   git clone -q https://github.com/siweipancc/TreeTransplant.git "Tree Transplant (Fix)" # 1 mod: Tree Transplant (Fix)
   git clone -q https://github.com/skellady/Sunberry-Village.git "Sunberry Village" # 1 mod: Sunberry Village
   git clone -q https://github.com/skistad-studios/BetterJunimosForestryRedux.git "Better Junimos Forestry Redux" # 1 mod: Better Junimos Forestry Redux
   git clone -q https://github.com/skuldomg/catGifts.git "Cat Gifts" # 1 mod: Cat Gifts
   git clone -q https://github.com/skuldomg/freeDusty.git "Free Dusty" # 1 mod: Free Dusty
   git clone -q https://github.com/skuldomg/petChoicePerks.git "Pet Choice Perks" # 1 mod: Pet Choice Perks
   git clone -q https://github.com/skuldomg/priceDrops.git "Price Drops" # 1 mod: Price Drops
   git clone -q https://github.com/Sleepingoff/DialogueExpander.git "Dialogue Expander" # 1 mod: Dialogue Expander
   git clone -q https://github.com/slimerrain/stardew-mods.git "Uncle Iroh Approved Tea" # 1 mod: Uncle Iroh Approved Tea
   git clone -q https://github.com/SlivaStari/BuildableForge.git "Buildable Enchanting Forge" # 1 mod: Buildable Enchanting Forge
   git clone -q https://github.com/SlivaStari/CombineManyRings.git "Combine Many Rings" # 1 mod: Combine Many Rings
   git clone -q https://github.com/SlivaStari/ManyEnchantments.git "Many Enchantments" # 1 mod: Many Enchantments
   git clone -q https://github.com/slothsoft/stardew-challenger.git "Challenger" # 2 mods: Challenger, Challenger → Automate
   git clone -q https://github.com/slothsoft/stardew-informant.git "Informant" # 1 mod: Informant
   git clone -q https://github.com/slserpent/Stardew-Valley-Mods.git "~slserpent" # 2 mods: Better Collection Sorting, Better Weapon Stats
   git clone -q https://github.com/Smapifan/TMXLoader.git "TMXL Map Toolkit (Unofficial Update)" # 1 mod: TMXL Map Toolkit (Unofficial Update)
   git clone -q https://github.com/Smapifan/Werewolf-in-the-Valley.git "Werewolf in the Valley (Unofficial Update)" # 1 mod: Werewolf in the Valley (Unofficial Update)
   git clone -q https://github.com/Smoked-Fish/AnythingAnywhere.git "Build and Place Anything Anywhere" # 1 mod: Build and Place Anything Anywhere
   git clone -q https://github.com/Smoked-Fish/ParticleFramework.git "Particle Framework" # 1 mod: Particle Framework
   git clone -q https://github.com/snowe2010/starbound-mods.git "Crops Watered Indicator" # 1 mod: Crops Watered Indicator
   git clone -q https://github.com/SoapStuff/Remote-Fridge-Storage.git "Remote Fridge Storage" # 1 mod: Remote Fridge Storage
   git clone -q https://github.com/SocietyNiu/VocabValley.git "Vocab Valley" # 1 mod: Vocab Valley
   git clone -q https://github.com/SolusCleansing/RentedToolsImproved.git "Rented Tools Improved" # 1 mod: Rented Tools Improved
   git clone -q https://github.com/Sonozuki/StardewMods.git "~Sonozuki" # 9 mods: Better 10 Hearts, Better Crab Pots, Better Mixed Seeds, Better Rarecrows, Clean Cellar, Console Chat, Customize Cursor Sprite, Miller Time, More Grass
   git clone -q https://github.com/sophiesalacia/StardewMods.git "~sophiesalacia" # 12 mods: Calcifer, Car Warp, Colored Crystalariums, Configurable Bundle Costs, Configurable Luck, Configurable Special Orders Unlock, Joja's Dark Secret, Lasting Conversation Topics, Mines Tokens, New Years Eve → Fireworks, Splash Text, Sturdy Saplings
   git clone -q https://github.com/sorrylate/SaveGameOptions.git "Save Game Options" # 1 mod: Save Game Options
   git clone -q https://github.com/sorrylate/Stardew-Valley-Mod.git "Tool Upgrade Delivery" # 1 mod: Tool Upgrade Delivery
   git clone -q https://github.com/SourceZh/MultiplayerIdle.git "Multiplayer Idle" # 1 mod: Multiplayer Idle
   git clone -q https://github.com/SourceZh/VirtualKeyboard.git "Virtual Keyboard" # 1 mod: Virtual Keyboard
   git clone -q https://github.com/spacechase0/BiggerBackpack.git "Bigger Backpack" # 1 mod: Bigger Backpack
   git clone -q https://github.com/spacechase0/Bow.git "Bow" # 1 mod: Bow
   git clone -q https://github.com/spacechase0/Cobalt.git "Cobalt" # 1 mod: Cobalt
   git clone -q https://github.com/spacechase0/ColorfulChests.git "Colorful Chests (spacechase0)" # 1 mod: Colorful Chests
   git clone -q https://github.com/spacechase0/CustomCrops.git "Custom Crops" # 1 mod: Custom Crops
   git clone -q https://github.com/spacechase0/CustomFarmTypes.git "Custom Farm Types" # 1 mod: Custom Farm Types
   git clone -q https://github.com/spacechase0/MapImageExporter.git "Map Image Export" # 1 mod: Map Image Export
   git clone -q https://github.com/spacechase0/SeedCatalogue.git "Seed Catalogue" # 1 mod: Seed Catalogue
   git clone -q https://github.com/spacechase0/ShipAnywhere.git "Ship Anywhere" # 1 mod: Ship Anywhere
   git clone -q https://github.com/spacechase0/StardewEditor.git "Stardew Editor" # 1 mod: Stardew Editor
   git clone -q https://github.com/spacechase0/StardewValleyMods.git "~spacechase0" # 74 mods: A Quality Mod, Advanced Social Interactions, Angry Seagulls, Animal Social Menu, Another Hunger Mod, Backstory Questions Framework, Better Meteorites, Better Shop Menu, Bigger Craftables, Blahaj Blast, Bug Net, Capstone Professions, Carry Chest, Combat Level Damage Scaler, Console Code, Content Code, Content Patcher Animations, Controller Radial Keybindings, Cooking Skill, Custom Critters, Custom NPC Fixes, Customize Exterior, Dimensional Pockets, Displays, Dynamic Game Assets, Dynamic Game Assets → DGA Automate, Experience Bars, Extended Reach, Flower Color Picker, Flower Rain, Generic Mod Config Menu, Giant Omelet, Gradient Hair Colors, Hybrid Crop Engine, Json Assets, Jump Over, Junimos Accept Cash, Literally Can't Even, Magic, Mana Bar, Monster Slayer Anywhere, Monsters - The Framework, Moon Misadventures, More Buildings, More Giant Crops, More Grass Starters, More Rings, Multi Fertilizer, New Game Plus, Object Time Left, Persistent Mines, Preexisting Relationship, Profit Calculator, Pyromancer's Journey, Qi Chests, Random Cats in Events, Realtime Minimap, Responsive Knockback, Rush Orders, Satchels, Sharing is Caring, Sizable Fish, Skill Training, Sleepy Eye, SpaceCore, Spenny, Spenny Deluxe, Spenny Lite, Statue of Generosity, Super Hopper, Surfing Festival, Theft of the Winter Star, Three-Heart Dance Partner, Throwable Axe
   git clone -q https://github.com/spacechase0/StardewValleyMP.git "Makeshift Multiplayer" # 1 mod: Makeshift Multiplayer
   git clone -q https://github.com/spacechase0/TestModWorkflow.git "Test Mod for Automated Workflows" # 1 mod: Test Mod for Automated Workflows
   git clone -q https://github.com/spacechase0/ToolGeodes.git "Tool Geodes" # 1 mod: Tool Geodes
   git clone -q https://github.com/spacechase0/WeaponReskinner.git "Weapon Reskinner" # 1 mod: Weapon Reskinner
   git clone -q https://github.com/spajus/stardew-valley-fast-loads.git "Fast Loads" # 1 mod: Fast Loads
   git clone -q https://github.com/Speshkitty/CustomiseChildBedroom.git "Customise Child Bedroom" # 1 mod: Customise Child Bedroom
   git clone -q https://github.com/Speshkitty/FarmToTableReadyMeals.git "Farm to Table Ready Meals" # 1 mod: Farm to Table Ready Meals
   git clone -q https://github.com/Speshkitty/FishInfo.git "Fish Info" # 1 mod: Fish Info
   git clone -q https://github.com/Speshkitty/QueenOfSauceRandomiser.git "Queen of Sauce Randomizer" # 1 mod: Queen of Sauce Randomizer
   git clone -q https://github.com/Speshkitty/SlimeWaterFiller.git "Slime Water Filler" # 1 mod: Slime Water Filler
   git clone -q https://github.com/spicykai/StardewValley-Mods.git "Tileman Challenge" # 1 mod: Tileman Challenge
   git clone -q https://github.com/Spiderbuttons/ArbitraryTilesheetAccess.git "Arbitrary Tilesheet Access" # 1 mod: Arbitrary Tilesheet Access
   git clone -q https://github.com/Spiderbuttons/AutoSorterBuilding.git "Auto-Sorter Building" # 1 mod: Auto-Sorter Building
   git clone -q https://github.com/Spiderbuttons/BebberBobbers.git "Bebber Bobbers" # 1 mod: Bebber Bobbers
   git clone -q https://github.com/Spiderbuttons/BETAS.git "Button's Extra Trigger Action Stuff" # 1 mod: Button's Extra Trigger Action Stuff
   git clone -q https://github.com/Spiderbuttons/ButtonsExtraBooks.git "Button's Extra Books" # 1 mod: Button's Extra Books
   git clone -q https://github.com/Spiderbuttons/CrossModConfigTokens.git "Cross-Mod Compatibility Tokens" # 1 mod: Cross-Mod Compatibility Tokens
   git clone -q https://github.com/Spiderbuttons/CustomMuseumFramework.git "Custom Museum Framework (CMF)" # 1 mod: Custom Museum Framework (CMF)
   git clone -q https://github.com/Spiderbuttons/CustomScheduleKeys.git "Custom Schedule Keys" # 1 mod: Custom Schedule Keys
   git clone -q https://github.com/Spiderbuttons/ExtendedSpecialCurrencyDisplay.git "Extended Qi Gem and Golden Walnut Display" # 1 mod: Extended Qi Gem and Golden Walnut Display
   git clone -q https://github.com/Spiderbuttons/FrameworkForEmotes.git "Framework for Emote Mods" # 1 mod: Framework for Emote Mods
   git clone -q https://github.com/Spiderbuttons/Graveyards.git "Graveyards" # 1 mod: Graveyards
   git clone -q https://github.com/Spiderbuttons/MirrorMode.git "Mirror Mode" # 1 mod: Mirror Mode
   git clone -q https://github.com/Spiderbuttons/Soundboard.git "Soundboard" # 1 mod: Soundboard
   git clone -q https://github.com/Spiderbuttons/SpecialPowerUtilities.git "Special Power Utilities" # 1 mod: Special Power Utilities
   git clone -q https://github.com/sqbr/GetGS.git "Gender Setter" # 1 mod: Gender Setter
   git clone -q https://github.com/StarAmy/BreedingOverhaul.git "Breeding Overhaul" # 1 mod: Breeding Overhaul
   git clone -q https://github.com/stardew-access/stardew-access.git "Stardew Access" # 1 mod: Stardew Access
   git clone -q https://github.com/Stardew-Valley-Modding/Bookcase.git "Bookcase" # 1 mod: Bookcase
   git clone -q https://github.com/Stardew-Valley-Modding/HealthBars.git "Health Bars (Darkhax)" # 1 mod: Health Bars (Darkhax)
   git clone -q https://github.com/Stardew-Valley-Modding/PriceTooltips.git "Price Tooltips" # 1 mod: Price Tooltips
   git clone -q https://github.com/Stardew-Valley-Modding/Snail-Mail.git "Snail Mail" # 1 mod: Snail Mail
   git clone -q https://github.com/StardewConfigFramework/StardewConfigMenu.git "Stardew Config Menu" # 1 mod: Stardew Config Menu
   git clone -q https://github.com/stellarashes/SDVMods.git "Auto Watering (stellarashes)" # 1 mod: Auto Watering (stellarashes)
   git clone -q https://github.com/StephenKairos/Teban100-StardewMods.git "~StephenKairos" # 2 mods: Auto Gate, Rope Bridge
   git clone -q https://github.com/StephHoel/ModsStardewValley.git "~StephHoel" # 3 mods: Add Money, Auto Water, Configure Machine Speed
   git clone -q https://github.com/steven-kraft/StardewDiscord.git "Stardew Discord" # 1 mod: Stardew Discord
   git clone -q https://github.com/Stingrayss/StardewValley.git "~Stingrayss" # 2 mods: Community Center Anywhere, Telephone Purchasing
   git clone -q https://github.com/stokastic/AnimalChooser.git "Animal Chooser" # 1 mod: Animal Chooser
   git clone -q https://github.com/stokastic/CoopGrabber.git "Deluxe Auto-Grabber" # 1 mod: Deluxe Auto-Grabber
   git clone -q https://github.com/stokastic/PrismaticTools.git "Prismatic Tools" # 1 mod: Prismatic Tools
   git clone -q https://github.com/stokastic/SmartCursor.git "Smart Cursor" # 1 mod: Smart Cursor
   git clone -q https://github.com/stoobinator/Stardew-Valley-Fertilizer-Anytime.git "Fertilizer Anytime" # 1 mod: Fertilizer Anytime
   git clone -q https://github.com/stoobinator/Stardew-Valley-Grace-Period.git "Crop Grace Period" # 1 mod: Crop Grace Period
   git clone -q https://github.com/strobel1ght/Personal-Anvil.git "Personal Anvil" # 1 mod: Personal Anvil
   git clone -q https://github.com/strobel1ght/ShroomSpotterRedux.git "Shroom Spotter Redux" # 1 mod: Shroom Spotter Redux
   git clone -q https://github.com/Strrato/StardewValleyRunningKey.git "Running Key" # 1 mod: Running Key
   git clone -q https://github.com/Strrato/StardewWrapMultiplayer.git "Warp Multiplayer" # 1 mod: Warp Multiplayer
   git clone -q https://github.com/su226/StardewValleyMods.git "Stay Up" # 1 mod: Stay Up
   git clone -q https://github.com/SummerFleur2997/StardewMods.git "~SummerFleur2997" # 5 mods: Better Retaining Soils, Bigger Containers, Convenient Chests Unofficial, Skull Cavern Floor Capping, Why Not Jump In That Mine Shaft
   git clone -q https://github.com/sunsst/Stardew-Valley-IPv6.git "IPv6" # 1 mod: IPv6
   git clone -q https://github.com/super-aardvark/AardvarkMods-SDV.git "~super-aardvark" # 2 mods: Anti-Social NPCs, SDV Game of Life
   git clone -q https://github.com/supercam19/DPS-Stat.git "DPS Stat" # 1 mod: DPS Stat
   git clone -q https://github.com/SvardXI/HAS_Tweaks.git "Health and Stamina Tweaks" # 1 mod: Health and Stamina Tweaks
   git clone -q https://github.com/Swampen/stardew-valley-mods.git "~Swampen" # 2 mods: The Queen Of Sauce Reminder, Traveling Cart Reminder
   git clone -q https://github.com/SwizzyStudios/SV-SwizzyMeads.git "Swizzy Meads" # 1 mod: Swizzy Meads
   git clone -q https://github.com/SymaLoernn/Stardew_HatsOnPetsPlus.git "Hats on Pets Plus" # 1 mod: Hats on Pets Plus
   git clone -q https://github.com/tadfoster/StardewValleyMods.git "Day Limiter" # 1 mod: Day Limiter
   git clone -q https://github.com/taggartaa/AutoAnimalDoors.git "Auto Animal Doors" # 1 mod: Auto Animal Doors
   git clone -q https://github.com/tbonehunter/resist.git "No Collision" # 1 mod: No Collision
   git clone -q https://github.com/Tbonetomtom/StardewMods.git "Money Management Mod" # 1 mod: Money Management Mod
   git clone -q https://github.com/TechnicalityCreations/StardewMods.git "~TechnicalityCreations" # 3 mods: Hay Subscription, Spoiler-Free Gift Indicator, Yearly Night Market Pearl
   git clone -q https://github.com/TehPers/StardewValleyMods.git "~TehPers" # 5 mods: Shroom Spotter, Stardew Content Compatibility Layer (SCCL), Teh's Festive Slimes, Teh's Fishing Overhaul, TehCore
   git clone -q https://github.com/tekladis/sv-rollback.git "Rollback" # 1 mod: Rollback
   git clone -q https://github.com/telshin/twitch-for-stardew.git "Twitch in Stardew Valley" # 1 mod: Twitch in Stardew Valley
   git clone -q https://github.com/Teoshen/StardewValleyMods.git "Custom Hot Springs Regeneration Rate" # 1 mod: Custom Hot Springs Regeneration Rate
   git clone -q https://github.com/tesla1889tv/ControlValleyMod.git "Control Valley" # 1 mod: Control Valley
   git clone -q https://github.com/tfsJoe/CharaChatSV.git "Chara.Chat" # 1 mod: Chara.Chat
   git clone -q https://github.com/thakyZ/StardewValleyMods.git "No Pause When Inactive - Global" # 1 mod: No Pause When Inactive - Global
   git clone -q https://github.com/thakyZ/zDailyIncrease.git "zDailyIncrease" # 1 mod: zDailyIncrease
   git clone -q https://github.com/ThatNorthernMonkey/AdjustArtisanPrices.git "Adjust Artisan Prices" # 1 mod: Adjust Artisan Prices
   git clone -q https://github.com/ThatNorthernMonkey/HarvestWithScythe.git "Harvest With Scythe" # 1 mod: Harvest With Scythe
   git clone -q https://github.com/ThatNorthernMonkey/NoSoilDecay.git "No Soil Decay" # 1 mod: No Soil Decay
   git clone -q https://github.com/thatnzguy/WindowResize.git "Window Resize" # 1 mod: Window Resize
   git clone -q https://github.com/thebeardphantom/Stardew-Mods.git "Timescale Display" # 1 mod: Timescale Display
   git clone -q https://github.com/theghost99/StardewUnattendedServer.git "Stardew Unattended Server Mod" # 1 mod: Stardew Unattended Server Mod
   git clone -q https://github.com/thejcannon/Stardew-FoodBuffStacking.git "Food Buff Stacking" # 1 mod: Food Buff Stacking
   git clone -q https://github.com/TheMightyAmondee/BetterMeowmere.git "Better Meowmere" # 1 mod: Better Meowmere
   git clone -q https://github.com/TheMightyAmondee/CustomDeathPenaltyPlus.git "Custom Death Penalty Plus" # 1 mod: Custom Death Penalty Plus
   git clone -q https://github.com/TheMightyAmondee/CustomTokens.git "Custom Tokens" # 1 mod: Custom Tokens
   git clone -q https://github.com/TheMightyAmondee/EasierSpecialOrders.git "Easier Special Orders" # 1 mod: Easier Special Orders
   git clone -q https://github.com/TheMightyAmondee/EventLimiter.git "Event Limiter" # 1 mod: Event Limiter
   git clone -q https://github.com/TheMightyAmondee/GoodbyeAmericanEnglish.git "Goodbye American English" # 1 mod: Goodbye American English
   git clone -q https://github.com/TheMightyAmondee/Pravoloxinone.git "Pravoloxinone" # 1 mod: Pravoloxinone
   git clone -q https://github.com/TheMightyAmondee/Shoplifter.git "Shoplifter" # 1 mod: Shoplifter
   git clone -q https://github.com/TheMightyAmondee/SkullCavernToggle.git "Skull Cavern Toggle" # 1 mod: Skull Cavern Toggle
   git clone -q https://github.com/TheMightyAmondee/WeatherTotems.git "Weather Totems Expanded" # 1 mod: Weather Totems Expanded
   git clone -q https://github.com/thespbgamer/LovedLabelsRedux.git "Loved Labels Redux" # 1 mod: Loved Labels Redux
   git clone -q https://github.com/thespbgamer/ZoomLevel.git "Zoom Level" # 1 mod: Zoom Level
   git clone -q https://github.com/TheThor59/StardewMods.git "~TheThor59" # 2 mods: Enemy Health Bars, Hoe and Water Direction
   git clone -q https://github.com/TheUltimateNuke/StardewValleyMods.git "No Mine Exit Dialogue" # 1 mod: No Mine Exit Dialogue
   git clone -q https://github.com/TheUnproductive/KidAutoPetter.git "Kid Auto Petter" # 1 mod: Kid Auto Petter
   git clone -q https://github.com/thiagomasson/Sprinting.git "Sprinting (thiagomasson)" # 1 mod: Sprinting
   git clone -q https://github.com/thimadera/StardewMods.git "~thimadera" # 3 mods: Real Clock, Stack Everything Redux, Stack Everything Redux (Unofficial)
   git clone -q https://github.com/Tlitookilakin/AeroCore.git "AeroCore" # 1 mod: AeroCore
   git clone -q https://github.com/tlitookilakin/BetterBeehouses.git "Better Beehouses" # 1 mod: Better Beehouses
   git clone -q https://github.com/tlitookilakin/FarmWarpsPatch.git "Farm Warps Patch" # 1 mod: Farm Warps Patch
   git clone -q https://github.com/tlitookilakin/HappyHomeDesigner.git "Happy Home Designer" # 1 mod: Happy Home Designer
   git clone -q https://github.com/tlitookilakin/MachineUpgradeSystem.git "Machine Upgrade System" # 1 mod: Machine Upgrade System
   git clone -q https://github.com/tlitookilakin/MessyCrops.git "Messy Crops" # 1 mod: Messy Crops
   git clone -q https://github.com/tlitookilakin/ProfessionBooks.git "Profession Books" # 1 mod: Profession Books
   git clone -q https://github.com/tlitookilakin/WarpnetDeepwoods.git "Deep Woods Warp Network Integration" # 1 mod: Deep Woods Warp Network Integration
   git clone -q https://github.com/tlitookilakin/WarpNetwork.git "Warp Network" # 1 mod: Warp Network
   git clone -q https://github.com/TMThong/Stardew-Mods.git "~TMThong" # 4 mods: Find Object Mod, Mini Tool NPC, Multiplayer Mod, No Warp Delay
   git clone -q https://github.com/Tocseoj/StardewValleyMods.git "~Tocseoj" # 3 mods: Big Crop Bonus, Ladder Light, Mossy Tree Bubble
   git clone -q https://github.com/Tondorian/StardewValleyMods.git "Skill Prestige for Love of Cooking" # 1 mod: Skill Prestige for Love of Cooking
   git clone -q https://github.com/Torsang/FarmingToolsPatch.git "Farming Tools Patch" # 1 mod: Farming Tools Patch
   git clone -q https://github.com/Traktori7/StardewValleyMods.git "~Traktori7" # 6 mods: Fix Categories in Recipes, Industrial Furnace, Industrial Furnace for Automate, Quality Scrubber, Quality Scrubber for Automate, Show Birthdays
   git clone -q https://github.com/treehats/WeeklyBreakReminder.git "Weekly Break Reminder" # 1 mod: Weekly Break Reminder
   git clone -q https://github.com/trienow/Stardew-CraftPriority.git "Craft Priority" # 1 mod: Craft Priority
   git clone -q https://github.com/truman-world/puppy-stardew-server.git "Auto Hide Host" # 1 mod: Auto Hide Host
   git clone -q https://github.com/trunip190/MilkVillagers.git "Milk the Villagers" # 1 mod: Milk the Villagers
   git clone -q https://github.com/TSlex/StardewValley.git "~TSlex" # 2 mods: Item Research 'n' Spawn, Time Skipper
   git clone -q https://github.com/tstaples/CleanFarm.git "Clean Farm" # 1 mod: Clean Farm
   git clone -q https://github.com/tstaples/GiftTasteHelper.git "Gift Taste Helper" # 1 mod: Gift Taste Helper
   git clone -q https://github.com/tstaples/StackSplitX.git "StackSplitX" # 1 mod: StackSplitX
   git clone -q https://github.com/tsubasamoon13-ux/ExtraEventCommands.git "Extra Event Commands" # 1 mod: Extra Event Commands
   git clone -q https://github.com/TwinBuilderOne/StardewValleyMods.git "More Bundles" # 1 mod: More Bundles
   git clone -q https://github.com/TWT233/FishingTweaks.git "Fishing Tweaks" # 1 mod: Fishing Tweaks
   git clone -q https://github.com/tylergibbs2/StardewValleyMods.git "~tylergibbs2" # 9 mods: Battle Royalley - Year 2, Cold Pets, Default Farmer, Junimo Boy, Live Progress Bar, Seasonal Save Slots, Stardew Hitboxes, Stardew Nametags, Stardew Valley - Roguelike
   git clone -q https://github.com/TyoAtrosa/TreeShaker.git "Tree Shaker" # 1 mod: Tree Shaker
   git clone -q https://github.com/Underscore76/ClayMap.git "Clay Map" # 1 mod: Clay Map
   git clone -q https://github.com/Underscore76/NoDebrisWeather.git "No Debris Weather" # 1 mod: No Debris Weather
   git clone -q https://github.com/Underscore76/SDVPracticeMod.git "Speedrun Practice Mod" # 1 mod: Speedrun Practice Mod
   git clone -q https://github.com/unidarkshin/Stardew-Mods.git "Infinite Inventory" # 1 mod: Infinite Inventory
   git clone -q https://github.com/UnkLegacy/QiSprinklers.git "Qi Sprinklers" # 1 mod: Qi Sprinklers
   git clone -q https://github.com/Unremarkable/Stardew-Valley-Multiplayer-Sell-Price-Fix.git "Multiplayer Sell Price Fix" # 1 mod: Multiplayer Sell Price Fix
   git clone -q https://github.com/urbanyeti/stardew-better-friendship.git "Better Friendship" # 1 mod: Better Friendship
   git clone -q https://github.com/urbanyeti/stardew-better-ranching.git "Better Ranching" # 1 mod: Better Ranching
   git clone -q https://github.com/Uwazouri/ExpandedFridge.git "Expanded Fridge" # 1 mod: Expanded Fridge
   git clone -q https://github.com/vabrell/sdw-seed-maker-mod.git "Seed Maker - Better Quality More Seeds" # 1 mod: Seed Maker - Better Quality More Seeds
   git clone -q https://github.com/vaindil/sdv-moodfix.git "MoodFix" # 1 mod: MoodFix
   git clone -q https://github.com/valruno/ChooseRandomFarmEvent.git "Choose Random Farm Event" # 1 mod: Choose Random Farm Event
   git clone -q https://github.com/veleek/IdlePause.git "Idle Pause" # 1 mod: Idle Pause
   git clone -q https://github.com/Veniamin-Arefev/StardewMods.git "Adventurers Guild Multiplayer Shared Kills" # 1 mod: Adventurers Guild Multiplayer Shared Kills
   git clone -q https://github.com/vgperson/CommunityCenterHelper.git "Community Center Helper" # 1 mod: Community Center Helper
   git clone -q https://github.com/vgperson/RangedTools.git "Ranged Tools" # 1 mod: Ranged Tools
   git clone -q https://github.com/Videogamers0/SDV-CombineMachines.git "Combine Machines" # 1 mod: Combine Machines
   git clone -q https://github.com/Videogamers0/SDV-ItemBags.git "Item Bags" # 1 mod: Item Bags
   git clone -q https://github.com/Videogamers0/SDV-MachineAugmentors.git "Machine Augmentors" # 1 mod: Machine Augmentors
   git clone -q https://github.com/vincebel7/YearRoundCrops.git "Year-Round Crops" # 1 mod: Year-Round Crops
   git clone -q https://github.com/voltaek/StardewMods.git "~voltaek" # 2 mods: Colored Honey Labels, Honey Harvest Sync
   git clone -q https://github.com/WarpWorld/CCPack-PC-StardewValley.git "Crowd Control" # 1 mod: Crowd Control
   git clone -q https://github.com/wbc666/NewGreenhouseUpgrades.git "New Greenhouse Upgrades" # 1 mod: New Greenhouse Upgrades
   git clone -q https://github.com/WDEvanson/CGEvents.git "Stardew CG Events" # 1 mod: Stardew CG Events
   git clone -q https://github.com/weizinai/StardewValleyMods.git "~weizinai" # 16 mods: Active Menu Anywhere, Auto Break Geode, Auto Refresh Mine Shaft, Better Cabin, Custom Cabin Fix, Custom Machine Experience, Fast Control Input, Free Lock, Friendship Decay Modify, Help Wanted Redux For 1.6, Lazy Mod, Multiplayer Mod Limit, Pi Core, Ready Check Kick, Save Mod Info, Spectator Mode
   git clone -q https://github.com/Wellbott/StardewValleyMods.git "~Wellbott" # 3 mods: Slim Hutch, Smaller Fish Ponds, Stabbing Sword Special
   git clone -q https://github.com/WhaleK233/Red-Panda-Bazaar.git "Red Panda Bazaar" # 1 mod: Red Panda Bazaar
   git clone -q https://github.com/WhiteMinds/mod-sv-autofish.git "Auto Fish" # 1 mod: Auto Fish
   git clone -q https://github.com/WilliamDOliveira/mods-for-stardew-valley.git "~WilliamDOliveira" # 4 mods: Auto Catch Fish, Bomb Drop, I Dig Where I Want, Watering Can Upgrade
   git clone -q https://github.com/wilsoncwc/StardewValleyMods.git "Fixed Ginger Island Beach Garden Pots" # 1 mod: Fixed Ginger Island Beach Garden Pots
   git clone -q https://github.com/Windmill-City/FishingTrainer.git "Fishing Trainer" # 1 mod: Fishing Trainer
   git clone -q https://github.com/Windmill-City/InputFix.git "Input Fix" # 1 mod: Input Fix
   git clone -q https://github.com/WizardsLizards/CauldronOfChance.git "Cauldron of Chance" # 1 mod: Cauldron of Chance
   git clone -q https://github.com/wojciech-graj/DoomValley.git "Doom Valley" # 1 mod: Doom Valley
   git clone -q https://github.com/wrongcoder/SDV-TuneBobbers.git "Tune Bobbers" # 1 mod: Tune Bobbers
   git clone -q https://github.com/wrongcoder/SDV-UnscheduledEvaluation.git "Unscheduled Evaluation" # 1 mod: Unscheduled Evaluation
   git clone -q https://github.com/WuZhuoran/Stardew_AnimalSitter.git "Animal Sitter LTS" # 1 mod: Animal Sitter LTS
   git clone -q https://github.com/Xebeth/StardewValley-ListModsCommand.git "Mod List Command" # 1 mod: Mod List Command
   git clone -q https://github.com/Xebeth/StardewValley-SpeedMod.git "Speed Mod" # 1 mod: Speed Mod
   git clone -q https://github.com/xeru98/StardewMods.git "Better Special Orders" # 1 mod: Better Special Orders
   git clone -q https://github.com/xirsoi/MayonnaisePlusPlus.git "Mayonnaise++" # 1 mod: Mayonnaise++
   git clone -q https://github.com/xxfttkx/AutoTouchStatue.git "Auto Touch Statue" # 1 mod: Auto Touch Statue
   git clone -q https://github.com/XxHarvzBackxX/airstrike.git "Airstrike" # 1 mod: Airstrike
   git clone -q https://github.com/XxHarvzBackxX/Custom-Obelisks.git "Custom Obelisks" # 1 mod: Custom Obelisks
   git clone -q https://github.com/XxHarvzBackxX/Custom-Winter-Star-Gifts.git "Custom Winter Star Gifts" # 1 mod: Custom Winter Star Gifts
   git clone -q https://github.com/XxHarvzBackxX/fabrication.git "Fabrication" # 1 mod: Fabrication
   git clone -q https://github.com/XxHarvzBackxX/recyclableCola.git "Recyclable Cola" # 1 mod: Recyclable Cola
   git clone -q https://github.com/xynerorias/pierres-roulette.git "Pierre's Roulette Shop" # 1 mod: Pierre's Roulette Shop
   git clone -q https://github.com/xynerorias/Seed-Shortage.git "Seed Shortage" # 1 mod: Seed Shortage
   git clone -q https://github.com/Yariazen/YariazenMods.git "Buildable Greenhouse Updated" # 1 mod: Buildable Greenhouse Updated
   git clone -q https://github.com/ylLty/StardewValleyDGLAB.git "Stardew Valley X DG-LAB" # 1 mod: Stardew Valley X DG-LAB
   git clone -q https://github.com/ylsama/RightClickMoveMode.git "Mouse Move Mode" # 1 mod: Mouse Move Mode
   git clone -q https://github.com/YonKuma/MoodGuard.git "Mood Guard" # 1 mod: Mood Guard
   git clone -q https://github.com/yoshimax2/Befriend-Marlon-and-Gunther.git "Befriend Marlon and Gunther" # 1 mod: Befriend Marlon and Gunther
   git clone -q https://github.com/YsEmei/RangeExtender.git "Range Extender" # 1 mod: Range Extender
   git clone -q https://github.com/YsEmei/SkipGalaxyAuthentication.git "Skip Galaxy Authentication" # 1 mod: Skip Galaxy Authentication
   git clone -q https://github.com/ytloe/Steps-Taken-RNG-Prediction.git "Steps Taken RNG Prediction on Screen" # 1 mod: Steps Taken RNG Prediction on Screen
   git clone -q https://github.com/YTSC/StardewValleyMods.git "~YTSC" # 2 mods: Enhanced Slingshots, Quality Fish Ponds
   git clone -q https://github.com/yuri-moens/BetterActivateSprinklers.git "Better Activate Sprinklers" # 1 mod: Better Activate Sprinklers
   git clone -q https://github.com/yuri-moens/LadderLocator.git "Ladder Locator" # 1 mod: Ladder Locator
   git clone -q https://github.com/YyeahdudeE/OrganizeShortcut.git "Organize Inventory Shortcut" # 1 mod: Organize Inventory Shortcut
   git clone -q https://github.com/zack-hill/stardew-valley-mods.git "Stash Items" # 1 mod: Stash Items
   git clone -q https://github.com/zack20136/StardrewValley-Mods.git "Chest Feature Set" # 1 mod: Chest Feature Set
   git clone -q https://github.com/Zamiell/stardew-valley-mods.git "~Zamiell" # 7 mods: Always Organize Chests, Automatic Screenshot Taker, Eat & Drink From Inventory, Morning Auto-Pause, Shift Toolbar When Paused, Swap Rings, Visible Artifact Spots
   git clone -q https://github.com/ZaneYork/CustomToolEffect.git "Custom Tool Effect" # 1 mod: Custom Tool Effect
   git clone -q https://github.com/ZaneYork/SDV_CustomLocalization.git "Custom Localization" # 1 mod: Custom Localization
   git clone -q https://github.com/ZaneYork/SDV_Mods.git "~ZaneYork" # 2 mods: Custom Crops Decay, Dynamic Price
   git clone -q https://github.com/zeldela/sdv-mods.git "~zeldela" # 3 mods: Daily Luck on HUD, Fast Fish Ponds (and Rebalancing), Remember Birthday Gifts
   git clone -q https://github.com/ZhuoYun233/YetAnotherAutoWatering-StardrewValleyMod.git "Yet Another Auto Watering" # 1 mod: Yet Another Auto Watering
   git clone -q https://github.com/zombifier/My_Stardew_Mods.git "~zombifier" # 18 mods: Animal Squeeze Through, Chicken Feed - a Poultry Diet Rework, Cloths and Colors, Custom Carpenters, Custom Tapper Framework, Darkwings - Spookier Void Chickens, Extra Animal Configs, Extra Machine Configs, Fish Pond Aquaponics, Flavored And Generic Fuel For Artisan Recipes, Fresh Farm Produce, Furniture Machine, Golden Animal Cracker Extractor, Immersive Manure, Lenient Stashing, Luremaster B Gone, Rage Bait, Toggle Items Framework
   git clone -q https://github.com/Zoryn4163/SMAPI-Mods.git "~Zoryn4163" # 8 mods: Better RNG, Calendar Anywhere, Durable Fences, Fishing Mod, Health Bars, Junimo Deposit Anywhere, Movement Mod, Regen Mod
   git clone -q https://github.com/zunderscore/StardewWebApi.git "Stardew Web API" # 1 mod: Stardew Web API
   git clone -q https://github.com/Zyin055/TVAnnouncements.git "TV Announcements" # 1 mod: TV Announcements
   git clone -q https://gitlab.com/Capaldi12/wherearethey.git "Where Are They" # 1 mod: Where Are They
   git clone -q https://gitlab.com/delixx/stardew-valley-unlockable-areas.git "Unlockable Areas" # 1 mod: Unlockable Areas
   git clone -q https://gitlab.com/delixx/stardew-valley/coop-cursor.git "Coop Cursor" # 1 mod: Coop Cursor
   git clone -q https://gitlab.com/delixx/stardew-valley/custom-farm-loader.git "~delixx" # 2 mods: Custom Farm Loader, Easy Farm Switcher
   git clone -q https://gitlab.com/delixx/stardew-valley/desert-bloom-farm.git "Desert Bloom Farm" # 1 mod: Desert Bloom Farm
   git clone -q https://gitlab.com/delixx/stardew-valley/gacha-geodes.git "Gacha Geodes" # 1 mod: Gacha Geodes
   git clone -q https://gitlab.com/delixx/stardew-valley/lenient-window-resize.git "Lenient Window Resize" # 1 mod: Lenient Window Resize
   git clone -q https://gitlab.com/delixx/stardew-valley/personal-indoor-farm.git "Personal Indoor Farm" # 1 mod: Personal Indoor Farm
   git clone -q https://gitlab.com/delixx/stardew-valley/quicksave.git "Quick Save" # 1 mod: Quick Save
   git clone -q https://gitlab.com/delixx/stardew-valley/unlockable-bundles.git "Unlockable Bundles" # 1 mod: Unlockable Bundles
   git clone -q https://gitlab.com/enom/time-before-harvest-enhanced.git "Time Before Harvest Enhanced" # 1 mod: Time Before Harvest Enhanced
   git clone -q https://gitlab.com/geko_x/stardew-mods.git "~geko_x" # 2 mods: Shut Up, Zoomy Farmer
   git clone -q https://gitlab.com/hbc-mods/stardew-valley/sv-notepad.git "SV Notepad" # 1 mod: SV Notepad
   git clone -q https://gitlab.com/kdau/cropbeasts.git "Cropbeasts" # 1 mod: Cropbeasts
   git clone -q https://gitlab.com/kdau/eastscarp.git "East Scarp → C#" # 1 mod: East Scarp → C#
   git clone -q https://gitlab.com/kdau/flowerbombs.git "Flower Bombs" # 1 mod: Flower Bombs
   git clone -q https://gitlab.com/kdau/portabletv.git "Portable TV" # 1 mod: Portable TV
   git clone -q https://gitlab.com/kdau/predictivemods.git "~kdau" # 2 mods: Public Access TV, Scrying Orb
   git clone -q https://gitlab.com/kdau/pregnancyrole.git "Pregnancy Role" # 1 mod: Pregnancy Role
   git clone -q https://gitlab.com/kdau/prismaticpride.git "Prismatic Pride" # 1 mod: Prismatic Pride
   git clone -q https://gitlab.com/micfort/betterjunimoscropfields.git "Better Junimos Crop Fields" # 1 mod: Better Junimos Crop Fields
   git clone -q https://gitlab.com/nikperic/energytime.git "Energy Time" # 1 mod: Energy Time
   git clone -q https://gitlab.com/popska/UsefulHats.git "Hat Buffs" # 1 mod: Hat Buffs
   git clone -q https://gitlab.com/shanks3042/stardewvalleyeasyprairieking.git "Difficulty Changer for Journey of the Prairie King" # 1 mod: Difficulty Changer for Journey of the Prairie King
   git clone -q https://gitlab.com/speeder1/ChestNameWithHoverLabel.git "Chest Label System" # 1 mod: Chest Label System
   git clone -q https://gitlab.com/speeder1/farming-implements-combat.git "Combat with Farming Implements" # 1 mod: Combat with Farming Implements
   git clone -q https://gitlab.com/speeder1/FenceSlowDecayMod.git "Slower Fence Decay" # 1 mod: Slower Fence Decay
   git clone -q https://gitlab.com/speeder1/SMAPIHealthbarMod.git "Enemy Health Bars" # 1 mod: Enemy Health Bars
   git clone -q https://gitlab.com/speeder1/SMAPISprinklerMod.git "Better Sprinklers" # 1 mod: Better Sprinklers
   git clone -q https://gitlab.com/speeder1/SPDSprintAndDashMod.git "Sprint and Dash Buttons" # 1 mod: Sprint and Dash Buttons
   git clone -q https://gitlab.com/tophatsquid/sdv-configurable-junimo-kart.git "Configurable Junimo Kart" # 1 mod: Configurable Junimo Kart
   git clone -q https://gitlab.com/tophatsquid/sdv-tidy-fields.git "Tidy Fields" # 1 mod: Tidy Fields
   git clone -q https://gitlab.com/yuri0r/toolbelt.git "ToolBelt" # 1 mod: ToolBelt
   ```

## Update this readme
This readme is updated periodically using the `build fetch commands.linq` script in this folder.
To rebuild the commands, just run that script and copy the provided commands into the readme
above.

## See also
* Join [#making-mods-general on the Stardew Valley Discord](https://smapi.io/community#Discord) (free) or
  [my Patreon](https://www.patreon.com/pathoschild) (paid) for monthly analyses, stats, and a
  link to download a full mod dump containing every Stardew Valley mod on CurseForge/ModDrop/Nexus.
