using screenerWpf.CanvasHandler.Drawers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace screenerWpf
{
    public enum EditAction { None, DrawArrow, AddText, Move, Delete, AddBubble, DrawRectangle }

    public class CanvasActionHandler
    {
        private DrawableCanvas drawableCanvas;
        private IDrawable currentDrawable;
        private EditAction currentAction = EditAction.None;

        // Dodanie obiektów Drawer
        private ArrowDrawer arrowDrawer;
        private RectangleDrawer rectangleDrawer;
        private SpeechBubbleDrawer speechBubbleDrawer;
        private TextDrawer textDrawer;

        public CanvasActionHandler(DrawableCanvas canvas)
        {
            drawableCanvas = canvas;

            // Inicjalizacja obiektów Drawer
            arrowDrawer = new ArrowDrawer(currentDrawable, canvas);
            rectangleDrawer = new RectangleDrawer(canvas);
            speechBubbleDrawer = new SpeechBubbleDrawer(canvas);
            textDrawer = new TextDrawer(canvas);
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
            }
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