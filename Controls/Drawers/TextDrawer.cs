using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using screenerWpf.Controls;
using screenerWpf.Models.DrawableElements;

namespace screenerWpf.CanvasHandler.Drawers
{
    public class TextDrawer : DrawableElementDrawer
    {
        private TextBox editableTextBox;

        public TextDrawer(DrawableCanvas canvas) : base(canvas)
        {

        }

        public override void StartDrawing(MouseButtonEventArgs e)
        {
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
            DrawableCanvas.Children.Add(editableTextBox);
            editableTextBox.Focus();

            editableTextBox.LostFocus += EditableTextBox_LostFocus;
        }

        private void EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
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
            editableTextBox = null; // Clear the temporary TextBox
        }

        public override void UpdateDrawing(MouseEventArgs e)
        {

        }

        public override void FinishDrawing()
        {
        }
    }
}
