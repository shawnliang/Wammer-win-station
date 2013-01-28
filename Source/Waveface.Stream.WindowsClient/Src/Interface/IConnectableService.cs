
namespace Waveface.Stream.WindowsClient
{
	public interface IConnectableService
	{
		string Name { get; }
		string Description { get; }
		System.Drawing.Image Icon { get; }


		bool IsEnabled(string user_id, string session_token, string apikey);
		void Connect(string session_token, string apikey);
		void Disconnect(string session_token, string apikey);
	}
}
