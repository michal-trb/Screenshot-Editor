using System;
using System.Windows;
using System.Windows.Threading;

namespace screenerWpf.Views
{
    public partial class VideoPlayerWindow : Window
    {
        private DispatcherTimer timer;
        private string videoFilePath;

        public VideoPlayerWindow(string filePath)
        {
            InitializeComponent();
            videoFilePath = filePath;

            mediaPlayer.Source = new Uri(videoFilePath);
            // Inicjalizacja timera
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += DispatcherTimer_Tick;
            timer.Start();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timelineSlider.IsMouseCaptureWithin)
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(timelineSlider.Value);
            }
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            timelineSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            timelineSlider.Value = 0;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!timelineSlider.IsMouseCaptureWithin)
            {
                timelineSlider.Value = mediaPlayer.Position.TotalSeconds;
            }
        }
    }
}
