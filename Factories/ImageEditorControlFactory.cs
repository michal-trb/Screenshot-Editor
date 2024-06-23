using screenerWpf.Interfaces;
using System.Windows.Media.Imaging;

namespace screenerWpf.Factories
{
    public class ImageEditorControlFactory : IImageEditorControlFactory
    {
        public ImageEditorControl Create(BitmapSource initialBitmap)
        { 
            return new ImageEditorControl(initialBitmap);
        }
    }
}
