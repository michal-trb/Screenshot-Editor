using Helpers.DpiHelper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using screenerWpf.Interfaces;
using screenerWpf.Properties;
using System;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace screenerWpf.Controls
{
    public class CanvasSavingHandler : ICanvasSavingHandler
    {
        private DrawableCanvas drawableCanvas;

        public CanvasSavingHandler(DrawableCanvas canvas)
        {
            drawableCanvas = canvas;
        }

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

        private void SaveImageToFile(WriteableBitmap editedImage, string filename)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Create))
            {
                BitmapEncoder encoder = GetBitmapEncoder(Path.GetExtension(filename));
                encoder.Frames.Add(BitmapFrame.Create(editedImage));
                encoder.Save(fileStream);
            }
        }

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
}
