using System;
using System.Collections.Generic;

namespace Waveface
{
    public class BatchPostItem
    {
        public PostType PostType { get; set; }
        public string Text { get; set; }
        public string Previews { get; set; }
        public string LongSideResizeOrRatio { get; set; }
        public List<string> Files { get; set; }
        public Dictionary<string, string> UploadedFiles { get; set; }
        public DateTime OrgPostTime { get; set; }
        public bool PostOK { get; set; }
        public bool ErrorAndDeletePost { get; set; }

        public BatchPostItem()
        {
            Files = new List<string>();
            UploadedFiles = new Dictionary<string, string>();

            Text = string.Empty;
            Previews = string.Empty;
            LongSideResizeOrRatio = string.Empty;
        }
    }
}
