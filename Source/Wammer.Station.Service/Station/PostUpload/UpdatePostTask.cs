using Wammer.Cloud;
using Waveface.Stream.Model;

namespace Wammer.PostUpload
{
	public class UpdatePostTask : PostUploadTask
	{
		protected override void Do(Driver driver)
		{
			var postApi = new PostApi(driver);
			postApi.UpdatePost(Timestamp, LastUpdateTime, Parameters);
		}
	}
}
