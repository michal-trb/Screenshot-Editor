using System.Drawing;
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
        Task CaptureWithScrollAsync();
    }
}
