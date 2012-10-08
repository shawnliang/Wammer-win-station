using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station.AttachmentView
{
	internal class AttachmentViewHandlerDB : IAttachmentViewHandlerDB
	{

		public Model.Attachment GetAttachment(string object_id)
		{
			return Model.AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id));
		}

		public Model.Driver GetUserByGroupId(string group_id)
		{
			return Model.DriverCollection.Instance.FindDriverByGroupId(group_id);
		}
	}
}
