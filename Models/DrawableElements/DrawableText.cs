namespace screenerWpf.Models.DrawableElements;

using System;
using System.Windows;
using System.Windows.Media;

/// <summary>
/// Represents a drawable text element that can be displayed on a canvas.
/// </summary>
public class DrawableText : DrawableElement
{
    /// <summary>
    /// Gets or sets the text to be displayed.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the typeface used for rendering the text.
    /// </summary>
    public Typeface Typeface { get; set; }

    /// <summary>
    /// Gets or sets the font size of the text.
    /// </summary>
    public double FontSize { get; set; }

    /// <summary>
    /// Draws the text element on the specified drawing context.
    /// </summary>
    /// <param name="context">The drawing context used for rendering the text.</param>
    public override void Draw(DrawingContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

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

    /// <summary>
    /// Checks if a given point intersects with the text geometry.
    /// </summary>
    /// <param name="point">The point to test for intersection.</param>
    /// <returns>True if the point is within the text bounds; otherwise, false.</returns>
    public override bool HitTest(Point point)
    {
        FormattedText formattedText = new FormattedText(
            Text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            Typeface,
            FontSize,
            new SolidColorBrush(Color));

        Geometry textGeometry = formattedText.BuildGeometry(Position);

        double hitTestBuffer = 3.0;
        Geometry inflatedGeometry = textGeometry.GetWidenedPathGeometry(new Pen(Brushes.Black, hitTestBuffer));

        return inflatedGeometry.FillContains(point);
    }

    /// <summary>
    /// Gets the bounding rectangle of the text.
    /// </summary>
    /// <returns>A <see cref="Rect"/> representing the bounds of the text element.</returns>
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

    /// <summary>
    /// Creates a copy of the current text element with an offset position.
    /// </summary>
    /// <returns>A new instance of <see cref="DrawableText"/> with the same properties as the original.</returns>
    public override DrawableElement Clone()
    {
        return new DrawableText
        {
            Text = this.Text,
            Color = this.Color,
            Position = new Point(Position.X + 5, Position.Y + 5),
            Size = this.Size,
            Scale = this.Scale,
            FontSize = this.FontSize,
            Typeface = this.Typeface,
        };
    }
}
