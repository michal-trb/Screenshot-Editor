using screenerWpf.Commands;
using screenerWpf.Controls;
using screenerWpf.Helpers;
using screenerWpf.Interfaces;
using screenerWpf.Models;
using screenerWpf.Models.DrawableElements;
using screenerWpf.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace screenerWpf.ViewModels
{
    internal class ImageEditorViewModel : INotifyPropertyChanged
    {
        private BitmapSource initialImage; // Dodano właściwość dla obrazu inicjalnego

        private IDrawable copiedElement;

        private readonly ICanvasInputHandler inputHandler;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand MinimizeCommand { get; private set; }
        public ICommand MaximizeRestoreCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand SavePdfCommand { get; private set; }
        public ICommand DrawArrowCommand { get; private set; }
        public ICommand AddTextCommand { get; private set; }
        public ICommand DrawRectCommand { get; private set; }
        public ICommand SpeechBubbleCommand { get; private set; }
        public ICommand BlurCommand { get; private set; }
        public ICommand BrushCommand { get; private set; }
        public ICommand RecognizeTextCommand { get; private set; }
        public ICommand UploadToDropboxCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ResizeCommand { get; private set; }
        public ICommand AddNiceBackgroundCommand { get; private set; }
        public ObservableCollection<ColorInfo> Colors { get; private set; }
        public ObservableCollection<int> Thicknesses { get; private set; }
        public ObservableCollection<string> FontFamilies { get; private set; }
        public ObservableCollection<int> FontSizes { get; private set; }
        public ObservableCollection<int> TransparencySizes { get; private set; }
        public event Action MinimizeRequest;
        public event Action MaximizeRestoreRequest;
        public event Action CloseRequest;
        private readonly DrawableCanvas drawableCanvas;

        public ImageEditorViewModel(ICanvasInputHandler inputHandler, DrawableCanvas drawableCanvas, BitmapSource initialBitmap)
        {
            this.inputHandler = inputHandler ?? throw new ArgumentNullException(nameof(inputHandler));
            this.drawableCanvas = drawableCanvas;
            this.initialImage = initialBitmap; // Przypisanie obrazu inicjalnego

            InitializeCommands();
            InitializeThicknesses();
            InitializeFontFamilies();
            InitializeFontSizes();
            InitializeTransparencySizes();
            InitializeStartValues();

            MinimizeCommand = new RelayCommand(_ => OnMinimize());
            MaximizeRestoreCommand = new RelayCommand(_ => OnMaximizeRestore());
            CloseCommand = new RelayCommand(_ => OnClose());

            UploadToDropboxCommand = new RelayCommand(async _ => await new DropboxUploader().UploadFileAsync(ExecuteSaveFast()));
            this.drawableCanvas = drawableCanvas;
        }

        private void InitializeStartValues()
        {
            SelectedFontFamily = "Arial";
            SelectedFontSize = 12;
            SelectedThickness = 2;
            SelectedTransparency = 100;
            SelectedColor = System.Windows.Media.Colors.Black;
        }

        private void InitializeTransparencySizes()
        {
            TransparencySizes = new ObservableCollection<int> { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        }

        private void InitializeFontFamilies()
        {
            FontFamilies = new ObservableCollection<string>
            {
                "Arial", "Calibri", "Times New Roman", "Verdana", "Courier New"
            };
        }

        private void InitializeFontSizes()
        {
            FontSizes = new ObservableCollection<int> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40 };
        }

        private void InitializeThicknesses()
        {
            Thicknesses = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }

        private void InitializeCommands()
        {
            SavePdfCommand = new RelayCommand(ExecuteSavePdf);
            DrawArrowCommand = new RelayCommand(ExecuteDrawArrow);
            AddTextCommand = new RelayCommand(ExecuteAddText);
            DrawRectCommand = new RelayCommand(ExecuteDrawRect);
            SpeechBubbleCommand = new RelayCommand(ExecuteSpeechBubble);
            BlurCommand = new RelayCommand(ExecuteBlur);
            BrushCommand = new RelayCommand(ExecuteBrush);
            RecognizeTextCommand = new RelayCommand(ExecuteRecognizeText);
            UndoCommand = new RelayCommand(ExecuteUndo);
            RedoCommand = new RelayCommand(ExecuteRedo);
            CopyCommand = new RelayCommand(ExecuteCopy);
            PasteCommand = new RelayCommand(ExecutePaste);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            SaveCommand = new RelayCommand(ExecuteSave);
            ResizeCommand = new RelayCommand(ExecuteResize);
            AddNiceBackgroundCommand = new RelayCommand(ExecuteAddNiceBackground);
        }

        private void OnMinimize()
        {
            MinimizeRequest?.Invoke();
        }

        private void OnMaximizeRestore()
        {
            MaximizeRestoreRequest?.Invoke();
        }

        private void OnClose()
        {
            CloseRequest?.Invoke();
        }

        private void ExecuteDrawArrow(object obj)
        {

            UpdateVisibility(true, true, false, false, false);
            inputHandler.DrawArrow();
        }
        private void ExecuteAddText(object obj)
        {
            UpdateVisibility(true, false, true, true, false);
            inputHandler.AddText();
        }

        private void ExecuteDrawRect(object obj)
        {
            UpdateVisibility(true, true, false, false, true);
            inputHandler.DrawRect();
        }

        private void ExecuteSpeechBubble(object obj)
        {
            UpdateVisibility(true, true, true, true, false);
            inputHandler.SpeechBubble();
        }

        private void ExecuteBlur(object obj)
        {
            UpdateVisibility(false, false, false, false, false);
            inputHandler.Blur();
        }

        private void ExecuteBrush(object obj)
        {
            UpdateVisibility(true, true, false, false, true);
            inputHandler.Brush();
        }

        private void ExecuteRecognizeText(object obj)
        {
            UpdateVisibility(false, true, false, false, false);
            inputHandler.RecognizeText();
        }

        private void ExecuteSave(object obj)
        {
            inputHandler.Save();
        }

        private void ExecuteSavePdf(object obj)
        {
            inputHandler.SavePdf();
        }

        private string ExecuteSaveFast()
        {
            return inputHandler.SaveFast();
        }

        private void UpdateVisibility(
            bool color,
            bool thickness,
            bool fontFamily,
            bool fontSize,
            bool transparency)
        {
            IsColorVisible = color;
            IsThicknessVisible = thickness;
            IsFontFamilyVisible = fontFamily;
            IsFontSizeVisible = fontSize;
            IsTransparencyVisible = transparency;
        }

        private bool isColorVisible;
        public bool IsColorVisible
        {
            get => isColorVisible;
            set
            {
                isColorVisible = value;
                OnPropertyChanged(nameof(IsColorVisible));
            }
        }

        private bool isThicknessVisible;
        public bool IsThicknessVisible
        {
            get => isThicknessVisible;
            set
            {
                isThicknessVisible = value;
                OnPropertyChanged(nameof(IsThicknessVisible));
            }
        }

        private bool isFontFamilyVisible;
        public bool IsFontFamilyVisible
        {
            get => isFontFamilyVisible;
            set
            {
                isFontFamilyVisible = value;
                OnPropertyChanged(nameof(IsFontFamilyVisible));
            }
        }

        private bool isFontSizeVisible;
        public bool IsFontSizeVisible
        {
            get => isFontSizeVisible;
            set
            {
                isFontSizeVisible = value;
                OnPropertyChanged(nameof(IsFontSizeVisible));
            }
        }

        private bool isTransparencyVisible;
        public bool IsTransparencyVisible
        {
            get => isTransparencyVisible;
            set
            {
                isTransparencyVisible = value;
                OnPropertyChanged(nameof(IsTransparencyVisible));
            }
        }
        private int selectedThickness;
        public int SelectedThickness
        {
            get => selectedThickness;
            set
            {
                if (selectedThickness != value)
                {
                    selectedThickness = value;
                    OnPropertyChanged(nameof(SelectedThickness));
                    inputHandler.ChangeArrowThickness(value);
                }
            }
        }

        private ColorInfo selectedColor;
        public Color SelectedColor
        {
            get => selectedColor?.ColorBrush.Color ?? System.Windows.Media.Colors.Black; // Zwróć Color z ColorInfo
            set
            {
                // Tworzenie nowego ColorInfo z podanego Color
                var newColorInfo = new ColorInfo { ColorBrush = new SolidColorBrush(value) };

                if (selectedColor?.ColorBrush.Color != value) // Porównanie Color, nie ColorInfo
                {
                    selectedColor = newColorInfo;
                    OnPropertyChanged(nameof(SelectedColor));
                    inputHandler.ChangeColor(value); // Zachowaj logikę zmiany koloru
                }
            }
        }

        private string selectedFontFamily;
        public string SelectedFontFamily
        {
            get => selectedFontFamily;
            set
            {
                if (selectedFontFamily != value)
                {
                    selectedFontFamily = value;
                    OnPropertyChanged(nameof(SelectedFontFamily));
                    inputHandler.ChangeFontFamily(new FontFamily(value));
                }
            }
        }

        private int selectedFontSize;
        public int SelectedFontSize
        {
            get => selectedFontSize;
            set
            {
                if (selectedFontSize != value)
                {
                    selectedFontSize = value;
                    OnPropertyChanged(nameof(SelectedFontSize));
                    inputHandler.ChangeFontSize(value);
                }
            }
        }

        private int selectedTransparency;
        public int SelectedTransparency
        {
            get => selectedTransparency;
            set
            {
                if (selectedTransparency != value)
                {
                    selectedTransparency = value;
                    OnPropertyChanged(nameof(SelectedTransparency));
                    inputHandler.ChangeTransparency(value);
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExecuteUndo(object parameter)
        {
            drawableCanvas.Undo();
        }

        private void ExecuteRedo(object parameter)
        {
            drawableCanvas.Redo();
        }

        private void ExecuteDelete(object parameter)
        {
            if (inputHandler != null)
            {
                inputHandler.CommandBinding_DeleteExecuted();
            }
        }

        private void ExecuteCopy(object parameter)
        {
            if (drawableCanvas.selectedElement != null)
            {
                // Zakładając, że masz metodę Clone() w klasach elementów rysowalnych
                copiedElement = drawableCanvas.selectedElement.Clone();
            }
            if (inputHandler != null)
            {
                inputHandler.CommandBinding_CopyExecuted();
            }
        }

        private void ExecutePaste(object parameter)
        {
            if (copiedElement != null)
            {
                var elementToPaste = copiedElement.Clone();
                drawableCanvas.AddElement(elementToPaste);
                drawableCanvas.InvalidateVisual();
            }
            if (copiedElement == null && inputHandler != null)
            {
                inputHandler.CommandBinding_PasteExecuted();
            }
        }

        private void ExecuteResize(object obj)
        {
            var resizeDialog = new ResizeDialog((int)drawableCanvas.ActualWidth, (int)drawableCanvas.ActualHeight);
            if (resizeDialog.ShowDialog() == true)
            {
                int newWidth = resizeDialog.NewWidth;
                int newHeight = resizeDialog.NewHeight;

                drawableCanvas.Height = newHeight;
                drawableCanvas.Width = newWidth;
            }
            drawableCanvas.InvalidateVisual();
        }

        private void ExecuteAddNiceBackground(object obj)
        {
            // 1. Dodaj tło na cały canvas
            var backgroundSize = new Size(drawableCanvas.ActualWidth, drawableCanvas.ActualHeight);
            var background = new DrawableBackground
            {
                Position = new Point(0, 0),
                Size = backgroundSize
            };
            drawableCanvas.AddElementAtBottom(background); // Dodaje tło na samym dole

            // 2. Dodaj screenshot na wierzch tła
            var screenshotSize = new Size(initialImage.PixelWidth * 0.7, initialImage.PixelHeight * 0.7);
            var screenshotPosition = new Point(
                (backgroundSize.Width - screenshotSize.Width) / 2, // Wyśrodkuj w poziomie
                (backgroundSize.Height - screenshotSize.Height) / 2 // Wyśrodkuj w pionie
            );

            // 3. Przeskaluj i wyśrodkuj istniejące elementy (oprócz tła)

            var screenshot = new DrawableScreenshot(initialImage, screenshotPosition, screenshotSize);
            drawableCanvas.AddElement(screenshot); // Dodaj screenshot na wierzch tła4

            ScaleAndRepositionElements(0.7, screenshotPosition); // Skalowanie do 70% i wyśrodkowanie

        }

        private void ScaleAndRepositionElements(double scale, Point screenshotPosition)
        {
            List<DrawableElement> scaledElements = new List<DrawableElement>();

            foreach (var element in drawableCanvas.elementManager.Elements)
            {
                if (element is DrawableScreenshot
                    || element is DrawableBackground)
                {
                    continue;
                }

                if (element is DrawableRectangle rectangle)
                {
                    rectangle.Size = new Size(rectangle.Size.Width * scale, rectangle.Size.Height * scale);
                    double offsetX = screenshotPosition.X + rectangle.Position.X * scale;
                    double offsetY = screenshotPosition.Y + rectangle.Position.Y* scale;

                    rectangle.Position = new Point(offsetX, offsetY);
                }
                if (element is DrawableBlur blur)
                {   
                    blur.Size = new Size(blur.Size.Width * scale, blur.Size.Height * scale);
                    double offsetX = screenshotPosition.X + blur.Position.X * scale;
                    double offsetY = screenshotPosition.Y + blur.Position.Y * scale;

                    blur.Position = new Point(offsetX, offsetY);
                }
                if (element is DrawableSpeechBubble bubble)
                {
                    bubble.Size = new Size(bubble.Size.Width * scale, bubble.Size.Height * scale);
                    double offsetX = screenshotPosition.X + bubble.Position.X * scale;
                    double offsetY = screenshotPosition.Y + bubble.Position.Y * scale;
                    bubble.Position = new Point(offsetX, offsetY);

                    double offsetXtail = screenshotPosition.X + bubble.EndTailPoint.X * scale;
                    double offsetYtail = screenshotPosition.Y + bubble.EndTailPoint.Y * scale;
                    bubble.EndTailPoint = new Point(offsetXtail, offsetYtail);
                }
                if (element is DrawableArrow arrow)
                {
                    arrow.Size = new Size(arrow.Size.Width * scale, arrow.Size.Height * scale);
                    double offsetX = screenshotPosition.X + arrow.Position.X * scale;
                    double offsetY = screenshotPosition.Y + arrow.Position.Y * scale;
                    arrow.Position = new Point(offsetX, offsetY);

                    double offsetXend = screenshotPosition.X + arrow.EndPoint.X * scale;
                    double offsetYend = screenshotPosition.Y + arrow.EndPoint.Y * scale;
                    arrow.EndPoint = new Point(offsetXend, offsetYend);
                }
                if (element is DrawableBrush brush)
                {
                    for (int i = 0; i < brush.points.Count; i++)
                    {
                        Point originalPoint = brush.points[i];
                        double scaledX = screenshotPosition.X + originalPoint.X * scale;
                        double scaledY = screenshotPosition.Y + originalPoint.Y * scale;
                        brush.points[i] = new Point(scaledX, scaledY);
                    }
                    brush.needsRedraw = true; 
                }
                if (element is DrawableText text)
                {
                    double offsetX = screenshotPosition.X + text.Position.X * scale;
                    double offsetY = screenshotPosition.Y + text.Position.Y * scale;
                    text.Position = new Point(offsetX, offsetY);
                }
                else
                {
                    var newSize = new Size(element.Size.Width * scale, element.Size.Height * scale);

                    double offsetX = screenshotPosition.X + element.Position.X * scale;
                    double offsetY = screenshotPosition.Y + element.Position.Y * scale; 

                    element.Size = newSize;
                    element.Position = new Point(offsetX, offsetY);

                }
                scaledElements.Add(element);
            }

            // Przesuń każdy przeskalowany element na pierwszy plan
            foreach (var scaledElement in scaledElements)
            {
                drawableCanvas.elementManager.BringToFront(scaledElement);
            }
            drawableCanvas.InvalidateVisual(); // Odśwież widok, aby zobaczyć zmiany
        }
    }
}
