using System;
using System.Windows;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace screenerWpf.Views
{
    public partial class OverlayWindow : Window
    {
        public event EventHandler StopRequested;

        public OverlayWindow(IntPtr targetWindowHandle)
        {
            InitializeComponent();
            SetWindowPosAndSize(targetWindowHandle);
            OpenControlWindow();
        }

        private void SetWindowPosAndSize(IntPtr targetWindowHandle)
        {
            GetWindowRect(targetWindowHandle, out RECT rect);

            this.Left = rect.Left;
            this.Top = rect.Top;
            this.Width = rect.Right - rect.Left;
            this.Height = rect.Bottom - rect.Top;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_LAYERED);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            int borderWidth = 2; // Szerokość obwódki

            // Rysuj czerwoną obwódkę wokół okna
            drawingContext.DrawRectangle(
                Brushes.Transparent,
                new Pen(Brushes.Red, borderWidth),
                new Rect(
                    borderWidth / 2.0,
                    borderWidth / 2.0,
                    this.Width - borderWidth,
                    this.Height - borderWidth));
        }

        private void OpenControlWindow()
        {
            var controlWindow = new ControlWindow();
            controlWindow.StopRequested += ControlWindow_StopRequested;
            controlWindow.Show();

            // Ustaw pozycję controlWindow względem OverlayWindow
            controlWindow.Left = this.Left; // Możesz dostosować te wartości
            controlWindow.Top = this.Top + this.Height; // Umieść pod OverlayWindow
        }

        private void ControlWindow_StopRequested(object sender, EventArgs e)
        {
            StopRequested?.Invoke(this, EventArgs.Empty);
            // Tutaj umieść dodatkową logikę potrzebną po zatrzymaniu
        }

        // P/Invoke deklaracje
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
    }
}
