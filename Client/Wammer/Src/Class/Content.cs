using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface
{
	/// <summary>
	/// 
	/// </summary>
	public class Content:ContentBase
	{

		public Content()
		{

		}

		public Content(string name, string filePath, ContentType type) : base(name, filePath, type)
		{
		}
	}
}
