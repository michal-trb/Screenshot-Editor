using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screenerWpf.Sevices.CaptureServices
{
    public class AreaRecord
    {
        private ScreenRecorder recorder;

        public AreaRecord()
        {
            recorder = new ScreenRecorder();
        }

        public void StartRecordingArea(string videoPath, Rectangle area)
        {
            recorder.StartRecordingArea(videoPath, area);
        }

        public void StopRecording()
        {
            recorder.StopRecording();
        }
    }
}
