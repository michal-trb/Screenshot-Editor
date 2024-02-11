using System;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{
    public class DrawableScreenshot : DrawableElement
    {
        private ImageSource imageSource;

        // Zmieniamy konstruktor, aby nie wymagał base(4), ponieważ nie będziemy rysować uchwytów
        public DrawableScreenshot(ImageSource source, Point position, Size size)
        {
            this.imageSource = source;
            this.Position = position;
            this.Size = size;
            this.CanBeSelected = false;
            // Rozmiar można skalować już w konstruktorze
        }

        public override DrawableElement Clone()
        {
            // Implementacja klonowania, jeśli potrzebujesz. Może rzucać wyjątek, jeśli klonowanie nie jest wspierane.
            throw new NotImplementedException();
        }

        public override void Draw(DrawingContext context)
        {
            // Rysowanie obrazu
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
