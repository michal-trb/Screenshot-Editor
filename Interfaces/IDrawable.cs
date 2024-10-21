namespace screenerWpf.Interfaces;

using screenerWpf.Models.DrawableElements;
using System.Windows;
using System.Windows.Media.Imaging;

/// <summary>
/// Interface representing a drawable element with common operations for rendering, selection, and manipulation.
/// </summary>
public interface IDrawable
{
    /// <summary>
    /// Selects the element, typically changing its visual appearance to indicate selection.
    /// </summary>
    void Select();

    /// <summary>
    /// Moves the element by a specified delta value.
    /// </summary>
    /// <param name="delta">The vector by which to move the element.</param>
    void Move(Vector delta);

    /// <summary>
    /// Gets the bounding box of the element.
    /// </summary>
    /// <returns>A <see cref="Rect"/> representing the bounds of the element.</returns>
    Rect GetBounds();

    /// <summary>
    /// Creates a clone of the element.
    /// </summary>
    /// <returns>A new instance of <see cref="DrawableElement"/> that is a copy of the current element.</returns>
    DrawableElement Clone();
}
