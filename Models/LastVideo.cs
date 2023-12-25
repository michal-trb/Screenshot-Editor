using System.Windows.Media.Imaging;
using System.Windows.Media;
using System;
using System.IO;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System.Windows.Interop;
using System.Drawing;
using System.Drawing.Imaging;

namespace screenerWpf.Models
{
    public class LastVideo
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public ImageSource Thumbnail { get; set; }

        public LastVideo(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            Thumbnail = LoadThumbnail(filePath);
        }

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
            File.Delete(outputFile.Filename); // Usuwa plik tymczasowy

            return bitmapSource;
        }
    }
}
