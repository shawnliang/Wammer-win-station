using System.ComponentModel.Composition;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	[Export(typeof(IConnectableService))]
	class FacebookConnectableService : WebRedirectConnectableService
	{
		public FacebookConnectableService()
			: base("facebook", "Facebook", Resources.SVC_FACEBOOK_DESC)
		{
		}

		public override System.Drawing.Image Icon
		{
			get { return Properties.Resources.Facebook; }
		}
	}
}
