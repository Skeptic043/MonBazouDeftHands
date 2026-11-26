param(
    [string]$CompilerPath = "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
)
$ErrorActionPreference = 'Stop'
$validation = Join-Path $PSScriptRoot '.local/validation'
New-Item -ItemType Directory -Force $validation | Out-Null
$output = Join-Path $validation 'CoreTests.exe'
if (Test-Path -LiteralPath $output) { Remove-Item -LiteralPath $output }
$sources = @(
    'src/RotateMath.cs',
    'tests/CoreTests.cs'
) | ForEach-Object { Join-Path $PSScriptRoot $_ }
& $CompilerPath /nologo /target:exe /warnaserror+ "/out:$output" @sources 2>&1 | Tee-Object -FilePath (Join-Path $validation 'test-compiler.log')
if ($LASTEXITCODE -ne 0) { throw "Test compiler failed with exit code $LASTEXITCODE." }
if (!(Test-Path -LiteralPath $output -PathType Leaf)) { throw 'Test compiler did not create the executable.' }
& $output 2>&1 | Tee-Object -FilePath (Join-Path $validation 'core-tests.log')
if ($LASTEXITCODE -ne 0) { throw "Tests failed with exit code $LASTEXITCODE." }
