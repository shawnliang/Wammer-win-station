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
        Thread thread;
        string station;

        public int error_count { get; private set; }
        public int success_count { get; private set; }
        public TimeSpan total_duration { get; private set; }

        public UploadThread(int upload_count, byte[] image_data, ManualResetEvent start_event, string session_token, string group_id, string station)
        {
            this.image_data = image_data;
            this.upload_count = upload_count;
            this.start_event = start_event;
            this.session_token = session_token;
            this.group_id = group_id;
            this.station = station;

            total_duration = TimeSpan.FromTicks(0);
        }

        public void Start()
        {
            thread = new Thread(this.Do);
            thread.Start();
        }

        public void Join()
        {
            thread.Join();
        }

        private void Do()
        {
            if (!start_event.WaitOne())
                throw new InvalidOperationException("start_event is not waitable");

            for (int i = 0; i < upload_count; i++)
            {
                DateTime start = DateTime.Now;

                try
                {
                    Attachment.UploadImage("http://" + station + ":9981/v2/attachments/upload",
                        image_data,
                        group_id,
                        null,
                        "hard_code_file_from_test_prog.jpg",
                        "image/jpeg",
                        ImageMeta.Origin,
                        "0ffd0a63-65ef-512b-94c7-ab3b33117363",
                        session_token);

                    DateTime stop = DateTime.Now;

                    total_duration += stop - start;
                    success_count++;
                }
                catch (Wammer.Cloud.WammerCloudException e)
                {
                    error_count++;
                    Console.WriteLine("Image upload failed: " + e.ToString() + " at thread " + Thread.CurrentThread.ManagedThreadId);
                }
                catch (Exception e)
                {
                    error_count++;
                    Console.WriteLine("Image upload failed: " + e.Message + " at thread " + Thread.CurrentThread.ManagedThreadId);
                }
            }
        }
    }
}
