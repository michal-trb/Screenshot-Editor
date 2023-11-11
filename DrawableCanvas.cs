using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf
{
    public class DrawableCanvas : Canvas
    {
        public List<DrawableElement> Elements { get; private set; } = new List<DrawableElement>();
        private DrawableElement selectedElement;

        public DrawableCanvas()
        {
            MouseLeftButtonDown += DrawableCanvas_MouseLeftButtonDown;
            MouseRightButtonDown += DrawableCanvas_MouseRightButtonDown;
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
            // Iterate through elements in reverse order to select the topmost element
            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                if (Elements[i].HitTest(clickPoint))
                {
                    SelectElement(Elements[i]);
                    break;
                }
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
  
        public void DeleteSelectedElement()
        {
            if (selectedElement != null)
            {
                Elements.Remove(selectedElement);
                selectedElement = null;
                InvalidateVisual(); // Redraw the canvas
            }
        }
    }
}