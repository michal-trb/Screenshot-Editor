using System.Windows;
using System.Windows.Input;
using screenerWpf.Models.DrawableElements;

namespace screenerWpf.Interfaces
{
    public interface ICanvasSelectionHandler
    {
        // Obsługa kliknięcia lewym przyciskiem myszy
        void HandleLeftButtonDown(MouseButtonEventArgs e);

        // Obsługa ruchu myszy
        void HandleMouseMove(MouseEventArgs e);

        // Obsługa zwolnienia lewego przycisku myszy
        void HandleLeftButtonUp(MouseButtonEventArgs e);

        // Usunięcie zaznaczonego elementu
        void DeleteSelectedElement();

        // Sprawdzenie, czy jakikolwiek element jest zaznaczony
        bool HasSelectedElement();

        // Pobranie zaznaczonego elementu
        IDrawable GetSelectedElement();
    }
}
