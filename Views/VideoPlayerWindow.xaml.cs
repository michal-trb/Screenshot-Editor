namespace screenerWpf.Views;

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

/// <summary>
/// A window that provides functionalities to play, pause, and stop a video file.
/// It includes a timeline slider to navigate through the video.
/// </summary>
public partial class VideoPlayerWindow : Window
{
    private DispatcherTimer timer;
    private string videoFilePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerWindow"/> class.
    /// Sets up the media player to play the specified video file.
    /// </summary>
    /// <param name="filePath">The path to the video file to be played.</param>
    public VideoPlayerWindow(string filePath)
    {
        InitializeComponent();
        videoFilePath = filePath;
        mediaPlayer.Source = new Uri(videoFilePath);
        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(10)
        };
        timer.Tick += DispatcherTimer_Tick;
        timer.Start();
        mediaPlayer.Play();
    }

    /// <summary>
    /// Minimizes the window.
    /// </summary>
    private void MinimizeWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Maximizes or restores the window based on its current state.
    /// </summary>
    private void MaximizeRestoreWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    /// <summary>
    /// Closes the window.
    /// </summary>
    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Allows the user to drag the window by clicking and holding the title bar.
    /// </summary>
    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }

    /// <summary>
    /// Starts playing the video.
    /// </summary>
    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        mediaPlayer.Play();
    }

    /// <summary>
    /// Pauses the video.
    /// </summary>
    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        mediaPlayer.Pause();
    }

    /// <summary>
    /// Stops the video.
    /// </summary>
    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        mediaPlayer.Stop();
    }

    /// <summary>
    /// Handles changes in the timeline slider to allow video navigation.
    /// </summary>
    private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (timelineSlider.IsMouseCaptureWithin)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(timelineSlider.Value);
        }
    }

    /// <summary>
    /// Handles the event when the video reaches its end.
    /// Stops the video and resets the timeline slider.
    /// </summary>
    private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
    {
        mediaPlayer.Stop();
        timelineSlider.Value = 0;
        UpdateTimeText();
    }

    /// <summary>
    /// Updates the timeline slider and time text periodically using a dispatcher timer.
    /// </summary>
    private void DispatcherTimer_Tick(object sender, EventArgs e)
    {
        if (!timelineSlider.IsMouseCaptureWithin)
        {
            timelineSlider.Value = mediaPlayer.Position.TotalSeconds;
            UpdateTimeText();
        }
    }

    /// <summary>
    /// Initializes the timeline slider when the media has opened successfully.
    /// </summary>
    private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
    {
        if (mediaPlayer.NaturalDuration.HasTimeSpan)
        {
            timelineSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }
        UpdateTimeText();
    }

    /// <summary>
    /// Updates the displayed current time and total time of the video.
    /// </summary>
    private void UpdateTimeText()
    {
        var currentTime = mediaPlayer.Position;
        var totalTime = mediaPlayer.NaturalDuration.HasTimeSpan ? mediaPlayer.NaturalDuration.TimeSpan : TimeSpan.Zero;
        timeText.Text = $"{currentTime.ToString(@"mm\:ss")} : {totalTime.ToString(@"mm\:ss")}";
    }
}
