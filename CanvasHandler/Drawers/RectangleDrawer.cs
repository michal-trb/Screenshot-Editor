using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf.CanvasHandler.Drawers
{
    public class RectangleDrawer : DrawableElementDrawer
    {
        private DrawableRectangle CurrentRectangle { get; set; }

        public RectangleDrawer(DrawableCanvas canvas) : base(canvas)
        {

        }

        public override void StartDrawing(MouseButtonEventArgs e)
        {
            Point startPoint = e.GetPosition(DrawableCanvas);
            CurrentRectangle = new DrawableRectangle
            {
                Position = startPoint,
                Color = CanvasInputHandler.GetCurrentColor(),
                Size = new Size(0, 0)
            };
            DrawableCanvas.AddElement(CurrentRectangle);
        }

        public override void UpdateDrawing(MouseEventArgs e)
        {
            if (CurrentRectangle == null) return;

            var currentPoint = e.GetPosition(DrawableCanvas);
            double width = Math.Abs(currentPoint.X - CurrentRectangle.Position.X);
            double height = Math.Abs(currentPoint.Y - CurrentRectangle.Position.Y);

            CurrentRectangle.Size = new Size(width, height);
            DrawableCanvas.InvalidateVisual();
        }

        public override void FinishDrawing()
        {
            CurrentRectangle = null;
        }
    }

}
