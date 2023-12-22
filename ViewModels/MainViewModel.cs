using System.Windows.Input;
using System.Drawing; // dla operacji na Bitmap
using System.Windows.Media.Imaging; // dla BitmapSource
using screenerWpf.Commands;
using System;
using screenerWpf.Interfaces;
using screenerWpf.Sevices;
using System.IO;
using screenerWpf.Controls;
using System.Windows.Controls;
using screenerWpf.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;

namespace screenerWpf
{
    public class MainViewModel
    {
        private readonly IScreenCaptureService screenCaptureService;
        private readonly IWindowService windowService;
        public ObservableCollection<LastScreenshot> LastScreenshots { get; private set; }


        public ICommand MinimizeCommand { get; private set; }
        public ICommand MaximizeRestoreCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand CaptureFullCommand { get; private set; }
        public ICommand CaptureAreaCommand { get; private set; }

        public event Action MinimizeRequest;
        public event Action MaximizeRestoreRequest;
        public event Action CloseRequest;

        public MainViewModel(IScreenCaptureService screenCaptureService, IWindowService windowService)
        {
            this.screenCaptureService = screenCaptureService;
            this.windowService = windowService;
            CaptureFullCommand = new RelayCommand(ExecuteCaptureFull);
            CaptureAreaCommand = new RelayCommand(ExecuteCaptureArea);
            MinimizeCommand = new RelayCommand(o => MinimizeRequest?.Invoke());
            MaximizeRestoreCommand = new RelayCommand(o => MaximizeRestoreRequest?.Invoke());
            CloseCommand = new RelayCommand(o => CloseRequest?.Invoke());
            LastScreenshots = new ObservableCollection<LastScreenshot>();
            LoadLastScreenshots();
        }
        private void ExecuteCaptureFull(object parameter)
        {
            Bitmap bitmap = screenCaptureService.CaptureScreen();
            ShowEditorWindow(bitmap);
        }

        private void ExecuteCaptureArea(object parameter)
        {
            Rectangle area = windowService.SelectArea();
            if (!area.IsEmpty)
            {
                Bitmap bitmap = screenCaptureService.CaptureArea(area);
                ShowEditorWindow(bitmap);
            }
        }

        private void ShowEditorWindow(Bitmap bitmap)
        {
            BitmapSource bitmapSource = ConvertBitmapToBitmapSource(bitmap);
            this.windowService.ShowImageEditorWindow(bitmapSource);
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

        private void LoadLastScreenshots()
        {
            string screenshotsDirectory = "C:\\Users\\xmich\\Pictures\\Screenpresso";
            DirectoryInfo di = new DirectoryInfo(screenshotsDirectory);
            var screenshotFiles = di.GetFiles("*.png").OrderByDescending(f => f.LastWriteTime).Take(6);

            foreach (var file in screenshotFiles)
            {
                LastScreenshots.Add(new LastScreenshot(file.FullName));
            }
        }

        public void OpenScreenshot(string filePath)
        {
            try
            {
                var bitmapImage = new BitmapImage(new Uri(filePath));
                this.windowService.ShowImageEditorWindow(bitmapImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można otworzyć pliku: {ex.Message}", "Błąd");
            }
        }


    }
}