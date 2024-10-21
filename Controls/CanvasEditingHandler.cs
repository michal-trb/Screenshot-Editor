namespace screenerWpf.Controls;

using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Handles the editing of drawable elements on the canvas.
/// This class is responsible for starting and managing the editing process of text and speech bubble elements on the drawable canvas.
/// </summary>
public class CanvasEditingHandler : ICanvasEditingHandler
{
    private DrawableCanvas drawableCanvas;
    private IDrawable editableElement;

    /// <summary>
    /// Initializes a new instance of the CanvasEditingHandler class with the specified drawable canvas.
    /// </summary>
    /// <param name="canvas">The drawable canvas where elements are edited.</param>
    public CanvasEditingHandler(DrawableCanvas canvas)
    {
        drawableCanvas = canvas;
    }

    /// <summary>
    /// Starts the editing process for a given drawable element at a specified location on the canvas.
    /// </summary>
    /// <param name="element">The drawable element to be edited.</param>
    /// <param name="location">The location where the editing is initiated.</param>
    public void StartEditing(IDrawable element, Point location)
    {
        if (element is DrawableText drawableText)
        {
            EditTextBox(drawableText, location);
            editableElement = element;
        }
        else if (element is DrawableSpeechBubble drawableSpeechBubble)
        {
            EditSpeechBubble(drawableSpeechBubble, location);
            editableElement = element;
        }
    }

    /// <summary>
    /// Opens a dialog to edit the text of a speech bubble element.
    /// </summary>
    /// <param name="drawableSpeechBubble">The speech bubble element to be edited.</param>
    /// <param name="location">The location on the canvas where the editing is initiated.</param>
    private void EditSpeechBubble(DrawableSpeechBubble drawableSpeechBubble, Point location)
    {
        var dialog = new TextEditingDialog(drawableSpeechBubble.Text);
        if (dialog.ShowDialog() == true)
        {
            drawableSpeechBubble.Text = dialog.EditedText;
            drawableCanvas.InvalidateVisual(); // Refresh the canvas to show updated text
        }
    }

    /// <summary>
    /// Opens a dialog to edit the text of a drawable text element.
    /// </summary>
    /// <param name="drawableText">The text element to be edited.</param>
    /// <param name="location">The location on the canvas where the editing is initiated.</param>
    private void EditTextBox(DrawableText drawableText, Point location)
    {
        var dialog = new TextEditingDialog(drawableText.Text);
        if (dialog.ShowDialog() == true)
        {
            drawableText.Text = dialog.EditedText;
        }
    }
}

/// <summary>
/// A dialog window used for editing text elements on the canvas.
/// </summary>
public partial class TextEditingDialog : Window
{
    /// <summary>
    /// Gets the edited text after the dialog is closed.
    /// </summary>
    public string EditedText { get; private set; }

    /// <summary>
    /// Initializes a new instance of the TextEditingDialog class with the initial text to edit.
    /// </summary>
    /// <param name="initialText">The initial text to be displayed in the editing dialog.</param>
    public TextEditingDialog(string initialText)
    {
        var textBox = new TextBox
        {
            Text = initialText,
            Margin = new Thickness(10),
            AcceptsReturn = true,
            AcceptsTab = true,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            MaxWidth = 300,
            MaxHeight = 200
        };

        var okButton = new Button
        {
            Content = "OK",
            Width = 80,
            Height = 30,
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Right
        };
        okButton.Click += (s, e) =>
        {
            EditedText = textBox.Text;
            this.DialogResult = true;
        };

        var stackPanel = new StackPanel();
        stackPanel.Children.Add(textBox);
        stackPanel.Children.Add(okButton);

        this.Content = stackPanel;
        this.SizeToContent = SizeToContent.WidthAndHeight;
        this.ResizeMode = ResizeMode.NoResize;
    }
}