namespace screenerWpf.ViewModels;

using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using screenerWpf.Properties;
using System;

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
    /// Whether screenshots should be automatically saved.
    /// </summary>
    public bool AutoSaveScreenshots
    {
        get => Properties.Settings.Default.AutoSaveScreenshots;
        set
        {
            Properties.Settings.Default.AutoSaveScreenshots = value;
            OnPropertyChanged(nameof(AutoSaveScreenshots));
        }
    }

    public string DefaultFontFamily
    {
        get => Properties.Settings.Default.DefaultFontFamily;
        set
        {
            Properties.Settings.Default.DefaultFontFamily = value;
            OnPropertyChanged(nameof(DefaultFontFamily));
        }
    }

    public int DefaultFontSize
    {
        get => Properties.Settings.Default.DefaultFontSize;
        set
        {
            Properties.Settings.Default.DefaultFontSize = value;
            OnPropertyChanged(nameof(DefaultFontSize));
        }
    }

    public int DefaultThickness
    {
        get => Properties.Settings.Default.DefaultThickness;
        set
        {
            Properties.Settings.Default.DefaultThickness = value;
            OnPropertyChanged(nameof(DefaultThickness));
        }
    }

    public int DefaultTransparency
    {
        get => Properties.Settings.Default.DefaultTransparency;
        set
        {
            Properties.Settings.Default.DefaultTransparency = value;
            OnPropertyChanged(nameof(DefaultTransparency));
        }
    }

    public string DefaultColor
    {
        get => Properties.Settings.Default.DefaultColor;
        set
        {
            Properties.Settings.Default.DefaultColor = value;
            OnPropertyChanged(nameof(DefaultColor));
        }
    }

    public ObservableCollection<string> FontFamilies { get; } = new ObservableCollection<string>
    {
        "Arial", "Calibri", "Times New Roman", "Verdana", "Courier New"
    };

    public ObservableCollection<int> FontSizes { get; } = new ObservableCollection<int>
    {
        8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40
    };

    public ObservableCollection<int> Thicknesses { get; } = new ObservableCollection<int>
    {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10
    };

    public ObservableCollection<int> TransparencySizes { get; } = new ObservableCollection<int>
    {
        10, 20, 30, 40, 50, 60, 70, 80, 90, 100
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsViewModel"/> class.
    /// Loads the current application settings.
    /// </summary>
    public OptionsViewModel()
    {
        // Inicjalizacja kolekcji
        InitializeCollections();

        // Wczytaj zapisane wartości lub ustaw domyślne
        LoadSavedSettings();
    }

    private void InitializeCollections()
    {
        FontFamilies.Clear();
        foreach (var font in new[] { "Arial", "Calibri", "Times New Roman", "Verdana", "Courier New" })
        {
            FontFamilies.Add(font);
        }

        FontSizes.Clear();
        for (int i = 8; i <= 40; i += 2)
        {
            FontSizes.Add(i);
        }

        Thicknesses.Clear();
        for (int i = 1; i <= 10; i++)
        {
            Thicknesses.Add(i);
        }

        TransparencySizes.Clear();
        for (int i = 10; i <= 100; i += 10)
        {
            TransparencySizes.Add(i);
        }
    }

    private void LoadSavedSettings()
    {
        // Wczytaj ścieżki
        ScreenshotsSavePath = Settings.Default.ScreenshotsSavePath;
        ScreenshotsLibraryPath = Settings.Default.ScreenshotsLibrary;
        RecordsSavePath = Settings.Default.RecordsSavePath;

        // Wczytaj ustawienie automatycznego zapisu
        AutoSaveScreenshots = Settings.Default.AutoSaveScreenshots;

        // Wczytaj domyślne wartości
        DefaultFontFamily = Settings.Default.DefaultFontFamily;
        DefaultFontSize = Settings.Default.DefaultFontSize;
        DefaultThickness = Settings.Default.DefaultThickness;
        DefaultTransparency = Settings.Default.DefaultTransparency;
        DefaultColor = Settings.Default.DefaultColor;
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
        try
        {
            // Zapisz ścieżki
            Properties.Settings.Default.ScreenshotsSavePath = ScreenshotsSavePath;
            Properties.Settings.Default.ScreenshotsLibrary = ScreenshotsLibraryPath;
            Properties.Settings.Default.RecordsSavePath = RecordsSavePath;

            // Zapisz ustawienia automatycznego zapisu
            Properties.Settings.Default.AutoSaveScreenshots = AutoSaveScreenshots;

            // Zapisz domyślne wartości
            Properties.Settings.Default.DefaultFontFamily = DefaultFontFamily;
            Properties.Settings.Default.DefaultFontSize = DefaultFontSize;
            Properties.Settings.Default.DefaultThickness = DefaultThickness;
            Properties.Settings.Default.DefaultTransparency = DefaultTransparency;

            // Konwertuj kolor na string przed zapisaniem
            if (DefaultColor != null)
            {
                Properties.Settings.Default.DefaultColor = DefaultColor.ToString();
            }

            // Zapisz wszystkie zmiany
            Properties.Settings.Default.Save();

            MessageBox.Show("Ustawienia zostały zapisane pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Wystąpił błąd podczas zapisywania ustawień: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
