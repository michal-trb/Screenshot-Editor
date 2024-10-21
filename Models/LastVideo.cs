namespace screenerWpf.Models;

using System.Windows.Media.Imaging;
using System.Windows.Media;
using System;
using System.IO;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System.Windows.Interop;
using System.Drawing;

/// <summary>
/// Represents a video file, including its file path, file name, and a thumbnail image extracted from the video.
/// </summary>
public class LastVideo
{
    /// <summary>
    /// Gets or sets the file path of the video.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Gets or sets the file name of the video.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail image extracted from the video.
    /// </summary>
    public ImageSource Thumbnail { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LastVideo"/> class.
    /// Loads the video information and attempts to generate a thumbnail from the provided file path.
    /// </summary>
    /// <param name="filePath">The full path of the video file.</param>
    public LastVideo(string filePath)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
        try
        {
            Thumbnail = LoadThumbnail(filePath);
        }
        catch
        {
            // Handle exceptions silently.
        }
    }

    /// <summary>
    /// Loads the thumbnail image from the video file using the MediaToolkit library.
    /// </summary>
    /// <param name="filePath">The full path of the video file.</param>
    /// <returns>An <see cref="ImageSource"/> representing the thumbnail image extracted from the video.</returns>
    private ImageSource LoadThumbnail(string filePath)
    {
        var inputFile = new MediaFile { Filename = filePath };
        var outputFile = new MediaFile { Filename = $"{Path.GetTempPath()}{Guid.NewGuid()}.jpg" };

        using (var engine = new Engine())
        {
            var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(0) };
            engine.GetThumbnail(inputFile, outputFile, options);
        }

        var bitmap = new Bitmap(outputFile.Filename);
        var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
            bitmap.GetHbitmap(),
            IntPtr.Zero,
            System.Windows.Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        bitmap.Dispose();
        File.Delete(outputFile.Filename); // Deletes the temporary file.

        return bitmapSource;
    }
}
