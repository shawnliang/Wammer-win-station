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


	public class NullNamedTask : INamedTask
	{

		public string Name
		{
			get { return "nulltask"; }
		}

		public void Execute()
		{
			
		}
	}
}