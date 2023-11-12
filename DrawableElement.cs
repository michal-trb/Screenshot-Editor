using screenerWpf;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System;

public abstract class DrawableElement : IDrawable
{
    public Point Position { get; set; }
    public Color Color { get; set; }
    public bool IsSelected { get; set; }

    public abstract void Draw(DrawingContext context);
    public abstract bool HitTest(Point point);
    public abstract Rect GetBounds();

    // Implement the Select method from IDrawable
    public virtual void Select()
    {
        IsSelected = true;
        // Add any other logic you need when the element is selected
    }

    public bool ContainsPoint(Point point)
    {
        throw new System.NotImplementedException();
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

    public virtual void MoveBy(Vector delta)
    {
        Position = new Point(Position.X + delta.X, Position.Y + delta.Y);
    }
}
