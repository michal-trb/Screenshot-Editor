namespace screenerWpf.Sevices.CaptureServices;

using global::Helpers.DpiHelper;
using System.Drawing;
using System.Windows;
using Size = System.Drawing.Size;

public class FullScreenshot
{
    public static Bitmap CaptureScreen()
    {
        // Uzyskanie współczynnika DPI
        var presentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);

        DpiHelper.UpdateDpi();
        var currentDpi = DpiHelper.CurrentDpi;

        int screenWidth = (int)(SystemParameters.PrimaryScreenWidth * currentDpi.DpiX / 96);
        int screenHeight = (int)(SystemParameters.PrimaryScreenHeight * currentDpi.DpiY / 96);

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
