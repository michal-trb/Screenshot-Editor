namespace screenerWpf.Interfaces;

using System.Windows.Input;

public interface ICanvasSelectionHandler
{
    /// <summary>
    /// Handles the mouse button down event to potentially select an element on the canvas.
    /// </summary>
    /// <param name="e">The mouse button event data.</param>
    void HandleLeftButtonDown(MouseButtonEventArgs e);

    /// <summary>
    /// Handles the mouse move event to potentially update the position of a selected element.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    void HandleMouseMove(MouseEventArgs e);

    /// <summary>
    /// Handles the mouse button up event to finalize the selection or movement of an element.
    /// </summary>
    /// <param name="e">The mouse button event data.</param>
    void HandleLeftButtonUp(MouseButtonEventArgs e);

    /// <summary>
    /// Deletes the currently selected element from the canvas.
    /// </summary>
    void DeleteSelectedElement();

    /// <summary>
    /// Checks if there is currently an element selected on the canvas.
    /// </summary>
    /// <returns>True if an element is selected, otherwise false.</returns>
    bool HasSelectedElement();

    /// <summary>
    /// Gets the currently selected element on the canvas.
    /// </summary>
    /// <returns>The selected element or null if no element is selected.</returns>
    IDrawable GetSelectedElement();

    /// <summary>
    /// Handles a mouse double-click event, potentially for editing an element.
    /// </summary>
    /// <param name="e">The mouse button event data.</param>
    void HandleDoubleClick(MouseButtonEventArgs e);
}