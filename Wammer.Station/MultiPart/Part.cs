using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.MultiPart
{
    public class Part
    {
        private byte[] data;
        private int start;
        private int len;

        private string text;

        public Part(byte[] data, int start, int len)
        {
            this.data = data;
            this.start = start;
            this.len = len;
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
    }
}
