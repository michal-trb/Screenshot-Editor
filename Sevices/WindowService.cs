using screenerWpf.Controls;
using screenerWpf.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace screenerWpf.Sevices
{
    public class WindowService : IWindowService
    {
        private readonly IImageEditorWindowFactory imageEditorWindowFactory;

        public WindowService(IImageEditorWindowFactory imageEditorWindowFactory)
        {
            this.imageEditorWindowFactory = imageEditorWindowFactory;
        }

        public void ShowImageEditorWindow(BitmapSource image)
        {
            var editor = imageEditorWindowFactory.Create(image);
            editor.ShowDialog();
        }

        public Rectangle SelectArea()
        {
            AreaSelector selector = new AreaSelector();
            bool? result = selector.ShowDialog();
            if (result == true)
            {
                return new Rectangle(
                    (int)selector.SelectedRectangle.X,
                    (int)selector.SelectedRectangle.Y,
                    (int)selector.SelectedRectangle.Width,
                    (int)selector.SelectedRectangle.Height);
            }
            return Rectangle.Empty;
        }
    }
}
