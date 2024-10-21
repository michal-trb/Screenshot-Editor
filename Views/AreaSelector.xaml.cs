namespace screenerWpf;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

/// <summary>
/// A window that allows the user to select a specific area on the screen by drawing a rectangle.
/// </summary>
public partial class AreaSelector : Window
{
    /// <summary>
    /// Gets the rectangle selected by the user.
    /// </summary>
    public Rect SelectedRectangle { get; private set; }

    private Point startPoint;
    private Point currentPoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="AreaSelector"/> class.
    /// Sets up a transparent overlay window for selecting a rectangular area on the screen.
    /// </summary>
    public AreaSelector()
    {
        InitializeComponent();

        this.Left = SystemParameters.VirtualScreenLeft;
        this.Top = SystemParameters.VirtualScreenTop;
        this.Width = SystemParameters.VirtualScreenWidth;
        this.Height = SystemParameters.VirtualScreenHeight;

        this.WindowStyle = WindowStyle.None;
        this.Topmost = true;
        this.Opacity = 1;
        this.Background = new SolidColorBrush(Colors.White) { Opacity = 0.1 };
        this.AllowsTransparency = true;
        this.ShowInTaskbar = false;

        this.MouseDown += Grid_MouseDown;
        this.MouseMove += Grid_MouseMove;
        this.MouseUp += Grid_MouseUp;
        this.KeyDown += Grid_KeyDown;
    }

    /// <summary>
    /// Overrides the default render method to draw the selected rectangle and dim the rest of the screen.
    /// </summary>
    /// <param name="drawingContext">The drawing instructions for a specific element.</param>
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        Rect fullRect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

        CombinedGeometry combinedGeometry = new CombinedGeometry
        {
            Geometry1 = new RectangleGeometry(fullRect),
            Geometry2 = new RectangleGeometry(SelectedRectangle),
            GeometryCombineMode = GeometryCombineMode.Exclude
        };

        drawingContext.DrawGeometry(new SolidColorBrush(Colors.White) { Opacity = 0.1 }, null, combinedGeometry);

        // Draw the red border of the selected rectangle.
        drawingContext.DrawRectangle(null, new Pen(Brushes.Red, 2), SelectedRectangle);
    }

    /// <summary>
    /// Handles the mouse button release event to finalize the selected area and close the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
            if (this.IsVisible && !this.DialogResult.HasValue)
            {
                this.DialogResult = true;
            }
            this.Close();
        }
    }

    /// <summary>
    /// Handles the mouse button down event to start selecting an area.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            startPoint = e.GetPosition(this);
            SelectedRectangle = new Rect(startPoint, new Size(0, 0));
            this.InvalidateVisual();
        }
    }

    /// <summary>
    /// Handles the mouse move event to update the selected area as the user drags the mouse.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void Grid_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            currentPoint = e.GetPosition(this); // Update the current position of the cursor.
            SelectedRectangle = new Rect(startPoint, currentPoint);
            this.InvalidateVisual();
        }
    }

    /// <summary>
    /// Handles the key down event to close the window if the Escape key is pressed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void Grid_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            if (this.IsVisible)
            {
                this.DialogResult = false;
            }
            this.Close();
        }
    }
}