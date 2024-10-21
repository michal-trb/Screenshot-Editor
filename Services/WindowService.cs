namespace screenerWpf.Sevices;

using global::Helpers.DpiHelper;
using screenerWpf.Interfaces;
using screenerWpf.Views;
using System.Drawing;

/// <summary>
/// Provides services related to window actions, including area selection and showing video player windows.
/// </summary>
public class WindowService : IWindowService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowService"/> class.
    /// </summary>
    public WindowService() { }

    /// <summary>
    /// Allows the user to select an area of the screen and returns its coordinates as a <see cref="Rectangle"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Rectangle"/> representing the selected area. Returns <see cref="Rectangle.Empty"/> if no area is selected.
    /// </returns>
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

    /// <summary>
    /// Opens a new window to play the specified video file.
    /// </summary>
    /// <param name="videoPath">The file path to the video that should be played.</param>
    public void ShowVideoPlayerWindow(string videoPath)
    {
        VideoPlayerWindow playerWindow = new VideoPlayerWindow(videoPath);
        playerWindow.Show();
    }
}
