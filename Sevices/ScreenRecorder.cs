using ScreenRecorderLib;
using System;
using System.Drawing;

class ScreenRecorder
{
    private Recorder recorder;
    public event Action<string> RecordingCompleted;
    private string videoPath;

    public ScreenRecorder()
    {
        recorder = Recorder.CreateRecorder();
        recorder.OnRecordingComplete += OnRecordingComplete;
        recorder.OnRecordingFailed += OnRecordingFailed;
        recorder.OnStatusChanged += OnStatusChanged;
    }

    public void StartRecording(string videoPath)
    {
        this.videoPath = videoPath;
        recorder.Record(videoPath);
    }

    public void StopRecording()
    {
        recorder.Stop();
        RecordingCompleted?.Invoke(videoPath);
    }

    public void StartRecordingArea(string videoPath, Rectangle area)
    {
        this.videoPath = videoPath;
        RecorderOptions options = new RecorderOptions
        {
            OutputOptions = new OutputOptions
            {
                RecorderMode = RecorderMode.Video,
                OutputFrameSize = new ScreenSize(area.Width, area.Height),
                Stretch = StretchMode.Uniform,
                SourceRect = new ScreenRect(area.Left, area.Top, area.Width, area.Height)
            },
        };

        recorder = Recorder.CreateRecorder(options);
        recorder.Record(videoPath);
    }

    private void OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
    {
    }

    private void OnRecordingFailed(object sender, RecordingFailedEventArgs e)
    {
        Console.WriteLine($"Recording failed: {e.Error}");
    }

    private void OnStatusChanged(object sender, RecordingStatusEventArgs e)
    {
        Console.WriteLine($"Recorder status: {e.Status}");
    }
}
