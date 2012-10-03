using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.AttachmentView
{
	public class AttachmentViewStorage : IAttachmentViewStorage
	{
		public System.IO.Stream GetAttachmentStream(Model.ImageMeta meta, Model.Driver user, string fileName)
		{
			if (meta == Model.ImageMeta.Origin || meta == Model.ImageMeta.None)
			{
				var storage = new FileStorage(user);
				return storage.Load(fileName);
			}
			else
			{
				return FileStorage.LoadFromCacheFolder(fileName);
			}
		}
	}
}
