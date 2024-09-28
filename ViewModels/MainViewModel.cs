using screenerWpf.Commands;
using screenerWpf.Helpers;
using screenerWpf.Interfaces;
using screenerWpf.Models;
using screenerWpf.Properties;
using screenerWpf.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks; // Dodane dla asynchroniczności
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
        public ICommand CaptureWindowCommand { get; private set; }
        public ICommand RecordVideoCommand { get; private set; }
        public ICommand RecordAreaVideoCommand { get; private set; }

        private StopRecordingWindow stopRecordingWindow;

        public MainViewModel(IScreenCaptureService screenCaptureService, IWindowService windowService)
        {
            this.screenCaptureService = screenCaptureService ?? throw new ArgumentNullException(nameof(screenCaptureService));
            this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

            ToggleScreenshotPopupCommand = new RelayCommand(param => TogglePopup(ref _isScreenshotPopupOpen, ref _isRecordPopupOpen));
            ToggleRecordPopupCommand = new RelayCommand(param => TogglePopup(ref _isRecordPopupOpen, ref _isScreenshotPopupOpen));

            CaptureFullCommand = new RelayCommand(async param => await ExecuteCaptureFullAsync());
            CaptureAreaCommand = new RelayCommand(async param => await ExecuteCaptureAreaAsync());
            CaptureWindowCommand = new RelayCommand(async param => await ExecuteCaptureWindowAsync());
            RecordVideoCommand = new RelayCommand(async param => await ExecuteRecordVideoAsync());
            RecordAreaVideoCommand = new RelayCommand(async param => await ExecuteAreaRecordVideoAsync());

            LastScreenshots = new ObservableCollection<LastScreenshot>();
            LastVideos = new ObservableCollection<LastVideo>();

            LoadLastScreenshots();
            LoadLastVideos();
        }

        private void TogglePopup(ref bool popupToToggle, ref bool otherPopup)
        {
            popupToToggle = !popupToToggle;
            if (popupToToggle)
            {
                otherPopup = false;
            }
            OnPropertyChanged(null);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task ExecuteCaptureFullAsync()
        {
            await CaptureAsync(async () =>
            {
                using var bitmap = screenCaptureService.CaptureScreen();
                await ShowEditorWindowAsync(bitmap);
            });
        }

        public async Task ExecuteCaptureWindowAsync()
        {
            await CaptureAsync(async () =>
            {
                using var bitmap = screenCaptureService.CaptureWindow();
                if (bitmap != null)
                {
                    await ShowEditorWindowAsync(bitmap);
                }
            });
        }

        public async Task ExecuteCaptureAreaAsync()
        {
            await CaptureAsync(async () =>
            {
                var area = windowService.SelectArea();
                if (!area.IsEmpty)
                {
                    var bitmap = screenCaptureService.CaptureArea(area);
                    await ShowEditorWindowAsync(bitmap);
                }
                else
                {
                    RestoreMainWindow();
                }
            });
        }

        private async Task CaptureAsync(Func<Task> captureAction)
        {
            MinimizeMainWindow();
            await Task.Delay(200); // Zapewnienie, że okno zostało zminimalizowane
            try
            {
                await captureAction();
            }
            catch (Exception ex)
            {
                // Logowanie błędu lub wyświetlenie komunikatu dla użytkownika
                MessageBox.Show($"Wystąpił błąd podczas wykonywania zrzutu ekranu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                RestoreMainWindow();
            }
        }

        private async Task ShowEditorWindowAsync(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return;
            }

            BitmapSource bitmapSource = ConvertBitmapToBitmapSource(bitmap);
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var mainWindow = Application.Current.MainWindow as Main;
                mainWindow?.DisplayScreenshotEditor(bitmapSource);
            });
        }

        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            BitmapSource bitmapSource;
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                bitmapSource = BitmapFrame.Create(memory, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            }

            return bitmapSource;
        }

        private void LoadLastScreenshots()
        {
            ClosePopups();
            LastScreenshots.Clear();

            if (string.IsNullOrEmpty(Settings.Default.ScreenshotsLibrary))
            {
                return;
            }

            try
            {
                string screenshotsDirectory = Settings.Default.ScreenshotsLibrary;
                if (!Directory.Exists(screenshotsDirectory))
                {
                    return;
                }

                DirectoryInfo di = new DirectoryInfo(screenshotsDirectory);
                var screenshotFiles = di.GetFiles("*.png")
                    .OrderByDescending(f => f.LastWriteTime)
                    .Take(5);

                foreach (var file in screenshotFiles)
                {
                    LastScreenshots.Add(new LastScreenshot(file.FullName));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można załadować ostatnich zrzutów ekranu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLastVideos()
        {
            ClosePopups();
            LastVideos.Clear();

            if (string.IsNullOrEmpty(Settings.Default.RecordsSavePath))
            {
                return;
            }

            try
            {
                string recordsDirectory = Settings.Default.RecordsSavePath;
                if (!Directory.Exists(recordsDirectory))
                {
                    return;
                }

                DirectoryInfo di = new DirectoryInfo(recordsDirectory);
                var videoFiles = di.GetFiles("*.mp4")
                    .OrderByDescending(f => f.LastWriteTime)
                    .Take(5);

                foreach (var file in videoFiles)
                {
                    LastVideos.Add(new LastVideo(file.FullName));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można załadować ostatnich nagrań wideo: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OpenScreenshot(string filePath)
        {
            try
            {
                using var bitmap = new Bitmap(filePath);
                ShowEditorWindowAsync(bitmap).Wait();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można otworzyć pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OpenVideo(string filePath)
        {
            try
            {
                windowService.ShowVideoPlayerWindow(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można otworzyć pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task ExecuteRecordVideoAsync()
        {
            MinimizeMainWindow();
            await Task.Delay(200); // Zapewnienie, że okno zostało zminimalizowane

            try
            {
                screenCaptureService.StartRecording();
                ShowStopRecordingButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można rozpocząć nagrywania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                RestoreMainWindow();
            }
        }

        public async Task ExecuteAreaRecordVideoAsync()
        {
            MinimizeMainWindow();
            await Task.Delay(200); // Zapewnienie, że okno zostało zminimalizowane

            try
            {
                var area = windowService.SelectArea();
                if (!area.IsEmpty)
                {
                    screenCaptureService.StartAreaRecording(area);
                    ShowStopRecordingButton();
                }
                else
                {
                    RestoreMainWindow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można rozpocząć nagrywania obszaru: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                RestoreMainWindow();
            }
        }

        public void StopRecording()
        {
            try
            {
                screenCaptureService.StopRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można zatrzymać nagrywania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                HideStopRecordingButton();
                RestoreMainWindow();
            }
        }

        private void ShowStopRecordingButton()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                stopRecordingWindow = new StopRecordingWindow();
                stopRecordingWindow.Show();
            });
        }

        private void HideStopRecordingButton()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (stopRecordingWindow != null)
                {
                    stopRecordingWindow.Close();
                    stopRecordingWindow = null;
                }
            });
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

        private void RestoreMainWindow()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            }
        }
    }
}
