using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screenerWpf.Sevices.CaptureServices
{
    public class FullRecord
    {
        private ScreenRecorder recorder;

        public FullRecord()
        {
            recorder = new ScreenRecorder();
        }

        public void StartRecording(string videoPath)
        {
            recorder.StartRecording(videoPath);
        }

        public void StopRecording()
        {
            recorder.StopRecording();
        }
    }
}
