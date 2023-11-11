using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace screenerWpf
{
    public class CanvasInputHandler 
    {
        public ObservableCollection<IDrawable> Elements { get; set; } = new ObservableCollection<IDrawable>();
        private DrawableCanvas drawableCanvas;
        private DrawableArrow currentArrow; // Used to track the arrow being drawn
        private TextBox editableTextBox;
        private enum EditAction { None, DrawArrow, AddText, Move, Delete }
        private EditAction currentAction = EditAction.None;
        private Color arrowColor;
        private double arrowThickness;
        private ComboBox arrowColorComboBox;
        private ComboBox arrowThicknessComboBox;


        public CanvasInputHandler(
            DrawableCanvas canvas,
            ComboBox colorComboBox,
            ComboBox thicknessComboBox)
        {
            this.drawableCanvas = canvas;
            this.arrowColorComboBox = colorComboBox;
            this.arrowThicknessComboBox = thicknessComboBox;

            arrowColor = Colors.Black;
            arrowThickness = 2.0;

            // Ustaw początkowy wybór dla rozwijanych list

        }

        // Metody obsługujące zdarzenia myszy...
        public void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentAction == EditAction.DrawArrow)
            {
                // Rozpoczynamy rysowanie strzałki
                Point startPoint = e.GetPosition(drawableCanvas);
                currentArrow = new DrawableArrow
                {
                    Position = startPoint,
                    Color = arrowColor, // Użyj wybranego koloru
                    Thickness = arrowThickness // Użyj wybranej grubości
                };
                drawableCanvas.AddElement(currentArrow);
            }
            else if (currentAction == EditAction.AddText)
            {
                // Start adding text
                Point location = e.GetPosition(drawableCanvas);
                CreateEditableText(location);
            }
            else
            {
                // Implement the logic for selecting an element if clicked on
                drawableCanvas.SelectElementAtPoint(e.GetPosition(drawableCanvas));
            }
        }


        public void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (currentAction == EditAction.DrawArrow && currentArrow != null)
            {
                // Kończymy rysowanie strzałki
                Point endPoint = e.GetPosition(drawableCanvas);
                currentArrow.EndPoint = endPoint; // Ustalamy końcowy punkt strzałki
                currentArrow = null; // Zakończenie rysowania strzałki
                drawableCanvas.InvalidateVisual(); // Przerysowanie canvas, aby utrwalić strzałkę
            }
            if (currentAction != EditAction.AddText)
            {
                currentAction = EditAction.None;
            }

            currentAction = EditAction.None;
        }

        public void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentAction == EditAction.DrawArrow
                && currentArrow != null
                && e.LeftButton == MouseButtonState.Pressed)
            {
                // Aktualizujemy końcowy punkt strzałki
                Point currentPoint = e.GetPosition(drawableCanvas);
                currentArrow.EndPoint = currentPoint; // Załóżmy, że DrawableArrow ma teraz właściwość EndPoint
                drawableCanvas.InvalidateVisual(); // Przerysowanie canvas, żeby pokazać tymczasową strzałkę z grotem
            }
        }

        // Metody dla przycisków i innych akcji...
        public void DrawArrowButton_Click(object sender, RoutedEventArgs e)
        {
            currentAction = EditAction.DrawArrow;
            if (arrowColorComboBox.SelectedItem is ComboBoxItem selectedColorItem
                && selectedColorItem.Background is SolidColorBrush colorBrush)
            {
                arrowColor = colorBrush.Color;
            }
            else
            {
                arrowColor = Colors.Black;
            }

            if (arrowThicknessComboBox.SelectedItem is ComboBoxItem selectedThicknessItem
               && double.TryParse(selectedThicknessItem.Content.ToString(), out double selectedThickness))
            {
                arrowThickness = selectedThickness;
            }
            else
            {
                arrowThickness = 2.0;
            }
     
        }
        public void ArrowThicknessComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (arrowThicknessComboBox.SelectedItem is ComboBoxItem selectedThicknessItem
                && double.TryParse(selectedThicknessItem.Content.ToString(), out double selectedThickness))
            {
                arrowThickness = selectedThickness;
            }
        }

        public void ArrowColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (arrowColorComboBox.SelectedItem is ComboBoxItem selectedColorItem
                && selectedColorItem.Background is SolidColorBrush colorBrush)
            {
                arrowColor = colorBrush.Color;
            }
        }

        public void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            currentAction = EditAction.AddText;
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming 'editedImage' is your WriteableBitmap that holds the drawn content.
            WriteableBitmap editedImage = this.GetEditedImageBitmap(); // You'll need to implement this method.

            // Configure save file dialog box
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Image"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|BMP Files (*.bmp)|*.bmp"; // Filter files by extension

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save image
                using (FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder;
                    switch (Path.GetExtension(dlg.FileName).ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                            encoder = new JpegBitmapEncoder();
                            break;
                        case ".bmp":
                            encoder = new BmpBitmapEncoder();
                            break;
                        default:
                            encoder = new PngBitmapEncoder();
                            break;
                    }

                    encoder.Frames.Add(BitmapFrame.Create(editedImage));
                    encoder.Save(fileStream);
                }
            }
        }

        private WriteableBitmap GetEditedImageBitmap()
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)drawableCanvas.ActualWidth,
                (int)drawableCanvas.ActualHeight,
                96d,
                96d,
                PixelFormats.Pbgra32);

            // Render the canvas with all its children (which should include your drawings and text)
            renderBitmap.Render(drawableCanvas);

            // Convert to WriteableBitmap if needed (if not already a WriteableBitmap)
            WriteableBitmap editableBitmap = new WriteableBitmap(renderBitmap);

            return editableBitmap;
        }

        private void FinishTextEditing(TextBox textBox)
        {
            if (textBox == null || !drawableCanvas.Children.Contains(textBox))
                return;

            Point textLocation = new Point(Canvas.GetLeft(textBox), Canvas.GetTop(textBox));

            // ... istniejący kod do tworzenia DrawableText

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
            Elements.Add(drawableText);
            drawableCanvas.Children.Remove(textBox);

            // Dodatkowo zmusza canvas do uzyskania fokusu
            drawableCanvas.Focus();

            // Clear the temporary TextBox reference as the editing is finished
            editableTextBox = null;
        }

        private void EditableTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Jeśli naciśnięto Enter, kończymy edycję
                FinishTextEditing(sender as TextBox);
            }
        }

        public void CommandBinding_DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            drawableCanvas.DeleteSelectedElement();
        }

        private void CreateEditableText(Point location)
        {
            // Create a TextBox for user to enter textv
            editableTextBox = new TextBox
            {
                Width = 100, // Assume a default width; you might want to adjust this
                Height = 20, // Assume a default height; you might want to adjust this
                Text = "New Text",
                Foreground = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Thickness(0), // Hide the border for now
                AcceptsReturn = false, // Set as needed
                AcceptsTab = false, // Set as needed
            };
            editableTextBox.KeyDown += EditableTextBox_KeyDown;

            Canvas.SetLeft(editableTextBox, location.X);
            Canvas.SetTop(editableTextBox, location.Y);
            drawableCanvas.Children.Add(editableTextBox);

            editableTextBox.Focus(); // Immediately allow typing
            editableTextBox.LostFocus += EditableTextBox_LostFocus; // Event when focus is lost
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
    }
}
