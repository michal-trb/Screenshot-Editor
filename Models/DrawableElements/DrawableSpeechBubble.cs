namespace screenerWpf.Models.DrawableElements;

using System;
using System.Windows;
using System.Windows.Media;

/// <summary>
/// Represents a speech bubble drawable element with a resizable bubble and tail.
/// </summary>
public class DrawableSpeechBubble : DrawableWithHandles
{
    /// <summary>
    /// Gets or sets the position of the speech bubble.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Gets or sets the size of the speech bubble.
    /// </summary>
    public Size Size { get; set; }

    /// <summary>
    /// Gets or sets the text displayed within the speech bubble.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the position of the tail end point.
    /// </summary>
    public Point EndTailPoint { get; set; }

    /// <summary>
    /// Gets or sets the font size used in the speech bubble.
    /// </summary>
    public double FontSize { get; set; }

    /// <summary>
    /// Gets or sets the typeface used for the text in the speech bubble.
    /// </summary>
    public Typeface Typeface { get; set; }

    /// <summary>
    /// Gets or sets the brush used to paint the text and bubble.
    /// </summary>
    public Brush Brush { get; set; }

    private bool isTailBeingDragged = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableSpeechBubble"/> class.
    /// </summary>
    public DrawableSpeechBubble() : base(1)
    {
    }

    /// <summary>
    /// Draws the speech bubble, including its body and tail, as well as the contained text.
    /// </summary>
    /// <param name="context">The drawing context used for rendering the speech bubble.</param>
    public override void Draw(DrawingContext context)
    {
        // Setting minimum size constraints
        double minWidth = 100.0;
        double minHeight = 50.0;

        // Formatting the text
        FormattedText formattedText = new FormattedText(
            Text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            Typeface,
            FontSize,
            Brush);

        Size textSize = new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);

        // Adjusting bubble size based on text
        double bubbleWidth = Math.Max(minWidth, textSize.Width + 20);
        double bubbleHeight = Math.Max(minHeight, textSize.Height + 20);

        Size = new Size(bubbleWidth, bubbleHeight);
        Rect rect = new Rect(Position, Size);

        // Draw bubble tail
        DrawSpeechBubbleTail(context, rect, EndTailPoint);

        // Draw bubble body
        double cornerRadius = 10.0;
        context.DrawRoundedRectangle(Brushes.White, new Pen(Brushes.Black, 1), rect, cornerRadius, cornerRadius);

        // Draw text
        Point textPosition = new Point(Position.X + 10, Position.Y + 10);
        context.DrawText(formattedText, textPosition);

        // Draw selection handles if selected
        if (IsSelected)
        {
            UpdateHandlePoints();
            DrawSelectionHandles(context);
        }
    }

    /// <summary>
    /// Draws the tail of the speech bubble.
    /// </summary>
    /// <param name="context">The drawing context used for rendering the tail.</param>
    /// <param name="bubbleRect">The bubble rectangle dimensions.</param>
    /// <param name="endPoint">The end point of the tail.</param>
    private void DrawSpeechBubbleTail(DrawingContext context, Rect bubbleRect, Point endPoint)
    {
        Point tailStart = new Point(bubbleRect.Left + bubbleRect.Width / 2, bubbleRect.Bottom - bubbleRect.Height / 2);

        Vector direction = endPoint - tailStart;
        direction.Normalize();

        Vector orthogonal = new Vector(-direction.Y, direction.X) * (20 / 2);

        Point leftPoint = tailStart + orthogonal;
        Point rightPoint = tailStart - orthogonal;

        StreamGeometry tailGeometry = new StreamGeometry();
        using (StreamGeometryContext geometryContext = tailGeometry.Open())
        {
            geometryContext.BeginFigure(leftPoint, true, true);
            geometryContext.LineTo(endPoint, true, false);
            geometryContext.LineTo(rightPoint, true, false);
            geometryContext.LineTo(leftPoint, true, false);
        }

        context.DrawGeometry(Brushes.White, new Pen(Brushes.Black, 1), tailGeometry);
    }

    /// <summary>
    /// Updates the handle points used for resizing or dragging the speech bubble.
    /// </summary>
    protected override void UpdateHandlePoints()
    {
        HandlePoints[0] = EndTailPoint;
    }

    /// <summary>
    /// Performs a hit test to determine whether the point is within the speech bubble bounds or its tail.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the point is within the bounds; otherwise, false.</returns>
    public override bool HitTest(Point point)
    {
        GeometryGroup geometryGroup = new GeometryGroup();
        Rect bubbleRect = new Rect(Position, Size);
        geometryGroup.Children.Add(new RectangleGeometry(bubbleRect));
        geometryGroup.Children.Add(new LineGeometry(bubbleRect.BottomLeft, EndTailPoint));

        return geometryGroup.FillContains(point) || IsNearHandle(point, EndTailPoint);
    }

    /// <summary>
    /// Gets the bounding rectangle of the speech bubble.
    /// </summary>
    /// <returns>A <see cref="Rect"/> representing the bounds of the speech bubble.</returns>
    public override Rect GetBounds()
    {
        FormattedText formattedText = new FormattedText(
            Text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            12,
            Brushes.Black);

        Size textSize = new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
        double width = Math.Max(Size.Width, textSize.Width);
        double height = Size.Height + textSize.Height;

        return new Rect(Position, new Size(width, height));
    }

    /// <summary>
    /// Moves the speech bubble by the given vector.
    /// </summary>
    /// <param name="delta">The vector by which to move the speech bubble.</param>
    public override void Move(Vector delta)
    {
        if (isTailBeingDragged)
        {
            EndTailPoint = new Point(EndTailPoint.X + delta.X, EndTailPoint.Y + delta.Y);
        }
        else
        {
            Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
            EndTailPoint = new Point(EndTailPoint.X + delta.X, EndTailPoint.Y + delta.Y);
        }
    }

    /// <summary>
    /// Sets whether the tail is currently being dragged by the user.
    /// </summary>
    /// <param name="value">A boolean indicating whether the tail is being dragged.</param>
    public void SetTailBeingDragged(bool value)
    {
        isTailBeingDragged = value;
    }

    /// <summary>
    /// Creates a clone of the current speech bubble.
    /// </summary>
    /// <returns>A new instance of <see cref="DrawableSpeechBubble"/> with the same properties.</returns>
    public override DrawableElement Clone()
    {
        return new DrawableSpeechBubble
        {
            Text = this.Text,
            EndTailPoint = this.EndTailPoint,
            Color = this.Color,
            Position = new Point(Position.X + 5, Position.Y + 5),
            Size = this.Size,
            Scale = this.Scale,
            FontSize = this.FontSize,
            Typeface = this.Typeface,
            Brush = this.Brush,
        };
    }

    /// <summary>
    /// Determines whether a given point is near a specific handle point.
    /// </summary>
    /// <param name="point">The point to be tested.</param>
    /// <param name="handle">The position of the handle point.</param>
    /// <returns>True if the point is near the handle point; otherwise, false.</returns>
    private bool IsNearHandle(Point point, Point handle)
    {
        double handleRadius = 10;
        return (point - handle).Length <= handleRadius;
    }
}
