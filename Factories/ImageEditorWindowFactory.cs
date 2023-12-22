using screenerWpf.Controls;
using screenerWpf.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace screenerWpf.Factories
{
    public class ImageEditorWindowFactory : IImageEditorWindowFactory
    {
        public ImageEditorWindow Create(BitmapSource initialBitmap)
        { 
            return new ImageEditorWindow(initialBitmap);
        }
    }
}
