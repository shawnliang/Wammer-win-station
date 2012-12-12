using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	public class NullTask : ITask
	{
		#region ITask Members

		public void Execute()
		{
		}

		#endregion
	}
}