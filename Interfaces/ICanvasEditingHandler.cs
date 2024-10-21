namespace screenerWpf.Interfaces;

using System.Windows;

public interface ICanvasEditingHandler
{
    /// <summary>
    /// Starts editing a drawable element at the specified location on the canvas.
    /// </summary>
    /// <param name="element">The drawable element to be edited.</param>
    /// <param name="location">The location where the editing will start.</param>
    void StartEditing(IDrawable element, Point location);
}