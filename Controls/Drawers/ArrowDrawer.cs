namespace screenerWpf.CanvasHandler.Drawers;

using screenerWpf.Controls;
using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;
using System.Windows;
using System.Windows.Input;

/// <summary>
/// Handles the drawing of arrow elements on a drawable canvas.
/// This class inherits from <see cref="DrawableElementDrawer"/> and manages the lifecycle of an arrow being drawn, from start to finish.
/// </summary>
public class ArrowDrawer : DrawableElementDrawer
{
    /// <summary>
    /// Gets or sets the current arrow that is being drawn.
    /// </summary>
    private DrawableArrow CurrentArrow { get; set; }

    /// <summary>
    /// Gets the currently drawable element. This property is used to keep track of the arrow being manipulated.
    /// </summary>
    public IDrawable CurrentDrawable { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrowDrawer"/> class with the specified canvas and current drawable element.
    /// </summary>
    /// <param name="currentDrawable">The drawable element that is currently being drawn or manipulated.</param>
    /// <param name="canvas">The canvas on which to draw the arrow.</param>
    public ArrowDrawer(IDrawable currentDrawable, DrawableCanvas canvas) : base(canvas)
    {
        CurrentDrawable = currentDrawable;
    }

    /// <summary>
    /// Starts drawing the arrow on the canvas.
    /// </summary>
    /// <param name="e">The mouse button event data used to determine the starting position of the arrow.</param>
    public override void StartDrawing(MouseButtonEventArgs e)
    {
        Point location = e.GetPosition(DrawableCanvas);
        CurrentArrow = new DrawableArrow
        {
            Position = location,
            Color = CanvasInputHandler.GetCurrentColor(),
            Thickness = CanvasInputHandler.GetCurrentThickness(),
        };
        DrawableCanvas.AddElement(CurrentArrow);
    }

    /// <summary>
    /// Updates the endpoint of the arrow as the mouse moves, providing a visual representation of the arrow length and direction.
    /// </summary>
    /// <param name="e">The mouse event data used to determine the current endpoint of the arrow.</param>
    public override void UpdateDrawing(MouseEventArgs e)
    {
        if (CurrentArrow == null) return;
        CurrentArrow.EndPoint = e.GetPosition(DrawableCanvas);
        DrawableCanvas.InvalidateVisual();
    }

    /// <summary>
    /// Completes the arrow drawing process and finalizes the drawn arrow on the canvas.
    /// </summary>
    public override void FinishDrawing()
    {
        CurrentArrow = null;
    }
}