namespace screenerWpf.Interfaces;

public interface ICanvasSavingHandler
{
    /// <summary>
    /// Opens a dialog to save the canvas content to an image file.
    /// </summary>
    void SaveCanvasToFile();

    /// <summary>
    /// Saves the canvas content to an image file with default parameters (e.g., a predefined path and filename).
    /// </summary>
    /// <returns>The full path of the saved image file.</returns>
    string SaveCanvasToFileFast();

    /// <summary>
    /// Opens a dialog to save the canvas content to a PDF file.
    /// </summary>
    void SaveCanvasToPdfFile();
}
