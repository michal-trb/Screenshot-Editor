using System;
using System.Windows;
using System.Windows.Input;
using screenerWpf.Controls;
using screenerWpf.Models.DrawableElements;

namespace screenerWpf.CanvasHandler.Drawers
{ 
    public class RectangleDrawer : DrawableElementDrawer
    {
        private DrawableRectangle CurrentRectangle { get; set; }
        private Point startingPoint;

        public RectangleDrawer(DrawableCanvas canvas) : base(canvas)
        {
        }

        public override void StartDrawing(MouseButtonEventArgs e)
        {
            startingPoint = e.GetPosition(DrawableCanvas);
            CurrentRectangle = new DrawableRectangle
            {
                Position = startingPoint,
                Size = new Size(0, 0),
                StrokeColor = CanvasInputHandler.GetCurrentColor(),
                StrokeThickness = CanvasInputHandler.GetCurrentThickness(),
            };
            DrawableCanvas.AddElement(CurrentRectangle);
        }

        public override void UpdateDrawing(MouseEventArgs e)
        {
            if (CurrentRectangle == null) return;

            var currentPoint = e.GetPosition(DrawableCanvas);

            // Obliczanie lewego górnego narożnika prostokąta
            double left = Math.Min(currentPoint.X, startingPoint.X);
            double top = Math.Min(currentPoint.Y, startingPoint.Y);

            // Obliczanie szerokości i wysokości prostokąta
            double width = Math.Abs(currentPoint.X - startingPoint.X);
            double height = Math.Abs(currentPoint.Y - startingPoint.Y);

            CurrentRectangle.Position = new Point(left, top);
            CurrentRectangle.Size = new Size(width, height);

            DrawableCanvas.InvalidateVisual();
        }

        public override void FinishDrawing()
        {
            CurrentRectangle = null;
        }
    }

}
