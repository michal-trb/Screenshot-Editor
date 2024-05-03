using System.Windows;
using System.Windows.Media.Imaging;

namespace screenerWpf.Interfaces
{
    public interface IDrawable
    {
        void Draw(WriteableBitmap bitmap);
        void Remove();
        void Select();
        bool ContainsPoint(Point point);
        bool Contains(Point point);
        void Move(Vector delta);
        Point GetLocation();
        Rect GetBounds();
        DrawableElement Clone();
    }
}
