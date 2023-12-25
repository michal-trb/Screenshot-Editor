using System.Drawing;
using screenerWpf.Interfaces;
using System;
using Point = System.Drawing.Point;
using System.IO;
using System.Windows;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using screenerWpf.Properties;
using screenerWpf.Views;

using screenerWpf.Sevices.CaptureServices;
using System.Threading.Tasks;

namespace screenerWpf.Sevices
{
    public class ScreenCaptureService : IScreenCaptureService
    {
        private ScreenRecorder screenRecorder;
        private IWindowService windowService;

        public ScreenCaptureService(IWindowService windowService)
        {
            this.windowService = windowService;
            screenRecorder = new ScreenRecorder();
            screenRecorder.RecordingCompleted += OnRecordingCompleted;
        }

        private void OnRecordingCompleted(string filePath)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                windowService.ShowVideoPlayerWindow(filePath);
            });
        }

        public Bitmap CaptureScreen()
        {
            return FullScreenshot.CaptureScreen();
        }

        public Bitmap CaptureArea(Rectangle area)
        {
            return AreaScreenshot.CaptureArea(area);
        }

        public void StartRecording()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Recording_{timestamp}.mp4";
            string filePath = Path.Combine(Settings.Default.RecordsSavePath, fileName);
            screenRecorder.StartRecording(filePath);
        }

        public void StopRecording()
        {
            screenRecorder.StopRecording();
        }

        public void StartAreaRecording(Rectangle area)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Recording_{timestamp}.mp4";
            string filePath = Path.Combine(Settings.Default.RecordsSavePath, fileName);
            screenRecorder.StartRecordingArea(filePath, area);
        }

        public Task CaptureWithScrollAsync()
        {
            var windowsScreenshot = new WindowScreenshot();
            return windowsScreenshot.CaptureWithScrollAsync();
        }
    }
}
