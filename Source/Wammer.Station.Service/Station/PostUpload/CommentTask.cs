using Wammer.Cloud;
using Wammer.Model;

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