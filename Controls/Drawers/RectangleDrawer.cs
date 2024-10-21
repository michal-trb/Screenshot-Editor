namespace screenerWpf.CanvasHandler.Drawers;

using screenerWpf.Controls;
using screenerWpf.Models.DrawableElements;
using System;
using System.Windows;
using System.Windows.Input;

/// <summary>
/// Provides functionality to draw rectangular shapes on a <see cref="DrawableCanvas"/>.
/// </summary>
public class RectangleDrawer : DrawableElementDrawer
{
    /// <summary>
    /// The rectangle that is currently being drawn.
    /// </summary>
    private DrawableRectangle CurrentRectangle { get; set; }

    /// <summary>
    /// The starting point of the drawing, representing the point where the mouse was first pressed.
    /// </summary>
    private Point startingPoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleDrawer"/> class.
    /// </summary>
    /// <param name="canvas">The canvas on which the rectangle will be drawn.</param>
    public RectangleDrawer(DrawableCanvas canvas) : base(canvas)
    {
    }

    /// <summary>
    /// Begins the drawing of a rectangle by recording the starting point of the mouse press.
    /// </summary>
    /// <param name="e">Mouse button event data that provides the position of the mouse when pressed.</param>
    public override void StartDrawing(MouseButtonEventArgs e)
    {
        startingPoint = e.GetPosition(DrawableCanvas);
        CurrentRectangle = new DrawableRectangle
        {
            Position = startingPoint,
            Size = new Size(0, 0),
            Color = CanvasInputHandler.GetCurrentColor(),
            Thickness = CanvasInputHandler.GetCurrentThickness(),
            Transparency = CanvasInputHandler.GetCurrentTransparency()
        };
        DrawableCanvas.AddElement(CurrentRectangle);
    }

    /// <summary>
    /// Updates the dimensions of the rectangle as the user drags the mouse.
    /// </summary>
    /// <param name="e">Mouse event data that provides the current position of the mouse.</param>
    public override void UpdateDrawing(MouseEventArgs e)
    {
        if (CurrentRectangle == null) return;

        var currentPoint = e.GetPosition(DrawableCanvas);

        // Calculate the top-left corner of the rectangle
        double left = Math.Min(currentPoint.X, startingPoint.X);
        double top = Math.Min(currentPoint.Y, startingPoint.Y);

        // Calculate the width and height of the rectangle
        double width = Math.Abs(currentPoint.X - startingPoint.X);
        double height = Math.Abs(currentPoint.Y - startingPoint.Y);

        CurrentRectangle.Position = new Point(left, top);
        CurrentRectangle.Size = new Size(width, height);

        DrawableCanvas.InvalidateVisual();
    }

    /// <summary>
    /// Finishes the drawing operation, allowing the rectangle to remain on the canvas.
    /// </summary>
    public override void FinishDrawing()
    {
        CurrentRectangle = null;
    }
}