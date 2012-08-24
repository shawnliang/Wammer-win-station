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

		[TestInitialize]
		public void Setup()
		{
			NotificationWebSocketService.ClearAllChannels();
			wsServer = new WebSocketNotifyChannels(wsPort);
			wsServer.Start();
		}

		[TestCleanup]
		public void TearDown()
		{
			wsServer.Stop();
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
			var client = new StreamWebSocketClient("ws://127.0.0.1:9999");
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
			var client = new StreamWebSocketClient("ws://127.0.0.1:9999");
			client.Connect("session", "apikey", "user", 2000);
			client.Close();

			// verify
			Assert.IsTrue(removed.WaitOne(3000));
			var remains = wsServer.GetChannelsByUser("user");
			Assert.AreEqual(0, remains.Count());
		}
	}



	class StreamWebSocketClient
	{
		WebSocket socket;
		string errorMsg;
		ManualResetEvent connectEvt = new ManualResetEvent(false);
		

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
			socket.Close();
		}
	}


	
}
