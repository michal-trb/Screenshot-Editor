using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop; // Added for Imaging.CreateBitmapSourceFromHBitmap
using System.IO;
using System.Management;

namespace screenerWpf
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }

        private void BtnCaptureFull_Click(object sender, RoutedEventArgs e)
        {
            using (Bitmap bitmap = CaptureScreen())
            {
                SaveScreenshot(bitmap);
            }
        }

        private void BtnCaptureArea_Click(object sender, RoutedEventArgs e)
        {
            AreaSelector selector = new AreaSelector();
            bool? result = selector.ShowDialog();
            if (result == true)
            {
                var area = new System.Drawing.Rectangle((int)selector.SelectedRectangle.X,
                                                                        (int)selector.SelectedRectangle.Y,
                                                                        (int)selector.SelectedRectangle.Width,
                                                                        (int)selector.SelectedRectangle.Height);
                using (Bitmap bitmap = CaptureSelectedArea(area))
                {
                    SaveScreenshot(bitmap);
                }
            }
            selector.Close();
        }

        private void SaveScreenshot(Bitmap bitmap)
        {
            ImageEditorWindow editor = new ImageEditorWindow(ConvertBitmapToBitmapSource(bitmap));
            editor.ShowDialog();
        }

        public Bitmap CaptureScreen()
        {
            ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_VideoController");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string currentResolution = obj["CurrentHorizontalResolution"].ToString() + "x" + obj["CurrentVerticalResolution"].ToString();

                    // Przetwarzanie uzyskanej rozdzielczości
                    int screenWidth = Convert.ToInt32(obj["CurrentHorizontalResolution"]);
                    int screenHeight = Convert.ToInt32(obj["CurrentVerticalResolution"]);

                    Bitmap bitmap = new Bitmap(screenWidth, screenHeight);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(screenWidth, screenHeight));
                    }
                    return bitmap;
                }
            }

            return null; // Jeśli nie uda się uzyskać informacji o rozdzielczości
        }
        private Bitmap CaptureSelectedArea(System.Drawing.Rectangle area)
        {
            Bitmap bitmap = new Bitmap(area.Width, area.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(area.Location, System.Drawing.Point.Empty, area.Size);
            }
            return bitmap;
        }

        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
