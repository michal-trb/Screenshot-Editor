namespace screenerWpf.Services;

using screenerWpf.Controls;
using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;

/// <summary>
/// Represents an action to remove an element from the canvas, which supports undo and redo operations.
/// </summary>
public class RemoveElementAction : IUndoableAction
{
    private DrawableCanvas canvas;
    /// <summary>
    /// Gets the element to be removed from the canvas.
    /// </summary>
    public DrawableElement Element { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveElementAction"/> class.
    /// </summary>
    /// <param name="canvas">The canvas from which the element will be removed.</param>
    /// <param name="element">The drawable element to remove from the canvas.</param>
    public RemoveElementAction(DrawableCanvas canvas, DrawableElement element)
    {
        this.canvas = canvas;
        Element = element;
    }

    /// <summary>
    /// Executes the action to remove the element from the canvas.
    /// </summary>
    public void Execute()
    {
        canvas.elementManager.RemoveElement(Element);
    }

    /// <summary>
    /// Undoes the action by adding the element back to the canvas.
    /// </summary>
    public void UnExecute()
    {
        canvas.elementManager.AddElement(Element);
    }
}
