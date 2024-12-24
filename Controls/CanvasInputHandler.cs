namespace screenerWpf.Controls;

using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;
using screenerWpf.Sevices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


/// <summary>
/// Handles the input events for the drawable canvas, including mouse interactions and user actions.
/// This class is responsible for coordinating various input actions, such as drawing, selecting, and editing elements on the canvas.
/// </summary>
public class CanvasInputHandler : ICanvasInputHandler
{
    private readonly DrawableCanvas drawableCanvas;
    private readonly ICanvasActionHandler actionHandler;
    private readonly ICanvasSelectionHandler selectionHandler;
    private readonly ICanvasEditingHandler editingHandler;
    private readonly ICanvasSavingHandler savingHandler;

    public static FontFamily SelectedFontFamily { get; private set; } = new FontFamily("Arial");
    public static double SelectedFontSize { get; private set; } = 12.0;
    public static double ArrowThickness { get; private set; } = 2.0;
    public static double Transparency { get; private set; } = 0;
    public static Color SelectedColor { get; private set; } = Colors.Black;

    private EditAction currentAction = EditAction.None;

    /// <summary>
    /// Initializes a new instance of the CanvasInputHandler class.
    /// </summary>
    /// <param name="canvas">The drawable canvas that will handle the user interactions.</param>
    public CanvasInputHandler(DrawableCanvas canvas)
    {
        drawableCanvas = canvas;

        actionHandler = new CanvasActionHandler(drawableCanvas);
        editingHandler = new CanvasEditingHandler(drawableCanvas);
        savingHandler = new CanvasSavingHandler(drawableCanvas);
        selectionHandler = new CanvasSelectionHandler(drawableCanvas, editingHandler, actionHandler);
    }

    /// <summary>
    /// Handles the mouse left button down event for the canvas, delegating to selection and action handlers.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The mouse button event arguments.</param>
    public void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // First try to handle selection
        selectionHandler.HandleLeftButtonDown(e);

        // If no element is selected, handle drawing
        if (!selectionHandler.HasSelectedElement())
        {
            actionHandler.HandleLeftButtonDown(e);
        }
    }

    /// <summary>
    /// Handles the mouse double-click event for the canvas, delegating to the selection handler.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The mouse button event arguments.</param>
    public void Canvas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        selectionHandler.HandleDoubleClick(e);
    }

    /// <summary>
    /// Handles the mouse left button up event for the canvas, delegating to selection and action handlers.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The mouse button event arguments.</param>
    public void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        selectionHandler.HandleLeftButtonUp(e);
        actionHandler.HandleLeftButtonUp(e);
    }

    /// <summary>
    /// Handles the mouse move event for the canvas, delegating to selection and action handlers.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The mouse event arguments.</param>
    public void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        selectionHandler.HandleMouseMove(e);
        actionHandler.HandleMouseMove(e);
    }

    /// <summary>
    /// Executes the delete command, deleting the selected element.
    /// </summary>
    public void CommandBinding_DeleteExecuted()
    {
        selectionHandler.DeleteSelectedElement();
    }

    /// <summary>
    /// Toggles the drawing action and updates the current action state.
    /// </summary>
    /// <param name="action">The action to toggle</param>
    public void ToggleAction(EditAction action)
    {
        actionHandler.SetCurrentAction(action);

        UpdateDrawingColorAndThickness();
    }

    /// <summary>
    /// Sets the action to draw an arrow on the canvas.
    /// </summary>
    public void DrawArrow()
    {
        ToggleAction(EditAction.DrawArrow);
    }

    /// <summary>
    /// Sets the action to draw a rectangle on the canvas.
    /// </summary>
    public void DrawRect()
    {
        ToggleAction(EditAction.DrawRectangle);
    }

    /// <summary>
    /// Sets the action to add text to the canvas.
    /// </summary>
    public void AddText()
    {
        ToggleAction(EditAction.AddText);
    }

    /// <summary>
    /// Saves the current canvas to a file.
    /// </summary>
    public void Save()
    {
        savingHandler.SaveCanvasToFile();
    }

    /// <summary>
    /// Saves the current canvas quickly to a file and returns the file path.
    /// </summary>
    /// <returns>The file path of the saved canvas.</returns>
    public string SaveFast()
    {
        return savingHandler.SaveCanvasToFileFast();
    }

    /// <summary>
    /// Updates the drawing color and thickness settings.
    /// </summary>
    private void UpdateDrawingColorAndThickness()
    {
        // Updates the color and thickness of the arrow or rectangle to match the user's selections.
    }

    /// <summary>
    /// Sets the action to add a speech bubble to the canvas.
    /// </summary>
    public void SpeechBubble()
    {
        ToggleAction(EditAction.AddBubble);
    }

    /// <summary>
    /// Sets the action to blur an area on the canvas.
    /// </summary>
    public void Blur()
    {
        ToggleAction(EditAction.DrawBlur);
    }

    /// <summary>
    /// Sets the action to paint on the canvas with a brush tool.
    /// </summary>
    public void Brush()
    {
        ToggleAction(EditAction.BrushPainting);
    }

    /// <summary>
    /// Initiates text recognition on the current canvas.
    /// </summary>
    public void RecognizeText()
    {
        var textRecognitionHandler = new TextRecognitionHandler(drawableCanvas);
        textRecognitionHandler.StartRecognizeFromImage();
    }

    /// <summary>
    /// Changes the font family for the selected text or speech bubble element.
    /// </summary>
    /// <param name="selectedFontFamily">The new font family to apply.</param>
    public void ChangeFontFamily(FontFamily selectedFontFamily)
    {
        SelectedFontFamily = selectedFontFamily;
        var typeface = GetCurrentTypeface();
        if (selectionHandler.HasSelectedElement())
        {
            var selectedElement = selectionHandler.GetSelectedElement();
            if (selectedElement is DrawableText text)
            {
                text.Typeface = typeface;
                drawableCanvas.InvalidateVisual();
            }
            if (selectedElement is DrawableSpeechBubble speechBubble)
            {
                speechBubble.Typeface = typeface;
                drawableCanvas.InvalidateVisual();
            }
        }
    }

    /// <summary>
    /// Changes the font size for the selected text or speech bubble element.
    /// </summary>
    /// <param name="fontSize">The new font size to apply.</param>
    public void ChangeFontSize(double fontSize)
    {
        SelectedFontSize = fontSize;
        if (selectionHandler.HasSelectedElement())
        {
            var selectedElement = selectionHandler.GetSelectedElement();
            if (selectedElement is DrawableText text)
            {
                text.FontSize = SelectedFontSize;
                drawableCanvas.InvalidateVisual();
            }
            if (selectedElement is DrawableSpeechBubble speechBubble)
            {
                speechBubble.FontSize = SelectedFontSize;
                drawableCanvas.InvalidateVisual();
            }
        }
    }

    /// <summary>
    /// Changes the color for the selected element.
    /// </summary>
    /// <param name="color">The new color to apply.</param>
    public void ChangeColor(Color color)
    {
        SelectedColor = color;
        if (selectionHandler.HasSelectedElement())
        {
            var selectedElement = selectionHandler.GetSelectedElement();
            if (selectedElement is DrawableText text)
            {
                text.Color = SelectedColor;
                drawableCanvas.InvalidateVisual();
            }
            if (selectedElement is DrawableSpeechBubble speechBubble)
            {
                speechBubble.Brush = new SolidColorBrush(SelectedColor);
                drawableCanvas.InvalidateVisual();
            }
            if (selectedElement is DrawableRectangle rectangle)
            {
                rectangle.Color = SelectedColor;
                drawableCanvas.InvalidateVisual();
            }
            if (selectedElement is DrawableBrush brush)
            {
                brush.Color = SelectedColor;
                drawableCanvas.InvalidateVisual();
            }
            if (selectedElement is DrawableArrow arrow)
            {
                arrow.Color = SelectedColor;
                drawableCanvas.InvalidateVisual();
            }
        }
    }

    /// <summary>
    /// Changes the thickness of the arrow or rectangle for the selected element.
    /// </summary>
    /// <param name="comboBoxArrowThickness">The new thickness value to apply.</param>
    public void ChangeArrowThickness(double comboBoxArrowThickness)
    {
        ArrowThickness = comboBoxArrowThickness;
        if (selectionHandler.HasSelectedElement())
        {
            var selectedElement = selectionHandler.GetSelectedElement();
            if (selectedElement != null)
            {
                if (selectedElement is DrawableArrow arrow)
                {
                    arrow.Thickness = ArrowThickness;
                    drawableCanvas.InvalidateVisual();
                }
                if (selectedElement is DrawableRectangle rectangle)
                {
                    rectangle.Thickness = ArrowThickness;
                    drawableCanvas.InvalidateVisual();
                }
                if (selectedElement is DrawableBrush brush)
                {
                    brush.thickness = ArrowThickness;
                    drawableCanvas.InvalidateVisual();
                }
            }
        }
    }

    /// <summary>
    /// Changes the transparency level of the selected element.
    /// </summary>
    /// <param name="transparency">The new transparency level to apply.</param>
    public void ChangeTransparency(double transparency)
    {
        Transparency = transparency;
        if (selectionHandler.HasSelectedElement())
        {
            var selectedElement = selectionHandler.GetSelectedElement();
            if (selectedElement != null)
            {
                if (selectedElement is DrawableBrush brush)
                {
                    brush.transparency = Transparency;
                    drawableCanvas.InvalidateVisual();
                }
                if (selectedElement is DrawableRectangle rectangle)
                {
                    rectangle.Transparency = Transparency;
                    drawableCanvas.InvalidateVisual();
                }
            }
        }
    }

    /// <summary>
    /// Gets the current selected color.
    /// </summary>
    /// <returns>The currently selected color.</returns>
    public static Color GetCurrentColor()
    {
        return SelectedColor;
    }

    /// <summary>
    /// Gets the current typeface based on the selected font family.
    /// </summary>
    /// <returns>The current typeface.</returns>
    public static Typeface GetCurrentTypeface()
    {
        return new Typeface(SelectedFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
    }

    /// <summary>
    /// Gets the current selected font family.
    /// </summary>
    /// <returns>The current font family.</returns>
    public static FontFamily GetCurrentFontFamily()
    {
        return SelectedFontFamily;
    }

    /// <summary>
    /// Gets the current selected font size.
    /// </summary>
    /// <returns>The current font size.</returns>
    public static double GetCurrentFontSize()
    {
        return SelectedFontSize;
    }

    /// <summary>
    /// Gets the current thickness for drawing arrows or rectangles.
    /// </summary>
    /// <returns>The current thickness.</returns>
    public static double GetCurrentThickness()
    {
        return ArrowThickness;
    }

    /// <summary>
    /// Gets the current transparency level.
    /// </summary>
    /// <returns>The current transparency level.</returns>
    public static double GetCurrentTransparency()
    {
        return Transparency;
    }

    /// <summary>
    /// Placeholder for editing text functionality.
    /// </summary>
    public void EditText()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Executes the copy command, copying the current selection.
    /// </summary>
    public void CommandBinding_CopyExecuted()
    {
        actionHandler.HandleCopy();
    }

    /// <summary>
    /// Executes the paste command, pasting the last copied selection onto the canvas.
    /// </summary>
    public void CommandBinding_PasteExecuted()
    {
        actionHandler.HandlePaste();
    }

    public void SetCurrentAction(EditAction select)
    {
        actionHandler.SetCurrentAction(select);
    }
}
