﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public interface IPersonalCloudStatus
	{
		IEnumerable<PersonalCloudNode> GetNodes(string user_id, string session_token, string apikey);
	}
}