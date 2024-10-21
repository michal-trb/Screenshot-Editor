namespace screenerWpf.Sevices;

using screenerWpf.Controls;
using screenerWpf.Helpers;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using Tesseract;

/// <summary>
/// Class responsible for handling text recognition (OCR) from images on the drawable canvas.
/// </summary>
public class TextRecognitionHandler
{
    private DrawableCanvas canvas;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRecognitionHandler"/> class.
    /// </summary>
    /// <param name="canvas">The <see cref="DrawableCanvas"/> instance that contains the image data.</param>
    public TextRecognitionHandler(DrawableCanvas canvas)
    {
        this.canvas = canvas;
    }

    /// <summary>
    /// Starts the text recognition process for the current image on the canvas.
    /// The recognized text is then copied to the system clipboard.
    /// </summary>
    internal void StartRecognizeFromImage()
    {
        var canvasBitmap = CanvasBitmapGenerator.GetCanvasBitmap(canvas);
        if (canvasBitmap != null)
        {
            string recognizedText = RecognizeTextFromImage(canvasBitmap);
            Clipboard.SetText(recognizedText);
        }
    }

    /// <summary>
    /// Recognizes text from a given <see cref="BitmapSource"/> using Tesseract OCR.
    /// </summary>
    /// <param name="bitmapSource">The <see cref="BitmapSource"/> to process for text recognition.</param>
    /// <returns>A string containing the recognized text.</returns>
    public string RecognizeTextFromImage(BitmapSource bitmapSource)
    {
        // Convert BitmapSource to Bitmap
        Bitmap bitmap;
        using (var outStream = new System.IO.MemoryStream())
        {
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapSource));
            enc.Save(outStream);
            bitmap = new Bitmap(outStream);
        }

        // Use Tesseract to recognize text
        using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
        {
            using (var img = PixConverter.ToPix(bitmap))
            {
                using (var page = engine.Process(img))
                {
                    return page.GetText();
                }
            }
        }
    }
}