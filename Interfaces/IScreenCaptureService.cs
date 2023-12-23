using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screenerWpf.Interfaces
{
    public interface IScreenCaptureService
    {
        Bitmap CaptureScreen();
        Bitmap CaptureArea(Rectangle area);
        void StartRecording();
        void StopRecording();
        void StartAreaRecording(Rectangle area);
    }
}
