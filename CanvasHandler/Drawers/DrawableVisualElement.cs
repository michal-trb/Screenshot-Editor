using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace screenerWpf.CanvasHandler.Drawers
{
    internal class DrawableVisualElement : DrawableElement
    {
        public DrawingVisual Visual { get; set; }
        public Point Position { get; set; }
        public Size Size { get; set; }
        private DrawableCanvas Canvas { get; set; } 

        public DrawableVisualElement(DrawableCanvas canvas) 
        {
            Canvas = canvas;
        }

        public override void Draw(DrawingContext context)
        {
            Rect rect = new Rect(Position, Size);

            if (Visual != null)
            {
                // Stosowanie transformacji do rysowanych obiektów
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform(Position.X, Position.Y));

                context.PushTransform(transformGroup);
                context.DrawDrawing(Visual.Drawing);
                context.Pop();
            }
        }


        public override Rect GetBounds()
        {
            return new Rect(Position, Size);
        }

        public override bool HitTest(Point point)
        {
            Rect bounds = GetBounds();
            return bounds.Contains(point);
        }

        public override void Move(Vector delta)
        {
            Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
            UpdateVisual(); // Aktualizuj obraz po przesunięciu
        }

        public void UpdateVisual()
        {
            RenderTargetBitmap originalBitmap = Canvas.GetOriginalTargetBitmap();

            if (Size.Width <= 1 || Size.Height <= 1)
            {
                return; // Nie aktualizuj, jeśli szerokość lub wysokość jest zerowa lub ujemna
            }

            // Definiowanie nowego wycinka z oryginalnego obrazu
            Int32Rect cropRect = new Int32Rect(
                (int)Position.X,
                (int)Position.Y,
                (int)Size.Width,
                (int)Size.Height);

            // Tworzenie CroppedBitmap na podstawie nowego wycinka
            CroppedBitmap croppedBitmap = new CroppedBitmap(originalBitmap, cropRect);

            // Aktualizacja DrawingVisual z nowym fragmentem obrazu
            Visual = new DrawingVisual();
            using (DrawingContext dc = Visual.RenderOpen())
            {
                // Stosowanie efektu rozmycia
                BlurEffect blurEffect = new BlurEffect { Radius = 2 }; // Zwiększony radius dla mocniejszego efektu

                // Tworzenie tymczasowego DrawingVisual do nałożenia efektu rozmycia
                DrawingVisual tempVisual = new DrawingVisual();
                using (DrawingContext tempDc = tempVisual.RenderOpen())
                {
                    tempDc.DrawImage(croppedBitmap, new Rect(0, 0, cropRect.Width, cropRect.Height));
                }
                tempVisual.Effect = blurEffect;

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
        }

    }
}
