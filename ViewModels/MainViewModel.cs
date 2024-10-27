namespace screenerWpf;

using screenerWpf.Commands;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private readonly IScreenCaptureService screenCaptureService;
    private readonly IWindowService windowService;
    private readonly ScreenSelector screenSelector;

    public ObservableCollection<LastScreenshot> LastScreenshots { get; private set; }
    public ObservableCollection<LastVideo> LastVideos { get; private set; }

    public ICommand CaptureFullCommand { get; private set; }
    public ICommand CaptureAreaCommand { get; private set; }
    public ICommand CaptureWindowCommand { get; private set; }
    public ICommand CaptureScreenUnderCursorCommand { get; private set; }
    public ICommand RecordVideoCommand { get; private set; }
    public ICommand RecordAreaVideoCommand { get; private set; }

    private StopRecordingWindow stopRecordingWindow;

    public MainViewModel(IScreenCaptureService screenCaptureService, IWindowService windowService)
    {
        this.screenCaptureService = screenCaptureService ?? throw new ArgumentNullException(nameof(screenCaptureService));
        this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

        CaptureFullCommand = new RelayCommand(async param => await ExecuteCaptureFullAsync());
        CaptureAreaCommand = new RelayCommand(async param => await ExecuteCaptureAreaAsync());
        CaptureWindowCommand = new RelayCommand(async param => await ExecuteCaptureWindowAsync());
        CaptureScreenUnderCursorCommand = new RelayCommand(async param => await ExecuteCaptureScreenUnderCursorAsync());
        RecordVideoCommand = new RelayCommand(async param => await ExecuteRecordVideoAsync());
        RecordAreaVideoCommand = new RelayCommand(async param => await ExecuteAreaRecordVideoAsync());

        LastScreenshots = new ObservableCollection<LastScreenshot>();
        LastVideos = new ObservableCollection<LastVideo>();

        LoadLastScreenshots();
        LoadLastVideos();
    }

    /// <summary>
    /// Triggers the PropertyChanged event to notify listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Captures the entire screen asynchronously and opens the editor window to display the capture.
    /// </summary>
    public async Task ExecuteCaptureFullAsync()
    {
        await CaptureAsync(async () =>
        {
            using var bitmap = screenCaptureService.CaptureScreen();
            await ShowEditorWindowAsync(bitmap);
        });
    }

    /// <summary>
    /// Captures a specific window asynchronously and opens the editor window to display the capture.
    /// </summary>
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

    /// <summary>
    /// Captures a specific area of the screen asynchronously and opens the editor window to display the capture.
    /// </summary>
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

    /// <summary>
    /// Captures the screen under the cursor asynchronously and opens the editor window to display the capture.
    /// </summary>
    public async Task ExecuteCaptureScreenUnderCursorAsync()
    {
        await CaptureAsync(async () =>
        {
            var screenSelector = new ScreenSelector();
            if (screenSelector.ShowDialog() == true)
            {
                System.Drawing.Point cursorPosition = screenSelector.GetSelectedCursorPosition();
                using var bitmap = screenCaptureService.CaptureScreenUnderCursor(cursorPosition);
                await ShowEditorWindowAsync(bitmap);
            }
            else
            {
                RestoreMainWindow();
            }
        });
    }

    /// <summary>
    /// Performs the capture operation asynchronously, handling minimization and restoration of the main window.
    /// </summary>
    /// <param name="captureAction">The asynchronous capture action to execute.</param>
    private async Task CaptureAsync(Func<Task> captureAction)
    {
        MinimizeMainWindow();
        await Task.Delay(200); // Ensure the window has minimized
        try
        {
            await captureAction();
        }
        catch (Exception ex)
        {

        }
        finally
        {
            RestoreMainWindow();
        }
    }

    /// <summary>
    /// Displays the screenshot editor window with the provided bitmap image.
    /// </summary>
    /// <param name="bitmap">The bitmap image to be displayed in the editor.</param>
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

    /// <summary>
    /// Converts a System.Drawing.Bitmap to a WPF-compatible BitmapSource.
    /// </summary>
    /// <param name="bitmap">The bitmap to convert.</param>
    /// <returns>A BitmapSource representing the input bitmap.</returns>
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

    /// <summary>
    /// Loads the most recent screenshots from the designated folder into the LastScreenshots collection.
    /// </summary>
    private void LoadLastScreenshots()
    {
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
            MessageBox.Show($"Unable to load recent screenshots: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Loads the most recent videos from the designated folder into the LastVideos collection.
    /// </summary>
    private void LoadLastVideos()
    {
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
            MessageBox.Show($"Unable to load recent videos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Opens a screenshot file and displays it in the editor window.
    /// </summary>
    /// <param name="filePath">The file path of the screenshot to open.</param>
    public void OpenScreenshot(string filePath)
    {
        try
        {
            using var bitmap = new Bitmap(filePath);
            ShowEditorWindowAsync(bitmap).Wait();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to open file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Opens a video file in the video player window.
    /// </summary>
    /// <param name="filePath">The file path of the video to open.</param>
    public void OpenVideo(string filePath)
    {
        try
        {
            windowService.ShowVideoPlayerWindow(filePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to open file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Starts recording the entire screen asynchronously.
    /// </summary>
    public async Task ExecuteRecordVideoAsync()
    {
        MinimizeMainWindow();
        await Task.Delay(200); // Ensure the window has minimized

        try
        {
            screenCaptureService.StartRecording();
            ShowStopRecordingButton();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to start recording: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            RestoreMainWindow();
        }
    }

    /// <summary>
    /// Starts recording a specific area of the screen asynchronously.
    /// </summary>
    public async Task ExecuteAreaRecordVideoAsync()
    {
        MinimizeMainWindow();
        await Task.Delay(200); // Ensure the window has minimized

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
            MessageBox.Show($"Unable to start area recording: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            RestoreMainWindow();
        }
    }

    /// <summary>
    /// Stops the ongoing screen recording and restores the main window.
    /// </summary>
    public void StopRecording()
    {
        try
        {
            screenCaptureService.StopRecording();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to stop recording: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            HideStopRecordingButton();
            RestoreMainWindow();
        }
    }

    /// <summary>
    /// Displays the stop recording button to allow the user to stop the current recording.
    /// </summary>
    private void ShowStopRecordingButton()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            stopRecordingWindow = new StopRecordingWindow();
            stopRecordingWindow.Show();
        });
    }

    /// <summary>
    /// Hides the stop recording button once the recording is stopped.
    /// </summary>
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

    /// <summary>
    /// Minimizes the main application window.
    /// </summary>
    private void MinimizeMainWindow()
    {
        var mainWindow = Application.Current.MainWindow;
        if (mainWindow != null)
        {
            mainWindow.WindowState = WindowState.Minimized;
        }
    }

    /// <summary>
    /// Restores and activates the main application window.
    /// </summary>
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
