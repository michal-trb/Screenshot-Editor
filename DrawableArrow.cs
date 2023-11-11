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

            // Obliczenie długości i szerokości główki strzałki
            double headLength = Thickness + 10; // Możesz dostosować te wartości
            double headWidth = Thickness + 5;

            // Kierunek strzałki
            Vector direction = EndPoint - Position;
            direction.Normalize();

            // Obliczenie nowego punktu końcowego dla linii, tak aby nie nachodziła na główkę
            Point newEndPoint = EndPoint - direction * headLength;

            // Rysowanie linii strzałki
            Pen pen = new Pen(new SolidColorBrush(Color), Thickness);
            context.DrawLine(pen, Position, newEndPoint);

            // Rysowanie główki strzałki
            DrawArrowHead(context, newEndPoint, EndPoint, headWidth, headLength, Color);

            if (IsSelected)
            {
                Pen selectionPen = new Pen(Brushes.Red, 2); // Długopis do rysowania obwódki
                Rect boundingBox = GetBounds();
                context.DrawRectangle(null, selectionPen, boundingBox);
            }
        }

        private void DrawArrowHead(
            DrawingContext context,
            Point lineEndPoint,
            Point arrowEndPoint,
            double headWidth,
            double headHeight,
            Color color)
        {
            // Kierunek strzałki
            Vector direction = arrowEndPoint - lineEndPoint;
            direction.Normalize();

            // Punkty na główce strzałki
            Point point1 = arrowEndPoint - direction * headHeight + new Vector(-direction.Y, direction.X) * headWidth / 2;
            Point point2 = arrowEndPoint - direction * headHeight - new Vector(-direction.Y, direction.X) * headWidth / 2;

            // Rysowanie główki strzałki
            PathFigure pathFigure = new PathFigure
            {
                StartPoint = point1,
                IsClosed = true // Zamknięcie ścieżki
            };
            pathFigure.Segments.Add(new LineSegment(arrowEndPoint, true));
            pathFigure.Segments.Add(new LineSegment(point2, true));

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            context.DrawGeometry(new SolidColorBrush(color), new Pen(new SolidColorBrush(color), 1), pathGeometry);
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
