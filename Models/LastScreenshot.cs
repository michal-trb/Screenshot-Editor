namespace screenerWpf.Models;

using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// <summary>
/// Represents information about a screenshot, including its file path, file name, and thumbnail image.
/// </summary>
public class LastScreenshot
{
    /// <summary>
    /// Gets or sets the file path of the screenshot.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Gets or sets the file name of the screenshot.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail image of the screenshot.
    /// </summary>
    public ImageSource Thumbnail { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LastScreenshot"/> class.
    /// Loads the screenshot information from the provided file path.
    /// </summary>
    /// <param name="filePath">The full path of the screenshot file.</param>
    public LastScreenshot(string filePath)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
        Thumbnail = LoadThumbnail(filePath);
    }

    /// <summary>
    /// Loads the thumbnail image for the screenshot.
    /// </summary>
    /// <param name="filePath">The file path of the screenshot.</param>
    /// <returns>An <see cref="ImageSource"/> representing the thumbnail.</returns>
    private ImageSource LoadThumbnail(string filePath)
    {
        BitmapImage thumb = new BitmapImage();
        thumb.BeginInit();
        thumb.UriSource = new Uri(filePath);
        thumb.DecodePixelWidth = 50;
        thumb.EndInit();
        return thumb;
    }
}
