using System.Drawing;
using System.Windows.Media.Imaging;

namespace screenerWpf.Interfaces
{
    public interface IWindowService
    {
        Rectangle SelectArea();
        void ShowImageEditorWindow(BitmapSource image, ICloudStorageUploader uploader);
        void ShowVideoPlayerWindow(string videoPath);

    }
}
