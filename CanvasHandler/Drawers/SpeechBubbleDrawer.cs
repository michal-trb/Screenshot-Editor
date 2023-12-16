using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf.CanvasHandler.Drawers
{
    public class SpeechBubbleDrawer : DrawableElementDrawer
    {
        private DrawableSpeechBubble CurrentSpeechBubble { get; set; }
        private TextBox EditableTextBox { get; set; }
        public string InitialText { get; set; }

        public SpeechBubbleDrawer(DrawableCanvas canvas, string initialText = "") : base(canvas)
        {
            InitialText = initialText;
        }

        public override void StartDrawing(MouseButtonEventArgs e)
        {
            Point location = e.GetPosition(DrawableCanvas);
            CurrentSpeechBubble = new DrawableSpeechBubble
            {
                Position = location,
                Size = new Size(100, 50),
                Text = InitialText,
                EndPoint = new Point(location.X + 50, location.Y + 50),
                Color = CanvasInputHandler.GetCurrentColor(),
            };
            DrawableCanvas.AddElement(CurrentSpeechBubble);

            CreateEditableTextBox(location, CurrentSpeechBubble);
        }

        private void CreateEditableTextBox(Point location, DrawableSpeechBubble speechBubble)
        {
            EditableTextBox = new TextBox
            {
                Width = speechBubble.Size.Width - 20,
                Height = speechBubble.Size.Height - 20,
                Text = speechBubble.Text,
                FontFamily = CanvasInputHandler.GetCurrentFontFamily(),
                FontSize = CanvasInputHandler.GetCurrentFontSize(),
                Foreground = new SolidColorBrush(CanvasInputHandler.GetCurrentColor()),
                Background = new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Thickness(0),
                AcceptsReturn = true,
                AcceptsTab = true
            };

            Canvas.SetLeft(EditableTextBox, location.X + 10);
            Canvas.SetTop(EditableTextBox, location.Y + 10);
            DrawableCanvas.Children.Add(EditableTextBox);
            EditableTextBox.Focus();

            EditableTextBox.LostFocus += (sender, e) => FinishTextEditingInSpeechBubble(sender as TextBox, speechBubble);
            EditableTextBox.TextChanged += (sender, e) => TextChangedInSpeechBubble(sender as TextBox, speechBubble);
        }

        private void FinishTextEditingInSpeechBubble(TextBox textBox, DrawableSpeechBubble speechBubble)
        {
            if (textBox == null)
                return;

            speechBubble.Text = textBox.Text; // Aktualizacja tekstu w dymku
                                              // Usuwanie TextBox tylko jeśli edycja została zakończona
            if (!textBox.IsKeyboardFocusWithin)
            {
                DrawableCanvas.Children.Remove(textBox);
                EditableTextBox = null;
            }
            DrawableCanvas.InvalidateVisual(); // Przerysowanie canvas, aby zaktualizować tekst w dymku
        }

        private void TextChangedInSpeechBubble(TextBox textBox, DrawableSpeechBubble speechBubble)
        {
            if (textBox == null)
                return;

            speechBubble.Text = textBox.Text; // Aktualizacja tekstu w dymku

            // Obliczanie wymiarów tekstu
            FormattedText formattedText = new FormattedText(
                textBox.Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                CanvasInputHandler.GetCurrentTypeface(),
                CanvasInputHandler.GetCurrentFontSize(),
                new SolidColorBrush(CanvasInputHandler.GetCurrentColor()));

            // Dostosowanie rozmiaru dymku do tekstu
            double minWidth = 100.0;
            double minHeight = 50.0;
            double bubbleWidth = Math.Max(minWidth, formattedText.WidthIncludingTrailingWhitespace + 20);
            double bubbleHeight = Math.Max(minHeight, formattedText.Height + 20);

            // Aktualizacja rozmiaru dymku
            speechBubble.Size = new System.Windows.Size(bubbleWidth, bubbleHeight);

            // Aktualizacja rozmiaru i pozycji TextBox
            textBox.Width = bubbleWidth - 20;
            textBox.Height = bubbleHeight - 20;
            Canvas.SetLeft(textBox, speechBubble.Position.X + 10); // Aktualizacja pozycji TextBox
            Canvas.SetTop(textBox, speechBubble.Position.Y + 10);

            DrawableCanvas.InvalidateVisual(); // Przerysowanie canvas, aby zaktualizować rozmiar dymku i TextBox
        }

        public override void UpdateDrawing(MouseEventArgs e)
        {
            // Aktualizacja rysowania, jeśli jest potrzebna
        }

        public override void FinishDrawing()
        {
            CurrentSpeechBubble = null;
        }
    }
}
