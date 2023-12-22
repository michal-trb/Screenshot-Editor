using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace screenerWpf.Models
{
    public class ColorInfo
    {
        public required string Name { get; set; }
        public required SolidColorBrush ColorBrush { get; set; }
    }

}
