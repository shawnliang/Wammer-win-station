using System;
using System.Collections.Generic;

namespace Waveface
{
    public class NewPostItem
    {
        private List<string> m_files = new List<string>();
        private List<string> m_uploadedFiles; 
        private string m_text;
        private string m_resizeRatio;
        private DateTime m_orgPostTime;

        public List<string> Files
        {
            get { return m_files; }
            set { m_files = value; }
        }

        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public List<string> UploadedFiles
        {
            get { return m_uploadedFiles; }
            set { m_uploadedFiles = value; }
        }

        public string ResizeRatio
        {
            get { return m_resizeRatio; }
            set { m_resizeRatio = value; }
        }

        public DateTime OrgPostTime
        {
            get { return m_orgPostTime; }
            set { m_orgPostTime = value; }
        }
    }
}
