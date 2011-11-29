using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public enum StationApiError
	{
		Error = -1,
		DriverExist = -30,
		BadPath = -31,
		AuthFailed = -32,
		AlreadyHasStaion = -33,
		ConnectToCloudError = -34
	}

	public enum DropboxApiError
	{
		Base = -200,
		DropboxNotInstalled = Base + 1,
		DropboxNotConnected = Base + 2,
		NoSyncFolder = Base + 3
	}
}
