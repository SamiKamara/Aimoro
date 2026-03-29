using System.Drawing;

namespace Aimoro.App.UI;

public sealed class SettingsForm : Form
{
    private readonly Panel _scrollPanel = new()
    {
        Dock = DockStyle.Fill,
        AutoScroll = true,
        Margin = new Padding(0, 0, 0, 8)
    };

    private readonly TableLayoutPanel _contentPanel = new()
    {
        Dock = DockStyle.Top,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        ColumnCount = 1,
        Margin = Padding.Empty
    };

    private readonly FlowLayoutPanel _buttonPanel = new()
    {
        Dock = DockStyle.Fill,
        FlowDirection = FlowDirection.RightToLeft,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        Margin = Padding.Empty,
        Padding = new Padding(0, 4, 0, 0)
    };

    private readonly CheckBox _overlayEnabledCheckBox = new()
    {
        AutoSize = true,
        Text = "Enable the reticle when Aimoro starts"
    };

    private readonly CheckBox _autoDetectCheckBox = new()
    {
        AutoSize = true,
        Text = "Automatically place the reticle on the monitor with a detected Steam game"
    };

    private readonly ComboBox _monitorComboBox = new()
    {
        DropDownStyle = ComboBoxStyle.DropDownList
    };

    private readonly HotkeyTextBox _toggleHotkeyTextBox = new();
    private readonly HotkeyTextBox _cycleHotkeyTextBox = new();
    private readonly HotkeyTextBox _openSettingsHotkeyTextBox = new();
    private readonly CheckBox _holdToShowCheckBox = new()
    {
        AutoSize = true,
        Text = "Only show the reticle while a mouse button is held"
    };

    private readonly ComboBox _holdToShowMouseButtonComboBox = new()
    {
        DropDownStyle = ComboBoxStyle.DropDownList
    };

    private readonly NumericUpDown _reticleLengthUpDown = CreateNumeric(4, 120);
    private readonly NumericUpDown _reticleGapUpDown = CreateNumeric(0, 60);
    private readonly NumericUpDown _reticleThicknessUpDown = CreateNumeric(1, 12);
    private readonly NumericUpDown _reticleOpacityUpDown = CreateNumeric(20, 255);
    private readonly CheckBox _centerDotCheckBox = new()
    {
        AutoSize = true,
        Text = "Show a center dot"
    };

    private readonly NumericUpDown _centerDotSizeUpDown = CreateNumeric(1, 20);
    private readonly Panel _colorPreviewPanel = new()
    {
        Width = 32,
        Height = 32,
        BorderStyle = BorderStyle.FixedSingle,
        Margin = new Padding(0, 0, 8, 0)
    };

    private readonly Button _pickColorButton = new()
    {
        AutoSize = true,
        Text = "Pick color..."
    };

    private readonly ColorDialog _colorDialog = new()
    {
        FullOpen = true
    };

    private Color _selectedColor;
    private bool _sizedToContent;

    public SettingsForm(AppSettings settings)
    {
        ResultSettings = settings.Clone();
        settings.Normalize();
        _selectedColor = ColorTranslator.FromHtml(settings.ReticleColorHex);

        Text = "Aimoro Settings";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ShowInTaskbar = false;
        ClientSize = new Size(600, 640);

        BuildLayout();
        BindValues(settings);
    }

    public AppSettings ResultSettings { get; private set; }

    private static NumericUpDown CreateNumeric(int minimum, int maximum)
    {
        return new NumericUpDown
        {
            Minimum = minimum,
            Maximum = maximum,
            Width = 80
        };
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 1,
            RowCount = 2
        };

        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var behaviorGroup = CreateBehaviorGroup();
        var hotkeysGroup = CreateHotkeysGroup();
        var reticleGroup = CreateReticleGroup();
        var buttonPanel = CreateButtonPanel();

        behaviorGroup.Margin = new Padding(0, 0, 0, 12);
        hotkeysGroup.Margin = new Padding(0, 0, 0, 12);
        reticleGroup.Margin = Padding.Empty;

        _contentPanel.Controls.Add(behaviorGroup, 0, 0);
        _contentPanel.Controls.Add(hotkeysGroup, 0, 1);
        _contentPanel.Controls.Add(reticleGroup, 0, 2);

        _scrollPanel.Controls.Add(_contentPanel);
        root.Controls.Add(_scrollPanel, 0, 0);
        root.Controls.Add(buttonPanel, 0, 1);

        Controls.Add(root);
    }

    private GroupBox CreateBehaviorGroup()
    {
        var group = new GroupBox
        {
            Text = "Behavior",
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        var layout = CreateTwoColumnLayout();
        layout.Controls.Add(_overlayEnabledCheckBox, 0, 0);
        layout.SetColumnSpan(_overlayEnabledCheckBox, 2);

        layout.Controls.Add(_autoDetectCheckBox, 0, 1);
        layout.SetColumnSpan(_autoDetectCheckBox, 2);

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Manual target monitor"
        }, 0, 2);

        layout.Controls.Add(_monitorComboBox, 1, 2);

        var helpLabel = CreateNoteLabel("If auto targeting is off, the reticle stays on the selected display.");
        layout.Controls.Add(helpLabel, 0, 3);
        layout.SetColumnSpan(helpLabel, 2);

        layout.Controls.Add(_holdToShowCheckBox, 0, 4);
        layout.SetColumnSpan(_holdToShowCheckBox, 2);

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Hold button"
        }, 0, 5);

        layout.Controls.Add(_holdToShowMouseButtonComboBox, 1, 5);

        _autoDetectCheckBox.CheckedChanged += (_, _) => UpdateMonitorState();
        _holdToShowCheckBox.CheckedChanged += (_, _) => UpdateHoldModeState();

        group.Controls.Add(layout);
        return group;
    }

    private GroupBox CreateHotkeysGroup()
    {
        var group = new GroupBox
        {
            Text = "Hotkeys",
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        var layout = CreateTwoColumnLayout();
        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Toggle reticle"
        }, 0, 0);
        layout.Controls.Add(_toggleHotkeyTextBox, 1, 0);

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Cycle monitor"
        }, 0, 1);
        layout.Controls.Add(_cycleHotkeyTextBox, 1, 1);

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Open settings"
        }, 0, 2);
        layout.Controls.Add(_openSettingsHotkeyTextBox, 1, 2);

        var noteLabel = CreateNoteLabel("Focus a hotkey box and press the combination you want. Press Delete to clear it.");
        layout.Controls.Add(noteLabel, 0, 3);
        layout.SetColumnSpan(noteLabel, 2);

        group.Controls.Add(layout);
        return group;
    }

    private GroupBox CreateReticleGroup()
    {
        var group = new GroupBox
        {
            Text = "Reticle",
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        var layout = CreateTwoColumnLayout();

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Color"
        }, 0, 0);

        var colorPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        colorPanel.Controls.Add(_colorPreviewPanel);
        colorPanel.Controls.Add(_pickColorButton);
        layout.Controls.Add(colorPanel, 1, 0);

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Arm length"
        }, 0, 1);
        layout.Controls.Add(_reticleLengthUpDown, 1, 1);

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Gap from center"
        }, 0, 2);
        layout.Controls.Add(_reticleGapUpDown, 1, 2);

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Line thickness"
        }, 0, 3);
        layout.Controls.Add(_reticleThicknessUpDown, 1, 3);

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Opacity"
        }, 0, 4);
        layout.Controls.Add(_reticleOpacityUpDown, 1, 4);

        layout.Controls.Add(_centerDotCheckBox, 0, 5);
        layout.SetColumnSpan(_centerDotCheckBox, 2);

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Center dot size"
        }, 0, 6);
        layout.Controls.Add(_centerDotSizeUpDown, 1, 6);

        _pickColorButton.Click += (_, _) => PickColor();
        _centerDotCheckBox.CheckedChanged += (_, _) => _centerDotSizeUpDown.Enabled = _centerDotCheckBox.Checked;

        group.Controls.Add(layout);
        return group;
    }

    private FlowLayoutPanel CreateButtonPanel()
    {
        var saveButton = new Button
        {
            AutoSize = true,
            Text = "Save"
        };

        saveButton.Click += (_, _) => SaveAndClose();

        AcceptButton = saveButton;
        _buttonPanel.Controls.Add(saveButton);
        return _buttonPanel;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_sizedToContent)
        {
            return;
        }

        AdjustSizeToContent();
        _sizedToContent = true;
        CenterToScreen();
    }

    private void AdjustSizeToContent()
    {
        var targetClientWidth = 600;
        var buttonSize = _buttonPanel.GetPreferredSize(Size.Empty);

        var workingArea = Screen.FromPoint(Cursor.Position).WorkingArea;
        var nonClientWidth = Width - ClientSize.Width;
        var nonClientHeight = Height - ClientSize.Height;
        var maxClientWidth = Math.Max(560, workingArea.Width - nonClientWidth - 32);
        var maxClientHeight = Math.Max(520, workingArea.Height - nonClientHeight - 32);

        var clientWidth = Math.Min(targetClientWidth, maxClientWidth);
        ClientSize = new Size(clientWidth, ClientSize.Height);

        PerformLayout();

        var contentSize = _contentPanel.GetPreferredSize(new Size(clientWidth - 24, 0));
        var desiredClientHeight = Math.Max(640, contentSize.Height + buttonSize.Height + 36);

        ClientSize = new Size(
            clientWidth,
            Math.Min(desiredClientHeight, maxClientHeight));
    }

    private static TableLayoutPanel CreateTwoColumnLayout()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            Padding = new Padding(12)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        return layout;
    }

    private static Label CreateNoteLabel(string text)
    {
        return new Label
        {
            AutoSize = true,
            ForeColor = SystemColors.GrayText,
            MaximumSize = new Size(470, 0),
            Text = text
        };
    }

    private void BindValues(AppSettings settings)
    {
        _overlayEnabledCheckBox.Checked = settings.OverlayEnabled;
        _autoDetectCheckBox.Checked = settings.AutoDetectSteamGameMonitor;

        foreach (var screen in Screen.AllScreens)
        {
            _monitorComboBox.Items.Add(new DisplayOption(screen.DeviceName, DisplayInfoFormatter.ToDisplayLabel(screen)));
        }

        foreach (var mouseButton in Enum.GetValues<HoldToShowMouseButton>())
        {
            _holdToShowMouseButtonComboBox.Items.Add(
                new HoldMouseButtonOption(mouseButton, mouseButton.ToDisplayString()));
        }

        var selectedOption = _monitorComboBox.Items
            .OfType<DisplayOption>()
            .FirstOrDefault(option => string.Equals(option.DeviceName, settings.SelectedMonitorDeviceName, StringComparison.OrdinalIgnoreCase));

        _monitorComboBox.SelectedItem = selectedOption ?? _monitorComboBox.Items.OfType<DisplayOption>().FirstOrDefault();

        _toggleHotkeyTextBox.Hotkey = settings.ToggleHotkey;
        _cycleHotkeyTextBox.Hotkey = settings.CycleMonitorHotkey;
        _openSettingsHotkeyTextBox.Hotkey = settings.OpenSettingsHotkey;
        _holdToShowCheckBox.Checked = settings.HoldToShowEnabled;
        _holdToShowMouseButtonComboBox.SelectedItem = _holdToShowMouseButtonComboBox.Items
            .OfType<HoldMouseButtonOption>()
            .FirstOrDefault(option => option.MouseButton == settings.HoldToShowMouseButton)
            ?? _holdToShowMouseButtonComboBox.Items.OfType<HoldMouseButtonOption>().FirstOrDefault();
        _reticleLengthUpDown.Value = settings.ReticleLength;
        _reticleGapUpDown.Value = settings.ReticleGap;
        _reticleThicknessUpDown.Value = settings.ReticleThickness;
        _reticleOpacityUpDown.Value = settings.ReticleOpacity;
        _centerDotCheckBox.Checked = settings.ShowCenterDot;
        _centerDotSizeUpDown.Value = settings.CenterDotSize;
        _colorPreviewPanel.BackColor = _selectedColor;

        UpdateMonitorState();
        UpdateHoldModeState();
        _centerDotSizeUpDown.Enabled = _centerDotCheckBox.Checked;
    }

    private void UpdateMonitorState()
    {
        _monitorComboBox.Enabled = !_autoDetectCheckBox.Checked;
    }

    private void UpdateHoldModeState()
    {
        _holdToShowMouseButtonComboBox.Enabled = _holdToShowCheckBox.Checked;
    }

    private void PickColor()
    {
        _colorDialog.Color = _selectedColor;
        if (_colorDialog.ShowDialog(this) == DialogResult.OK)
        {
            _selectedColor = _colorDialog.Color;
            _colorPreviewPanel.BackColor = _selectedColor;
        }
    }

    private void SaveAndClose()
    {
        if (!_toggleHotkeyTextBox.Hotkey.IsValid)
        {
            MessageBox.Show(
                this,
                "The reticle toggle hotkey is required.",
                "Aimoro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var duplicateMessage = ValidateHotkeys();
        if (duplicateMessage is not null)
        {
            MessageBox.Show(
                this,
                duplicateMessage,
                "Aimoro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        ResultSettings = new AppSettings
        {
            OverlayEnabled = _overlayEnabledCheckBox.Checked,
            AutoDetectSteamGameMonitor = _autoDetectCheckBox.Checked,
            SelectedMonitorDeviceName = (_monitorComboBox.SelectedItem as DisplayOption)?.DeviceName,
            ToggleHotkey = _toggleHotkeyTextBox.Hotkey,
            CycleMonitorHotkey = _cycleHotkeyTextBox.Hotkey,
            OpenSettingsHotkey = _openSettingsHotkeyTextBox.Hotkey,
            HoldToShowEnabled = _holdToShowCheckBox.Checked,
            HoldToShowMouseButton = (_holdToShowMouseButtonComboBox.SelectedItem as HoldMouseButtonOption)?.MouseButton ?? HoldToShowMouseButton.RightButton,
            ReticleColorHex = ColorTranslator.ToHtml(_selectedColor),
            ReticleLength = (int)_reticleLengthUpDown.Value,
            ReticleGap = (int)_reticleGapUpDown.Value,
            ReticleThickness = (int)_reticleThicknessUpDown.Value,
            ReticleOpacity = (int)_reticleOpacityUpDown.Value,
            ShowCenterDot = _centerDotCheckBox.Checked,
            CenterDotSize = (int)_centerDotSizeUpDown.Value
        };

        ResultSettings.Normalize();
        DialogResult = DialogResult.OK;
        Close();
    }

    private string? ValidateHotkeys()
    {
        var configuredHotkeys = new List<(string Label, HotkeyDefinition Hotkey)>
        {
            ("Toggle reticle", _toggleHotkeyTextBox.Hotkey),
            ("Cycle monitor", _cycleHotkeyTextBox.Hotkey),
            ("Open settings", _openSettingsHotkeyTextBox.Hotkey)
        };

        var duplicateGroup = configuredHotkeys
            .Where(entry => entry.Hotkey.IsValid)
            .GroupBy(entry => entry.Hotkey.ToDisplayString(), StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateGroup is null)
        {
            return null;
        }

        var duplicatedActions = string.Join(", ", duplicateGroup.Select(entry => entry.Label));
        return $"These actions are using the same hotkey ({duplicateGroup.Key}): {duplicatedActions}. Pick different shortcuts.";
    }

    private sealed record DisplayOption(string DeviceName, string Label)
    {
        public override string ToString() => Label;
    }

    private sealed record HoldMouseButtonOption(HoldToShowMouseButton MouseButton, string Label)
    {
        public override string ToString() => Label;
    }
}
