using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
	[Flags]
	public enum PostType
	{
		All = 0,
		Text = 1,
		Photo = 2,
		Music = 4,
		Video = 8,
		Doc = 16,
		WebLink = 32
	}
}
