using screenerWpf.Controls;
using screenerWpf.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screenerWpf.Services
{
    public class AddElementAction : IUndoableAction
    {
        private DrawableCanvas canvas;
        public DrawableElement Element { get; private set; }

        public AddElementAction(DrawableCanvas canvas, DrawableElement element)
        {
            this.canvas = canvas;
            Element = element;
        }

        public void Execute()
        {
            canvas.elementManager.AddElement(Element);
        }

        public void Unexecute()
        {
            canvas.elementManager.RemoveElement(Element);
        }
    }

    public class RemoveElementAction : IUndoableAction
    {
        private DrawableCanvas canvas;
        public DrawableElement Element { get; private set; }

        public RemoveElementAction(DrawableCanvas canvas, DrawableElement element)
        {
            this.canvas = canvas;
            Element = element;
        }

        public void Execute()
        {
            canvas.elementManager.RemoveElement(Element);
        }

        public void Unexecute()
        {
            canvas.elementManager.AddElement(Element);
        }
    }

}
