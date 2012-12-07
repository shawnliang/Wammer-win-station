
namespace Wammer.Station.AttachmentView
{
	public class AttachmentViewStorage : IAttachmentViewStorage
	{
		public System.IO.Stream GetAttachmentStream(Model.ImageMeta meta, Model.Driver user, string fileName)
		{
			return new FileStorage(user).Load(fileName, meta);
		}
	}
}
