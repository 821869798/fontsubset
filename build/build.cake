///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var BUILD_NUMBER = EnvironmentVariable("GITHUB_RUN_NUMBER", "1");
var VERSION_PREFIX = Argument("VersionPrefix", "0.1");


enum OperatingSystem 
{
    Windows,
    MacOS,
    Linux
}

string _versionString = string.Empty;
bool _isGithubActionsBuild;
ConvertableDirectoryPath _srcConsoleDirectory,_srcGuiDirectory, _buildDirectory, _outputDirectory;
OperatingSystem _operatingSystem;

///////////////////////////////////////////////////////////////////////////////
// Custom Function
///////////////////////////////////////////////////////////////////////////////
string GetPath(ConvertableDirectoryPath path)
{
    return MakeAbsolute(path).FullPath;
}

DotNetPublishSettings GetDefaultDotNetPublishSettings()
{
    var defaultPublishSettings = new DotNetPublishSettings()
    {
        SelfContained = true,
        PublishSingleFile = true,
        PublishTrimmed = true,
        Framework = "net8.0",
        IncludeAllContentForSelfExtract = true,
        IncludeNativeLibrariesForSelfExtract = true,
        Configuration = "Release",
    };
    return defaultPublishSettings;
}

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");

    _versionString = string.Format("{0}.{1}", VERSION_PREFIX, BUILD_NUMBER);
    _isGithubActionsBuild = GitHubActions.IsRunningOnGitHubActions;

    if(IsRunningOnLinux())
    {
       _operatingSystem = OperatingSystem.Linux;
    }
    else if(IsRunningOnMacOs())
    {
       _operatingSystem = OperatingSystem.MacOS;
    }
    else
    {
       _operatingSystem = OperatingSystem.Windows;
    }

    _srcConsoleDirectory = Directory("../src/fontsubset-console");
    _srcGuiDirectory = Directory("../src/fontsubset-gui");
    _buildDirectory = Directory("../build");
    _outputDirectory = Directory("../publish");

});

Teardown(ctx =>
{
    // Executed AFTER the last task.
    //Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("BuildInitialization")
   .Does(() =>
{
    Information($"Build is running on {_operatingSystem} operating system{(_isGithubActionsBuild ? " using Github Actions infrastructure" : string.Empty)}.");
    Information($"Source Console Directory: {GetPath(_srcConsoleDirectory)}");
    Information($"Source Gui Directory: {GetPath(_srcGuiDirectory)}");
    Information($"Build Directory: {GetPath(_buildDirectory)}");
    Information($"Version: {_versionString}");

    Information($"Clean up any existing output in directory '{GetPath(_outputDirectory)}'.");
    CleanDirectories(GetPath(_outputDirectory));
});

Task("Build-Windows")
    .WithCriteria(() => _operatingSystem == OperatingSystem.Windows)
    .IsDependentOn("BuildInitialization")
    .Does(() => 
{
    var outputDirectory = _outputDirectory + Directory("win-x64");

    Information("Build for Windows x64");
    var settings = GetDefaultDotNetPublishSettings();
    settings.Runtime = "win-x64";
    settings.OutputDirectory = GetPath(outputDirectory);
    DotNetPublish(GetPath(_srcConsoleDirectory), settings);
    DotNetPublish(GetPath(_srcGuiDirectory), settings);

    /*
    Information("Create portable ZIP archive from the build.");
    Zip(outputDirectory, _outputDirectory + File("windows_portable.zip"));

    Information("Create installation setup.");
    var setupSettings = new InnoSetupSettings
    {
        OutputDirectory = _outputDirectory,
        EnableOutput = true,
        Defines = new Dictionary<string, string> 
        {
            { "APP_NAME", APP_NAME },
            { "APP_VERSION", _versionString },
            { "APP_ROOT", GetPath(Directory("../")) }
        }
    };
    InnoSetup(_buildDirectory + File("setup.iss"), setupSettings);
    */

});


Task("Build-Linux")
    .WithCriteria(() => _operatingSystem == OperatingSystem.Linux)
    .IsDependentOn("BuildInitialization")
    .Does(() => 
{
    var outputDirectory = _outputDirectory + Directory("linux-x64");

    Information("Build for linux x64");
    var settings = GetDefaultDotNetPublishSettings();
    settings.Runtime = "linux-x64";
    settings.OutputDirectory = GetPath(outputDirectory);
    DotNetPublish(GetPath(_srcConsoleDirectory), settings);
    DotNetPublish(GetPath(_srcGuiDirectory), settings);


});


Task("Build-MacOS")
    .WithCriteria(() => _operatingSystem == OperatingSystem.MacOS)
    .IsDependentOn("BuildInitialization")
    .Does(() => 
{
    var outputDirectory = _outputDirectory + Directory("macos-x64");

    Information("Build for MacOS x64");
    var settings = GetDefaultDotNetPublishSettings();
    settings.Runtime = "osx-x64";
    settings.OutputDirectory = GetPath(outputDirectory);
    DotNetPublish(GetPath(_srcConsoleDirectory), settings);
    DotNetPublish(GetPath(_srcGuiDirectory), settings);

    outputDirectory = _outputDirectory + Directory("macos-arm64");

    Information("Build for MacOS arm64");
    var settingsForArm = GetDefaultDotNetPublishSettings();
    settingsForArm.Runtime = "osx-arm64";
    settingsForArm.OutputDirectory = GetPath(outputDirectory);
    DotNetPublish(GetPath(_srcConsoleDirectory), settingsForArm);
    DotNetPublish(GetPath(_srcGuiDirectory), settingsForArm);


});

Task("Default")
.IsDependentOn("Build-Windows")
.IsDependentOn("Build-Linux")
.IsDependentOn("Build-MacOS")
.Does(() => {
   Information("Hello Cake!");
});


RunTarget(target);