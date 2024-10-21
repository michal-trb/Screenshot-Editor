namespace screenerWpf.Sevices.CaptureServices;

using screenerWpf.Views;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

/// <summary>
/// Provides functionality for capturing a screenshot of a specific window selected by the user.
/// </summary>
public class WindowScreenshot
{
    /// <summary>
    /// Captures a screenshot of a single window that the user selects.
    /// </summary>
    /// <returns>A <see cref="Bitmap"/> containing the screenshot of the selected window, or null if no window was selected.</returns>
    public Bitmap CaptureSingleWindow()
    {
        IntPtr windowHandle = HighlightAndSelectWindow();
        if (windowHandle == IntPtr.Zero)
            return null;

        Bitmap screenshot = CaptureWindow(windowHandle);
        return screenshot;
    }

    /// <summary>
    /// Highlights and allows the user to select a window by clicking on it.
    /// </summary>
    /// <returns>An <see cref="IntPtr"/> representing the handle of the selected window.</returns>
    private IntPtr HighlightAndSelectWindow()
    {
        var overlay = new OverlayWindow();
        overlay.Show();

        IntPtr lastWindowUnderCursor = IntPtr.Zero;
        IntPtr selectedWindow = IntPtr.Zero;

        while (true)
        {
            POINT cursorPoint;
            GetCursorPos(out cursorPoint);
            IntPtr currentWindowUnderCursor = WindowFromPoint(cursorPoint);

            if (currentWindowUnderCursor != lastWindowUnderCursor)
            {
                overlay.UpdatePositionAndSize(currentWindowUnderCursor);
                lastWindowUnderCursor = currentWindowUnderCursor;
            }

            if ((GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0)
            {
                selectedWindow = currentWindowUnderCursor;
                break;
            }

            Thread.Sleep(10);
        }

        overlay.Close();
        return selectedWindow;
    }

    /// <summary>
    /// Captures a screenshot of the specified window using its handle.
    /// </summary>
    /// <param name="hWnd">The handle of the window to capture.</param>
    /// <returns>A <see cref="Bitmap"/> containing the screenshot of the specified window.</returns>
    public static Bitmap CaptureWindow(IntPtr hWnd)
    {
        RECT rect;
        int size = Marshal.SizeOf(typeof(RECT));
        DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out rect, size);

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
        }
        return bmp;
    }

    [DllImport("user32.dll")]
    private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

    private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern IntPtr WindowFromPoint(POINT Point);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    private const int VK_LBUTTON = 0x01; // Key code for the left mouse button
}
