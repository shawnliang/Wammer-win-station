using Wammer.Cloud;
using Waveface.Stream.Model;

namespace Wammer.PostUpload
{
	public class HidePostTask : PostUploadTask
	{
		protected override void Do(Driver driver)
		{
			var postApi = new PostApi(driver);
			postApi.HidePost(Timestamp, Parameters);
		}
	}
}