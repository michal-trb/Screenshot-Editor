using System;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf
{
    public class DrawableSpeechBubble : DrawableElement
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public string Text { get; set; }
        public Point EndPoint { get; set; } // Pozycja końca ogonka

        public bool isTailBeingDragged = false;

        public DrawableSpeechBubble()
        {
            // Inicjalizacja domyślnych wartości
            Position = new Point(0, 0);
            Size = new Size(100, 50); // Przykładowy domyślny rozmiar
            Text = "Tekst dymku";
            EndPoint = new Point(Position.X - 15, Position.Y - 15); // Przykładowa pozycja końca ogonka
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
                new Typeface("Arial"),
                12,
                Brushes.Black);
            Size textSize = new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);

            // Dostosowanie rozmiaru dymku do tekstu
            double bubbleWidth = Math.Max(minWidth, textSize.Width + 20); // Dodatkowe miejsce na marginesy
            double bubbleHeight = Math.Max(minHeight, textSize.Height + 20);

            // Ustawianie nowego rozmiaru dymku
            Size = new Size(bubbleWidth, bubbleHeight);
            Rect rect = new Rect(Position, Size);

            // Rysowanie ogonka dymku
            DrawSpeechBubbleTail(context, rect, EndPoint);

            // Rysowanie prostokąta dymku
            double cornerRadius = 10.0;
            context.DrawRoundedRectangle(Brushes.White, new Pen(Brushes.Black, 1), rect, cornerRadius, cornerRadius);

            // Rysowanie tekstu
            Point textPosition = new Point(Position.X + 10, Position.Y + 10); // Dodatkowe marginesy
            context.DrawText(formattedText, textPosition);

            // Rysowanie ramki selekcyjnej, jeśli jest zaznaczony
            if (IsSelected)
            {
                Pen selectionPen = new Pen(Brushes.Red, 2);
                context.DrawRoundedRectangle(null, selectionPen, rect, cornerRadius, cornerRadius);
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

        public override bool HitTest(Point point)
        {
            // Tworzenie FormattedText dla wymiarów tekstu
            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                12,
                Brushes.Black); // Używamy czcionki i koloru dla dymku

            // Tworzenie geometrii na podstawie tekstu i prostokąta dymku
            Geometry textGeometry = formattedText.BuildGeometry(Position);
            Rect rect = new Rect(Position, Size);
            Geometry rectangleGeometry = new RectangleGeometry(rect);

            // Łączenie geometrii tekstu i prostokąta
            CombinedGeometry combinedGeometry = new CombinedGeometry(GeometryCombineMode.Union, rectangleGeometry, textGeometry);

            // Rozszerzenie geometrii o bufor dla łatwiejszego kliknięcia
            double hitTestBuffer = 20.0;
            Geometry inflatedGeometry = combinedGeometry.GetWidenedPathGeometry(new Pen(Brushes.Black, hitTestBuffer));

            // Sprawdzenie, czy punkt znajduje się w rozszerzonej geometrii
            return inflatedGeometry.FillContains(point);
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
            Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
        }

        public void SetTailBeingDragged(bool value)
        {
            isTailBeingDragged = value;
        }
    }
}