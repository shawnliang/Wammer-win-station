using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Wammer.Model;

namespace TestUploadImage
{
    class UploadThread
    {
        byte[] image_data;
        int upload_count;
        ManualResetEvent start_event;
        string session_token;
        string group_id;

        public UploadThread(int upload_count, byte[] image_data, ManualResetEvent start_event, string session_token, string group_id)
        {
            this.image_data = image_data;
            this.upload_count = upload_count;
            this.start_event = start_event;
            this.session_token = session_token;
            this.group_id = group_id;
        }

        private void Do()
        {
            if (!start_event.WaitOne())
                throw new InvalidOperationException("start_event is not waitable");

            for (int i = 0; i < upload_count; i++)
            {
                try
                {
                    Attachment.UploadImage("http://localhost:9981/v2/attachments/upload",
                        image_data, 
                        group_id, 
                        null, 
                        "hard_code_file_from_test_prog.jpg", 
                        "image/jpeg", 
                        ImageMeta.Origin,
                        "api-key", 
                        session_token);
                }
                catch (Exception e)
                {

                }

            }
        }
    }
}
