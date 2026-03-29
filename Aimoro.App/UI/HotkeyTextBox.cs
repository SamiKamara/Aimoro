using System.ComponentModel;

namespace Aimoro.App.UI;

public sealed class HotkeyTextBox : TextBox
{
    private HotkeyDefinition _hotkey = HotkeyDefinition.Empty();

    public HotkeyTextBox()
    {
        ReadOnly = true;
        ShortcutsEnabled = false;
        Text = _hotkey.ToDisplayString();
        BackColor = SystemColors.Window;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HotkeyDefinition Hotkey
    {
        get => _hotkey.Clone();
        set
        {
            _hotkey = value?.Clone() ?? HotkeyDefinition.Empty();
            Text = _hotkey.ToDisplayString();
        }
    }

    public event EventHandler? HotkeyChanged;

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData is Keys.Tab or (Keys.Shift | Keys.Tab))
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }

        var key = keyData & Keys.KeyCode;
        if (key is Keys.Back or Keys.Delete)
        {
            Hotkey = HotkeyDefinition.Empty();
            HotkeyChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        if (HotkeyDefinition.IsModifierKey(key))
        {
            return true;
        }

        var capturedHotkey = HotkeyDefinition.FromKeyData(keyData);
        if (!capturedHotkey.IsValid)
        {
            return true;
        }

        Hotkey = capturedHotkey;
        HotkeyChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
}
