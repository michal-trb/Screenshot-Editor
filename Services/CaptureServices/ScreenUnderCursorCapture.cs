namespace screenerWpf.Services.CaptureServices;

using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using global::Helpers.DpiHelper;

public static class ScreenUnderCursorCapture
{
    public static Bitmap CaptureScreenUnderCursor(Point cursorPosition)
    {
        DpiHelper.UpdateDpi();
        var currentDpi = DpiHelper.CurrentDpi;

        var virtualScreenWidth = SystemInformation.VirtualScreen.Width;
        var virtualScreenLeft = SystemInformation.VirtualScreen.Left;

        var adjustedCursorPosition = new Point(
            cursorPosition.X + virtualScreenLeft,
            cursorPosition.Y
        );

        Screen targetScreen = Screen.AllScreens
            .FirstOrDefault(screen => screen.Bounds.Contains(adjustedCursorPosition))
            ?? Screen.PrimaryScreen;

        int screenWidth = (int)(targetScreen.Bounds.Width * currentDpi.DpiX / 96);
        int screenHeight = (int)(targetScreen.Bounds.Height * currentDpi.DpiY / 96);

        Rectangle captureArea = new Rectangle(
            targetScreen.Bounds.X - virtualScreenLeft, 
            targetScreen.Bounds.Y,
            screenWidth,
            screenHeight
        );

        return AreaScreenshot.CaptureArea(captureArea);
    }
}
