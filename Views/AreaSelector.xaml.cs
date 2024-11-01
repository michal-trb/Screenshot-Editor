namespace screenerWpf;

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using Point = System.Windows.Point;
using Pen = System.Windows.Media.Pen;
using Brushes = System.Windows.Media.Brushes;

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

    private const int MAGNIFIER_SIZE = 120;  // Size of the magnifier window
    private const int MAGNIFIER_ZOOM = 2;    // Magnification level
    private const int CROSSHAIR_SIZE = 10;   // Size of the crosshair in magnifier

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

        // Add dimension information only if we are selecting an area
        if (SelectedRectangle.Width > 0 && SelectedRectangle.Height > 0)
        {
            // Prepare text with dimensions
            var dimensionsText = $"{(int)SelectedRectangle.Width} x {(int)SelectedRectangle.Height}";

            // Create formatted text
            var formattedText = new FormattedText(
                dimensionsText,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                14,
                Brushes.White,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            // Add background under text for better readability
            var textBackground = new Rect(
                currentPoint.X + 20,  // Offset from cursor
                currentPoint.Y + 20,
                formattedText.Width + 10,  // Add padding
                formattedText.Height + 6   // Add padding
            );

            drawingContext.DrawRectangle(
                new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 0, 0)),  // Semi-transparent black background
                null,
                textBackground
            );

            // Draw the text
            drawingContext.DrawText(
                formattedText,
                new Point(textBackground.X + 5, textBackground.Y + 3)  // Center the text in the background
            );
        }
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
            SelectedRectangle = new Rect(startPoint, new System.Windows.Size(0, 0));
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
