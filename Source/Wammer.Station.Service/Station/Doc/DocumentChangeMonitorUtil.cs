﻿using System;
using System.IO;
using Wammer.Model;
using Waveface.Stream.Model;

namespace Wammer.Station.Doc
{
	public class DocumentChangeMonitorUtil : IDocumentChangeMonitorUtil
	{
		public DateTime GetFileWriteTime(string path)
		{
			return File.GetLastAccessTime(path);
		}

		public void ProcessChangedDoc(MonitorItem target, DateTime fileModifyTime)
		{
			var user = DriverCollection.Instance.FindOneById(target.user_id);
			if (user == null)
				return;

			ImportDoc.Process(user, Guid.NewGuid().ToString(), target.path, DateTime.Now);
		}
	}
}
