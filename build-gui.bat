@echo off
cd fontsubset-gui

set publish=..\publish-gui

set param=--self-contained:true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=None -p:DebugSymbols=false

echo dotnet publish windows x64
dotnet publish -c:Release -f:net6.0 -r:win-x64 -o:%publish%\win-x64 %param% > nul

echo dotnet publish macos x64
dotnet publish -c:Release -f:net6.0 -r:osx-x64 -o:%publish%\osx-x64 %param% -p:PublishSingleFile=false > nul

echo dotnet publish linux x64
dotnet publish -c:Release -f:net6.0 -r:linux-x64 -o:%publish%\linux-x64 %param% > nul

echo complete build
pause