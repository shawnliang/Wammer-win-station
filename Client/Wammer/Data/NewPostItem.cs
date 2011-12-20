using System;
using System.Collections.Generic;

namespace Waveface
{
    public class NewPostItem
    {
        public PostType PostType { get; set; }
        public string Text { get; set; }
        public string Previews { get; set; }
        public string ResizeRatio { get; set; }
        public List<string> Files { get; set; }
        public Dictionary<string, string> UploadedFiles { get; set; }
        public DateTime OrgPostTime { get; set; }
        public bool PostOK { get; set; }

        public NewPostItem()
        {
            Files = new List<string>();
            UploadedFiles = new Dictionary<string, string>();

            Text = string.Empty;
            Previews = string.Empty;
            ResizeRatio = string.Empty;
        }
    }
}
