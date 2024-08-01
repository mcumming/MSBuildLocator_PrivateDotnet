# Install private .NET SDK
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

# $LocalDotnet is the path to the locally-installed SDK to ensure the
#   correct version of the tools are executed.
$LocalDotnet = ""
# $InstallDir and $CliVersion variables can come from options to the
#   script.
$InstallDir = "./cli-tools"
$CliVersion = "9.0"

# Test the path provided by $InstallDir to confirm it exists. If it
#   does, it's removed. This is not strictly required, but it's a
#   good way to reset the environment.
if (Test-Path $InstallDir) {
	Remove-Item ./dotnet
	Remove-Item -Recurse $InstallDir
}
New-Item -Type "directory" -Path $InstallDir

Write-Host "Downloading the CLI installer..."

# Use the Invoke-WebRequest PowerShell cmdlet to obtain the
#   installation script and save it into the installation directory.
if ($IsWindows) {
	$InstallScript = "dotnet-install.ps1"
}
else {
	$InstallScript = "dotnet-install.sh"
}

Invoke-WebRequest `
	-Uri "https://dot.net/v1/$InstallScript" `
	-OutFile "$InstallDir/$InstallScript"

if ($IsMacOS -or $IsLinux) {
	# Make the script executable on macOS and Linux.
	chmod +x "$InstallDir/$InstallScript"
}

Write-Host "Installing the CLI requested version ($CliVersion) ..."

# Install the SDK of the version specified in $CliVersion into the
#   specified location ($InstallDir).
& $InstallDir/$InstallScript -Channel $CliVersion -Quality preview`
	-InstallDir $InstallDir

Write-Host "Downloading and installation of the SDK is complete."

# $LocalDotnet holds the path to dotnet.exe for future use by the
#   script.
$LocalDotnet = "$InstallDir/dotnet"

# Run the build process now. Implement your build script here.
New-Item -Path ./dotnet -ItemType SymbolicLink -Value $LocalDotnet