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
            double cornerRadius = 10.0; // Zaokrąglenie rogów dymku
            Rect rect = new Rect(Position, Size);
            DrawSpeechBubbleTail(context, rect, EndPoint);

            context.DrawRoundedRectangle(Brushes.White, new Pen(Brushes.Black, 1), rect, cornerRadius, cornerRadius);

            // Rysowanie tekstu
            Point textPosition = new Point(Position.X + cornerRadius, Position.Y + cornerRadius);
            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                12,
                Brushes.Black);
            context.DrawText(formattedText, textPosition);

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

        public override void MoveBy(Vector delta)
        {
            Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
        }

        public void SetTailBeingDragged(bool value)
        {
            isTailBeingDragged = value;
        }
    }
}