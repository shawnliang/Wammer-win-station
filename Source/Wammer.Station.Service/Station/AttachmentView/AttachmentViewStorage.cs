
using Waveface.Stream.Model;
namespace Wammer.Station.AttachmentView
{
	public class AttachmentViewStorage : IAttachmentViewStorage
	{
		public System.IO.Stream GetAttachmentStream(ImageMeta meta, Driver user, string fileName)
		{
			return new FileStorage(user).Load(fileName, meta);
		}
	}
}
