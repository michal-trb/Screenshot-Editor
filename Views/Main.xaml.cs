using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Management;
using System.Reflection;
using System.Windows.Input;
using screenerWpf.Interfaces;
using screenerWpf.Sevices;
using screenerWpf.Factories;

namespace screenerWpf
{

    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            IScreenCaptureService screenCaptureService = new ScreenCaptureService();
            IImageEditorWindowFactory editorWindowFactory = new ImageEditorWindowFactory();
            IWindowService windowService = new WindowService(editorWindowFactory);

            var viewModel = new MainViewModel(screenCaptureService, windowService);

            DataContext = viewModel;

            viewModel.MinimizeRequest += MinimizeWindow;
            viewModel.MaximizeRestoreRequest += MaximizeRestoreWindow;
            viewModel.CloseRequest += CloseWindow;
        }

        private void MinimizeWindow()
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreWindow()
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow()
        {
            this.Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (DataContext is MainViewModel viewModel)
            {
                viewModel.MinimizeRequest -= MinimizeWindow;
                viewModel.MaximizeRestoreRequest -= MaximizeRestoreWindow;
                viewModel.CloseRequest -= CloseWindow;
            }
        }
    }
}
