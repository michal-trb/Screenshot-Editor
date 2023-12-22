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

            // Create the FormattedText object
            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                Typeface,
                FontSize,
                new SolidColorBrush(Color)); // Use the Color property of DrawableElement

            // Draw the text
            context.DrawText(formattedText, Position);

            // If the text is selected, draw a red outline
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
            double hitTestBuffer = 3.0; // Możesz dostosować tę wartość do potrzeb
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
                Brushes.Black); // Use a placeholder brush for measurements

            Size textSize = new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);

            return new Rect(Position, textSize);
        }
    }
}
