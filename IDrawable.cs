using System.Windows;
using System.Windows.Media.Imaging;

namespace screenerWpf
{
    public interface IDrawable
    {
        void Draw(WriteableBitmap bitmap);
        void Remove();
        void Select(); // Define the method for selecting the element
        bool ContainsPoint(Point point);
        bool Contains(Point point);
        void Move(Vector delta);
        Point GetLocation();
        Rect GetBounds();
    }
}
