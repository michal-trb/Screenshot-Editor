namespace screenerWpf.ViewModels;

using screenerWpf.Commands;
using screenerWpf.Controls;
using screenerWpf.Interfaces;
using screenerWpf.Models;
using screenerWpf.Models.DrawableElement;
using screenerWpf.Models.DrawableElements;
using screenerWpf.Properties;
using screenerWpf.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// <summary>
/// ViewModel class for the Image Editor. Manages commands, bindings, and properties for the image editor UI.
/// </summary>
internal class ImageEditorViewModel : INotifyPropertyChanged
{
    private BitmapSource initialImage;
    private IDrawable copiedElement;
    private readonly ICanvasInputHandler inputHandler;
    private readonly DrawableCanvas drawableCanvas;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action MinimizeRequest;
    public event Action MaximizeRestoreRequest;
    public event Action CloseRequest;

    public ICommand MinimizeCommand { get; private set; }
    public ICommand MaximizeRestoreCommand { get; private set; }
    public ICommand CloseCommand { get; private set; }
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

    /// <summary>
    /// Constructor for the ImageEditorViewModel class.
    /// Initializes commands, input handler, canvas, and default property values.
    /// </summary>
    /// <param name="inputHandler">Handler for canvas input.</param>
    /// <param name="drawableCanvas">The canvas where drawing elements are displayed.</param>
    /// <param name="initialBitmap">The initial bitmap to be edited.</param>
    public ImageEditorViewModel(ICanvasInputHandler inputHandler, DrawableCanvas drawableCanvas, BitmapSource initialBitmap)
    {
        this.inputHandler = inputHandler ?? throw new ArgumentNullException(nameof(inputHandler));
        this.drawableCanvas = drawableCanvas;
        this.initialImage = initialBitmap;

        InitializeCommands();
        InitializeThicknesses();
        InitializeFontFamilies();
        InitializeFontSizes();
        InitializeTransparencySizes();
        InitializeStartValues();

        MinimizeCommand = new RelayCommand(_ => OnMinimize());
        MaximizeRestoreCommand = new RelayCommand(_ => OnMaximizeRestore());
        CloseCommand = new RelayCommand(_ => OnClose());

        this.drawableCanvas = drawableCanvas;
    }

    /// <summary>
    /// Initializes default property values for the editor.
    /// </summary>
    private void InitializeStartValues()
    {
        SelectedFontFamily = Settings.Default.DefaultFontFamily;
        SelectedFontSize = Settings.Default.DefaultFontSize;
        SelectedThickness = Settings.Default.DefaultThickness;
        SelectedTransparency = Settings.Default.DefaultTransparency;

        // Konwertuj kolor z formatu string na Color
        var colorConverter = new BrushConverter();
        var defaultColor = (Color)ColorConverter.ConvertFromString(Settings.Default.DefaultColor);
        SelectedColor = defaultColor;
    }

    /// <summary>
    /// Initializes the collection of transparency values available for selection.
    /// </summary>
    private void InitializeTransparencySizes()
    {
        TransparencySizes = new ObservableCollection<int> { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
    }

    /// <summary>
    /// Initializes the collection of font families available for text elements.
    /// </summary>
    private void InitializeFontFamilies()
    {
        FontFamilies = new ObservableCollection<string>
            {
                "Arial", "Calibri", "Times New Roman", "Verdana", "Courier New"
            };
    }

    /// <summary>
    /// Initializes the collection of font sizes available for text elements.
    /// </summary>
    private void InitializeFontSizes()
    {
        FontSizes = new ObservableCollection<int> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40 };
    }

    /// <summary>
    /// Initializes the collection of thickness values available for drawing tools.
    /// </summary>
    private void InitializeThicknesses()
    {
        Thicknesses = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    }

    /// <summary>
    /// Initializes the various commands for interaction with the UI.
    /// </summary>
    private void InitializeCommands()
    {
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

    /// <summary>
    /// Triggers the minimize request event.
    /// </summary>
    private void OnMinimize()
    {
        MinimizeRequest?.Invoke();
    }

    /// <summary>
    /// Triggers the maximize or restore window request event.
    /// </summary>
    private void OnMaximizeRestore()
    {
        MaximizeRestoreRequest?.Invoke();
    }

    /// <summary>
    /// Triggers the close request event.
    /// </summary>
    private void OnClose()
    {
        CloseRequest?.Invoke();
    }

    /// <summary>
    /// Executes the Draw Arrow command.
    /// </summary>
    private void ExecuteDrawArrow(object obj)
    {
        UpdateVisibility(true, true, false, false, false);
        inputHandler.DrawArrow();
    }

    /// <summary>
    /// Executes the Add Text command.
    /// </summary>
    private void ExecuteAddText(object obj)
    {
        UpdateVisibility(true, false, true, true, false);
        inputHandler.AddText();
    }

    /// <summary>
    /// Executes the Draw Rectangle command.
    /// </summary>
    private void ExecuteDrawRect(object obj)
    {
        UpdateVisibility(true, true, false, false, true);
        inputHandler.DrawRect();
    }
    /// <summary>
    /// Executes the Speech Bubble command to add a speech bubble to the canvas.
    /// </summary>
    private void ExecuteSpeechBubble(object obj)
    {
        UpdateVisibility(true, true, true, true, false);
        inputHandler.SpeechBubble();
    }

    /// <summary>
    /// Executes the Blur command to add a blur effect to the selected area on the canvas.
    /// </summary>
    private void ExecuteBlur(object obj)
    {
        UpdateVisibility(false, false, false, false, false);
        inputHandler.Blur();
    }

    /// <summary>
    /// Executes the Brush command to allow freehand drawing on the canvas.
    /// </summary>
    private void ExecuteBrush(object obj)
    {
        UpdateVisibility(true, true, false, false, true);
        inputHandler.Brush();
    }

    /// <summary>
    /// Executes the Recognize Text command to perform OCR (Optical Character Recognition) on the canvas.
    /// </summary>
    private void ExecuteRecognizeText(object obj)
    {
        UpdateVisibility(false, true, false, false, false);
        inputHandler.RecognizeText();
    }

    /// <summary>
    /// Executes the Save command to save the current state of the canvas as an image.
    /// </summary>
    private void ExecuteSave(object obj)
    {
        inputHandler.Save();
    }

    /// <summary>
    /// Executes a quick save of the current canvas state and returns the saved file path.
    /// </summary>
    private string ExecuteSaveFast()
    {
        return inputHandler.SaveFast();
    }

    /// <summary>
    /// Updates the visibility of certain UI elements based on the action being performed.
    /// </summary>
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

    /// <summary>
    /// Indicates whether the color options are visible in the UI.
    /// </summary>
    private bool isColorVisible;
    public bool IsColorVisible
    {
        /// <summary>
        /// Gets or sets a value indicating whether the color options are visible in the UI.
        /// </summary>
        get => isColorVisible;
        set
        {
            isColorVisible = value;
            OnPropertyChanged(nameof(IsColorVisible));
        }
    }

    /// <summary>
    /// Indicates whether the thickness options are visible in the UI.
    /// </summary>
    private bool isThicknessVisible;
    public bool IsThicknessVisible
    {
        /// <summary>
        /// Gets or sets a value indicating whether the thickness options are visible in the UI.
        /// </summary>
        get => isThicknessVisible;
        set
        {
            isThicknessVisible = value;
            OnPropertyChanged(nameof(IsThicknessVisible));
        }
    }

    /// <summary>
    /// Indicates whether the font family options are visible in the UI.
    /// </summary>
    private bool isFontFamilyVisible;
    public bool IsFontFamilyVisible
    {
        /// <summary>
        /// Gets or sets a value indicating whether the font family options are visible in the UI.
        /// </summary>
        get => isFontFamilyVisible;
        set
        {
            isFontFamilyVisible = value;
            OnPropertyChanged(nameof(IsFontFamilyVisible));
        }
    }

    /// <summary>
    /// Indicates whether the font size options are visible in the UI.
    /// </summary>
    private bool isFontSizeVisible;
    public bool IsFontSizeVisible
    {
        /// <summary>
        /// Gets or sets a value indicating whether the font size options are visible in the UI.
        /// </summary>
        get => isFontSizeVisible;
        set
        {
            isFontSizeVisible = value;
            OnPropertyChanged(nameof(IsFontSizeVisible));
        }
    }

    /// <summary>
    /// Indicates whether the transparency options are visible in the UI.
    /// </summary>
    private bool isTransparencyVisible;
    public bool IsTransparencyVisible
    {
        /// <summary>
        /// Gets or sets a value indicating whether the transparency options are visible in the UI.
        /// </summary>
        get => isTransparencyVisible;
        set
        {
            isTransparencyVisible = value;
            OnPropertyChanged(nameof(IsTransparencyVisible));
        }
    }

    /// <summary>
    /// Stores the selected thickness value for drawing elements.
    /// </summary>
    private int selectedThickness;
    public int SelectedThickness
    {
        /// <summary>
        /// Gets or sets the thickness value for drawing elements.
        /// </summary>
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

    /// <summary>
    /// Stores the selected color for drawing elements.
    /// </summary>
    private ColorInfo selectedColor;
    public Color SelectedColor
    {
        /// <summary>
        /// Gets or sets the color used for drawing elements.
        /// </summary>
        get => selectedColor?.ColorBrush.Color ?? System.Windows.Media.Colors.Black;
        set
        {
            var newColorInfo = new ColorInfo { ColorBrush = new SolidColorBrush(value) };

            if (selectedColor?.ColorBrush.Color != value)
            {
                selectedColor = newColorInfo;
                OnPropertyChanged(nameof(SelectedColor));
                inputHandler.ChangeColor(value);
            }
        }
    }

    /// <summary>
    /// Stores the selected font family for text elements.
    /// </summary>
    private string selectedFontFamily;
    public string SelectedFontFamily
    {
        /// <summary>
        /// Gets or sets the font family used for text elements.
        /// </summary>
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

    /// <summary>
    /// Stores the selected font size for text elements.
    /// </summary>
    private int selectedFontSize;
    public int SelectedFontSize
    {
        /// <summary>
        /// Gets or sets the font size used for text elements.
        /// </summary>
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

    /// <summary>
    /// Stores the selected transparency level for drawing elements.
    /// </summary>
    private int selectedTransparency;
    public int SelectedTransparency
    {
        /// <summary>
        /// Gets or sets the transparency level used for drawing elements.
        /// </summary>
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

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Executes the Undo command to revert the last action on the canvas.
    /// </summary>
    private void ExecuteUndo(object parameter)
    {
        drawableCanvas.Undo();
    }

    /// <summary>
    /// Executes the Redo command to reapply the last undone action on the canvas.
    /// </summary>
    private void ExecuteRedo(object parameter)
    {
        drawableCanvas.Redo();
    }

    /// <summary>
    /// Executes the Delete command to remove the selected element from the canvas.
    /// </summary>
    private void ExecuteDelete(object parameter)
    {
        if (inputHandler != null)
        {
            inputHandler.CommandBinding_DeleteExecuted();
        }
    }

    /// <summary>
    /// Executes the Copy command to copy the currently selected element on the canvas.
    /// </summary>
    private void ExecuteCopy(object parameter)
    {
        if (drawableCanvas.selectedElement != null)
        {
            copiedElement = drawableCanvas.selectedElement.Clone();
        }
        if (inputHandler != null)
        {
            inputHandler.CommandBinding_CopyExecuted();
        }
    }

    /// <summary>
    /// Executes the Paste command to paste the previously copied element onto the canvas.
    /// </summary>
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

    /// <summary>
    /// Executes the Resize command to change the canvas size.
    /// </summary>
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

    /// <summary>
    /// Executes the Add Nice Background command to add a background and reposition elements.
    /// </summary>
    private void ExecuteAddNiceBackground(object obj)
    {
        // 1. Add a background to the entire canvas
        var backgroundSize = new Size(drawableCanvas.ActualWidth, drawableCanvas.ActualHeight);
        var background = new DrawableBackground
        {
            Position = new Point(0, 0),
            Size = backgroundSize
        };
        drawableCanvas.AddElementAtBottom(background);

        // Add screenshot on top of the background
        var screenshotSize = new Size(initialImage.PixelWidth * 0.7, initialImage.PixelHeight * 0.7);
        var screenshotPosition = new Point(
            (backgroundSize.Width - screenshotSize.Width) / 2, // Center horizontally
            (backgroundSize.Height - screenshotSize.Height) / 2 // Center vertically
        );

        var screenshot = new DrawableScreenshot(initialImage, screenshotPosition, screenshotSize);
        drawableCanvas.AddElement(screenshot);

        ScaleAndRepositionElements(0.7, screenshotPosition);
    }

    /// <summary>
    /// Scales and repositions existing elements on the canvas.
    /// </summary>
    /// <param name="scale">The scale factor to resize the elements.</param>
    /// <param name="screenshotPosition">The position at which the elements should be centered.</param>
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
                double offsetY = screenshotPosition.Y + rectangle.Position.Y * scale;

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

        foreach (var scaledElement in scaledElements)
        {
            drawableCanvas.elementManager.BringToFront(scaledElement);
        }
        drawableCanvas.InvalidateVisual();
    }

    public void ResetToSelectionMode()
    {
        UpdateVisibility(false, false, false, false, false);
        inputHandler.SetCurrentAction(EditAction.Select);
    }
}
