using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using screenerWpf.Controls;

namespace screenerWpf.Models.DrawableElements
{
    public class DrawableBlur : DrawableWithHandles
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public Color StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        public Effect BlurEffect { get; set; }
        public DrawingVisual Visual { get; set; }
        private DrawableCanvas Canvas { get; set; }
        private enum DragHandle { None, TopLeft, TopRight, BottomLeft, BottomRight }

        private DragHandle currentDragHandle = DragHandle.None;
        public DrawableBlur(DrawableCanvas canvas) : base(4)
        {
            Canvas = canvas;
            Position = new Point(0, 0);
            Size = new Size(100, 50);
            StrokeColor = Colors.Transparent;
            StrokeThickness = 1.0;
            BlurEffect = new BlurEffect { Radius = 10 };
            Visual = new DrawingVisual();
        }

        public override void Draw(DrawingContext context)
        {
            UpdateVisual();
            Rect rect = new Rect(Position, Size);

            if (Visual != null)
            {
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform(Position.X, Position.Y));

                context.PushTransform(transformGroup);
                context.DrawDrawing(Visual.Drawing);
                context.Pop();
            }
            if (IsSelected)
            {
                UpdateHandlePoints();
                DrawSelectionHandles(context);
            }
        }
        protected override void UpdateHandlePoints()
        {
            Rect rect = new Rect(Position, Size);
            HandlePoints[0] = rect.TopLeft; // Lewy górny narożnik
            HandlePoints[1] = rect.TopRight; // Prawy górny narożnik
            HandlePoints[2] = rect.BottomLeft; // Lewy dolny narożnik
            HandlePoints[3] = rect.BottomRight; // Prawy dolny narożnik
        }

        public override bool HitTest(Point point)
        {
            Rect rect = new Rect(Position, Size);
            currentDragHandle = DragHandle.None;

            if (IsNearCorner(point, Position)) currentDragHandle = DragHandle.TopLeft;
            else if (IsNearCorner(point, new Point(Position.X + Size.Width, Position.Y))) currentDragHandle = DragHandle.TopRight;
            else if (IsNearCorner(point, new Point(Position.X, Position.Y + Size.Height))) currentDragHandle = DragHandle.BottomLeft;
            else if (IsNearCorner(point, new Point(Position.X + Size.Width, Position.Y + Size.Height))) currentDragHandle = DragHandle.BottomRight;

            return rect.Contains(point) || currentDragHandle != DragHandle.None;
        }


        private bool IsNearCorner(Point point, Point corner)
        {
            // Metoda sprawdzająca, czy punkt znajduje się blisko narożnika
            double tolerance = 10; // Możesz dostosować tolerancję
            return Math.Abs(point.X - corner.X) <= tolerance && Math.Abs(point.Y - corner.Y) <= tolerance;
        }

        public override Rect GetBounds()
        {
            return new Rect(Position, Size);
        }

        public override void Move(Vector delta)
        {
            switch (currentDragHandle)
            {
                case DragHandle.None:
                    Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
                    break;
                case DragHandle.TopLeft:
                    Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
                    Size = new Size(Size.Width - delta.X, Size.Height - delta.Y);
                    break;
                case DragHandle.TopRight:
                    Size = new Size(Size.Width + delta.X, Size.Height - delta.Y);
                    Position = new Point(Position.X, Position.Y + delta.Y);
                    break;
                case DragHandle.BottomLeft:
                    Size = new Size(Size.Width - delta.X, Size.Height + delta.Y);
                    Position = new Point(Position.X + delta.X, Position.Y);
                    break;
                case DragHandle.BottomRight:
                    Size = new Size(Size.Width + delta.X, Size.Height + delta.Y);
                    break;
            }
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
                BlurEffect blurEffect = new BlurEffect { Radius = 10 }; // Zwiększony radius dla mocniejszego efektu

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
