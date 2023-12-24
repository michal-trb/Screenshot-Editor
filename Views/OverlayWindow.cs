using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

public class OverlayWindow : Window
{
    public OverlayWindow(IntPtr targetWindowHandle)
    {
        this.WindowStyle = WindowStyle.None;
        this.AllowsTransparency = true;
        this.Background = Brushes.Transparent;
        this.Topmost = true;
        this.ShowInTaskbar = false;

        // Ustaw rozmiar i położenie okna nakładki
        SetWindowPosAndSize(targetWindowHandle);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        // Ustaw odpowiednie style okna
        var hwnd = new WindowInteropHelper(this).Handle;
        SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
    }

    private void SetWindowPosAndSize(IntPtr targetWindowHandle)
    {
        GetWindowRect(targetWindowHandle, out RECT rect);
        this.Left = rect.Left;
        this.Top = rect.Top;
        this.Width = rect.Right - rect.Left;
        this.Height = rect.Bottom - rect.Top;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        // Rysuj czerwoną obwódkę
        drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Red, 5), new Rect(0, 0, this.Width, this.Height));
    }

    // P/Invoke deklaracje
    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_LAYERED = 0x80000;
}
