namespace screenerWpf.Helpers;

using screenerWpf.Models.DrawableElements;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

/// <summary>
/// Manages a collection of drawable elements on a canvas, allowing addition, removal, and manipulation of their order.
/// </summary>
public class ElementManager
{
    /// <summary>
    /// Gets the list of drawable elements currently managed by the <see cref="ElementManager"/>.
    /// </summary>
    public List<DrawableElement> Elements { get; private set; } = new List<DrawableElement>();

    /// <summary>
    /// Adds a drawable element to the collection.
    /// </summary>
    /// <param name="element">The drawable element to add.</param>
    public void AddElement(DrawableElement element)
    {
        if (element == null)
        {
            return;
        }

        Elements.Add(element);
    }

    /// <summary>
    /// Removes a drawable element from the collection.
    /// </summary>
    /// <param name="element">The drawable element to remove.</param>
    public void RemoveElement(DrawableElement element)
    {
        if (element == null)
        {
            return;
        }

        Elements.Remove(element);
    }

    /// <summary>
    /// Gets the first drawable element located at the specified point.
    /// </summary>
    /// <param name="point">The point to hit test for an element.</param>
    /// <returns>
    /// The <see cref="DrawableElement"/> at the specified point, or <c>null</c> if no such element exists.
    /// </returns>
    public DrawableElement GetElementAtPoint(Point point)
    {
        return Elements.FirstOrDefault(element => element.CanBeSelected && element.HitTest(point));
    }

    /// <summary>
    /// Moves the specified drawable element to the front of the collection, ensuring it is drawn last.
    /// </summary>
    /// <param name="element">The drawable element to bring to the front.</param>
    public void BringToFront(DrawableElement element)
    {
        if (element == null)
        {
            return;
        }

        Elements.Remove(element);
        Elements.Add(element);
    }
}