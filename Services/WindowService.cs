namespace screenerWpf.Sevices;

using global::Helpers.DpiHelper;
using screenerWpf.Interfaces;
using screenerWpf.Views;
using System.Drawing;

public class WindowService : IWindowService
{
    public WindowService()
    {
    }

        public Rectangle SelectArea()
    {
        AreaSelector selector = new AreaSelector();

        bool? result = selector.ShowDialog();

        if (result == true)
        {
            DpiHelper.UpdateDpi();
            var currentDpi = DpiHelper.CurrentDpi;
            double dpiX = currentDpi.DpiX / 96.0;
            double dpiY = currentDpi.DpiY / 96.0;

            var scaledRect = new Rectangle(
                (int)(selector.SelectedRectangle.X * dpiX),
                (int)(selector.SelectedRectangle.Y * dpiY),
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
