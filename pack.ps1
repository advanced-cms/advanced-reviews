msbuild /p:Configuration=Release
$fullVersion=[System.Reflection.Assembly]::LoadFrom("src\alloy\bin\AdvancedApprovalReviews.dll").GetName().Version
echo $fullVersion
$version="$($fullVersion.major).$($fullVersion.minor).$($fullVersion.build)"
Write-Host Creating nuget with $version
cd src\alloy\modules\_protected\episerver-addons.ExternalReviews
Robocopy.exe 1.0.0\ $version\ /S
7z a episerver-addons.ExternalReviews.zip $version Views module.config
Remove-Item $version -force -recurse
cd ..\episerver-addons.Reviews
Robocopy.exe 1.0.0\ $version\ /S
7z a episerver-addons.Reviews.zip $version Views module.config
Remove-Item $version -force -recurse
cd ..\..\..\
nuget pack advanced-reviews.nuspec -Version $version
cd ..\..\
mv src\alloy\advanced-reviews.$version.nupkg advanced-reviews.$version.nupkg
