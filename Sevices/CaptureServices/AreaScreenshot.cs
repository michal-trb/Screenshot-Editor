using System.Drawing;

namespace screenerWpf.Sevices.CaptureServices
{
    public class AreaScreenshot
    {
        public static Bitmap CaptureArea(Rectangle area)
        {
            // Tworzenie bitmapy o wymiarach określonego obszaru
            Bitmap bitmap = new Bitmap(area.Width, area.Height);

            // Użycie Graphics do skopiowania wybranego obszaru ekranu na bitmapę
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(area.Location, Point.Empty, area.Size);
            }

            return bitmap;
        }
    }
}
