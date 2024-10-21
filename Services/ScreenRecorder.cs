namespace screenerWpf.Sevices;

using ScreenRecorderLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

/// <summary>
/// Class responsible for recording screen or specific areas using ScreenRecorderLib.
/// </summary>
class ScreenRecorder
{
    private Recorder recorder;
    public event Action<string> RecordingCompleted;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenRecorder"/> class.
    /// </summary>
    public ScreenRecorder() { }

    /// <summary>
    /// Starts recording the full screen.
    /// </summary>
    /// <param name="videoPath">The path where the recorded video will be saved.</param>
    public void StartRecording(string videoPath)
    {
        InitializeRecorder();
        recorder.Record(videoPath);
    }

    /// <summary>
    /// Stops the ongoing recording.
    /// </summary>
    public void StopRecording()
    {
        recorder.Stop();
    }

    /// <summary>
    /// Starts recording a specific area of the screen.
    /// </summary>
    /// <param name="videoPath">The path where the recorded video will be saved.</param>
    /// <param name="area">The area to record, represented as a <see cref="Rectangle"/>.</param>
    public void StartRecordingArea(string videoPath, Rectangle area)
    {
        double left = area.Left;
        double top = area.Top;
        double width = area.Width;
        double height = area.Height;
        var sources = new List<RecordingSourceBase>();

        sources.AddRange(Recorder.GetDisplays());

        RecorderOptions options = new RecorderOptions
        {
            SourceOptions = new SourceOptions
            {
                RecordingSources = sources
            },
            OutputOptions = new OutputOptions
            {
                RecorderMode = RecorderMode.Video,
                Stretch = StretchMode.Uniform,
                SourceRect = new ScreenRect(left, top, width, height)
            },
        };

        recorder = Recorder.CreateRecorder(options);
        recorder.Record(videoPath);
        recorder.OnRecordingComplete += OnRecordingComplete;
        recorder.OnRecordingFailed += OnRecordingFailed;
        recorder.OnStatusChanged += OnStatusChanged;
    }

    /// <summary>
    /// Event handler for when the recording is completed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RecordingCompleteEventArgs"/> that contains event data.</param>
    private void OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
    {
        RecordingCompleted?.Invoke(e.FilePath);
        DeinitializeRecorder();
    }

    /// <summary>
    /// Event handler for when the recording has failed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RecordingFailedEventArgs"/> that contains event data.</param>
    private void OnRecordingFailed(object sender, RecordingFailedEventArgs e)
    {
        DeinitializeRecorder();
    }

    /// <summary>
    /// Event handler for when the recorder status changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RecordingStatusEventArgs"/> that contains event data.</param>
    private void OnStatusChanged(object sender, RecordingStatusEventArgs e)
    {
        Console.WriteLine($"Recorder status: {e.Status}");
    }

    /// <summary>
    /// Initializes the recorder and subscribes to its events.
    /// </summary>
    private void InitializeRecorder()
    {
        recorder = Recorder.CreateRecorder();
        recorder.OnRecordingComplete += OnRecordingComplete;
        recorder.OnRecordingFailed += OnRecordingFailed;
        recorder.OnStatusChanged += OnStatusChanged;
    }

    /// <summary>
    /// Deinitializes the recorder and unsubscribes from its events.
    /// </summary>
    private void DeinitializeRecorder()
    {
        if (recorder != null)
        {
            recorder.OnRecordingComplete -= OnRecordingComplete;
            recorder.OnRecordingFailed -= OnRecordingFailed;
            recorder.OnStatusChanged -= OnStatusChanged;
            recorder.Dispose();
            recorder = null;
        }
    }
}
