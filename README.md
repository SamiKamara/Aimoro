# Aimoro

Aimoro is a standalone Windows reticle overlay app built in C# / WinForms.

It provides:

- An always-on-top center reticle overlay
- Live reticle customization with sliders and exact numeric inputs for opacity, scale, arm length, gap, thickness, and an optional center dot
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

- Main and outline colors
- Scale
- Arm length
- Gap from center
- Line thickness
- Opacity
- Center dot visibility and size

Click either color preview square or its **Change** button to open the color picker.

Numeric settings provide sliders for quick adjustments while retaining editable number fields for exact values. The sliders focus on practical everyday ranges; typing a valid value outside a slider's range keeps that value and places the slider at its nearest end.

| Setting | Slider range | Accepted numeric range |
| --- | ---: | ---: |
| Scale | 0.5–3.0 | 0.5–5.0 |
| Arm length | 4–50 | 4–120 |
| Gap from center | 0–30 | 0–60 |
| Line thickness | 1–8 | 1–12 |
| Opacity | 50–255 | 20–255 |
| Center dot size | 1–10 | 1–20 |

Changes are applied and saved immediately; the settings window only needs to be closed when you are done.

If you prefer to publish manually:

```powershell
dotnet publish .\Aimoro.App\Aimoro.App.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o .\artifacts\publish\win-x64
```

## Notes And Limits

- The overlay is designed for borderless-windowed and standard top-level game windows.
- Some exclusive fullscreen modes, protected overlays, or anti-cheat environments may block always-on-top windows.
- Steam auto targeting works by locating Steam library folders, then matching running windowed processes whose executable path is inside those libraries.
- If a game is not detected correctly, disable auto targeting and select the monitor manually.
