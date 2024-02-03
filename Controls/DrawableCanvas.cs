using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using screenerWpf.Helpers;
using screenerWpf.Interfaces;

namespace screenerWpf.Controls
{
    public class DrawableCanvas : Canvas
    {
        public ElementManager elementManager = new ElementManager();
        private DrawableElement selectedElement;
        public bool isFirstClick = true;
        public RenderTargetBitmap originalTargetBitmap;


        public DrawableCanvas()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem copyMenuItem = new MenuItem { Header = "Kopiuj" };
            copyMenuItem.Click += CopyMenuItem_Click;
            contextMenu.Items.Add(copyMenuItem);
            this.ContextMenu = contextMenu;
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
