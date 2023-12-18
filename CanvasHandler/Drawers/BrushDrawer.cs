using System.Windows.Input;
using System.Windows.Media;
using screenerWpf.DrawableElements;

namespace screenerWpf.CanvasHandler.Drawers
{
    public class BrushDrawer : DrawableElementDrawer
    {
        private DrawableBrush currentBrushStroke;

        public BrushDrawer(DrawableCanvas canvas) : base(canvas)
        {
        }

        public override void StartDrawing(MouseButtonEventArgs e)
        {
            var position = e.GetPosition(DrawableCanvas);
            currentBrushStroke = new DrawableBrush(
                CanvasInputHandler.GetCurrentColor(),
                CanvasInputHandler.GetCurrentThickness(),
                CanvasInputHandler.GetCurrentTransparency());  
            DrawableCanvas.AddElement(currentBrushStroke);
        }

        public override void UpdateDrawing(MouseEventArgs e)
        {
            if (currentBrushStroke == null) return;
            var position = e.GetPosition(DrawableCanvas);
            currentBrushStroke.AddPoint(position);
            DrawableCanvas.InvalidateVisual();
        }

        public override void FinishDrawing()
        {
            currentBrushStroke = null;
        }
    }
}
