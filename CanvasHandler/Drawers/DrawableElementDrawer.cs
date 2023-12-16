using screenerWpf;
using System.Windows.Input;

public abstract class DrawableElementDrawer
{
    protected DrawableCanvas DrawableCanvas { get; private set; }

    protected DrawableElementDrawer(DrawableCanvas canvas)
    {
        DrawableCanvas = canvas;
    }

    public abstract void StartDrawing(MouseButtonEventArgs e);
    public abstract void UpdateDrawing(MouseEventArgs e);
    public abstract void FinishDrawing();
}
