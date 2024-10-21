namespace screenerWpf.Models.DrawableElements;

using System.Windows;
using System.Windows.Media;

/// <summary>
/// Represents a drawable background element with a gradient fill and border.
/// This element cannot be selected, moved, or interacted with.
/// </summary>
public class DrawableBackground : DrawableElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableBackground"/> class.
    /// Sets the element to be non-selectable.
    /// </summary>
    public DrawableBackground()
    {
        this.CanBeSelected = false;
    }

    /// <summary>
    /// Draws the background using a gradient fill and a border.
    /// </summary>
    /// <param name="context">The drawing context to use for rendering.</param>
    public override void Draw(DrawingContext context)
    {
        // Gradient
        LinearGradientBrush gradientBrush = new LinearGradientBrush(
            Colors.LightSkyBlue,
            Colors.WhiteSmoke,
            new Point(0, 0),
            new Point(1, 1));

        // Draw background
        context.DrawRectangle(gradientBrush, null, new Rect(Position, Size));

        // Draw border
        Pen borderPen = new Pen(Brushes.DarkSlateGray, 3);
        context.DrawRectangle(null, borderPen, new Rect(Position, Size));
    }

    /// <summary>
    /// Overrides the Select method to prevent this element from being selected.
    /// </summary>
    public override void Select()
    {
        // Empty implementation to prevent selection.
    }

    /// <summary>
    /// Determines whether the background contains the given point.
    /// Always returns false to prevent click detection.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>Always returns false.</returns>
    public override bool Contains(Point point)
    {
        return false;
    }

    /// <summary>
    /// Overrides the move operation to prevent moving this background element.
    /// </summary>
    /// <param name="delta">The vector representing the movement.</param>
    public override void Move(Vector delta)
    {
        // Empty implementation to prevent movement.
    }

    /// <summary>
    /// Gets the bounds of the background element.
    /// Always returns <see cref="Rect.Empty"/> to indicate no bounding box for interactions.
    /// </summary>
    /// <returns>A <see cref="Rect"/> representing an empty rectangle.</returns>
    public override Rect GetBounds()
    {
        return Rect.Empty;
    }

    /// <summary>
    /// Creates a clone of the current background element.
    /// Cloning is not needed for the background; returns null.
    /// </summary>
    /// <returns>Always returns null.</returns>
    public override DrawableElement Clone()
    {
        return null;
    }
}
