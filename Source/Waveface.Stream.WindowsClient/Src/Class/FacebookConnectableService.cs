using System.ComponentModel.Composition;

namespace Waveface.Stream.WindowsClient
{
	[Export(typeof(IConnectableService))]
	class FacebookConnectableService : WebRedirectConnectableService
	{
		public FacebookConnectableService()
			: base("facebook", "Facebook")
		{
		}
	}
}
