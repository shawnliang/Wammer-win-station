using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Waveface.Stream.Model;

namespace UT_WammerStation
{
	[TestClass]
	public class TestMongoDB
	{
		[TestMethod]
		public void updateArrayElement()
		{
			Attachment a = new Attachment
			{
				object_id = "123",
				web_meta = new WebProperty
				{
					thumbs = new List<WebThumb> {
							    new WebThumb { id = 1},
								new WebThumb { id = 2},
								new WebThumb { id = 3},
								new WebThumb { id = 4},
							 }
				}
			};

			AttachmentCollection.Instance.Save(a);


			AttachmentCollection.Instance.UpdateWebThumbSavedFile("123", 3, "saved.jpg");

			var modified = AttachmentCollection.Instance.FindOneById("123");
			Assert.AreEqual("saved.jpg", modified.web_meta.thumbs[2].saved_file_name);
		}
	}
}
