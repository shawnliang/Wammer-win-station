#region

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using HTML2XHTML.App_GlobalResources;

#endregion

namespace Waveface.Util
{
    public class XMLUtil
    {
        // Serialize a instance to xml file using default coding
        public static int WriteXML(String xmlFileName, Object oData, bool useNullNameSpace)
        {
            XmlSerializer _xs = new XmlSerializer(oData.GetType());
            XmlTextWriter _writer = null;
           
            try
            {
                _writer = new XmlTextWriter(xmlFileName, Encoding.GetEncoding(0));
                _writer.Formatting = Formatting.Indented;
               
                if (useNullNameSpace)
                {
                    XmlSerializerNamespaces _ns = new XmlSerializerNamespaces();
                    _ns.Add("", "");
                    _xs.Serialize(_writer, oData, _ns);
                }
                else
                {
                    _xs.Serialize(_writer, oData);
                }

                _writer.Close();
            }
            finally
            {
                if (_writer != null)
                {
                    _writer.Close();
                }
            }

            return 0;
        }

        // get the xml content of the instance without any xsl information and use empty namespace
        public static String GetXML(Object oData)
        {
            return GetXML(oData, null, true);
        }

        public static String GetShortXML(Object oData, String strType)
        {
            XmlRootAttribute _xrAttr = new XmlRootAttribute(strType);
            XmlSerializer _xs = new XmlSerializer(oData.GetType(), _xrAttr);
            XmlTextWriter _writer = null;
            MemoryStream _msStream = null;
            String _strResult = null;

            try
            {
                _msStream = new MemoryStream();
                _writer = new XmlTextWriter(_msStream, Encoding.GetEncoding(0));
                _writer.Formatting = Formatting.None;
                _writer.WriteRaw(null);

                XmlSerializerNamespaces _ns = new XmlSerializerNamespaces();
                _ns.Add("", "");
                _xs.Serialize(_writer, oData, _ns);
                _writer.Close();
                _strResult = Encoding.Default.GetString(_msStream.ToArray());
            }
            finally
            {
                if (_msStream != null)
                {
                    _msStream.Close();
                }

                if (_writer != null)
                {
                    _writer.Close();
                }
            }

            return _strResult;
        }

        // get the corresponding xml content of the object
        public static String GetXML(Object oData, String xslURL, bool useNullNameSpace)
        {
            XmlSerializer _xs = new XmlSerializer(oData.GetType());
            XmlTextWriter _writer = null;
            MemoryStream _msStream = null;
            String _strResult = null;
           
            try
            {
                _msStream = new MemoryStream();
                Encoding defaultEnc = Encoding.GetEncoding(0);
                _writer = new XmlTextWriter(_msStream, defaultEnc);
                _writer.Formatting = Formatting.Indented;
                
                if (xslURL != null)
                {
                    _writer.WriteRaw("<?xml version=\"1.0\" encoding=\"" + defaultEnc.BodyName + "\" ?>\r\n");
                    _writer.WriteRaw(String.Format("<?xml:stylesheet type=\"text/xsl\" href=\"{0}\"?>\r\n", xslURL));
                }
                if (useNullNameSpace)
                {
                    XmlSerializerNamespaces _ns = new XmlSerializerNamespaces();
                    _ns.Add("", "");
                    _xs.Serialize(_writer, oData, _ns);
                }
                else
                {
                    _xs.Serialize(_writer, oData);
                }

                _writer.Close();
                _strResult = Encoding.Default.GetString(_msStream.ToArray());
            }
            finally
            {
                if (_msStream != null)
                {
                    _msStream.Close();
                }
                if (_writer != null)
                {
                    _writer.Close();
                }
            }

            return _strResult;
        }

        // deserialize a xml file into a object
        public static T ReadXML<T>(String xmlFilename)
        {
            return (T) ReadXML(xmlFilename, typeof (T));
        }

        // deserialize a xml file to a instance
        public static Object ReadXML(String xmlFilename, Type dataType)
        {
            Object _objXml = null;
            XmlSerializer _xs = new XmlSerializer(dataType);
            XmlTextReader _reader = null;
            
            try
            {
                _reader = new XmlTextReader(xmlFilename);
                _objXml = _xs.Deserialize(_reader);
            }
            finally
            {
                if (_reader != null)
                {
                    _reader.Close();
                }
            }

            return _objXml;
        }

        // deserialize a XML to an object from a string
        public static Object LoadXML<T>(String s)
        {
            return LoadXML(s, typeof (T));
        }

        // deserialize a string to a instance
        public static Object LoadXML(String s, Type dataType)
        {
            byte[] _bData = Encoding.GetEncoding(0).GetBytes(s);
            return LoadXML(_bData, dataType);
        }

        // deserialize a byte array to a instance
        public static Object LoadXML(byte[] bRawData, Type dataType)
        {
            Object _objXml = null;
            XmlSerializer _xs = new XmlSerializer(dataType);
            XmlTextReader _reader = null;
            MemoryStream _msStream = null;
            
            try
            {
                _msStream = new MemoryStream();
                _msStream.Write(bRawData, 0, bRawData.Length);
                _msStream.Seek(0, SeekOrigin.Begin);
                _reader = new XmlTextReader(_msStream);
                _objXml = _xs.Deserialize(_reader);
            }
            finally
            {
                if (_msStream != null)
                {
                    _msStream.Close();
                }

                if (_reader != null)
                {
                    _reader.Close();
                }
            }

            return _objXml;
        }

        // This methond convert a html file to an xhtml file
        public static String HTML2XHTML(String strOriginalContent)
        {
            return HTML2XHTML(strOriginalContent, null);
        }

        // This methond convert a html file to an xhtml file
        public static String HTML2XHTML(String strOriginalContent, String strOutputPath)
        {
            String _strTempPath = strOutputPath ?? Path.GetTempPath();
            String _strFileName = String.Format("{0}tidy.exe", _strTempPath);
            
            //check wether tidy execuble exists
            if (!File.Exists(_strFileName))
            {
                SysUtil.WriteFile(_strFileName, Resource.tidy);
            }

            //Create process
            ProcessStartInfo _psiInfo = new ProcessStartInfo();
            _psiInfo.FileName = _strFileName;
            _psiInfo.CreateNoWindow = true;
            _psiInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _psiInfo.WorkingDirectory = _strTempPath;

            String _strMainFileName = Guid.NewGuid().ToString("N");

            //Specify the in/out/error file name,which is located in the temporary path
            String _strInFileName = String.Format("{0}{1}.in", _strTempPath, _strMainFileName);
            String _strOutFileName = String.Format("{0}{1}.out", _strTempPath, _strMainFileName);
            String _strErrorFileName = String.Format("{0}{1}.log", _strTempPath, _strMainFileName);
            File.Delete(_strInFileName);
           
            //UTF8 Version,and we suppose the original content is encoded though the default encoding of the system
            byte[] _baUtf8Data = Encoding.Convert(Encoding.GetEncoding(0), Encoding.UTF8,
                                                 Encoding.GetEncoding(0).GetBytes(strOriginalContent));
            SysUtil.WriteFile(_strInFileName, _baUtf8Data);

            //UTF8 Version
            _psiInfo.Arguments = String.Format(" -raw -utf8 -asxhtml -i -f {0}.log -o {0}.out {0}.in", _strMainFileName);
            File.Delete(_strOutFileName);
            Process _proc = Process.Start(_psiInfo);
            _proc.WaitForExit();
            File.Delete(_strInFileName);
            File.Delete(_strErrorFileName);

            byte[] _baResult = SysUtil.ReadFileAsBytes(_strOutFileName);

            //We need a head for xhtml processing
            String _strContent = Encoding.GetEncoding(0).GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(0), _baResult));
            _strContent =
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
                _strContent;

            File.Delete(_strOutFileName);
            return _strContent;
        }
    }
}