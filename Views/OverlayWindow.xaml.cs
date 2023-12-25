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
        private ControlWindow controlWindow;

        public OverlayWindow(IntPtr targetWindowHandle)
        {
            InitializeComponent();
            SetWindowPosAndSize(targetWindowHandle);
            OpenControlWindow();
            this.Closed += OverlayWindow_Closed; // Dodanie obsługi zdarzenia zamknięcia
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

            int borderWidth = 2;

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
            controlWindow = new ControlWindow();
            controlWindow.StopRequested += ControlWindow_StopRequested;
            controlWindow.Show();

            controlWindow.Left = this.Left;
            controlWindow.Top = this.Top + this.Height;
        }

        private void ControlWindow_StopRequested(object sender, EventArgs e)
        {
            StopRequested?.Invoke(this, EventArgs.Empty);
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

        private void OverlayWindow_Closed(object sender, EventArgs e)
        {
            // Zamknij ControlWindow jeśli jest otwarte
            if (controlWindow != null)
            {
                controlWindow.Close();
            }
        }
    }
}
