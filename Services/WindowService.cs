using screenerWpf.Interfaces;
using screenerWpf.Views;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Schema;

namespace screenerWpf.Sevices
{
    public class WindowService : IWindowService
    {
        private readonly IImageEditorWindowFactory imageEditorWindowFactory;

        public WindowService(IImageEditorWindowFactory imageEditorWindowFactory)
        {
            this.imageEditorWindowFactory = imageEditorWindowFactory;
        }

        public void ShowImageEditorWindow(BitmapSource image)
        {
            var editor = imageEditorWindowFactory.Create(image);
            editor.ShowDialog();
        }

        public Rectangle SelectArea()
        {
            AreaSelector selector = new AreaSelector();
            selector.Cursor = Cursors.Cross; // Change cursor to crosshair
            bool? result = selector.ShowDialog();

            if (result == true)
            {
                // The selected area is now highlighted with a semi-transparent overlay.
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
