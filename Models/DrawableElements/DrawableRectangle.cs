using System;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{
    public class DrawableRectangle : DrawableWithHandles
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public Color StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        private enum DragHandle { None, TopLeft, TopRight, BottomLeft, BottomRight }
        private DragHandle currentDragHandle = DragHandle.None;
        public DrawableRectangle() : base(4)
        {
            Position = new Point(0, 0);
            Size = new Size(100, 50); // Domyślny rozmiar
            StrokeColor = Colors.Black; // Domyślny kolor obrysu
            StrokeThickness = 1.0; // Domyślna grubość obrysu
        }

        public override void Draw(DrawingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            // Najpierw rysuj elementy specyficzne dla tej klasy
            Rect rect = new Rect(Position, Size);
            Brush fillBrush = new SolidColorBrush();
            Pen strokePen = new Pen(new SolidColorBrush(StrokeColor), StrokeThickness);
            context.DrawRectangle(fillBrush, strokePen, rect);

            // Następnie rysuj uchwyty, jeśli element jest zaznaczony
            if (IsSelected)
            {
                UpdateHandlePoints(); // Aktualizuj pozycje uchwytów
                DrawSelectionHandles(context); // Narysuj uchwyty
            }
        }

        public override bool HitTest(Point point)
        {
            Rect rect = new Rect(Position, Size);
            currentDragHandle = DragHandle.None;

            if (IsNearCorner(point, Position)) currentDragHandle = DragHandle.TopLeft;
            else if (IsNearCorner(point, new Point(Position.X + Size.Width, Position.Y))) currentDragHandle = DragHandle.TopRight;
            else if (IsNearCorner(point, new Point(Position.X, Position.Y + Size.Height))) currentDragHandle = DragHandle.BottomLeft;
            else if (IsNearCorner(point, new Point(Position.X + Size.Width, Position.Y + Size.Height))) currentDragHandle = DragHandle.BottomRight;

            return rect.Contains(point) || currentDragHandle != DragHandle.None;
        }

        protected override void UpdateHandlePoints()
        {
            Rect rect = new Rect(Position, Size);
            HandlePoints[0] = rect.TopLeft;     // Lewy górny
            HandlePoints[1] = rect.TopRight;    // Prawy górny
            HandlePoints[2] = rect.BottomLeft;  // Lewy dolny
            HandlePoints[3] = rect.BottomRight; // Prawy dolny
        }

        private bool IsNearCorner(Point point, Point corner)
        {
            // Metoda sprawdzająca, czy punkt znajduje się blisko narożnika
            double tolerance = 10; // Możesz dostosować tolerancję
            return Math.Abs(point.X - corner.X) <= tolerance && Math.Abs(point.Y - corner.Y) <= tolerance;
        }

        public override Rect GetBounds()
        {
            return new Rect(Position, Size);
        }

        public override void Move(Vector delta)
        {
            switch (currentDragHandle)
            {
                case DragHandle.None:
                    Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
                    break;
                case DragHandle.TopLeft:
                    Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
                    Size = new Size(Size.Width - delta.X, Size.Height - delta.Y);
                    break;
                case DragHandle.TopRight:
                    Size = new Size(Size.Width + delta.X, Size.Height - delta.Y);
                    Position = new Point(Position.X, Position.Y + delta.Y);
                    break;
                case DragHandle.BottomLeft:
                    Size = new Size(Size.Width - delta.X, Size.Height + delta.Y);
                    Position = new Point(Position.X + delta.X, Position.Y);
                    break;
                case DragHandle.BottomRight:
                    Size = new Size(Size.Width + delta.X, Size.Height + delta.Y);
                    break;
            }
        }
    }
}
