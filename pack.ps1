$defaultVersion="1.0.0"
$workingDirectory = Get-Location
$zip = "$workingDirectory\packages\7-Zip.CommandLine\18.1.0\tools\7za.exe"
$nuget = "$workingDirectory\build\tools\nuget.exe"

function ZipCurrentModule
{
    Param ([String]$moduleName)
    Robocopy.exe $defaultVersion\ $version\ /S
    Remove-Item "$moduleName.zip" -Force -Recurse -ErrorAction Ignore
    ((Get-Content -Path module.config -Raw).TrimEnd() -Replace $defaultVersion, $version ) | Set-Content -Path module.config
    Start-Process -NoNewWindow -Wait -FilePath $zip -ArgumentList "a", "$moduleName.zip", "$version", "Views", "module.config"
    ((Get-Content -Path module.config -Raw).TrimEnd() -Replace $version, $defaultVersion ) | Set-Content -Path module.config
    Remove-Item $version -Force -Recurse -ErrorAction Ignore
}

$fullVersion=[System.Reflection.Assembly]::LoadFrom("src\alloy\bin\AdvancedApprovalReviews.dll").GetName().Version
$version="$($fullVersion.major).$($fullVersion.minor).$($fullVersion.build)"
Write-Host "Creating nuget with $version version"

Set-Location src\alloy\modules\_protected\advanced-cms.ExternalReviews
ZipCurrentModule -moduleName advanced-cms.ExternalReviews

Set-Location ..\advanced-cms.Reviews
ZipCurrentModule -moduleName advanced-cms.Reviews

Set-Location $workingDirectory
Start-Process -NoNewWindow -Wait -FilePath $nuget -ArgumentList "pack", "$workingDirectory\src\alloy\Advanced.CMS.AdvancedReviews.nuspec", "-Version $version"
