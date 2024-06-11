using screenerWpf.Interfaces;
using screenerWpf.Views;
using System;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Schema;

namespace screenerWpf.Sevices
{
    public class WindowService : IWindowService
    {
        private readonly IImageEditorWindowFactory imageEditorWindowFactory;
        public MainViewModel MainViewModel { get; set; }

        public WindowService(IImageEditorWindowFactory imageEditorWindowFactory)
        {
            this.imageEditorWindowFactory = imageEditorWindowFactory;
        }

        public void ShowImageEditorWindow(BitmapSource image)
        {
            var editor = imageEditorWindowFactory.Create(image);
            editor.WindowClosed += OnImageEditorWindowClosed;
            editor.ShowDialog();
        }

        private void OnImageEditorWindowClosed()
        {
            MainViewModel?.BringMainWindowToFront();
        }

        public Rectangle SelectArea()
        {
            AreaSelector selector = new AreaSelector();
            bool? result = selector.ShowDialog();

            if (result == true)
            {
                double dpiX, dpiY;
                using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    dpiX = graphics.DpiX / 96.0;
                    dpiY = graphics.DpiY / 96.0;
                }

                var scaledRect = new Rectangle(
                    (int)((selector.SelectedRectangle.X) * dpiX),
                    (int)((selector.SelectedRectangle.Y) * dpiY),
                    (int)(selector.SelectedRectangle.Width * dpiX),
                    (int)(selector.SelectedRectangle.Height * dpiY));

                return scaledRect;
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
