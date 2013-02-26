using MongoDB.Driver.Builders;
using System;
using Waveface.Stream.Model;

namespace Wammer.Station.AttachmentView
{
	internal class AttachmentViewHandlerDB : IAttachmentViewHandlerDB
	{

		public Attachment GetAttachment(string object_id)
		{
			return AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id));
		}

		public Driver GetUserByGroupId(string group_id)
		{
			return DriverCollection.Instance.FindDriverByGroupId(group_id);
		}

		public void UpdateLastAccessTime(string object_id)
		{
			AttachmentCollection.Instance.Update(
				Query.EQ("_id", object_id),
				Update.Set("last_access", DateTime.Now));
		}
	}
}
