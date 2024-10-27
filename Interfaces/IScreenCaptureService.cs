namespace screenerWpf.Interfaces;

using System.Drawing;

/// <summary>
/// Interface that provides methods for screen capturing and video recording services.
/// </summary>
public interface IScreenCaptureService
{
    /// <summary>
    /// Captures the entire screen as a bitmap image.
    /// </summary>
    /// <returns>A <see cref="Bitmap"/> representing the captured screen.</returns>
    Bitmap CaptureScreen();

    /// <summary>
    /// Captures a specific rectangular area of the screen as a bitmap image.
    /// </summary>
    /// <param name="area">The <see cref="Rectangle"/> that defines the area to capture.</param>
    /// <returns>A <see cref="Bitmap"/> of the specified screen area.</returns>
    Bitmap CaptureArea(Rectangle area);

    /// <summary>
    /// Captures the screen under the cursor as a bitmap image.
    /// </summary>
    /// <returns>A <see cref="Bitmap"/> representing the captured screen under the cursor.</returns>
    Bitmap CaptureScreenUnderCursor(System.Drawing.Point cursorPosition);

    /// <summary>
    /// Starts recording the entire screen.
    /// </summary>
    void StartRecording();

    /// <summary>
    /// Stops the ongoing screen recording.
    /// </summary>
    void StopRecording();

    /// <summary>
    /// Starts recording a specific rectangular area of the screen.
    /// </summary>
    /// <param name="area">The <see cref="Rectangle"/> that defines the area to record.</param>
    void StartAreaRecording(Rectangle area);

    /// <summary>
    /// Captures the active window as a bitmap image.
    /// </summary>
    /// <returns>A <see cref="Bitmap"/> representing the captured active window.</returns>
    Bitmap CaptureWindow();
}
