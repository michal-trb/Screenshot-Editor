using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace screenerWpf
{
    public partial class ImageEditorWindow : Window, INotifyPropertyChanged
    {
        private WriteableBitmap canvasBitmap;
        private BitmapSource initialImage;
        private CanvasInputHandler inputHandler;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ImageEditorWindow(BitmapSource initialBitmap)
        {
            InitializeComponent();
            this.initialImage = initialBitmap;
            this.inputHandler = new CanvasInputHandler(
                drawableCanvas);
            drawableCanvas.Width = initialBitmap.PixelWidth;
            drawableCanvas.Height = initialBitmap.PixelHeight;

            Loaded += (sender, e) => drawableCanvas.Focus();
            CreateCanvasBitmap();
            InitializeFontFamilyComboBox(); // Wywołaj metodę inicjalizującą ComboBox
            InitializeFontSizeComboBox();
            InitializeTransparencyComboBox();
            colorComboBox.SelectedIndex = 0; // Zakładając, że Czarny jest pierwszym elementem
            arrowThicknessComboBox.SelectedIndex = 1; // Zakładając, że "2" jest drugim elementem
            transparencyComboBox.SelectedIndex = 10;

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
                    new Int32Rect(
                        0,
                        0,
                        initialImage.PixelWidth,
                        initialImage.PixelHeight),
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
                    context.DrawImage(initialImage, new Rect(
                        0,
                        0,
                        scaledWidth,
                        scaledHeight));
                }
                renderBitmap.Render(visual);

                // Now copy the pixels from the RenderTargetBitmap to the WriteableBitmap
                int stride = scaledWidth * (renderBitmap.Format.BitsPerPixel / 8);
                byte[] pixelData = new byte[stride * scaledHeight];
                renderBitmap.CopyPixels(pixelData, stride, 0);
                canvasBitmap.WritePixels(
                    new Int32Rect(0, 0, scaledWidth, scaledHeight),
                    pixelData,
                    stride,
                    0);

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

        private void CommandBinding_DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            inputHandler.CommandBinding_DeleteExecuted(sender, e);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void ArrowThicknessComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (arrowThicknessComboBox.SelectedItem is ComboBoxItem selectedThicknessItem
                && double.TryParse(selectedThicknessItem.Content.ToString(), out double selectedThickness))
            {
                inputHandler.ArrowThicknessComboBox_SelectionChanged(selectedThickness);
            }
        }

        public void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (colorComboBox.SelectedItem is ComboBoxItem selectedColorItem
                && selectedColorItem.Background is SolidColorBrush colorBrush)
            {
                // Update the current color in the canvas
                inputHandler.ColorComboBox_SelectionChanged(colorBrush.Color);
            }
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontFamilyComboBox.SelectedItem is ComboBoxItem selectedFontFamilyItem)
            {
                var fontFamily = new FontFamily(selectedFontFamilyItem.Content.ToString());
                inputHandler.FontFamilyComboBox_SelectionChanged(fontFamily);
            }
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontSizeComboBox.SelectedItem is ComboBoxItem selectedFontSizeItem
                && double.TryParse(selectedFontSizeItem.Content.ToString(), out double fontSize))
            {
                inputHandler.FontSizeComboBox_SelectionChanged(fontSize);
            }
        }

        private void TransparencyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (transparencyComboBox.SelectedItem is ComboBoxItem selectedTransparencyItem
                && double.TryParse(selectedTransparencyItem.Content.ToString(), out double transparency))
            {
                inputHandler.TransparencyComboBox_SelectionChanged(transparency);
            }
        }

        private void InitializeFontFamilyComboBox()
        {
            var fontFamilies = new List<string>
            {
                "Arial",
                "Calibri",
                "Times New Roman",
                "Verdana",
                "Courier New"
            };

            foreach (var fontFamily in fontFamilies)
            {
                fontFamilyComboBox.Items.Add(new ComboBoxItem
                {
                    Content = fontFamily
                });
            }
            var defaultFontFamily = fontFamilyComboBox.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == "Arial");
            if (defaultFontFamily != null)
            {
                fontFamilyComboBox.SelectedItem = defaultFontFamily;
            }
        }

        private void InitializeFontSizeComboBox()
        {
            var fontSizes = new[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40 };
            foreach (var size in fontSizes)
            {
                fontSizeComboBox.Items.Add(new ComboBoxItem
                {
                    Content = size.ToString()
                });
            }
            var defaultFontSize = fontSizeComboBox.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == "12");
            if (defaultFontSize != null)
            {
                fontSizeComboBox.SelectedItem = defaultFontSize;
            }
        }

        private void InitializeTransparencyComboBox()
        {
            var transparencySizes = new[] { 10,20,30,40,50,60,70,80,90,100 };
            foreach (var size in transparencySizes)
            {
                transparencyComboBox.Items.Add(new ComboBoxItem
                {
                    Content = size.ToString()
                });
            }
            var defaultFontSize = transparencyComboBox.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == "100");
            if (defaultFontSize != null)
            {
                transparencyComboBox.SelectedItem = defaultFontSize;
            }
        }

        private bool isColorComboBoxSelected;

        public bool IsColorComboBoxSelected
        {
            get => isColorComboBoxSelected;
            set
            {
                isColorComboBoxSelected = value;
                OnPropertyChanged(nameof(IsColorComboBoxSelected));
            }
        }

        private bool isThicknessComboBoxSelected;

        public bool IsThicknessComboBoxSelected
        {
            get { return isThicknessComboBoxSelected; }
            set
            {
                isThicknessComboBoxSelected = value;
                OnPropertyChanged(nameof(IsThicknessComboBoxSelected));
            }
        }

        private bool isFontFamilySelected;

        public bool IsFontFamilySelected
        {
            get { return isFontFamilySelected; }
            set
            {
                isFontFamilySelected = value;
                OnPropertyChanged(nameof(IsFontFamilySelected));
            }
        }

        private bool isTransparencySelected;

        public bool IsTransparencySelected
        {
            get { return isTransparencySelected; }
            set
            {
                isTransparencySelected = value;
                OnPropertyChanged(nameof(IsTransparencySelected));
            }
        }

        private bool isFontSizeSelected;

        public bool IsFontSizeSelected
        {
            get { return isFontSizeSelected; }
            set
            {
                isFontSizeSelected = value;
                OnPropertyChanged(nameof(IsFontSizeSelected));
            }
        }
        public void DrawArrowButton_Click(object sender, RoutedEventArgs e)
        {
            IsThicknessComboBoxSelected = true;
            IsColorComboBoxSelected = true;
            IsFontSizeSelected = false;
            IsFontFamilySelected = false;
            IsTransparencySelected = false;

            inputHandler.DrawArrowButton_Click(sender, e);
        }

        public void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            IsThicknessComboBoxSelected = false;
            IsColorComboBoxSelected = true;
            IsFontSizeSelected = true;
            IsFontFamilySelected = true;
            IsTransparencySelected = false;

            inputHandler.AddTextButton_Click(sender, e);
        }


        public void DrawRectButton_Click(object sender, RoutedEventArgs e)
        {
            IsThicknessComboBoxSelected = true;
            IsColorComboBoxSelected = true;
            IsFontSizeSelected = false;
            IsFontFamilySelected = false;
            IsTransparencySelected = true;

            inputHandler.DrawRectButton_Click(sender, e);
        }

        private void SpeechBubbleButton_Click(object sender, RoutedEventArgs e)
        {
            IsThicknessComboBoxSelected = true;
            IsColorComboBoxSelected = true;
            IsFontSizeSelected = true;
            IsFontFamilySelected = true;
            IsTransparencySelected = false;

            inputHandler.SpeechBubbleButton_Click(sender, e);
        }

        private void BlurButton_Click(object sender, RoutedEventArgs e)
        {
            IsThicknessComboBoxSelected = false;
            IsColorComboBoxSelected = false;
            IsFontSizeSelected = false;
            IsFontFamilySelected = false;
            IsTransparencySelected = false;

            inputHandler.BlurButton_Click(sender, e);
        }

        private void BrushButton_Click(object sender, RoutedEventArgs e)
        {
            IsThicknessComboBoxSelected = true;
            IsColorComboBoxSelected = true;
            IsFontSizeSelected = false;
            IsFontFamilySelected = false;
            IsTransparencySelected = true;
            inputHandler.BrushButton_Click(sender, e);
        }
    }
}
