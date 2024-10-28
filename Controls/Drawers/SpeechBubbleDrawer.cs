namespace screenerWpf.CanvasHandler.Drawers;

using screenerWpf.Controls;
using screenerWpf.Models.DrawableElements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

/// <summary>
/// Represents a drawer for creating speech bubble elements on the canvas.
/// </summary>
public class SpeechBubbleDrawer : DrawableElementDrawer
{
    /// <summary>
    /// Holds the current speech bubble being drawn.
    /// </summary>
    private DrawableSpeechBubble CurrentSpeechBubble { get; set; }

    /// <summary>
    /// The text box used for editing the text inside the speech bubble.
    /// </summary>
    private TextBox editableTextBox { get; set; }

    /// <summary>
    /// Gets or sets the initial text of the speech bubble.
    /// </summary>
    public string InitialText { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechBubbleDrawer"/> class.
    /// </summary>
    /// <param name="canvas">The canvas on which the speech bubble will be drawn.</param>
    /// <param name="initialText">The initial text for the speech bubble. Defaults to an empty string.</param>
    public SpeechBubbleDrawer(DrawableCanvas canvas, string initialText = "") : base(canvas)
    {
        InitialText = initialText;
    }

    /// <summary>
    /// Starts the drawing process for the speech bubble.
    /// </summary>
    /// <param name="e">Mouse button event data that provides the location where drawing should begin.</param>
    public override void StartDrawing(MouseButtonEventArgs e)
    {        // Clean up any existing textbox
        if (editableTextBox != null)
        {
            editableTextBox.LostFocus -= EditableTextBox_LostFocus;
            DrawableCanvas.Children.Remove(editableTextBox);
            editableTextBox = null;
            return; // Exit if we're cleaning up existing textbox
        }

        Point location = e.GetPosition(DrawableCanvas);
        CurrentSpeechBubble = new DrawableSpeechBubble
        {
            Position = location,
            Size = new Size(100, 50),
            Text = InitialText,
            Color = CanvasInputHandler.GetCurrentColor(),
            FontSize = CanvasInputHandler.GetCurrentFontSize(),
            Typeface = CanvasInputHandler.GetCurrentTypeface(),
            EndTailPoint = new Point(location.X - 15, location.Y + 80),
            Brush = new SolidColorBrush(CanvasInputHandler.GetCurrentColor())
        };
        DrawableCanvas.AddElement(CurrentSpeechBubble);

        CreateEditableTextBox(location, CurrentSpeechBubble);

        // Add canvas mouse down handler
        DrawableCanvas.MouseDown += Canvas_MouseDown;
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
                FinishTextEditingInSpeechBubble(editableTextBox, CurrentSpeechBubble);
            }
        }
    }

    /// <summary>
    /// Creates an editable text box within the speech bubble.
    /// </summary>
    /// <param name="location">The location where the text box will be placed.</param>
    /// <param name="speechBubble">The speech bubble element that the text box is linked to.</param>
    private void CreateEditableTextBox(Point location, DrawableSpeechBubble speechBubble)
    {
        editableTextBox = new TextBox
        {
            Width = speechBubble.Size.Width - 20,
            Height = speechBubble.Size.Height - 20,
            Text = speechBubble.Text,
            FontFamily = CanvasInputHandler.GetCurrentFontFamily(),
            FontSize = CanvasInputHandler.GetCurrentFontSize(),
            Foreground = new SolidColorBrush(Colors.Transparent),
            Background = new SolidColorBrush(Colors.Transparent),
            BorderThickness = new Thickness(0),
            AcceptsReturn = true,
            AcceptsTab = true
        };

        Canvas.SetLeft(editableTextBox, location.X + 10);
        Canvas.SetTop(editableTextBox, location.Y + 10);
        DrawableCanvas.Children.Add(editableTextBox);
        editableTextBox.Focus();

        editableTextBox.LostFocus += (sender, e) => FinishTextEditingInSpeechBubble(sender as TextBox, speechBubble);
        editableTextBox.TextChanged += (sender, e) => TextChangedInSpeechBubble(sender as TextBox, speechBubble);
    }

    /// <summary>
    /// Finalizes text editing in the speech bubble, removing the text box.
    /// </summary>
    /// <param name="textBox">The text box being used for editing.</param>
    /// <param name="speechBubble">The speech bubble that contains the text.</param>
    private void FinishTextEditingInSpeechBubble(TextBox textBox, DrawableSpeechBubble speechBubble)
    {
        if (textBox == null) return;

        speechBubble.Text = textBox.Text;

        // Remove event handlers
        textBox.LostFocus -= (sender, e) => FinishTextEditingInSpeechBubble(sender as TextBox, speechBubble);
        DrawableCanvas.MouseDown -= Canvas_MouseDown;

        DrawableCanvas.Children.Remove(textBox);
        editableTextBox = null;
        DrawableCanvas.InvalidateVisual();
    }

    /// <summary>
    /// Updates the speech bubble text and adjusts its size as the text changes.
    /// </summary>
    /// <param name="textBox">The text box being edited.</param>
    /// <param name="speechBubble">The speech bubble containing the text box.</param>
    private void TextChangedInSpeechBubble(TextBox textBox, DrawableSpeechBubble speechBubble)
    {
        if (textBox == null)
            return;

        speechBubble.Text = textBox.Text; // Update the speech bubble's text
        double fontSize = textBox.FontSize;

        FormattedText formattedText = new FormattedText(
            textBox.Text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
            fontSize,
            new SolidColorBrush(CanvasInputHandler.GetCurrentColor()));

        // Adjust speech bubble size to fit the text
        double minWidth = 100.0;
        double minHeight = 50.0;
        double bubbleWidth = Math.Max(minWidth, formattedText.WidthIncludingTrailingWhitespace + 20);
        double bubbleHeight = Math.Max(minHeight, formattedText.Height + 20);

        // Update speech bubble size
        speechBubble.Size = new Size(bubbleWidth, bubbleHeight);

        // Update TextBox size and position
        textBox.Width = bubbleWidth - 20;
        textBox.Height = bubbleHeight - 20;
        Canvas.SetLeft(textBox, speechBubble.Position.X + 10);
        Canvas.SetTop(textBox, speechBubble.Position.Y + 10);

        DrawableCanvas.InvalidateVisual(); // Redraw the canvas to reflect updated bubble size
    }

    /// <summary>
    /// Updates the drawing process (not used in this implementation).
    /// </summary>
    /// <param name="e">Mouse event data that provides the current position of the mouse.</param>
    public override void UpdateDrawing(MouseEventArgs e)
    {
        // No implementation needed for speech bubbles in this context
    }

    /// <summary>
    /// Completes the drawing process for the speech bubble.
    /// </summary>
    public override void FinishDrawing()
    {
        CurrentSpeechBubble = null;
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
    }
}