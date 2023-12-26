using screenerWpf.Interfaces;
using System.Windows.Media.Imaging;

namespace screenerWpf.Factories
{
    public class ImageEditorWindowFactory : IImageEditorWindowFactory
    {
        public ImageEditorWindow Create(BitmapSource initialBitmap)
        { 
            return new ImageEditorWindow(initialBitmap);
        }
    }
}
