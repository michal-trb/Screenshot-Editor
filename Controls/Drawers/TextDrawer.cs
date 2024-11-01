namespace screenerWpf.CanvasHandler.Drawers;

using screenerWpf.Controls;
using screenerWpf.Models.DrawableElements;
using screenerWpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

/// <summary>
/// Represents a drawer for creating and adding text elements to the canvas.
/// </summary>
public class TextDrawer : DrawableElementDrawer
{
    /// <summary>
    /// The TextBox that allows users to input text onto the canvas.
    /// </summary>
    private TextBox editableTextBox;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextDrawer"/> class.
    /// </summary>
    /// <param name="canvas">The canvas on which the text will be drawn.</param>
    public TextDrawer(DrawableCanvas canvas) : base(canvas)
    {

    }

    /// <summary>
    /// Starts the drawing process by allowing the user to place a text box onto the canvas.
    /// </summary>
    /// <param name="e">Mouse button event data that provides the location where drawing should begin.</param>
    public override void StartDrawing(MouseButtonEventArgs e)
    {
        // Clean up any existing textbox
        if (editableTextBox != null)
        {
            editableTextBox.LostFocus -= EditableTextBox_LostFocus;
            DrawableCanvas.Children.Remove(editableTextBox);
            editableTextBox = null;
            return; // Exit if we're cleaning up existing textbox
        }

        Point location = e.GetPosition(DrawableCanvas);

        editableTextBox = new TextBox
        {
            Width = 200,
            Height = 30,
            Text = "New Text",
            FontFamily = CanvasInputHandler.GetCurrentFontFamily(),
            FontSize = CanvasInputHandler.GetCurrentFontSize(),
            Foreground = new SolidColorBrush(CanvasInputHandler.GetCurrentColor()),
            Background = new SolidColorBrush(Colors.Transparent),
            BorderThickness = new Thickness(0),
            AcceptsReturn = true,
            AcceptsTab = true
        };

        Canvas.SetLeft(editableTextBox, location.X);
        Canvas.SetTop(editableTextBox, location.Y);

        // Add event handlers
        editableTextBox.LostFocus += EditableTextBox_LostFocus;
        DrawableCanvas.MouseDown += Canvas_MouseDown;

        DrawableCanvas.Children.Add(editableTextBox);
        editableTextBox.Focus();
        editableTextBox.SelectAll();
    }

    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (editableTextBox != null)
        {
            Point clickPoint = e.GetPosition(DrawableCanvas);
            Point textBoxPoint = new Point(Canvas.GetLeft(editableTextBox), Canvas.GetTop(editableTextBox));
            Rect textBoxBounds = new Rect(textBoxPoint, new Size(editableTextBox.Width, editableTextBox.Height));

            if (!textBoxBounds.Contains(clickPoint))
            {
                // Click was outside the textbox
                DrawableCanvas.MouseDown -= Canvas_MouseDown;
                EditableTextBox_LostFocus(editableTextBox, new RoutedEventArgs());
            }
        }
    }

    /// <summary>
    /// Handles the LostFocus event of the text box, finalizing the input and adding it as a drawable text element.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (editableTextBox == null) return;

        TextBox textBox = (TextBox)sender;

        // Remove event handlers
        textBox.LostFocus -= EditableTextBox_LostFocus;
        DrawableCanvas.MouseDown -= Canvas_MouseDown;

        Point textLocation = new Point(Canvas.GetLeft(textBox), Canvas.GetTop(textBox));

        var drawableText = new DrawableText
        {
            Position = textLocation,
            Text = textBox.Text,
            Typeface = CanvasInputHandler.GetCurrentTypeface(),
            FontSize = CanvasInputHandler.GetCurrentFontSize(),
            Color = CanvasInputHandler.GetCurrentColor(),
        };

        DrawableCanvas.AddElement(drawableText);
        DrawableCanvas.Children.Remove(textBox);
        editableTextBox = null;
        DrawableCanvas.InvalidateVisual();


        // Reset UI state to selection mode
        if (DrawableCanvas.DataContext is ImageEditorViewModel viewModel)
        {
            viewModel.ResetToSelectionMode();
        }

    }

    /// <summary>
    /// Updates the drawing process (not used in this implementation).
    /// </summary>
    /// <param name="e">Mouse event data that provides the current position of the mouse.</param>
    public override void UpdateDrawing(MouseEventArgs e)
    {
        // No implementation needed for updating text drawing in this context
    }

    /// <summary>
    /// Completes the drawing process for the text element.
    /// </summary>
    public override void FinishDrawing()
    {
        // No specific implementation needed for finishing the drawing in this context
    }
}