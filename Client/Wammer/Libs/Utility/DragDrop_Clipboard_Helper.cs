
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Waveface.Component;
using Waveface.DragDropLib;

namespace Waveface
{
    class DragDrop_Clipboard_Helper
    {
        private bool m_createNewPost = true;

        public DragDrop_Clipboard_Helper()
        {
        }

        public DragDrop_Clipboard_Helper(bool createNewPost)
        {
            m_createNewPost = createNewPost;
        }

        #region All

        public void Drag_Enter(DragEventArgs e, bool forceDisable)
        {
            if (forceDisable)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

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

            FlashWindow.Start(Main.Current);
        }

        public void Drag_Over(DragEventArgs e, bool forceDisable)
        {
            if (forceDisable)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

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

        public List<string> Drag_Drop(DragEventArgs e)
        {
            FlashWindow.Stop(Main.Current);

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

            return DoDataPaste(e.Data);
        }

        public void Drag_Leave()
        {
            FlashWindow.Stop(Main.Current);

            IDropTargetHelper _dropHelper = (IDropTargetHelper)new DragDropHelper();
            _dropHelper.DragLeave();
        }

        private List<string> DoDataPaste(IDataObject data)
        {
            string _dirText = string.Empty;

            List<string> _pics = new List<string>();

            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    string[] _dropFiles = (string[])data.GetData(DataFormats.FileDrop, false);


                    foreach (string _file in _dropFiles)
                    {
                        if (Directory.Exists(_file))
                        {
                            DirectoryInfo _d = new DirectoryInfo(_file);

                            _dirText += _d.Name + Environment.NewLine;

                            FileInfo[] _fileInfos = _d.GetFiles();

                            foreach (FileInfo _f in _fileInfos)
                            {
                                FileAttributes _a = File.GetAttributes(_f.FullName);

                                // 過濾隱藏檔
                                if ((_a & FileAttributes.Hidden) == FileAttributes.Hidden)
                                    continue;

                                string _mime = FileUtility.GetMimeType(_f).ToLower();

                                if (_mime.IndexOf("image") >= 0)
                                    _pics.Add(_f.FullName);
                            }
                        }
                        else
                        {
                            string _mime = FileUtility.GetMimeType(new FileInfo(_file)).ToLower();

                            if (_mime.IndexOf("image") >= 0)
                                _pics.Add(_file);
                        }
                    }

                    if (_pics.Count > 0)
                    {
                        if (m_createNewPost)
                            Main.Current.Post(_pics, PostType.Photo, _dirText);
                    }
                }
                catch
                {
                }

                return _pics;
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

                    if ((_imageSrc.ToLower().IndexOf("http://") == 0) || (_imageSrc.ToLower().IndexOf("https://") == 0))
                        _imgURL = _imageSrc;
                    else
                        _imgURL = _baseUrl + _imageSrc.Substring(1);

                    if (!NetworkInterface.GetIsNetworkAvailable())
                        return null;

                    Main.Current.Cursor = Cursors.WaitCursor;

                    Image _image = HttpHelp.DownloadImage(_imgURL);

                    string _imgLocalPath = Main.GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

                    _image.Save(_imgLocalPath);

                    Main.Current.Cursor = Cursors.Default;

                    List<string> _hPics = new List<string>
                                             {
                                                 _imgLocalPath
                                             };

                    if (m_createNewPost)
                    {
                        Main.Current.Post(_hPics, PostType.Photo, "");
                    }
                    else
                    {
                        return _hPics;
                    }

                    return null;
                }
                catch (Exception _e)
                {
                    Main.Current.Cursor = Cursors.Default;
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

                    return null;
                }
                catch
                {
                }
            }

            if (data.GetDataPresent(DataFormats.Text))
            {
                string _text = data.GetData(DataFormats.StringFormat).ToString();

                // MessageBox.Show(_text);

                return null;
            }

            return null;
        }

        #endregion

        #region HTML Image

        public void Drag_Enter_HtmlImage(DragEventArgs e, bool forceDisable)
        {
            if (forceDisable)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent("HTML Format"))
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
                e.Effect = DragDropEffects.None;
            }

            FlashWindow.Start(Main.Current);
        }

        public void Drag_Over_HtmlImage(DragEventArgs e, bool forceDisable)
        {
            if (forceDisable)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent("HTML Format"))
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
                e.Effect = DragDropEffects.None;
            }
        }

        public List<string> Drag_Drop_HtmlImage(DragEventArgs e)
        {
            FlashWindow.Stop(Main.Current);

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

            return DoDataPaste_HtmlImage(e.Data);
        }

        public void Drag_Leave_HtmlImage()
        {
            FlashWindow.Stop(Main.Current);

            IDropTargetHelper _dropHelper = (IDropTargetHelper)new DragDropHelper();
            _dropHelper.DragLeave();
        }

        private List<string> DoDataPaste_HtmlImage(IDataObject data)
        {
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

                    if ((_imageSrc.ToLower().IndexOf("http://") == 0) || (_imageSrc.ToLower().IndexOf("https://") == 0))
                        _imgURL = _imageSrc;
                    else
                        _imgURL = _baseUrl + _imageSrc.Substring(1);

                    if (!NetworkInterface.GetIsNetworkAvailable())
                        return null;

                    Main.Current.Cursor = Cursors.WaitCursor;

                    Image _image = HttpHelp.DownloadImage(_imgURL);

                    string _imgLocalPath = Main.GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

                    _image.Save(_imgLocalPath);

                    Main.Current.Cursor = Cursors.Default;

                    List<string> _hPics = new List<string>
                                             {
                                                 _imgLocalPath
                                             };

                    if (m_createNewPost)
                    {
                        Main.Current.Post(_hPics, PostType.Photo, "");
                    }
                    else
                    {
                        return _hPics;
                    }
                }
                catch (Exception _e)
                {
                    Main.Current.Cursor = Cursors.Default;
                }
            }

            return null;
        }

        #endregion

        #region Misc

        /*
        public void Clipboard_Paste()
        {
            DoDataPaste(Clipboard.GetDataObject());
        }
        */

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

        #endregion

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
