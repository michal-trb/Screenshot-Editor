using screenerWpf.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screenerWpf.Helpers
{
    public class HistoryManager
    {
        private Stack<IUndoableAction> undoStack = new Stack<IUndoableAction>();
        private Stack<IUndoableAction> redoStack = new Stack<IUndoableAction>();

        public void AddAction(IUndoableAction action)
        {
            undoStack.Push(action);
            redoStack.Clear();
        }

        public bool CanUndo => undoStack.Any();
        public bool CanRedo => redoStack.Any();

        public void Undo()
        {
            if (!CanUndo) return;

            var action = undoStack.Pop();
            action.Unexecute();
            redoStack.Push(action);
        }

        public void Redo()
        {
            if (!CanRedo) return;

            var action = redoStack.Pop();
            action.Execute();
            undoStack.Push(action);
        }
    }

}
