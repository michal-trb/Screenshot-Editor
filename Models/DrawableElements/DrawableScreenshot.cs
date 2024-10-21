namespace screenerWpf.Models.DrawableElements;

using System;
using System.Windows;
using System.Windows.Media;

/// <summary>
/// Represents a screenshot element that is drawn on the canvas but cannot be moved or selected.
/// </summary>
public class DrawableScreenshot : DrawableElement
{
    private ImageSource imageSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableScreenshot"/> class.
    /// Sets up a static image that cannot be selected or interacted with.
    /// </summary>
    /// <param name="source">The source image to display as a screenshot.</param>
    /// <param name="position">The position of the top-left corner of the screenshot.</param>
    /// <param name="size">The size of the screenshot to be drawn.</param>
    public DrawableScreenshot(ImageSource source, Point position, Size size)
    {
        this.imageSource = source;
        this.Position = position;
        this.Size = size;
        this.CanBeSelected = false;
    }

    /// <summary>
    /// Creates a clone of the current screenshot element.
    /// This method is not implemented since screenshots are static and non-interactive.
    /// </summary>
    /// <returns>Throws a <see cref="NotImplementedException"/>.</returns>
    public override DrawableElement Clone()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Draws the screenshot onto the canvas.
    /// </summary>
    /// <param name="context">The drawing context used for rendering the screenshot.</param>
    public override void Draw(DrawingContext context)
    {
        if (imageSource != null)
        {
            context.DrawImage(imageSource, new Rect(Position, Size));
        }
        // Selection handles are not drawn since this element cannot be selected or moved.
    }

    /// <summary>
    /// Gets the bounding rectangle of the screenshot element.
    /// </summary>
    /// <returns>A <see cref="Rect"/> representing the bounds of the screenshot.</returns>
    public override Rect GetBounds()
    {
        // Method should return the correct bounds for the element, but we block interaction.
        return new Rect(Position, Size);
    }

    /// <summary>
    /// Performs a hit test to determine if the point is within the screenshot bounds.
    /// Always returns false to prevent this element from being selected.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>Always returns false.</returns>
    public override bool HitTest(Point point)
    {
        return false;
    }

    /// <summary>
    /// Moves the screenshot by the given vector.
    /// This operation is disabled and therefore does nothing.
    /// </summary>
    /// <param name="delta">The vector by which to attempt to move the screenshot.</param>
    public override void Move(Vector delta)
    {
        // Movement is blocked, so this method does nothing.
    }
}
