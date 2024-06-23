using System.Windows.Media.Imaging;

namespace screenerWpf.Interfaces
{
    public interface IImageEditorControlFactory
    {
        ImageEditorControl Create(BitmapSource initialBitmap);
    }
}
