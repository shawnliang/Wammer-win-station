using CommandLine;
using System;

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
