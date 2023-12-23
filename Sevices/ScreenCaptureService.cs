using System.Drawing;
using screenerWpf.Interfaces;
using System.Management;
using System;
using Size = System.Drawing.Size;
using Point = System.Drawing.Point;
using System.IO;
using System.Windows;

namespace screenerWpf.Sevices
{
    public class ScreenCaptureService : IScreenCaptureService
    {
        private ScreenRecorder screenRecorder;
        private IWindowService windowService; // Zakładam, że IWindowService może otwierać VideoPlayerWindow

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
                windowService.ShowVideoPlayerWindow(filePath); // Otwarcie okna odtwarzacza wideo
            });
        }

        public Bitmap CaptureScreen()
        {
            ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_VideoController");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    int screenWidth = Convert.ToInt32(obj["CurrentHorizontalResolution"]);
                    int screenHeight = Convert.ToInt32(obj["CurrentVerticalResolution"]);

                    Bitmap bitmap = new Bitmap(screenWidth, screenHeight);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                    }
                    return bitmap;
                }
            }

            return null; // Jeśli nie uda się uzyskać informacji o rozdzielczości
        }

        public Bitmap CaptureArea(Rectangle area)
        {
            Bitmap bitmap = new Bitmap(area.Width, area.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(area.Location, Point.Empty, area.Size);
            }
            return bitmap;
        }

        public void StartRecording()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            string fileName = $"Recording_{timestamp}.mp4";

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), fileName);

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

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), fileName);

            screenRecorder.StartRecordingArea(filePath, area);
        }
    }
}
