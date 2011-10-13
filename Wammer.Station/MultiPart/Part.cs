using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Wammer.MultiPart
{
    public class Part
    {
        private byte[] data;
        private int start;
        private int len;

        private string text;
        private NameValueCollection headers;

        public Part(byte[] data, int start, int len, NameValueCollection headers)
        {
            this.data = data;
            this.start = start;
            this.len = len;
            this.headers = headers;
        }

        public string Text
        {
            get
            {
                if (text == null)
                    text = Encoding.UTF8.GetString(data, start, len);

                return text;
            }
        }

        public NameValueCollection Headers
        {
            get { return headers; }
        }
    }
}
