using System.ComponentModel;
using System.Configuration.Install;

namespace Wammer.Station.Service
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
			serviceInstaller1.ServiceName = StationService.SERVICE_NAME;
		}
	}
}