using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf
{
    public class CanvasSelectionHandler
    {
        private DrawableCanvas drawableCanvas;
        private IDrawable selectedElement;
        private Point lastMousePosition;

        public CanvasSelectionHandler(DrawableCanvas canvas)
        {
            drawableCanvas = canvas;
        }

        public void HandleLeftButtonDown(MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(drawableCanvas);
            SelectElementAt(clickPosition);
            lastMousePosition = clickPosition;
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && selectedElement != null)
            {
                Point currentMousePosition = e.GetPosition(drawableCanvas);
                Vector delta = currentMousePosition - lastMousePosition;
                MoveSelectedElement(delta);
                lastMousePosition = currentMousePosition;
            }
        }

        public void HandleLeftButtonUp(MouseButtonEventArgs e)
        {
            // Zakończ przesuwanie elementu lub zakończ selekcję, jeśli to konieczne
            lastMousePosition = new Point();
        }

        private void SelectElementAt(Point position)
        {
            IDrawable element = drawableCanvas.FindElementAt(position);
            if (element != null)
            {
                selectedElement = element;
            }
            else
            {
                DeselectCurrentElement();
            }
        }

        private void MoveSelectedElement(Vector delta)
        {
            if (selectedElement == null) return;

            selectedElement.Move(delta);
            drawableCanvas.InvalidateVisual(); // Odświeżenie płótna
        }

        private void DeselectCurrentElement()
        {
            selectedElement = null;
        }

        public void DeleteSelectedElement()
        {
            if (selectedElement != null)
            {
                drawableCanvas.RemoveElement(selectedElement);
                DeselectCurrentElement();
                drawableCanvas.InvalidateVisual(); // Odświeżenie płótna
            }
        }

        // Metoda sprawdzająca, czy jakikolwiek element jest zaznaczony
        public bool HasSelectedElement()
        {
            return selectedElement != null;
        }

        // Metoda zwracająca zaznaczony element
        public IDrawable GetSelectedElement()
        {
            return selectedElement;
        }
    }
}
