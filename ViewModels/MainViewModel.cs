using System.Windows.Input;
using System.Drawing; // dla operacji na Bitmap
using System.Windows.Media.Imaging; // dla BitmapSource
using screenerWpf.Commands;
using System;
using screenerWpf.Interfaces;
using System.IO;
using screenerWpf.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;
using screenerWpf.Views;
using screenerWpf.Properties;

namespace screenerWpf
{
    public class MainViewModel
    {
        private readonly IScreenCaptureService screenCaptureService;
        private readonly IWindowService windowService;
        public ObservableCollection<LastScreenshot> LastScreenshots { get; private set; }
        public ObservableCollection<LastVideo> LastVideos { get; private set; }

        public ICommand CaptureFullCommand { get; private set; }
        public ICommand CaptureAreaCommand { get; private set; }
        public ICommand CaptureWindowScrollCommand { get; private set; }
        public ICommand CaptureWindowCommand { get; private set; }
        public ICommand RecordVideoCommand { get; private set; }
        public ICommand RecordAreaVideoCommand { get; private set; }
        public StopRecordingWindow stopRecordingWindow { get; private set; }


        public MainViewModel(IScreenCaptureService screenCaptureService, IWindowService windowService)
        {
            this.screenCaptureService = screenCaptureService;
            this.windowService = windowService;
            CaptureFullCommand = new RelayCommand(ExecuteCaptureFull);
            CaptureAreaCommand = new RelayCommand(ExecuteCaptureArea);
            CaptureWindowScrollCommand = new RelayCommand(ExecuteCaptureWindowScroll);
            CaptureWindowCommand = new RelayCommand(ExecuteCaptureWindow);
            RecordVideoCommand = new RelayCommand(ExecuteRecordVideo);
            RecordAreaVideoCommand = new RelayCommand(ExecuteAreaRecordVideo);
            LastScreenshots = new ObservableCollection<LastScreenshot>();
            LastVideos = new ObservableCollection<LastVideo>();
            LoadLastScreenshots();
            LoadLastVideos();
        }
        private void ExecuteCaptureFull(object parameter)
        {
            Bitmap bitmap = screenCaptureService.CaptureScreen();
            ShowEditorWindow(bitmap);
        }

        private void ExecuteCaptureWindowScroll(object parameter)
        {
            screenCaptureService.CaptureWithScrollAsync();
        }

        private void ExecuteCaptureWindow(object parameter)
        {
            screenCaptureService.CaptureWindow();
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
            string screenshotsDirectory = Settings.Default.ScreenshorsLibrary;
            DirectoryInfo di = new DirectoryInfo(screenshotsDirectory);
            var screenshotFiles = di.GetFiles("*.png").OrderByDescending(f => f.LastWriteTime).Take(5);

            foreach (var file in screenshotFiles)
            {
                LastScreenshots.Add(new LastScreenshot(file.FullName));
            }
        }
        private void LoadLastVideos()
        {
            string recordsDirectory = Settings.Default.RecordsSavePath;
            DirectoryInfo di = new DirectoryInfo(recordsDirectory);
            var screenshotFiles = di.GetFiles("*.mp4").OrderByDescending(f => f.LastWriteTime).Take(5);

            foreach (var file in screenshotFiles)
            {
                LastVideos.Add(new LastVideo(file.FullName));
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
        public void OpenVideo(string filePath)
        {
            try
            {
                this.windowService.ShowVideoPlayerWindow(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można otworzyć pliku: {ex.Message}", "Błąd");
            }
        }
        private void ExecuteRecordVideo(object parameter)
        {
            // Rozpoczęcie nagrywania
            screenCaptureService.StartRecording();
            ShowStopRecordingButton();
        }

        private void ExecuteAreaRecordVideo(object parameter)
        {
            var area = windowService.SelectArea();
            if (!area.IsEmpty)
            {
                screenCaptureService.StartAreaRecording(area);
                ShowStopRecordingButton();
            }
        }

        public void StopRecording()
        {
            screenCaptureService.StopRecording();
            HideStopRecordingButton();
        }

        private void ShowStopRecordingButton()
        {
            stopRecordingWindow = new StopRecordingWindow();
            stopRecordingWindow.Show();
        }

        private void HideStopRecordingButton()
        {
            if (stopRecordingWindow != null)
            {
                stopRecordingWindow.Close();
                stopRecordingWindow = null;
            }
        }
    }
}