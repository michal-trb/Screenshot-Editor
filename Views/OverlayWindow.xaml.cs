namespace screenerWpf.Views;

using System;
using System.Windows;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using static screenerWpf.Sevices.CaptureServices.WindowScreenshot;

/// <summary>
/// A window that overlays the screen or a target window. It highlights areas of interest and manages a control window.
/// </summary>
public partial class OverlayWindow : Window
{
    /// <summary>
    /// Event triggered when a stop action is requested from the control window.
    /// </summary>
    public event EventHandler StopRequested;

    private ControlWindow controlWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="OverlayWindow"/> class with default settings for overlay.
    /// </summary>
    public OverlayWindow()
    {
        InitializeComponent();
        this.AllowsTransparency = true;
        this.WindowStyle = WindowStyle.None;
        this.Topmost = true;
        this.Background = Brushes.Transparent;
    }

    /// <summary>
    /// Creates and displays an overlay window based on the provided rectangle, with DPI adjustments.
    /// </summary>
    /// <param name="rect">The rectangle representing the area to overlay, with coordinates in screen space.</param>
    public void CreateOverlayFromRect(System.Drawing.Rectangle rect)
    {
        var dpiScale = VisualTreeHelper.GetDpi(this);
        var dpiFactorX = dpiScale.DpiScaleX;
        var dpiFactorY = dpiScale.DpiScaleY;

        this.Left = rect.Left / dpiFactorX -2;
        this.Top = rect.Top / dpiFactorY -2;
        this.Width = (rect.Right - rect.Left) / dpiFactorX +4;
        this.Height = (rect.Bottom - rect.Top) / dpiFactorY +4;

        this.Show(); // Shows the overlay window.
    }

    /// <summary>
    /// Initializes and sets extended window style to make the overlay layered and transparent.
    /// </summary>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var hwnd = new WindowInteropHelper(this).Handle;
        SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_LAYERED);
    }

    /// <summary>
    /// Updates the position and size of the overlay window based on the provided RECT, adjusting to ignore the window's shadow area if present.
    /// </summary>
    /// <param name="rect">The RECT structure representing the adjusted window boundaries excluding any shadow area.</param>
    public void UpdatePositionAndSize(RECT rect)
    {
        var dpiScale = VisualTreeHelper.GetDpi(this);
        var dpiFactorX = dpiScale.DpiScaleX;
        var dpiFactorY = dpiScale.DpiScaleY;

        this.Left = rect.Left / dpiFactorX;
        this.Top = rect.Top / dpiFactorY;
        this.Width = (rect.Right - rect.Left) / dpiFactorX;
        this.Height = (rect.Bottom - rect.Top) / dpiFactorY;
        this.InvalidateVisual();
    }

    /// <summary>
    /// Draws a red border around the window for visual emphasis.
    /// </summary>
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        int borderWidth = 2;
        drawingContext.DrawRectangle(
            Brushes.Transparent,
            new Pen(Brushes.Red, borderWidth),
            new Rect(
                borderWidth / 2.0,
                borderWidth / 2.0,
                this.Width + borderWidth,
                this.Height + borderWidth));
    }


    /// <summary>
    /// Handles the stop request from the control window and closes the overlay window.
    /// </summary>
    private void ControlWindow_StopRequested(object sender, EventArgs e)
    {
        StopRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Closes the control window when the overlay window is closed.
    /// </summary>
    private void OverlayWindow_Closed(object sender, EventArgs e)
    {
        if (controlWindow != null)
        {
            controlWindow.Close();
        }
    }

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_LAYERED = 0x80000;
}
