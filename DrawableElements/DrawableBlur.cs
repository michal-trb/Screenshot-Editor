using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace screenerWpf.DrawableElements
{
    public class DrawableBlur : DrawableElement
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public Color StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        public Effect BlurEffect { get; set; }

        public DrawableBlur()
        {
            Position = new Point(0, 0);
            Size = new Size(100, 50);
            StrokeColor = Colors.Transparent;
            StrokeThickness = 1.0;
            BlurEffect = new BlurEffect
            {
                Radius = 100 // Ustawienie promienia rozmycia
            };
        }

        public override void Draw(DrawingContext context)
        {
            Rect rect = new Rect(Position, Size);
            Brush fillBrush = new SolidColorBrush(Colors.Transparent);
            Pen strokePen = new Pen(new SolidColorBrush(StrokeColor), StrokeThickness);

            // Zastosowanie efektu rozmycia
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawRectangle(fillBrush, strokePen, rect);
            }

            // Ustawienie efektu rozmycia na wizualnym elemencie
            if (BlurEffect != null)
            {
                visual.Effect = BlurEffect;
            }

            context.DrawDrawing(visual.Drawing);
            if (IsSelected)
            {
                Pen selectionPen = new Pen(Brushes.Red, 2);
                context.DrawRectangle(null, selectionPen, rect);
            }
        }

        public override bool HitTest(Point point)
        {
            Rect rect = new Rect(Position, Size);
            return rect.Contains(point);
        }

        public override Rect GetBounds()
        {
            return new Rect(Position, Size);
        }

        public override void Move(Vector delta)
        {
            Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
        }
    }
}
