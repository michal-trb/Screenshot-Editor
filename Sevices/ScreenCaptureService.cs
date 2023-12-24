using System.Drawing;
using screenerWpf.Interfaces;
using System.Management;
using System;
using Size = System.Drawing.Size;
using Point = System.Drawing.Point;
using System.IO;
using System.Windows;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace screenerWpf.Sevices
{
    public class ScreenCaptureService : IScreenCaptureService
    {
        private ScreenRecorder screenRecorder;
        private IWindowService windowService;

        public ScreenCaptureService(IWindowService windowService)
        {
            this.windowService = windowService;
            screenRecorder = new ScreenRecorder();
            screenRecorder.RecordingCompleted += OnRecordingCompleted;
        }

        private void OnRecordingCompleted(string filePath)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                windowService.ShowVideoPlayerWindow(filePath); // Otwarcie okna odtwarzacza wideo
            });
        }

        public Bitmap CaptureScreen()
        {
            ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_VideoController");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    int screenWidth = Convert.ToInt32(obj["CurrentHorizontalResolution"]);
                    int screenHeight = Convert.ToInt32(obj["CurrentVerticalResolution"]);

                    Bitmap bitmap = new Bitmap(screenWidth, screenHeight);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                    }
                    return bitmap;
                }
            }

            return null; // Jeśli nie uda się uzyskać informacji o rozdzielczości
        }

        public Bitmap CaptureArea(Rectangle area)
        {
            Bitmap bitmap = new Bitmap(area.Width, area.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(area.Location, Point.Empty, area.Size);
            }
            return bitmap;
        }

        public void StartRecording()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            string fileName = $"Recording_{timestamp}.mp4";

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), fileName);

            screenRecorder.StartRecording(filePath);
        }


        public void StopRecording()
        {
            screenRecorder.StopRecording();
        }

        public void StartAreaRecording(Rectangle area)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            string fileName = $"Recording_{timestamp}.mp4";

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), fileName);

            screenRecorder.StartRecordingArea(filePath, area);
        }

        public void CaptureWithScroll()
        {
            IntPtr windowHandle = SelectWindowWithMouse();
            if (windowHandle == IntPtr.Zero)
                return;

            List<Bitmap> screenshots = new List<Bitmap>();

            while (true)
            {
                Bitmap screenshot = CaptureWindow(windowHandle);
                if (screenshots.Count > 0 && CompareBitmaps(screenshots.Last(), screenshot))
                {
                    screenshot.Dispose();
                    break;
                }

                screenshots.Add(screenshot);
                ScrollDown(windowHandle);
                Thread.Sleep(1000);
            }

            // Teraz połącz wszystkie zrzuty ekranu
            Bitmap finalImage = CombineScreenshots(screenshots);
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "final_screenshot.png");
            finalImage.Save(path);
            finalImage.Dispose();

            // Wyczyść listę zrzutów ekranu
            foreach (var bmp in screenshots)
            {
                bmp.Dispose();
            }
        }

        private Bitmap CombineScreenshots(List<Bitmap> screenshots)
        {
            if (screenshots.Count == 0) return null;

            int width = screenshots[0].Width;
            int height = screenshots.Sum(bmp => bmp.Height);

            Bitmap finalImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                int currentY = 0;
                foreach (var bmp in screenshots)
                {
                    g.DrawImage(bmp, 0, currentY);
                    currentY += bmp.Height;
                }
            }

            return finalImage;
        }

        private bool CompareBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Size != bmp2.Size)
            {
                return false;
            }

            for (int y = 0; y < bmp1.Height; y++)
            {
                for (int x = 0; x < bmp1.Width; x++)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point Point);

        public IntPtr SelectWindowWithMouse()
        {
            MessageBox.Show("Po zamknięciu tego okna kliknij na okno, które chcesz przewinąć.", "Wybierz okno", MessageBoxButton.OK, MessageBoxImage.Information);
            Thread.Sleep(1000);
            Point cursorPoint;
            GetCursorPos(out cursorPoint);
            return WindowFromPoint(cursorPoint);
        }

        const int SB_TOP = 6;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const uint WM_VSCROLL = 0x0115;
        const int SB_PAGEBOTTOM = 7;
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        struct INPUT
        {
            public uint type;
            public InputUnion U;
            public static int Size => Marshal.SizeOf(typeof(INPUT));
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        const uint INPUT_KEYBOARD = 1;
        const ushort VK_DOWN = 0x28; // Klawisz strzałki w dół
        const ushort VK_PGDN = 0x22; // Klawisz strony w dół
        const uint KEYEVENTF_KEYUP = 0x0002;

        public static void ScrollDown(IntPtr hWnd)
        {
            INPUT input = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = VK_PGDN,
                        dwFlags = 0
                    }
                }
            };

            for (int i = 0; i < 3; i++) // Symuluj 3 naciśnięcia klawisza Page Down
            {
                SendInput(1, ref input, INPUT.Size);
                input.U.ki.dwFlags = KEYEVENTF_KEYUP;
                SendInput(1, ref input, INPUT.Size);
                Thread.Sleep(150); // Krótkie opóźnienie między naciśnięciami
            }
        }


        public static void SaveScreenshot(Bitmap screenshot, string filePath)
        {
            screenshot.Save(filePath);
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetScrollInfo(IntPtr hWnd, int nBar, ref SCROLLINFO lpsi);

        [DllImport("user32.dll")]
        static extern int GetScrollPos(IntPtr hWnd, int nBar);

        const int SB_VERT = 1;

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLINFO
        {
            public int cbSize;
            public int fMask;
            public int min;
            public int max;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }


        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rect);

        public static Bitmap CaptureWindow(IntPtr hWnd)
        {
            // Pobierz wymiary okna
            Rectangle windowRect;
            GetWindowRect(hWnd, out windowRect);
            int width = windowRect.Width - windowRect.X;
            int height = windowRect.Height - windowRect.Y;

            // Stwórz bitmapę o tych wymiarach
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Zrób zrzut ekranu
                g.CopyFromScreen(windowRect.X, windowRect.Y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }
            return bmp;
        }
    }
}
