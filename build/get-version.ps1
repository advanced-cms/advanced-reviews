Param([string]$branchName, [Int]$buildCounter)

if (!$branchName -or !$buildCounter) {
    Write-Error "`$branchName and `$buildCounter parameters must be supplied"
    exit 1
}

Function GetVersion($path) {
    [xml] $versionFile = Get-Content $path
    return $versionFile.SelectSingleNode("Project/PropertyGroup/VersionPrefix").InnerText
}

$assemblyVersion = GetVersion "build/version.props"

switch -wildcard ($branchName) {
    "master" {
        $preReleaseInfo = ""
    }
    "develop" {
        $preReleaseInfo = "-inte-{0:D6}"
    }
    "hotfix/*" {
        $preReleaseInfo = "-ci-{0:D6}"
    }
    "bugfix/*" {
        $preReleaseInfo = "-ci-{0:D6}"
    }
    "release/*" {
        $preReleaseInfo = "-pre-{0:D6}"
    }
    "feature/*" {
        $isMatch = $branchName -match ".*/([A-Z]+-[\d]+)-"
        if ($isMatch -eq $TRUE) {
            $feature = $Matches[1]
            $preReleaseInfo = "-feature-$feature-{0:D6}"
        }
        else {
            $preReleaseInfo = "-feature-{0:D6}"
        }
    }
    default { $preReleaseInfo = "-ci-{0:D6}" }
}

$informationalVersion = "$assemblyVersion$preReleaseInfo" -f $buildCounter

"AssemblyVersion: $assemblyVersion"
"AssemblyInformationalVersion: $informationalVersion"

"##teamcity[setParameter name='assemblyVersion' value='$assemblyVersion']"
"##teamcity[setParameter name='packageVersion' value='$informationalVersion']"
