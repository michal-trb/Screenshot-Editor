using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Management;
using System.Reflection;
using System.Windows.Input;
using screenerWpf.Interfaces;
using screenerWpf.Sevices;
using screenerWpf.Factories;
using screenerWpf.Models;
using System.Windows.Controls;
using System.Windows.Media;
using screenerWpf.Views;

namespace screenerWpf
{

    public partial class Main : Window
    {
        private readonly MainViewModel viewModel;

        public Main()
        {
            InitializeComponent();

            IScreenCaptureService screenCaptureService = new ScreenCaptureService();
            IImageEditorWindowFactory editorWindowFactory = new ImageEditorWindowFactory();
            IWindowService windowService = new WindowService(editorWindowFactory);

            viewModel = new MainViewModel(
                screenCaptureService,
                windowService);

            DataContext = viewModel;

        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow optionsWindow = new OptionsWindow();
            optionsWindow.ShowDialog(); // Pokaż okno jako okno dialogowe
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
    }
}
