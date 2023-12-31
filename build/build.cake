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
ConvertableDirectoryPath _sourceDirectory, _buildDirectory, _outputDirectory;
OperatingSystem _operatingSystem;


///////////////////////////////////////////////////////////////////////////////
// Custom Function
///////////////////////////////////////////////////////////////////////////////
string GetPath(ConvertableDirectoryPath path)
{
   return MakeAbsolute(path).FullPath;
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

    _sourceDirectory = Directory("../src");
    _buildDirectory = Directory("../build");
    _outputDirectory = Directory("../publish");

});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("BuildInitialization")
   .Does(() =>
{
   Information($"Build is running on {_operatingSystem} operating system{(_isGithubActionsBuild ? " using Github Actions infrastructure" : string.Empty)}.");
   Information($"Source Directory: {GetPath(_sourceDirectory)}");
   Information($"Build Directory: {GetPath(_buildDirectory)}");
   Information($"Version: {_versionString}");

   Information($"Clean up any existing output in directory '{GetPath(_outputDirectory)}'.");
   CleanDirectories(GetPath(_outputDirectory));
});

Task("Default")
.IsDependentOn("BuildInitialization")
.Does(() => {
   Information("Hello Cake!");
});


RunTarget(target);