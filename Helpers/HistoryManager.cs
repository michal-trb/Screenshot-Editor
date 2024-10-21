namespace screenerWpf.Helpers;

using screenerWpf.Interfaces;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the undo and redo history of actions performed on the canvas.
/// </summary>
public class HistoryManager
{
    private Stack<IUndoableAction> undoStack = new Stack<IUndoableAction>();
    private Stack<IUndoableAction> redoStack = new Stack<IUndoableAction>();

    /// <summary>
    /// Adds an action to the undo history and clears the redo history.
    /// </summary>
    /// <param name="action">The action to add to the undo history.</param>
    public void AddAction(IUndoableAction action)
    {
        undoStack.Push(action);
        redoStack.Clear();
    }

    /// <summary>
    /// Gets a value indicating whether an undo operation is available.
    /// </summary>
    public bool CanUndo => undoStack.Any();

    /// <summary>
    /// Gets a value indicating whether a redo operation is available.
    /// </summary>
    public bool CanRedo => redoStack.Any();

    /// <summary>
    /// Undoes the most recent action if available.
    /// </summary>
    public void Undo()
    {
        if (!CanUndo) return;

        var action = undoStack.Pop();
        action.UnExecute();
        redoStack.Push(action);
    }

    /// <summary>
    /// Redoes the most recently undone action if available.
    /// </summary>
    public void Redo()
    {
        if (!CanRedo) return;

        var action = redoStack.Pop();
        action.Execute();
        undoStack.Push(action);
    }
}