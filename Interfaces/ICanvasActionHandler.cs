namespace screenerWpf.Interfaces;

using screenerWpf.Controls;
using System.Windows.Input;

/// <summary>
/// Defines the methods for handling various canvas actions such as drawing, moving, copying, and pasting elements.
/// </summary>
public interface ICanvasActionHandler
{
    /// <summary>
    /// Handles the action to be performed when the left mouse button is pressed down on the canvas.
    /// </summary>
    /// <param name="e">Event data containing information about the mouse button press.</param>
    void HandleLeftButtonDown(MouseButtonEventArgs e);

    /// <summary>
    /// Handles the action to be performed when the left mouse button is released on the canvas.
    /// </summary>
    /// <param name="e">Event data containing information about the mouse button release.</param>
    void HandleLeftButtonUp(MouseButtonEventArgs e);

    /// <summary>
    /// Handles the action to be performed when the mouse is moved across the canvas.
    /// </summary>
    /// <param name="e">Event data containing information about the mouse movement.</param>
    void HandleMouseMove(MouseEventArgs e);

    /// <summary>
    /// Handles the action to paste a copied element onto the canvas.
    /// </summary>
    void HandlePaste();

    /// <summary>
    /// Handles the action to copy the selected element on the canvas.
    /// </summary>
    void HandleCopy();

    /// <summary>
    /// Sets the current action to be performed on the canvas, such as drawing or editing elements.
    /// </summary>
    /// <param name="action">The action to set, which determines the type of operation on the canvas.</param>
    void SetCurrentAction(EditAction action);

    /// <summary>
    /// Gets the current action being performed on the canvas.
    /// </summary>
    /// <returns>The current action.</returns>
    EditAction GetCurrentAction();
}
