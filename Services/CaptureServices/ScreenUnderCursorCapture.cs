namespace screenerWpf.Services.CaptureServices;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using global::Helpers.DpiHelper;

public static class ScreenUnderCursorCapture
{
    public static Bitmap CaptureScreenUnderCursor(Point cursorPosition)
    {
        DpiHelper.UpdateDpi();
        var currentDpi = DpiHelper.CurrentDpi;

        // Znajdź monitor, na którym znajduje się kursor
        Screen screenWithCursor = Screen.FromPoint(cursorPosition);

        // Oblicz rzeczywiste wymiary ekranu z uwzględnieniem DPI
        int screenWidth = (int)(screenWithCursor.Bounds.Width * currentDpi.DpiX / 96);
        int screenHeight = (int)(screenWithCursor.Bounds.Height * currentDpi.DpiY / 96);

        // Utwórz prostokąt reprezentujący obszar ekranu do przechwycenia
        Rectangle captureArea = new Rectangle(
            screenWithCursor.Bounds.X,
            screenWithCursor.Bounds.Y,
            screenWidth,
            screenHeight
        );

        // Użyj metody CaptureArea z klasy AreaScreenshot do przechwycenia ekranu
        return AreaScreenshot.CaptureArea(captureArea);
    }
}
