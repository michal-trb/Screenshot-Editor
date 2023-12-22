using screenerWpf.Sevices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf.Controls
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
        public static double Transparency { get; private set; } = 0;
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

        public void Canvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                actionHandler.HandleCopy();
                e.Handled = true;
            }
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                actionHandler.HandlePaste();
                e.Handled = true;
            }
        }

        // Metody obsługi przycisków i innych akcji
        public void DrawArrow()
        {
            actionHandler.SetCurrentAction(EditAction.DrawArrow);
            UpdateDrawingColorAndThickness();
        }

        internal void DrawRect()
        {
            actionHandler.SetCurrentAction(EditAction.DrawRectangle);
            UpdateDrawingColorAndThickness();
        }

        public void AddText()
        {
            actionHandler.SetCurrentAction(EditAction.AddText);
        }

        public void Save()
        {
            savingHandler.SaveCanvasToFile();
        }

        private void UpdateDrawingColorAndThickness()
        {
            // Aktualizacja koloru i grubości strzałki
            // Analogicznie do poprzedniej implementacji
        }

        public void SpeechBubble()
        {
            actionHandler.SetCurrentAction(EditAction.AddBubble);
        }

        public void Blur()
        {
            actionHandler.SetCurrentAction(EditAction.DrawBlur);
        }

        public void Brush()
        {
            actionHandler.SetCurrentAction(EditAction.BrushPainting);
        }

        public void RecognizeText()
        {
            var textRecognitionHandler = new TextRecognitionHandler(drawableCanvas);
            textRecognitionHandler.StartRecognizeFromImage();
        }
        public void EditText()
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

        public void ChangeFontFamily(FontFamily selectedFontFamily)
        {
            SelectedFontFamily = selectedFontFamily;
        }

        public void ChangeFontSize(double fontSize)
        {
            SelectedFontSize = fontSize;
        }

        public void ChangeColor(Color comboBox)
        {
            SelectedColor = comboBox;
        }

        public void ChangeArrowThickness(double comboBoxArrowThickness)
        {
            ArrowThickness = comboBoxArrowThickness;
        }

        internal void ChangeTransparency(double transparency)
        {
            Transparency = transparency;
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

        public static double GetCurrentTransparency()
        {
            return Transparency;
        }
    }
}
