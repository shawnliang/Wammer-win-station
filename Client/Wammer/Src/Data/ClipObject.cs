#region

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#endregion

namespace Waveface
{
    //
    // 目前無用
    //
    public class ClipObject
    {
        private const int BUFFER_SIZE = 30;

        public byte[] audioData;

        //The actual data contained herein
        public string content = string.Empty;
        public string[] dataFormats = new string[BUFFER_SIZE];
        public bool[] formatStatus = new bool[BUFFER_SIZE];
        public Image image;

        public byte[][] rawByteBuffers = new byte[BUFFER_SIZE][];
        public string[] rawDataBuffers = new string[BUFFER_SIZE];
        public string rtfContent = string.Empty;
        public DateTime time = DateTime.Now;
        public string title = string.Empty;
        public string url = string.Empty;

        public ClipObject()
        {
        }

        public ClipObject(string txt, Image img)
        {
            title = txt;
            image = img;
        }

        public ClipObject(IDataObject data)
        {
            DataObject _dataObj = new DataObject(data);

            //Note_ The order of these is critical!
            if (ContainsFormat(_dataObj, "DeviceIndependentBitmap") ||
                ContainsFormat(_dataObj, "Bitmap") ||
                ContainsFormat(_dataObj, "System.Drawing.Bitmap"))
                ProcessDib(_dataObj);
            else if (ContainsFormat(_dataObj, "Rich Text Format"))
                ProcessRtf(_dataObj);
            else if (ContainsFormat(_dataObj, "HTML Format"))
                ProcessHtml(_dataObj);
            else if (ContainsFormat(_dataObj, "DragContext"))
                ProcessFileDrop(_dataObj);
            else if (ContainsFormat(_dataObj, "Text"))
                ProcessText(_dataObj);
            else if (ContainsFormat(_dataObj, "FileNameW"))
                ProcessFileName(_dataObj);
            else
                ProcessUnknown(_dataObj);
        }

        #region Processing methods

        private void ProcessRtf(DataObject data)
        {
            string _text = (string)data.GetData("Text");
            string _rtfContent = (string)data.GetData("Rich Text Format");

            title = GetTrimmedLine(_text);

            rtfContent = _rtfContent;
        }

        private void ProcessText(DataObject data)
        {
            content = (string)data.GetData("Text");

            title = GetTrimmedLine(content);
        }

        private void ProcessDib(DataObject data)
        {
            //TODO_ Can I get text of some kind to indicate where from?
            title = "Image (Unknown Source)";
        }

        private void ProcessHtml(DataObject data)
        {
            string _str = string.Empty;

            //Replace white space chars
            _str = (string)data.GetData("Text");
            title = GetTrimmedLine(_str);

            _str = (string)data.GetData("HTML Format");
            string _workStr = _str;

            Regex _r = new Regex("SourceURL:.*");
            Match _m = _r.Match(_str);

            if (_m.Success)
            {
                _workStr = _m.Value;
                url = _workStr.Substring(10);

                if (title == string.Empty)
                    title = url;
            }

            _r = new Regex("StartFragment:.*");
            _m = _r.Match(_str);

            if (_m.Success)
            {
                _workStr = _m.Value;
                int _sf = int.Parse(_workStr.Substring(14));
                _r = new Regex("EndFragment:.*");
                _m = _r.Match(_str);

                if (_m.Success)
                {
                    _workStr = _m.Value;
                    int _ef = int.Parse(_workStr.Substring(12));
                    rtfContent = _str.Substring(_sf, _ef - _sf);
                }
            }
        }

        private void ProcessFileName(DataObject data)
        {
            string[] _strs = (string[])data.GetData("FileNameW");

            if (_strs.Length > 0)
                content = _strs[0];
            else
                throw new InvalidDataException("ProcessWave: FileNameW length 0");

            //Is it a valid Wave Audio file?
            if (content.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                ProcessWave(data);
            }
            else
            {
                //Check if its a valid image file by checking extensioin.
                //  If its not that then I give up and just set to raw data format.
                try
                {
                    //TODO_ If you want to add an image type add to the list and surround with \b 
                    Regex _r = new Regex(@"(?i-)(\b\.bmp\b|\b\.png\b|\b\.jpg\b|\b\.jpeg\b|\b\.gif\b|\b\.ico\b)");
                    Match m = _r.Match(content);

                    if (m.Success)
                        ProcessImage(data);
                    else
                        ProcessOther(data);
                }
                catch
                {
                    throw new InvalidProgramException("Umm, I seem to have got lost");
                }
            }
        }

        private void ProcessWave(DataObject data)
        {
            int _index = content.LastIndexOf('\\');

            if (_index > 0)
                title = content.Substring(++_index);
            else
                throw new InvalidDataException("ProcessWave: fileName from FileNameW problem");
        }

        private void ProcessImage(DataObject data)
        {
            image = Image.FromFile(content);

            int index = content.LastIndexOf('\\');
            title = content.Substring(++index);
        }

        private void ProcessOther(DataObject data)
        {
            int _index = content.LastIndexOf('\\');
            title = content.Substring(++_index);
        }

        private void ProcessFileDrop(DataObject data)
        {
            string[] _strs = (string[])data.GetData("FileDrop");

            title = "File Drop";

            StringBuilder _sb = new StringBuilder("File List contains: " + _strs.Length.ToString() + " file(s)\n\n");

            foreach (string _s in _strs)
                _sb.Append(_s + "\n");

            content = _sb.ToString();
        }

        private void ProcessUnknown(DataObject data)
        {
            title = "Not a clue?";
        }

        // Retreive the raw data from the DataObject
        public void ProcessRawData(DataObject data)
        {
            StringBuilder _sb = new StringBuilder();
            string[] _data = new string[BUFFER_SIZE];
            dataFormats = data.GetFormats(true);
            int _index = 0;
            bool _status = true;

            try
            {
                foreach (string _s in dataFormats)
                {
                    object _d = data.GetData(_s, true);

                    if (_d == null)
                    {
                        rawDataBuffers[_index] = "ERROR - Processing data!\n" +
                                                 "This is one I'm not sure of!";
                        _status = false;
                    }
                    else
                    {
                        switch (_d.GetType().ToString())
                        {
                            case "System.String[]":
                                _data = (string[])data.GetData(_s, true);

                                foreach (string str in _data)
                                    _sb.Append(str + "\n\n");

                                rawDataBuffers[_index] = _sb.ToString();
                                _sb.Remove(0, _sb.Length);
                                break;

                            case "System.IO.MemoryStream":
                                MemoryStream ms = (MemoryStream)data.GetData(_s, true);
                                rawByteBuffers[_index] = ms.ToArray();
                                break;

                            case "System.String":
                                rawDataBuffers[_index] = (string)data.GetData(_s, true);
                                break;

                            case "System.Drawing.Bitmap":
                                image = (Bitmap)data.GetData(DataFormats.Bitmap, true);
                                break;

                            default:
                                rawByteBuffers[_index] = (byte[])_d;
                                audioData = (byte[])_d;
                                break;
                        }
                    }

                    formatStatus[_index] = _status;
                    _status = true;
                    ++_index;
                }
            }
            catch
            {
                throw new InvalidDataException();
            }
        }

        #endregion

        #region Helper methods

        // Check DataFormats for the format passd
        private bool ContainsFormat(DataObject data, string format)
        {
            bool _res = false;
            string[] _strs = data.GetFormats(true);

            foreach (string _s in _strs)
            {
                if (_s == format)
                {
                    _res = true;
                    break;
                }
            }

            return _res;
        }

        // Given a selection return the first line, trimmed!
        private string GetTrimmedLine(string str)
        {
            if (str == null)
                return string.Empty;

            string _retStr = str;
            string _workStr = string.Empty;

            //Trim off beginning spaces, CR, LF
            _workStr = str.TrimStart(' ', '\r', '\n');

            //Gets a single line
            Regex _r = new Regex("(?m)(^.*$)");
            Match _m = null;
            _m = _r.Match(_workStr);

            //Trim end of string as needed
            if (_m.Success)
                _workStr = _m.Value.Trim();

            //Now get it to the required length
            if (_workStr.Length > 32)
                _retStr = _workStr.Substring(0, 32) + "...";
            else
                _retStr = _workStr;

            return _retStr;
        }

        #endregion
    }
}