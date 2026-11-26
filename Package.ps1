param(
    [string]$OutputPath
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem

$pluginSource = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'src/Plugin.cs') -Raw
$versionMatch = [regex]::Match($pluginSource, 'public const string PluginVersion = "(\d+\.\d+\.\d+)";')
if (!$versionMatch.Success) { throw 'Cannot read the three-part PluginVersion constant.' }
$version = $versionMatch.Groups[1].Value
$pluginPath = Join-Path $PSScriptRoot 'bin/Release/EasyRotate.dll'
if (!(Test-Path -LiteralPath $pluginPath -PathType Leaf)) { throw 'Build the plugin before packaging.' }
$identity = [Reflection.AssemblyName]::GetAssemblyName($pluginPath)
if ($identity.Name -ne 'EasyRotate' -or $identity.Version -ne [version]"$version.0") {
    throw "Build output identity does not match EasyRotate $version.0. Rebuild before packaging."
}

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $PSScriptRoot "artifacts/EasyRotate-$version.zip"
} elseif (![IO.Path]::IsPathRooted($OutputPath)) {
    $OutputPath = Join-Path $PSScriptRoot $OutputPath
}
$OutputPath = [IO.Path]::GetFullPath($OutputPath)
if ([IO.Path]::GetExtension($OutputPath) -ne '.zip') { throw 'OutputPath must end with .zip.' }
if (Test-Path -LiteralPath $OutputPath) { throw 'Output already exists. Choose a new OutputPath to preserve the previous package.' }

$prefix = 'BepInEx/plugins/EasyRotate/'
$allowlist = [ordered]@{
    'bin/Release/EasyRotate.dll' = 'EasyRotate.dll'
    'LICENSE'                    = 'LICENSE'
    'CHANGELOG.md'               = 'CHANGELOG.md'
    'README.md'                  = 'README.md'
}
$expected = @()
foreach ($item in $allowlist.GetEnumerator()) {
    $source = Join-Path $PSScriptRoot $item.Key
    if (!(Test-Path -LiteralPath $source -PathType Leaf)) { throw "Missing package input: $($item.Key)" }
    $hash = (Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash
    $expected += [pscustomobject]@{ Source = $item.Key; Entry = $prefix + $item.Value; SHA256 = $hash }
}

$runDirectory = Join-Path $PSScriptRoot ('.local/package-checks/' + [guid]::NewGuid().ToString('N'))
$stageDirectory = Join-Path $runDirectory 'stage'
$extractDirectory = Join-Path $runDirectory 'extracted'
$temporaryZip = Join-Path $runDirectory 'candidate.zip'
New-Item -ItemType Directory -Path $stageDirectory -Force | Out-Null
foreach ($item in $expected) {
    $staged = Join-Path $stageDirectory $item.Entry
    New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($staged)) -Force | Out-Null
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot $item.Source) -Destination $staged
}
[IO.Compression.ZipFile]::CreateFromDirectory($stageDirectory, $temporaryZip, [IO.Compression.CompressionLevel]::Optimal, $false)
$archive = [IO.Compression.ZipFile]::OpenRead($temporaryZip)
try {
    $actualEntries = @($archive.Entries | ForEach-Object { $_.FullName.Replace('\', '/') } | Sort-Object)
    $expectedEntries = @($expected | ForEach-Object Entry | Sort-Object)
    if ($actualEntries.Count -ne $expectedEntries.Count -or @(Compare-Object $expectedEntries $actualEntries).Count -ne 0) {
        throw 'ZIP inventory does not exactly match the package allowlist.'
    }
} finally { $archive.Dispose() }
[IO.Compression.ZipFile]::ExtractToDirectory($temporaryZip, $extractDirectory)
$extractedFiles = @(Get-ChildItem -LiteralPath $extractDirectory -File -Recurse)
if ($extractedFiles.Count -ne $expected.Count) { throw 'Fresh extraction has an unexpected file count.' }
foreach ($item in $expected) {
    $extracted = Join-Path $extractDirectory $item.Entry
    if (!(Test-Path -LiteralPath $extracted -PathType Leaf) -or
        (Get-FileHash -LiteralPath $extracted -Algorithm SHA256).Hash -ne $item.SHA256) {
        throw "Fresh extraction hash mismatch: $($item.Entry)"
    }
    if ((Get-FileHash -LiteralPath (Join-Path $PSScriptRoot $item.Source) -Algorithm SHA256).Hash -ne $item.SHA256) {
        throw "Package input changed during verification: $($item.Source). Run packaging again after changes finish."
    }
}

New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($OutputPath)) -Force | Out-Null
[IO.File]::Move($temporaryZip, $OutputPath)
$packageHash = (Get-FileHash -LiteralPath $OutputPath -Algorithm SHA256).Hash
$reportPath = Join-Path $runDirectory 'verification.json'
[pscustomobject]@{
    Version = $version
    Package = [IO.Path]::GetFileName($OutputPath)
    SHA256  = $packageHash
    Entries = $expected
} | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $reportPath -Encoding UTF8
[pscustomobject]@{ Package = $OutputPath; SHA256 = $packageHash; VerifiedFiles = $expected.Count; Report = $reportPath }
