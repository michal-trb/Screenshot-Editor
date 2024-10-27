namespace screenerWpf.Services.CaptureServices;

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using global::Helpers.DpiHelper;

/// <summary>
/// Provides functionality to capture the screen area where the cursor is currently located.
/// </summary>
public static class ScreenUnderCursorCapture
{
    /// <summary>
    /// Captures the screen area under the current position of the cursor.
    /// This method identifies the screen where the cursor is located and captures the corresponding screen's content,
    /// taking into account DPI scaling for accurate image resolution.
    /// </summary>
    /// <param name="cursorPosition">The current position of the cursor in virtual screen coordinates.</param>
    /// <returns>A <see cref="Bitmap"/> containing the captured screenshot of the area under the cursor.</returns>
    public static Bitmap CaptureScreenUnderCursor(Point cursorPosition)
    {
        DpiHelper.UpdateDpi();
        var currentDpi = DpiHelper.CurrentDpi;

        // Get the virtual screen dimensions and the offset for the virtual screen.
        var virtualScreenWidth = SystemInformation.VirtualScreen.Width;
        var virtualScreenLeft = SystemInformation.VirtualScreen.Left;

        // Adjust the cursor position relative to the virtual screen offset.
        var adjustedCursorPosition = new Point(
            cursorPosition.X + virtualScreenLeft,
            cursorPosition.Y
        );

        // Find the screen containing the adjusted cursor position or default to the primary screen.
        Screen targetScreen = Screen.AllScreens
            .FirstOrDefault(screen => screen.Bounds.Contains(adjustedCursorPosition))
            ?? Screen.PrimaryScreen;

        // Calculate the actual width and height of the screen considering the DPI scaling.
        int screenWidth = (int)(targetScreen.Bounds.Width * currentDpi.DpiX / 96);
        int screenHeight = (int)(targetScreen.Bounds.Height * currentDpi.DpiY / 96);

        // Define the area of the screen to capture.
        Rectangle captureArea = new Rectangle(
            targetScreen.Bounds.X - virtualScreenLeft,
            targetScreen.Bounds.Y,
            screenWidth,
            screenHeight
        );

        return AreaScreenshot.CaptureArea(captureArea);
    }
}
