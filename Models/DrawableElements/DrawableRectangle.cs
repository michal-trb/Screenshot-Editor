namespace screenerWpf.Models.DrawableElements;

using System;
using System.Windows;
using System.Windows.Media;

/// <summary>
/// Represents a drawable rectangle with properties for color, thickness, and transparency.
/// </summary>
public class DrawableRectangle : DrawableWithHandles
{
    /// <summary>
    /// Gets or sets the position of the top-left corner of the rectangle.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Gets or sets the size of the rectangle.
    /// </summary>
    public Size Size { get; set; }

    /// <summary>
    /// Gets or sets the color of the rectangle border.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Gets or sets the thickness of the rectangle border.
    /// </summary>
    public double Thickness { get; set; }

    /// <summary>
    /// Gets or sets the transparency level of the rectangle, from 0 to 100.
    /// </summary>
    public double Transparency { get; internal set; }

    private enum DragHandle { None, TopLeft, TopRight, BottomLeft, BottomRight }
    private DragHandle currentDragHandle = DragHandle.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableRectangle"/> class.
    /// </summary>
    public DrawableRectangle() : base(4)
    {
        Position = new Point(0, 0);
        Size = new Size(100, 50);
        Color = Colors.Black;
        Thickness = 1.0;
        Transparency = 100;
    }

    /// <summary>
    /// Draws the rectangle on the canvas, including handles if it is selected.
    /// </summary>
    /// <param name="context">The drawing context used for rendering the rectangle.</param>
    public override void Draw(DrawingContext context)
    {
        if (context == null)
            throw new ArgumentNullException("context");

        Rect rect = new Rect(Position, Size);
        Brush fillBrush = new SolidColorBrush();
        Pen strokePen = new Pen(new SolidColorBrush(Color.FromArgb((byte)(255 * Transparency / 100), Color.R, Color.G, Color.B)), Thickness);
        context.DrawRectangle(fillBrush, strokePen, rect);

        if (IsSelected)
        {
            UpdateHandlePoints();
            DrawSelectionHandles(context);
        }
    }

    /// <summary>
    /// Performs a hit test to determine if a point is within the rectangle bounds or near a drag handle.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the point is within the rectangle or near a drag handle, otherwise false.</returns>
    public override bool HitTest(Point point)
    {
        Rect rect = new Rect(Position, Size);
        currentDragHandle = DragHandle.None;

        if (IsNearCorner(point, Position)) currentDragHandle = DragHandle.TopLeft;
        else if (IsNearCorner(point, new Point(Position.X + Size.Width, Position.Y))) currentDragHandle = DragHandle.TopRight;
        else if (IsNearCorner(point, new Point(Position.X, Position.Y + Size.Height))) currentDragHandle = DragHandle.BottomLeft;
        else if (IsNearCorner(point, new Point(Position.X + Size.Width, Position.Y + Size.Height))) currentDragHandle = DragHandle.BottomRight;

        return rect.Contains(point) || currentDragHandle != DragHandle.None;
    }

    /// <summary>
    /// Updates the positions of the drag handles used for resizing the rectangle.
    /// </summary>
    protected override void UpdateHandlePoints()
    {
        Rect rect = new Rect(Position, Size);
        HandlePoints[0] = rect.TopLeft;     // Top-left corner
        HandlePoints[1] = rect.TopRight;    // Top-right corner
        HandlePoints[2] = rect.BottomLeft;  // Bottom-left corner
        HandlePoints[3] = rect.BottomRight; // Bottom-right corner
    }

    /// <summary>
    /// Checks if a given point is near a specified corner of the rectangle.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <param name="corner">The corner to compare against.</param>
    /// <returns>True if the point is near the corner, otherwise false.</returns>
    private bool IsNearCorner(Point point, Point corner)
    {
        double tolerance = 10; // Tolerance distance for detecting near corner
        return Math.Abs(point.X - corner.X) <= tolerance && Math.Abs(point.Y - corner.Y) <= tolerance;
    }

    /// <summary>
    /// Gets the bounding rectangle of the rectangle element.
    /// </summary>
    /// <returns>A <see cref="Rect"/> representing the bounds of the rectangle.</returns>
    public override Rect GetBounds()
    {
        return new Rect(Position, Size);
    }

    /// <summary>
    /// Moves the rectangle or resizes it if a drag handle is being used.
    /// </summary>
    /// <param name="delta">The vector by which to move or resize the rectangle.</param>
    public override void Move(Vector delta)
    {
        double newX = Position.X;
        double newY = Position.Y;
        double newWidth = Size.Width;
        double newHeight = Size.Height;

        switch (currentDragHandle)
        {
            case DragHandle.None:
                newX += delta.X;
                newY += delta.Y;
                break;
            case DragHandle.TopLeft:
                newX += delta.X;
                newY += delta.Y;
                newWidth -= delta.X;
                newHeight -= delta.Y;
                break;
            case DragHandle.TopRight:
                newWidth += delta.X;
                newY += delta.Y;
                newHeight -= delta.Y;
                break;
            case DragHandle.BottomLeft:
                newX += delta.X;
                newWidth -= delta.X;
                newHeight += delta.Y;
                break;
            case DragHandle.BottomRight:
                newWidth += delta.X;
                newHeight += delta.Y;
                break;
        }

        // Adjust if the width or height becomes negative
        if (newWidth < 0)
        {
            newX += newWidth;
            newWidth = -newWidth;
            // Switch drag handle to the opposite side on X-axis
            currentDragHandle = currentDragHandle == DragHandle.TopLeft ? DragHandle.TopRight :
                                currentDragHandle == DragHandle.TopRight ? DragHandle.TopLeft :
                                currentDragHandle == DragHandle.BottomLeft ? DragHandle.BottomRight :
                                DragHandle.BottomLeft;
        }
        if (newHeight < 0)
        {
            newY += newHeight;
            newHeight = -newHeight;
            // Switch drag handle to the opposite side on Y-axis
            currentDragHandle = currentDragHandle == DragHandle.TopLeft ? DragHandle.BottomLeft :
                                currentDragHandle == DragHandle.BottomLeft ? DragHandle.TopLeft :
                                currentDragHandle == DragHandle.TopRight ? DragHandle.BottomRight :
                                DragHandle.TopRight;
        }

        // Set new values for position and size
        Position = new Point(newX, newY);
        Size = new Size(newWidth, newHeight);
    }

    /// <summary>
    /// Creates a clone of this rectangle element.
    /// </summary>
    /// <returns>A new <see cref="DrawableRectangle"/> instance with the same properties.</returns>
    public override DrawableElement Clone()
    {
        return new DrawableRectangle
        {
            Thickness = this.Thickness,
            Transparency = this.Transparency,
            Color = this.Color,
            Position = new Point(Position.X + 5, Position.Y + 5),
            Size = this.Size,
            Scale = this.Scale,
        };
    }
}
