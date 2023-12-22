using System;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{
    public class DrawableSpeechBubble : DrawableWithHandles
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public string Text { get; set; }
        public Point EndTailPoint { get; set; } // Pozycja końca ogonka
        public double FontSize { get; set; }
        public Typeface Typeface { get; set; }
        public Brush Brush { get; set; }

        public bool isTailBeingDragged = false;

        public DrawableSpeechBubble() : base(1)
        {

        }

        public override void Draw(DrawingContext context)
        {
            // Ustawienie minimalnych rozmiarów dymku
            double minWidth = 100.0;
            double minHeight = 50.0;

            // Obliczanie wymiarów tekstu
            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                Typeface,
                FontSize, // Użyj aktualnego rozmiaru czcionki
                Brush);
            Size textSize = new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);

            // Dostosowanie rozmiaru dymku do tekstu
            double bubbleWidth = Math.Max(minWidth, textSize.Width + 20); // Dodatkowe miejsce na marginesy
            double bubbleHeight = Math.Max(minHeight, textSize.Height + 20);

            // Ustawianie nowego rozmiaru dymku
            Size = new Size(bubbleWidth, bubbleHeight);
            Rect rect = new Rect(Position, Size);

            // Rysowanie ogonka dymku
            DrawSpeechBubbleTail(context, rect, EndTailPoint);

            // Rysowanie prostokąta dymku
            double cornerRadius = 10.0;
            context.DrawRoundedRectangle(Brushes.White, new Pen(Brushes.Black, 1), rect, cornerRadius, cornerRadius);

            // Rysowanie tekstu
            Point textPosition = new Point(Position.X + 10, Position.Y + 10); // Dodatkowe marginesy
            context.DrawText(formattedText, textPosition);

            // Rysowanie ramki selekcyjnej, jeśli jest zaznaczony
            if (IsSelected)
            {
                UpdateHandlePoints(); // Aktualizuj pozycje uchwytów
                DrawSelectionHandles(context); // Narysuj uchwyty
            }
        }

        private void DrawSpeechBubbleTail(DrawingContext context, Rect bubbleRect, Point endPoint)
        {
            // Punkt startowy ogonka na środku dolnej krawędzi dymku
            Point tailStart = new Point(bubbleRect.Left + bubbleRect.Width / 2, bubbleRect.Top);

            StreamGeometry tailGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = tailGeometry.Open())
            {
                geometryContext.BeginFigure(tailStart, true /* is filled */, true /* is closed */);
                geometryContext.LineTo(new Point(tailStart.X - 10, tailStart.Y + 10), true /* is stroked */, false /* is smooth join */);
                geometryContext.LineTo(endPoint, true /* is stroked */, false /* is smooth join */);
                geometryContext.LineTo(new Point(tailStart.X + 10, tailStart.Y + 25), true /* is stroked */, false /* is smooth join */);
                geometryContext.LineTo(tailStart, true /* is stroked */, false /* is smooth join */);
            }

            context.DrawGeometry(Brushes.White, new Pen(Brushes.Black, 1), tailGeometry);
        }

        protected override void UpdateHandlePoints()
        {
            HandlePoints[0] = EndTailPoint; // Pozycja uchwytu na końcówce ogonka
        }

        public override bool HitTest(Point point)
        {
            // Utwórz geometrię obejmującą zarówno ciało dymku, jak i ogonek
            GeometryGroup geometryGroup = new GeometryGroup();
            Rect bubbleRect = new Rect(Position, Size);
            geometryGroup.Children.Add(new RectangleGeometry(bubbleRect));
            geometryGroup.Children.Add(new LineGeometry(bubbleRect.BottomLeft, EndTailPoint)); // Przykładowa geometria dla ogonka

            // Sprawdzenie, czy punkt znajduje się w obrębie geometrii
            return geometryGroup.FillContains(point) || IsNearHandle(point, EndTailPoint);
        }

        public override Rect GetBounds()
        {
            // Tworzenie FormattedText dla wymiarów tekstu
            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                12,
                Brushes.Black);

            // Uzyskiwanie rozmiaru tekstu
            Size textSize = new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);

            // Obliczanie końcowych wymiarów prostokąta dymku
            double width = Math.Max(Size.Width, textSize.Width);
            double height = Size.Height + textSize.Height;

            // Uwzględnienie pozycji dymku
            return new Rect(Position, new Size(width, height));
        }

        public override void Move(Vector delta)
        {
            if (isTailBeingDragged)
            {
                // Przesuwanie tylko ogonka
                EndTailPoint = new Point(EndTailPoint.X + delta.X, EndTailPoint.Y + delta.Y);
            }
            else
            {
                // Przesuwanie całego dymku
                Position = new Point(Position.X + delta.X, Position.Y + delta.Y);

                // Aktualizacja pozycji ogonka względem przesunięcia całego dymku
                // Jeśli ogonek ma zachować swoją pozycję względem ekranu, a nie dymku, 
                // ta linia powinna być zakomentowana lub usunięta.
                EndTailPoint = new Point(EndTailPoint.X + delta.X, EndTailPoint.Y + delta.Y);
            }
        }


        public void SetTailBeingDragged(bool value)
        {
            isTailBeingDragged = value;
        }

        private bool IsNearHandle(Point point, Point handle)
        {
            double handleRadius = 10; // Promień, w którym punkt jest uznawany za bliski uchwytowi
            return (point - handle).Length <= handleRadius;
        }
    }
}