#region

using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

#endregion

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

            if (_idx > 0)
            {
                return url.Substring(_idx + 1);
            }

            return url;
        }

        public static string UTF8ToISO_8859_1(string str)
        {
            byte[] _utf8Bytes = Encoding.UTF8.GetBytes(str);
            byte[] _iso_8859_1_Bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), _utf8Bytes);

            return Encoding.UTF8.GetString(_iso_8859_1_Bytes);
        }

        public static string CalculateMD5Hash(string strInput)
        {
            MD5 _md5 = MD5.Create();

            byte[] _inputBytes = Encoding.ASCII.GetBytes(strInput);
            byte[] _hash = _md5.ComputeHash(_inputBytes);

            StringBuilder _sb = new StringBuilder();

            for (int i = 0; i < _hash.Length; i++)
                _sb.Append(_hash[i].ToString("x2"));

            return _sb.ToString();
        }

        #region Chinese

        public static bool IsChineseString(string testStr)
        {
            char[] _words = testStr.ToCharArray();
            
            foreach (char _word in _words)
            {
                if (IsBig5Code(_word.ToString()) || IsGBCode(_word.ToString()) || IsGBKCode(_word.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsGBCode(string word)
        {
            byte[] _bytes = Encoding.GetEncoding("GB2312").GetBytes(word);
            
            if (_bytes.Length <= 1) // if there is only one byte, it is ASCII code or other code
            {
                return false;
            }
            else
            {
                byte _byte1 = _bytes[0];
                byte _byte2 = _bytes[1];

                if (_byte1 >= 176 && _byte1 <= 247 && _byte2 >= 160 && _byte2 <= 254) //判断是否是GB2312
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static bool IsGBKCode(string word)
        {
            byte[] _bytes = Encoding.GetEncoding("GBK").GetBytes(word);

            if (_bytes.Length <= 1) // if there is only one byte, it is ASCII code
            {
                return false;
            }
            else
            {
                byte _byte1 = _bytes[0];
                byte _byte2 = _bytes[1];
               
                if (_byte1 >= 129 && _byte1 <= 254 && _byte2 >= 64 && _byte2 <= 254) //判断是否是GBK编码
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static  bool IsBig5Code(string word)
        {
            byte[] _bytes = Encoding.GetEncoding("Big5").GetBytes(word);

            if (_bytes.Length <= 1) // if there is only one byte, it is ASCII code
            {
                return false;
            }
            else
            {
                byte _byte1 = _bytes[0];
                byte _byte2 = _bytes[1];
                
                if ((_byte1 >= 129 && _byte1 <= 254) && ((_byte2 >= 64 && _byte2 <= 126) || (_byte2 >= 161 && _byte2 <= 254))) //判断是否是Big5编码
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion
    }
}