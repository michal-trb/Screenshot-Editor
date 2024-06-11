using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace screenerWpf.Models
{
    public class LastScreenshot
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public ImageSource Thumbnail { get; set; }

        public LastScreenshot(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            Thumbnail = LoadThumbnail(filePath);
        }

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
}
