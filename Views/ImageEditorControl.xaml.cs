namespace screenerWpf;

using global::Helpers.DpiHelper;
using screenerWpf.Controls;
using screenerWpf.Interfaces;
using screenerWpf.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public partial class ImageEditorControl : UserControl
{
    private WriteableBitmap canvasBitmap;
    private BitmapSource initialImage;
    private readonly ICanvasInputHandler inputHandler;
    public event Action WindowClosed;
    public event Action<ImageEditorControl> LoadedAndSizeUpdated;

    /// <summary>
    /// Initializes a new instance of the ImageEditorControl class.
    /// Sets up initial image, input handler, and attaches event handlers.
    /// </summary>
    /// <param name="initialBitmap">The initial bitmap to be used in the editor.</param>
    public ImageEditorControl(BitmapSource initialBitmap)
    {
        InitializeComponent();
        initialImage = initialBitmap;
        inputHandler = new CanvasInputHandler(drawableCanvas);

        drawableCanvas.Width = initialBitmap.PixelWidth;
        drawableCanvas.Height = initialBitmap.PixelHeight;

        Loaded += (sender, e) =>
        {
            drawableCanvas.Focus();
            AdjustControlSize();
            OnLoadedAndSizeUpdated();
        };
        CreateCanvasBitmap();

        drawableCanvas.SizeChanged += DrawableCanvas_SizeChanged;

        var viewModel = new ImageEditorViewModel(inputHandler, drawableCanvas, initialBitmap);

        DataContext = viewModel;
    }

    /// <summary>
    /// Adjusts the control size based on the dimensions of the initial image.
    /// Ensures minimum width and height for the control.
    /// </summary>
    private void AdjustControlSize()
    {
        double targetWidth = initialImage.PixelWidth * 0.7;
        double targetHeight = initialImage.PixelHeight * 0.7;

        targetWidth = Math.Max(targetWidth, 920);
        targetHeight = Math.Max(targetHeight, 600);

        Width = targetWidth;
        Height = targetHeight;
    }

    /// <summary>
    /// Invokes the LoadedAndSizeUpdated event after the control is loaded and size adjusted.
    /// </summary>
    private void OnLoadedAndSizeUpdated()
    {
        LoadedAndSizeUpdated?.Invoke(this);
    }

    /// <summary>
    /// Creates a writeable bitmap from the initial image to be used as a canvas background.
    /// </summary>
    private void CreateCanvasBitmap()
    {
        if (initialImage != null)
        {
            canvasBitmap = CreateWriteableBitmapFromSource(initialImage);
            UpdateCanvasBackground();
        }
    }

    /// <summary>
    /// Creates a WriteableBitmap from the given BitmapSource.
    /// </summary>
    /// <param name="source">The BitmapSource to create the WriteableBitmap from.</param>
    /// <returns>A WriteableBitmap representation of the source image.</returns>
    private WriteableBitmap CreateWriteableBitmapFromSource(BitmapSource source)
    {
        var bitmap = new WriteableBitmap(
            source.PixelWidth,
            source.PixelHeight,
            source.DpiX,
            source.DpiY,
            PixelFormats.Pbgra32,
            null);

        source.CopyPixels(
            new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight),
            bitmap.BackBuffer,
            bitmap.BackBufferStride * bitmap.PixelHeight,
            bitmap.BackBufferStride);

        bitmap.Freeze(); // Freeze the bitmap for performance optimization.
        return bitmap;
    }

    /// <summary>
    /// Updates the canvas background with the current canvas bitmap.
    /// </summary>
    private void UpdateCanvasBackground()
    {
        ImageBrush brush = new ImageBrush(canvasBitmap)
        {
            Stretch = Stretch.Uniform
        };
        drawableCanvas.Background = brush;
    }

    /// <summary>
    /// Handles size changes of the drawable canvas.
    /// Rescales the bitmap accordingly.
    /// </summary>
    private void DrawableCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (drawableCanvas.ActualWidth > 0 && drawableCanvas.ActualHeight > 0 && initialImage != null)
        {
            canvasBitmap = ScaleBitmap(initialImage, drawableCanvas.ActualWidth, drawableCanvas.ActualHeight);
            UpdateCanvasBackground();
        }
    }

    /// <summary>
    /// Scales the bitmap to the given dimensions.
    /// </summary>
    /// <param name="source">The source BitmapSource to scale.</param>
    /// <param name="actualWidth">The target width for scaling.</param>
    /// <param name="actualHeight">The target height for scaling.</param>
    /// <returns>A WriteableBitmap that has been scaled to the given dimensions.</returns>
    private WriteableBitmap ScaleBitmap(BitmapSource source, double actualWidth, double actualHeight)
    {
        double scaleX = actualWidth / source.PixelWidth;
        double scaleY = actualHeight / source.PixelHeight;
        double scale = Math.Min(scaleX, scaleY);

        var scaledWidth = (int)(source.PixelWidth * scale);
        var scaledHeight = (int)(source.PixelHeight * scale);

        return RenderScaledBitmap(source, scaledWidth, scaledHeight);
    }

    /// <summary>
    /// Renders a scaled bitmap using a DrawingVisual.
    /// </summary>
    /// <param name="source">The source BitmapSource to render.</param>
    /// <param name="width">The target width for the rendered bitmap.</param>
    /// <param name="height">The target height for the rendered bitmap.</param>
    /// <returns>A WriteableBitmap representation of the rendered bitmap.</returns>
    private WriteableBitmap RenderScaledBitmap(BitmapSource source, int width, int height)
    {
        var currentDpi = DpiHelper.CurrentDpi;

        var renderBitmap = new RenderTargetBitmap(
            width,
            height,
            currentDpi.DpiX,
            currentDpi.DpiY,
            PixelFormats.Pbgra32);

        var visual = new DrawingVisual();

        using (var context = visual.RenderOpen())
        {
            var rect = new Rect(0, 0, width, height);
            context.DrawImage(source, rect);
        }

        renderBitmap.Render(visual);

        return ConvertRenderTargetToWriteableBitmap(renderBitmap);
    }

    /// <summary>
    /// Converts a RenderTargetBitmap to a WriteableBitmap.
    /// </summary>
    /// <param name="renderBitmap">The RenderTargetBitmap to convert.</param>
    /// <returns>A WriteableBitmap representation of the RenderTargetBitmap.</returns>
    private WriteableBitmap ConvertRenderTargetToWriteableBitmap(RenderTargetBitmap renderBitmap)
    {
        var pixelData = new byte[renderBitmap.PixelWidth * renderBitmap.PixelHeight * (renderBitmap.Format.BitsPerPixel / 8)];
        renderBitmap.CopyPixels(pixelData, renderBitmap.PixelWidth * (renderBitmap.Format.BitsPerPixel / 8), 0);

        var writeableBitmap = new WriteableBitmap(renderBitmap.PixelWidth, renderBitmap.PixelHeight, 96, 96, PixelFormats.Pbgra32, null);

        writeableBitmap.WritePixels(new Int32Rect(0, 0, renderBitmap.PixelWidth, renderBitmap.PixelHeight), pixelData, renderBitmap.PixelWidth * (renderBitmap.Format.BitsPerPixel / 8), 0);

        return writeableBitmap;
    }

    /// <summary>
    /// Handles mouse left button down events on the canvas.
    /// </summary>
    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        inputHandler.Canvas_MouseLeftButtonDown(sender, e);
    }

    /// <summary>
    /// Handles mouse double click events on the canvas.
    /// </summary>
    private void Canvas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DrawableCanvas canvas)
        {
            inputHandler.Canvas_MouseDoubleClick(sender, e);
        }
    }

    /// <summary>
    /// Handles mouse left button up events on the canvas.
    /// </summary>
    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        inputHandler.Canvas_MouseLeftButtonUp(sender, e);
    }

    /// <summary>
    /// Handles mouse move events on the canvas.
    /// </summary>
    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        inputHandler.Canvas_MouseMove(sender, e);
    }
}
