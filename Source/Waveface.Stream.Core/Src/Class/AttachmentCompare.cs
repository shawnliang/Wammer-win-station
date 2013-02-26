using System.Collections.Generic;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	public class AttachmentCompare : IEqualityComparer<Attachment>
	{
		public bool Equals(Attachment x, Attachment y)
		{
			return x.object_id.Equals(y.object_id);
		}

		public int GetHashCode(Attachment obj)
		{
			return obj.object_id.GetHashCode();
		}
	}
}
