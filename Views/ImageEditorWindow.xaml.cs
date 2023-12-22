using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using screenerWpf.Controls;
using screenerWpf.Interfaces;
using screenerWpf.ViewModels;

namespace screenerWpf
{
    public partial class ImageEditorWindow : Window
    {
        private WriteableBitmap canvasBitmap;
        private BitmapSource initialImage;
        private readonly ICanvasInputHandler inputHandler;

        public ImageEditorWindow(BitmapSource initialBitmap)
        {
            InitializeComponent();
            this.initialImage = initialBitmap;
            this.inputHandler = new CanvasInputHandler(drawableCanvas);

            drawableCanvas.Width = initialBitmap.PixelWidth;
            drawableCanvas.Height = initialBitmap.PixelHeight;

            Loaded += (sender, e) => drawableCanvas.Focus();
            CreateCanvasBitmap();

            drawableCanvas.SizeChanged += DrawableCanvas_SizeChanged;
            drawableCanvas.PreviewKeyDown += this.inputHandler.Canvas_PreviewKeyDown;
            var viewModel = new ImageEditorViewModel(this.inputHandler);
            DataContext = viewModel;

            // Podłączenie zdarzeń ViewModel do metod obsługi okna
            viewModel.MinimizeRequest += MinimizeWindow;
            viewModel.MaximizeRestoreRequest += MaximizeRestoreWindow;
            viewModel.CloseRequest += CloseWindow;
        }

        private void CreateCanvasBitmap()
        {
            if (initialImage != null)
            {
                canvasBitmap = CreateWriteableBitmapFromSource(initialImage);
                UpdateCanvasBackground();
            }
        }

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

            bitmap.Freeze(); // Freeze the bitmap for performance benefits.
            return bitmap;
        }

        private void MinimizeWindow()
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreWindow()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow()
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void UpdateCanvasBackground()
        {
            ImageBrush brush = new ImageBrush(canvasBitmap)
            {
                Stretch = Stretch.Uniform // This will ensure the image is scaled properly.
            };
            drawableCanvas.Background = brush;
        }

        private void DrawableCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (drawableCanvas.ActualWidth > 0 && drawableCanvas.ActualHeight > 0 && initialImage != null)
            {
                canvasBitmap = ScaleBitmap(initialImage, drawableCanvas.ActualWidth, drawableCanvas.ActualHeight);
                UpdateCanvasBackground();
            }
        }

        private WriteableBitmap ScaleBitmap(BitmapSource source, double actualWidth, double actualHeight)
        {
            double scaleX = actualWidth / source.PixelWidth;
            double scaleY = actualHeight / source.PixelHeight;
            double scale = Math.Min(scaleX, scaleY);

            var scaledWidth = (int)(source.PixelWidth * scale);
            var scaledHeight = (int)(source.PixelHeight * scale);

            return RenderScaledBitmap(source, scaledWidth, scaledHeight);
        }

        private WriteableBitmap RenderScaledBitmap(BitmapSource source, int width, int height)
        {
            var renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var context = visual.RenderOpen())
            {
                var rect = new Rect(0, 0, width, height);
                context.DrawImage(source, rect);
            }

            renderBitmap.Render(visual);

            return ConvertRenderTargetToWriteableBitmap(renderBitmap);
        }

        private WriteableBitmap ConvertRenderTargetToWriteableBitmap(RenderTargetBitmap renderBitmap)
        {
            var pixelData = new byte[renderBitmap.PixelWidth * renderBitmap.PixelHeight * (renderBitmap.Format.BitsPerPixel / 8)];
            renderBitmap.CopyPixels(pixelData, renderBitmap.PixelWidth * (renderBitmap.Format.BitsPerPixel / 8), 0);

            var writeableBitmap = new WriteableBitmap(renderBitmap.PixelWidth, renderBitmap.PixelHeight, 96, 96, PixelFormats.Pbgra32, null);
            writeableBitmap.WritePixels(new Int32Rect(0, 0, renderBitmap.PixelWidth, renderBitmap.PixelHeight), pixelData, renderBitmap.PixelWidth * (renderBitmap.Format.BitsPerPixel / 8), 0);

            return writeableBitmap;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            inputHandler.Canvas_MouseLeftButtonDown(sender, e);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            inputHandler.Canvas_MouseLeftButtonUp(sender, e);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            inputHandler.Canvas_MouseMove(sender, e);
        }

        private void CommandBinding_DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            inputHandler.CommandBinding_DeleteExecuted(sender, e);
        }
    }
}
