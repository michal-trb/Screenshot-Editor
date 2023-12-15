using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace screenerWpf
{
    public class DrawableCanvas : Canvas
    {
        public List<DrawableElement> Elements { get; private set; } = new List<DrawableElement>();
        private DrawableElement selectedElement;
        private Point lastMousePosition;
        private bool isDragging;

        private const double SpeechBubbleTailTolerance = 40; // Tolerance in pixels
        private const double ArrowTolerance = 30; // Tolerance in pixels

        public DrawableCanvas()
        {
            MouseLeftButtonDown += DrawableCanvas_MouseLeftButtonDown;
            MouseRightButtonDown += DrawableCanvas_MouseRightButtonDown;
            MouseLeftButtonUp += DrawableCanvas_MouseLeftButtonUp;
            MouseMove += DrawableCanvas_MouseMove;
        }

        public ImageSource BackgroundImage
        {
            get { return (ImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundImage
        public static readonly DependencyProperty BackgroundImageProperty =
            DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(DrawableCanvas),
                new PropertyMetadata(null, OnBackgroundImageChanged));

        private static void OnBackgroundImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Przy zmianie właściwości BackgroundImage wymuś ponowne narysowanie
            (d as DrawableCanvas)?.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            // Rysowanie obrazu tła
            if (BackgroundImage != null)
            {
                Rect rect = new Rect(0, 0, ActualWidth, ActualHeight);
                dc.DrawImage(BackgroundImage, rect);
            }

            // Rysowanie pozostałych elementów
            base.OnRender(dc);

            foreach (var element in Elements)
            {
                element.Draw(dc);
            }
        }

        private void DrawableCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(this);
            lastMousePosition = clickPoint;
            isDragging = false;

            if (!TrySelectSpeechBubbleTail(clickPoint) 
                && !TrySelectElement(clickPoint))
            {
                DeselectCurrentElement();
            }
        }

        private bool TrySelectSpeechBubbleTail(Point clickPoint)
        {
            foreach (var element in Elements.OfType<DrawableSpeechBubble>())
            {
                if (IsNearPoint(element.EndPoint, clickPoint, SpeechBubbleTailTolerance))
                {
                    HandleElementSelection(element, tailBeingDragged: true);
                    return true;
                }
            }

            return false;
        }

        private bool TrySelectElement(Point clickPoint)
        {
            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                if (Elements[i].HitTest(clickPoint))
                {
                    HandleElementSelection(Elements[i]);
                    HandleArrowSpecificLogic(Elements[i], clickPoint);
                    return true;
                }
            }

            return false;
        }

        private void HandleElementSelection(DrawableElement element, bool tailBeingDragged = false)
        {
            SelectElement(element);
            isDragging = true;

            if (element is DrawableSpeechBubble speechBubble)
            {
                speechBubble.SetTailBeingDragged(tailBeingDragged);
            }
        }

        private void HandleArrowSpecificLogic(DrawableElement element, Point clickPoint)
        {
            if (element is DrawableArrow arrow)
            {
                arrow.SetStartBeingDragged(IsNearPoint(arrow.Position, clickPoint, ArrowTolerance));
                arrow.SetEndBeingDragged(IsNearPoint(arrow.EndPoint, clickPoint, ArrowTolerance));
            }
            // Add logic for other element types if necessary
        }

        private bool IsNearPoint(Point point1, Point point2, double tolerance)
        {
            return Math.Abs(point1.X - point2.X) <= tolerance &&
                   Math.Abs(point1.Y - point2.Y) <= tolerance;
        }


        private void DeselectCurrentElement()
        {
            if (selectedElement != null)
            {
                selectedElement.IsSelected = false;
                selectedElement = null;
                InvalidateVisual(); // Przerysowanie canvas, aby odzwierciedlić zmianę stanu zaznaczenia
            }
        }

        private void DrawableCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedElement is DrawableArrow arrow)
            {
                arrow.SetEndBeingDragged(false);
                arrow.SetStartBeingDragged(false);
            }
            else if (selectedElement is DrawableSpeechBubble speechBubble)
            {
                speechBubble.SetTailBeingDragged(false);
            }

            isDragging = false;
        }

        private void DrawableCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //This code is only for draging Tail in speechBubble
            if (isDragging && selectedElement != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(this);
                Vector delta = currentPosition - lastMousePosition;

                if (selectedElement is DrawableSpeechBubble speechBubble && speechBubble.isTailBeingDragged)
                {
                    speechBubble.EndPoint = new Point(speechBubble.EndPoint.X + delta.X, speechBubble.EndPoint.Y + delta.Y);
                }
                lastMousePosition = currentPosition;
                InvalidateVisual();
            }
        }

        private void SelectElement(DrawableElement element)
        {
            if (selectedElement != null)
            {
                selectedElement.IsSelected = false;
            }

            selectedElement = element;

            if (selectedElement != null)
            {
                selectedElement.IsSelected = true;
                // You might want to bring the selected element to the front
                Elements.Remove(selectedElement);
                Elements.Add(selectedElement);
            }

            InvalidateVisual(); // Redraw canvas
        }

        private void DrawableCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedElement != null)
            {
                Elements.Remove(selectedElement);
                selectedElement = null;
                InvalidateVisual(); // Redraw canvas
            }

            this.Focus();
        }

        // Method to add drawable elements to the canvas
        public void AddElement(DrawableElement element)
        {
            Elements.Add(element);
            InvalidateVisual(); // Redraw canvas
        }

        public void SelectElementAtPoint(Point point)
        {
            foreach (var element in Elements) // Use the initialized list 'Elements' instead of 'drawableElements'
            {
                if (element.HitTest(point)) // Assuming 'HitTest' is a method to determine if the point hits the element
                {
                    SelectElement(element);
                    break; // If you want only the first hit element to be selected, otherwise remove this line for multi-selection.
                }
            }
            InvalidateVisual();
            Focus(); // Set focus to the DrawableCanvas to receive key events
                     // This will cause the canvas to be redrawn with the new selection state.
        }

        internal void RemoveElement(IDrawable selectedElement)
        {
            if (selectedElement != null)
            {
                Elements.Remove((DrawableElement)selectedElement);
                InvalidateVisual(); // Redraw the canvas
            }
        }

        internal IDrawable FindElementAt(Point position)
        {
            // Przykładowa implementacja może przejrzeć wszystkie rysowane elementy
            // i sprawdzić, czy dany punkt jest w ich obszarze.
            foreach (IDrawable element in Elements)
            {
                if (element.ContainsPoint(position))
                {
                    return element;
                }
            }
            return null;
        }
    }
}