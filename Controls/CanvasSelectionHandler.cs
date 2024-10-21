namespace screenerWpf.Controls;

using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

/// <summary>
/// Handles the selection and movement of drawable elements on a canvas.
/// This class manages user interactions for selecting, dragging, and editing elements,
/// as well as initiating editing operations like text editing and bubble speech modifications.
/// </summary>
public class CanvasSelectionHandler : ICanvasSelectionHandler
{
    private DrawableCanvas drawableCanvas;
    private IDrawable selectedElement;
    private Point lastMousePosition;
    private ICanvasEditingHandler editingHandler;
    private const double SpeechBubbleTailTolerance = 40;
    private const double ArrowTolerance = 30;

    /// <summary>
    /// Initializes a new instance of the CanvasSelectionHandler class with the specified canvas and editing handler.
    /// </summary>
    /// <param name="canvas">The drawable canvas on which elements are managed.</param>
    /// <param name="editingHandler">The handler used to edit selected elements on the canvas.</param>
    public CanvasSelectionHandler(DrawableCanvas canvas, ICanvasEditingHandler editingHandler)
    {
        drawableCanvas = canvas;
        this.editingHandler = editingHandler;
    }

    /// <summary>
    /// Handles the mouse left button down event, selecting an element if present.
    /// </summary>
    /// <param name="e">The mouse button event arguments.</param>
    public void HandleLeftButtonDown(MouseButtonEventArgs e)
    {
        if (drawableCanvas.isFirstClick)
        {
            drawableCanvas.originalTargetBitmap = drawableCanvas.GetRenderTargetBitmap();
            drawableCanvas.isFirstClick = false;
        }

        Point clickPosition = e.GetPosition(drawableCanvas);
        lastMousePosition = clickPosition;

        if (!TrySelectSpeechBubbleTail(clickPosition) && !TrySelectElement(clickPosition))
        {
            // If no speech bubble tail or arrow was clicked, check for other elements
            var element = drawableCanvas.elementManager.GetElementAtPoint(clickPosition);

            if (element != null)
            {
                selectedElement = element;
            }
            else
            {
                DeselectCurrentElement();
            }
        }
    }

    /// <summary>
    /// Handles the double-click event, starting the editing process for text or speech bubble elements.
    /// </summary>
    /// <param name="e">The mouse button event arguments.</param>
    public void HandleDoubleClick(MouseButtonEventArgs e)
    {
        Point clickPosition = e.GetPosition(drawableCanvas);
        lastMousePosition = clickPosition;

        if (drawableCanvas.isFirstClick)
        {
            drawableCanvas.originalTargetBitmap = drawableCanvas.GetRenderTargetBitmap();
            drawableCanvas.isFirstClick = false;
        }

        var element = drawableCanvas.elementManager.GetElementAtPoint(clickPosition);
        if (element != null)
        {
            selectedElement = element;

            if (element is DrawableText drawableText)
            {
                editingHandler.StartEditing(drawableText, clickPosition);
            }
            else if (element is DrawableSpeechBubble speechBubble)
            {
                editingHandler.StartEditing(speechBubble, clickPosition);
            }
        }
        else
        {
            DeselectCurrentElement();
        }
    }

    /// <summary>
    /// Attempts to select the tail of a speech bubble if clicked near enough.
    /// </summary>
    /// <param name="clickPoint">The position of the mouse click.</param>
    /// <returns>True if a speech bubble tail was selected, otherwise false.</returns>
    private bool TrySelectSpeechBubbleTail(Point clickPoint)
    {
        foreach (var element in drawableCanvas.elementManager.Elements.OfType<DrawableSpeechBubble>())
        {
            if (IsNearPoint(element.EndTailPoint, clickPoint, SpeechBubbleTailTolerance))
            {
                selectedElement = element;
                element.SetTailBeingDragged(true);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Attempts to select an element at the specified point.
    /// </summary>
    /// <param name="clickPoint">The position of the mouse click.</param>
    /// <returns>True if an element was selected, otherwise false.</returns>
    private bool TrySelectElement(Point clickPoint)
    {
        var element = drawableCanvas.elementManager.GetElementAtPoint(clickPoint);

        if (element != null)
        {
            selectedElement = element;
            HandleArrowSpecificLogic(element, clickPoint);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Handles specific logic for selecting an arrow, including dragging the start or end points.
    /// </summary>
    /// <param name="element">The drawable element to handle.</param>
    /// <param name="clickPoint">The position of the mouse click.</param>
    private void HandleArrowSpecificLogic(DrawableElement element, Point clickPoint)
    {
        if (element is DrawableArrow arrow)
        {
            arrow.SetStartBeingDragged(IsNearPoint(arrow.Position, clickPoint, ArrowTolerance));
            arrow.SetEndBeingDragged(IsNearPoint(arrow.EndPoint, clickPoint, ArrowTolerance));
        }
    }

    /// <summary>
    /// Checks if two points are within a certain tolerance of each other.
    /// </summary>
    /// <param name="point1">The first point.</param>
    /// <param name="point2">The second point.</param>
    /// <param name="tolerance">The tolerance within which the points are considered "near" each other.</param>
    /// <returns>True if the points are near each other, otherwise false.</returns>
    private bool IsNearPoint(Point point1, Point point2, double tolerance)
    {
        return Math.Abs(point1.X - point2.X) <= tolerance && Math.Abs(point1.Y - point2.Y) <= tolerance;
    }

    /// <summary>
    /// Handles the mouse move event, updating the position of the selected element if it is being dragged.
    /// </summary>
    /// <param name="e">The mouse event arguments.</param>
    public void HandleMouseMove(MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && selectedElement != null)
        {
            Point currentMousePosition = e.GetPosition(drawableCanvas);
            Vector delta = currentMousePosition - lastMousePosition;
            selectedElement.Move(delta);
            lastMousePosition = currentMousePosition;
            drawableCanvas.InvalidateVisual();
        }
    }

    /// <summary>
    /// Handles the mouse left button up event, stopping any active dragging actions.
    /// </summary>
    /// <param name="e">The mouse button event arguments.</param>
    public void HandleLeftButtonUp(MouseButtonEventArgs e)
    {
        if (selectedElement is DrawableSpeechBubble speechBubble)
        {
            speechBubble.SetTailBeingDragged(false);
        }
        else if (selectedElement is DrawableArrow arrow)
        {
            arrow.SetEndBeingDragged(false);
            arrow.SetStartBeingDragged(false);
        }
    }

    /// <summary>
    /// Deselects the currently selected element.
    /// </summary>
    private void DeselectCurrentElement()
    {
        selectedElement = null;
        drawableCanvas.InvalidateVisual();
    }

    /// <summary>
    /// Deletes the currently selected element from the canvas.
    /// </summary>
    public void DeleteSelectedElement()
    {
        if (selectedElement != null)
        {
            drawableCanvas.RemoveElement(selectedElement);
            DeselectCurrentElement();
            drawableCanvas.InvalidateVisual();
        }
    }

    /// <summary>
    /// Determines if there is an element currently selected.
    /// </summary>
    /// <returns>True if an element is selected, otherwise false.</returns>
    public bool HasSelectedElement()
    {
        return selectedElement != null;
    }

    /// <summary>
    /// Gets the currently selected drawable element.
    /// </summary>
    /// <returns>The selected drawable element, or null if none is selected.</returns>
    public IDrawable GetSelectedElement()
    {
        return selectedElement;
    }
}
