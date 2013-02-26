using System.ComponentModel.Composition;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	[Export(typeof(IConnectableService))]
	class FoursquareConnectableService : WebRedirectConnectableService
	{
		public FoursquareConnectableService()
			: base("foursquare", "foursquare", Resources.SVC_FOURSQUARE_DESC)
		{
		}

		public override System.Drawing.Image Icon
		{
			get { return Properties.Resources.Foursquare; }
		}
	}
}
