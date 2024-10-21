namespace screenerWpf.CanvasHandler.Drawers;

using screenerWpf.Controls;
using screenerWpf.Models.DrawableElement;
using screenerWpf.Models.DrawableElements;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;


/// <summary>
/// Handles drawing a blurred area on the drawable canvas.
/// </summary>
public class BlurDrawer : DrawableElementDrawer
{
    /// <summary>
    /// Represents the current blur area being drawn.
    /// </summary>
    private DrawableBlur CurrentBlurArea { get; set; }

    private Point startDragPoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlurDrawer"/> class with the specified drawable canvas.
    /// </summary>
    /// <param name="canvas">The canvas where the blur area will be drawn.</param>
    public BlurDrawer(DrawableCanvas canvas) : base(canvas)
    {
    }

    /// <summary>
    /// Starts the process of drawing the blur area by recording the start point.
    /// </summary>
    /// <param name="e">The mouse button event that contains the start position of the blur area.</param>
    public override void StartDrawing(MouseButtonEventArgs e)
    {
        startDragPoint = e.GetPosition(DrawableCanvas);
        InitializeDrawableElements(startDragPoint);
    }

    /// <summary>
    /// Updates the blur area as the mouse moves, resizing the area based on the current position.
    /// </summary>
    /// <param name="e">The mouse event that contains the current position of the cursor.</param>
    public override void UpdateDrawing(MouseEventArgs e)
    {
        if (CurrentBlurArea == null) return;

        var currentPoint = e.GetPosition(DrawableCanvas);
        UpdateBlurArea(currentPoint);

        DrawableCanvas.InvalidateVisual();
    }

    /// <summary>
    /// Finishes drawing the blur area by clearing the current blur reference.
    /// </summary>
    public override void FinishDrawing()
    {
        CurrentBlurArea = null;
    }

    /// <summary>
    /// Initializes the drawable elements used for the blur area, setting its initial position and properties.
    /// </summary>
    /// <param name="startPoint">The start point where the blur area drawing begins.</param>
    private void InitializeDrawableElements(Point startPoint)
    {
        CurrentBlurArea = new DrawableBlur(DrawableCanvas)
        {
            Position = startPoint,
            Size = new Size(0, 0),
            StrokeColor = Colors.Transparent,
            StrokeThickness = 0,
            BlurEffect = new BlurEffect { Radius = 10 }
        };

        DrawableCanvas.AddElement(CurrentBlurArea);
    }

    /// <summary>
    /// Updates the size and position of the blur area based on the current cursor position.
    /// </summary>
    /// <param name="currentPoint">The current point of the cursor used to define the blur area's dimensions.</param>
    private void UpdateBlurArea(Point currentPoint)
    {
        var x = Math.Min(currentPoint.X, startDragPoint.X);
        var y = Math.Min(currentPoint.Y, startDragPoint.Y);
        var width = Math.Abs(currentPoint.X - startDragPoint.X);
        var height = Math.Abs(currentPoint.Y - startDragPoint.Y);

        CurrentBlurArea.Position = new Point(x, y);
        CurrentBlurArea.Size = new Size(width, height);
        CurrentBlurArea.UpdateVisual();
    }
}