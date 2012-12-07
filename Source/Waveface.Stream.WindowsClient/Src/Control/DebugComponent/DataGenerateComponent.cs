using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Waveface.Stream.Model;
using Waveface.Stream.ClientFramework;
using MongoDB.Driver.Builders;

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
            var sessionToken = StreamClient.Instance.LoginedUser.SessionToken;
            var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

            if (loginedSession == null)
                return;

            var userID = loginedSession.user.user_id;

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
            var sessionToken = StreamClient.Instance.LoginedUser.SessionToken;
            var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

            if (loginedSession == null)
                return;

            var userID = loginedSession.user.user_id;

            var posts = PostCollection.Instance.Find(Query.And(Query.EQ("creator_id", userID), Query.EQ("hidden", "false"), Query.EQ("code_name", "StreamEvent")))
                        .SetLimit(1)
                        .SetSortOrder(SortBy.Descending("event_time"));

            var post = posts.FirstOrDefault();

            if (post == null)
                return;

			var r = new Random(Guid.NewGuid().GetHashCode());
			var attachments = AttachmentCollection.Instance.FindAll();
			var attachmentID = AttachmentCollection.Instance.FindAll().SetSkip((int)(attachments.Count()) - 1).SetLimit(1).FirstOrDefault().object_id;
            post.attachment_id_array.Add(attachmentID);

            PostCollection.Instance.Save(post);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var sessionToken = StreamClient.Instance.LoginedUser.SessionToken;
            var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

            if (loginedSession == null)
                return;

            var userID = loginedSession.user.user_id;
            var groupID = loginedSession.groups.First().group_id;

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
    }
}
