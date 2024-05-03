using screenerWpf.Controls;
using System.Windows.Input;

namespace screenerWpf.Interfaces
{
    public interface ICanvasActionHandler
    {
        void HandleLeftButtonDown(MouseButtonEventArgs e);
        void HandleLeftButtonUp(MouseButtonEventArgs e);
        void HandleMouseMove(MouseEventArgs e);
        void HandlePaste();
        void HandleCopy();
        void SetCurrentAction(EditAction action);
    }
}
