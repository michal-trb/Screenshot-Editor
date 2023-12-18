using System;
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

        public static FontFamily SelectedFontFamily { get; private set; } = new FontFamily("Arial");
        public static double SelectedFontSize { get; private set; } = 12.0;
        public static double ArrowThickness { get; private set; } = 2.0;
        public static Color SelectedColor { get; private set; } = Colors.Black;

        public CanvasInputHandler(
            DrawableCanvas canvas)
        {
            drawableCanvas = canvas;

            actionHandler = new CanvasActionHandler(
                canvas);
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

        internal void DrawRectButton_Click(object sender, RoutedEventArgs e)
        {
            actionHandler.SetCurrentAction(EditAction.DrawRectangle);
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

        public void BlurButton_Click(object sender, RoutedEventArgs e)
        {
            actionHandler.SetCurrentAction(EditAction.DrawBlur);
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
            SelectedFontFamily = selectedFontFamily;
        }

        public void FontSizeComboBox_SelectionChanged(double fontSize)
        {
            SelectedFontSize = fontSize;
        }

        public void ColorComboBox_SelectionChanged(Color comboBox)
        {
            SelectedColor = comboBox;
        }

        public void ArrowThicknessComboBox_SelectionChanged(double comboBoxArrowThickness)
        {
            ArrowThickness = comboBoxArrowThickness;
        }

        public static Color GetCurrentColor()
        {
            return SelectedColor;
        }

        public static Typeface GetCurrentTypeface()
        {
            // Zwróć aktualny rodzaj czcionki
            return new Typeface(SelectedFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        }

        public static FontFamily GetCurrentFontFamily()
        {
            // Zwróć aktualny rodzaj czcionki
            return SelectedFontFamily;
        }

        public static double GetCurrentFontSize()
        {
            // Zwróć aktualny rozmiar czcionki
            return SelectedFontSize;
        }

        public static double GetCurrentThickness()
        {
            return ArrowThickness;
        }
    }
}
