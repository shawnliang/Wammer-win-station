using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface
{
	class VersionNotSupportedException : Exception
	{
		public VersionNotSupportedException()
		{
		}

		public VersionNotSupportedException(string msg)
			:base(msg)
		{
		}
	}
}
