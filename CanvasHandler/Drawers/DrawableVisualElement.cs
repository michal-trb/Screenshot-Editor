using System.Windows;
using System.Windows.Media;

namespace screenerWpf.CanvasHandler.Drawers
{
    internal class DrawableVisualElement : DrawableElement
    {
        public DrawingVisual Visual { get; set; }
        public Point Position { get; set; }
        public Size Size { get; set; }

        public override void Draw(DrawingContext context)
        {
            Rect rect = new Rect(Position, Size);

            if (Visual != null && Visual.Drawing != null)
            {
                // Rysowanie zawartości Visual na kontekście DrawingContext
                context.DrawDrawing(Visual.Drawing);
            }
            if (IsSelected)
            {
                Pen selectionPen = new Pen(Brushes.Red, 2);
                context.DrawRectangle(null, selectionPen, rect);
            }
        }

        public override Rect GetBounds()
        {
            return new Rect(Position, Size);
        }

        public override bool HitTest(Point point)
        {
            Rect bounds = GetBounds();
            return bounds.Contains(point);
        }

        public override void Move(Vector delta)
        {
            Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
        }
    }
}
