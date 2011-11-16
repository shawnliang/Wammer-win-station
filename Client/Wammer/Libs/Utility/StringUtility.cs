using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Waveface
{
    public class StringUtility
    {
        public static string Compress(string text)
        {
            byte[] _buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream _ms = new MemoryStream();

            using (GZipStream _zip = new GZipStream(_ms, CompressionMode.Compress, true))
            {
                _zip.Write(_buffer, 0, _buffer.Length);
            }

            _ms.Position = 0;
            MemoryStream _outStream = new MemoryStream();

            byte[] _compressed = new byte[_ms.Length];
            _ms.Read(_compressed, 0, _compressed.Length);

            byte[] _gzBuffer = new byte[_compressed.Length + 4];
            Buffer.BlockCopy(_compressed, 0, _gzBuffer, 4, _compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(_buffer.Length), 0, _gzBuffer, 0, 4);

            return Convert.ToBase64String(_gzBuffer);
        }

        public static string Decompress(string compressedText)
        {
            byte[] _gzBuffer = Convert.FromBase64String(compressedText);

            using (MemoryStream _ms = new MemoryStream())
            {
                int _msgLength = BitConverter.ToInt32(_gzBuffer, 0);
                _ms.Write(_gzBuffer, 4, _gzBuffer.Length - 4);

                byte[] _buffer = new byte[_msgLength];

                _ms.Position = 0;

                using (GZipStream _zip = new GZipStream(_ms, CompressionMode.Decompress))
                {
                    _zip.Read(_buffer, 0, _buffer.Length);
                }

                return Encoding.UTF8.GetString(_buffer);
            }
        }

        public static string ToUTF8(string s)
        {
            //Worng
            byte[] _byteArray = Encoding.UTF8.GetBytes(s);
            byte[] _asciiArray = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, _byteArray);
            return Encoding.UTF8.GetString(_asciiArray);
        }

        public static string GetUrlFileName(string url)
        {
            int _idx = url.LastIndexOf("/");

            if(_idx > 0)
            {
                return url.Substring(_idx + 1);
            }

            return url;
        }
    }
}
