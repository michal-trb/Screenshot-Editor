using screenerWpf.Controls;
using screenerWpf.Models.DrawableElements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace screenerWpf.CanvasHandler.Drawers
{
    public class BlurDrawer : DrawableElementDrawer
    {
        private DrawableBlur CurrentBlurArea { get; set; }

        public BlurDrawer(DrawableCanvas canvas) : base(canvas)
        {
        }

        public override void StartDrawing(MouseButtonEventArgs e)
        {
            Point startPoint = e.GetPosition(DrawableCanvas);
            InitializeDrawableElements(startPoint);
        }

        public override void UpdateDrawing(MouseEventArgs e)
        {
            if (CurrentBlurArea == null) return;

            var currentPoint = e.GetPosition(DrawableCanvas);
            double width = Math.Abs(currentPoint.X - CurrentBlurArea.Position.X);
            double height = Math.Abs(currentPoint.Y - CurrentBlurArea.Position.Y);

            CurrentBlurArea.Size = new Size(width, height);
            CurrentBlurArea.UpdateVisual(); // Aktualizacja wizualna obszaru rozmycia

            DrawableCanvas.InvalidateVisual();
        }

        public override void FinishDrawing()
        {
            CurrentBlurArea = null;
        }

        private void InitializeDrawableElements(Point startPoint)
        {
            CurrentBlurArea = new DrawableBlur(DrawableCanvas)
            {
                Position = startPoint,
                Size = new Size(0, 0),
                StrokeColor = Colors.Transparent,
                StrokeThickness = 0,
                BlurEffect = new BlurEffect { Radius = 10 }
            };

            DrawableCanvas.AddElement(CurrentBlurArea);
        }
    }
}
