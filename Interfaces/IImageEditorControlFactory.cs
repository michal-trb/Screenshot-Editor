namespace screenerWpf.Interfaces;

using System.Windows.Media.Imaging;

/// <summary>
/// Interface for creating instances of <see cref="ImageEditorControl"/>.
/// </summary>
public interface IImageEditorControlFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="ImageEditorControl"/> using the provided initial bitmap.
    /// </summary>
    /// <param name="initialBitmap">The initial bitmap to be used in the image editor.</param>
    /// <returns>A new instance of <see cref="ImageEditorControl"/>.</returns>
    ImageEditorControl Create(BitmapSource initialBitmap);
}