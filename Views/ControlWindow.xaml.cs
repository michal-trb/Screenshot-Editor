namespace screenerWpf.Views;

using System;
using System.Windows;

/// <summary>
/// A window that provides controls for stopping ongoing operations.
/// </summary>
public partial class ControlWindow : Window
{
    /// <summary>
    /// Event triggered when the stop button is clicked, indicating that the user wants to stop an operation.
    /// </summary>
    public event EventHandler StopRequested;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlWindow"/> class.
    /// Sets up the window's UI components.
    /// </summary>
    public ControlWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the click event of the Stop button.
    /// Triggers the <see cref="StopRequested"/> event and closes the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        StopRequested?.Invoke(this, EventArgs.Empty);
        this.Close();
    }
}