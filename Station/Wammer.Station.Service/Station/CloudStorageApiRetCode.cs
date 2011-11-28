using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public enum DropboxApiError
	{
		Base = -200,
		DropboxNotInstalled = Base + 1,
		DropboxNotConnected = Base + 2,
		NoSyncFolder = Base + 3,
		GetOAuthFailed = Base + 4,
		ConnectDropboxFailed = Base + 5
	}
}
