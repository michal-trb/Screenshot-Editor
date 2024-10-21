namespace screenerWpf.ViewModels;

using System.ComponentModel;

/// <summary>
/// ViewModel responsible for managing the application's options, such as screenshot save paths, video save paths, and Dropbox keys.
/// Implements INotifyPropertyChanged to enable dynamic data updates in the UI.
/// </summary>
public class OptionsViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Triggered when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private string _screenshotsSavePath;
    private string _screenshotsLibraryPath;
    private string _recordsSavePath;
    private string _dropboxAppKey;
    private string _dropboxAppSecret;

    /// <summary>
    /// The save path for screenshots.
    /// </summary>
    public string ScreenshotsSavePath
    {
        get => _screenshotsSavePath;
        set
        {
            _screenshotsSavePath = value;
            OnPropertyChanged(nameof(ScreenshotsSavePath));
        }
    }

    /// <summary>
    /// The path to the screenshot library.
    /// </summary>
    public string ScreenshotsLibraryPath
    {
        get => _screenshotsLibraryPath;
        set
        {
            _screenshotsLibraryPath = value;
            OnPropertyChanged(nameof(ScreenshotsLibraryPath));
        }
    }

    /// <summary>
    /// The save path for video recordings.
    /// </summary>
    public string RecordsSavePath
    {
        get => _recordsSavePath;
        set
        {
            _recordsSavePath = value;
            OnPropertyChanged(nameof(RecordsSavePath));
        }
    }

    /// <summary>
    /// The Dropbox application key used for authentication.
    /// </summary>
    public string DropboxAppKey
    {
        get => _dropboxAppKey;
        set
        {
            _dropboxAppKey = value;
            OnPropertyChanged(nameof(DropboxAppKey));
        }
    }

    /// <summary>
    /// The Dropbox application secret used for authentication.
    /// </summary>
    public string DropboxAppSecret
    {
        get => _dropboxAppSecret;
        set
        {
            _dropboxAppSecret = value;
            OnPropertyChanged(nameof(DropboxAppSecret));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsViewModel"/> class.
    /// Loads the current application settings.
    /// </summary>
    public OptionsViewModel()
    {
        LoadSettings();
    }

    /// <summary>
    /// Loads application settings from the application properties and sets appropriate fields.
    /// </summary>
    private void LoadSettings()
    {
        ScreenshotsSavePath = Properties.Settings.Default.ScreenshotsSavePath;
        ScreenshotsLibraryPath = Properties.Settings.Default.ScreenshotsLibrary;
        RecordsSavePath = Properties.Settings.Default.RecordsSavePath;
        DropboxAppKey = Properties.Settings.Default.DropboxAppKey;
        DropboxAppSecret = Properties.Settings.Default.DropboxAppSecret;
    }

    /// <summary>
    /// Saves the current settings to the application properties.
    /// </summary>
    public void SaveSettings()
    {
        Properties.Settings.Default.ScreenshotsSavePath = ScreenshotsSavePath;
        Properties.Settings.Default.ScreenshotsLibrary = ScreenshotsLibraryPath;
        Properties.Settings.Default.RecordsSavePath = RecordsSavePath;
        Properties.Settings.Default.DropboxAppKey = DropboxAppKey;
        Properties.Settings.Default.DropboxAppSecret = DropboxAppSecret;
        Properties.Settings.Default.Save();
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event after a property value changes.
    /// </summary>
    /// <param name="name">The name of the property that changed.</param>
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
