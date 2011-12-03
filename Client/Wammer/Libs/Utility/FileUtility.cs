
namespace Waveface
{
    using System.IO;
    using Microsoft.Win32;

    public class FileUtility
    {
        public static string GetMimeType(FileInfo fileInfo)
        {
            string _mimeType = "application/unknown";

            RegistryKey _regKey = Registry.ClassesRoot.OpenSubKey(
                fileInfo.Extension.ToLower()
            );

            if (_regKey != null)
            {
                object _contentType = _regKey.GetValue("Content Type");

                if (_contentType != null)
                    _mimeType = _contentType.ToString();
            }

            return _mimeType;
        }

        // Reads data from a stream until the end is reached. The
        // data is returned as a byte array. An IOException is
        // thrown if any of the underlying IO calls fail.
        public static byte[] ReadFully(Stream stream)
        {
            byte[] _buffer = new byte[32768];

            using (MemoryStream _ms = new MemoryStream())
            {
                while (true)
                {
                    int _read = stream.Read(_buffer, 0, _buffer.Length);
                    
                    if (_read <= 0)
                        return _ms.ToArray();

                    _ms.Write(_buffer, 0, _read);
                }
            }
        }

        public static byte[] ConvertFileToByteArray(string fileName)
        {
            byte[] _ret;

            using (FileStream _fr = new FileStream(fileName, FileMode.Open))
            {
                _ret = ReadFully(_fr);
                
                //對, 但有問題的方式 
                //
                //using (BinaryReader _br = new BinaryReader(_fr))
                //{
                //    _ret = _br.ReadBytes((int)_fr.Length);
                //}
            }

            return _ret;
        }
    }
}
