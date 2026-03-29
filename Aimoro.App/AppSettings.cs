using System.Drawing;

namespace Aimoro.App;

public sealed class AppSettings
{
    public bool OverlayEnabled { get; set; } = true;

    public bool AutoDetectSteamGameMonitor { get; set; } = true;

    public string? SelectedMonitorDeviceName { get; set; }

    public HotkeyDefinition ToggleHotkey { get; set; } = HotkeyDefinition.DefaultToggle();

    public HotkeyDefinition CycleMonitorHotkey { get; set; } = HotkeyDefinition.DefaultCycle();

    public HotkeyDefinition OpenSettingsHotkey { get; set; } = HotkeyDefinition.DefaultOpenSettings();

    public bool HoldToShowEnabled { get; set; } = true;

    public HoldToShowMouseButton HoldToShowMouseButton { get; set; } = HoldToShowMouseButton.RightButton;

    public string ReticleColorHex { get; set; } = "#FF4B4B";

    public int ReticleOpacity { get; set; } = 220;

    public int ReticleLength { get; set; } = 20;

    public int ReticleGap { get; set; } = 8;

    public int ReticleThickness { get; set; } = 3;

    public bool ShowCenterDot { get; set; } = true;

    public int CenterDotSize { get; set; } = 4;

    public AppSettings Clone()
    {
        return new AppSettings
        {
            OverlayEnabled = OverlayEnabled,
            AutoDetectSteamGameMonitor = AutoDetectSteamGameMonitor,
            SelectedMonitorDeviceName = SelectedMonitorDeviceName,
            ToggleHotkey = ToggleHotkey.Clone(),
            CycleMonitorHotkey = CycleMonitorHotkey.Clone(),
            OpenSettingsHotkey = OpenSettingsHotkey.Clone(),
            HoldToShowEnabled = HoldToShowEnabled,
            HoldToShowMouseButton = HoldToShowMouseButton,
            ReticleColorHex = ReticleColorHex,
            ReticleOpacity = ReticleOpacity,
            ReticleLength = ReticleLength,
            ReticleGap = ReticleGap,
            ReticleThickness = ReticleThickness,
            ShowCenterDot = ShowCenterDot,
            CenterDotSize = CenterDotSize
        };
    }

    public void Normalize()
    {
        ToggleHotkey ??= HotkeyDefinition.DefaultToggle();
        CycleMonitorHotkey ??= HotkeyDefinition.DefaultCycle();
        OpenSettingsHotkey ??= HotkeyDefinition.DefaultOpenSettings();

        if (!ToggleHotkey.IsValid)
        {
            ToggleHotkey = HotkeyDefinition.DefaultToggle();
        }

        if (CycleMonitorHotkey.Key != Keys.None && !CycleMonitorHotkey.IsValid)
        {
            CycleMonitorHotkey = HotkeyDefinition.DefaultCycle();
        }

        if (OpenSettingsHotkey.Key != Keys.None && !OpenSettingsHotkey.IsValid)
        {
            OpenSettingsHotkey = HotkeyDefinition.DefaultOpenSettings();
        }

        ReticleLength = Math.Clamp(ReticleLength, 4, 120);
        ReticleGap = Math.Clamp(ReticleGap, 0, 60);
        ReticleThickness = Math.Clamp(ReticleThickness, 1, 12);
        ReticleOpacity = Math.Clamp(ReticleOpacity, 20, 255);
        CenterDotSize = Math.Clamp(CenterDotSize, 1, 20);
        HoldToShowMouseButton = Enum.IsDefined(HoldToShowMouseButton)
            ? HoldToShowMouseButton
            : HoldToShowMouseButton.RightButton;

        try
        {
            _ = ColorTranslator.FromHtml(ReticleColorHex);
        }
        catch
        {
            ReticleColorHex = "#FF4B4B";
        }
    }

    public Color GetReticleColor()
    {
        Normalize();
        var baseColor = ColorTranslator.FromHtml(ReticleColorHex);
        return Color.FromArgb(ReticleOpacity, baseColor);
    }
}
