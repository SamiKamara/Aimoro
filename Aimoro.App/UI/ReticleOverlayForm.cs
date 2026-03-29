using Aimoro.App.Native;
using System.Drawing.Drawing2D;

namespace Aimoro.App.UI;

public sealed class ReticleOverlayForm : Form
{
    private const int OverlayPadding = 8;
    private AppSettings _settings = new();
    private Screen _targetScreen = Screen.PrimaryScreen ?? Screen.AllScreens.First();

    public ReticleOverlayForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        DoubleBuffered = true;
        BackColor = Color.Magenta;
        TransparencyKey = Color.Magenta;

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.UserPaint,
            true);

        UpdateOverlayBounds();
    }

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            const int wsExLayered = 0x00080000;
            const int wsExTransparent = 0x00000020;
            const int wsExToolWindow = 0x00000080;
            const int wsExNoActivate = 0x08000000;

            var parameters = base.CreateParams;
            parameters.ExStyle |= wsExLayered | wsExTransparent | wsExToolWindow | wsExNoActivate;
            return parameters;
        }
    }

    public void ApplySettings(AppSettings settings)
    {
        _settings = settings.Clone();
        _settings.Normalize();
        UpdateOverlayBounds();
        Invalidate();
    }

    public void SetTargetScreen(Screen screen)
    {
        _targetScreen = screen;
        UpdateOverlayBounds();
        Invalidate();
    }

    private void UpdateOverlayBounds()
    {
        var maxCrosshairExtent = _settings.ReticleGap + _settings.ReticleLength + (int)Math.Ceiling(_settings.ReticleThickness / 2d);
        var centerDotExtent = _settings.ShowCenterDot
            ? (int)Math.Ceiling(_settings.CenterDotSize / 2d)
            : 0;

        var radius = Math.Max(maxCrosshairExtent, centerDotExtent) + OverlayPadding;
        var size = (radius * 2) + 1;
        var screenBounds = _targetScreen.Bounds;
        var centerX = screenBounds.Left + (screenBounds.Width / 2);
        var centerY = screenBounds.Top + (screenBounds.Height / 2);
        var newBounds = new Rectangle(centerX - radius, centerY - radius, size, size);

        if (Bounds != newBounds)
        {
            Bounds = newBounds;
        }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.Clear(TransparencyKey);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var center = new Point(ClientSize.Width / 2, ClientSize.Height / 2);
        using var pen = new Pen(_settings.GetReticleColor(), _settings.ReticleThickness)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };

        var length = _settings.ReticleLength;
        var gap = _settings.ReticleGap;

        e.Graphics.DrawLine(
            pen,
            center.X - gap - length,
            center.Y,
            center.X - gap,
            center.Y);

        e.Graphics.DrawLine(
            pen,
            center.X + gap,
            center.Y,
            center.X + gap + length,
            center.Y);

        e.Graphics.DrawLine(
            pen,
            center.X,
            center.Y - gap - length,
            center.X,
            center.Y - gap);

        e.Graphics.DrawLine(
            pen,
            center.X,
            center.Y + gap,
            center.X,
            center.Y + gap + length);

        if (_settings.ShowCenterDot)
        {
            var size = _settings.CenterDotSize;
            var rectangle = new Rectangle(center.X - (size / 2), center.Y - (size / 2), size, size);
            using var brush = new SolidBrush(_settings.GetReticleColor());
            e.Graphics.FillEllipse(brush, rectangle);
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeMethods.WM_MOUSEACTIVATE)
        {
            m.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
            return;
        }

        base.WndProc(ref m);
    }
}
