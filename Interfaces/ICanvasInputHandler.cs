using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf.Interfaces
{
    public interface ICanvasInputHandler
    {
        void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e);
        void Canvas_MouseDoubleClick(object sender, MouseButtonEventArgs e);
        void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e);
        void Canvas_MouseMove(object sender, MouseEventArgs e);
        void DrawArrow();
        void DrawRect();
        void AddText();
        void Save();
        void SpeechBubble();
        void Blur();
        void Brush();
        void RecognizeText();
        void EditText();
        void ChangeFontFamily(FontFamily selectedFontFamily);
        void ChangeFontSize(double fontSize);
        void ChangeColor(Color color);
        void ChangeArrowThickness(double thickness);
        void ChangeTransparency(double transparency);
        string SaveFast();
        void SavePdf();
        void CommandBinding_DeleteExecuted();
        void CommandBinding_CopyExecuted();
        void CommandBinding_PasteExecuted();
    }
}
