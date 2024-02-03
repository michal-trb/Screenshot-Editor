using screenerWpf.Controls;
using screenerWpf.Models.DrawableElements;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace screenerWpf.CanvasHandler.Drawers
{
    public class BlurDrawer : DrawableElementDrawer
    {
        private DrawableBlur CurrentBlurArea { get; set; }
        private Point startDragPoint;

        public BlurDrawer(DrawableCanvas canvas) : base(canvas)
        {
        }

        public override void StartDrawing(MouseButtonEventArgs e)
        {
            startDragPoint = e.GetPosition(DrawableCanvas);
            InitializeDrawableElements(startDragPoint);
        }

        public override void UpdateDrawing(MouseEventArgs e)
        {
            if (CurrentBlurArea == null) return;

            var currentPoint = e.GetPosition(DrawableCanvas);
            UpdateBlurArea(currentPoint);

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

        private void UpdateBlurArea(Point currentPoint)
        {
            var x = Math.Min(currentPoint.X, startDragPoint.X);
            var y = Math.Min(currentPoint.Y, startDragPoint.Y);
            var width = Math.Abs(currentPoint.X - startDragPoint.X);
            var height = Math.Abs(currentPoint.Y - startDragPoint.Y);

            CurrentBlurArea.Position = new Point(x, y);
            CurrentBlurArea.Size = new Size(width, height);
            CurrentBlurArea.UpdateVisual();
        }
    }
}
