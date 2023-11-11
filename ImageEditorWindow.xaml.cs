using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace screenerWpf
{
    public partial class ImageEditorWindow : Window
    {
        private WriteableBitmap canvasBitmap;
        private BitmapSource initialImage;
        private CanvasInputHandler inputHandler;
        public ImageEditorWindow(BitmapSource initialBitmap)
        {
            InitializeComponent();
            this.initialImage = initialBitmap;

            this.inputHandler = new CanvasInputHandler(drawableCanvas, arrowColorComboBox, arrowThicknessComboBox);
            drawableCanvas.Width = initialBitmap.PixelWidth;
            drawableCanvas.Height = initialBitmap.PixelHeight;

            Loaded += (sender, e) => drawableCanvas.Focus();
            CreateCanvasBitmap();

            drawableCanvas.SizeChanged += DrawableCanvas_SizeChanged;
        }

        private void CreateCanvasBitmap()
        {
            if (initialImage != null)
            {
                canvasBitmap = new WriteableBitmap(
                    initialImage.PixelWidth,
                    initialImage.PixelHeight,
                    initialImage.DpiX,
                    initialImage.DpiY,
                    PixelFormats.Pbgra32,
                    null);
                initialImage.CopyPixels(
                    new Int32Rect(0, 0, initialImage.PixelWidth, initialImage.PixelHeight),
                    canvasBitmap.BackBuffer,
                    canvasBitmap.BackBufferStride * canvasBitmap.PixelHeight,
                    canvasBitmap.BackBufferStride);
                canvasBitmap.Freeze(); // Freeze the bitmap for performance benefits.
                UpdateCanvasBackground();
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
            // Check if the canvas has a valid size
            if (drawableCanvas.ActualWidth > 0 && drawableCanvas.ActualHeight > 0)
            {
                double scaleX = drawableCanvas.ActualWidth / initialImage.PixelWidth;
                double scaleY = drawableCanvas.ActualHeight / initialImage.PixelHeight;
                double scale = Math.Min(scaleX, scaleY); // Maintain aspect ratio

                var scaledWidth = (int)(initialImage.PixelWidth * scale);
                var scaledHeight = (int)(initialImage.PixelHeight * scale);

                canvasBitmap = new WriteableBitmap(
                    scaledWidth,
                    scaledHeight,
                    96, // Use standard 96 DPI x-density
                    96, // Use standard 96 DPI y-density
                    PixelFormats.Pbgra32,
                    null);

                // Use a Transform to scale the original image to fit the new size
                ScaleTransform transform = new ScaleTransform(scale, scale);

                // Render the scaled image onto the new WriteableBitmap
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                    scaledWidth,
                    scaledHeight,
                    96, // Use standard 96 DPI x-density
                    96, // Use standard 96 DPI y-density
                    PixelFormats.Pbgra32);

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext context = visual.RenderOpen())
                {
                    context.DrawImage(initialImage, new Rect(0, 0, scaledWidth, scaledHeight));
                }
                renderBitmap.Render(visual);

                // Now copy the pixels from the RenderTargetBitmap to the WriteableBitmap
                int stride = scaledWidth * (renderBitmap.Format.BitsPerPixel / 8);
                byte[] pixelData = new byte[stride * scaledHeight];
                renderBitmap.CopyPixels(pixelData, stride, 0);
                canvasBitmap.WritePixels(new Int32Rect(0, 0, scaledWidth, scaledHeight), pixelData, stride, 0);

                UpdateCanvasBackground();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
           inputHandler.SaveButton_Click(sender, e);
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

        public void DrawArrowButton_Click(object sender, RoutedEventArgs e)
        {
            inputHandler.DrawArrowButton_Click(sender, e);
        }

        public void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            inputHandler.AddTextButton_Click(sender, e);
        }

        private void CommandBinding_DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            inputHandler.CommandBinding_DeleteExecuted(sender, e);
        }
    }
}
