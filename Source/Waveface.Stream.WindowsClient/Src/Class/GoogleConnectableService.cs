using Newtonsoft.Json;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Waveface.Stream.Model;
using System.Linq;
using Waveface.Stream.ClientFramework;
using System.ComponentModel.Composition;

namespace Waveface.Stream.WindowsClient
{
	[Export(typeof(IConnectableService))]
	class GoogleConnectableService : WebRedirectConnectableService
	{
		public GoogleConnectableService()
			:base("google", "Google")
		{
		}
	}
}
