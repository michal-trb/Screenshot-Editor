using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Helpers.DpiHelper;
using screenerWpf.Controls;

namespace screenerWpf.Helpers
{
    public static class CanvasBitmapGenerator
    {
        public static BitmapSource GetCanvasBitmap(DrawableCanvas canvas)
        {
            int width = (int)canvas.ActualWidth;
            int height = (int)canvas.ActualHeight;

            if (width == 0 || height == 0)
            {
                return null;
            }
            var currentDpi = DpiHelper.CurrentDpi;

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                width,
                height,
                currentDpi.DpiX,
                currentDpi.DpiY,
                PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush canvasBrush = new VisualBrush(canvas);
                context.DrawRectangle(canvasBrush, null, new Rect(new Point(0, 0), new Size(width, height)));
            }

            renderBitmap.Render(visual);
            return renderBitmap;
        }
    }
}
