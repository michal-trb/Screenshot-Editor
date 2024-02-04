using ScreenRecorderLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

class ScreenRecorder
{
    private Recorder recorder;
    public event Action<string> RecordingCompleted;
    private string videoPath;

    public ScreenRecorder()
    {
        // Konstruktor jest teraz pusty
    }

    private void InitializeRecorder()
    {
        // Inicjalizacja Recordera
        recorder = Recorder.CreateRecorder();
        recorder.OnRecordingComplete += OnRecordingComplete;
        recorder.OnRecordingFailed += OnRecordingFailed;
        recorder.OnStatusChanged += OnStatusChanged;
    }

    public void StartRecording(string videoPath)
    {
        InitializeRecorder();
        this.videoPath = videoPath;
        recorder.Record(videoPath);
    }

    public void StopRecording()
    {
        recorder.Stop();
        RecordingCompleted?.Invoke(videoPath);
        // Opcjonalnie zdezaktywuj i wyczyść obiekt recorder po zakończeniu nagrania
        DeinitializeRecorder();
    }

    public void StartRecordingArea(string videoPath, Rectangle area)
    {
        this.videoPath = videoPath;
        area = AdjustAreaToVirtualScreen(area);

        // Wykrywanie dostępnych monitorów i dodawanie ich jako źródeł nagrania
        var monitors = Recorder.GetDisplays();
        var sources = new List<RecordingSourceBase>();

        foreach (var monitor in monitors)
        {
            sources.Add(new DisplayRecordingSource(monitor.DeviceName));
        }

        RecorderOptions options = new RecorderOptions
        {
            SourceOptions = new SourceOptions
            {
                RecordingSources = sources
            },
            OutputOptions = new OutputOptions
            {
                RecorderMode = RecorderMode.Video,
                OutputFrameSize = new ScreenSize(area.Width, area.Height),
                Stretch = StretchMode.Uniform,
                SourceRect = new ScreenRect(area.Left, area.Top, area.Width, area.Height)
            },
        };

        recorder = Recorder.CreateRecorder(options);
        recorder.OnRecordingComplete += OnRecordingComplete;
        recorder.OnRecordingFailed += OnRecordingFailed;
        recorder.OnStatusChanged += OnStatusChanged;
        recorder.Record(videoPath);
    }

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

    private Rectangle AdjustAreaToVirtualScreen(Rectangle area)
    {
        int virtualScreenLeft = Convert.ToInt32(SystemParameters.VirtualScreenLeft);
        int virtualScreenTop = Convert.ToInt32(SystemParameters.VirtualScreenTop);

        Rectangle adjustedArea = new Rectangle(
            area.Left - virtualScreenLeft,
            area.Top - virtualScreenTop,
            area.Width,
            area.Height);

        return adjustedArea;
    }

    private void OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
    {
        // Obsługa zdarzenia zakończenia nagrywania
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
