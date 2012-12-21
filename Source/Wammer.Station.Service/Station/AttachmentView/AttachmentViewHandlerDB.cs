using MongoDB.Driver.Builders;
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
	}
}
