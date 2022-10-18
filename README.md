# Elden-Ring-Debug-Tool
A tool for testing and debugging mods in Elden Ring  

![build](https://github.com/Nordgaren/Elden-Ring-Debug-Tool/actions/workflows/build.yml/badge.svg) 
![github__version](https://img.shields.io/github/v/release/Nordgaren/Elden-Ring-Debug-Tool)
[![GitHub all releases](https://img.shields.io/github/downloads/Nordgaren/Elden-Ring-Debug-Tool/total)](https://github.com/Nordgaren/Elden-Ring-Debug-Tool/releases/latest)
 
# WARNING  
Backup your saves before using this tool, and restore the backups before going online. Using this too to skip around the game (for example going to the madness grace before talking to melina or killing margit) will cause you to get banned!  

## Requirements 
* [.NET 6 Desktop Runtime x64](https://download.visualstudio.microsoft.com/download/pr/f13d7b5c-608f-432b-b7ec-8fe84f4030a1/5e06998f9ce23c620b9d6bac2dae6c1d/windowsdesktop-runtime-6.0.4-win-x64.exe)  
* [.Net Framework 4.6.1]( https://www.microsoft.com/en-us/download/details.aspx?id=48130)
* [Visual C++ Redistributable x64 Packages for Visual Studio 2013](https://www.microsoft.com/en-gb/download/details.aspx?id=40784)

## How To Use 
* Launch Elden Ring and get on one of your characters
* Launch Elden Ring Debug Tool.  
* Go to "Settings" and select "Spawn Untradeable"  
* Go to "Inventory" tab. You can select a category and search for the item that way, or you can check "Search All" by the search bar and search for the item that way.  
* Select an item in the listbox and hit create on the item you want to spawn. The "Ash" combobox will only be avaiable on weapons that can equip ashes.  
* If the tool doesn't connect, you may have to run it as administrator before it can see the Elden Ring process.  
* **SEAMLESS COOP ITEM CATEGORY**: Inside the Elden Ring Debug Tool folder is a Resources folder. In `Resources/ItemCategories.txt` and remove the two slashes from start of `//0x40000000 false Items/Goods/SeamlessCoop.txt Seamless Coop` (line 10). This will load the Seamless Coop items the next time the debug tool is started.  
* After re-launching the program,iIn the "Inventory" tab, there will now be a category called "Seamless Coop". This will give you a selection of the four items added by the Seamless Coop mod. Click the item you wish to create and hit the "Create" button.   

## Known Issues  
* Hotkeys don't save between sessions at this moment. They will Soon™  

## Installing  
* Extract contents of zip archive to it's own folder. You may have to run as admin if Elden Ring Debug Tool crashes  

## Advanced Usage
* For a param to load there has to be both a def in `Resources/Params/Defs` and a definition in on of the files in `Resources/Params/Pointers`  
* The format for pointer is `Offset:Name`. You can organize these files however you want. They will all be opened, read, split and added to the Param list, if there is a corresponding def. If the param has a shared def, you can optionally format like so `Offset:Name:ParamDefName` and the tool will look for the correct paramdef.  
* Can add names to a file with the same name as the param in `Resources/Params/Names` to add row names. Default name will just be the Row ID.  
* After saving params, you can decrypt your regulation.bin by dragging and dropping it over the debug tools exe. Rename the param to have the correct capitalization, unpack the regulation.bin with Yabber and replace the one with the saved param and then re-pack the regulation.bin with [Yabber](https://github.com/JKAnderson/Yabber/releases). You can re-encrypt the regulation file, as well, but it is unnecessary.  

## Libraries
[ErdTools](https://github.com/Nordgaren/Erd-Tools) by Me, which uses:  
 * [SoulsFormats](https://github.com/JKAnderson/SoulsFormats) by [TKGP](https://github.com/JKAnderson/)  
 * My fork of [Property Hook](https://github.com/Nordgaren/PropertyHook) by [TKGP](https://github.com/JKAnderson/)  

[Octokit](https://github.com/octokit/octokit.net) by [Octokit](https://github.com/octokit) team

[GlobalHotkeys](https://github.com/mrousavy/Hotkeys) by [Marc Rousavy](https://github.com/mrousavy)  

[SettingsProviders](https://github.com/Bluegrams/SettingsProviders) By [Bluegrams](https://github.com/Bluegrams/)

## Thank You  
**[TKGP](https://github.com/JKAnderson/)** Author of [DS Gadget](https://github.com/JKAnderson/DS-Gadget) [Property Hook](https://github.com/JKAnderson/PropertyHook) and [SoulsFormats](https://github.com/JKAnderson/SoulsFormats)  
**[vawser](https://github.com/vawser/)** Author of [Yapped Rune Bear](https://github.com/vawser/Yapped-Rune-Bear) and curator of knowledge.  
**Pav** Author of one of the CE Tables I used to find pointers and offsets, as well as provided the pointer list  
**[inuNorii](https://github.com/inunorii)** Creator of The Grand Archives table, which I also used for this, and one of the admins at TGA Discord.  
**[FrankvdStam](https://github.com/FrankvdStam)** Helped with the build action, MVVM stuff and general troubleshooting.  
**[King Borehaha](https://github.com/kingborehaha/DS-Gadget-Local-Loader)** Who's local loading system has worked really well for a lot of things, including this project  
**[jamesq7](https://github.com/kh0nsu)** For helping figure out the cheat for enabling the map during combat and his rewrite of Wulfs Target script
**[wulf2k](https://github.com/Wulf2k)** Original author of the target script and various other contributions  

# Change Log 
### Beta 0.8.4  
* Updated to 1.07

* Added current animation to Player and Target tab.  

* Params read strings from memory, if they are available.  

* Reloading the game now refreshes the param view, so old params/values aren't still present in the list 

* Cleaned up program icon thanks to Rayan  

* Target tab should now no longer miss any enemies when targeting them  

* Changed enemy handle to a long integer, and display it as hex  

* Can now lock target before acquiring one, and it will lock on  

* Fixed a bug where the grace view would freeze when locking/unlocking all graces.  

### Beta 0.8.3

* un-oofed SiteOfGrace xml  

### Beta 0.8.2

* Added inventory counts to the Inventory page.  

* Spawn Untradeable is now enabled by default. Be careful spawning in cut content (hidden in ItemCategories.txt) and using it online in vanilla. Still useful to turn off for mass a item gibs.  

### Beta 0.8.1

* Player panel with some info (Mainly the same as target). A compelete redesign of target and Player panel is coming soon!  

* Misc tab where you can set and unset event flags by ID.   

* Grace management tab where you can manage the graces that are open, set your last grace and warp to any grace.  

* Multi Item Spawning enabled.  

* Support for computers who's language setup couldn't find the Elden Ring process.  

* Item Gib now responds correctly and only gives you a message box if you haven't gotten the item before.  

* Bug fixes who's patch notes got deleted by clumsyness.  

### Beta 0.8

* Continued re-coloring things.  

* Added each tab as it's own dockable pane that can be stacked like regular tabs, or put side by side. They can also be showns and hidden.  

* Fields now all have proper increment/decrement controls.  

* Fields that are multi-bit wide now work, for instance Bullet -> FollowType is now a 3 bit wide number, with a max of 7, instead of just a checkbox.  

* Fixed null values showing as 0  

* Fixed tooltips disappearing when user has scalled UI in Windows settings   (.Net 6 Update may be required)  

### Beta 0.7.2

* Fixed a glitch with the inventory not updating properly  

* Beautified the inventory datagrid  

### Beta 0.7.1

* Fixed missing tooltips  

* Added translated paramdefs

### Beta 0.7

* Fixed weather cheat and added some labels  

* Added some info to the enemy tab  

* Made the hotkeys easier to make and added a system to reference parameters in other tabs  

* Hotkeys don't save at the moment. Currently looking into the options with the new upcoing dock system  

* Everything should load up much faster and the window shouldn't freeze while loading  

* Can now lock target as well as lock some stats on the target    

* Can double click on any of the labels in the enemy panel to copy the label to your clipboard. 

### Beta 0.6.1

* Fixed a bug where some enemies didn't show up in target.  

* Fixed a bug where double clicking in the Inventory datagrid would cause the tool to crash.  

### Beta 0.6

* Added target debug tab. Work in progress, but gives some basic information. 

* Fixed item list. Should be no more duplicates and the categories should make sense.  

* Fixed ash of war selection not updating infusion.  

### Beta 0.5.3

* Fixed field search not working (whoops)

### Beta 0.5.2

* Added new events to items that are spawned, but don't give you the event attached to them (I.E. Maps)  

* Enable opening map in Combat in new Cheats tab.

* Updated for patch 1.04  

### Beta 0.5.1

* Fixed item Max Upgrade so it now looks at originEquipWep  

### Beta 0.5  

* Inventory tab has been added, major thanks to [inuNorii](https://github.com/inunorii)

* Can now give yourself any item in the game, and add items to the list via the Resources/Items files and the Rsources/ERItemCategories.txt file.      

* Items are BY DEFAULT limited to only items that can be shared in multiplayer. You can turn this off with the checkbox in the panel next to the Give panel.  

* Panel on the right shows items currently in player inventory.  

* Added settings to local folder and added settings tab.  

* Added hotkeys tab in new settings tab.  

* Added a warning when the app starts up.  

### Beta 0.4   

* Can now save params using the games built-in function thanks to Pav! Params save to `ELDEN RING/capture/param`.  

* Added drag and drop functionality to decrypt and re-encrypt regulation.bin to the exe.  

* Can reset params back to how they were when the tool loaded.  

### Beta 0.3.1  

* Fixed crash some users were experiencing and added global exception handler  

### Beta 0.3  

* Temporarily Reverted UI Changes  


### Beta 0.2  

* Reads ALL params, now.  

* Row Search  

* Field Search  

* UI Update  

* Optimized Field loading/saving fields of rows that are already loaded  

### Beta 0.1  

* Reads most params. Can edit them, too.  
