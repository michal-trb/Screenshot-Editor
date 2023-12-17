using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace screenerWpf
{
    public class DrawableCanvas : Canvas
    {
        private ElementManager elementManager = new ElementManager();
        private DrawableElement selectedElement;
        private Point lastMousePosition;
        private bool isDragging;
        private bool isFirstClick = true;
        private RenderTargetBitmap originalTargetBitmap;

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

        public static readonly DependencyProperty BackgroundImageProperty =
            DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(DrawableCanvas), new PropertyMetadata(null, OnBackgroundImageChanged));

        private static void OnBackgroundImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrawableCanvas canvas)
            {
                canvas.InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (BackgroundImage != null)
            {
                Rect rect = new Rect(0, 0, ActualWidth, ActualHeight);
                dc.DrawImage(BackgroundImage, rect);
            }

            base.OnRender(dc);
            foreach (var element in elementManager.Elements)
            {
                element.Draw(dc);
            }
        }

        private void DrawableCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isFirstClick)
            {
                originalTargetBitmap = GetRenderTargetBitmap();
                isFirstClick = false;
            }

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
            foreach (var element in elementManager.Elements.OfType<DrawableSpeechBubble>())
            {
                if (IsNearPoint(element.EndTailPoint, clickPoint, SpeechBubbleTailTolerance))
                {
                    HandleElementSelection(element, tailBeingDragged: true);
                    return true;
                }
            }
            return false;
        }

        private bool TrySelectElement(Point clickPoint)
        {
            var element = elementManager.GetElementAtPoint(clickPoint);
            if (element != null)
            {
                HandleElementSelection(element);
                HandleArrowSpecificLogic(element, clickPoint);
                return true;
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
        }

        private bool IsNearPoint(Point point1, Point point2, double tolerance)
        {
            return Math.Abs(point1.X - point2.X) <= tolerance && Math.Abs(point1.Y - point2.Y) <= tolerance;
        }

        private void DeselectCurrentElement()
        {
            if (selectedElement != null)
            {
                selectedElement.IsSelected = false;
                selectedElement = null;
                InvalidateVisual();
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
            if (isDragging && selectedElement != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(this);
                Vector delta = currentPosition - lastMousePosition;

                if (selectedElement is DrawableSpeechBubble speechBubble && speechBubble.isTailBeingDragged)
                {
                    speechBubble.EndTailPoint = new Point(speechBubble.EndTailPoint.X + delta.X, speechBubble.EndTailPoint.Y + delta.Y);
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
                elementManager.BringToFront(selectedElement);
            }

            InvalidateVisual();
        }

        private void DrawableCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedElement != null)
            {
                elementManager.RemoveElement(selectedElement);
                selectedElement = null;
                InvalidateVisual();
            }
            Focus();
        }

        public void AddElement(DrawableElement element)
        {
            elementManager.AddElement(element);
            InvalidateVisual();
        }

        public void SelectElementAtPoint(Point point)
        {
            var element = elementManager.GetElementAtPoint(point);
            if (element != null)
            {
                SelectElement(element);
            }
            InvalidateVisual();
            Focus();
        }

        internal void RemoveElement(IDrawable selectedElement)
        {
            if (selectedElement != null)
            {
                elementManager.RemoveElement((DrawableElement)selectedElement);
                InvalidateVisual();
            }
        }

        internal IDrawable FindElementAt(Point position)
        {
            return elementManager.GetElementAtPoint(position);
        }

        internal RenderTargetBitmap GetRenderTargetBitmap()
        {
            // Ustalenie wymiarów bitmapy - powinny odpowiadać wymiarom płótna
            int width = (int)Math.Ceiling(ActualWidth);
            int height = (int)Math.Ceiling(ActualHeight);

            // Utworzenie bitmapy renderującej
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

            // Utworzenie wizualizacji na podstawie płótna
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(this);
                context.DrawRectangle(brush, null, new Rect(new Point(), new Size(width, height)));
            }

            // Renderowanie wizualizacji na bitmapie
            renderBitmap.Render(visual);

            return renderBitmap;
        }

        internal RenderTargetBitmap GetOriginalTargetBitmap()
        {
            return originalTargetBitmap;
        }
    }
}
