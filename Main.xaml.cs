using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop; // Added for Imaging.CreateBitmapSourceFromHBitmap
using System.IO;
using System.Management;
using System.Reflection;

namespace screenerWpf
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCaptureFull_Click(object sender, RoutedEventArgs e)
        {
            using (Bitmap bitmap = CaptureScreen())
            {
                SaveScreenshot(bitmap);
            }
        }

        private void SaveScreenshot(Bitmap bitmap)
        {
            ImageEditorWindow editor = new ImageEditorWindow(ConvertBitmapToBitmapSource(bitmap));
            editor.ShowDialog();
        }

        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
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

        private void BtnCaptureArea_Click(object sender, RoutedEventArgs e)
        {
            AreaSelector selector = new AreaSelector();
            bool? result = selector.ShowDialog();
            if (result == true)
            {
                // Pobranie skalowania DPI
                var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
                var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
                var dpiX = (int)dpiXProperty.GetValue(null, null);
                var dpiY = (int)dpiYProperty.GetValue(null, null);

                // Przeliczenie na rzeczywiste piksele
                var area = new System.Drawing.Rectangle(
                    (int)(selector.SelectedRectangle.X / (96d / dpiX)),
                    (int)(selector.SelectedRectangle.Y / (96d / dpiY)),
                    (int)(selector.SelectedRectangle.Width / (96d / dpiX)),
                    (int)(selector.SelectedRectangle.Height / (96d / dpiY)));

                using (Bitmap bitmap = CaptureSelectedArea(area))
                {
                    SaveScreenshot(bitmap);
                }
            }
            selector.Close();
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
    }
}
