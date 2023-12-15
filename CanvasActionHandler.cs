using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf
{
    public enum EditAction { None, DrawArrow, AddText, Move, Delete, AddBubble }

    public class CanvasActionHandler
    {
        private DrawableCanvas drawableCanvas;
        private IDrawable currentDrawable;
        private EditAction currentAction = EditAction.None;
        private Color color;
        private double arrowThickness;
        private TextBox editableTextBox;
        private FontFamily selectedFontFamily = new FontFamily("Arial");

        public CanvasActionHandler(DrawableCanvas canvas, Color initialColor, double initialThickness)
        {
            drawableCanvas = canvas;
            color = initialColor;
            arrowThickness = initialThickness;
        }

        public void HandleLeftButtonDown(MouseButtonEventArgs e)
        {
            switch (currentAction)
            {
                case EditAction.DrawArrow:
                    StartDrawingArrow(e);
                    break;
                case EditAction.AddText:
                    StartAddingText(e);
                    break;
                case EditAction.AddBubble:
                    StartAddingSpeechBubble(e);
                    break;
                default:
                    SelectElementAtMousePosition(e);
                    break;
            }
        }
    
        public void HandleLeftButtonUp(MouseButtonEventArgs e)
        {
            if (currentAction == EditAction.DrawArrow && currentDrawable is DrawableArrow arrow)
            {
                FinishDrawingArrow(arrow, e.GetPosition(drawableCanvas));
            }

            currentAction = EditAction.None;
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            if (currentAction == EditAction.DrawArrow && currentDrawable is DrawableArrow arrow)
            {
                UpdateDrawingArrow(arrow, e.GetPosition(drawableCanvas), e.LeftButton == MouseButtonState.Pressed);
            }
        }

        private void StartDrawingArrow(MouseEventArgs e)
        {
            Point location = e.GetPosition(drawableCanvas);

            var arrow = new DrawableArrow
            {
                Position = location,
                Color = color,
                Thickness = arrowThickness
            };
            drawableCanvas.AddElement(arrow);
            currentDrawable = arrow;
        }

        private void UpdateDrawingArrow(DrawableArrow arrow, Point currentPoint, bool isDrawing)
        {
            if (!isDrawing) return;

            arrow.EndPoint = currentPoint;
            drawableCanvas.InvalidateVisual();
        }

        private void FinishDrawingArrow(DrawableArrow arrow, Point endPoint)
        {
            arrow.EndPoint = endPoint;
            drawableCanvas.InvalidateVisual();
            currentDrawable = null;
        }

        private void StartAddingSpeechBubble(MouseEventArgs e)
        {
            Point location = e.GetPosition(drawableCanvas);

            var speechBubble = new DrawableSpeechBubble
            {
                Position = location,
                Size = new Size(100, 50),
                Text = "",
                EndPoint = new Point(location.X - 10, location.Y + 60)
            };

            drawableCanvas.AddElement(speechBubble);
            currentDrawable = speechBubble;
            CreateEditableTextInSpeechBubble(location, speechBubble); // Tworzenie edytowalnego TextBox
        }

        public void SetCurrentAction(EditAction action)
        {
            currentAction = action;
        }

        public void UpdateDrawingColor(Color newColor)
        {
            color = newColor;
        }

        public void UpdateArrowThickness(double newThickness)
        {
            arrowThickness = newThickness;
        }


        private void StartAddingText(MouseButtonEventArgs e)
        {
            Point location = e.GetPosition(drawableCanvas);
            // Create a TextBox for user to enter text
            editableTextBox = new TextBox
            {
                Width = 200, // Adjust as needed
                Height = 30, // Adjust as needed
                Text = "New Text",
                FontFamily = CanvasInputHandler.selectedFontFamily,
                FontSize = CanvasInputHandler.selectedFontSize,
                Foreground = new SolidColorBrush(color),
                Background = new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Thickness(0),
                AcceptsReturn = true,
                AcceptsTab = true
            };
            Canvas.SetLeft(editableTextBox, location.X);
            Canvas.SetTop(editableTextBox, location.Y);
            drawableCanvas.Children.Add(editableTextBox);

            if (editableTextBox != null )
            {
                editableTextBox.Focus(); // Immediately allow typing
                editableTextBox.LostFocus += EditableTextBox_LostFocus; // Event when focus is lost
            }
        }
        private void EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Point textLocation = new Point(Canvas.GetLeft(textBox), Canvas.GetTop(textBox));

            // Create a DrawableText from the input
            DrawableText drawableText = new DrawableText
            {
                Position = textLocation,
                Text = textBox.Text,
                Typeface = new Typeface(
                    textBox.FontFamily,
                    textBox.FontStyle,
                    textBox.FontWeight,
                    textBox.FontStretch),
                FontSize = textBox.FontSize,
                Color = (textBox.Foreground as SolidColorBrush).Color
            };

            // Add DrawableText to the list of drawable elements and remove the TextBox
            drawableCanvas.AddElement(drawableText);
            drawableCanvas.Children.Remove(textBox);
            editableTextBox = null; // Clear the temporary TextBox
        }

        private void SelectElementAtMousePosition(MouseButtonEventArgs e)
        {
            drawableCanvas.SelectElementAtPoint(e.GetPosition(drawableCanvas));
        }

        private void CreateEditableTextInSpeechBubble(Point location, DrawableSpeechBubble speechBubble)
        {
            editableTextBox = new TextBox
            {
                Width = speechBubble.Size.Width - 20, // Pozostawienie marginesów
                Height = speechBubble.Size.Height - 20,
                Text = speechBubble.Text,
                FontFamily = CanvasInputHandler.selectedFontFamily,
                FontSize = CanvasInputHandler.selectedFontSize,
                Foreground = Brushes.Transparent, // Ustawienie przeźroczystego tekstu
                Background = Brushes.Transparent, // Ustawienie przeźroczystego tekstu
                BorderThickness = new Thickness(0),
                AcceptsReturn = true,
                AcceptsTab = true
            };

            // Ustawienie pozycji TextBox tak, aby był wycentrowany w dymku
            Canvas.SetLeft(editableTextBox, location.X + 10); // Dodanie marginesu
            Canvas.SetTop(editableTextBox, location.Y + 10);
            drawableCanvas.Children.Add(editableTextBox);
            editableTextBox.Focus();
            editableTextBox.LostFocus += (sender, e) => FinishTextEditingInSpeechBubble(sender as TextBox, speechBubble);
            editableTextBox.TextChanged += (sender, e) => TextChangedInSpeechBubble(sender as TextBox, speechBubble);
        }

        private void FinishTextEditingInSpeechBubble(TextBox textBox, DrawableSpeechBubble speechBubble)
        {
            if (textBox == null)
                return;

            speechBubble.Text = textBox.Text; // Aktualizacja tekstu w dymku
                                              // Usuwanie TextBox tylko jeśli edycja została zakończona
            if (!textBox.IsKeyboardFocusWithin)
            {
                drawableCanvas.Children.Remove(textBox);
                editableTextBox = null;
            }
            drawableCanvas.InvalidateVisual(); // Przerysowanie canvas, aby zaktualizować tekst w dymku
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
                new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
                textBox.FontSize,
                Brushes.Black);

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

            drawableCanvas.InvalidateVisual(); // Przerysowanie canvas, aby zaktualizować rozmiar dymku i TextBox
        }
    }
}