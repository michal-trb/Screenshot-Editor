using screenerWpf.Helpers;
using screenerWpf.Interfaces;
using screenerWpf.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace screenerWpf.Controls
{
    public class DrawableCanvas : Canvas
    {
        public ElementManager elementManager = new ElementManager();
        private HistoryManager historyManager = new HistoryManager();
        public DrawableElement selectedElement;
        public bool isFirstClick = true;
        public RenderTargetBitmap originalTargetBitmap;
        private ScaleTransform scaleTransform = new ScaleTransform();
        private TranslateTransform moveTransform = new TranslateTransform();
        public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent(
           "MouseDoubleClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(DrawableCanvas));

        public event MouseButtonEventHandler MouseDoubleClick
        {
            add { AddHandler(MouseDoubleClickEvent, value); }
            remove { RemoveHandler(MouseDoubleClickEvent, value); }
        }

        public DrawableCanvas()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem copyMenuItem = new MenuItem { Header = "Kopiuj" };
            copyMenuItem.Click += CopyMenuItem_Click;
            contextMenu.Items.Add(copyMenuItem);
            this.ContextMenu = contextMenu;
            this.MouseWheel += DrawableCanvas_MouseWheel;

            // Ustaw transformacje
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(moveTransform);

            this.MouseWheel += DrawableCanvas_MouseWheel;
            // Ustawienie transformacji
            this.RenderTransform = scaleTransform;
            this.ClipToBounds = true;

            // Ustawienie Clip
            this.Loaded += (sender, e) => UpdateClip();
            this.SizeChanged += (sender, e) => UpdateClip();
        }

        private void UpdateClip()
        {
            this.Clip = new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.ClickCount == 2)
            {
                // Podnoszenie zdarzenia MouseDoubleClick
                var newEventArgs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
                newEventArgs.RoutedEvent = MouseDoubleClickEvent;
                RaiseEvent(newEventArgs);
            }
        }

        private void DrawableCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                const double scaleFactor = 1.1;
                const double minScale = 0.2; // Minimalna skala

                double zoom = e.Delta > 0 ? scaleFactor : 1 / scaleFactor;
                double newScaleX = scaleTransform.ScaleX * zoom;
                double newScaleY = scaleTransform.ScaleY * zoom;

                if (newScaleX >= minScale && newScaleY >= minScale)
                {
                    var mousePosition = e.GetPosition(this);

                    if (newScaleX < 1 && newScaleY < 1)
                    {
                        // Gdy skala jest mniejsza niż 1, wyśrodkuj zawartość
                        CenterContent();
                    }
                    else
                    {
                        // W przeciwnym przypadku stosuj standardowe skalowanie
                        scaleTransform.CenterX = mousePosition.X;
                        scaleTransform.CenterY = mousePosition.Y;
                    }

                    scaleTransform.ScaleX = newScaleX;
                    scaleTransform.ScaleY = newScaleY;
                }

                e.Handled = true;
            }
        }

        private void CenterContent()
        {
            // Centrowanie zawartości canvasa
            double offsetX = (this.ActualWidth - (this.ActualWidth * scaleTransform.ScaleX)) / 2;
            double offsetY = (this.ActualHeight - (this.ActualHeight * scaleTransform.ScaleY)) / 2;

            moveTransform.X = offsetX;
            moveTransform.Y = offsetY;
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CopyCanvasToClipboard();
        }

        private void CopyCanvasToClipboard()
        {
            RenderTargetBitmap bitmap = GetRenderTargetBitmap(); // Metoda, którą już zdefiniowałeś
            Clipboard.SetImage(bitmap);
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

        public void SelectElementAtPoint(Point point)
        {
            var element = elementManager.GetElementAtPoint(point);
            if (element != null)
            {
                SelectElement(element);
            }
            else
            {
                DeselectCurrentElement();
            }
            Focus();
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

        public void AddElement(DrawableElement element)
        {
            elementManager.AddElement(element);
            historyManager.AddAction(new AddElementAction(this, element));
            InvalidateVisual();
        }

        public void RemoveElement(IDrawable element)
        {
            if (element != null)
            {
                elementManager.RemoveElement((DrawableElement)element);
                historyManager.AddAction(new RemoveElementAction(this, (DrawableElement)element));
                InvalidateVisual();
            }
        }

        public void Undo()
        {
            if (historyManager.CanUndo)
            {
                historyManager.Undo();
                InvalidateVisual();
            }
        }

        public void Redo()
        {
            if (historyManager.CanRedo)
            {
                historyManager.Redo();
                InvalidateVisual();
            }
        }
    }
}