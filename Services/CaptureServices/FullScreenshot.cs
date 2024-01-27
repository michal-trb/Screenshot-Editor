using System.Drawing;
using System.Windows;
using Size = System.Drawing.Size;

namespace screenerWpf.Sevices.CaptureServices
{
    public class FullScreenshot
    {
        public static Bitmap CaptureScreen()
        {
            // Uzyskanie współczynnika DPI
            var presentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);

            double dpiX = 96; // Domyślne DPI
            double dpiY = 96; // Domyślne DPI

            if (presentationSource != null)
            {
                dpiX = presentationSource.CompositionTarget.TransformToDevice.M11 * 96;
                dpiY = presentationSource.CompositionTarget.TransformToDevice.M22 * 96;
            }
            // Uzyskanie rozmiarów ekranu głównego z uwzględnieniem DPI
            int screenWidth = (int)(SystemParameters.PrimaryScreenWidth * dpiX / 96);
            int screenHeight = (int)(SystemParameters.PrimaryScreenHeight * dpiY / 96);

            // Tworzenie bitmapy o tych rozmiarach
            Bitmap bitmap = new Bitmap(screenWidth, screenHeight);

            // Rysowanie zrzutu ekranu na bitmapie
            using (Graphics g = Graphics.FromImage(bitmap))
            {

                g.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
            }

            return bitmap;
        }
    }
}
