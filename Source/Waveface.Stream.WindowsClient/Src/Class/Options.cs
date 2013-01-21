using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.WindowsClient
{
	public class Options
	{
		[Option("d", "debug")]
		public Boolean IsDebugMode { get; set; }

		[OptionArray("i", "import")]
		public String[] Imports { get; set; }
	}
}
