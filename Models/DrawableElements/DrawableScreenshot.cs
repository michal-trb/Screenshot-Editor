using System;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{
    public class DrawableScreenshot : DrawableElement
    {
        private ImageSource imageSource;

        // Zmieniam konstruktor, aby nie wymagał base(4), ponieważ nie będziemy rysować uchwytów
        public DrawableScreenshot(ImageSource source, Point position, Size size)
        {
            this.imageSource = source;
            this.Position = position;
            this.Size = size;
            this.CanBeSelected = false;
        }

        public override DrawableElement Clone()
        {
            throw new NotImplementedException();
        }

        public override void Draw(DrawingContext context)
        {
            if (imageSource != null)
            {
                context.DrawImage(imageSource, new Rect(Position, Size));
            }
            // Nie rysujemy uchwytów selekcji, ponieważ nie chcemy umożliwić przesuwania ani zaznaczania tego elementu
        }

        public override Rect GetBounds()
        {
            // Metoda powinna zwracać prawidłowe granice dla elementu, ale zablokujemy interakcję z tym elementem
            return new Rect(Position, Size);
        }

        public override bool HitTest(Point point)
        {
            // Zawsze zwracamy false, aby uniemożliwić wybieranie tego elementu
            return false;
        }

        public override void Move(Vector delta)
        {
            // Przesunięcie jest zablokowane, więc ta metoda nie robi nic
        }
    }
}
