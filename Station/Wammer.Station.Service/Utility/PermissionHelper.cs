using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using Wammer.Model;

namespace Wammer.Utility
{
	public class PermissionHelper
	{
		public static bool IsGroupPermissionOK(string groupId, LoginedSession session)
		{
			foreach (Group group in session.groups)
			{
				if (group.group_id == groupId)
				{
					return true;
				}
			}
			return false;
		}
	}
}
