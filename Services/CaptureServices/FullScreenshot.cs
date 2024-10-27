namespace screenerWpf.Services.CaptureServices;

using global::Helpers.DpiHelper;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Size = System.Drawing.Size;

/// <summary>
/// Provides functionality to capture a screenshot of the entire screen, considering the current DPI settings.
/// </summary>
public class FullScreenshot
{
    /// <summary>
    /// Captures a full screenshot of the primary screen.
    /// </summary>
    /// <returns>A <see cref="Bitmap"/> containing the screenshot of the entire primary screen.</returns>
    public static Bitmap CaptureScreen()
    {
        DpiHelper.UpdateDpi();
        var currentDpi = DpiHelper.CurrentDpi;

        // Calculate the width and height based on DPI
        int screenWidth = (int)(SystemParameters.VirtualScreenWidth * currentDpi.DpiX / 96);
        int screenHeight = (int)(SystemParameters.VirtualScreenHeight * currentDpi.DpiY / 96);

        // Create a bitmap with the screen dimensions
        Bitmap bitmap = new Bitmap(screenWidth, screenHeight);

        // Capture the screen onto the bitmap
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(
                SystemInformation.VirtualScreen.X,
                SystemInformation.VirtualScreen.Y,
                0,
                0,
                SystemInformation.VirtualScreen.Size,
                CopyPixelOperation.SourceCopy);
        }

        return bitmap;
    }
}
