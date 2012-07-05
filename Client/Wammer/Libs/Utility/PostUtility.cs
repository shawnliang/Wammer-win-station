
using System.Collections.Generic;
using Waveface.API.V2;

namespace Waveface
{
    public class PostUtility
    {
        public static Post GenPhotoAttachments(Post post)
        {
            if (post.type == "image")
            {
                if (post.attachments.Count != post.attachment_id_array.Count)
                {
                    List<Attachment> _attachments = new List<Attachment>();

                    foreach (string _id in post.attachment_id_array)
                    {
                        Attachment _a = new Attachment
                        {
                            object_id = _id,
                            type = "image",
                            file_name = _id + "_medium.dat"
                        };

                        _attachments.Add(_a);
                    }

                    post.attachments = _attachments;
                }
            }

            return post;
        }
    }
}
