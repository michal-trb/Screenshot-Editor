using System.Windows;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{
    public class DrawableBackground : DrawableElement
    {
        public DrawableBackground()
        {
            this.CanBeSelected = false;
        }

        public override void Draw(DrawingContext context)
        {
            // Gradient
            LinearGradientBrush gradientBrush = new LinearGradientBrush(
                Colors.LightSkyBlue,
                Colors.WhiteSmoke,
                new Point(0, 0),
                new Point(1, 1));

            // Tło
            context.DrawRectangle(gradientBrush, null, new Rect(Position, Size));

            // Ramka
            Pen borderPen = new Pen(Brushes.DarkSlateGray, 3);
            context.DrawRectangle(null, borderPen, new Rect(Position, Size));
        }

        public override void Select()
        {
            // Pusta implementacja, aby zapobiec zaznaczaniu.
        }

        public override bool Contains(Point point)
        {
            // Zawsze zwracaj false, aby zapobiec wykrywaniu kliknięć.
            return false;
        }

        public override void Move(Vector delta)
        {
            // Pusta implementacja, aby zapobiec przesuwaniu.
        }

        public override Rect GetBounds()
        {
            // Można zwrócić Rect.Empty lub utrzymać oryginalną wielkość, ale zignorować w logice interakcji.
            return Rect.Empty;
        }

        public override DrawableElement Clone()
        {
            // Clone nie jest potrzebne w przypadku tła, ale jeśli potrzebujesz, dostosuj według potrzeb.
            return null;

        }
    }
}
