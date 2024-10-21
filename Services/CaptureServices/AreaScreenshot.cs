namespace screenerWpf.Services.CaptureServices;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

/// <summary>
/// Provides methods to capture screenshots of the entire screen or specific areas.
/// </summary>
public class AreaScreenshot
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("gdi32.dll")]
    private static extern uint BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, CopyPixelOperation dwRop);

    [DllImport("user32.dll")]
    private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

    /// <summary>
    /// Captures a screenshot of the entire desktop screen.
    /// </summary>
    /// <returns>A <see cref="Bitmap"/> representing the entire screen.</returns>
    public static Bitmap CaptureScreen()
    {
        IntPtr desktopWindow = GetDesktopWindow();
        IntPtr desktopDC = GetWindowDC(desktopWindow);

        // Get the dimensions of the entire virtual screen.
        int screenWidth = Convert.ToInt32(SystemParameters.VirtualScreenWidth);
        int screenHeight = Convert.ToInt32(SystemParameters.VirtualScreenHeight);

        Bitmap screenImage = new Bitmap(screenWidth, screenHeight);

        using (Graphics g = Graphics.FromImage(screenImage))
        {
            IntPtr gHdc = g.GetHdc();
            BitBlt(gHdc, 0, 0, screenWidth, screenHeight, desktopDC, 0, 0, CopyPixelOperation.SourceCopy);
            g.ReleaseHdc(gHdc);
        }

        ReleaseDC(desktopWindow, desktopDC);
        return screenImage;
    }

    /// <summary>
    /// Captures a screenshot of a specific rectangular area of the screen.
    /// </summary>
    /// <param name="area">The area of the screen to capture.</param>
    /// <returns>A <see cref="Bitmap"/> of the specified area.</returns>
    public static Bitmap CaptureArea(Rectangle area)
    {
        return CropBitmap(CaptureScreen(), area);
    }

    /// <summary>
    /// Crops a specified rectangular section from the provided bitmap.
    /// </summary>
    /// <param name="source">The source <see cref="Bitmap"/> to crop from.</param>
    /// <param name="section">The rectangular area to crop.</param>
    /// <returns>A new <see cref="Bitmap"/> containing the cropped section.</returns>
    public static Bitmap CropBitmap(Bitmap source, Rectangle section)
    {
        Bitmap cropped = new Bitmap(section.Width, section.Height);
        using (Graphics g = Graphics.FromImage(cropped))
        {
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
        }
        return cropped;
    }
}
