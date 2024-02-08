using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using screenerWpf.Commands;
using screenerWpf.Controls;
using screenerWpf.Interfaces;
using screenerWpf.Models;
using screenerWpf.Views;

namespace screenerWpf.ViewModels
{
    internal class ImageEditorViewModel : INotifyPropertyChanged
    {
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
        public ObservableCollection<ColorInfo> Colors { get; private set; }
        public ObservableCollection<int> Thicknesses { get; private set; }
        public ObservableCollection<string> FontFamilies { get; private set; }
        public ObservableCollection<int> FontSizes { get; private set; }
        public ObservableCollection<int> TransparencySizes { get; private set; }
        public event Action MinimizeRequest;
        public event Action MaximizeRestoreRequest;
        public event Action CloseRequest;
        private readonly DrawableCanvas drawableCanvas;

        public ImageEditorViewModel(ICanvasInputHandler inputHandler, DrawableCanvas drawableCanvas)
        {
            this.inputHandler = inputHandler ?? throw new ArgumentNullException(nameof(inputHandler));

            InitializeCommands();
            InitializeColors();
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
            SelectedColor = Colors.FirstOrDefault(c => c.ColorBrush.Color == System.Windows.Media.Colors.Black);
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

        private void InitializeColors()
        {
            Colors = new ObservableCollection<ColorInfo>
            {
                new ColorInfo { Name = "Czarny", ColorBrush = new SolidColorBrush(System.Windows.Media.Colors.Black) },
                new ColorInfo { Name = "Czerwony", ColorBrush = new SolidColorBrush(System.Windows.Media.Colors.Red) },
                new ColorInfo { Name = "Zielony", ColorBrush = new SolidColorBrush(System.Windows.Media.Colors.Green) },
                new ColorInfo { Name = "Niebieski", ColorBrush = new SolidColorBrush(System.Windows.Media.Colors.Blue) },
            };
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
        public ColorInfo SelectedColor
        {
            get => selectedColor;
            set
            {
                if (selectedColor != value)
                {
                    selectedColor = value;
                    OnPropertyChanged(nameof(SelectedColor));
                    inputHandler.ChangeColor(value?.ColorBrush.Color ?? System.Windows.Media.Colors.Transparent);
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

    }
}
