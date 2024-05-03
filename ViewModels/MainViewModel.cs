using screenerWpf.Commands;
using screenerWpf.Helpers;
using screenerWpf.Interfaces;
using screenerWpf.Models;
using screenerWpf.Properties;
using screenerWpf.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing; // dla operacji na Bitmap
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging; // dla BitmapSource

namespace screenerWpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IScreenCaptureService screenCaptureService;
        private readonly IWindowService windowService;
        public ObservableCollection<LastScreenshot> LastScreenshots { get; private set; }
        public ObservableCollection<LastVideo> LastVideos { get; private set; }
        private bool _isScreenshotPopupOpen;
        public bool IsScreenshotPopupOpen
        {
            get => _isScreenshotPopupOpen;
            set
            {
                _isScreenshotPopupOpen = value;
                OnPropertyChanged(nameof(IsScreenshotPopupOpen));
            }
        }

        private bool _isRecordPopupOpen;

        public bool IsRecordPopupOpen
        {
            get => _isRecordPopupOpen;
            set
            {
                _isRecordPopupOpen = value;
                OnPropertyChanged(nameof(IsRecordPopupOpen));
            }
        }

        public ICommand ToggleScreenshotPopupCommand { get; }
        public ICommand ToggleRecordPopupCommand { get; }
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
            ToggleScreenshotPopupCommand = new RelayCommand(param => ToggleScreenshotPopup());
            ToggleRecordPopupCommand = new RelayCommand(param => ToggleRecordPopup());
            CaptureFullCommand = new RelayCommand(ExecuteCaptureFull);
            CaptureAreaCommand = new RelayCommand(ExecuteCaptureArea);
            CaptureWindowCommand = new RelayCommand(ExecuteCaptureWindow);
            RecordVideoCommand = new RelayCommand(ExecuteRecordVideo);
            RecordAreaVideoCommand = new RelayCommand(ExecuteAreaRecordVideo);
            LastScreenshots = new ObservableCollection<LastScreenshot>();
            LastVideos = new ObservableCollection<LastVideo>();
            PopupManager.PopupsClosed += (s, e) => ClosePopups();

            LoadLastScreenshots();
            LoadLastVideos();
        }

        public void ToggleScreenshotPopup()
        {
            IsScreenshotPopupOpen = !IsScreenshotPopupOpen;
            if (IsScreenshotPopupOpen)
            {
                IsRecordPopupOpen = false;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BringMainWindowToFront()
        {
            // Lokalizowanie głównego okna aplikacji
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // Ustawienie okna jako aktywnego
                mainWindow.Activate();

                // Sprawdzenie, czy okno nie jest minimalizowane
                if (mainWindow.WindowState == WindowState.Minimized)
                {
                    mainWindow.WindowState = WindowState.Normal;
                }

                // Przynoszenie okna na pierwszy plan
                mainWindow.Topmost = true;  // Ustawienie okna jako najwyższego
                mainWindow.Topmost = false; // Następnie odwołanie tej decyzji, aby uniknąć zachowania "zawsze na wierzchu"
            }
        }

        public void ToggleRecordPopup()
        {
            IsRecordPopupOpen = !IsRecordPopupOpen;
            if (IsRecordPopupOpen)
            {
                IsScreenshotPopupOpen = false;
            }
        }

        public void ExecuteCaptureFull(object parameter)
        {
            MinimizeMainWindow();

            Bitmap bitmap = screenCaptureService.CaptureScreen();
            ShowEditorWindow(bitmap);
        }

        public void ExecuteCaptureFullJumpTask()
        {
            MessageBox.Show("Przed Minimalize.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            if (screenCaptureService == null)
            {
                MessageBox.Show("screenCaptureService jest null", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (screenCaptureService != null)
            {
                MessageBox.Show("screenCaptureService nie jest null", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Bitmap bitmap = screenCaptureService.CaptureScreen();
            MessageBox.Show("Po capture screen", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);

            ShowEditorWindow(bitmap);
        }

        public void ExecuteCaptureWindow(object parameter)
        {
            MinimizeMainWindow();

            var bitmap = screenCaptureService.CaptureWindow();
            if (bitmap != null)
            {
                ShowEditorWindow(bitmap);
            }
        }

        public void ExecuteCaptureArea(object parameter)
        {
            MinimizeMainWindow();

            Rectangle area = windowService.SelectArea();
            if (!area.IsEmpty)
            {
                Bitmap bitmap = screenCaptureService.CaptureArea(area);
                ShowEditorWindow(bitmap);
            }
        }

        public void ShowEditorWindow(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return;
            }

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
            ClosePopups();
            if (string.IsNullOrEmpty(Settings.Default.ScreenshotsLibrary))
            {
                return;
            }

            string screenshotsDirectory = Settings.Default.ScreenshotsLibrary;
            if (!Directory.Exists(screenshotsDirectory))
            {
                return;
            }

            DirectoryInfo di = new DirectoryInfo(screenshotsDirectory);
            var screenshotFiles = di.GetFiles("*.png").OrderByDescending(f => f.LastWriteTime).Take(5);

            foreach (var file in screenshotFiles)
            {
                LastScreenshots.Add(new LastScreenshot(file.FullName));
            }
        }


        private void LoadLastVideos()
        {
            ClosePopups();
            if (string.IsNullOrEmpty(Settings.Default.RecordsSavePath))
            {
                return;
            }

            string recordsDirectory = Settings.Default.RecordsSavePath;
            if (!Directory.Exists(recordsDirectory))
            {
                return;
            }
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

        public void ExecuteRecordVideo(object parameter)
        {
            // Rozpoczęcie nagrywania
            MinimizeMainWindow();
            screenCaptureService.StartRecording();
            ShowStopRecordingButton();
        }

        public void ExecuteAreaRecordVideo(object parameter)
        {
            MinimizeMainWindow();
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

        private void ClosePopups()
        {
            IsScreenshotPopupOpen = false;
            IsRecordPopupOpen = false;
        }

        private void MinimizeMainWindow()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.WindowState = WindowState.Minimized;
            }
        }
    }
}