using System.Windows.Media;
using System.Windows;
using System;

public abstract class DrawableWithHandles : DrawableElement
{
    protected enum DragHandle { None, TopLeft, TopRight, BottomLeft, BottomRight }
    protected DragHandle currentDragHandle = DragHandle.None;
    protected double handleSize = 6; // Rozmiar uchwytu
    protected Point[] HandlePoints { get; set; }

    // Konstruktor, który pozwala na ustawienie liczby uchwytów
    protected DrawableWithHandles(int handleCount)
    {
        HandlePoints = new Point[handleCount];
    }
    // Metody wspólne dla wszystkich elementów z uchwytami
    protected void DrawSelectionHandles(DrawingContext context)
    {
        Brush handleBrush = Brushes.Red; // Kolor uchwytu
        foreach (var point in HandlePoints)
        {
            DrawHandle(context, point);
        }
    }

    private void DrawHandle(DrawingContext context, Point position)
    {
        Vector offset = new Vector(-handleSize / 2, -handleSize / 2);
        position += offset;
        context.DrawRectangle(Brushes.Red, null, new Rect(position, new Size(handleSize, handleSize)));
    }

    protected bool IsNearCorner(Point point, Point corner)
    {
        double tolerance = 10; // Możesz dostosować tolerancję
        return (Math.Abs(point.X - corner.X) <= tolerance && Math.Abs(point.Y - corner.Y) <= tolerance);
    }

    // Pozostałe metody abstrakcyjne i właściwości...
}
