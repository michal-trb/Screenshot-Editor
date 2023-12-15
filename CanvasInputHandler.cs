using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf
{
    public class CanvasInputHandler
    {
        private DrawableCanvas drawableCanvas;
        private CanvasActionHandler actionHandler;
        private CanvasSelectionHandler selectionHandler;
        private CanvasEditingHandler editingHandler;
        private CanvasSavingHandler savingHandler;

        private ComboBox colorComboBox;
        private ComboBox arrowThicknessComboBox;
        public static FontFamily selectedFontFamily = new FontFamily("Arial");
        public static double selectedFontSize = 12.0;

        public CanvasInputHandler(
            DrawableCanvas canvas,
            ComboBox colorComboBox,
            ComboBox thicknessComboBox)
        {
            drawableCanvas = canvas;
            this.colorComboBox = colorComboBox;
            this.arrowThicknessComboBox = thicknessComboBox;

            Color initialColor = Colors.Black;
            double initialThickness = 2.0;

            actionHandler = new CanvasActionHandler(
                canvas,
                initialColor,
                initialThickness);
            selectionHandler = new CanvasSelectionHandler(drawableCanvas);
            editingHandler = new CanvasEditingHandler(drawableCanvas);
            savingHandler = new CanvasSavingHandler(drawableCanvas);
        }

        // Zdarzenia myszy przekierowane do odpowiednich handlerów
        public void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectionHandler.HandleLeftButtonDown(e);
            actionHandler.HandleLeftButtonDown(e);
        }

        public void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            selectionHandler.HandleLeftButtonUp(e);
            actionHandler.HandleLeftButtonUp(e);
        }

        public void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            selectionHandler.HandleMouseMove(e);
            actionHandler.HandleMouseMove(e);
        }

        // Metody obsługi przycisków i innych akcji
        public void DrawArrowButton_Click(object sender, RoutedEventArgs e)
        {
            actionHandler.SetCurrentAction(EditAction.DrawArrow);
            UpdateDrawingColorAndThickness();
        }

        public void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            actionHandler.SetCurrentAction(EditAction.AddText);
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            savingHandler.SaveCanvasToFile();
        }

        private void UpdateDrawingColorAndThickness()
        {
            // Aktualizacja koloru i grubości strzałki
            // Analogicznie do poprzedniej implementacji
        }

        public void SpeechBubbleButton_Click(object sender, RoutedEventArgs e)
        {
            actionHandler.SetCurrentAction(EditAction.AddBubble);
        }

        public void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (colorComboBox.SelectedItem is ComboBoxItem selectedColorItem
                && selectedColorItem.Background is SolidColorBrush colorBrush)
            {
                UpdateDrawingColor(colorBrush.Color);
            }
        }

        public void ArrowThicknessComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (arrowThicknessComboBox.SelectedItem is ComboBoxItem selectedThicknessItem
                && double.TryParse(selectedThicknessItem.Content.ToString(), out double selectedThickness))
            {
                UpdateArrowThickness(selectedThickness);
            }
        }

        private void UpdateDrawingColor(Color newColor)
        {
            actionHandler.UpdateDrawingColor(newColor);
        }

        private void UpdateArrowThickness(double newThickness)
        {
            actionHandler.UpdateArrowThickness(newThickness);
        }

        public void EditTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectionHandler.HasSelectedElement())
            {
                var selectedElement = selectionHandler.GetSelectedElement();
                if (selectedElement != null)
                {
                    // Zakładamy, że IDrawable ma właściwość, która zwraca jego lokalizację
                    Point location = selectedElement.GetLocation();
                    editingHandler.StartEditing(selectedElement, location);
                }
            }
        }


        public void CommandBinding_DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            selectionHandler.DeleteSelectedElement();
        }

        public void FontFamilyComboBox_SelectionChanged(FontFamily selectedFontFamily)
        {
            selectedFontFamily = selectedFontFamily;
        }

        public void FontSizeComboBox_SelectionChanged(double fontSize)
        {
            selectedFontSize = fontSize;
        }
    }
}
