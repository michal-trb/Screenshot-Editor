using System.Windows.Input;
using System.Drawing; // dla operacji na Bitmap
using System.Windows.Media.Imaging; // dla BitmapSource
using screenerWpf.Commands;
using System;
using screenerWpf.Interfaces;
using screenerWpf.Sevices;
using System.IO;

namespace screenerWpf
{
    public class MainViewModel
    {
        private readonly IScreenCaptureService screenCaptureService;
        private readonly IWindowService windowService;

        public ICommand MinimizeCommand { get; private set; }
        public ICommand MaximizeRestoreCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand CaptureFullCommand { get; private set; }
        public ICommand CaptureAreaCommand { get; private set; }

        public event Action MinimizeRequest;
        public event Action MaximizeRestoreRequest;
        public event Action CloseRequest;

        public MainViewModel(IScreenCaptureService screenCaptureService, IWindowService windowService)
        {
            this.screenCaptureService = screenCaptureService;
            this.windowService = windowService;
            CaptureFullCommand = new RelayCommand(ExecuteCaptureFull);
            CaptureAreaCommand = new RelayCommand(ExecuteCaptureArea);
            MinimizeCommand = new RelayCommand(o => MinimizeRequest?.Invoke());
            MaximizeRestoreCommand = new RelayCommand(o => MaximizeRestoreRequest?.Invoke());
            CloseCommand = new RelayCommand(o => CloseRequest?.Invoke());
        }
        private void ExecuteCaptureFull(object parameter)
        {
            Bitmap bitmap = screenCaptureService.CaptureScreen();
            ShowEditorWindow(bitmap);
        }

        private void ExecuteCaptureArea(object parameter)
        {
            Rectangle area = windowService.SelectArea();
            if (!area.IsEmpty)
            {
                Bitmap bitmap = screenCaptureService.CaptureArea(area);
                ShowEditorWindow(bitmap);
            }
        }

        private void ShowEditorWindow(Bitmap bitmap)
        {
            BitmapSource bitmapSource = ConvertBitmapToBitmapSource(bitmap);
            windowService.ShowImageEditorWindow(bitmapSource);
        }

        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}