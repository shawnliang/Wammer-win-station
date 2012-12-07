using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Text;
using Wammer.PostUpload;
using Wammer.Station.Notify;
using Wammer.Utility;

namespace UT_WammerStation.WebSocketChannel
{
	/// <summary>
	/// Summary description for TestPostUpsertNotifier
	/// </summary>
	[TestClass]
	public class TestPostUpsertNotifier
	{
		[TestInitialize]
		public void Setup()
		{
			NotificationWebSocketService.ClearAllChannels();
		}

		[TestMethod]
		public void TestOnPostUpserted()
		{
			Mock<INotifyChannels> server = new Mock<INotifyChannels>(MockBehavior.Strict);
			Mock<IPostUpsertNotifierDB> db = new Mock<IPostUpsertNotifierDB>(MockBehavior.Strict);
			server.Setup(x => x.NotifyToUserChannels("user", "except")).Verifiable();

			PostUpsertNotifier notifier = new PostUpsertNotifier(server.Object, db.Object);

			notifier.OnPostUpserted(this, new PostUpsertEventArgs("postId", "except", "user"));

			// verify
			server.VerifyAll();
		}

		[TestMethod]
		public void TestBypassed_ResponseIsTotallyUseless()
		{
			Mock<INotifyChannels> server = new Mock<INotifyChannels>(MockBehavior.Strict);
			server.Setup(x => x.NotifyToAllChannels("")).Verifiable();
			Mock<IPostUpsertNotifierDB> db = new Mock<IPostUpsertNotifierDB>(MockBehavior.Strict);

			PostUpsertNotifier notifier = new PostUpsertNotifier(server.Object, db.Object);
			var resp = Encoding.UTF8.GetBytes(new MinimalPostResponse().ToFastJSON());
			notifier.OnPostRequestBypassed(this, new Wammer.Station.BypassedEventArgs(resp));
		}

		[TestMethod]
		public void TestBypassed_ResponseHasOnlySessionToken()
		{
			Mock<INotifyChannels> server = new Mock<INotifyChannels>(MockBehavior.Strict);
			Mock<IPostUpsertNotifierDB> db = new Mock<IPostUpsertNotifierDB>(MockBehavior.Strict);
			server.Setup(x => x.NotifyToAllChannels("session")).Verifiable();

			PostUpsertNotifier notifier = new PostUpsertNotifier(server.Object, db.Object);
			var resp = Encoding.UTF8.GetBytes(new MinimalPostResponse() { session_token = "session" }.ToFastJSON());
			notifier.OnPostRequestBypassed(this, new Wammer.Station.BypassedEventArgs(resp));
		}

		[TestMethod]
		public void TestBypassed_ResponseHasOnlyGroupId()
		{
			Mock<INotifyChannels> server = new Mock<INotifyChannels>(MockBehavior.Strict);
			server.Setup(x => x.NotifyToUserChannels("user_id", "")).Verifiable();
			Mock<IPostUpsertNotifierDB> db = new Mock<IPostUpsertNotifierDB>(MockBehavior.Strict);
			db.Setup(x => x.GetUserIdByGroupId("group")).Returns("user_id").Verifiable();

			PostUpsertNotifier notifier = new PostUpsertNotifier(server.Object, db.Object);
			var resp = Encoding.UTF8.GetBytes(new MinimalPostResponse() { post = new MinimalPostResponse.Post { group_id = "group" } }.ToFastJSON());
			notifier.OnPostRequestBypassed(this, new Wammer.Station.BypassedEventArgs(resp));


			// verify
			server.VerifyAll();
			db.VerifyAll();
		}

		[TestMethod]
		public void TestBypassed_ResponseHasGroupIdAndSession()
		{
			Mock<INotifyChannels> server = new Mock<INotifyChannels>(MockBehavior.Strict);
			server.Setup(x => x.NotifyToUserChannels("user_id", "session")).Verifiable();
			Mock<IPostUpsertNotifierDB> db = new Mock<IPostUpsertNotifierDB>(MockBehavior.Strict);
			db.Setup(x => x.GetUserIdByGroupId("group")).Returns("user_id").Verifiable();

			PostUpsertNotifier notifier = new PostUpsertNotifier(server.Object, db.Object);
			var resp = Encoding.UTF8.GetBytes(new MinimalPostResponse() { post = new MinimalPostResponse.Post { group_id = "group" }, session_token = "session" }.ToFastJSON());
			notifier.OnPostRequestBypassed(this, new Wammer.Station.BypassedEventArgs(resp));


			// verify
			server.VerifyAll();
			db.VerifyAll();
		}
	}
}
