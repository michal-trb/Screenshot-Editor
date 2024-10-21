namespace screenerWpf.Models.DrawableElement;

using global::Helpers.DpiHelper;
using screenerWpf.Controls;
using screenerWpf.Models.DrawableElements;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

/// <summary>
/// Represents a drawable blur element that can be resized, moved, and applied with a blur effect.
/// </summary>
public class DrawableBlur : DrawableWithHandles
{
    /// <summary>
    /// Gets or sets the position of the blur element.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Gets or sets the size of the blur element.
    /// </summary>
    public Size Size { get; set; }

    /// <summary>
    /// Gets or sets the stroke color for the blur element.
    /// </summary>
    public Color StrokeColor { get; set; }

    /// <summary>
    /// Gets or sets the stroke thickness for the blur element.
    /// </summary>
    public double StrokeThickness { get; set; }

    /// <summary>
    /// Gets or sets the blur effect applied to the element.
    /// </summary>
    public Effect BlurEffect { get; set; }

    /// <summary>
    /// Gets or sets the visual representation of the element.
    /// </summary>
    public DrawingVisual Visual { get; set; }

    private DrawableCanvas Canvas { get; set; }
    private enum DragHandle { None, TopLeft, TopRight, BottomLeft, BottomRight }

    private DragHandle currentDragHandle = DragHandle.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableBlur"/> class with a specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas on which the blur element is drawn.</param>
    public DrawableBlur(DrawableCanvas canvas) : base(4)
    {
        Canvas = canvas;
        Position = new Point(0, 0);
        Size = new Size(100, 50);
        StrokeColor = Colors.Transparent;
        StrokeThickness = 1.0;
        BlurEffect = new BlurEffect { Radius = 10 };
        Visual = new DrawingVisual();
    }

    /// <summary>
    /// Draws the blur element and handles selection visual cues.
    /// </summary>
    /// <param name="context">The drawing context used for rendering the element.</param>
    public override void Draw(DrawingContext context)
    {
        UpdateVisual();

        if (Visual != null)
        {
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new TranslateTransform(Position.X, Position.Y));

            context.PushTransform(transformGroup);
            context.DrawDrawing(Visual.Drawing);
            context.Pop();
        }

        if (IsSelected)
        {
            UpdateHandlePoints();
            DrawSelectionHandles(context);
        }
    }

    /// <summary>
    /// Updates the positions of the drag handles for resizing the blur element.
    /// </summary>
    protected override void UpdateHandlePoints()
    {
        Rect rect = new Rect(Position, Size);
        HandlePoints[0] = rect.TopLeft; // Top-left corner
        HandlePoints[1] = rect.TopRight; // Top-right corner
        HandlePoints[2] = rect.BottomLeft; // Bottom-left corner
        HandlePoints[3] = rect.BottomRight; // Bottom-right corner
    }

    /// <summary>
    /// Determines if the specified point is within the blur element or near one of the resize handles.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the point is within the element or near a handle; otherwise, false.</returns>
    public override bool HitTest(Point point)
    {
        Rect rect = new Rect(Position, Size);
        currentDragHandle = DragHandle.None;

        if (IsNearCorner(point, Position)) currentDragHandle = DragHandle.TopLeft;
        else if (IsNearCorner(point, new Point(Position.X + Size.Width, Position.Y))) currentDragHandle = DragHandle.TopRight;
        else if (IsNearCorner(point, new Point(Position.X, Position.Y + Size.Height))) currentDragHandle = DragHandle.BottomLeft;
        else if (IsNearCorner(point, new Point(Position.X + Size.Width, Position.Y + Size.Height))) currentDragHandle = DragHandle.BottomRight;

        return rect.Contains(point) || currentDragHandle != DragHandle.None;
    }

    /// <summary>
    /// Determines if a point is near a given corner within a specified tolerance.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <param name="corner">The corner point.</param>
    /// <returns>True if the point is near the corner; otherwise, false.</returns>
    private bool IsNearCorner(Point point, Point corner)
    {
        double tolerance = 10; // Distance tolerance
        return Math.Abs(point.X - corner.X) <= tolerance && Math.Abs(point.Y - corner.Y) <= tolerance;
    }

    /// <summary>
    /// Gets the bounding rectangle for the blur element.
    /// </summary>
    /// <returns>A <see cref="Rect"/> that represents the bounds of the blur element.</returns>
    public override Rect GetBounds()
    {
        return new Rect(Position, Size);
    }

    /// <summary>
    /// Moves or resizes the blur element based on the provided vector.
    /// </summary>
    /// <param name="delta">The vector representing the movement or resize amount.</param>
    public override void Move(Vector delta)
    {
        // Define new variables for position and size
        double newX = Position.X;
        double newY = Position.Y;
        double newWidth = Size.Width;
        double newHeight = Size.Height;

        switch (currentDragHandle)
        {
            case DragHandle.None:
                newX += delta.X;
                newY += delta.Y;
                break;
            case DragHandle.TopLeft:
                newX += delta.X;
                newY += delta.Y;
                newWidth -= delta.X;
                newHeight -= delta.Y;
                break;
            case DragHandle.TopRight:
                newWidth += delta.X;
                newY += delta.Y;
                newHeight -= delta.Y;
                break;
            case DragHandle.BottomLeft:
                newX += delta.X;
                newWidth -= delta.X;
                newHeight += delta.Y;
                break;
            case DragHandle.BottomRight:
                newWidth += delta.X;
                newHeight += delta.Y;
                break;
        }

        // Ensure width and height are non-negative
        if (newWidth < 0)
        {
            newX += newWidth;
            newWidth = -newWidth;
            currentDragHandle = FlipHorizontalHandle(currentDragHandle);
        }
        if (newHeight < 0)
        {
            newY += newHeight;
            newHeight = -newHeight;
            currentDragHandle = FlipVerticalHandle(currentDragHandle);
        }

        // Set new values for position and size
        Position = new Point(newX, newY);
        Size = new Size(newWidth, newHeight);
    }

    private DragHandle FlipHorizontalHandle(DragHandle handle)
    {
        return handle switch
        {
            DragHandle.TopLeft => DragHandle.TopRight,
            DragHandle.TopRight => DragHandle.TopLeft,
            DragHandle.BottomLeft => DragHandle.BottomRight,
            DragHandle.BottomRight => DragHandle.BottomLeft,
            _ => handle,
        };
    }

    private DragHandle FlipVerticalHandle(DragHandle handle)
    {
        return handle switch
        {
            DragHandle.TopLeft => DragHandle.BottomLeft,
            DragHandle.BottomLeft => DragHandle.TopLeft,
            DragHandle.TopRight => DragHandle.BottomRight,
            DragHandle.BottomRight => DragHandle.TopRight,
            _ => handle,
        };
    }

    /// <summary>
    /// Updates the visual representation of the blur effect.
    /// </summary>
    public void UpdateVisual()
    {
        RenderTargetBitmap originalBitmap = Canvas.GetOriginalTargetBitmap();

        if (Size.Width <= 1 || Size.Height <= 1)
        {
            return;
        }

        Int32Rect cropRect = new Int32Rect(
            (int)Position.X,
            (int)Position.Y,
            (int)Size.Width,
            (int)Size.Height);

        CroppedBitmap croppedBitmap = new CroppedBitmap(originalBitmap, cropRect);

        Visual = new DrawingVisual();
        using (DrawingContext dc = Visual.RenderOpen())
        {
            BlurEffect blurEffect = new BlurEffect { Radius = 10 };

            DrawingVisual tempVisual = new DrawingVisual();
            using (DrawingContext tempDc = tempVisual.RenderOpen())
            {
                tempDc.DrawImage(croppedBitmap, new Rect(0, 0, cropRect.Width, cropRect.Height));
            }
            tempVisual.Effect = blurEffect;

            var currentDpi = DpiHelper.CurrentDpi;

            RenderTargetBitmap blurredBitmap = new RenderTargetBitmap(
                cropRect.Width,
                cropRect.Height,
                currentDpi.DpiX,
                currentDpi.DpiY,
                PixelFormats.Pbgra32);
            blurredBitmap.Render(tempVisual);

            dc.DrawImage(blurredBitmap, new Rect(0, 0, cropRect.Width, cropRect.Height));
        }
    }

    /// <summary>
    /// Creates a clone of the current blur element with adjusted position.
    /// </summary>
    /// <returns>A cloned instance of the <see cref="DrawableBlur"/> element.</returns>
    public override DrawableElement Clone()
    {
        return new DrawableBlur(Canvas)
        {
            StrokeColor = this.StrokeColor,
            StrokeThickness = this.StrokeThickness,
            BlurEffect = this.BlurEffect,
            Visual = this.Visual,
            Canvas = this.Canvas,
            Color = this.Color,
            Position = new Point(Position.X + 5, Position.Y + 5),
            Size = this.Size,
            Scale = this.Scale,
        };
    }
}
