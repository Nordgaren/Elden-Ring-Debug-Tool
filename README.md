# Elden-Ring-Debug-Tool
A tool for testing and debugging mods in Elden Ring
 
# WARNING  
Backup your saves before using this tool, and restore the backups before going online.  

## Requirements 
* [.NET 5 Desktop Runtime x64](https://download.visualstudio.microsoft.com/download/pr/b1902c77-e022-4b3e-a01a-e8830df936ff/09d0957435bf8c37eae11b4962d4221b/windowsdesktop-runtime-5.0.15-win-x64.exe)  

## Known Issues
* Loading param row fields is still a TAD sow. I am still looking to optimize it, a bit, if I can.

## Installing  
* Extract contents of zip archive to it's own folder. You may have to run as admin if Elden Ring Debug Tool crashes  

## Usage
* For a param to load there has to be both a def in `Resources/Params/Defs` and a definition in on of the files in `Resources/Params/Pointers`  
* The format for pointer is `Offset:Name`. You can organize these files however you want. They will all be opened, read, split and added to the Param list, if there is a corresponding def. If the param has a shared def, you can optionally format like so `Offset:Name:ParamDefName` and the tool will look for the correct paramdef.  
* Can add names to a file with the same name as the param in `Resources/Params/Names` to add row names. Default name will just be the Row ID.  
* After saving params, you can decrypt your regulation.bin by dragging and dropping it over the debug tools exe. Rename the param to have the correct capitalization, unpack the regulation.bin with Yabber and replace the one with the saved param and then re-pack the regulation.bin with [Yabber](https://github.com/JKAnderson/Yabber/releases). You can re-encrypt the regulation file, as well, but it is unnecessary.  

## Libraries
[SoulsFormats](https://github.com/JKAnderson/SoulsFormats) by [TKGP](https://github.com/JKAnderson/)  
My fork of [Property Hook](https://github.com/Nordgaren/PropertyHook) by [TKGP](https://github.com/JKAnderson/)  

## Thank You  
**[TKGP](https://github.com/JKAnderson/)** Author of [DS Gadget](https://github.com/JKAnderson/DS-Gadget) [Property Hook](https://github.com/JKAnderson/PropertyHook) and [SoulsFormats](https://github.com/JKAnderson/SoulsFormats)  
**Pav** Author of one of the CE Tables I used to find pointers and offsets, as well as provided the pointer list  
**[inuNorii](https://github.com/inunorii)** Creator of The Grand Archives table, which I also used for this, and one of the admins at TGA Discord.  
**[King Borehaha](https://github.com/kingborehaha/DS-Gadget-Local-Loader)** Who's local loading system has worked really well for a lot of things, including this project  

# Change Log  
### Beta 0.5  

* Inventory tab has been added.  

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
