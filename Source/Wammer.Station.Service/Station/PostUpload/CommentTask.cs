using Wammer.Cloud;
using Waveface.Stream.Model;

namespace Wammer.PostUpload
{
	public class CommentTask : PostUploadTask
	{
		protected override void Do(Driver driver)
		{
			var postApi = new PostApi(driver);
			postApi.NewComment(Timestamp, Parameters);
		}
	}
}