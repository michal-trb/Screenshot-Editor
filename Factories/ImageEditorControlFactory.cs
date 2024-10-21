namespace screenerWpf.Factories;

using screenerWpf.Interfaces;
using System.Windows.Media.Imaging;

/// <summary>
/// Factory class responsible for creating instances of the <see cref="ImageEditorControl"/>.
/// This factory allows for consistent and centralized creation of the image editor control.
/// </summary>
public class ImageEditorControlFactory : IImageEditorControlFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref="ImageEditorControl"/> with the specified initial bitmap.
    /// </summary>
    /// <param name="initialBitmap">The initial bitmap to be loaded into the image editor control.</param>
    /// <returns>A new instance of the <see cref="ImageEditorControl"/> initialized with the provided bitmap.</returns>
    public ImageEditorControl Create(BitmapSource initialBitmap)
    {
        return new ImageEditorControl(initialBitmap);
    }
}
