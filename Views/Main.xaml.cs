using System.Windows;
using System.Windows.Input;
using screenerWpf.Factories;
using screenerWpf.Models;
using System.Windows.Controls;
using System;
using System.Windows.Controls.Primitives;
using System.IO;
using screenerWpf.Helpers;
using screenerWpf.Sevices;
using screenerWpf.Resources;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace screenerWpf
{
    public partial class Main : Window
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
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
                        Application.Current.Dispatcher.Invoke(() => viewModel.ExecuteCaptureArea(null));
                        handled = true;
                        break;
                    case HOTKEY_ID + 1:
                        Application.Current.Dispatcher.Invoke(() => viewModel.ExecuteCaptureFull(null));
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }


        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (!IsMouseOverPopup() 
                && !IsClickOnPopupButton(e))
            {
                CloseAllPopups();
            }
        }

        private bool IsMouseOverPopup()
        {
            // Sprawdź, czy kursor myszy znajduje się nad jednym z popupów
            return ScreenshotPopup.IsMouseOver || RecordPopup.IsMouseOver;
        }

        private bool IsClickOnPopupButton(MouseButtonEventArgs e)
        {
            // Sprawdź, czy kliknięto na jednym z przycisków otwierających popupy
            var source = e.Source as FrameworkElement;
            return source == OpenScreenshotPopupButton || source == OpenRecordPopupButton;
        }

        private void CloseAllPopups()
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.IsScreenshotPopupOpen = false;
                viewModel.IsRecordPopupOpen = false;
            }
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            PopupManager.CloseAllPopups();
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
            this.Height += 105; // Zwiększ wysokość okna
            UpdateWindowWidth();
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height -= 105; // Zmniejsz wysokość okna
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

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            UpdatePopupPosition(ScreenshotPopup);
            UpdatePopupPosition(RecordPopup);
        }

        private void UpdatePopupPosition(Popup popup)
        {
            if (popup.IsOpen)
            {
                // Force update of the Popup's position
                var offset = popup.HorizontalOffset;
                popup.HorizontalOffset = offset + 1;
                popup.HorizontalOffset = offset;
            }
        }

        private void OpenScreenshotsFolder(object sender, RoutedEventArgs e)
        {
            PopupManager.CloseAllPopups();
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
            PopupManager.CloseAllPopups();
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

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopupManager.CloseAllPopups();
        }
    }
}
