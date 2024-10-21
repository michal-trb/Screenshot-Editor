namespace screenerWpf.Models.DrawableElements;

using System.Windows.Media;
using System.Windows;
using System;
using screenerWpf.Interfaces;


/// <summary>
/// Represents a drawable element on the canvas, providing properties and methods for handling drawing, movement, and interaction.
/// </summary>
public abstract class DrawableElement : IDrawable
{
    /// <summary>
    /// Gets or sets the position of the drawable element.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Gets or sets the size of the drawable element.
    /// </summary>
    public Size Size { get; set; }

    /// <summary>
    /// Gets or sets the color of the drawable element.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Gets or sets whether the element is currently selected.
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Gets or sets the scale of the element.
    /// </summary>
    public double Scale { get; internal set; }

    /// <summary>
    /// Gets or sets whether the element can be selected.
    /// </summary>
    public bool CanBeSelected { get; set; } = true;

    /// <summary>
    /// Draws the element using the specified drawing context.
    /// </summary>
    /// <param name="context">The drawing context used to draw the element.</param>
    public abstract void Draw(DrawingContext context);

    /// <summary>
    /// Gets the bounding rectangle of the element.
    /// </summary>
    /// <returns>A <see cref="Rect"/> representing the bounds of the element.</returns>
    public abstract Rect GetBounds();

    /// <summary>
    /// Selects the element, setting its selected state.
    /// </summary>
    public virtual void Select()
    {
        IsSelected = true;
    }

    /// <summary>
    /// Determines if the element contains the given point.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the element contains the point; otherwise, false.</returns>
    public virtual bool Contains(Point point)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Moves the element by the given vector.
    /// </summary>
    /// <param name="delta">The vector specifying the movement.</param>
    public virtual void Move(Vector delta)
    {
        Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
    }

    /// <summary>
    /// Determines if a given point hits the element.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the point hits the element; otherwise, false.</returns>
    public virtual bool HitTest(Point point)
    {
        Rect rect = new Rect(Position, Size);
        Geometry rectangleGeometry = new RectangleGeometry(rect);
        return rectangleGeometry.FillContains(point);
    }

    /// <summary>
    /// Creates a copy of the current drawable element.
    /// </summary>
    /// <returns>A new instance of <see cref="DrawableElement"/> that is a copy of this element.</returns>
    public abstract DrawableElement Clone();
}
