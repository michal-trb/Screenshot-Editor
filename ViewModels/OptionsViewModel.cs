using System.ComponentModel;

namespace screenerWpf.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _screenshotsSavePath;
        private string _screenshotsLibraryPath;
        private string _recordsSavePath;
        private string _dropboxAppKey;
        private string _dropboxAppSecret;

        public string ScreenshotsSavePath
        {
            get => _screenshotsSavePath;
            set
            {
                _screenshotsSavePath = value;
                OnPropertyChanged(nameof(ScreenshotsSavePath));
            }
        }

        public string ScreenshotsLibraryPath
        {
            get => _screenshotsLibraryPath;
            set
            {
                _screenshotsLibraryPath = value;
                OnPropertyChanged(nameof(ScreenshotsLibraryPath));
            }
        }

        public string RecordsSavePath
        {
            get => _recordsSavePath;
            set
            {
                _recordsSavePath = value;
                OnPropertyChanged(nameof(RecordsSavePath));
            }
        }

        public string DropboxAppKey
        {
            get => _dropboxAppKey;
            set
            {
                _dropboxAppKey = value;
                OnPropertyChanged(nameof(DropboxAppKey));
            }
        }
        public string DropboxAppSecret
        {
            get => _dropboxAppSecret;
            set
            {
                _dropboxAppSecret = value;
                OnPropertyChanged(nameof(DropboxAppSecret));
            }
        }

        public OptionsViewModel()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            ScreenshotsSavePath = Properties.Settings.Default.ScreenshotsSavePath;
            ScreenshotsLibraryPath = Properties.Settings.Default.ScreenshorsLibrary;
            RecordsSavePath = Properties.Settings.Default.RecordsSavePath;
            DropboxAppKey = Properties.Settings.Default.DropboxAppKey;
            DropboxAppSecret = Properties.Settings.Default.DropboxAppSecret;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ScreenshotsSavePath = ScreenshotsSavePath;
            Properties.Settings.Default.ScreenshorsLibrary = ScreenshotsLibraryPath;
            Properties.Settings.Default.RecordsSavePath = RecordsSavePath;
            Properties.Settings.Default.DropboxAppKey = DropboxAppKey;
            Properties.Settings.Default.DropboxAppSecret= DropboxAppSecret;
            Properties.Settings.Default.Save();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
