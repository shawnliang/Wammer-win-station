using Wammer.Cloud;
using Wammer.Model;

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