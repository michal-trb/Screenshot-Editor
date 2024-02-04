using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf
{
    public partial class AreaSelector : Window
    {
        public Rect SelectedRectangle { get; private set; }
        private Point startPoint;
        private Point currentPoint; 

        public AreaSelector()
        {
            InitializeComponent();

            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            this.Topmost = true;
            this.Opacity = 1;
            this.Background = new SolidColorBrush(Colors.White) { Opacity = 0.1 };
            this.AllowsTransparency = true;
            this.ShowInTaskbar = false;

            this.MouseDown += Grid_MouseDown;
            this.MouseMove += Grid_MouseMove;
            this.MouseUp += Grid_MouseUp;
            this.KeyDown += Grid_KeyDown;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Rect fullRect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

            CombinedGeometry combinedGeometry = new CombinedGeometry
            {
                Geometry1 = new RectangleGeometry(fullRect),
                Geometry2 = new RectangleGeometry(SelectedRectangle),
                GeometryCombineMode = GeometryCombineMode.Exclude
            };

            drawingContext.DrawGeometry(new SolidColorBrush(Colors.White) { Opacity = 0.1 }, null, combinedGeometry);

            // Rysowanie czerwonego prostokąta
            drawingContext.DrawRectangle(null, new Pen(Brushes.Red, 2), SelectedRectangle);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                if (this.IsVisible && !this.DialogResult.HasValue)
                {
                    this.DialogResult = true;
                }
                this.Close();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                startPoint = e.GetPosition(this);
                SelectedRectangle = new Rect(startPoint, new Size(0, 0));
                this.InvalidateVisual();
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(this); // Aktualizacja bieżącej pozycji kursora
                SelectedRectangle = new Rect(startPoint, currentPoint);
                this.InvalidateVisual();
            }
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (this.IsVisible)
                {
                    this.DialogResult = false;
                }
                this.Close();
            }
        }
    }
}
