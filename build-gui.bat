@echo off
cd fontsubset-gui

set publish=..\publish-gui

set param=--self-contained:true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=None -p:DebugSymbols=false

echo dotnet publish windows x64
dotnet publish -c:Release -f:net6.0 -r:win-x64 -o:%publish%\win-x64 %param% > nul

echo dotnet publish linux x64
dotnet publish -c:Release -f:net6.0 -r:linux-x64 -o:%publish%\linux-x64 %param% > nul

echo dotnet publish macos x64
dotnet publish -c:Release -f:net6.0 -r:osx-x64 -o:%publish%\osx-x64 %param% -p:PublishSingleFile=false > nul

echo create macos bundle
set osxPath=%publish%\osx-x64
set exeName=fontsubset-gui
set bundlePath=%osxPath%\%exeName%.app
REM echo d| xcopy /E/Y ".\Bundle\%exeName%.app" "%bundlePath%" > nul
REM echo f| xcopy /Y "%osxPath%\%exeName%" "%bundlePath%\Contents\Resources\script" > nul
REM del /q %osxPath%\%exeName% > nul
echo f| xcopy /Y "..\Bundle\Contents\Info.plist" "%bundlePath%\Contents\" > nul
mkdir "%bundlePath%\Contents\MacOS\" > nul 2>&1
echo d| move /Y "%osxPath%\*.*" "%bundlePath%\Contents\MacOS\" > nul


echo complete build
pause