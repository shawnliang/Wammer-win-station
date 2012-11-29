using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
    public enum GeneralApiError
    {
        Base = 0x0000,
        SessionNotExist = Base + 12,
        NotSupportClient = 999
    }
}
