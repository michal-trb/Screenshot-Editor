using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;

namespace screenerWpf.Controls
{
    public class CanvasEditingHandler : ICanvasEditingHandler
    {
        private DrawableCanvas drawableCanvas;
        private IDrawable editableElement;

        public CanvasEditingHandler(DrawableCanvas canvas)
        {
            drawableCanvas = canvas;
        }

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

        private void EditSpeechBubble(DrawableSpeechBubble drawableSpeechBubble, Point location)
        {
            var dialog = new TextEditingDialog(drawableSpeechBubble.Text);
            if (dialog.ShowDialog() == true)
            {
                drawableSpeechBubble.Text = dialog.EditedText;
                drawableCanvas.InvalidateVisual(); // Odświeżanie canvas, aby pokazać zaktualizowany tekst
            }
        }


        private void EditTextBox(DrawableText drawableText, Point location)
        {
            var dialog = new TextEditingDialog(drawableText.Text);
            if (dialog.ShowDialog() == true)
            {
                drawableText.Text = dialog.EditedText;
            }
        }

        private void EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && editableElement is DrawableText drawableText)
            {
                // Aktualizacja tekstu w DrawableText
                drawableText.Text = textBox.Text;

                // Usunięcie TextBox z płótna i odświeżenie
                drawableCanvas.Children.Remove(textBox);
                drawableCanvas.InvalidateVisual();

                // Wyczyszczenie referencji
                editableTextBox = null;
                editableElement = null;
            }
        }
    }
}
public partial class TextEditingDialog : Window
{
    public string EditedText { get; private set; }

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