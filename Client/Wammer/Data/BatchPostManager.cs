#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NLog;
using Newtonsoft.Json;
using Waveface.API.V2;

#endregion

namespace Waveface
{
    public class BatchPostManager
    {
        public event ProgressUpdateUI_Delegate UpdateUI;
        public event ShowMessage_Delegate ShowMessage;
        public event ShowMessage_Delegate UploadDone;
        public event ShowMessage_Delegate EditUpdateDone;
        public event ShowDialog_Delegate ShowFileMissDialog;
        public event ShowDialog_Delegate OverQuotaMissDialog;

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private WorkItem m_photoWorkItem;

        #region Properties

        public List<BatchPostItem> PhotoItems { get; set; }

        public bool StartUpload { get; set; }

        #endregion

        public BatchPostManager()
        {
            PhotoItems = new List<BatchPostItem>();
        }

        public void Start()
        {
            m_photoWorkItem = AbortableThreadPool.QueueUserWorkItem(ThreadMethod, 0);
        }

        public WorkItemStatus AbortThread()
        {
            Save();

            return AbortableThreadPool.Cancel(m_photoWorkItem, true);
        }

        public void Add(BatchPostItem item)
        {
            PhotoItems.Add(item);

            Save();
        }

        public void Remove(BatchPostItem item)
        {
            PhotoItems.Remove(item);

            Save();
        }

        public bool CheckPostInQueue(string postID)
        {
            foreach (BatchPostItem _item in PhotoItems)
            {
                if (_item.Post != null)
                {
                    if (_item.Post.post_id == postID)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public int GetQueuedUnsendFilesCount()
        {
            int _ret = 0;

            foreach (BatchPostItem _item in PhotoItems)
            {
                _ret += (_item.Files.Count - _item.UploadedFiles.Count);
            }

            return _ret;
        }

        private void ThreadMethod(object state)
        {
            if (ShowMessage != null)
            {
                ShowMessage(I18n.L.T("BatchPostManager.DragDropHere"));
            }

            Thread.Sleep(5000);

            StartUpload = true;

            while (true)
            {
                BatchPostItem _postItem;
                bool _editMode = false;

                if (Main.Current != null && !Main.Current.RT.REST.IsNetworkAvailable)
                {
                    UpdateUI(0, "");

                    continue;
                }

                lock (this)
                {
                    if (PhotoItems.Count > 0)
                    {
                        _postItem = PhotoItems[0];
                        _editMode = _postItem.EditMode;
                    }
                    else
                    {
                        _postItem = null;

                        if (ShowMessage != null)
                            ShowMessage(I18n.L.T("BatchPostManager.DragDropHere"));
                    }
                }

                if (_postItem != null)
                {
                    if (StartUpload)
                    {
                        BatchPostItem _retItem = UploadPhoto(_postItem);

                        if (_retItem.PostOK)
                        {
                            lock (this)
                            {
                                Remove(_postItem);

                                if (_editMode)
                                {
                                    if (EditUpdateDone != null)
                                        EditUpdateDone(I18n.L.T("PostForm.EditPostSuccess")); //@
                                }
                                else
                                {
                                    if (UploadDone != null)
                                        UploadDone(I18n.L.T("PostForm.PostSuccess"));
                                }
                            }
                        }
                        else
                        {
                            if (_retItem.ErrorAndDeletePost)
                            {
                                Remove(_postItem);

                                UpdateUI(int.MinValue, "");

                                if (_editMode)
                                {
                                    if (EditUpdateDone != null)
                                        EditUpdateDone(I18n.L.T("PostForm.EditPostError")); //@

                                    s_logger.Error("Remove EditUpdate Post");
                                }
                                else
                                {
                                    if (UploadDone != null)
                                        UploadDone(I18n.L.T("PostForm.PostError"));

                                    s_logger.Error("Remove New Post");
                                }

                                continue;
                            }

                            lock (this)
                            {
                                Save();
                            }
                        }
                    }
                }

                Thread.Sleep(2000);
            }
        }

        private BatchPostItem UploadPhoto(BatchPostItem pItem)
        {
            int _count = 0;
            string _tmpStamp = DateTime.Now.Ticks.ToString();

            // 先Create New Post或Update MetaData
            if (!pItem.PostSendMetaData)
            {
                string _ids = "[";

                for (int i = 0; i < pItem.ObjectIDs.Count; i++)
                {
                    _ids += "\"" + pItem.ObjectIDs[i] + "\"" + ",";
                }

                _ids = _ids.Substring(0, _ids.Length - 1); // 去掉最後一個","
                _ids += "]";

                try
                {
                    string _coverAttach = string.Empty;

                    if (pItem.CoverAttachIndex == -1)
                    {
                        _coverAttach = pItem.ObjectIDs[0];
                    }
                    else
                    {
                        _coverAttach = pItem.ObjectIDs[pItem.CoverAttachIndex];
                    }

                    if (pItem.EditMode)
                    {
                        Dictionary<string, string> _params = new Dictionary<string, string>();

                        if (pItem.Text != string.Empty)
                        {
                            _params.Add("content", pItem.Text);
                        }

                        _params.Add("attachment_id_array", _ids);
                        _params.Add("type", "image");
                        _params.Add("cover_attach", _coverAttach);

                        string _time = Main.Current.GetPostUpdateTime(pItem.Post);

                        MR_posts_update _update = Main.Current.RT.REST.Posts_update(pItem.PostID, _time, _params);

                        if (_update == null)
                        {
                            pItem.PostOK = false;

                            return pItem;
                        }

                        s_logger.Trace("[" + _tmpStamp + "]" + "Batch Post:" + pItem.Text + ", Added File(s)=" +
                                       pItem.ObjectIDs_Edit.Count + ", Update Post");
                    }
                    else
                    {
                        MR_posts_new _np = Main.Current.RT.REST.Posts_New(pItem.PostID, pItem.Text, _ids, "", "image", _coverAttach);

                        if (_np == null)
                        {
                            pItem.PostOK = false;
                            return pItem;
                        }

                        pItem.PostID = _np.post.post_id;

                        s_logger.Trace("[" + _tmpStamp + "]" + "Batch Post:" + pItem.Text + ", File(s)=" +
                                       pItem.ObjectIDs.Count + ", Create New Post");
                    }

                    pItem.PostSendMetaData = true;

                    var photoPostInfo = createPhotoPostInfo(pItem);

                    Main.Current.ReloadAllData(photoPostInfo);

                    lock (this)
                    {
                        Save();
                    }
                }
                catch (Exception _e)
                {
                    NLogUtility.Exception(s_logger, _e, "UploadPhoto:File_UploadFile:PostSendMetaData");

                    pItem.PostOK = false;
                    return pItem;
                }
            }

            // 再傳CoverAttach圖片
            if (pItem.PostSendMetaData && !pItem.CoverAttachUploaded)
            {
                int _coverAttachIndex = (pItem.CoverAttachIndex == -1) ? 0 : pItem.CoverAttachIndex;

                if (pItem.EditMode)
                {
                    string _coverAttach = pItem.ObjectIDs[_coverAttachIndex];

                    for (int i = 0; i < pItem.ObjectIDs_Edit.Count; i++)
                    {
                        if (pItem.ObjectIDs_Edit[i] == _coverAttach)
                        {
                            if (pItem.PreUploadedPhotos.Keys.Contains(pItem.Files[i]))
                                break;

                            BatchPostItem _pi = UploadOneFile(pItem, i, _tmpStamp, pItem.Files[i]);

                            if (_pi != null)
                                return _pi;

                            break;
                        }
                    }
                }
                else
                {
                    if (!pItem.PreUploadedPhotos.Keys.Contains(pItem.Files[_coverAttachIndex]))
                    {
                        BatchPostItem _pi = UploadOneFile(pItem, _coverAttachIndex, _tmpStamp,
                                                          pItem.Files[_coverAttachIndex]);

                        if (_pi != null)
                            return _pi;
                    }
                }

                pItem.CoverAttachUploaded = true;

                lock (this)
                {
                    Save();
                }
            }

            while (true)
            {
                if (StartUpload)
                {
                    string _file = pItem.Files[_count];

					if (pItem.UploadedFiles != null && pItem.UploadedFiles.Keys.Contains(_file))
                    {
                        s_logger.Trace("[" + _tmpStamp + "]" + "Batch Sended Photo [" + _count + "]" + _file);
                    }
                    else
                    {
						if (pItem.PreUploadedPhotos != null && pItem.PreUploadedPhotos.Keys.Contains(_file))
                        {
                            pItem.UploadedFiles.Add(_file, pItem.PreUploadedPhotos[_file]);
                        }
                        else
                        {
                            BatchPostItem _pi = UploadOneFile(pItem, _count, _tmpStamp, _file);

                            if (_pi != null)
                                return _pi;
                        }
                    }

                    _count++;

                    int _counts = pItem.Files.Count;

                    if (UpdateUI != null)
                    {
                        string _msg;

                        if (PhotoItems.Count == 1)
                        {
                            _msg = string.Format(I18n.L.T("OnePostUpload"), _count, _counts - _count);
                        }
                        else
                        {
                            _msg = string.Format(I18n.L.T("MultiplePostUpload"), _count, _counts - _count,
                                                 PhotoItems.Count - 1);
                        }

                        UpdateUI(_count * 100 / _counts, _msg);
                    }

                    if (_count == _counts)
                        break;
                }
                else
                {
                    pItem.PostOK = false;

                    return pItem;
                }
            }

            pItem.PostOK = true;
            return pItem;
        }

        private static PhotoPostInfo createPhotoPostInfo(BatchPostItem item)
        {
            var info = new PhotoPostInfo { post_id = item.PostID };

            if (item.EditMode)
            {
                for (int i = 0; i < item.ObjectIDs_Edit.Count; i++)
                {
                    info.sources.Add(
                        item.ObjectIDs_Edit[i],
                        item.Files[i]
                    );
                }
            }
            else
            {
                for (int i = 0; i < item.ObjectIDs.Count; i++)
                {
                    info.sources.Add(
                        item.ObjectIDs[i],
                        item.Files[i]);
                }
            }

            return info;
        }

        private BatchPostItem UploadOneFile(BatchPostItem pItem, int count, string tmpStamp, string file)
        {
            try
            {
                if (!File.Exists(file))
                {
                    // 原始檔案不存在. 作錯誤處裡
                    s_logger.Error("Image File does not exist: [" + file + "]");

                    if (ShowFileMissDialog != null)
                        ShowFileMissDialog(file);

                    while (Main.Current.NewPostThreadErrorDialogResult == DialogResult.None)
                        Thread.Sleep(500);

                    switch (Main.Current.NewPostThreadErrorDialogResult)
                    {
                        case DialogResult.Cancel: // Delete Post
                            pItem.ErrorAndDeletePost = true;
                            pItem.PostOK = false;
                            return pItem;

                        case DialogResult.Yes: // Remove Picture
                            s_logger.Error("Remove: [" + file + "]");

                            if (pItem.CoverAttachIndex == count)
                                pItem.CoverAttachIndex = 0;

                            pItem.Files.Remove(file);
                            pItem.ObjectIDs.RemoveAt(count);

                            UpdatePostWhenRemovePhoto(pItem);

                            pItem.PostOK = false;

                            UpdateUI(int.MinValue, "");

                            return pItem;

                        case DialogResult.Retry: // DoNothing
                            s_logger.Error("Ignore & Retry Miss File: [" + file + "]");

                            pItem.PostOK = false;
                            return pItem;
                    }
                }

                string _text = new FileName(file).Name;
                string _resizedImage = ImageUtility.ResizeImage(file, _text, pItem.LongSideResizeOrRatio, 100);

                string _objID = pItem.EditMode ? pItem.ObjectIDs_Edit[count] : pItem.ObjectIDs[count];

                MR_attachments_upload _uf = Main.Current.RT.REST.File_UploadFile(_text, _resizedImage, _objID, true,
                                                                                 pItem.PostID);

                if (_uf == null)
                {
                    pItem.PostOK = false;
                    return pItem;
                }

                pItem.UploadedFiles.Add(file, _uf.object_id);

                s_logger.Trace("[" + tmpStamp + "][" + _objID + "] Batch Upload Photo [" + count + "]" + file);
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "UploadPhoto:File_UploadFile");

                pItem.PostOK = false;
                return pItem;
            }

            lock (this)
            {
                Save();
            }

            return null;
        }

        private bool UpdatePostWhenRemovePhoto(BatchPostItem pItem)
        {
            string _ids = "[";

            for (int i = 0; i < pItem.ObjectIDs.Count; i++)
            {
                _ids += "\"" + pItem.ObjectIDs[i] + "\"" + ",";
            }

            _ids = _ids.Substring(0, _ids.Length - 1); // 去掉最後一個","
            _ids += "]";

            try
            {
                Dictionary<string, string> _params = new Dictionary<string, string>();

                _params.Add("attachment_id_array", _ids);
                _params.Add("type", "image");
                _params.Add("cover_attach", pItem.ObjectIDs[pItem.CoverAttachIndex]);

                string _time = string.Empty;

                MR_posts_getSingle _singlePost = Main.Current.RT.REST.Posts_GetSingle(pItem.PostID);

                if ((_singlePost != null) && (_singlePost.post != null))
                {
                    if (_singlePost.post.update_time != null)
                    {
                        _time = _singlePost.post.update_time;
                    }
                }

                MR_posts_update _update = Main.Current.RT.REST.Posts_update(pItem.PostID, _time, _params);

                if (_update == null)
                {
                    pItem.PostOK = false;

                    return false;
                }

                lock (this)
                {
                    Save();
                }

                Main.Current.ReloadAllData();
            }
            catch (Exception _e)
            {
                return false;
            }

            return true;
        }

        /*
            private long CheckStoragesUsage()
            {
                try
                {
                    MR_storages_usage _storagesUsage = Main.Current.RT.REST.Storages_Usage();

                    if (_storagesUsage != null)
                    {
                        long m_avail_month_total_objects = _storagesUsage.storages.waveface.available.avail_month_total_objects;
                        long m_month_total_objects = _storagesUsage.storages.waveface.quota.month_total_objects;

                        //Hack
                        if (m_month_total_objects == -1)
                        {
                            return long.MaxValue;
                        }

                        return m_avail_month_total_objects;
                    }
                }
                catch (Exception _e)
                {
                    NLogUtility.Exception(s_logger, _e, "CheckStoragesUsage");
                }

                return long.MinValue;
            }
            */

        #region IO

        public bool Save()
        {
            try
            {
                string _json = JsonConvert.SerializeObject(this);

                if (!GCONST.DEBUG)
                    _json = StringUtility.Compress(_json);

                string _filePath = Path.Combine(Main.GCONST.RunTimeDataPath, Main.Current.RT.Login.user.user_id + "_NP.txt");

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

        public static BatchPostManager Load()
        {
            try
            {
                string _json = string.Empty;
                string _filePath = Path.Combine(Main.GCONST.RunTimeDataPath, Main.Current.RT.Login.user.user_id + "_NP.txt");

                StreamReader _sr = File.OpenText(_filePath);
                _json = _sr.ReadToEnd();
                _sr.Close();

                if (!GCONST.DEBUG)
                    _json = StringUtility.Decompress(_json);

                BatchPostManager _npm = JsonConvert.DeserializeObject<BatchPostManager>(_json);

                s_logger.Trace("Load:OK");

                return _npm;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "Load");
            }

            return null;
        }

        #endregion
    }

    class PhotoPostInfo
    {
        public string post_id { get; set; }
        //key: object_id, value: source file path
        public Dictionary<string, string> sources { get; set; }

        public PhotoPostInfo()
        {
            sources = new Dictionary<string, string>();
        }
    }

}