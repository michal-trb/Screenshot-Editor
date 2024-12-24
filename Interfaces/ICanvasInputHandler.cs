namespace screenerWpf.Interfaces;

using screenerWpf.Controls;
using System.Windows.Input;
using System.Windows.Media;

public interface ICanvasInputHandler
{
    /// <summary>
    /// Handles the event when the left mouse button is pressed down on the canvas.
    /// </summary>
    void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e);

    /// <summary>
    /// Handles the event when the mouse is double-clicked on the canvas.
    /// </summary>
    void Canvas_MouseDoubleClick(object sender, MouseButtonEventArgs e);

    /// <summary>
    /// Handles the event when the left mouse button is released on the canvas.
    /// </summary>
    void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e);

    /// <summary>
    /// Handles the mouse movement event over the canvas.
    /// </summary>
    void Canvas_MouseMove(object sender, MouseEventArgs e);

    /// <summary>
    /// Initiates the drawing of an arrow.
    /// </summary>
    void DrawArrow();

    /// <summary>
    /// Initiates the drawing of a rectangle.
    /// </summary>
    void DrawRect();

    /// <summary>
    /// Adds a text element to the canvas.
    /// </summary>
    void AddText();

    /// <summary>
    /// Saves the canvas content to an image file.
    /// </summary>
    void Save();

    /// <summary>
    /// Adds a speech bubble to the canvas.
    /// </summary>
    void SpeechBubble();

    /// <summary>
    /// Adds a blur effect to the canvas.
    /// </summary>
    void Blur();

    /// <summary>
    /// Enables brush painting mode on the canvas.
    /// </summary>
    void Brush();

    /// <summary>
    /// Recognizes text in the selected area of the canvas.
    /// </summary>
    void RecognizeText();

    /// <summary>
    /// Initiates editing of text on the canvas.
    /// </summary>
    void EditText();

    /// <summary>
    /// Changes the font family of the selected text or speech bubble.
    /// </summary>
    void ChangeFontFamily(FontFamily selectedFontFamily);

    /// <summary>
    /// Changes the font size of the selected text or speech bubble.
    /// </summary>
    void ChangeFontSize(double fontSize);

    /// <summary>
    /// Changes the color of the selected drawable element.
    /// </summary>
    void ChangeColor(Color color);

    /// <summary>
    /// Changes the thickness of an arrow or rectangle.
    /// </summary>
    void ChangeArrowThickness(double thickness);

    /// <summary>
    /// Changes the transparency of the selected drawable element.
    /// </summary>
    void ChangeTransparency(double transparency);

    /// <summary>
    /// Saves the canvas content to an image file with default parameters.
    /// </summary>
    /// <returns>The full path of the saved image.</returns>
    string SaveFast();

    /// <summary>
    /// Executes the delete command to remove the selected element.
    /// </summary>
    void CommandBinding_DeleteExecuted();

    /// <summary>
    /// Executes the copy command to copy the selected element.
    /// </summary>
    void CommandBinding_CopyExecuted();

    /// <summary>
    /// Executes the paste command to paste the copied element.
    /// </summary>
    void CommandBinding_PasteExecuted();
    void SetCurrentAction(EditAction select);
}