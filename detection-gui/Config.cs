using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace detection_gui
{
    internal class Config
    {
        public string? ModelFileLocation { get; set; }
        public string? InputFolder { get; set; }
        public string? OutputFolder { get; set; }
    }
}
