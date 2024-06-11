using screenerWpf.Interfaces;
using System;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{
    public class DrawableArrow : DrawableWithHandles
    {
        public Point EndPoint { get; set; }
        public double Thickness { get; internal set; }
        private bool isStartBeingDragged = false;
        private bool isEndBeingDragged = false;

        public DrawableArrow() : base(2)
        {
        }

        public void SetStartBeingDragged(bool value)
        {
            isStartBeingDragged = value;
        }

        public void SetEndBeingDragged(bool value)
        {
            isEndBeingDragged = value;
        }

        public override void Draw(DrawingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            // Obliczenie długości i szerokości główki strzałki
            double headLength = Thickness + 10; 
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
                UpdateHandlePoints(); // Aktualizuj pozycje uchwytów
                DrawSelectionHandles(context); // Narysuj uchwyty
            }
        }
        protected override void UpdateHandlePoints()
        {
            HandlePoints[0] = Position; // Początek strzałki
            HandlePoints[1] = EndPoint; // Koniec strzałki
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
            double buffer = 15.0; // Hit test buffer
            LineGeometry geometry = new LineGeometry(Position, EndPoint);
            return geometry.StrokeContains(new Pen(Brushes.Black, buffer), point);
        }

        public override Rect GetBounds()
        {
            // Pobierz szerokość główki strzałki, załóżmy że to stała wartość
            double arrowHeadWidth = 10.0;

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
            double buffer = 5.0;
            LineGeometry lineGeometry = new LineGeometry(Position, EndPoint);
            return lineGeometry.StrokeContains(new Pen(Brushes.Black, buffer), point);
        }

        public override void Move(Vector delta)
        {
            if (isStartBeingDragged)
            {
                Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
            }
            if (isEndBeingDragged)
            {
                EndPoint = new Point(EndPoint.X + delta.X, EndPoint.Y + delta.Y);
            }
            if (!isStartBeingDragged && !isEndBeingDragged)
            {
                // Przesuwamy całą strzałkę
                base.Move(delta);
                EndPoint = new Point(EndPoint.X + delta.X, EndPoint.Y + delta.Y);
            }
        }

        public override DrawableElement Clone()
        {
            return new DrawableArrow
            {
                EndPoint = this.EndPoint,
                Thickness = this.Thickness,
                Color = this.Color,
                Position = new Point(Position.X + 5, Position.Y + 5),
                Size = this.Size,
                Scale = this.Scale,
            };
        }
    }
}
