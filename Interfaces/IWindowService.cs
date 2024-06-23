using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace screenerWpf.Interfaces
{
    public interface IWindowService
    {
        Rectangle SelectArea();
        void ShowImageEditorControl(BitmapSource image);
        void ShowVideoPlayerWindow(string videoPath);

    }
}
