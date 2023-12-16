using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf.CanvasHandler.Drawers
{
    public class TextDrawer : DrawableElementDrawer
    {
        private TextBox editableTextBox;
        public Color Color { get; set; }
        public Typeface Typeface { get; set; }
        public double FontSize { get; set; }

        public TextDrawer(DrawableCanvas canvas, Color color, Typeface typeface, double fontSize) : base(canvas)
        {
            Color = color;
            Typeface = typeface;
            FontSize = fontSize;
        }

        public override void StartDrawing(MouseButtonEventArgs e)
        {
            Point location = e.GetPosition(DrawableCanvas);

            // Create a TextBox for user to enter text
            editableTextBox = new TextBox
            {
                Width = 200, // Adjust as needed
                Height = 30, // Adjust as needed
                Text = "New Text",
                FontFamily = Typeface.FontFamily,
                FontSize = FontSize,
                Foreground = new SolidColorBrush(Color),
                Background = new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Thickness(0),
                AcceptsReturn = true,
                AcceptsTab = true
            };

            Canvas.SetLeft(editableTextBox, location.X);
            Canvas.SetTop(editableTextBox, location.Y);
            DrawableCanvas.Children.Add(editableTextBox);
            editableTextBox.Focus();

            // Event when focus is lost
            editableTextBox.LostFocus += EditableTextBox_LostFocus;
        }

        private void EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Point textLocation = new Point(Canvas.GetLeft(textBox), Canvas.GetTop(textBox));

            // Create a DrawableText from the input
            var drawableText = new DrawableText
            {
                Position = textLocation,
                Text = textBox.Text,
                Typeface = new Typeface(
                    textBox.FontFamily,
                    textBox.FontStyle,
                    textBox.FontWeight,
                    textBox.FontStretch),
                FontSize = textBox.FontSize,
                Color = Color
            };

            // Add DrawableText to the list of drawable elements and remove the TextBox
            DrawableCanvas.AddElement(drawableText);
            DrawableCanvas.Children.Remove(textBox);
            editableTextBox = null; // Clear the temporary TextBox
        }

        public override void UpdateDrawing(MouseEventArgs e)
        {
            // This method can be used to update text dynamically, if needed
        }

        public override void FinishDrawing()
        {
            // Finish drawing logic, if needed
        }
    }
}
