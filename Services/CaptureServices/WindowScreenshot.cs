using screenerWpf.Properties;
using screenerWpf.Views;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace screenerWpf.Sevices.CaptureServices
{
    public class WindowScreenshot
    {
        public void CaptureSingleWindow()
        {
            IntPtr windowHandle = HighlightAndSelectWindow();
            if (windowHandle == IntPtr.Zero)
                return;

            Bitmap screenshot = CaptureWindow(windowHandle);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"screenshot_{timestamp}.png";
            string path = Path.Combine(Settings.Default.ScreenshotsSavePath, fileName);
            screenshot.Save(path);
            screenshot.Dispose();
        }

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
                    overlay.UpdatePositionAndSize(currentWindowUnderCursor);
                    lastWindowUnderCursor = currentWindowUnderCursor;
                }

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

        public static Bitmap CaptureWindow(IntPtr hWnd)
        {
            GetWindowRect(hWnd, out RECT windowRect);
            Rectangle rect = RectToRectangle(windowRect);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(rect.Width, rect.Height));
            }
            return bmp;
        }

        private static Rectangle RectToRectangle(RECT rect)
        {
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private const int VK_LBUTTON = 0x01; // Kod klawisza dla lewego przycisku myszy
    }
}
