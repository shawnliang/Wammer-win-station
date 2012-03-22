using System.ComponentModel;


namespace Wammer.Station.Service
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : System.Configuration.Install.Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
			this.serviceInstaller1.ServiceName = StationService.SERVICE_NAME;
		}
	}
}
