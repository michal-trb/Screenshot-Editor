namespace screenerWpf.Controls;

using global::Helpers.DpiHelper;
using screenerWpf.CanvasHandler.Drawers;
using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// <summary>
/// Enumeration representing different types of edit actions that can be performed on the canvas.
/// </summary>
public enum EditAction
{
    None,               // No action / Selection mode
    DrawArrow,          // Drawing arrow
    AddText,           // Adding text
    Move,              // Moving element
    Delete,            // Deleting element
    AddBubble,         // Adding speech bubble
    DrawRectangle,     // Drawing rectangle
    DrawBlur,          // Drawing blur
    BrushPainting,     // Brush painting
    RecognizeText,     // Text recognition
    Select,            // Element selection
    EditText,          // Text editing
    EditBubble,        // Speech bubble editing
    DragTail,          // Dragging speech bubble tail
    DragArrow,
}

/// <summary>
/// Handles various canvas actions such as drawing, text addition, blurring, and other edit operations.
/// </summary>
public class CanvasActionHandler : ICanvasActionHandler
{
    private DrawableCanvas drawableCanvas;
    private IDrawable currentDrawable;
    private EditAction currentAction = EditAction.None;

    private ArrowDrawer arrowDrawer;
    private RectangleDrawer rectangleDrawer;
    private SpeechBubbleDrawer speechBubbleDrawer;
    private TextDrawer textDrawer;
    private BlurDrawer blurDrawer;
    private BrushDrawer brushDrawer;

    /// <summary>
    /// Initializes a new instance of the CanvasActionHandler class with the specified drawable canvas.
    /// </summary>
    /// <param name="canvas">The canvas on which actions will be performed.</param>
    public CanvasActionHandler(DrawableCanvas canvas)
    {
        drawableCanvas = canvas;
        arrowDrawer = new ArrowDrawer(currentDrawable, canvas);
        rectangleDrawer = new RectangleDrawer(canvas);
        speechBubbleDrawer = new SpeechBubbleDrawer(canvas);
        textDrawer = new TextDrawer(canvas);
        blurDrawer = new BlurDrawer(canvas);
        brushDrawer = new BrushDrawer(canvas);
    }

    /// <summary>
    /// Handles the mouse left button down event for different drawing actions.
    /// </summary>
    /// <param name="e">The mouse button event arguments.</param>
    public void HandleLeftButtonDown(MouseButtonEventArgs e)
    {
        switch (currentAction)
        {
            case EditAction.DrawArrow:
                arrowDrawer.StartDrawing(e);
                break;
            case EditAction.AddText:
                textDrawer.StartDrawing(e);
                break;
            case EditAction.AddBubble:
                speechBubbleDrawer.StartDrawing(e);
                break;
            case EditAction.DrawRectangle:
                rectangleDrawer.StartDrawing(e);
                break;
            case EditAction.DrawBlur:
                blurDrawer.StartDrawing(e);
                break;
            case EditAction.BrushPainting:
                brushDrawer.StartDrawing(e);
                break;
        }
    }

    /// <summary>
    /// Handles the mouse left button up event to finish the current drawing action.
    /// </summary>
    /// <param name="e">The mouse button event arguments.</param>
    public void HandleLeftButtonUp(MouseButtonEventArgs e)
    {
        switch (currentAction)
        {
            case EditAction.DrawArrow:
                arrowDrawer.FinishDrawing();
                break;
            case EditAction.AddText:
                textDrawer.FinishDrawing();
                SetCurrentAction(EditAction.None);
                break;
            case EditAction.AddBubble:
                speechBubbleDrawer.FinishDrawing();
                break;
            case EditAction.DrawRectangle:
                rectangleDrawer.FinishDrawing();
                break;
            case EditAction.DrawBlur:
                blurDrawer.FinishDrawing();
                break;
            case EditAction.BrushPainting:
                brushDrawer.FinishDrawing();
                break;
        }
    }

    /// <summary>
    /// Handles the mouse move event to update the current drawing action as the mouse moves.
    /// </summary>
    /// <param name="e">The mouse event arguments.</param>
    public void HandleMouseMove(MouseEventArgs e)
    {
        switch (currentAction)
        {
            case EditAction.DrawArrow:
                arrowDrawer.UpdateDrawing(e);
                break;
            case EditAction.DrawRectangle:
                rectangleDrawer.UpdateDrawing(e);
                break;
            case EditAction.DrawBlur:
                blurDrawer.UpdateDrawing(e);
                break;
            case EditAction.BrushPainting:
                brushDrawer.UpdateDrawing(e);
                break;
        }
    }

    /// <summary>
    /// Handles the paste operation, adding an image from the clipboard to the canvas.
    /// </summary>
    public void HandlePaste()
    {
        if (Clipboard.ContainsImage())
        {
            var imageSource = Clipboard.GetImage();
            Point canvasPosition = new Point(10, 10);
            var drawableImage = new DrawableImage(imageSource, canvasPosition);
            drawableCanvas.AddElement(drawableImage);
        }
    }

    /// <summary>
    /// Handles the copy operation, copying the current canvas to the clipboard as an image.
    /// </summary>
    public void HandleCopy()
    {
        var currentDpi = DpiHelper.CurrentDpi;

        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            (int)drawableCanvas.ActualWidth,
            (int)drawableCanvas.ActualHeight,
            currentDpi.DpiX,
            currentDpi.DpiY,
            PixelFormats.Pbgra32);

        DrawingVisual visual = new DrawingVisual();
        using (DrawingContext context = visual.RenderOpen())
        {
            VisualBrush canvasBrush = new VisualBrush(drawableCanvas);
            context.DrawRectangle(canvasBrush, null, new Rect(new Point(), new Size(drawableCanvas.ActualWidth, drawableCanvas.ActualHeight)));
        }

        renderBitmap.Render(visual);
        Clipboard.SetImage(renderBitmap);
    }

    /// <summary>
    /// Sets the current edit action to be performed on the canvas.
    /// </summary>
    /// <param name="action">The action to be set as the current edit action.</param>
    public void SetCurrentAction(EditAction action)
    {
        currentAction = action;
    }

    /// <summary>
    /// Gets the current action being performed on the canvas.
    /// </summary>
    /// <returns>The current EditAction.</returns>
    public EditAction GetCurrentAction()
    {
        return currentAction;
    }
}
