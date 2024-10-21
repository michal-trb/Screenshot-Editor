namespace screenerWpf.Interfaces;

using System.Drawing;

/// <summary>
/// Provides services for managing windows, including area selection and showing video player windows.
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// Allows the user to select an area of the screen.
    /// </summary>
    /// <returns>
    /// A <see cref="Rectangle"/> representing the selected area.
    /// </returns>
    Rectangle SelectArea();

    /// <summary>
    /// Displays a window for playing the specified video.
    /// </summary>
    /// <param name="videoPath">The path to the video file to be played.</param>
    void ShowVideoPlayerWindow(string videoPath);
}
