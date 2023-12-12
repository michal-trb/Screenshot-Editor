using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf
{
    public class CanvasEditingHandler
    {
        private DrawableCanvas drawableCanvas;
        private TextBox editableTextBox;
        private IDrawable editableElement;

        public CanvasEditingHandler(DrawableCanvas canvas)
        {
            drawableCanvas = canvas;
        }

        public void StartEditing(IDrawable element, Point location)
        {
            if (element is DrawableText drawableText)
            {
                CreateEditableTextBox(drawableText, location);
                editableElement = element;
            }
            else if (element is DrawableSpeechBubble speechBubble)
            {
                CreateEditableTextBoxInSpeechBubble(speechBubble, location);
                editableElement = element;
            }
            // Możliwe inne przypadki edycji dla różnych typów elementów
        }

        private void CreateEditableTextBox(DrawableText drawableText, Point location)
        {
            // Tworzenie TextBox dla edycji tekstu
            editableTextBox = new TextBox
            {
                Text = drawableText.Text,
                FontFamily = drawableText.Typeface.FontFamily,
                FontSize = drawableText.FontSize,
                Foreground = new SolidColorBrush(drawableText.Color),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                AcceptsReturn = true,
                AcceptsTab = true
            };

            PositionEditableTextBox(location);
            AttachTextBoxEvents();
        }

        private void CreateEditableTextBoxInSpeechBubble(DrawableSpeechBubble speechBubble, Point location)
        {
            // Analogicznie jak wyżej, ale dostosowane do dymku mowy
            // ...
        }

        private void PositionEditableTextBox(Point location)
        {
            Canvas.SetLeft(editableTextBox, location.X);
            Canvas.SetTop(editableTextBox, location.Y);
            drawableCanvas.Children.Add(editableTextBox);
            editableTextBox.Focus();
        }

        private void AttachTextBoxEvents()
        {
            editableTextBox.LostFocus += EditableTextBox_LostFocus;
            editableTextBox.KeyDown += EditableTextBox_KeyDown;
        }

        private void EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            FinishEditing(sender as TextBox);
        }

        private void EditableTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FinishEditing(sender as TextBox);
            }
        }

        private void FinishEditing(TextBox textBox)
        {
            if (textBox == null) return;

            if (editableElement is DrawableText drawableText)
            {
                drawableText.Text = textBox.Text;
            }
            else if (editableElement is DrawableSpeechBubble speechBubble)
            {
                speechBubble.Text = textBox.Text;
                // Możliwe dodatkowe aktualizacje dla dymków mowy
            }

            // Usuwanie TextBox i odświeżanie płótna
            drawableCanvas.Children.Remove(textBox);
            drawableCanvas.InvalidateVisual();
            editableTextBox = null;
            editableElement = null;
        }

        // Dodatkowe metody...
    }
}
