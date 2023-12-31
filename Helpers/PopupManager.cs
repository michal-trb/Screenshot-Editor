using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screenerWpf.Helpers
{
    public static class PopupManager
    {
        public static event EventHandler PopupsClosed;

        public static void CloseAllPopups()
        {
            PopupsClosed?.Invoke(null, EventArgs.Empty);
        }
    }

}
