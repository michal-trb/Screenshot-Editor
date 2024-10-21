namespace screenerWpf.Controls;

using global::Helpers.DpiHelper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using screenerWpf.Interfaces;
using screenerWpf.Properties;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;


/// <summary>
/// Handles the saving of canvas content to different file formats such as PNG, JPEG, BMP, and PDF.
/// </summary>
public class CanvasSavingHandler : ICanvasSavingHandler
{
    private DrawableCanvas drawableCanvas;

    /// <summary>
    /// Initializes a new instance of the <see cref="CanvasSavingHandler"/> class.
    /// </summary>
    /// <param name="canvas">The drawable canvas to be saved.</param>
    public CanvasSavingHandler(DrawableCanvas canvas)
    {
        drawableCanvas = canvas;
    }

    /// <summary>
    /// Opens a dialog to save the canvas content to a file in a user-selected format (PNG, JPEG, BMP).
    /// </summary>
    public void SaveCanvasToFile()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Image_{timestamp}";
        var saveFileDialog = new SaveFileDialog
        {
            FileName = fileName,
            DefaultExt = ".png",
            Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|BMP Files (*.bmp)|*.bmp",
            InitialDirectory = Settings.Default.ScreenshotsSavePath
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            WriteableBitmap editedImage = GetEditedImageBitmap();
            SaveImageToFile(editedImage, saveFileDialog.FileName);
        }
    }

    /// <summary>
    /// Saves the canvas content directly to a file without opening a dialog, using a default path and a timestamped file name.
    /// </summary>
    /// <returns>The full path to the saved image file.</returns>
    public string SaveCanvasToFileFast()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Image_{timestamp}.png";
        string defaultPath = Settings.Default.ScreenshotsSavePath;
        string fullPath = Path.Combine(defaultPath, fileName);

        WriteableBitmap editedImage = GetEditedImageBitmap();
        SaveImageToFile(editedImage, fullPath);

        return fullPath;
    }

    /// <summary>
    /// Generates a <see cref="WriteableBitmap"/> from the current state of the canvas.
    /// </summary>
    /// <returns>A bitmap representation of the edited canvas.</returns>
    private WriteableBitmap GetEditedImageBitmap()
    {
        var currentDpi = DpiHelper.CurrentDpi;

        var renderBitmap = new RenderTargetBitmap(
            (int)drawableCanvas.ActualWidth,
            (int)drawableCanvas.ActualHeight,
            currentDpi.DpiX,
            currentDpi.DpiY,
            PixelFormats.Pbgra32);

        renderBitmap.Render(drawableCanvas);

        return new WriteableBitmap(renderBitmap);
    }

    /// <summary>
    /// Saves the given image to a file using the specified filename.
    /// </summary>
    /// <param name="editedImage">The image to be saved.</param>
    /// <param name="filename">The file path where the image will be saved.</param>
    private void SaveImageToFile(WriteableBitmap editedImage, string filename)
    {
        using (FileStream fileStream = new FileStream(filename, FileMode.Create))
        {
            BitmapEncoder encoder = GetBitmapEncoder(Path.GetExtension(filename));
            encoder.Frames.Add(BitmapFrame.Create(editedImage));
            encoder.Save(fileStream);
        }
    }

    /// <summary>
    /// Gets the appropriate bitmap encoder based on the file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension of the image.</param>
    /// <returns>A <see cref="BitmapEncoder"/> suitable for the specified file extension.</returns>
    private BitmapEncoder GetBitmapEncoder(string fileExtension)
    {
        switch (fileExtension.ToLower())
        {
            case ".jpg":
            case ".jpeg":
                return new JpegBitmapEncoder();
            case ".bmp":
                return new BmpBitmapEncoder();
            default:
                return new PngBitmapEncoder();
        }
    }

    /// <summary>
    /// Opens a dialog to save the canvas content as a PDF file.
    /// </summary>
    public void SaveCanvasToPdfFile()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Image_{timestamp}";
        var saveFileDialog = new SaveFileDialog
        {
            FileName = fileName,
            DefaultExt = ".pdf",
            Filter = "PDF Files (*.pdf)|*.pdf",
            InitialDirectory = Settings.Default.ScreenshotsSavePath
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            WriteableBitmap editedImage = GetEditedImageBitmap();
            SaveImageToPdf(editedImage, saveFileDialog.FileName);
        }
    }

    /// <summary>
    /// Saves the given image as a PDF file.
    /// </summary>
    /// <param name="editedImage">The image to be saved as a PDF.</param>
    /// <param name="filename">The file path where the PDF will be saved.</param>
    private void SaveImageToPdf(WriteableBitmap editedImage, string filename)
    {
        using (var stream = new FileStream(filename, FileMode.Create))
        {
            var pageSize = new Rectangle(0, 0, editedImage.PixelWidth, editedImage.PixelHeight);

            Document document = new Document(pageSize, 0, 0, 0, 0);
            PdfWriter.GetInstance(document, stream);
            document.Open();

            var image = WriteableBitmapToPdfImage(editedImage);
            image.ScaleToFit(pageSize.Width, pageSize.Height);

            document.Add(image);

            document.Close();
        }
    }

    /// <summary>
    /// Converts a <see cref="WriteableBitmap"/> to an iTextSharp image that can be added to a PDF.
    /// </summary>
    /// <param name="writeableBitmap">The bitmap to be converted.</param>
    /// <returns>An <see cref="iTextSharp.text.Image"/> instance representing the bitmap.</returns>
    private Image WriteableBitmapToPdfImage(WriteableBitmap writeableBitmap)
    {
        using (var stream = new MemoryStream())
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(writeableBitmap));
            encoder.Save(stream);
            stream.Position = 0;

            return Image.GetInstance(stream.ToArray());
        }
    }
}
