using System.ComponentModel.Composition;

namespace Waveface.Stream.WindowsClient
{
	[Export(typeof(IConnectableService))]
	class GoogleConnectableService : WebRedirectConnectableService
	{
		public GoogleConnectableService()
			: base("google", "Google")
		{
		}
	}
}
