using System.Windows;
using System.Windows.Input;
using screenerWpf.Factories;
using screenerWpf.Models;
using System.Windows.Controls;
using System;
using System.Windows.Controls.Primitives;
using System.IO;
using screenerWpf.Helpers;

namespace screenerWpf
{

    public partial class Main : Window
    {
        private readonly IOptionsWindowFactory optionsWindowFactory;

        public Main(MainViewModel viewModel, IOptionsWindowFactory optionsWindowFactory)
        {
            InitializeComponent();

            this.optionsWindowFactory = optionsWindowFactory;

            DataContext = viewModel;

        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            PopupManager.CloseAllPopups();
            var optionsWindow = optionsWindowFactory.Create();
            optionsWindow.ShowDialog();
        }


        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            PopupManager.CloseAllPopups();
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreWindow(object sender, RoutedEventArgs e)
        {
            PopupManager.CloseAllPopups();
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            PopupManager.CloseAllPopups();
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Screenshot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PopupManager.CloseAllPopups();
            if (sender is Image image && image.DataContext is LastScreenshot screenshot)
            {
                (DataContext as MainViewModel)?.OpenScreenshot(screenshot.FilePath);
            }
        }

        private void Video_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PopupManager.CloseAllPopups();
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
                this.Width = 296; // Zmniejsz szerokość, gdy wszystkie Expandery są zwinięte
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
            string folderPath = Properties.Settings.Default.ScreenshorsLibrary;
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
