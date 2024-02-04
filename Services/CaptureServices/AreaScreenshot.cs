using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

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
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        public static Bitmap CaptureScreen()
        {
            IntPtr desktopWindow = GetDesktopWindow();
            IntPtr desktopDC = GetWindowDC(desktopWindow);

            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);

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
