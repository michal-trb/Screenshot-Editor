using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using screenerWpf.Factories;
using screenerWpf.Helpers;
using screenerWpf.Models;

namespace screenerWpf
{
    public partial class Main : Window
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private IntPtr _windowHandle;
        private const int HOTKEY_ID = 9000;
        private HwndSource _source;
        private readonly IOptionsWindowFactory optionsWindowFactory;
        MainViewModel viewModel;

        public Main(MainViewModel viewModel, IOptionsWindowFactory optionsWindowFactory)
        {
            InitializeComponent();
            InitializeHotKey();

            this.optionsWindowFactory = optionsWindowFactory;
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void InitializeHotKey()
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Loaded += (s, e) =>
                {
                    var helper = new WindowInteropHelper(Application.Current.MainWindow);
                    _windowHandle = helper.Handle;
                    _source = HwndSource.FromHwnd(_windowHandle);
                    _source.AddHook(HwndHook);

                    // Rejestracja Print Screen (0x2C) bez modyfikatorów
                    RegisterHotKey(_windowHandle, HOTKEY_ID, 0, 0x2C);
                    // Rejestracja Shift + Print Screen
                    // Shift - 0x0004, Print Screen - 0x2C
                    RegisterHotKey(_windowHandle, HOTKEY_ID + 1, 0x0004, 0x2C);
                };

                Application.Current.MainWindow.Closing += (s, e) =>
                {
                    UnregisterHotKey(_windowHandle, HOTKEY_ID);
                    UnregisterHotKey(_windowHandle, HOTKEY_ID + 1);
                    _source.RemoveHook(HwndHook);
                };
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                switch (wParam.ToInt32())
                {
                    case HOTKEY_ID:
                        Application.Current.Dispatcher.Invoke(() => viewModel.ExecuteCaptureAreaAsync());
                        handled = true;
                        break;
                    case HOTKEY_ID + 1:
                        Application.Current.Dispatcher.Invoke(() => viewModel.ExecuteCaptureFullAsync());
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = optionsWindowFactory.Create();
            optionsWindow.ShowDialog();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Screenshot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image && image.DataContext is LastScreenshot screenshot)
            {
                (DataContext as MainViewModel)?.OpenScreenshot(screenshot.FilePath);
            }
        }

        private void Video_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image && image.DataContext is LastVideo video)
            {
                (DataContext as MainViewModel)?.OpenVideo(video.FilePath);
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height += 105;
            UpdateWindowWidth();
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height -= 105;
            UpdateWindowWidth();
        }

        private void UpdateWindowWidth()
        {
            if (IsAnyExpanderExpanded())
            {
                this.Width = 520; // Zwiększ szerokość, jeśli którykolwiek Expander jest rozwinięty
            }
            else
            {
                this.Width = 304; // Zmniejsz szerokość, gdy wszystkie Expandery są zwinięte
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.IsRecordPopupOpen = false;
                viewModel.IsScreenshotPopupOpen = false;
            }
        }

        private bool IsAnyExpanderExpanded()
        {
            return ExpanderScreenshots.IsExpanded || ExpanderVideos.IsExpanded;
        }

        private void OpenScreenshotsFolder(object sender, RoutedEventArgs e)
        {
            string folderPath = Properties.Settings.Default.ScreenshotsLibrary;
            if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
            else
            {
                MessageBox.Show("Folder path is not set or does not exist.", "Error");
            }
        }

        private void OpenVideosFolder(object sender, RoutedEventArgs e)
        {
            string folderPath = Properties.Settings.Default.RecordsSavePath;
            if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
            else
            {
                MessageBox.Show("Folder path is not set or does not exist.", "Error");
            }
        }

        public void DisplayScreenshotEditor(BitmapSource screenshot)
        {
            var screenshotEditor = new ImageEditorControl(screenshot);
            screenshotEditor.LoadedAndSizeUpdated += ScreenshotEditor_LoadedAndSizeUpdated;
            ScreenshotEditorGrid.Children.Clear();
            ScreenshotEditorGrid.Children.Add(screenshotEditor);

            // Ukryj inne gridy
            GridScreenshots.Visibility = Visibility.Collapsed;
            GridVideos.Visibility = Visibility.Collapsed;
        }


        private void ScreenshotEditor_LoadedAndSizeUpdated(ImageEditorControl element)
        {
            AdjustWindowSize(element);
        }

        private void AdjustWindowSize(ImageEditorControl element)
        {
            double newWidth = element.Width + 40;  
            double newHeight = element.Height + 40;  

            this.Width = Math.Max(this.Width, newWidth);
            this.Height = Math.Max(this.Height, newHeight);
        }
    }
}
