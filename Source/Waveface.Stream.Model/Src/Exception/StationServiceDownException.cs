using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
    public class StationServiceDownException : Exception
    {
        public StationServiceDownException(string msg)
            : base(msg)
        {
        }
    }
}
