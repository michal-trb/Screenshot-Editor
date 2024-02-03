using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System;
using screenerWpf.Interfaces;

public abstract class DrawableElement : IDrawable
{
    public Point Position { get; set; }
    public Size Size { get; set; }
    public Color Color { get; set; }
    public bool IsSelected { get; set; }

    public abstract void Draw(DrawingContext context);
    public abstract Rect GetBounds();

    // Implement the Select method from IDrawable
    public virtual void Select()
    {
        IsSelected = true;
        // Add any other logic you need when the element is selected
    }

    public bool ContainsPoint(Point point)
    {
        Rect elementBounds = GetBounds();
        return elementBounds.Contains(point);
    }


    public void Draw(WriteableBitmap bitmap)
    {
        throw new System.NotImplementedException();
    }

    public void Remove()
    {
        throw new System.NotImplementedException();
    }

    public virtual bool Contains(Point point)
    { 
        throw new NotImplementedException();
    }

    public virtual void Move(Vector delta)
    {
        Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
    }

    public Point GetLocation()
    {
        throw new NotImplementedException();
    }

    public virtual bool HitTest(Point point)
    {
        Rect rect = new Rect(Position, Size);
        Geometry rectangleGeometry = new RectangleGeometry(rect);
        return rectangleGeometry.FillContains(point);
    }
}
