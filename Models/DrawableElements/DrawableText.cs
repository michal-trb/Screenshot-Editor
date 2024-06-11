using System;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{

    public class DrawableText : DrawableElement
    {
        public string Text { get; set; }
        public Typeface Typeface { get; set; }
        public double FontSize { get; set; }

        public override void Draw(DrawingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                Typeface,
                FontSize,
                new SolidColorBrush(Color));

            context.DrawText(formattedText, Position);

            if (IsSelected)
            {
                Pen selectionPen = new Pen(Brushes.Red, 2);
                Rect bounds = GetBounds();
                context.DrawRectangle(null, selectionPen, bounds);
            }
        }

        public override bool HitTest(Point point)
        {
            // Stwórz FormattedText, aby uzyskać wymiary tekstu
            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                Typeface,
                FontSize,
                new SolidColorBrush(Color));

            // Zbuduj geometrię na podstawie tekstu
            Geometry textGeometry = formattedText.BuildGeometry(Position);

            // Powiększ geometrię o określoną wartość "buforową" dla łatwiejszego kliknięcia
            double hitTestBuffer = 3.0;
            Geometry inflatedGeometry = textGeometry.GetWidenedPathGeometry(new Pen(Brushes.Black, hitTestBuffer));

            // Zwróć, czy rozszerzona geometria zawiera punkt kliknięcia
            return inflatedGeometry.FillContains(point);
        }

        public override Rect GetBounds()
        {
            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                Typeface,
                FontSize,
                Brushes.Black);

            Size textSize = new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);

            return new Rect(Position, textSize);
        }

        public override DrawableElement Clone()
        {
            return new DrawableText
            {
                Text = this.Text,
                Color = this.Color,
                Position = new Point(Position.X+5, Position.Y+5),
                Size = this.Size,
                Scale = this.Scale,
                FontSize = this.FontSize,
                Typeface = this.Typeface,
            };
        }
    }
}
