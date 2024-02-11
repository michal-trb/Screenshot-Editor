using screenerWpf.CanvasHandler.Drawers;
using screenerWpf.Interfaces;
using screenerWpf.Models.DrawableElements;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace screenerWpf.Controls
{
    public enum EditAction
    {
        None, DrawArrow, AddText, Move, Delete, AddBubble, DrawRectangle,
        DrawBlur, BrushPainting,
        RecognizeText
    }

    public class CanvasActionHandler : ICanvasActionHandler
    {
        private DrawableCanvas drawableCanvas;
        private IDrawable currentDrawable;
        private EditAction currentAction = EditAction.None;

        // Dodanie obiektów Drawer
        private ArrowDrawer arrowDrawer;
        private RectangleDrawer rectangleDrawer;
        private SpeechBubbleDrawer speechBubbleDrawer;
        private TextDrawer textDrawer;
        private BlurDrawer blurDrawer;
        private BrushDrawer brushDrawer;

        public CanvasActionHandler(DrawableCanvas canvas)
        {
            drawableCanvas = canvas;

            // Inicjalizacja obiektów Drawer
            arrowDrawer = new ArrowDrawer(currentDrawable, canvas);
            rectangleDrawer = new RectangleDrawer(canvas);
            speechBubbleDrawer = new SpeechBubbleDrawer(canvas);
            textDrawer = new TextDrawer(canvas);
            blurDrawer = new BlurDrawer(canvas);
            brushDrawer = new BrushDrawer(canvas);
        }

        public void HandleLeftButtonDown(MouseButtonEventArgs e)
        {
            switch (currentAction)
            {
                case EditAction.DrawArrow:
                    arrowDrawer.StartDrawing(e);
                    break;
                case EditAction.AddText:
                    textDrawer.StartDrawing(e);
                    break;
                case EditAction.AddBubble:
                    speechBubbleDrawer.StartDrawing(e);
                    break;
                case EditAction.DrawRectangle:
                    rectangleDrawer.StartDrawing(e);
                    break;
                case EditAction.DrawBlur:
                    blurDrawer.StartDrawing(e);
                    break;
                case EditAction.BrushPainting:
                    brushDrawer.StartDrawing(e);
                    break;
                default:
                    SelectElementAtMousePosition(e);
                    break;
            }
        }

        public void HandleLeftButtonUp(MouseButtonEventArgs e)
        {
            // Zakończenie rysowania dla bieżącej akcji
            switch (currentAction)
            {
                case EditAction.DrawArrow:
                    arrowDrawer.FinishDrawing();
                    break;
                case EditAction.AddText:
                    textDrawer.FinishDrawing();
                    break;
                case EditAction.AddBubble:
                    speechBubbleDrawer.FinishDrawing();
                    break;
                case EditAction.DrawRectangle:
                    rectangleDrawer.FinishDrawing();
                    break;
                case EditAction.DrawBlur:
                    blurDrawer.FinishDrawing();
                    break;
                case EditAction.BrushPainting:
                    brushDrawer.FinishDrawing();
                    break;
            }

            currentAction = EditAction.None;
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            // Aktualizacja rysowania dla bieżącej akcji
            switch (currentAction)
            {
                case EditAction.DrawArrow:
                    arrowDrawer.UpdateDrawing(e);
                    break;
                case EditAction.DrawRectangle:
                    rectangleDrawer.UpdateDrawing(e);
                    break;
                case EditAction.DrawBlur:
                    blurDrawer.UpdateDrawing(e);
                    break;
                case EditAction.BrushPainting:
                    brushDrawer.UpdateDrawing(e);
                    break;
            }
        }

        public void HandlePaste()
        {
            if (Clipboard.ContainsImage())
            {
                var imageSource = Clipboard.GetImage();
                Point canvasPosition = new Point(10, 10); // Przykładowa pozycja na płótnie
                var drawableImage = new DrawableImage(imageSource, canvasPosition);
                drawableCanvas.AddElement(drawableImage);
            }
        }

        public void HandleCopy()
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)drawableCanvas.ActualWidth,
                (int)drawableCanvas.ActualHeight,
                96, 96, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush canvasBrush = new VisualBrush(drawableCanvas);
                context.DrawRectangle(canvasBrush, null, new Rect(new Point(), new Size(drawableCanvas.ActualWidth, drawableCanvas.ActualHeight)));
            }

            renderBitmap.Render(visual);
            Clipboard.SetImage(renderBitmap);
        }

        public void SetCurrentAction(EditAction action)
        {
            currentAction = action;
        }
        private void SelectElementAtMousePosition(MouseButtonEventArgs e)
        {
            drawableCanvas.SelectElementAtPoint(e.GetPosition(drawableCanvas));
        }
    }
}