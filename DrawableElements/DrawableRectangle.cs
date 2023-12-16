using System.Windows;
using System.Windows.Media;

namespace screenerWpf
{
    public class DrawableRectangle : DrawableElement
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public Color StrokeColor { get; set; }
        public double StrokeThickness { get; set; }

        public DrawableRectangle()
        {
            Position = new Point(0, 0);
            Size = new Size(100, 50); // Domyślny rozmiar
            StrokeColor = Colors.Black; // Domyślny kolor obrysu
            StrokeThickness = 1.0; // Domyślna grubość obrysu
        }

        public override void Draw(DrawingContext context)
        {
            if (context == null)
                throw new System.ArgumentNullException("context");

            Rect rect = new Rect(Position, Size);
            Brush fillBrush = new SolidColorBrush();
            Pen strokePen = new Pen(new SolidColorBrush(StrokeColor), StrokeThickness);

            context.DrawRectangle(fillBrush, strokePen, rect);

            // Rysowanie ramki selekcyjnej, jeśli element jest zaznaczony
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
