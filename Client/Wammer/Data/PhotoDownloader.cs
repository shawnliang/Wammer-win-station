#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using NLog;

#endregion

namespace Waveface
{
    public class PhotoDownloader
    {
        public delegate void Thumbnail_Delegate(ImageItem item);
        public delegate void Photo_Delegate(ImageItem item);

        public event Thumbnail_Delegate ThumbnailEvent;
        public event Photo_Delegate PhotoEvent;

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private int ERROR_TRY = 2;

        private static PhotoDownloader s_photoDownloader;

        public List<ImageItem> ThumbnailItems { get; set; }
        public List<ImageItem> PhotoItems { get; set; }

        public static PhotoDownloader Current
        {
            get
            {
                if (s_photoDownloader == null)
                {
                    s_photoDownloader = new PhotoDownloader(); // Load() ?? new PhotoDownloader();
                }

                return s_photoDownloader;
            }
        }

        public PhotoDownloader()
        {
            ThumbnailItems = new List<ImageItem>();
            PhotoItems = new List<ImageItem>();

            ThreadPool.QueueUserWorkItem(state => { DownloadThreadMethod(); });
        }

        public void Add(ImageItem item)
        {
            if (item.PostItemType == PostItemType.Thumbnail)
            {
                lock (ThumbnailItems)
                {
                    ImageItem _tmp = null;

                    foreach (ImageItem _item in ThumbnailItems)
                    {
                        if (_item.ThumbnailPath == item.ThumbnailPath)
                        {
                            _tmp = _item;
                            break;
                        }
                    }

                    if (_tmp != null)
                    {
                        ThumbnailItems.Remove(_tmp);
                    }

                    ThumbnailItems.Insert(0, item);
                }
            }
            else
            {
                lock (PhotoItems)
                {
                    ImageItem _tmp = null;

                    foreach (ImageItem _item in PhotoItems)
                    {
                        if (_item.OriginPath == item.OriginPath)
                        {
                            _tmp = _item;
                            break;
                        }
                    }

                    if (_tmp != null)
                    {
                        PhotoItems.Remove(_tmp);
                    }

                    PhotoItems.Insert(0, item);
                }
            }

            // Save();
        }

        private void DownloadThreadMethod()
        {
            long _count = 0;

            ImageItem _item;
            string _url;
            string _localPath;
            bool _relpaceOriginToMedium;

            while (true)
            {
                _item = null;

                if ((ThumbnailItems.Count == 0) && (PhotoItems.Count == 0))
                {
                    Thread.Sleep(1000);
                    continue;
                }

                if ((_count++ % 3) != 2)
                {
                    if (ThumbnailItems.Count > 0)
                    {
                        _item = ThumbnailItems[0];
                    }
                }
                else
                {
                    if (PhotoItems.Count > 0)
                    {
                        _item = PhotoItems[0];
                    }
                }

                if (_item == null)
                {
                    Thread.Sleep(1);
                    continue;
                }

                _url = string.Empty;
                _localPath = string.Empty;

                switch (_item.PostItemType)
                {
                    case PostItemType.Thumbnail:
                        _url = _item.ThumbnailPath;
                        _localPath = _item.LocalFilePath;
                        break;

                    case PostItemType.Origin:
                        _url = _item.OriginPath;
                        _localPath = _item.LocalFilePath;
                        break;

                    case PostItemType.Medium:
                        _url = _item.MediumPath;
                        _localPath = _item.LocalFilePath2;
                        break;
                }

                _relpaceOriginToMedium = false;

                if (_item.PostItemType == PostItemType.Origin)
                {
                    if (File.Exists(_item.LocalFilePath2))
                    {
                        if(!Main.Current.RT.StationMode)
                        {
                            lock (PhotoItems)
                            {
                                if (PhotoItems.Contains(_item))
                                {
                                    PhotoItems.Remove(_item);
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        _url = _item.MediumPath;
                        _localPath = _item.LocalFilePath2;
                        _relpaceOriginToMedium = true;
                    }
                }

                try
                {
                    WebRequest _wReq = WebRequest.Create(_url);
                    _wReq.Timeout = 3000;

                    WebResponse _wRep = _wReq.GetResponse();

                    Image _img = Image.FromStream(_wRep.GetResponseStream());

                    if (!File.Exists(_localPath))
                        _img.Save(_localPath);

                    _img = null;

                    s_logger.Trace("GetFile:" + _localPath);

                    if (_item.PostItemType == PostItemType.Thumbnail)
                    {
                        lock (ThumbnailItems)
                        {
                            if (ThumbnailItems.Contains(_item))
                                ThumbnailItems.Remove(_item);
                        }

                        if (ThumbnailEvent != null)
                            ThumbnailEvent(_item);
                    }
                    else
                    {
                        lock (PhotoItems)
                        {
                            if (_relpaceOriginToMedium)
                            {
                                PhotoItems.Remove(_item);

                                if (PhotoItems.Count == 0)
                                    PhotoItems.Insert(0, _item);
                                else
                                    PhotoItems.Insert(PhotoItems.Count - 1, _item);
                            }
                            else
                            {
                                if (PhotoItems.Contains(_item))
                                    PhotoItems.Remove(_item);
                            }

                        }

                        if (PhotoEvent != null)
                            PhotoEvent(_item);
                    }
                }
                catch (Exception _e)
                {
                    _item.ErrorTry++;

                    if (_item.PostItemType == PostItemType.Thumbnail)
                    {
                        lock (ThumbnailItems)
                        {
                            if (ThumbnailItems.Contains(_item))
                            {
                                ThumbnailItems.Remove(_item);

                                if (_item.ErrorTry != ERROR_TRY)
                                {
                                    if (ThumbnailItems.Count == 0)
                                        ThumbnailItems.Insert(0, _item);
                                    else
                                        ThumbnailItems.Insert(ThumbnailItems.Count - 1, _item);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (PhotoItems)
                        {
                            if (PhotoItems.Contains(_item))
                            {
                                PhotoItems.Remove(_item);

                                if (_item.ErrorTry != ERROR_TRY)
                                {
                                    if (PhotoItems.Count == 0)
                                        PhotoItems.Insert(0, _item);
                                    else
                                        PhotoItems.Insert(PhotoItems.Count - 1, _item);
                                }
                            }
                        }
                    }

                    NLogUtility.Exception(s_logger, _e, "DownloadThreadMethod");
                }

                Thread.Sleep(1);
            }
        }

        #region IO

        /*
        public bool Save()
        {
            try
            {
                string _json = JsonConvert.SerializeObject(this);

                string _filePath = Main.GCONST.CachePath + Main.Current.RT.Login.user.user_id + "_NP.dat";

                using (StreamWriter _outfile = new StreamWriter(_filePath))
                {
                    _outfile.Write(_json);
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "Save");

                return false;
            }

            s_logger.Trace("Save: OK");

            return true;
        }

        public static PhotoDownloader Load()
        {
            try
            {
                string _json = string.Empty;
                string _filePath = Main.GCONST.CachePath + Main.Current.RT.Login.user.user_id + "_PD.dat";

                StreamReader _sr = File.OpenText(_filePath);
                _json = _sr.ReadToEnd();
                _sr.Close();

                PhotoDownloader _npm = JsonConvert.DeserializeObject<PhotoDownloader>(_json);

                s_logger.Trace("Load:OK");

                return _npm;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "Load");
            }

            return null;
        }

        */
        #endregion
    }
}