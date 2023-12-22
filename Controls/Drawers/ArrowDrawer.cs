using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using screenerWpf.Controls;
using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;

namespace screenerWpf.CanvasHandler.Drawers
{
    public class ArrowDrawer : DrawableElementDrawer
    {
        private DrawableArrow CurrentArrow { get; set; }

        public IDrawable CurrentDrawable { get; private set; }

        public ArrowDrawer(IDrawable currentDrawable, DrawableCanvas canvas) : base(canvas)
        {
            CurrentDrawable = currentDrawable;
        }

        public override void StartDrawing(MouseButtonEventArgs e)
        {
            Point location = e.GetPosition(DrawableCanvas);
            CurrentArrow = new DrawableArrow
            {
                Position = location,
                Color = CanvasInputHandler.GetCurrentColor(),
                Thickness = CanvasInputHandler.GetCurrentThickness(),
            };
            DrawableCanvas.AddElement(CurrentArrow);
        }

        public override void UpdateDrawing(MouseEventArgs e)
        {
            if (CurrentArrow == null) return;
            CurrentArrow.EndPoint = e.GetPosition(DrawableCanvas);
            DrawableCanvas.InvalidateVisual();
        }

        public override void FinishDrawing()
        {
            CurrentArrow = null;
        }
    }
}