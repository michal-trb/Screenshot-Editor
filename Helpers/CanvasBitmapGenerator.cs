namespace screenerWpf.Helpers;

using global::Helpers.DpiHelper;
using screenerWpf.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// <summary>
/// A helper class for generating a <see cref="BitmapSource"/> from a <see cref="DrawableCanvas"/>.
/// </summary>
public static class CanvasBitmapGenerator
{
    /// <summary>
    /// Generates a bitmap representation of the given <see cref="DrawableCanvas"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="DrawableCanvas"/> to convert to a bitmap.</param>
    /// <returns>
    /// A <see cref="BitmapSource"/> representing the visual content of the canvas, or <c>null</c> if the canvas dimensions are zero.
    /// </returns>
    public static BitmapSource? GetCanvasBitmap(DrawableCanvas canvas)
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
