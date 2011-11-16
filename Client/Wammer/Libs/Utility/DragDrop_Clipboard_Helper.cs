
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Waveface.Compoment;
using Waveface.DragDropLib;

namespace Waveface
{
    class DragDrop_Clipboard_Helper
    {
        public void Drag_Enter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) ||
                e.Data.GetDataPresent("HTML Format") ||
                e.Data.GetDataPresent(DataFormats.Text) ||
                e.Data.GetDataPresent("UniformResourceLocator")
                )
            {
                e.Effect = DragDropEffects.Copy;

                try
                {
                    Point _p = Cursor.Position;
                    Win32Point _wp;
                    _wp.x = _p.X;
                    _wp.y = _p.Y;
                    IDropTargetHelper _dropHelper = (IDropTargetHelper)new DragDropHelper();
                    _dropHelper.DragEnter(IntPtr.Zero, (System.Runtime.InteropServices.ComTypes.IDataObject)e.Data, ref _wp, (int)e.Effect);
                }
                catch
                {
                }
            }
            else
            {
                e.Effect = DragDropEffects.None; // Unknown data, ignore it
            }

            FlashWindow.Start(MainForm.THIS);
        }

        public void Drag_Over(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) ||
                e.Data.GetDataPresent("HTML Format") ||
                e.Data.GetDataPresent(DataFormats.Text) ||
                e.Data.GetDataPresent("UniformResourceLocator")
                )
            {
                e.Effect = DragDropEffects.Copy;

                try
                {
                    Point _p = Cursor.Position;
                    Win32Point _wp;
                    _wp.x = _p.X;
                    _wp.y = _p.Y;
                    IDropTargetHelper _dropHelper = (IDropTargetHelper)new DragDropHelper();
                    _dropHelper.DragOver(ref _wp, (int)e.Effect);
                }
                catch
                {
                }
            }
            else
            {
                e.Effect = DragDropEffects.None; // Unknown data, ignore it
            }
        }

        public void Drag_Drop(DragEventArgs e)
        {
            FlashWindow.Stop(MainForm.THIS);

            try
            {
                Point _p = Cursor.Position;
                Win32Point _wp;
                _wp.x = _p.X;
                _wp.y = _p.Y;
                IDropTargetHelper _dropHelper = (IDropTargetHelper)new DragDropHelper();
                _dropHelper.Drop((System.Runtime.InteropServices.ComTypes.IDataObject)e.Data, ref _wp, (int)e.Effect);
            }
            catch
            {
            }

            DoDataPaste(e.Data);
        }

        public void Drag_Leave()
        {
            FlashWindow.Stop(MainForm.THIS);

            IDropTargetHelper _dropHelper = (IDropTargetHelper)new DragDropHelper();
            _dropHelper.DragLeave();
        }

        public void Clipboard_Paste()
        {
            DoDataPaste(Clipboard.GetDataObject());
        }

        private void DoDataPaste(IDataObject data)
        {
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    List<string> _pics = new List<string>();
                    List<string> _imageFileInDir = new List<string>();

                    string[] _dropFileList = (string[])data.GetData(DataFormats.FileDrop, false);

                    // 檢查是否為目錄, 並取出圖檔
                    if (_dropFileList.Length == 1)
                    {
                        if (Directory.Exists(_dropFileList[0]))
                        {
                            DirectoryInfo _d = new DirectoryInfo(_dropFileList[0]);

                            FileInfo[] _fileInfos = _d.GetFiles();

                            foreach (FileInfo _f in _fileInfos)
                            {
                                FileAttributes _attributes = File.GetAttributes(_f.FullName);

                                // 過濾隱藏檔
                                if ((_attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                    continue;

                                string _mime = FileUtility.GetMimeType(_f).ToLower();

                                if (_mime.IndexOf("image") >= 0)
                                    _imageFileInDir.Add(_f.FullName);
                            }

                            if (_imageFileInDir.Count > 0)
                            {
                                MainForm.THIS.Post(_imageFileInDir, PostType.Photo);
                                return;
                            }
                        }
                    }

                    foreach (string _file in _dropFileList)
                    {
                        string _mime = FileUtility.GetMimeType(new FileInfo(_file)).ToLower();

                        if (_mime.IndexOf("image") >= 0)
                            _pics.Add(_file);
                    }

                    if (_pics.Count > 0)
                    {
                        MainForm.THIS.Post(_pics, PostType.Photo);
                    }
                }
                catch
                {
                }

                return;
            }

            // Drag Image
            if (data.GetDataPresent("HTML Format"))
            {
                try
                {
                    string _clipboardHtml = (string)data.GetData("HTML Format");
                    string _htmlFragment = getHtmlFragment(_clipboardHtml);
                    string _imageSrc = parseImageSrc(_htmlFragment);
                    string _baseUrl = parseBaseURL(_clipboardHtml);
                    string _imgURL;

                    if (_imageSrc.ToLower().IndexOf("http://") == 0)
                        _imgURL = _imageSrc;
                    else
                        _imgURL = _baseUrl + _imageSrc.Substring(1);

                    Image _image = HttpHelp.DownloadImage(_imgURL);

                    string _imgLocalPath = MainForm.GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

                    _image.Save(_imgLocalPath);

                    MainForm.THIS.Post(new List<string>
                                        {
                                            _imgLocalPath
                                        }, PostType.Photo);

                    return;
                }
                catch
                {
                }
            }

            // Drag Link
            if (data.GetDataPresent("HTML Format"))
            {
                try
                {
                    /*
                    string _clipboardHtml = (string)data.GetData("HTML Format");

                    Regex r = new Regex("SourceURL:.*");
                    Match m = r.Match(_clipboardHtml);

                    if (m.Success)
                    {
                        string workStr = m.Value;
                        string _url = workStr.Substring(10);

                        MessageBox.Show(_url);
                    }
                    */

                    //Replace white space chars
                    string _str = (string)data.GetData("Text");
                    string _title = GetTrimmedLine(_str);

                    _str = (string)data.GetData("HTML Format");
                    string _workStr = _str;

                    Regex _r = new Regex("SourceURL:.*");
                    Match _m = _r.Match(_str);

                    if (_m.Success)
                    {
                        _workStr = _m.Value;
                        string url = _workStr.Substring(10);

                        if (_title == string.Empty)
                            _title = url;
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
                            string rtfContent = _str.Substring(_sf, _ef - _sf);
                        }
                    }

                    return;
                }
                catch
                {
                }
            }

            if (data.GetDataPresent(DataFormats.Text))
            {
                string _text = data.GetData(DataFormats.StringFormat).ToString();

                MessageBox.Show(_text);

                return;
            }
        }

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
            //if (_workStr.Length > 32)
            //    _retStr = _workStr.Substring(0, 32) + "...";
            //else
                _retStr = _workStr;

            return _retStr;
        }

        #region HTML Format

        string parseBaseURL(string html)
        {
            return Regex.Match(html, @"http://.*?/", RegexOptions.IgnoreCase).Groups[0].Value;
        }

        string parseImageSrc(string html)
        {
            return Regex.Match(html, @"<img.*?src=[""'](.*?)[""'].*>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value;
        }

        string getHtmlFragment(string clipboardHtml)
        {
            int fragStartPos = int.Parse(Regex.Match(clipboardHtml, @"^StartFragment:(\d+)", RegexOptions.Multiline).Groups[1].Value);
            int fragEndPos = int.Parse(Regex.Match(clipboardHtml, @"^EndFragment:(\d+)", RegexOptions.Multiline).Groups[1].Value);

            return clipboardHtml.Substring(fragStartPos, fragEndPos - fragStartPos);
        }

        #endregion
    }
}
