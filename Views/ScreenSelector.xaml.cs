using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WinForms = System.Windows.Forms;
using Drawing = System.Drawing;
using Helpers.DpiHelper;
using WPFPoint = System.Windows.Point;
using WPFPen = System.Windows.Media.Pen;
using WPFBrushes = System.Windows.Media.Brushes;
using WPFColor = System.Windows.Media.Color;
using WPFKeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace screenerWpf.Views
{
    public partial class ScreenSelector : Window
    {
        private Drawing.Point selectedCursorPosition;
        private WinForms.Screen currentScreen;

        public ScreenSelector()
        {
            InitializeComponent();

            this.Left = SystemParameters.VirtualScreenLeft;
            this.Top = SystemParameters.VirtualScreenTop;
            this.Width = SystemParameters.VirtualScreenWidth;
            this.Height = SystemParameters.VirtualScreenHeight;

            this.WindowStyle = WindowStyle.None;
            this.Topmost = true;
            this.Opacity = 1;
            this.Background = new SolidColorBrush(Colors.White) { Opacity = 0.1 };
            this.AllowsTransparency = true;
            this.ShowInTaskbar = false;

            this.MouseMove += ScreenSelector_MouseMove;
            this.MouseDown += ScreenSelector_MouseDown;
            this.KeyDown += ScreenSelector_KeyDown;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Pobierz informacje o przestrzeni wirtualnej
            var virtualScreenLeft = SystemParameters.VirtualScreenLeft;
            var virtualScreenTop = SystemParameters.VirtualScreenTop;

            // Konwertuj pozycjê kursora do wspó³rzêdnych systemowych
            var systemCursorPosition = new Drawing.Point(
                selectedCursorPosition.X + (int)virtualScreenLeft,
                selectedCursorPosition.Y + (int)virtualScreenTop
            );

            foreach (WinForms.Screen screen in WinForms.Screen.AllScreens)
            {
                Rect screenRect = new Rect(
                    screen.Bounds.X - virtualScreenLeft,  // Konwertuj do przestrzeni wirtualnej
                    screen.Bounds.Y - virtualScreenTop,   // Konwertuj do przestrzeni wirtualnej
                    screen.Bounds.Width,
                    screen.Bounds.Height
                );

                // SprawdŸ, czy kursor jest na tym ekranie
                bool isActiveScreen = screen.Bounds.Contains(systemCursorPosition);

                if (isActiveScreen)
                {
                    // Aktywny ekran jest przezroczysty
                    drawingContext.DrawRectangle(WPFBrushes.Transparent, null, screenRect);
                }
                else
                {
                    // Nieaktywne ekrany s¹ przyciemnione
                    drawingContext.DrawRectangle(new SolidColorBrush(WPFColor.FromArgb(128, 0, 0, 0)), null, screenRect);
                }
            }
        }

        private void ScreenSelector_MouseMove(object sender, MouseEventArgs e)
        {
            WPFPoint mousePosition = e.GetPosition(this);
            WinForms.Screen newScreen = WinForms.Screen.FromPoint(new Drawing.Point((int)mousePosition.X, (int)mousePosition.Y));

            if (newScreen != currentScreen)
            {
                currentScreen = newScreen;
                this.InvalidateVisual();
            }

            selectedCursorPosition = new Drawing.Point((int)mousePosition.X, (int)mousePosition.Y);
            this.InvalidateVisual();
        }

        private void ScreenSelector_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ScreenSelector_KeyDown(object sender, WPFKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        public Drawing.Point GetSelectedCursorPosition()
        {
            return selectedCursorPosition;
        }
    }
}
