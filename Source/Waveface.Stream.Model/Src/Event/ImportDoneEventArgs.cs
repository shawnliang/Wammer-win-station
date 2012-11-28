using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
    public class ImportDoneEventArgs : EventArgs
    {
        public Exception Error { get; set; }
    }
}
