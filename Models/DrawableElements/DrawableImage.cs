namespace screenerWpf.Models.DrawableElements;

using System;
using System.Windows;
using System.Windows.Media;

/// <summary>
/// Represents an image that can be drawn on the canvas with drag handles for resizing.
/// </summary>
public class DrawableImage : DrawableWithHandles
{
    /// <summary>
    /// Source of the image to be drawn.
    /// </summary>
    private ImageSource imageSource;

    /// <summary>
    /// Current drag handle being used to resize the image.
    /// </summary>
    private DragHandle currentDragHandle = DragHandle.None;

    /// <summary>
    /// Gets or sets the size of the image.
    /// </summary>
    public Size Size { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableImage"/> class with the specified image source and position.
    /// </summary>
    /// <param name="source">The image source to be drawn.</param>
    /// <param name="position">The position of the image on the canvas.</param>
    public DrawableImage(ImageSource source, Point position) : base(4)
    {
        this.imageSource = source;
        this.Position = position;
        // Set the size based on the source image dimensions.
        this.Size = new Size(source.Width, source.Height);
    }

    /// <summary>
    /// Draws the image on the canvas.
    /// </summary>
    /// <param name="context">The drawing context used for rendering the image.</param>
    public override void Draw(DrawingContext context)
    {
        if (this.imageSource != null)
        {
            context.DrawImage(this.imageSource, new Rect(this.Position, this.Size));
        }
        if (IsSelected)
        {
            UpdateHandlePoints();
            DrawSelectionHandles(context);
        }
    }

    /// <summary>
    /// Gets the bounding rectangle of the image.
    /// </summary>
    /// <returns>A <see cref="Rect"/> representing the bounds of the image.</returns>
    public override Rect GetBounds()
    {
        return new Rect(this.Position, this.Size);
    }

    /// <summary>
    /// Performs a hit test to determine if a point is within the image bounds or near a drag handle.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the point is within the image or near a drag handle, otherwise false.</returns>
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
    /// Checks if a given point is near a specified corner.
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
    /// Moves the image or resizes it if a drag handle is being used.
    /// </summary>
    /// <param name="delta">The vector by which to move or resize the image.</param>
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
    /// Updates the positions of the drag handles used for resizing the image.
    /// </summary>
    protected override void UpdateHandlePoints()
    {
        Rect rect = new Rect(Position, Size);
        HandlePoints[0] = rect.TopLeft;      // Top-left corner
        HandlePoints[1] = rect.TopRight;     // Top-right corner
        HandlePoints[2] = rect.BottomLeft;   // Bottom-left corner
        HandlePoints[3] = rect.BottomRight;  // Bottom-right corner
    }

    /// <summary>
    /// Creates a clone of this image element.
    /// </summary>
    /// <returns>A new <see cref="DrawableImage"/> instance with the same properties.</returns>
    public override DrawableElement Clone()
    {
        return new DrawableImage(this.imageSource, new Point(Position.X + 5, Position.Y + 5));
    }
}
