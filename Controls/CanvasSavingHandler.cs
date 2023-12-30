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
                FileName = fileName, // Domyślna nazwa pliku
                DefaultExt = ".png", // Domyślne rozszerzenie pliku
                Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|BMP Files (*.bmp)|*.bmp", // Filtr plików,
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
            string fileName = $"Image_{timestamp}.png"; // Ustawienie nazwy pliku z rozszerzeniem
            string defaultPath = Settings.Default.ScreenshotsSavePath; // Ścieżka domyślna
            string fullPath = Path.Combine(defaultPath, fileName); // Pełna ścieżka do zapisu pliku

            WriteableBitmap editedImage = GetEditedImageBitmap();
            SaveImageToFile(editedImage, fullPath);

            return fullPath;
        }

        private WriteableBitmap GetEditedImageBitmap()
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)drawableCanvas.ActualWidth,
                (int)drawableCanvas.ActualHeight,
                96d, 96d, PixelFormats.Pbgra32);

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
                FileName = fileName, // Domyślna nazwa pliku
                DefaultExt = ".pdf", // Domyślne rozszerzenie pliku
                Filter = "PDF Files (*.pdf)|*.pdf", // Filtr plików
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
                // Oblicz rozmiar strony PDF na podstawie rozmiaru obrazu
                var pageSize = new Rectangle(0, 0, editedImage.PixelWidth, editedImage.PixelHeight);

                // Utwórz dokument PDF z określonym rozmiarem strony
                Document document = new Document(pageSize, 0, 0, 0, 0);
                PdfWriter.GetInstance(document, stream);
                document.Open();

                var image = WriteableBitmapToPdfImage(editedImage);
                // Dopasuj rozmiar obrazu do rozmiaru strony
                image.ScaleToFit(pageSize.Width, pageSize.Height);

                document.Add(image);

                document.Close();
            }
        }

        private Image WriteableBitmapToPdfImage(WriteableBitmap writeableBitmap)
        {
            // Konwersja WriteableBitmap na Image iTextSharp
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
