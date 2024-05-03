using System.Windows;

namespace screenerWpf.Interfaces
{
    public interface ICanvasEditingHandler
    {
        void StartEditing(IDrawable element, Point location); 
    }
}
