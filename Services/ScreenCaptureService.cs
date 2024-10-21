namespace screenerWpf.Sevices;

using System.Drawing;
using screenerWpf.Interfaces;
using System;
using System.IO;
using System.Windows;
using screenerWpf.Properties;
using screenerWpf.Sevices.CaptureServices;
using screenerWpf.Services.CaptureServices;
using screenerWpf.Views;


/// <summary>
/// Provides services to capture screenshots and record screen activities.
/// </summary>
public class ScreenCaptureService : IScreenCaptureService
{
    private ScreenRecorder screenRecorder;
    private IWindowService windowService;
    private OverlayWindow overlay;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenCaptureService"/> class.
    /// </summary>
    /// <param name="windowService">Service to handle window-related operations, such as displaying the video player window.</param>
    public ScreenCaptureService(IWindowService windowService)
    {
        this.windowService = windowService;
        screenRecorder = new ScreenRecorder();
        screenRecorder.RecordingCompleted += OnRecordingCompleted;
    }

    /// <summary>
    /// Handles the event when the recording is completed.
    /// </summary>
    /// <param name="filePath">Path to the saved video recording.</param>
    private void OnRecordingCompleted(string filePath)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            windowService.ShowVideoPlayerWindow(filePath);
        });
    }

    /// <summary>
    /// Captures a screenshot of the entire screen.
    /// </summary>
    /// <returns>A <see cref="Bitmap"/> representing the captured screenshot of the full screen.</returns>
    public Bitmap CaptureScreen()
    {
        return FullScreenshot.CaptureScreen();
    }

    /// <summary>
    /// Captures a screenshot of a specific area of the screen.
    /// </summary>
    /// <param name="area">The <see cref="Rectangle"/> representing the area to capture.</param>
    /// <returns>A <see cref="Bitmap"/> representing the captured area of the screen.</returns>
    public Bitmap CaptureArea(Rectangle area)
    {
        return AreaScreenshot.CaptureArea(area);
    }

    /// <summary>
    /// Starts recording the entire screen.
    /// </summary>
    public void StartRecording()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Recording_{timestamp}.mp4";
        string filePath = Path.Combine(Settings.Default.RecordsSavePath, fileName);
        screenRecorder.StartRecording(filePath);
    }

    /// <summary>
    /// Stops the ongoing screen recording.
    /// </summary>
    public void StopRecording()
    {
        screenRecorder.StopRecording();
        if (overlay != null)
        {
            overlay.Close();
        }
    }

    /// <summary>
    /// Starts recording a specific area of the screen.
    /// </summary>
    /// <param name="area">The <see cref="Rectangle"/> representing the area to record.</param>
    public void StartAreaRecording(Rectangle area)
    {
        this.overlay = new OverlayWindow();
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Recording_{timestamp}.mp4";
        string filePath = Path.Combine(Settings.Default.RecordsSavePath, fileName);
        screenRecorder.StartRecordingArea(filePath, area);
        overlay.CreateOverlayFromRect(area);
    }

    /// <summary>
    /// Captures a screenshot of a specific window.
    /// </summary>
    /// <returns>A <see cref="Bitmap"/> representing the captured window.</returns>
    public Bitmap CaptureWindow()
    {
        var windowsScreenshot = new WindowScreenshot();
        return windowsScreenshot.CaptureSingleWindow();
    }
}
