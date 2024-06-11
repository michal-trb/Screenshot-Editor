using System.Windows.Media;
using System.Windows;
using System;

public abstract class DrawableWithHandles : DrawableElement
{
    protected enum DragHandle { None, TopLeft, TopRight, BottomLeft, BottomRight }
    protected DragHandle currentDragHandle = DragHandle.None;
    protected double handleSize = 6; // Rozmiar uchwytu
    protected Point[] HandlePoints { get; set; }

    protected DrawableWithHandles(int handleCount)
    {
        HandlePoints = new Point[handleCount];
    }
    // Metody wspólne dla wszystkich elementów z uchwytami
    protected void DrawSelectionHandles(DrawingContext context)
    {
        if (!IsSelected) return;

        Brush handleBrush = Brushes.Red; // Kolor uchwytu
        foreach (var point in HandlePoints)
        {
            DrawHandle(context, point, handleBrush, handleSize);
        }
    }

    private void DrawHandle(DrawingContext context, Point position, Brush brush, double size)
    {
        Vector offset = new Vector(-size / 2, -size / 2);
        position += offset;
        context.DrawRectangle(brush, null, new Rect(position, new Size(size, size)));
    }

    protected abstract void UpdateHandlePoints();

    protected bool IsNearCorner(Point point, Point corner)
    {
        double tolerance = 10; 
        return (Math.Abs(point.X - corner.X) <= tolerance && Math.Abs(point.Y - corner.Y) <= tolerance);
    }

    public override void Move(Vector delta)
    {
        base.Move(delta);
        UpdateHandlePoints();
    }
}
