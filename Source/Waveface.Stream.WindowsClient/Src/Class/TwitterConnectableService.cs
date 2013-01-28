using System.ComponentModel.Composition;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	[Export(typeof(IConnectableService))]
	class TwitterConnectableService : WebRedirectConnectableService
	{
		public TwitterConnectableService()
			: base("twitter", "Twitter", Resources.SVC_TWITTER_DESC)
		{
		}
	}
}
