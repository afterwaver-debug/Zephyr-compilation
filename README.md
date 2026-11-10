# Zephyr

A lightweight Windows companion launcher for Roblox.

## Build an EXE using GitHub Actions (phone-friendly)

You do **not** need Visual Studio, a Windows PC, or the .NET SDK on your phone. GitHub's Windows runner compiles the project in the cloud.

1. Sign in to [GitHub](https://github.com/) and create a **new public or private repository** named `Zephyr`.
2. On your phone, extract this ZIP. Upload the **contents of the `Zephyr` folder** to the repository root. Make sure `.github/workflows/build-windows.yml` is included. GitHub's website may hide dot-folders in some upload pickers; if it does, use the GitHub mobile app or the website's **Add file → Create new file** to create `.github/workflows/build-windows.yml`, then paste its contents from this project.
3. Open the repository's **Actions** tab.
4. Select **Build Zephyr for Windows**.
5. Tap **Run workflow** → **Run workflow**.
6. Wait for the workflow to finish with a green check.
7. Open that completed run, scroll to **Artifacts**, and download `Zephyr-win-x64`.
8. Extract the artifact ZIP. It contains `Zephyr.exe`.

The workflow also runs automatically when relevant source files are pushed to the `main` branch.

## Build locally on Windows

Install the .NET 8 SDK, open PowerShell in this folder, then run:

```powershell
dotnet publish .\Zephyr.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o .\artifacts\Zephyr-win-x64
```

## Features

- Detects the installed Roblox Player executable.
- Opens the official Roblox website for normal launch flow.
- Applies Normal or Above Normal priority to an already-running Roblox client.
- Optionally applies the chosen priority when Roblox starts.
- Shortcuts to Windows Game Mode and Graphics settings.

## Notes

- Target: Windows x64, .NET 8 WPF.
- The published app is self-contained; a separate .NET runtime is not required on the target PC.
- Process priority is only a small scheduling preference. It is not an FPS unlocker and cannot guarantee higher FPS.
- Zephyr does not inject code, edit Roblox memory, or apply undocumented FastFlags.
- This is an early source release; test it before relying on it.
