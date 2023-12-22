using System.Windows;
using System.Windows.Controls;
using screenerWpf.Models.DrawableElements;

namespace screenerWpf.Interfaces
{
    public interface ICanvasEditingHandler
    {
        // Rozpoczęcie edycji danego elementu
        void StartEditing(IDrawable element, Point location);

        // Zakończenie edycji i zapisanie zmian
        void FinishEditing(TextBox textBox);

        // Dodatkowe metody, które mogą być potrzebne
        // ...
    }
}
