﻿#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using NLog;
using Waveface.API.V2;
using Waveface.DetailUI;

#endregion

namespace Waveface
{
    public class PhotoDownloader
    {
        #region Delegates

        public delegate void Photo_Delegate(ImageItem item);

        public delegate void Thumbnail_Delegate(ImageItem item);

        #endregion

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private int ERROR_TRY = 1;
        private string m_currentURL;
        private Dictionary<string, DateTime> m_downlaodErrorOriginFiles;

        private WorkItem m_thumbnailWorkItem;
        private WorkItem m_photoItemsWorkItem2;

        public PhotoDownloader()
        {
            ThumbnailItems = new List<ImageItem>();
            PhotoItems = new List<ImageItem>();

            m_downlaodErrorOriginFiles = new Dictionary<string, DateTime>();
        }

        public List<ImageItem> ThumbnailItems { get; set; }
        public List<ImageItem> PhotoItems { get; set; }
        public event Thumbnail_Delegate ThumbnailEvent;
        public event Photo_Delegate PhotoEvent;

        public void Start()
        {
            m_thumbnailWorkItem = AbortableThreadPool.QueueUserWorkItem(DownloadThreadMethod, 0);
            m_photoItemsWorkItem2 = AbortableThreadPool.QueueUserWorkItem(DownloadThreadMethod, 1);
        }

        public WorkItemStatus AbortThread()
        {
            AbortableThreadPool.Cancel(m_thumbnailWorkItem, true);
            return AbortableThreadPool.Cancel(m_photoItemsWorkItem2, true);
        }

        public void Add(ImageItem item, bool forceRetry)
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
                        if (_item.PostItemType == PostItemType.Origin)
                        {
                            if (_item.LocalFilePath_Origin == item.LocalFilePath_Origin)
                            {
                                _tmp = _item;
                                break;
                            }
                        }

                        if (_item.PostItemType == PostItemType.Medium)
                        {
                            if (_item.LocalFilePath_Medium == item.LocalFilePath_Medium)
                            {
                                _tmp = _item;
                                break;
                            }
                        }
                    }

                    if (_tmp != null)
                    {
                        PhotoItems.Remove(_tmp);
                    }

                    PhotoItems.Insert(0, item);
                }
            }
        }

        public void RemoveAll()
        {
            lock (ThumbnailItems)
            {
                ThumbnailItems.Clear();
            }

            lock (PhotoItems)
            {
                PhotoItems.Clear();
            }

            s_logger.Trace("Remove All Cache Item.");
        }

        private void DownloadThreadMethod(object state)
        {
            long _count = 0;
            string _localPath = string.Empty;
            string _url = string.Empty;

            Thread.Sleep(3000);

            while (true)
            {
                if (((int)state == 0 && ThumbnailItems.Count == 0) || ((int)state == 1 && PhotoItems.Count == 0))
                {
                    Thread.Sleep(1000);
                    continue;
                }

                ImageItem _item = null;

                Thread.Sleep(50);

                if ((int)state == 0)
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
                    continue;
                }

                switch (_item.PostItemType)
                {
                    case PostItemType.Thumbnail:
                        _url = _item.ThumbnailPath.Replace("[IP]", WService.HostIP);
                        _localPath = _item.LocalFilePath_Origin;
                        break;

                    case PostItemType.Origin:
                        _url = Main.Current.RT.StationMode ? _item.OriginPath : _item.CloudOriginPath;
                        _localPath = _item.LocalFilePath_Origin;
                        break;

                    case PostItemType.Medium:
                        _url = _item.MediumPath.Replace("[IP]", WService.HostIP);
                        _localPath = _item.LocalFilePath_Medium;
                        break;
                }

                if (File.Exists(_localPath))
                {
                    lock (PhotoItems)
                    {
                        if (PhotoItems.Contains(_item))
                            PhotoItems.Remove(_item);
                    }

                    continue;
                }

                bool _relpaceOriginToMedium = false;

                if (_item.PostItemType == PostItemType.Origin)
                {
                    if (File.Exists(_item.LocalFilePath_Medium))
                    {
                        if (!canGetOrigin(_item))
                        {
                            lock (PhotoItems)
                            {
                                PhotoItems.Remove(_item);

                                if (PhotoItems.Count == 0)
                                    PhotoItems.Insert(0, _item);
                                else
                                    PhotoItems.Insert(PhotoItems.Count - 1, _item);
                            }

                            continue;
                        }
                    }
                    else
                    {
                        _url = _item.MediumPath.Replace("[IP]", WService.HostIP);
                        _localPath = _item.LocalFilePath_Medium;

                        _relpaceOriginToMedium = true;
                    }
                }

                try
                {
                    m_currentURL = _url;

                    WebRequest _wReq = WebRequest.Create(_url);
                    _wReq.Timeout = 15000;

                    WebResponse _wRep = _wReq.GetResponse();

                    Image _img = Image.FromStream(_wRep.GetResponseStream());

                    if ((Environment.GetCommandLineArgs().Length == 1) || (_item.PostItemType == PostItemType.Thumbnail))
                    {
                        if (!File.Exists(_localPath))
                        {
                            _img.Save(_localPath);
                        }
                    }

                    _img = null;

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

                    s_logger.Trace("GetFile:" + _localPath);
                }
                catch (Exception _e)
                {
                    NLogUtility.Exception_Warn(s_logger, _e, "Download Image URL", m_currentURL);

                    try
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
                                if (_item.PostItemType == PostItemType.Origin)
                                {
                                    m_downlaodErrorOriginFiles.Add(_url, DateTime.Now);
                                }

                                if (PhotoItems.Contains(_item))
                                {
                                    PhotoItems.Remove(_item);

                                    try
                                    {
                                        if (_item.PostID == Photo_DV.PostID)
                                        {
                                            if (PhotoItems.Count == 0)
                                            {
                                                PhotoItems.Insert(0, _item);
                                            }
                                            else
                                            {
                                                if (PhotoItems.Count > Photo_DV.UnloadPhotosCount)
                                                {
                                                    PhotoItems.Insert(new Random().Next(Photo_DV.UnloadPhotosCount), _item);
                                                }
                                                else
                                                {
                                                    PhotoItems.Insert(0, _item);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (_item.ErrorTry != ERROR_TRY)
                                            {
                                                if (PhotoItems.Count == 0)
                                                {
                                                    PhotoItems.Insert(0, _item);
                                                }
                                                else
                                                {
                                                    PhotoItems.Insert(PhotoItems.Count - 1, _item);
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private bool canGetOrigin(ImageItem item)
        {
            if (m_downlaodErrorOriginFiles.ContainsKey(item.OriginPath))
            {
                DateTime _dt = m_downlaodErrorOriginFiles[item.OriginPath];

                if (DateTime.Now > _dt.AddMinutes(GCONST.OriginFileReDownloadDelayTime))
                {
                    m_downlaodErrorOriginFiles.Remove(item.OriginPath);
                }
                else
                {
                    return false;
                }
            }

            if (m_downlaodErrorOriginFiles.ContainsKey(item.CloudOriginPath))
            {
                DateTime _dt = m_downlaodErrorOriginFiles[item.CloudOriginPath];

                if (DateTime.Now > _dt.AddMinutes(GCONST.OriginFileReDownloadDelayTime))
                {
                    m_downlaodErrorOriginFiles.Remove(item.CloudOriginPath);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static void PreloadPictures(Post post, bool allSize)
        {
            List<string> _filePathMediums = new List<string>();
            List<string> _urlMediums = new List<string>();

            if (post.type != "image" || post.attachment_id_array == null || post.attachment_id_array.Count == 0)
                return;


            foreach (var object_id in post.attachment_id_array)
            {
                string _urlM = Main.Current.RT.REST.attachments_getImageURL(object_id, "medium");
                string _localFileM = Main.Current.RT.REST.attachments_getThumbnailFilePath(object_id, "medium");

                _filePathMediums.Add(_localFileM);
                _urlMediums.Add(_urlM);
            }

            for (int i = _filePathMediums.Count - 1; i >= 0; i--)
            {
                if (File.Exists(_filePathMediums[i]))
                    continue;

                ImageItem _item = new ImageItem();

                _item.PostItemType = PostItemType.Medium;
                _item.MediumPath = _urlMediums[i];
                _item.LocalFilePath_Medium = _filePathMediums[i];
                _item.PostID = post.post_id;

                Main.Current.PhotoDownloader.Add(_item, false);
            }
        }
    }
}