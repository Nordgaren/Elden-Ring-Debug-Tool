::!/bin/cmd

cd bin\Publish
7za.exe a -t7z "Elden Ring Debug Tool %1.7z" .\win-x64\* -xr!*.pdb
