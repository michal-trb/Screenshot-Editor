using ScreenRecorderLib;
using System;
using System.Drawing;

class ScreenRecorder
{
    private Recorder recorder;
    private string videoPath;

    public ScreenRecorder(string videoPath)
    {
        this.videoPath = videoPath;
        recorder = Recorder.CreateRecorder();
        recorder.OnRecordingComplete += OnRecordingComplete;
        recorder.OnRecordingFailed += OnRecordingFailed;
        recorder.OnStatusChanged += OnStatusChanged;
    }

    public void StartRecording()
    {
        recorder.Record(videoPath);
    }

    public void StopRecording()
    {
        recorder.Stop();
    }

    public void StartRecordingArea(Rectangle area)
    {
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
        Console.WriteLine($"Recording complete. File saved to: {e.FilePath}");
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
