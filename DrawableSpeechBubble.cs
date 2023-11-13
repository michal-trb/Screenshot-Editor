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

        public DrawableSpeechBubble()
        {
            // Inicjalizacja domyślnych wartości
            Position = new Point(0, 0);
            Size = new Size(100, 50); // Przykładowy domyślny rozmiar
            Text = "Tekst dymku";
        }

        public override void Draw(DrawingContext context)
        {
            // Rysowanie prostokąta dymku
            Rect rect = new Rect(Position, Size);
            context.DrawRectangle(null, new Pen(Brushes.Black, 1), rect);

            // Rysowanie tekstu w dymku
            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                12,
                Brushes.Black);

            context.DrawText(formattedText, Position);

            if (IsSelected)
            {
                // Create a red pen for the selection outline
                Pen selectionPen = new Pen(Brushes.Red, 2);
                // Get the bounds for the selection outline
                Rect bounds = GetBounds();
                // Draw the red outline around the text
                context.DrawRectangle(null, selectionPen, bounds);
            }
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
    }
}