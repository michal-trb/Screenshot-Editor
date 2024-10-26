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
    /// Highlights and allows the user to select a window by clicking on it, ignoring the window's shadow area if present.
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
                // Pobierz RECT okna, aby uwzględnić obszar cienia (shadow area)
                RECT windowRect;
                GetWindowRect(currentWindowUnderCursor, out windowRect);

                // Użycie DwmGetWindowAttribute do uzyskania rozszerzonej ramki okna
                RECT frameRect;
                int result = DwmGetWindowAttribute(currentWindowUnderCursor, DWMWA_EXTENDED_FRAME_BOUNDS, out frameRect, Marshal.SizeOf(typeof(RECT)));

                // Jeśli uzyskano ramkę rozszerzoną, dostosuj obwódkę, aby nie obejmowała cienia
                if (result == 0) // 0 oznacza, że operacja DwmGetWindowAttribute zakończyła się sukcesem
                {
                    // Obliczamy wielkość cienia na podstawie różnicy między ramką rozszerzoną a rzeczywistym obszarem okna
                    int shadowLeft = frameRect.Left - windowRect.Left;
                    int shadowTop = frameRect.Top - windowRect.Top;
                    int shadowRight = windowRect.Right - frameRect.Right;
                    int shadowBottom = windowRect.Bottom - frameRect.Bottom;

                    // Zmniejszenie obszaru do wyróżnienia o obszar cienia
                    windowRect.Left += shadowLeft;
                    windowRect.Top += shadowTop;
                    windowRect.Right -= shadowRight;
                    windowRect.Bottom -= shadowBottom;
                }

                // Aktualizacja pozycji i rozmiaru okna overlay z uwzględnieniem cienia okna
                overlay.UpdatePositionAndSize(windowRect);
                lastWindowUnderCursor = currentWindowUnderCursor;
            }

            // Sprawdzenie, czy użytkownik kliknął lewy przycisk myszy (wybór okna)
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
    /// Captures a screenshot of a specific window by its handle.
    /// </summary>
    /// <param name="hWnd">Handle to the window to capture.</param>
    /// <returns>A Bitmap containing the captured image of the window.</returns>
    public static Bitmap CaptureWindow(IntPtr hWnd)
    {
        var rect = GetWindowRectangle(hWnd);

        if (rect.Left == 0 && rect.Top == 0 && rect.Right == 0 && rect.Bottom == 0)
        {
            GetWindowRect(hWnd, out rect);
        }

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        // Utwórz bitmapę na podstawie wymiarów okna
        Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
        }

        return bmp;
    }

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [Flags]
    private enum DwmWindowAttribute : uint
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY,
        DWMWA_TRANSITIONS_FORCEDISABLED,
        DWMWA_ALLOW_NCPAINT,
        DWMWA_CAPTION_BUTTON_BOUNDS,
        DWMWA_NONCLIENT_RTL_LAYOUT,
        DWMWA_FORCE_ICONIC_REPRESENTATION,
        DWMWA_FLIP3D_POLICY,
        DWMWA_EXTENDED_FRAME_BOUNDS,
        DWMWA_HAS_ICONIC_BITMAP,
        DWMWA_DISALLOW_PEEK,
        DWMWA_EXCLUDED_FROM_PEEK,
        DWMWA_CLOAK,
        DWMWA_CLOAKED,
        DWMWA_FREEZE_REPRESENTATION,
        DWMWA_LAST
    }

    /// <summary>
    /// Gets the extended frame bounds of the given window, including any shadows or frame extensions.
    /// </summary>
    /// <param name="hWnd">Handle of the window to retrieve the frame bounds.</param>
    /// <returns>A <see cref="RECT"/> structure containing the dimensions of the extended frame bounds of the window.</returns>
    public static RECT GetWindowRectangle(IntPtr hWnd)
    {
        RECT rect;

        int size = Marshal.SizeOf(typeof(RECT));
        DwmGetWindowAttribute(hWnd, (int)DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, out rect, size);

        return rect;
    }

    private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

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
