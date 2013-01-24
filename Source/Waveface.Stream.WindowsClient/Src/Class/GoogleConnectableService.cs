using System.ComponentModel.Composition;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	[Export(typeof(IConnectableService))]
	class GoogleConnectableService : WebRedirectConnectableService
	{
		public GoogleConnectableService()
			: base("google", "Google", Resources.SVC_GOOGLE_DESC)
		{
		}
	}
}
