<h1 align="center">Legacy Changelog</h1>

<div align="center">

[中文](../zh/legacy-changelog.md) | English

</div>

# v1.6.7

## Subversions

- v1.6.7.0
- v1.6.7.1
- v1.6.7.2
- v1.6.7.3
- v1.6.7.4

## Additions
- Fast Respawn When No Boss
- Portable Buff support for Thorium Mod
- Stat Panel support for Calamity Mod and Thorium Mod classes
- Complete rework of the Stat Panel, ModCall for other mods to add their own categories and stats
- Complete rework of the Automatic Trash Bin
- Glassmorphism effect for this mod's UIs (off by default)
- Map icon for bait-lacking Fishing Machines
- Locator and map icon for Enchanted Swords
- Unconditional Sleeping
- Nondamaging Explosives
- Unconditional Pylon Teleportation (off by default)
- Tiles Not Blocking Lights, defaults to off
- Config favorites for managing config options
- Storage Telecom to remotely open Storage Managers
- Unconditional Team Teleportation
- Wand of Space updates:
    - More shapes, including slanted lines, 'L' shaped lines, filled/hollow rectangles/circles
    - Removed tile amount limitation for straight lines
    - Improved its wheel UI

## BUG Fixes

- Fishing Machine playing animation even if storage is full
- Water Candle, Peace Candle and Shadow Candle not taking effects as Portable Buff Stations
- One auto-bag-opening attempt consuming 2 bags
- Auto bag opening with auto selling only rewards 5% of expected coins
- Wand of Space can set tile slopping regardless
- Wand of Paint always produce Green Moss while cleaning mosses
- Portable Buff Stations not taking effects when stacking over Infinite Potion Requirement
- Walls stack over 999 are not consumed even if Infinite Wand Placeables is off
- This mod's RecipeGroup failing to handle cross-mods (e.g. sometimes "any gold bar" doesn't recognize Platinum Bars)
- Right click an item in Big Backpack takes the item instead of triggering its right click effect
- Recipes are not refreshed after auto money deposition, can be a coin farm hack
- Wand of Construction fixes:
    - Incorrect chair types
    - Can't place torches on walls or at left side of tiles due to placement order
    - Some tiles losing functionality after being placed (e.g. Target Dummy can't be attacked)
    - Incorrect tile direction
    - Unable to handle Luminite or Echo Coatings
    - Incorrect preview for banners under platforms
    - Incorrect mapping between some walls and items
- Some items' right-bottom icons not drawing correctly in some UIs (e.g. dangerous walls' skull icon not drawing in Big Backpack)
- **[v1.6.7.1]** Can't respawn when "Fast Respawn When No Boss" is off
- **[v1.6.7.1]** Tooltip of "Structure Markers" option doesn't mention Enchanted Sword in Stone
- **[v1.6.7.1]** Vfx of quick home is not synced in multiplayer
- **[v1.6.7.1]** Destroying beds won't reset spawn point when "Unlimited Bed Respawn" is on
- **[v1.6.7.1]** Legacy structure files aren't recognized
- **[v1.6.7.1]** Sfx of burst wands can be heard by everyone in multiplayer regardless of distance
- **[v1.6.7.1]** Tree Settings are not applied to ash trees
- **[v1.6.7.1]** Tool speed boost is not applied to hammers used to destroy walls
- **[v1.6.7.1]** Opening the full-screen map resets the UI state
- **[v1.6.7.1]** Space wand doesn't set slope for tiles properly sometimes
- **[v1.6.7.2]** Placing sand with Wand of Space causes some of the sand to drop in item form.
- **[v1.6.7.2]** In certain situations, the Shadow Candle's portable buff station does not work correctly.
- **[v1.6.7.2]** In certain situations, the automated trash bin UI is misaligned.
- **[v1.6.7.2]** The "Smart pickup" and "Auto pickup" options in the Big Backpack aren't functioning in multiplayer.
- **[v1.6.7.2]** Wand of Construction is unable to place torches.
- **[v1.6.7.2]** The feature "Info accessories work in portable storage" is incompatible with mod accessories.
- **[v1.6.7.4]** "Teleport all NPCs home" feature is incompatible with High FPS Support Mod

## Adjustments

- Town NPC Spawn Mechanics Improvement unlocks the Stylist after any pre-hard mode boss is defeated, no need of searching
- Fishing Machine only has 20% of the previous chance to consume baits
- Hide Buff Icons of Infinite Potions is moved to Personal Configs
- Added Reset UI Positions button in Personal Configs
- Increased Explorer Drone speed
- Recipe adjustments:
    - Explorer Drone: any iron bar *12 + any gold bar *6 + any gem *3 -> any iron bar *6 + any gold bar *2
    - Wand of Architecture: 2 instead of 8 Fallen Stars are needed
    - Wand of Liquid: 8 instead of 18 Shadow Scale / Tissue Sample and 6 instead of 12 any gold bar are needed
    - Fishing Machine: any iron bar *8 + any copper bar *4 + glass *20 -> any iron bar *4 + any copper bar *2
- Slight improvements to Loot Bag Item Drop Rate UI
- Map icons are now resizable
- Wire Cutter is now affected by Tool Speed
- Wand of Construction can now handle content of Signs
- Items which have a buff duration below 60s are no longer considered as infinite potions, which solves some mod compatibility issues (e.g. Astral Injection from Calamity Mod)
- **[v1.6.7.1]** "Faster Respawn" and "Fast Respawn When No Boss" is combined into one option
- **[v1.6.7.1]** Added a tip to hide the infinite buff icon in the buff icon's tooltip
- **[v1.6.7.2]** Adjusted some UI drawing logic to adapt to the High FPS Support Mod.
- **[v1.6.7.2]** Wand of Space now supports block wands (eg. Living Wood Wand, Leaf Wand).
- **[v1.6.7.2]** Adjusted the "Trees grow rapidly" function to "Trees grow rapidly & ignore layer conditions".
- **[v1.6.7.2]** Optimized some text.
- **[v1.6.7.3]** Added toggle to the Big Backpack button, the settings button position adapts to DragonLens.
- **[v1.6.7.4]** Removed the big backpack button and the custom settings button, to fix the issue of the buttons being massive.

# v1.6.6
The mod name have changed to Quality of Terraria since v1.6.6.0
## Subversions
- v1.6.6.0
- v1.6.6.1
- v1.6.6.2 (Not yet)
## Additions
- Locators, to indicate structures on map
- Item Finder, to quickly find items in chests and personal inventories
- Bag Opener, to batch open treasure bags and sell what obtained
- Explorer Drone, your good friend in deep rocks
- "Stormdark" color scheme
- Scarecrow Dummy, a configurable damage-testing dummy (not in multiplayer yet)
- "Infinite Wand Placeables" config option, you can turn it off now
- "Stackable Quest Fishes" config option
- **[v1.6.6.1]** Ability to see what items an item can be shimmered into
## BUG Fixes
- Breaking Fishing Machine on tables crashes the game
- Unable to disable Lucky Coin
- Destructive wands could not destroy certain grass blocks in one go
- Instant Home Teleportation respawns players when they should be waiting for respawn
- Wand of Liquid not functioning when its panel is closed
- **[v1.6.6.1]** Modded buff stations don't apply buffs
- **[v1.6.6.1]** Some modded vfx is invisible
- **[v1.6.6.1]** Storage Manager category icon displays Stormdark theme icon under Classic theme
- **[v1.6.6.1]** The festival feature (Christmas & Halloween) ignores the system time
- **[v1.6.6.1]** "Any Gem" is shown as a localization key
## Adjustments
- Tiles under Fishing Machine are now breakable
- Added a hotkey to open mod configs
- Destructive wands now consumes no mana
- Wand of Construction's respawn room now uses no wall
- Optimized Auto Money Deposition, and it works on Defender Medals
- Instant Home Teleportation can be disabled now
- UI position of Buff Tracker is now saved
- Some chat notifications are now pop-up notifications
- Improved `Mod.Call` for `AddPotion` and `AddStation`
- Added `Mod.Call` support for Fishing Machine
- **[v1.6.6.1]** Expanded the Fishing Machine to 40 slots
- **[v1.6.6.1]** The fishing speed of Fishing Machine is increased by 300% in Hardmode now
- **[v1.6.6.1]** Buff Tracker now supports pinyin search (Chinese related)

# v1.6.5
v1.6.5.0 is the mod's first release on 1.4.4 (v1.6.4 is a beta)
## Subversions
- v1.6.5.0
- v1.6.5.1
- v1.6.5.2
- v1.6.5.3
- v1.6.5.4
## Additions
- Increased using speed of and enabled auto-reuse for Life Crystal, Mana Crystal and Life Fruit
- Enabled configuring chest detection range of Storage Manager
- Middle-clicking an mount item now uses it
- You can customize the slope type when using Space Wand to place blocks now
- **[v1.6.5.1]** Items in Storage Manager can now be counted for crafting in multiplayer.
- **[v1.6.5.1]** Items can be quickly stacked to chests while using Storage Manager.
- **[v1.6.5.1]** Portable Banner is now supported by Storage Manager, items in "Furniture" chests can be counted.
- **[v1.6.5.1]** Added an Automatic Trash Bin, similar to Auto Trash.
- **[v1.6.5.2]** All hairstyles are now selectable when creating a player.
- **[v1.6.5.2]** Added a Teleport NPCs Home function.
- **[v1.6.5.2]** Added a No Pool Size Penalty function. You can even fish with 2 blocks of water.
- **[v1.6.5.3]** Added a Teleportation Rods hotkey.
- **[v1.6.5.3]** Added a Homing Items hotkey.
- **[v1.6.5.3]** The Fishing Machine now supports the Multiple Lures mod. Yes now you can fish in insanely fast speeds!
- **[v1.6.5.3]** Added "Keep Game Running When Focus Is Lost".
- **[v1.6.5.3]** Added "Red Potion Extension".
- **[v1.6.5.3]** Added "Expert Debuff Duration".
- **[v1.6.5.4]** Added "World Feature Panel".
- **[v1.6.5.4]** Added "Unlimited Bed Respawn".
## BUG Fixes
- Shadow Candle and War Table are now considered as buff stations
- Fixed no tile drop of left-facing Fishing Machine
- "Items Count For Crafting" now works
- Fixed Void Bag couldn't be opened by middle click
- Fixed Wand of Painting not taking certain paints
- Fixed Storage Manager couldn't be applied with certain paints
- Fixed Axe of Regrowth unable to harvest herbs
- Fixed middle click not opening GUI under high FPS
- **[v1.6.5.1]** Disabled unintentional interaction with Storage Manager GUI when recipes are listed.
- **[v1.6.5.1]** Fixed "Guaranteed Fruit Drop From Tree Shake".
- **[v1.6.5.1]** Fixed infinite buffs cannot be hiden in certain conditions.
- **[v1.6.5.1]** Fixed middle click cheat on unpurchased items.
- **[v1.6.5.1]** Fixed middle click not using Wand of Construction and Wand of Liquid in Big Backpack.
- **[v1.6.5.1]** Fixed Wand of Space randomly placing platforms in multiplayer.
- **[v1.6.5.1]** Fixed some Wand issues in multiplayer.
- **[v1.6.5.1]** Fixed Wand of Liquid not detecting certain liquid items.
- **[v1.6.5.1]** Fixed Big Backpack unable to receive items when inventory is full.
- **[v1.6.5.1]** Fixed no popup text when items are received by extra inventories.
- **[v1.6.5.2]** Fixed config option "Guaranteed Fruit Drop From Tree Shake" being always on.
- **[v1.6.5.2]** "Tooltip Simplification" now works
- **[v1.6.5.3]** Fixed this mod's item duplication bug.
- **[v1.6.5.3]** Corrected icons of the Wand of Liquids.
- **[v1.6.5.3]** Fixed "Guaranteed Fruit Drop From Tree Shakes" not supporting some trees.
- **[v1.6.5.3]** Handled exceptions when trying to put incompatible items into the Potion Bag and the Banner Box.
- **[v1.6.5.4]** Corrected some descriptions.
- **[v1.6.5.4]** Fixed the Wand of Space not slopping correctly in some cases.
## Adjustments
- Optimized Pinying searching, now supports polyphones
- Adjusted mod config UI
- Wands besides Wand of Contruction and Wand of Architecture now display tile to be placed
- Wand of Space now resets tiles' sloping while replacing tiles
- **[v1.6.5.1]** Quick Stack & Deposit All for Storage Manager are now animated.
- **[v1.6.5.1]** Removed "Better Town NPC Mechanism" because 1.4.4 already did it.
- **[v1.6.5.2]** Slightly reduced the amount of data transferred in multiplayer.
- **[v1.6.5.2]** Improved Auto Trash UI.
- **[v1.6.5.3]** Optimized most of this mod's UIs.
- **[v1.6.5.3]** Removed foreswing of the Magic-Mirror-like items.
- **[v1.6.5.3]** Added a hotkey to hide/show the Auto-Trash Bin.
- **[v1.6.5.3]** Added vfx to the Fishing Machine.
- **[v1.6.5.3]** Added a config for toggling explosion effects of this mod's wands.
- **[v1.6.5.3]** Improved how "Other Features" is displayed.
- **[v1.6.5.3]** Improved speed of taking items from this mod's storages via right clicking.

# v1.6.4
This mod has been updated to tModLoader 1.4.4 since v1.6.4.0
## Subversions
- v1.6.4.0
- v1.6.4.2
- v1.6.4.4
## BUG Fixes
- **[v1.6.4.2]** Fixed a part of the Wand of Construction material detection bug
- **[v1.6.4.2]** Fixed Better Reforging showing only one prefix
- **[v1.6.4.2]** Fixed double item drop of Fishing Machine and Storage Manager
- **[v1.6.4.2]** "Lava Slime No Lava" now works
## Adjustments
- **[v1.6.4.2]** Wormhole Potions no longer work outside of inventory and Void Vault
- **[v1.6.4.2]** Removed "Weapons Auto-reuse" as 1.4.4 added "Autofire" already
- **[v1.6.4.2]** Renamed "Better Reforging" to "Retrospective Reforging", and no longer reduce probability of rolled prefixes
- **[v1.6.4.4]** Adjusted UI
## Misc
- Updated to tModLoader v1.4.4
- **[v1.6.4.2]** Slitghly reduced cost of Retrospective Reforging
- **[v1.6.4.2]** Added Chinese name for the Wand of Technology projectile
- **[v1.6.4.2]** Improved display of "Non-configurable Features"
- **[v1.6.4.2]** Report issues in comments or our Discord server!

# v1.6.3
## Subversions
- v1.6.3
- v1.6.3.1
- v1.6.3.2
- v1.6.3.3
## BUG Fixes
- Fixed Santa Claus spawns and die when town NPC spawn mechanics improvement is enabled
- Fixed potential mod conflicts when crafting
- Fixed Grab Bag Item Drop Rate not detecting modded grab bags
- Fixed some Wands cannot be researched in Journey mode
- Fixed Wand of Space sees doors as solid tiles
- **[v1.6.3.1]** Fixed some incorrect text display
- **[v1.6.3.1]** Fixed quick stacked items in Storage Manager not updated for recipe
- **[v1.6.3.1]** Fixed the Storage Manager has no recipe or config
- **[v1.6.3.1]** Fixed potential System.NullReferenceException exception in SidedEventTrigger
- **[v1.6.3.1]** Fixed 'tile placement range boost' not working when 'tile placement speed boost' is disabled
- **[v1.6.3.1]** Fixed a minimap rendering issue
- **[v1.6.3.1]** Fixed Big Backpack auto pickup issues
- **[v1.6.3.2]** 修复了部分模组物品无法在圆角物品框中正常显示的BUG
- **[v1.6.3.2]** 修复了药水袋旗帜盒无法在大背包打开的问题
- **[v1.6.3.2]** 修复了部分在关闭跳帧模式下产生的 BUG
- **[v1.6.3.3]** 修复了按下K键会无限生成铁锭的BUG
- **[v1.6.3.3]** 修复了开启服务器时该模组会自动关闭的BUG
## Additions
- Added Player Stats Panel, can be configured in UI config
- Added Storage Manager, go easy with your chests
- Overhauled vanilla UI drawing (may increase GPU consumption), also affects font drawing position, can be configured
- Splitted Increase Tile Placement Range and Speed into speed boost (toggle) and range boost (slider)
## Misc
- Optimized multiplayer mode experience, lowered potential lag
- Optimized Town NPC Spawn Rate Modifications
- Wand of Void now can be obtained pre-boss
- **[v1.6.3.1]** Optimized UI botton animation of the Storage manager
- **[v1.6.3.1]** Increased chest detection range of the Storage Manager
- **[v1.6.3.1]** Increased max stack of all non-wand items in this mod to 9999
- **[v1.6.3.1]** Raised sell price of the Storage Manager to 5 gold
- **[v1.6.3.1]** Raised sell price of Fishing Machine to 2 gold
- **[v1.6.3.2]** Optimized Town NPC Spawn Rate Modifications (again)
- **[v1.6.3.2]** 模组储存空间内可参与合成的物品现在会计入 Recipe Browser 模组的合成材料数量统计了
- **[v1.6.3.2]** 优化了部分物品拾取的逻辑，大背包添加新物品拾取提示，以及收藏物品过渡动画
- **[v1.6.3.2]** 优化了部分重置原生用户界面的一些问题
- **[v1.6.3.3]** 为字体Y轴偏移配置添加了预览

# v1.6.2
## Subversions
- v1.6.2
- v1.6.2.1
- v1.6.2.2
- v1.6.2.3
- v1.6.2.4
- v1.6.2.6
## BUG Fixes
- Fixed some issues with multiplayer mode
- Fixed the pool cursor not showing in multiplayer mode
- Fixed sometimes the Fishing Machine cannot be opened
- Fixed the TileEntity instance of Fishing Machine could not despawn properly
- **[v1.6.2.1]** Fixed Wand of Void losing items in multiplayer mode
- **[v1.6.2.1]** Fixed Wand of Void failing to record chests' names and results in an error
- **[v1.6.2.1]** Fixed NPCs drop no coin after coin drop rate is adjusted to 25x and mod is reloaded
- **[v1.6.2.1]** Fixed Fishing Machine does not drop items inside when destroyed
- **[v1.6.2.2]** Fixed Auto Research still functioning even not in Joureney mode
- **[v1.6.2.2]** Fixed some Draw issue with the Huge Inventory
- **[v1.6.2.2]** Fixed Fishing Machine is considered in Ocean biome when Fargo's Mutant Mod's fountains biome feature is enabled and has no fountain nearby
- **[v1.6.2.3]** Fixed unable to creat new players
- **[v1.6.2.3]** Fixed unable to share buffs in portable inventories in multiplayer
- **[v1.6.2.3]** Fixed unable to toggle shared buffs in multiplayer
- **[v1.6.2.4]** Fixed certain item icons in ModUI not displaying
- **[v1.6.2.4]** Fixed only one infinite buff can be active when Protable Buff Station is off
- **[v1.6.2.6]** Fixed the buff sharing
## Additions
- Wand of Construction, can save structures as files and place them while having enough material
- Wand of Void, can move non-empty chests
- The Banner Box and the Potion Bag now have UIs, increased volumes and ability to collect items automatically
- Improved hotkey tooltip texts
- The Fishing Machine now supports Fargo's Mutant Mod's water fountain biomes
- Wand of Liquid adjusted
    - Now can select a maximum 30x30 area
    - Can infinitely place liquids if have respective 'bottomless bucket' items
    - The same way for the 'absorbant sponge' items
- Five filter options for the Fishing Machine, you can choose what to fish
- A function allows town NPCs to live in evil biomes
- **[v1.6.2.1]** Lifeform Analyzer now can filter unwanted creatures
- **[v1.6.2.1]** Auto Research in Journey mode
- **[v1.6.2.1]** Press Alt to inspect chest items
- **[v1.6.2.1]** Reworked Buff Tracker's UI
    - Smoothened UI borders
    - Combined multiple buff pages to a single scrolling page
    - Now you can search buffs
- **[v1.6.2.1]** Reworked 'Better Reforging'
    - Chance for prefixes to be rolled again is greatly reduced
    - Prefixes which have been rolled can be applied manually
    - Removed the 'Reforge Count' function
- **[v1.6.2.1]** Added recipes of Wand of Void and Wand of Construction
- **[v1.6.2.2]** Ammo in the Huge Inventory now can be used directly
- **[v1.6.2.2]** Added Tooltip Simplification ('favorite' tooltip removal, detailed tooltip in vanity slots)
- **[v1.6.2.2]** Mod Item toggles are now independent to each other
- **[v1.6.2.2]** Wand of Space/Architecture/Technology no longer consume materials stack not less than 999, 99 for chairs/workbenches/beds
- **[v1.6.2.4]** Added a UI config, to adjust the offset on Y axis of mod texts, modify this to get visual performance when using font packs
- **[v1.6.2.6]** Banner Box now can be toggled by middle click, slightly adjusted color and arrangement
- **[v1.6.2.6]** Improved how Wand of Technology works, maximum tile increased to 2500
- **[v1.6.2.6]** Besides Wand of Painting, all wands now cannot be used under debuff Creative Shock
- **[v1.6.2.6]** Added function Infinite Throwings, all throwings which stack >= 3996 will not be consumed
- **[v1.6.2.6]** Threshold for Fallen Stars to not be consumed is lowered to 999 instead of 3996
## Misc
- The server console can query the config verification password by typing /qolpassword
- Password format changed to four letters (not case-sensitive)
- **[v1.6.2.1]** Optimized format of Item Arrays and Lists stored using TagCompound
- **[v1.6.2.1]** Introduced AdditionalConfig to record non-ModConfig local config data
- **[v1.6.2.1]** Centered the item icons in configs

# v1.6.1
## Subversions
- v1.6.1.0
- v1.6.1.1
- v1.6.1.2
## BUG Fixes
- **[v1.6.1.0]** Fixed an issue with launching servers
- **[v1.6.1.1]** Fixed the Super Void Vault feature not working
- **[v1.6.1.1]** Fixed the Huge Inventory playing a sound without an item been taken out
- **[v1.6.1.1]** Fixed certain treasure bags and crates being taken up when right click them in the Huge Inventory
- **[v1.6.1.2]** Fixed an issue with infinite ingredients
## Additions
- **[v1.6.1.0]** Reworked the Huge Inventory's UI, now looks nicer
- **[v1.6.1.0]** The 'WMITF' feature now can be toggled off
- **[v1.6.1.1]** Items in the Huge Inventory now can be used as crafting materials directly
- **[v1.6.1.2]** The feature above can be toggled off
- **[v1.6.1.2]** Can stop the spread of the Corruption, the Crimson and the Hallow biomes

# v1.6.0
## Subversions
- v1.6.0.1
- v1.6.0.2
- v1.6.0.3
## BUG Fixes
- Fixed the fishing speed of the Fishing Machine being dramatically fast
- Fixed the Encumbering Stone working in personal inventories
- Fixed possible lagging while mining
- Fixed the Blinkroot and Shiverthorn sometimes not been re-planted
- Fixed the Fishing Machine duplicating items in multiplayer mode
- **[v1.6.0.2]** Fixed issue where traveling merchants could benefit from Better Town NPC Spawn Mechanics
- **[v1.6.0.2]** Fixed every town NPC can heal you if Emergency Treatment is on
- **[v1.6.0.3]** Fixed mod can't load with mods that add any NPCs
## Additions
- Reworked control panel of the Wand of Space, added new categories
- Added a Wand of Painting, can paint/remove paint of a large area
- Added a Refresh Shop button for the Travelling Merchant to refresh the shop immediately
- Added a function that makes the Nurse to heal you instantly when you talk to her
- The Wand of Liquids no longer consumes mana
- Added Faster Extractinator function
- Added 'Which Mod Is This From' function, showing which mod is an item or a NPC from
- Added a Grab Bag Item Drop Rate Display function
- Added a function to completely unlock a creature's bestiary upon killing it
- Added a function to adjust the number of kills needed to obtain a banner
- Added a Banner Box to store up to 200 stacks of banners
- Added a Portable Crafting Station feature
- Added a lavaless lava slime function, to stop lava slime from spawning lava
- Added the function of keeping buff after death
- Added a Better Town NPC Spawn Mechanics function
- Added Together Happier(multiplayer) functions
    - Shared Portable Crafting Stations for teammates
    - Shared Infinite Buffs for teammates
    - Range of the shared effects can be adjusted, unlimited by default
- Added 2 configuration presets
- Added a automatically-join-red-team function
- Added a Hotbar Swap function
- Added a function which displays minion slot occupancy in tooltip
## Misc
- Improved UI animations of the two Wands of Bursts
- Keybinds now can be translated without reloading this mod
- The Fishing Machine is now be crafted by Any Iron Bar * 8 + Any Copper Bar * 4 + Glass Block * 20
- The Wand Of Space can also be crafted by Any Wood * 24 + Any Evil Bar * 8 + Amethyst * 8
- Removed change in styles of the Potion Bags while containing potions
- **[v1.6.0.2]** Wand of space and wand of technology will drop for each player in multiplayer, and will not drop again when the player already has one.
- **[v1.6.0.3]** Fishing Machine is 1 time faster than v1.6.0

# v1.5.9
## Subversions
- v1.5.9.0
- v1.5.9.1
- v1.5.9.2
## BUG fix
- Fixed the Merchant's spawn condition not counting coins in the Money Trough
- Corrected the description of the NPC spawn speed multiplier
- Fixed the Cozy Fire buff and the Heart Lantern buff having no actual effect
- Fixed the Fireplace item not granting the Cozy Fire buff
- Fixed the infinite luck potions providing wrong amout of luck
- **[v1.5.9.1]** Fixed the Fishing Machine not working
- **[v1.5.9.1]** Fixed the Potion Bag unable to store potions in multiplayer mode
- **[v1.5.9.1]** Fixed the Wand of Space cannot be used without having a platform
- **[v1.5.9.1]** Fixed newly crafted Potion Bags share the same inventory
- **[v1.5.9.2]** Fixed the Fishing Machine not fishing from correct biomes
## Additions
- Added enemy spawn rate slider, only available while having the infinite [Battle Potion][Water Candle][Peace Candle][Sunflower] and [Calm Potion]
- Added Potion Bag, can contain 20 buff potions at most, potions inside also take effects
- Merged features of the mod Taller Trees(by Cyril), also supported adjusting height of palm trees
- Added gem trees always drop gem feature(disabled by default)
- Added The Travelling Merchant does not depart feature(disabled by default)
- Improved text display method, using Tags
- Added adjustable infinite buff requirement
- Portable banners now support mod banners
- Added Fishing Machine
- The Wormhole Potion functions in personal inventories
- The Garden Gnome functions in inventory
- The Wand of Space now support horizontal and vertical placement and you can choose which type of tile to place: platforms, solid blocks, ropes or minecart tracks
- **[v1.5.9.1]** Added a hotkey for the Buff Tracker
- **[v1.5.9.2]** The Fishing Machine now supports mod biomes
## Misc
- **[v1.5.9.1]** Recipes of mod items now use RecipeGroup
- **[v1.5.9.2]** Added Mod.Calls

# v1.5.8
## Subversions
- v1.5.8.0
- v1.5.8.1
- v1.5.8.2
- v1.5.8.3
- v1.5.8.4
- v1.5.8.5
- v1.5.8.6
- v1.5.8.7
- v1.5.8.8
## BUG fix
- Fix the bug of sound effect inverse while middle clicking the Eyebone
- Fixed the problem that the infinite buffs of the special campfires do not take effect
- **[v1.5.8.5]** Fixed the Buff Tracker flashing in certian conditions
- **[v1.5.8.5]** Fixed the wands'animation bug in multiplayer mode
- **[v1.5.8.5]** Fixed the auto-collected coins not combining into the higher-class coin in multiplayer mode
- **[v1.5.8.5]** Fixed the Wand of Liquids losing liquids in multiplayer mode
- **[v1.5.8.5]** Fixed the Staff of Regrowth auto-plant function not working in multiplayer mode
- **[v1.5.8.6]** Fixed sometimes closing the Buff Tracker also dsiables buffs
- **[v1.5.8.6]** Fixed the player character just stuck there
- **[v1.5.8.6]** Fixed sometimes the infinite items are not glowing
- **[v1.5.8.6]** Added an verification system for changing the server's mod configs
- **[v1.5.8.7]** Fixed the player character just stuck somewhere again
- **[v1.5.8.7]** Removed the "What is middle click" tooltip
- **[v1.5.8.7]** Fixed Cadance (a Calamity buff) not disabling Lifeforce and Regeneration
- **[v1.5.8.8]** Fixed well-fed buffs disabling other buffs
- **[v1.5.8.8]** Fixed incorrect potion tooltips
## Additions
- Add a Liquid Wand, which can drain/place liquid
- Added buff tracker, which can be disabled
- Left-click the infinite buff icon to open
- Infinite buffs are added to Calamity, Fargo's Mutant Mod support for placing Buff items
- Added support of Infinite Potions and Protable Buff Stations to Calamity Mod and Fargo's Mutant Mod
- Added auto-deposit function
- Added Respawn with Full Life function
- Added a function to accelerate tree growth
- Added a function which greatly increases the drop rate of fruit of tree shaking
- The tool speed boost now can be modified by a slider.
- Herbs' blooming condition now can be disbled
- Big Backpack:
    - Deposit all
    - Loot all
    - Quick stacking
    - Quick sorting
    - Ctrl + left click to trash/sell
- **[v1.5.8.4]** Accelerate herb growth
- **[v1.5.8.4]** Herbs can bloom in any condition
- **[v1.5.8.4]** Staff of Regrowth will plant seeds while gathering herbs
- **[v1.5.8.5]** Blacklist for tile placeing speed boost
## Misc
- Optimization, fixed lagging in certain condition
- The default value of the Extra Buff Slot is set to 99 (maximum value)
- Fixed the unloading problem
# v1.5.7
## Additions
- New function to hide buff icons of the Infinite Potions
- (Different from LuiAFK, this function only hides the buff icons)
- New function to open personal inventories(Piggy Bank, Safe, ect.) by Mouse 3(Middle click)
- New control panels for Wand of Architecture/Brusts/Starbrusts(Right click to open)
- Huge Inventory:
    - Fixed display bugs(Added animation for scrolling)
    - Shift + Left Click to quickly transfer
    - Alt + Left Click to mark as favorite