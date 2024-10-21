namespace screenerWpf.CanvasHandler.Drawers;

using screenerWpf.Controls;
using screenerWpf.Models.DrawableElements;
using System.Windows.Input;

/// <summary>
/// Responsible for handling the drawing of brush strokes on the canvas.
/// </summary>
public class BrushDrawer : DrawableElementDrawer
{
    private DrawableBrush currentBrushStroke;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrushDrawer"/> class.
    /// </summary>
    /// <param name="canvas">The canvas on which brush strokes are drawn.</param>
    public BrushDrawer(DrawableCanvas canvas) : base(canvas)
    {
    }

    /// <summary>
    /// Starts a new brush stroke on the canvas when the user presses the left mouse button.
    /// </summary>
    /// <param name="e">Mouse button event data.</param>
    public override void StartDrawing(MouseButtonEventArgs e)
    {
        var position = e.GetPosition(DrawableCanvas);
        currentBrushStroke = new DrawableBrush(
            CanvasInputHandler.GetCurrentColor(),
            CanvasInputHandler.GetCurrentThickness(),
            CanvasInputHandler.GetCurrentTransparency());
        DrawableCanvas.AddElement(currentBrushStroke);
    }

    /// <summary>
    /// Updates the current brush stroke as the user moves the mouse with the button pressed.
    /// </summary>
    /// <param name="e">Mouse event data.</param>
    public override void UpdateDrawing(MouseEventArgs e)
    {
        if (currentBrushStroke == null) return;
        var position = e.GetPosition(DrawableCanvas);
        currentBrushStroke.AddPoint(position);
        DrawableCanvas.InvalidateVisual();
    }

    /// <summary>
    /// Finalizes the current brush stroke when the user releases the mouse button.
    /// </summary>
    public override void FinishDrawing()
    {
        currentBrushStroke = null;
    }
}