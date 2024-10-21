namespace screenerWpf.Services;

using screenerWpf.Controls;
using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;

/// <summary>
/// Represents an action to add an element to the canvas, which supports undo and redo operations.
/// </summary>
public class AddElementAction : IUndoableAction
{
    private DrawableCanvas canvas;
    /// <summary>
    /// Gets the element to be added to the canvas.
    /// </summary>
    public DrawableElement Element { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddElementAction"/> class.
    /// </summary>
    /// <param name="canvas">The canvas to which the element will be added.</param>
    /// <param name="element">The drawable element to add to the canvas.</param>
    public AddElementAction(DrawableCanvas canvas, DrawableElement element)
    {
        this.canvas = canvas;
        Element = element;
    }

    /// <summary>
    /// Executes the action to add the element to the canvas.
    /// </summary>
    public void Execute()
    {
        canvas.elementManager.AddElement(Element);
    }

    /// <summary>
    /// Undoes the action by removing the element from the canvas.
    /// </summary>
    public void UnExecute()
    {
        canvas.elementManager.RemoveElement(Element);
    }
}
