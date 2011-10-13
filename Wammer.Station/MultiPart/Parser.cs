using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Wammer.MultiPart
{
    public class Parser
    {
        private static byte[] CRLF = Encoding.UTF8.GetBytes("\r\n");

        private byte[] head_boundry;
        private byte[] close_boundry;

        public Parser(string boundry)
        {
            head_boundry = Encoding.UTF8.GetBytes("--" + boundry);
            close_boundry = Encoding.UTF8.GetBytes("--" + boundry + "--");
        }

        public Part[] Parse(Stream stream)
        {
            List<Part> parts = new List<Part>();

            byte[] content = ToByteArray(stream);

            int startFrom = 0;
            while (startFrom < content.Length)
            {
                int index = IndexOf(content, startFrom, CRLF);
                if (index < 0)
                    throw new FormatException("Not a wellformed multipart content");

                if (HasSubString(content, index + 2, close_boundry))
                {
                    return parts.ToArray();
                }
                else if (HasSubString(content, index + 2, head_boundry))
                {
                    int bodyLen;
                    
                    // "\r\n" + head_boundry + "\r\n"
                    int bodyStartIdx = index + 2 + head_boundry.Length + 2;
                    Part part = ParsePartBody(content, bodyStartIdx, out bodyLen);
                    parts.Add(part);

                    startFrom = bodyStartIdx + bodyLen;
                    continue;
                }
                

                startFrom = index + 2;
            }

            throw new FormatException("Not a wellformed multipart content");
        }

        private Part ParsePartBody(byte[] data, int startIdx, out int partLen)
        {
            int index = 0;
            int startFrom = startIdx;

            bool headerFound = false;
            int headerEndIndex = -1;
            while ((index = IndexOf(data, startFrom, CRLF)) >= 0)
            {
                if (!headerFound && IsInFront(data, index, CRLF))
                {
                    headerFound = true;
                    headerEndIndex = index;
                }

                if (headerFound && HasSubString(data, index + 2, head_boundry))
                {
                    int dataStartIndex = headerEndIndex + 2;
                    partLen = index - startIdx;
                    return new Part(data, dataStartIndex, index - dataStartIndex);
                }

                startFrom = index + 2;
            }

            throw new FormatException("Bad part body format");
        }

        // is byte array "what" in front of EndInx?
        private static bool IsInFront(byte[] content, int endIdx, byte[] what)
        {
            int fromIdx = endIdx - what.Length;
            if (fromIdx < 0)
                return false;

            return CommonPrefixCount(content, fromIdx, what) == what.Length;
        }

        private static byte[] ToByteArray(Stream stream)
        {
            byte[] buf = new byte[32768];
            int nread = 0;
            MemoryStream m = new MemoryStream();
            do
            {
                nread = stream.Read(buf, 0, buf.Length);
                if (nread > 0)
                    m.Write(buf, 0, nread);
            }
            while (nread > 0);

            return m.ToArray();
        }


        private static bool HasSubString(byte[] data, int startIdx, byte[] substr)
        {
            return CommonPrefixCount(data, startIdx, substr) == substr.Length;
        }

        // count the common prefix bytes of a byte array
        private static int CommonPrefixCount(byte[] data, int startIdx, byte[] what)
        {
            int commPrefixCount = 0;

            for (int i = 0; i < what.Length && startIdx+i < data.Length; i++)
            {
                if (data[startIdx + i] == what[i])
                    commPrefixCount++;
                else
                    break;
            }

            return commPrefixCount;
        }

        // find "what" in a byte array
        private static int IndexOf(byte[] data, int startIdx, byte[] whatBytes)
        {
            int startFrom = startIdx;
            while (startFrom < data.Length)
            {
                int commonPrefix = CommonPrefixCount(data, startFrom, whatBytes);
                if (commonPrefix == whatBytes.Length)
                    return startFrom;

                startFrom += commonPrefix + 1;
            }

            return -1;
        }
    }
}
