using System.ComponentModel.Composition;

namespace Waveface.Stream.WindowsClient
{
	[Export(typeof(IConnectableService))]
	class TwitterConnectableService : WebRedirectConnectableService
	{
		public TwitterConnectableService()
			: base("twitter", "Twitter")
		{
		}
	}
}
