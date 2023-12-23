using screenerWpf.Factories;
using System;
using System.Windows;
using System.Windows.Input;
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
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += DispatcherTimer_Tick;
            timer.Start();
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

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            timelineSlider.Value = 0;
            UpdateTimeText();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!timelineSlider.IsMouseCaptureWithin)
            {
                timelineSlider.Value = mediaPlayer.Position.TotalSeconds;
                UpdateTimeText();
            }
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            timelineSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            UpdateTimeText();
        }

        private void UpdateTimeText()
        {
            var currentTime = mediaPlayer.Position;
            var totalTime = mediaPlayer.NaturalDuration.HasTimeSpan ? mediaPlayer.NaturalDuration.TimeSpan : TimeSpan.Zero;
            timeText.Text = $"{currentTime.ToString(@"mm\:ss")} : {totalTime.ToString(@"mm\:ss")}";
        }

    }
}
