using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace screenerWpf
{
    public class ElementManager
    {
        public List<DrawableElement> Elements { get; private set; } = new List<DrawableElement>();

        public void AddElement(DrawableElement element)
        {
            if (element == null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            Elements.Add(element);
        }

        public void RemoveElement(DrawableElement element)
        {
            if (element == null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            Elements.Remove(element);
        }

        public DrawableElement GetElementAtPoint(Point point)
        {
            // Zwraca pierwszy element, który pasuje do testu trafienia
            return Elements.FirstOrDefault(element => element.HitTest(point));
        }

        public void BringToFront(DrawableElement element)
        {
            if (element == null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            // Usuń i dodaj ponownie, aby umieścić na wierzchu listy
            Elements.Remove(element);
            Elements.Add(element);
        }
    }
}
