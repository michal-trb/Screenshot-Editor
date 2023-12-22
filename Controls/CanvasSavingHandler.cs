using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using screenerWpf.Interfaces;

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
            var saveFileDialog = new SaveFileDialog
            {
                FileName = "Image", // Domyślna nazwa pliku
                DefaultExt = ".png", // Domyślne rozszerzenie pliku
                Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|BMP Files (*.bmp)|*.bmp" // Filtr plików
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                WriteableBitmap editedImage = GetEditedImageBitmap();
                SaveImageToFile(editedImage, saveFileDialog.FileName);
            }
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
    }
}
