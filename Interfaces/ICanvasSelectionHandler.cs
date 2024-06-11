using System.Windows.Input;

namespace screenerWpf.Interfaces
{
    public interface ICanvasSelectionHandler
    {
        void HandleLeftButtonDown(MouseButtonEventArgs e);
        void HandleMouseMove(MouseEventArgs e);
        void HandleLeftButtonUp(MouseButtonEventArgs e);
        void DeleteSelectedElement();
        bool HasSelectedElement();
        IDrawable GetSelectedElement();
        void HandleDoubleClick(MouseButtonEventArgs e);
    }
}
