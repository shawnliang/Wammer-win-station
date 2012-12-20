using Wammer.Cloud;
using Wammer.Model;
using Waveface.Stream.Model;

namespace Wammer.PostUpload
{
	public class NewPostTask : PostUploadTask
	{
		protected override void Do(Driver driver)
		{
			var postApi = new PostApi(driver);
			postApi.NewPost(PostId, Timestamp, Parameters);
		}
	}
}
