using System.Windows;
using System.Windows.Controls;

namespace screenerWpf.Interfaces
{
    public interface ICanvasEditingHandler
    {
        void StartEditing(IDrawable element, Point location);

 
    }
}
