using System.Net.Sockets;

namespace Waveface.Common
{
	public class MongoDbHelper
	{
		public static bool IsMongoDBReady(string hostname, int port)
		{
			var tcp = new TcpClient();

			try
			{
				var asyncResult = tcp.BeginConnect(hostname, port, null, null);

				if (asyncResult.AsyncWaitHandle.WaitOne(5000))
				{
					tcp.EndConnect(asyncResult);
					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
			finally
			{
				tcp.Close();
			}
		}
	}
}
