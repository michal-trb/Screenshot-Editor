using screenerWpf.Properties;
using screenerWpf.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace screenerWpf.Sevices.CaptureServices
{
    public class WindowScreenshot
    {
        private bool IsSelectionMade = false;
        private TaskCompletionSource<bool> selectionCompleted = new TaskCompletionSource<bool>();
        private System.Timers.Timer cursorUpdateTimer;

        public async Task CaptureWithScrollAsync()
        {
            // Uruchom wyróżnianie okna pod kursorem

            // Wybierz okno
            IntPtr windowHandle = HighlightAndSelectWindow();

            selectionCompleted.SetResult(true); // Zaznacz, że wybór został dokonany

            if (windowHandle == IntPtr.Zero)
                return;

            var overlay = new OverlayWindow(windowHandle);
            var cts = new CancellationTokenSource();
            overlay.StopRequested += (s, e) => cts.Cancel();
            overlay.Show();

            List<Bitmap> screenshots = new List<Bitmap>();
            try
            {
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

                    // Oczekiwanie z możliwością anulowania
                    await Task.Delay(1000, cts.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // Zadanie zostało anulowane
            }
            finally
            {
                overlay.Close();
            }

            Bitmap finalImage = CombineScreenshots(screenshots);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"screenshot_{timestamp}.png";
            string path = Path.Combine(Settings.Default.ScreenshotsSavePath, fileName);
            finalImage.Save(path);
            finalImage.Dispose();

            foreach (var bmp in screenshots)
            {
                bmp.Dispose();
            }
        }

        public static Bitmap CaptureWindow(IntPtr hWnd)
        {
            // Uzyskaj prawidłowe wymiary okna z uwzględnieniem DPI
            GetWindowRect(hWnd, out RECT windowRect);
            Rectangle rect = RectToRectangle(windowRect);
            int borderWidth = 5;

            // Możesz potrzebować dodatkowych wywołań API, aby prawidłowo obsłużyć DPI
            int width = rect.Width - 2 * borderWidth;
            int height = rect.Height - 2 * borderWidth;

            // Stwórz bitmapę o zmniejszonych wymiarach, aby pominąć obwódkę
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.Left + borderWidth, rect.Top + borderWidth, 0, 0, new Size(width, height));
            }
            return bmp;
        }

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

        private IntPtr HighlightAndSelectWindow()
        {
            var overlay = new OverlayWindow();
            overlay.Show();

            IntPtr lastWindowUnderCursor = IntPtr.Zero;
            IntPtr selectedWindow = IntPtr.Zero;

            // Kontynuuj pętlę, dopóki nie zostanie wykonane kliknięcie
            while (true)
            {
                POINT cursorPoint;
                GetCursorPos(out cursorPoint);
                IntPtr currentWindowUnderCursor = WindowFromPoint(cursorPoint);

                // Aktualizuj OverlayWindow, jeśli wykryto nowe okno pod kursorem
                if (currentWindowUnderCursor != lastWindowUnderCursor)
                {
                    overlay.UpdatePositionAndSize(currentWindowUnderCursor);
                    lastWindowUnderCursor = currentWindowUnderCursor;
                }

                // Sprawdź, czy nastąpiło kliknięcie lewym przyciskiem myszy
                if ((GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0)
                {
                    selectedWindow = currentWindowUnderCursor;
                    break; // Zakończ pętlę po kliknięciu
                }

                Thread.Sleep(10); // Krótkie opóźnienie, aby zmniejszyć obciążenie procesora
            }

            overlay.Close();
            return selectedWindow;
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

            int sampleStep = 10; // Próbka co 10 pikseli
            for (int y = 0; y < bmp1.Height; y += sampleStep)
            {
                for (int x = 0; x < bmp1.Width; x += sampleStep)
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
        private static extern IntPtr WindowFromPoint(Point Point);
        private static Rectangle RectToRectangle(RECT rect)
        {
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        private const uint INPUT_KEYBOARD = 1;
        private const ushort VK_PGDN = 0x22; // Klawisz strony w dół
        private const uint KEYEVENTF_KEYUP = 0x0002;

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        private struct INPUT
        {
            public uint type;
            public InputUnion U;
            public static int Size => Marshal.SizeOf(typeof(INPUT));
        }

        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        // P/Invoke deklaracje
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private const int VK_LBUTTON = 0x01; // Kod klawisza dla lewego przycisku myszy
    }
}
