using screenerWpf.Controls;
using System.Windows.Input;

namespace screenerWpf.Interfaces
{
    public interface ICanvasActionHandler
    {
        // Metody odpowiedzialne za obsługę zdarzeń myszy
        void HandleLeftButtonDown(MouseButtonEventArgs e);
        void HandleLeftButtonUp(MouseButtonEventArgs e);
        void HandleMouseMove(MouseEventArgs e);

        // Metody do obsługi akcji kopiowania i wklejania
        void HandlePaste();
        void HandleCopy();

        // Metoda do ustawienia bieżącej akcji edycji
        void SetCurrentAction(EditAction action);
    }
}
