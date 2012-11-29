using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string msg)
            : base(msg)
        {
        }
    }
}
