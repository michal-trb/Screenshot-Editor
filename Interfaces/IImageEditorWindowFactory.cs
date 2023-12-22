using screenerWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace screenerWpf.Interfaces
{
    public interface IImageEditorWindowFactory
    {
        ImageEditorWindow Create(BitmapSource initialBitmap);
    }

}
