# Aimoro

Aimoro is a standalone Windows reticle overlay app built in C# / WinForms.

It provides:

- An always-on-top center reticle overlay
- Reticle customization for color, opacity, arm length, gap, thickness, and an optional center dot
- Global hotkeys to toggle the reticle and cycle monitors
- A global hotkey to open settings
- Automatic targeting for monitors that currently show a detected Steam game window
- Manual monitor selection when auto targeting is not what you want
- Optional hold-to-show behavior, enabled by default for right mouse hold
- Persistent settings stored in `%AppData%\Aimoro\settings.json`

## Requirements

- Windows
- .NET 9 SDK to build or run from source

## Run From Source

```powershell
dotnet run --project .\Aimoro.App\Aimoro.App.csproj
```

When the app starts, it lives in the Windows notification area. Double-click the tray icon or use the tray menu to open settings.

Launching `Aimoro.exe` or the desktop shortcut opens the settings window automatically. If Aimoro is already running, launching it again reuses the existing tray instance and opens that same settings window instead of starting a second copy.

## Build

```powershell
dotnet build .\Aimoro.sln
```

## Publish A Standalone EXE

Use the included batch script:

```cmd
publish-win-x64.cmd
```

That publishes a self-contained single-file `win-x64` build under:

```text
.\artifacts\publish\win-x64\
```

The published output is intended to contain the standalone `Aimoro.exe`.

## Default Hotkeys

- `Alt+A`: Toggle reticle
- `Ctrl+Alt+F9`: Cycle monitors and switch to manual targeting
- `Alt+O`: Open settings

You can change all of them from the settings window.

## Default Hold Mode

Aimoro now enables hold-to-show mode by default:

- Hold the right mouse button to show the reticle
- Release the right mouse button to hide it

You can turn this mode off, or switch it to a different mouse button, from settings.

## Reticle Settings

From the settings window, you can customize:

- Reticle color
- Arm length
- Gap from center
- Line thickness
- Opacity
- Center dot visibility and size

If you prefer to publish manually:

```powershell
dotnet publish .\Aimoro.App\Aimoro.App.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o .\artifacts\publish\win-x64
```

## Notes And Limits

- The overlay is designed for borderless-windowed and standard top-level game windows.
- Some exclusive fullscreen modes, protected overlays, or anti-cheat environments may block always-on-top windows.
- Steam auto targeting works by locating Steam library folders, then matching running windowed processes whose executable path is inside those libraries.
- If a game is not detected correctly, disable auto targeting and select the monitor manually.
