namespace screenerWpf.Services.CaptureServices;

using screenerWpf.Sevices.CaptureServices;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

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
    /// Captures a screenshot of a specific rectangular area of the screen.
    /// </summary>
    /// <param name="area">The area of the screen to capture.</param>
    /// <returns>A <see cref="Bitmap"/> of the specified area.</returns>
    public static Bitmap CaptureArea(Rectangle area)
    {
        return CropBitmap(FullScreenshot.CaptureScreen(), area);
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
