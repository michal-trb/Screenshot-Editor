using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Helpers.DpiHelper;
using screenerWpf.Controls;
using screenerWpf.Interfaces;

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
            double tolerance = 10; 
            return Math.Abs(point.X - corner.X) <= tolerance && Math.Abs(point.Y - corner.Y) <= tolerance;
        }

        public override Rect GetBounds()
        {
            return new Rect(Position, Size);
        }

        public override void Move(Vector delta)
        {
            // Zdefiniowanie nowych zmiennych dla pozycji i rozmiaru
            double newX = Position.X;
            double newY = Position.Y;
            double newWidth = Size.Width;
            double newHeight = Size.Height;

            switch (currentDragHandle)
            {
                case DragHandle.None:
                    newX += delta.X;
                    newY += delta.Y;
                    break;
                case DragHandle.TopLeft:
                    newX += delta.X;
                    newY += delta.Y;
                    newWidth -= delta.X;
                    newHeight -= delta.Y;
                    break;
                case DragHandle.TopRight:
                    newWidth += delta.X;
                    newY += delta.Y;
                    newHeight -= delta.Y;
                    break;
                case DragHandle.BottomLeft:
                    newX += delta.X;
                    newWidth -= delta.X;
                    newHeight += delta.Y;
                    break;
                case DragHandle.BottomRight:
                    newWidth += delta.X;
                    newHeight += delta.Y;
                    break;
            }

            // Sprawdzanie i dostosowywanie, jeśli szerokość lub wysokość są ujemne
            if (newWidth < 0)
            {
                newX += newWidth;
                newWidth = -newWidth;
                // Zmiana przeciąganego narożnika na przeciwny w osi X
                currentDragHandle = currentDragHandle == DragHandle.TopLeft ? DragHandle.TopRight :
                                    currentDragHandle == DragHandle.TopRight ? DragHandle.TopLeft :
                                    currentDragHandle == DragHandle.BottomLeft ? DragHandle.BottomRight :
                                    DragHandle.BottomLeft;
            }
            if (newHeight < 0)
            {
                newY += newHeight;
                newHeight = -newHeight;
                // Zmiana przeciąganego narożnika na przeciwny w osi Y
                currentDragHandle = currentDragHandle == DragHandle.TopLeft ? DragHandle.BottomLeft :
                                    currentDragHandle == DragHandle.BottomLeft ? DragHandle.TopLeft :
                                    currentDragHandle == DragHandle.TopRight ? DragHandle.BottomRight :
                                    DragHandle.TopRight;
            }

            // Ustawienie nowych wartości pozycji i rozmiaru
            Position = new Point(newX, newY);
            Size = new Size(newWidth, newHeight);
        }

        public void UpdateVisual()
        {
            RenderTargetBitmap originalBitmap = Canvas.GetOriginalTargetBitmap();

            if (Size.Width <= 1 || Size.Height <= 1)
            {
                return;
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
                var currentDpi = DpiHelper.CurrentDpi;

                // Renderowanie tempVisual do RenderTargetBitmap
                RenderTargetBitmap blurredBitmap = new RenderTargetBitmap(
                    cropRect.Width,
                    cropRect.Height,
                    currentDpi.DpiX,
                    currentDpi.DpiY,
                    PixelFormats.Pbgra32);
                blurredBitmap.Render(tempVisual);

                // Rysowanie RenderTargetBitmap na głównym DrawingVisual
                dc.DrawImage(blurredBitmap, new Rect(0, 0, cropRect.Width, cropRect.Height));
            }
        }

        public override DrawableElement Clone()
        {
            return new DrawableBlur(Canvas)
            {
                StrokeColor = this.StrokeColor,
                StrokeThickness = this.StrokeThickness,
                BlurEffect = this.BlurEffect,
                Visual = this.Visual,
                Canvas = this.Canvas,
                Color = this.Color,
                Position = new Point(Position.X + 5, Position.Y + 5),
                Size = this.Size,
                Scale = this.Scale,
            };
        }
    }
}
