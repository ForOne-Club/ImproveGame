[Chinese version here | 中文版](ChangeLog.md)

# v1.6.3
## Subversions
- v1.6.3
## BUG Fixes
- 修复了圣诞老人在开启NPC生成优化的情况下会重复入住并死去的BUG
- 修复了合成物品时可能的Mod冲突
- 修复了摸彩袋掉率显示无法检测模组摸彩袋的BUG
## Additions
- 新增玩家属性面板，可在 UI 设置调节显示开关。
## Misc
- 优化了多人模式下的体验，降低可能的延迟
- 优化了修改城镇NPC入住速度功能
- 虚空魔杖配方更改，现在是Boss前物品

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
- Added enemy spawn rate slider, only available while having the infinite [Battle Potion][Water Candle][Peace Cadnle][Sunflower] and [Calm Potion]
- Added Potion Bag, can cotain 20 buff potions at most, potions inside also take effects
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
- Large Backpack:
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