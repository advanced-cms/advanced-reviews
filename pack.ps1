$defaultVersion="1.0.0"
function ZipCurrentModule
{
    Param ([String]$moduleName)
    Robocopy.exe $defaultVersion\ $version\ /S
    ((Get-Content -Path module.config -Raw) -Replace $defaultVersion, $version ) | Set-Content -Path module.config
    7z a "$moduleName.zip" $version Views module.config
    git checkout module.config
    Remove-Item $version -Force -Recurse
}

$fullVersion=[System.Reflection.Assembly]::LoadFrom("src\alloy\bin\AdvancedApprovalReviews.dll").GetName().Version
$version="$($fullVersion.major).$($fullVersion.minor).$($fullVersion.build)"
Write-Host "Creating nuget with $version version"

Set-Location src\alloy\modules\_protected\episerver-addons.ExternalReviews
ZipCurrentModule -moduleName episerver-addons.ExternalReviews

Set-Location ..\episerver-addons.Reviews
ZipCurrentModule -moduleName episerver-addons.Reviews

Set-Location ..\..\..\
nuget pack advanced-reviews.nuspec -Version $version
Set-Location ..\..\
Move-Item src\alloy\advanced-reviews.$version.nupkg advanced-reviews.$version.nupkg -Force
