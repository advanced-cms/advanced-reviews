Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
& ./build/dotnet-install.ps1 -Architecture x64 -AzureFeed "https://dotnetcli.azureedge.net/dotnet" -JsonFile "./global.json"
if($LASTEXITCODE -ne 0) { throw "Failed" }