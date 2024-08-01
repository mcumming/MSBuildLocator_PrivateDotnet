// See https://aka.ms/new-console-template for more information

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

var dumpEnvironment = () => {
	var PathEnv = Environment.GetEnvironmentVariable("PATH");
	Console.WriteLine("###  PATH was {0}", PathEnv);

	Environment.SetEnvironmentVariable("PATH", "");
	Console.WriteLine("###  PATH is {0}", Environment.GetEnvironmentVariable("PATH"));

	var dotnetHostPathEnv = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH");
	Console.WriteLine("###  DOTNET_HOST_PATH is {0}", dotnetHostPathEnv);

	var currentProcessPath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
	Console.WriteLine("###  Current Process is at {0}", currentProcessPath);

	var dotnetHostPath = dotnetHostPathEnv ?? currentProcessPath;
	//Environment.SetEnvironmentVariable("DOTNET_HOST_PATH", PathEnv);
	Console.WriteLine("###  DOTNET_HOST_PATH is {0}", dotnetHostPath);

	if (!string.IsNullOrEmpty(dotnetHostPath))
	{
		var sdkRoot = Path.GetDirectoryName(dotnetHostPath)!;
		Console.WriteLine("###  sdkRoot is {0}", sdkRoot);
		var dotnetSdkVersion = Directory.EnumerateDirectories(Path.Combine(sdkRoot, "sdk")).First();
		Console.WriteLine("###  dotnetSdkVersion is {0}", dotnetSdkVersion);
		var msbuildSdkPath = Path.Combine(sdkRoot, "sdk", dotnetSdkVersion);
		Console.WriteLine("###  msbuildSdkPath is {0}", msbuildSdkPath);
		//MSBuildLocator.RegisterMSBuildPath(msbuildSdkPath);
	}
};
var registerMSBuild = () => {
	var msbuildInstances = MSBuildLocator.QueryVisualStudioInstances(new VisualStudioInstanceQueryOptions { DiscoveryTypes = DiscoveryType.DeveloperConsole | DiscoveryType.VisualStudioSetup | DiscoveryType.DotNetSdk });
	Console.WriteLine("\n###  Found {0} MSBuild instances", msbuildInstances.Count());
	foreach (var instance in msbuildInstances)
		Console.WriteLine("###  Found MSBuild instance {0} at {1}", instance.Version, instance.MSBuildPath);
	var msbuildInstance  = msbuildInstances.First();
	Console.WriteLine("###  Selected MSBuild instance {0} at {1}", msbuildInstance.Version, msbuildInstance.MSBuildPath);
	MSBuildLocator.RegisterInstance(msbuildInstance);
	Console.WriteLine("###  Registered MSBuild instance {0} at {1}", msbuildInstance.Version, msbuildInstance.MSBuildPath);
};


dumpEnvironment();
registerMSBuild();

var projectPath = Path.Combine(Directory.GetCurrentDirectory(), "src", "TestTool.csproj");

Console.WriteLine("###  Creating MSBuildWorkspace for net8.0");
using var workspace = MSBuildWorkspace.Create(new Dictionary<string, string> {
			{"TargetFrameworks", "net8.0"}
		});

Console.WriteLine("###  Opening project '{0}'", projectPath);
var project = await workspace.OpenProjectAsync(projectPath).ConfigureAwait(false);
Console.WriteLine("###  Project '{0}' opened", project.Name);
