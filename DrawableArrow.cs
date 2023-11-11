using System;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf
{
    public class DrawableArrow : DrawableElement
    {
        public Point EndPoint { get; set; }
        public double Thickness { get; internal set; }

        public override void Draw(DrawingContext context)
        {
            if (context == null)
                throw new System.ArgumentNullException("context");

            // Draw the arrow line
            Pen pen = new Pen(new SolidColorBrush(Color), Thickness);
            context.DrawLine(pen, Position, EndPoint);
            DrawArrowHead(context, Position, EndPoint, Thickness+5, Thickness+10); // szerokość i wysokość główki do dostosowania
            if (IsSelected)
            {
                Pen selectionPen = new Pen(Brushes.Red, 2); // Stwórz długopis do rysowania obwódki
                Rect boundingBox = GetBounds(); // Musisz zaimplementować tę metodę aby zwróciła prostokąt otaczający element
                context.DrawRectangle(null, selectionPen, boundingBox);
            }
        }

        private void DrawArrowHead(DrawingContext context, Point startPoint, Point endPoint, double headWidth, double headHeight)
        {
            // Kierunek strzałki
            Vector direction = endPoint - startPoint;
            direction.Normalize();

            // Punkty na główce strzałki
            Point point1 = endPoint - direction * headHeight + new Vector(-direction.Y, direction.X) * headWidth / 2;
            Point point2 = endPoint - direction * headHeight - new Vector(-direction.Y, direction.X) * headWidth / 2;

            // Rysowanie główki strzałki
            PathFigure pathFigure = new PathFigure
            {
                StartPoint = point1,
                IsClosed = true // Zamknięcie ścieżki
            };
            pathFigure.Segments.Add(new LineSegment(endPoint, true));
            pathFigure.Segments.Add(new LineSegment(point2, true));

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            context.DrawGeometry(Brushes.Black, new Pen(Brushes.Black, 1), pathGeometry);
        }

        public override bool HitTest(Point point)
        {
            // Implement hit testing logic for arrow
            // This is a simple example and doesn't account for line thickness, etc.
            double buffer = 5.0; // Hit test buffer
            LineGeometry geometry = new LineGeometry(Position, EndPoint);
            return geometry.StrokeContains(new Pen(Brushes.Black, buffer), point);
        }

        public override Rect GetBounds()
        {
            // Pobierz szerokość główki strzałki, załóżmy że to stała wartość
            double arrowHeadWidth = 10.0; // przykładowa szerokość główki strzałki

            // Oblicz minimalne i maksymalne X i Y dla lini strzałki
            double minX = Math.Min(Position.X, EndPoint.X) - arrowHeadWidth;
            double minY = Math.Min(Position.Y, EndPoint.Y) - arrowHeadWidth;
            double maxX = Math.Max(Position.X, EndPoint.X) + arrowHeadWidth;
            double maxY = Math.Max(Position.Y, EndPoint.Y) + arrowHeadWidth;

            // Utwórz i zwróć prostokąt ograniczający
            return new Rect(new Point(minX, minY), new Point(maxX, maxY));
        }

        public override bool Contains(Point point)
        {
            double buffer = 5.0; // Możesz dostosować bufor do grubości strzałki
            LineGeometry lineGeometry = new LineGeometry(Position, EndPoint);
            return lineGeometry.StrokeContains(new Pen(Brushes.Black, buffer), point);
        }
    }
}
