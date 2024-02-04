using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

namespace screenerWpf.Services.CaptureServices
{
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

        public static Bitmap CaptureScreen()
        {
            IntPtr desktopWindow = GetDesktopWindow();
            IntPtr desktopDC = GetWindowDC(desktopWindow);

            // Pobieranie wymiarów całego obszaru roboczego
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

        public static Bitmap CaptureArea(Rectangle area)
        {
            return CropBitmap(CaptureScreen(), area);
        }

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
}
