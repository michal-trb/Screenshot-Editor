namespace screenerWpf.Views;

using System.Windows;

/// <summary>
/// A window that provides a button to stop the current recording.
/// </summary>
public partial class StopRecordingWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StopRecordingWindow"/> class.
    /// Sets the window's position to the bottom-right of the screen and adjusts its opacity.
    /// </summary>
    public StopRecordingWindow()
    {
        InitializeComponent();
        Top = SystemParameters.WorkArea.Height - Height - 10;
        Left = SystemParameters.WorkArea.Width - Width - 10;
        this.Opacity = 0.25;
    }

    /// <summary>
    /// Handles the button click event to stop the recording.
    /// Calls the MainViewModelService to stop the recording and then closes the window.
    /// </summary>
    /// <param name="sender">The source of the button click event.</param>
    /// <param name="e">The event data associated with the button click.</param>
    private void StopRecordingButton_Click(object sender, RoutedEventArgs e)
    {
        // Invoke stopping the recording via MainViewModelService.
        ((App)Application.Current).MainViewModelService.StopRecording();
        Close();
    }
}
