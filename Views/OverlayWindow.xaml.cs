using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace screenerWpf.Views
{
    /// <summary>
    /// Logika interakcji dla klasy OverlayWindow.xaml
    /// </summary>
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Media;

    public partial class OverlayWindow : Window
    {
        public event EventHandler StopRequested;

        public OverlayWindow(IntPtr targetWindowHandle)
        {
            InitializeComponent();
            SetWindowPosAndSize(targetWindowHandle);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SetWindowPosAndSize(IntPtr targetWindowHandle)
        {
            GetWindowRect(targetWindowHandle, out RECT rect);
            int buttonHeight = 30; // Wysokość przycisku "Stop"

            this.Left = rect.Left;
            this.Top = rect.Top;
            this.Width = rect.Right - rect.Left;
            this.Height = rect.Bottom - rect.Top + buttonHeight; // Dodaj wysokość przycisku do wysokości okna
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            int borderWidth = 2; // Szerokość obwódki
            int buttonHeight = 30; // Wysokość przycisku "Stop"

            // Rysuj czerwoną obwódkę wokół okna, ale z dodatkowym obszarem na przycisk
            drawingContext.DrawRectangle(
                Brushes.Transparent,
                new Pen(Brushes.Red, borderWidth),
                new Rect(
                    borderWidth / 2.0,
                    borderWidth / 2.0,
                    this.Width - borderWidth,
                    this.Height - borderWidth - buttonHeight)); // Obwódka obejmuje całą wysokość okna, włącznie z dodatkowym obszarem
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
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;
    }

}
