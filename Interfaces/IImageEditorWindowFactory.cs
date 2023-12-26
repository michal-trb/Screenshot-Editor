using System.Windows.Media.Imaging;

namespace screenerWpf.Interfaces
{
    public interface IImageEditorWindowFactory
    {
        ImageEditorWindow Create(BitmapSource initialBitmap, ICloudStorageUploader googleDriveUploader);
    }
}
