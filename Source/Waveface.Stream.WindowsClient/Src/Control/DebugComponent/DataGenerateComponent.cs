using MongoDB.Driver.Builders;
using System;
using System.Linq;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	public partial class DataGenerateComponent : UserControl, IDebugComponent
	{
		public DataGenerateComponent()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var userID = StreamClient.Instance.LoginedUser.UserID;

			var posts = PostCollection.Instance.Find(Query.And(Query.EQ("creator_id", userID), Query.EQ("hidden", "false"), Query.EQ("code_name", "StreamEvent")));

			var postCount = posts.Count();
			if (postCount == 0)
				return;

			Random r = new Random(Guid.NewGuid().GetHashCode());
			var post = posts.ElementAt(r.Next((int)(postCount - 1)));

			post.post_id = Guid.NewGuid().ToString();
			post.timestamp = DateTime.Now;
			post.event_time = post.timestamp.ToLocalISO8601ShortString();
			post.update_time = post.timestamp;

			post.code_name = "StreamEvent";

			post.content = string.Format("Generate random event at {0}", post.event_time);


			PostCollection.Instance.Save(post);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var userID = StreamClient.Instance.LoginedUser.UserID;

			var posts = PostCollection.Instance.Find(Query.And(Query.EQ("creator_id", userID), Query.EQ("hidden", "false"), Query.EQ("code_name", "StreamEvent")))
						.SetLimit(1)
						.SetSortOrder(SortBy.Descending("event_time"));

			var post = posts.FirstOrDefault();

			if (post == null)
				return;

			var r = new Random(Guid.NewGuid().GetHashCode());
			var attachments = AttachmentCollection.Instance.FindAll();
			var attachmentID = AttachmentCollection.Instance.FindAll().SetSkip(r.Next((int)(attachments.Count()) - 1)).SetLimit(1).FirstOrDefault().object_id;
			post.attachment_id_array.Add(attachmentID);

			PostCollection.Instance.Save(post);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			var userID = StreamClient.Instance.LoginedUser.UserID;
			var groupID = StreamClient.Instance.LoginedUser.GroupID;

			var attachments = AttachmentCollection.Instance.Find(Query.EQ("group_id", groupID));

			var attachmentCount = attachments.Count();
			if (attachmentCount == 0)
				return;

			Random r = new Random(Guid.NewGuid().GetHashCode());
			var attachment = attachments.ElementAt(r.Next((int)(attachmentCount - 1)));

			attachment.object_id = Guid.NewGuid().ToString();
			attachment.event_time = DateTime.Now;

			AttachmentCollection.Instance.Save(attachment);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			var userID = StreamClient.Instance.LoginedUser.UserID;

			var collections = CollectionCollection.Instance.Find(Query.And(Query.EQ("creator_id", userID), Query.EQ("hidden", false)));

			var collectionCount = collections.Count();
			if (collectionCount == 0)
				return;

			Random r = new Random(Guid.NewGuid().GetHashCode());
			var collection = collections.ElementAt(r.Next((int)(collectionCount - 1)));

			collection.collection_id = Guid.NewGuid().ToString();
			collection.create_time = DateTime.Now.ToUTCISO8601ShortString();
			collection.modify_time = collection.create_time;

			collection.name = string.Format("Generate random collection at {0}", collection.create_time);

			CollectionCollection.Instance.Save(collection);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			var userID = StreamClient.Instance.LoginedUser.UserID;

			var collections = CollectionCollection.Instance.Find(Query.And(Query.EQ("creator_id", userID), Query.EQ("hidden", false)))
						.SetLimit(1)
						.SetSortOrder(SortBy.Descending("create_time"));

			var collection = collections.FirstOrDefault();

			if (collection == null)
				return;

			var r = new Random(Guid.NewGuid().GetHashCode());
			var attachments = AttachmentCollection.Instance.FindAll();
			var attachmentID = AttachmentCollection.Instance.FindAll().SetSkip(r.Next((int)(attachments.Count()) - 1)).SetLimit(1).FirstOrDefault().object_id;
			collection.attachment_id_array.Add(attachmentID);

			CollectionCollection.Instance.Save(collection);
		}
	}
}
