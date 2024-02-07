using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screenerWpf.Interfaces
{
    public interface IUndoableAction
    {
        void Execute();
        void Unexecute();
    }
}
