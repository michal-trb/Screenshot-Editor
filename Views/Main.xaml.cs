using System.Windows;
using System.Windows.Input;
using screenerWpf.Factories;
using screenerWpf.Models;
using System.Windows.Controls;
using System;
using System.Windows.Controls.Primitives;

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
                this.DragMove();
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

        private void Expander_Expanded_Screenshots(object sender, RoutedEventArgs e)
        {
            // Zwiększ wysokość okna, gdy którykolwiek Expander jest rozwinięty
            this.Height += 95; // Dodaj estymowaną wysokość rozwiniętego Expandera
        }

        private void Expander_Collapsed_Screenshots(object sender, RoutedEventArgs e)
        {
            // Zmniejsz wysokość okna, gdy Expander jest zwinięty
            this.Height -= 95; // Odejmij estymowaną wysokość rozwiniętego Expandera

            // Ustaw minimalną wysokość okna
        }
        private void Expander_Expanded_Videos(object sender, RoutedEventArgs e)
        {
            // Zwiększ wysokość okna, gdy którykolwiek Expander jest rozwinięty
            this.Height += 95; // Dodaj estymowaną wysokość rozwiniętego Expandera
        }

        private void Expander_Collapsed_Videos(object sender, RoutedEventArgs e)
        {
            // Zmniejsz wysokość okna, gdy Expander jest zwinięty
            this.Height -= 95; // Odejmij estymowaną wysokość rozwiniętego Expandera

            // Ustaw minimalną wysokość okna
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

    }
}
