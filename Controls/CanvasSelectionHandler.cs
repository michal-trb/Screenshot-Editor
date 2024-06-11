using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace screenerWpf.Controls
{
    public class CanvasSelectionHandler : ICanvasSelectionHandler
    {
        private DrawableCanvas drawableCanvas;
        private IDrawable selectedElement;
        private Point lastMousePosition;
        private ICanvasEditingHandler editingHandler;
        private const double SpeechBubbleTailTolerance = 40;
        private const double ArrowTolerance = 30; 
        public CanvasSelectionHandler(DrawableCanvas canvas, ICanvasEditingHandler editingHandler)
        {
            drawableCanvas = canvas;
            this.editingHandler = editingHandler;
        }

        public void HandleLeftButtonDown(MouseButtonEventArgs e)
        {
            if (drawableCanvas.isFirstClick)
            {
                drawableCanvas.originalTargetBitmap = drawableCanvas.GetRenderTargetBitmap();
                drawableCanvas.isFirstClick = false;
            }

            Point clickPosition = e.GetPosition(drawableCanvas);
            lastMousePosition = clickPosition;

            if (!TrySelectSpeechBubbleTail(clickPosition) && !TrySelectElement(clickPosition))
            {
                // Jeśli nie kliknięto w ogonek dymku ani w strzałkę, sprawdź inne elementy
                var element = drawableCanvas.elementManager.GetElementAtPoint(clickPosition);

                if (element != null)
                {
                    selectedElement = element;
                }
                else
                {
                    DeselectCurrentElement();
                }
            }
        }

        public void HandleDoubleClick(MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(drawableCanvas);
            lastMousePosition = clickPosition;

            if (drawableCanvas.isFirstClick)
            {
                drawableCanvas.originalTargetBitmap = drawableCanvas.GetRenderTargetBitmap();
                drawableCanvas.isFirstClick = false;
            }

            var element = drawableCanvas.elementManager.GetElementAtPoint(clickPosition);
            if (element != null)
            {
                selectedElement = element;

                if (element is DrawableText drawableText)
                {
                    editingHandler.StartEditing(drawableText, clickPosition);
                }
                else if (element is DrawableSpeechBubble speechBubble)
                {
                    editingHandler.StartEditing(speechBubble, clickPosition);
                }
            }
            else
            {
                DeselectCurrentElement();
            }
        }

        private bool TrySelectSpeechBubbleTail(Point clickPoint)
        {
            foreach (var element in drawableCanvas.elementManager.Elements.OfType<DrawableSpeechBubble>())
            {
                if (IsNearPoint(element.EndTailPoint, clickPoint, SpeechBubbleTailTolerance))
                {
                    selectedElement = element;
                    element.SetTailBeingDragged(true);
                    return true;
                }
            }
            return false;
        }

        private bool TrySelectElement(Point clickPoint)
        {
            var element = drawableCanvas.elementManager.GetElementAtPoint(clickPoint);

            if (element != null)
            {
                selectedElement = element;
                HandleArrowSpecificLogic(element, clickPoint);
                return true;
            }
            return false;
        }

        private void HandleArrowSpecificLogic(DrawableElement element, Point clickPoint)
        {
            if (element is DrawableArrow arrow)
            {
                arrow.SetStartBeingDragged(IsNearPoint(arrow.Position, clickPoint, ArrowTolerance));
                arrow.SetEndBeingDragged(IsNearPoint(arrow.EndPoint, clickPoint, ArrowTolerance));
            }
        }

        private bool IsNearPoint(Point point1, Point point2, double tolerance)
        {
            return Math.Abs(point1.X - point2.X) <= tolerance && Math.Abs(point1.Y - point2.Y) <= tolerance;
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && selectedElement != null)
            {
                Point currentMousePosition = e.GetPosition(drawableCanvas);
                Vector delta = currentMousePosition - lastMousePosition;
                selectedElement.Move(delta);
                lastMousePosition = currentMousePosition;
                drawableCanvas.InvalidateVisual();
            }
        }


        public void HandleLeftButtonUp(MouseButtonEventArgs e)
        {
            if (selectedElement is DrawableSpeechBubble speechBubble)
            {
                speechBubble.SetTailBeingDragged(false);
            }
            else if (selectedElement is DrawableArrow arrow)
            {
                arrow.SetEndBeingDragged(false);
                arrow.SetStartBeingDragged(false);
            }
        }

        private void DeselectCurrentElement()
        {
            selectedElement = null;
            drawableCanvas.InvalidateVisual();
        }

        public void DeleteSelectedElement()
        {
            if (selectedElement != null)
            {
                drawableCanvas.RemoveElement(selectedElement);
                DeselectCurrentElement();
                drawableCanvas.InvalidateVisual();
            }
        }

        public bool HasSelectedElement()
        {
            return selectedElement != null;
        }

        public IDrawable GetSelectedElement()
        {
            return selectedElement;
        }
    }
}
