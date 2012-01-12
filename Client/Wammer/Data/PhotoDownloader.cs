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

        private static PhotoDownloader s_current;

        public List<ImageItem> ThumbnailItems { get; set; }
        public List<ImageItem> PhotoItems { get; set; }

        private int ERROR_TRY = 2;
        private string m_currentURL;

        public static PhotoDownloader Current
        {
            get
            {
                if (s_current == null)
                {
                    s_current = new PhotoDownloader();
                }

                return s_current;
            }
            set { s_current = value; }
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
                        _localPath = _item.LocalFilePath_Origin;
                        break;

                    case PostItemType.Origin:
                        _url = _item.OriginPath;
                        _localPath = _item.LocalFilePath_Origin;
                        break;

                    case PostItemType.Medium:
                        _url = _item.MediumPath;
                        _localPath = _item.LocalFilePath_Medium;
                        break;
                }

                _relpaceOriginToMedium = false;

                if (_item.PostItemType == PostItemType.Origin)
                {
                    if (File.Exists(_item.LocalFilePath_Medium))
                    {
                        if (Main.Current.RT.StationMode)
                        {
                            _url = _item.OriginPath;
                        }
                        else
                        {
                            _url = _item.CloudOriginPath;
                        }
                    }
                    else
                    {
                        _url = _item.MediumPath;
                        _localPath = _item.LocalFilePath_Medium;
                        _relpaceOriginToMedium = true;
                    }
                }

                try
                {
                    m_currentURL = _url;

                    WebRequest _wReq = WebRequest.Create(_url);
                    _wReq.Timeout = 10000;

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
                    NLogUtility.Exception_Warn(s_logger, _e, "Download Image URL", m_currentURL);

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
                }

                Thread.Sleep(10);
            }
        }
    }
}