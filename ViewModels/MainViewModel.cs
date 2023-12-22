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
            string screenshotsDirectory = "C:\\Users\\xmich\\Pictures\\Screenpresso"; // Ścieżka do folderu ze screenshotami
            var screenshotFiles = Directory.GetFiles(screenshotsDirectory, "*.png"); // Zakładając, że screenshoty są w formacie PNG

            foreach (var file in screenshotFiles)
            {
                LastScreenshots.Add(new LastScreenshot(file));
            }
        }

        public void OpenScreenshot(string filePath)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer", filePath);
            }
            catch (Exception ex)
            {
                // Obsługa błędów, np. wyświetlenie komunikatu
                MessageBox.Show($"Nie można otworzyć pliku: {ex.Message}", "Błąd");
            }
        }

    }
}