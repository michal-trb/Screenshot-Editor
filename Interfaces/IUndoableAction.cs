namespace screenerWpf.Interfaces;

/// <summary>
/// Interface representing an action that can be undone and redone.
/// </summary>
public interface IUndoableAction
{
    /// <summary>
    /// Executes the action.
    /// </summary>
    void Execute();

    /// <summary>
    /// Reverts the action, effectively undoing its effects.
    /// </summary>
    void UnExecute();
}
