namespace screenerWpf.CanvasHandler.Drawers;

using screenerWpf.Controls;
using System.Windows.Input;

/// <summary>
/// Provides an abstract base class for drawing drawable elements on a canvas.
/// This class serves as a foundation for creating different drawable objects, such as arrows, rectangles, or other shapes.
/// </summary>
public abstract class DrawableElementDrawer
{
    /// <summary>
    /// Gets the canvas on which the drawable elements are being drawn.
    /// </summary>
    protected DrawableCanvas DrawableCanvas { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableElementDrawer"/> class with the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas where drawable elements will be drawn.</param>
    protected DrawableElementDrawer(DrawableCanvas canvas)
    {
        DrawableCanvas = canvas;
    }

    /// <summary>
    /// Begins the drawing process of a drawable element.
    /// </summary>
    /// <param name="e">The mouse button event data used to initiate the drawing.</param>
    public abstract void StartDrawing(MouseButtonEventArgs e);

    /// <summary>
    /// Updates the current drawing as the mouse moves, allowing dynamic changes to the drawable element.
    /// </summary>
    /// <param name="e">The mouse event data used to update the drawing.</param>
    public abstract void UpdateDrawing(MouseEventArgs e);

    /// <summary>
    /// Completes the drawing process and finalizes the drawable element on the canvas.
    /// </summary>
    public abstract void FinishDrawing();
}