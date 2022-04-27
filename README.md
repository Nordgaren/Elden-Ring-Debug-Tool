# Elden-Ring-Debug-Tool
A tool for testing and debugging mods in Elden Ring
 
# WARNING  
Backup your saves before using this tool, and restore the backups before going online.  

## Requirements 
* [.NET 6 Desktop Runtime x64](https://download.visualstudio.microsoft.com/download/pr/f13d7b5c-608f-432b-b7ec-8fe84f4030a1/5e06998f9ce23c620b9d6bac2dae6c1d/windowsdesktop-runtime-6.0.4-win-x64.exe)  
* [.Net Framework 4.6.1]( https://www.microsoft.com/en-us/download/details.aspx?id=48130)
* [Visual C++ Redistributable Packages for Visual Studio 2013](https://www.microsoft.com/en-gb/download/details.aspx?id=40784)

## Known Issues
* None at this time. If you find any bugs, please let me know!  

## Installing  
* Extract contents of zip archive to it's own folder. You may have to run as admin if Elden Ring Debug Tool crashes  

## Usage
* For a param to load there has to be both a def in `Resources/Params/Defs` and a definition in on of the files in `Resources/Params/Pointers`  
* The format for pointer is `Offset:Name`. You can organize these files however you want. They will all be opened, read, split and added to the Param list, if there is a corresponding def. If the param has a shared def, you can optionally format like so `Offset:Name:ParamDefName` and the tool will look for the correct paramdef.  
* Can add names to a file with the same name as the param in `Resources/Params/Names` to add row names. Default name will just be the Row ID.  
* After saving params, you can decrypt your regulation.bin by dragging and dropping it over the debug tools exe. Rename the param to have the correct capitalization, unpack the regulation.bin with Yabber and replace the one with the saved param and then re-pack the regulation.bin with [Yabber](https://github.com/JKAnderson/Yabber/releases). You can re-encrypt the regulation file, as well, but it is unnecessary.  

## Libraries
[ErdTools](https://github.com/Nordgaren/Erd-Tools) by Me.  
[SoulsFormats](https://github.com/JKAnderson/SoulsFormats) by [TKGP](https://github.com/JKAnderson/)  
My fork of [Property Hook](https://github.com/Nordgaren/PropertyHook) by [TKGP](https://github.com/JKAnderson/)  

[Octokit](https://github.com/octokit/octokit.net) by [Octokit](https://github.com/octokit) team

[GlobalHotkeys](https://github.com/mrousavy/Hotkeys) by [Marc Rousavy](https://github.com/mrousavy)  

[SettingsProviders](https://github.com/Bluegrams/SettingsProviders) By [Bluegrams](https://github.com/Bluegrams/)

## Thank You  
**[TKGP](https://github.com/JKAnderson/)** Author of [DS Gadget](https://github.com/JKAnderson/DS-Gadget) [Property Hook](https://github.com/JKAnderson/PropertyHook) and [SoulsFormats](https://github.com/JKAnderson/SoulsFormats)  
**Pav** Author of one of the CE Tables I used to find pointers and offsets, as well as provided the pointer list  
**[inuNorii](https://github.com/inunorii)** Creator of The Grand Archives table, which I also used for this, and one of the admins at TGA Discord.  
**[FrankvdStam](https://github.com/FrankvdStam)** Helped with the build action and general troubleshooting.  
**[King Borehaha](https://github.com/kingborehaha/DS-Gadget-Local-Loader)** Who's local loading system has worked really well for a lot of things, including this project  
**jamesq7** For helping figure out the cheat for enabling the map during combat and his rewrite of Wulfs Target script
**wulf2k** For helping figure out the cheat for enabling the map during combat and his rewrite of Wulfs Target script

# Change Log 
### Beta 0.6.2

* Fixed weather cheat and added some labels  

* Added some info to the enemy tab  


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
