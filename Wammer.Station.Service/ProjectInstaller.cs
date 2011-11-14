using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;


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
