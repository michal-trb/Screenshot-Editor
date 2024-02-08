using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{
    public class DrawableRectangle : DrawableWithHandles
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public Color Color { get; set; }
        public double Thickness { get; set; }
        public double Transparency { get; internal set; }

        private enum DragHandle { None, TopLeft, TopRight, BottomLeft, BottomRight }
        private DragHandle currentDragHandle = DragHandle.None;
        public DrawableRectangle() : base(4)
        {
            Position = new Point(0, 0);
            Size = new Size(100, 50); // Domyślny rozmiar
            Color = Colors.Black; // Domyślny kolor obrysu
            Thickness = 1.0; // Domyślna grubość obrysu
            Transparency = 100;
        }

        public override void Draw(DrawingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            Rect rect = new Rect(Position, Size);
            Brush fillBrush = new SolidColorBrush();
            Pen strokePen = new Pen(new SolidColorBrush(Color.FromArgb((byte)(255 * Transparency / 100), Color.R, Color.G, Color.B)), Thickness);
            context.DrawRectangle(fillBrush, strokePen, rect);

            if (IsSelected)
            {
                UpdateHandlePoints(); // Aktualizuj pozycje uchwytów
                DrawSelectionHandles(context); // Narysuj uchwyty
            }
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

        protected override void UpdateHandlePoints()
        {
            Rect rect = new Rect(Position, Size);
            HandlePoints[0] = rect.TopLeft;     // Lewy górny
            HandlePoints[1] = rect.TopRight;    // Prawy górny
            HandlePoints[2] = rect.BottomLeft;  // Lewy dolny
            HandlePoints[3] = rect.BottomRight; // Prawy dolny
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

        public override DrawableElement Clone()
        {
            return new DrawableRectangle
            {
                Thickness = this.Thickness,
                Transparency = this.Transparency,
                Color = this.Color,
                Position = new Point(Position.X + 5, Position.Y + 5),
                Size = this.Size,
                Scale = this.Scale,
            };
        }
    }
}
