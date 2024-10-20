namespace Helpers.DpiHelper;

using System;
using System.Drawing;
using System.Windows;

public static class DpiHelper
{
    /// <summary>
    /// Property to hold the current DPI values
    /// </summary>
    public static (double DpiX, double DpiY) CurrentDpi { get; private set; }

    /// <summary>
    /// Static constructor to initialize DPI values
    /// </summary>
    static DpiHelper()
    {
        UpdateDpi();
    }

    /// <summary>
    /// Method to update the DPI values
    /// </summary>
    public static void UpdateDpi()
    {
        var presentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);

        if (presentationSource != null && presentationSource.CompositionTarget != null)
        {
            double dpiX = presentationSource.CompositionTarget.TransformToDevice.M11 * 96;
            double dpiY = presentationSource.CompositionTarget.TransformToDevice.M22 * 96;
            CurrentDpi = (dpiX, dpiY);
        }
        else
        {
            /// <summary>
            /// If PresentationSource is not available, use Graphics to get default system DPI
            /// </summary>
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                double dpiX = graphics.DpiX;
                double dpiY = graphics.DpiY;
                CurrentDpi = (dpiX, dpiY);
            }
        }
    }

    /// <summary>
    /// Method to convert a size or value from device-independent units to pixels
    /// </summary>
    /// <param name="valueInDip">The value in device-independent units (DIP)</param>
    /// <returns>The value converted to pixels</returns>
    public static double ConvertToPixels(double valueInDip)
    {
        return valueInDip * (CurrentDpi.DpiX / 96.0);
    }

    /// <summary>
    /// Method to convert from pixels to device-independent units (DIP)
    /// </summary>
    /// <param name="valueInPixels">The value in pixels</param>
    /// <returns>The value converted to device-independent units (DIP)</returns>
    public static double ConvertToDip(double valueInPixels)
    {
        return valueInPixels * (96.0 / CurrentDpi.DpiX);
    }
}