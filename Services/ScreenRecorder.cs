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
    }

    public void StartRecordingArea(string videoPath, Rectangle area)
    {
        this.videoPath = videoPath;

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

    private void OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
    {
        RecordingCompleted?.Invoke(e.FilePath); // Użyj e.FilePath zamiast videoPath, aby upewnić się, że mamy ścieżkę do gotowego pliku
        DeinitializeRecorder(); // Możesz także tutaj zdezaktywować recorder, jeśli jest to potrzebne
    }

    private void OnRecordingFailed(object sender, RecordingFailedEventArgs e)
    {
        DeinitializeRecorder(); // Możesz także tutaj zdezaktywować recorder, jeśli jest to potrzebne
    }

    private void OnStatusChanged(object sender, RecordingStatusEventArgs e)
    {
        Console.WriteLine($"Recorder status: {e.Status}");
    }
}
