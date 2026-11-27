# Building Deft Hands

## Requirements

- Windows with PowerShell and the `.NET Framework 4` C# compiler (`csc.exe`).
- A local Mon Bazou installation (for managed Unity and game assemblies).
- BepInEx 5 (`BepInEx.dll`) and HarmonyX (`0Harmony.dll`).

Game and loader assemblies are build references only. They are not included in this repository or its packages.

## Build

From the repository folder:

```powershell
powershell -ExecutionPolicy Bypass -File .\Build.ps1
```

Override default paths with `-GameManagedPath`, `-BepInExPath`, or `-HarmonyPath` when needed. The output is `bin/Release/DeftHands.dll`.

## Test

```powershell
powershell -ExecutionPolicy Bypass -File .\Test.ps1
```

Runs the offline unit tests for item-control eligibility, axis selection and inversion, hold-depth clamping, and depth easing. Any failure exits non-zero.

## Package

```powershell
powershell -ExecutionPolicy Bypass -File .\Package.ps1
```

Writes `artifacts/DeftHands-<version>.zip` with `BepInEx/plugins/DeftHands/DeftHands.dll`, `README.md`, `CHANGELOG.md`, and `LICENSE`, then verifies a clean extraction against the built files.
