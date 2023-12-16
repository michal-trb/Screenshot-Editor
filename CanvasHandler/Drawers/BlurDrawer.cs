using screenerWpf.DrawableElements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace screenerWpf.CanvasHandler.Drawers
{
    public class BlurDrawer : DrawableElementDrawer
    {
        private DrawableBlur CurrentBlurArea { get; set; }

        internal DrawableVisualElement TemporaryVisualElement { get; private set; }

        private RenderTargetBitmap originalBitmap;

        public BlurDrawer(DrawableCanvas canvas) : base(canvas)
        {
        }

        public override void StartDrawing(MouseButtonEventArgs e)
        {
            Point startPoint = e.GetPosition(DrawableCanvas);
            originalBitmap = DrawableCanvas.GetRenderTargetBitmap();
            InitializeDrawableElements(startPoint);
        }

        public override void UpdateDrawing(MouseEventArgs e)
        {
            if (CurrentBlurArea == null) return;

            var currentPoint = e.GetPosition(DrawableCanvas);
            double width = Math.Abs(currentPoint.X - CurrentBlurArea.Position.X);
            double height = Math.Abs(currentPoint.Y - CurrentBlurArea.Position.Y);

            CurrentBlurArea.Size = new Size(width, height);

            // Aktualizacja tymczasowego elementu
            TemporaryVisualElement.Size = new Size(width, height);
            UpdateTemporaryVisualElement();

            DrawableCanvas.InvalidateVisual();
        }

        public override void FinishDrawing()
        {
            TemporaryVisualElement = null;
            CurrentBlurArea = null;
            originalBitmap = null; // Wyczyść oryginalny obraz
        }

        private void UpdateTemporaryVisualElement()
        {
            if (CurrentBlurArea.Size.Width <= 1 || CurrentBlurArea.Size.Height <= 1)
            {
                return; // Nie rysuj, jeśli szerokość lub wysokość jest zerowa lub ujemna
            }

            // Tworzenie DrawingVisual z efektem rozmycia
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                // Definiowanie wycinka z oryginalnego obrazu
                Int32Rect cropRect = new Int32Rect(
                    (int)CurrentBlurArea.Position.X,
                    (int)CurrentBlurArea.Position.Y,
                    (int)CurrentBlurArea.Size.Width,
                    (int)CurrentBlurArea.Size.Height);

                // Tworzenie CroppedBitmap na podstawie wycinka
                CroppedBitmap croppedBitmap = new CroppedBitmap(originalBitmap, cropRect);

                // Tworzenie tymczasowego DrawingVisual do nałożenia efektu rozmycia
                DrawingVisual tempVisual = new DrawingVisual();
                using (DrawingContext tempDc = tempVisual.RenderOpen())
                {
                    tempDc.DrawImage(croppedBitmap, new Rect(0, 0, cropRect.Width, cropRect.Height));
                }

                // Stosowanie efektu rozmycia
                tempVisual.Effect = new BlurEffect { Radius = 2 };

                // Renderowanie tempVisual do RenderTargetBitmap
                RenderTargetBitmap blurredBitmap = new RenderTargetBitmap(
                    cropRect.Width,
                    cropRect.Height,
                    96, // DpiX
                    96, // DpiY
                    PixelFormats.Pbgra32);
                blurredBitmap.Render(tempVisual);

                // Rysowanie RenderTargetBitmap na głównym DrawingVisual
                dc.DrawImage(blurredBitmap, new Rect(0, 0, cropRect.Width, cropRect.Height));
            }

            // Aktualizacja Visual w TemporaryVisualElement
            TemporaryVisualElement.Visual = visual;
        }


        private void InitializeDrawableElements(Point startPoint)
        {
            CurrentBlurArea = new DrawableBlur
            {
                Position = startPoint,
                Size = new Size(0, 0),
                StrokeColor = Colors.Transparent,
                StrokeThickness = 0,
                BlurEffect = new BlurEffect { Radius = 10 }
            };

            TemporaryVisualElement = new DrawableVisualElement(DrawableCanvas)
            {
                Visual = new DrawingVisual(),
                Position = startPoint,
                Size = new Size(0, 0)
            };
            DrawableCanvas.AddElement(TemporaryVisualElement);
        }
    }
}
