using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
    public class VersionNotSupportedException : Exception
    {
        public VersionNotSupportedException(string msg)
            : base(msg)
        {
        }
    }
}
