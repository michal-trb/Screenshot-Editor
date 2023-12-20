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

namespace screenerWpf
{
    /// <summary>
    /// Logika interakcji dla klasy AreaSelectionWindow.xaml
    /// </summary>
    public partial class AreaSelectionWindow : Window
    {
        private Point startPoint;
        private Rectangle selectionRectangle;

        public delegate void AreaSelectedHandler(Rect rect);
        public event AreaSelectedHandler AreaSelected;

        public AreaSelectionWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            Topmost = true;
            Background = new SolidColorBrush(Color.FromArgb(120, 0, 0, 0)); // Półprzezroczyste tło

            MouseDown += AreaSelectionWindow_MouseDown;
            MouseMove += AreaSelectionWindow_MouseMove;
            MouseUp += AreaSelectionWindow_MouseUp;

            selectionRectangle = new Rectangle
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                Fill = Brushes.Transparent,
                Visibility = Visibility.Collapsed
            };
            Content = selectionRectangle;
        }

        private void AreaSelectionWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(this);
            selectionRectangle.Width = 0;
            selectionRectangle.Height = 0;
            selectionRectangle.Visibility = Visibility.Visible;
            Canvas.SetLeft(selectionRectangle, startPoint.X);
            Canvas.SetTop(selectionRectangle, startPoint.Y);
        }

        private void AreaSelectionWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var endPoint = e.GetPosition(this);
                var x = Math.Min(endPoint.X, startPoint.X);
                var y = Math.Min(endPoint.Y, startPoint.Y);
                var width = Math.Max(endPoint.X, startPoint.X) - x;
                var height = Math.Max(endPoint.Y, startPoint.Y) - y;

                Canvas.SetLeft(selectionRectangle, x);
                Canvas.SetTop(selectionRectangle, y);
                selectionRectangle.Width = width;
                selectionRectangle.Height = height;
            }
        }

        private void AreaSelectionWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var rect = new Rect(Canvas.GetLeft(selectionRectangle), Canvas.GetTop(selectionRectangle),
                                selectionRectangle.Width, selectionRectangle.Height);
            AreaSelected?.Invoke(rect);
            Close();
        }
    }
}
