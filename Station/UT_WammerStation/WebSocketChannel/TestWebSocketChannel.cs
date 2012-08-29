using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSocketSharp;
using Wammer.Station.Notify;
using System.Threading;
using Wammer.Utility;

namespace UT_WammerStation.WebSocketChannel
{
	/// <summary>
	/// Summary description for WebSocketChannel
	/// </summary>
	[TestClass]
	public class TestWebSocketChannel
	{
		private WebSocketNotifyChannels wsServer;
		private int wsPort = 9999;
		private StreamWebSocketClient client;

		[TestInitialize]
		public void Setup()
		{
			NotificationWebSocketService.ClearAllChannels();
			wsServer = new WebSocketNotifyChannels(wsPort);
			wsServer.Start();


			client = new StreamWebSocketClient("ws://localhost:9999");
		}

		[TestCleanup]
		public void TearDown()
		{
			wsServer.Stop();
			client.Close();
		}

		[TestMethod]
		public void InitEmpty()
		{
			var channels = wsServer.GetChannelsByUser("user");
			Assert.AreEqual(0, channels.Count());

		}

		[TestMethod]
		public void ChannelIsAddedWhenConnectionAccepted()
		{
			ManualResetEvent added = new ManualResetEvent(false);
			wsServer.ChannelAdded += (sender, evt) => { added.Set(); };

			// connect 
			client.Connect("session", "apikey", "user", 2000);

			
			// verify
			Assert.IsTrue(added.WaitOne(3000));
			var channels = wsServer.GetChannelsByUser("user");
			Assert.AreEqual(1, channels.Count());
			Assert.AreEqual("apikey", channels.First().ApiKey);
			Assert.AreEqual("session", channels.First().SessionToken);
			Assert.AreEqual("user", channels.First().UserId);
		}


		[TestMethod]
		public void ChannelIsRemovedWhenConnectionClosed()
		{
			ManualResetEvent removed = new ManualResetEvent(false);
			wsServer.ChannelRemoved += (sender, evt) => { removed.Set(); };

			// connect + disconnect
			client.Connect("session", "apikey", "user", 2000);
			client.Close();

			// verify
			Assert.IsTrue(removed.WaitOne(3000));
			var remains = wsServer.GetChannelsByUser("user");
			Assert.AreEqual(0, remains.Count());
		}

		[TestMethod]
		public void NotifyTest()
		{
			ManualResetEvent added = new ManualResetEvent(false);
			INotifyChannel channel = null;
			wsServer.ChannelAdded += (s, e) => { channel = e.Channel; added.Set(); };
			
			// connect
			string notifyData = "";
			ManualResetEvent notified = new ManualResetEvent(false);
			client.socket.OnMessage += (s, e) => { notifyData = e.Data; notified.Set(); };
			client.Connect("session", "apikey", "user", 2000);
			Assert.IsTrue(added.WaitOne(3000));

			// send notify
			channel.Notify();

			// verify
			Assert.IsTrue(notified.WaitOne(3000));
			var cmd = fastJSON.JSON.Instance.ToObject<GenericCommand>(notifyData);
			Assert.IsNotNull(cmd.notify);
			Assert.IsTrue(cmd.notify.updated);
		}

		[TestMethod]
		public void NotifyAllExcept()
		{
			ManualResetEvent added = new ManualResetEvent(false);
			INotifyChannel channel = null;
			wsServer.ChannelAdded += (s, e) => { channel = e.Channel; added.Set(); };

			// connect
			string notifyData = "";
			ManualResetEvent notified = new ManualResetEvent(false);
			client.socket.OnMessage += (s, e) => { notifyData = e.Data; notified.Set(); };
			client.Connect("session", "apikey", "user", 2000);
			Assert.IsTrue(added.WaitOne(3000));

			// send notify
			wsServer.NotifyToUserChannels("user", "no_such_session");

			// verify
			Assert.IsTrue(notified.WaitOne(3000));
			var cmd = fastJSON.JSON.Instance.ToObject<GenericCommand>(notifyData);
			Assert.IsNotNull(cmd.notify);
			Assert.IsTrue(cmd.notify.updated);
		}

		[TestMethod]
		public void CloseWithStatusCodeAndReason()
		{
			
			ManualResetEvent added = new ManualResetEvent(false);
			ManualResetEvent closed = new ManualResetEvent(false);
			WebSocketSharp.Frame.CloseStatusCode closeStatus = WebSocketSharp.Frame.CloseStatusCode.NO_STATUS_CODE;
			string closeReason = "";
			wsServer.ChannelAdded += (s, e) =>
			{
				added.Set();
			};

			// connect
			client.socket.OnClose += (s, e) => 
			{
				closeStatus = e.Code;
				closeReason = e.Reason;
				closed.Set();
			};
			client.Connect("session", "api", "user", 1000);
			Assert.IsTrue(added.WaitOne(2000));

			// close connection
			var chs = wsServer.GetChannelsByUser("user");
			Assert.AreEqual(1, chs.Count());
			wsServer.CloseChannel(chs.First(), WebSocketSharp.Frame.CloseStatusCode.POLICY_VIOLATION, "test123");
			
			// verify
			Assert.AreEqual(0, wsServer.GetChannelsByUser("user").Count());
			Assert.IsTrue(closed.WaitOne(1000));
			Assert.AreEqual(WebSocketSharp.Frame.CloseStatusCode.POLICY_VIOLATION, closeStatus);
			Assert.AreEqual("test123", closeReason);
		}

		[TestMethod]
		public void NotifyAllExcept_ExceptSessionWontBeSent()
		{
			ManualResetEvent added = new ManualResetEvent(false);
			INotifyChannel channel = null;
			wsServer.ChannelAdded += (s, e) => { channel = e.Channel; added.Set(); };

			// connect
			string notifyData = "";
			ManualResetEvent notified = new ManualResetEvent(false);
			client.socket.OnMessage += (s, e) =>
			{
				notifyData = e.Data;
				notified.Set();
			};

			client.Connect("session", "apikey", "user", 2000);
			Assert.IsTrue(added.WaitOne(3000));

			// send notify
			wsServer.NotifyToUserChannels("user", "session");

			// verify
			Assert.IsFalse(notified.WaitOne(2000));
		}
	}



	class StreamWebSocketClient
	{
		public WebSocket socket;
		string errorMsg;
		ManualResetEvent connectEvt = new ManualResetEvent(false);

		bool isClosed;

		public StreamWebSocketClient(string url)
		{
			socket = new WebSocket(url);
			socket.OnOpen += new EventHandler(socket_OnOpen);
			socket.OnError += new EventHandler<ErrorEventArgs>(socket_OnError);
		}

		void socket_OnOpen(object sender, EventArgs e)
		{
			connectEvt.Set();
		}

		void socket_OnError(object sender, ErrorEventArgs e)
		{
			errorMsg = e.Message;
		}

		public void Connect(string session_token, string apikey, string user_id, int timeout)
		{
			socket.Connect();
			if (!connectEvt.WaitOne(timeout))
				throw new TimeoutException();

			if (errorMsg != null)
				throw new Exception(errorMsg);

			var cmd = new GenericCommand
			{
				connect = new ConnectMsg
				{
					apikey = apikey,
					session_token = session_token,
					user_id = user_id
				},
				subscribe = new SubscribeMSg { since_seq_num = 12345 }
			};

			socket.Send(cmd.ToFastJSON());
		}

		public void Close()
		{
			if (!isClosed)
				socket.Close();

			isClosed = true;
		}
	}


	
}
