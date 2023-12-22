using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace screenerWpf.Interfaces
{
    public interface IWindowService
    {
        Rectangle SelectArea();
        void ShowImageEditorWindow(BitmapSource image);
    }
}
