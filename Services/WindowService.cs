using screenerWpf.Interfaces;
using screenerWpf.Views;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace screenerWpf.Sevices
{
    public class WindowService : IWindowService
    {
        private readonly IImageEditorWindowFactory imageEditorWindowFactory;

        public WindowService(IImageEditorWindowFactory imageEditorWindowFactory)
        {
            this.imageEditorWindowFactory = imageEditorWindowFactory;
        }

        public void ShowImageEditorWindow(BitmapSource image, ICloudStorageUploader uploader)
        {
            var editor = imageEditorWindowFactory.Create(image, uploader);
            editor.ShowDialog();
        }

        public Rectangle SelectArea()
        {
            AreaSelector selector = new AreaSelector();
            bool? result = selector.ShowDialog();
            if (result == true)
            {
                return new Rectangle(
                    (int)selector.SelectedRectangle.X,
                    (int)selector.SelectedRectangle.Y,
                    (int)selector.SelectedRectangle.Width,
                    (int)selector.SelectedRectangle.Height);
            }
            return Rectangle.Empty;
        }

        public void ShowVideoPlayerWindow(string videoPath)
        {
            VideoPlayerWindow playerWindow = new VideoPlayerWindow(videoPath);
            playerWindow.Show();
        }
    }
}
