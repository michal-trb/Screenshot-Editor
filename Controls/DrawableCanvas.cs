namespace screenerWpf.Controls;

using global::Helpers.DpiHelper;
using screenerWpf.Helpers;
using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;
using screenerWpf.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// <summary>
/// Represents a drawable canvas that allows users to interactively add, remove, and manipulate drawable elements.
/// </summary>
public class DrawableCanvas : Canvas
{
    public ElementManager elementManager = new ElementManager();
    private HistoryManager historyManager = new HistoryManager();
    public DrawableElement selectedElement;
    public bool isFirstClick = true;
    public RenderTargetBitmap originalTargetBitmap;
    private ScaleTransform scaleTransform = new ScaleTransform();
    private TranslateTransform moveTransform = new TranslateTransform();
    public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent(
       "MouseDoubleClick",
       RoutingStrategy.Bubble,
       typeof(MouseButtonEventHandler),
       typeof(DrawableCanvas));

    /// <summary>
    /// Occurs when the mouse is double-clicked on the canvas.
    /// </summary>
    public event MouseButtonEventHandler MouseDoubleClick
    {
        add { AddHandler(MouseDoubleClickEvent, value); }
        remove { RemoveHandler(MouseDoubleClickEvent, value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableCanvas"/> class.
    /// Sets up context menus, transformations, and event handlers for the canvas.
    /// </summary>
    public DrawableCanvas()
    {
        ContextMenu contextMenu = new ContextMenu();
        MenuItem copyMenuItem = new MenuItem { Header = "Copy" };
        copyMenuItem.Click += CopyMenuItem_Click;
        contextMenu.Items.Add(copyMenuItem);
        this.ContextMenu = contextMenu;
        this.MouseWheel += DrawableCanvas_MouseWheel;

        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(scaleTransform);
        transformGroup.Children.Add(moveTransform);

        this.MouseWheel += DrawableCanvas_MouseWheel;
        this.RenderTransform = scaleTransform;
        this.ClipToBounds = true;

        this.Loaded += (sender, e) => UpdateClip();
        this.SizeChanged += (sender, e) => UpdateClip();
    }

    /// <summary>
    /// Updates the clipping region of the canvas to match its current size.
    /// </summary>
    private void UpdateClip()
    {
        this.Clip = new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
    }

    /// <summary>
    /// Overrides the mouse left button down event to detect double-clicks and raise the corresponding event.
    /// </summary>
    /// <param name="e">Mouse button event arguments.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (e.ClickCount == 2)
        {
            var newEventArgs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
            newEventArgs.RoutedEvent = MouseDoubleClickEvent;
            RaiseEvent(newEventArgs);
        }
    }

    /// <summary>
    /// Handles the mouse wheel event to zoom in and out of the canvas content.
    /// </summary>
    /// <param name="e">Mouse wheel event arguments.</param>
    private void DrawableCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            const double scaleFactor = 1.1;
            const double minScale = 0.2;

            double zoom = e.Delta > 0 ? scaleFactor : 1 / scaleFactor;
            double newScaleX = scaleTransform.ScaleX * zoom;
            double newScaleY = scaleTransform.ScaleY * zoom;

            if (newScaleX >= minScale && newScaleY >= minScale)
            {
                var mousePosition = e.GetPosition(this);

                if (newScaleX < 1 && newScaleY < 1)
                {
                    CenterContent();
                }
                else
                {
                    scaleTransform.CenterX = mousePosition.X;
                    scaleTransform.CenterY = mousePosition.Y;
                }

                scaleTransform.ScaleX = newScaleX;
                scaleTransform.ScaleY = newScaleY;
            }

            e.Handled = true;
        }
    }

    /// <summary>
    /// Centers the content of the canvas when zooming out beyond a certain scale.
    /// </summary>
    private void CenterContent()
    {
        double offsetX = (this.ActualWidth - (this.ActualWidth * scaleTransform.ScaleX)) / 2;
        double offsetY = (this.ActualHeight - (this.ActualHeight * scaleTransform.ScaleY)) / 2;

        moveTransform.X = offsetX;
        moveTransform.Y = offsetY;
    }

    /// <summary>
    /// Handles the click event for the "Copy" menu item, copying the canvas content to the clipboard.
    /// </summary>
    private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
    {
        CopyCanvasToClipboard();
    }

    /// <summary>
    /// Copies the current content of the canvas to the system clipboard.
    /// </summary>
    private void CopyCanvasToClipboard()
    {
        RenderTargetBitmap bitmap = GetRenderTargetBitmap();
        Clipboard.SetImage(bitmap);
    }

    /// <summary>
    /// Gets or sets the background image of the canvas.
    /// </summary>
    public ImageSource BackgroundImage
    {
        get { return (ImageSource)GetValue(BackgroundImageProperty); }
        set { SetValue(BackgroundImageProperty, value); }
    }

    /// <summary>
    /// Dependency property for the background image of the canvas.
    /// </summary>
    public static readonly DependencyProperty BackgroundImageProperty =
        DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(DrawableCanvas), new PropertyMetadata(null, OnBackgroundImageChanged));

    /// <summary>
    /// Called when the background image changes to invalidate the visual representation of the canvas.
    /// </summary>
    private static void OnBackgroundImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DrawableCanvas canvas)
        {
            canvas.InvalidateVisual();
        }
    }

    /// <summary>
    /// Renders the background image and all drawable elements on the canvas.
    /// </summary>
    /// <param name="dc">The drawing context for rendering content.</param>
    protected override void OnRender(DrawingContext dc)
    {
        if (BackgroundImage != null)
        {
            Rect rect = new Rect(0, 0, ActualWidth, ActualHeight);
            dc.DrawImage(BackgroundImage, rect);
        }

        base.OnRender(dc);
        foreach (var element in elementManager.Elements)
        {
            element.Draw(dc);
        }
    }

    /// <summary>
    /// Selects the specified element and brings it to the front of the canvas.
    /// </summary>
    /// <param name="element">The drawable element to be selected.</param>
    private void SelectElement(DrawableElement element)
    {
        if (selectedElement != null)
        {
            selectedElement.IsSelected = false;
        }

        selectedElement = element;

        if (selectedElement != null)
        {
            selectedElement.IsSelected = true;
            elementManager.BringToFront(selectedElement);
        }

        InvalidateVisual();
    }

    /// <summary>
    /// Selects an element at the given point on the canvas.
    /// </summary>
    /// <param name="point">The point at which to select an element.</param>
    public void SelectElementAtPoint(Point point)
    {
        var element = elementManager.GetElementAtPoint(point);

        if (element != null)
        {
            SelectElement(element);
        }
        else
        {
            DeselectCurrentElement();
        }
        Focus();
    }

    /// <summary>
    /// Deselects the currently selected element, if any.
    /// </summary>
    private void DeselectCurrentElement()
    {
        if (selectedElement != null)
        {
            selectedElement.IsSelected = false;
            selectedElement = null;
            InvalidateVisual();
        }
    }

    /// <summary>
    /// Generates a bitmap representing the current content of the canvas.
    /// </summary>
    /// <returns>A <see cref="RenderTargetBitmap"/> representing the canvas content.</returns>
    internal RenderTargetBitmap GetRenderTargetBitmap()
    {
        int width = (int)Math.Ceiling(ActualWidth);
        int height = (int)Math.Ceiling(ActualHeight);

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
            VisualBrush brush = new VisualBrush(this);
            context.DrawRectangle(
                brush,
                null, 
                new Rect(new Point(), new Size(width, height)));
        }

        renderBitmap.Render(visual);

        return renderBitmap;
    }

    /// <summary>
    /// Returns the original bitmap of the canvas before any modifications were made.
    /// </summary>
    /// <returns>A <see cref="RenderTargetBitmap"/> representing the original content.</returns>
    internal RenderTargetBitmap GetOriginalTargetBitmap()
    {
        return originalTargetBitmap;
    }

    /// <summary>
    /// Adds a new drawable element to the canvas.
    /// </summary>
    /// <param name="element">The element to be added.</param>
    public void AddElement(DrawableElement element)
    {
        elementManager.AddElement(element);
        historyManager.AddAction(new AddElementAction(this, element));
        InvalidateVisual();
    }

    /// <summary>
    /// Removes the specified drawable element from the canvas.
    /// </summary>
    /// <param name="element">The element to be removed.</param>
    public void RemoveElement(IDrawable element)
    {
        if (element != null)
        {
            elementManager.RemoveElement((DrawableElement)element);
            historyManager.AddAction(new RemoveElementAction(this, (DrawableElement)element));
            InvalidateVisual();
        }
    }

    /// <summary>
    /// Undoes the last action performed on the canvas.
    /// </summary>
    public void Undo()
    {
        if (historyManager.CanUndo)
        {
            historyManager.Undo();
            InvalidateVisual();
        }
    }

    /// <summary>
    /// Redoes the last undone action on the canvas.
    /// </summary>
    public void Redo()
    {
        if (historyManager.CanRedo)
        {
            historyManager.Redo();
            InvalidateVisual();
        }
    }

    /// <summary>
    /// Adds a drawable element at the bottom layer of the canvas.
    /// </summary>
    /// <param name="element">The element to be added at the bottom.</param>
    public void AddElementAtBottom(DrawableElement element)
    {
        elementManager.Elements.Insert(0, element);
        InvalidateVisual();
    }
}