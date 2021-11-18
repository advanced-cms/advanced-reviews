# NOTE: This script must currently be executed from the solution dir (due to specs)
Param([string] $configuration = "Release", [string] $logger="console", [string] $verbosity="normal")
$ErrorActionPreference = "Stop"

Import-Module ./build/exechelper.ps1

# Install .NET tooling

exec ./build/dotnet-cli-install.ps1

exec "dotnet" "build -c $configuration EPiServer.Labs.ContentManager.sln"

Pop-Location
