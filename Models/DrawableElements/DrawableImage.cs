using System;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{
    public class DrawableImage : DrawableWithHandles
    {
        private ImageSource imageSource;
        private DragHandle currentDragHandle = DragHandle.None;

        public Size Size { get; private set; }

        public DrawableImage(ImageSource source, Point position) : base(4)
        {
            this.imageSource = source;
            this.Position = position;
            // Ustawienie rozmiaru na podstawie źródła obrazu
            this.Size = new Size(source.Width, source.Height);
        }

        public override void Draw(DrawingContext context)
        {
            if (this.imageSource != null)
            {
                context.DrawImage(this.imageSource, new Rect(this.Position, this.Size));
            }
            if (IsSelected)
            {
                UpdateHandlePoints();
                DrawSelectionHandles(context);
            }
        }

        public override Rect GetBounds()
        {
            return new Rect(this.Position, this.Size);
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

        protected override void UpdateHandlePoints()
        {
            Rect rect = new Rect(Position, Size);
            HandlePoints[0] = rect.TopLeft; // Lewy górny narożnik
            HandlePoints[1] = rect.TopRight; // Prawy górny narożnik
            HandlePoints[2] = rect.BottomLeft; // Lewy dolny narożnik
            HandlePoints[3] = rect.BottomRight; // Prawy dolny narożnik
        }

        public override DrawableElement Clone()
        {
            return new DrawableImage(this.imageSource, new Point(Position.X + 5, Position.Y + 5));
        }
    }
}
