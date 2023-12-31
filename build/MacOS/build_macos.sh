ORIGIN="$(pwd)"
cd "../.."
param=--self-contained:true -p:PublishAot=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=None -p:DebugSymbols=false
dotnet publish -r osx-x64 -c Release $param -o bin/Release/net8.0/publishMac
cd "$ORIGIN"

APP_NAME="fontsubset-gui.app"
APP_OUTPUT_PATH="Output"
APP_TAR_NAME="fontsubset-gui_macOS_x86_64"
PUBLISH_OUTPUT_DIRECTORY="../../bin/Release/net8.0/publishMac/."
INFO_PLIST="Info.plist"
ICON_FILE="Icon.icns"


if [ -d "$APP_OUTPUT_PATH" ]
then
    rm -rf "$APP_OUTPUT_PATH"
fi

mkdir "$APP_OUTPUT_PATH"
mkdir "$APP_OUTPUT_PATH/$APP_NAME"

mkdir "$APP_OUTPUT_PATH/$APP_NAME/Contents"
mkdir "$APP_OUTPUT_PATH/$APP_NAME/Contents/MacOS"
mkdir "$APP_OUTPUT_PATH/$APP_NAME/Contents/Resources"

cp "$INFO_PLIST" "$APP_OUTPUT_PATH/$APP_NAME/Contents/Info.plist"

cp "$ICON_FILE" "$APP_OUTPUT_PATH/$APP_NAME/Contents/Resources/$ICON_FILE"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_OUTPUT_PATH/$APP_NAME/Contents/MacOS"
chmod +x "$APP_OUTPUT_PATH/$APP_NAME/Contents/MacOS/fontsubset-gui"
cd "$APP_OUTPUT_PATH"
tar -czvf "$APP_TAR_NAME.tar.gz" "$APP_NAME/"
mv "$APP_TAR_NAME.tar.gz" ../../"$APP_TAR_NAME.tar.gz"