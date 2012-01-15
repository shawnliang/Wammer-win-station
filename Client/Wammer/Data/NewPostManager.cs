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
    public class NewPostManager
    {
        public event ProgressUpdateUI_Delegate UpdateUI;
        public event ShowMessage_Delegate ShowMessage;
        public event ShowMessage_Delegate UploadDone;

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private bool m_startUpload;
        private bool m_downloading;

        private WorkItem m_workItem;

        #region Properties

        public List<NewPostItem> Items { get; set; }

        public bool StartUpload
        {
            get { return m_startUpload; }
            set { m_startUpload = value; }
        }

        public bool Downloading
        {
            get { return m_downloading; }
            set { m_downloading = value; }
        }

        #endregion

        public NewPostManager()
        {
            Items = new List<NewPostItem>();
        }

        public void Start()
        {
            m_workItem = AbortableThreadPool.QueueUserWorkItem(BatchPostThreadMethod, 0);
        }

        public WorkItemStatus AbortThread()
        {
            Save();

            return AbortableThreadPool.Cancel(m_workItem, true);
        }

        public void Add(NewPostItem item)
        {
            Items.Add(item);

            Save();
        }

        public void Remove(NewPostItem item)
        {
            Items.Remove(item);

            Save();
        }

        public int GetQueuedUnsendFilesCount()
        {
            int _ret = 0;

            foreach (NewPostItem _item in Items)
            {
                _ret += (_item.Files.Count - _item.UploadedFiles.Count);
            }

            return _ret;
        }

        private void BatchPostThreadMethod(object state)
        {
            if (ShowMessage != null)
            {
                ShowMessage(I18n.L.T("NewPostManager.DragDropHere"));
            }

            Thread.Sleep(3000);

            StartUpload = true;

            while (true)
            {
                NewPostItem _newPost;

                if (!Main.Current.RT.REST.IsNetworkAvailable)
                {
                    if (ShowMessage != null)
                        ShowMessage("");

                    UpdateUI(0, "");

                    continue;
                }

                lock (this)
                {
                    if (Items.Count > 0)
                    {
                        _newPost = Items[0];
                    }
                    else
                    {
                        _newPost = null;

                        if (ShowMessage != null)
                            ShowMessage(I18n.L.T("NewPostManager.DragDropHere"));
                    }
                }

                if (_newPost != null)
                {
                    if (ShowMessage != null)
                        ShowMessage("");

                    if (StartUpload)
                    {
                        NewPostItem _retItem = BatchPhotoPost(_newPost);

                        if (_retItem.PostOK)
                        {
                            lock (this)
                            {
                                Remove(_newPost);

                                if (UploadDone != null)
                                    UploadDone(I18n.L.T("PostForm.PostSuccess"));
                            }
                        }
                        else
                        {
                            if (_retItem.ErrorAndDeletePost)
                            {
                                Remove(_newPost);

                                UpdateUI(int.MinValue, "");

                                if (UploadDone != null)
                                    UploadDone(I18n.L.T("PostForm.PostError"));

                                s_logger.Error("Remove New Post");

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

        private NewPostItem BatchPhotoPost(NewPostItem newPost)
        {
            int _count = 0;
            string _tmpStamp = DateTime.Now.Ticks.ToString();

            s_logger.Trace("[" + _tmpStamp + "]" + "BatchPhotoPost:" + newPost.Text + ", Files=" + newPost.Files.Count);

            string _ids = "[";

            while (true)
            {
                if (StartUpload)
                {
                    string _file = newPost.Files[_count];

                    if (newPost.UploadedFiles.Keys.Contains(_file))
                    {
                        _ids += "\"" + newPost.UploadedFiles[_file] + "\"" + ",";

                        s_logger.Trace("[" + _tmpStamp + "]" + "Batch Sended Photo [" + _count + "]" + _file);
                    }
                    else
                    {
                        try
                        {
                            Downloading = true;

                            if (!File.Exists(_file))
                            {
                                // 原始檔案不存在. 作錯誤處裡
                                s_logger.Error("Image File does not exist: [" + _file + "]");

                                DialogResult _dr = MessageBox.Show("", "Waveface", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                                switch (_dr)
                                {
                                    case DialogResult.Cancel:  // Delete Post
                                        newPost.ErrorAndDeletePost = true;
                                        newPost.PostOK = false;
                                        return newPost;

                                    case DialogResult.Yes: // Remove Picture
                                        s_logger.Error("Remove: [" + _file + "]");

                                        newPost.Files.Remove(_file);
                                        newPost.PostOK = false;

                                        UpdateUI(int.MinValue, "");

                                        return newPost;

                                    case DialogResult.No:  // DoNothing
                                        s_logger.Error("Ignore & Retry Miss File: [" + _file + "]");

                                        newPost.PostOK = false;
                                        return newPost;
                                }
                            }

                            if (CheckStoragesUsage() <= 0)
                            {
                                // 雲端個人儲存空間不足. 作錯誤處裡 
                                s_logger.Error("(CheckStoragesUsage() <= 0)");

                                DialogResult _dr = MessageBox.Show("", "Waveface", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);

                                switch (_dr)
                                {
                                    case DialogResult.Cancel:  // Delete Post
                                        newPost.ErrorAndDeletePost = true;
                                        newPost.PostOK = false;
                                        return newPost;

                                    case DialogResult.No:  // DoNothing

                                        newPost.PostOK = false;
                                        return newPost;
                                }
                            }

                            string _text = new FileName(_file).Name;
                            string _resizedImage = ImageUtility.ResizeImage(_file, _text, newPost.ResizeRatio, 100);

                            MR_attachments_upload _uf = Main.Current.RT.REST.File_UploadFile(_text, _resizedImage, "",
                                                                                             true);
                            if (_uf == null)
                            {
                                newPost.PostOK = false;
                                return newPost;
                            }

                            _ids += "\"" + _uf.object_id + "\"" + ",";

                            newPost.UploadedFiles.Add(_file, _uf.object_id);

                            s_logger.Trace("[" + _tmpStamp + "]" + "Batch Upload Photo [" + _count + "]" + _file);

                            string _localFile = Main.GCONST.CachePath + _uf.object_id + "_origin"; // "_origin_" + _text;
                            File.Copy(_file, _localFile);

                            Downloading = false;
                        }
                        catch (Exception _e)
                        {
                            Downloading = false;

                            NLogUtility.Exception(s_logger, _e, "BatchPhotoPost:File_UploadFile");
                            newPost.PostOK = false;
                            return newPost;
                        }
                    }

                    _count++;

                    int _counts = newPost.Files.Count;

                    if (UpdateUI != null)
                    {
                        string _msg;

                        if (Items.Count == 1)
                        {
                            _msg = string.Format(I18n.L.T("OnePostUpload"), _count, _counts - _count);
                        }
                        else
                        {
                            _msg = string.Format(I18n.L.T("MultiplePostUpload"), _count, _counts - _count, Items.Count - 1);
                        }

                        UpdateUI(_count * 100 / _counts, _msg);
                    }

                    if (_count == _counts)
                        break;
                }
                else
                {
                    newPost.PostOK = false;
                    return newPost;
                }
            }

            _ids = _ids.Substring(0, _ids.Length - 1); // 去掉最後一個","
            _ids += "]";

            try
            {
                MR_posts_new _np = Main.Current.RT.REST.Posts_New(newPost.Text, _ids, "", "image");

                if (_np == null)
                {
                    newPost.PostOK = false;
                    return newPost;
                }

                s_logger.Trace("[" + _tmpStamp + "]" + "Batch Post:" + newPost.Text + ", Files=" + newPost.Files.Count);
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "BatchPhotoPost:File_UploadFile");

                newPost.PostOK = false;
                return newPost;
            }

            newPost.PostOK = true;
            return newPost;
        }

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

        #region IO

        public bool Save()
        {
            try
            {
                string _json = JsonConvert.SerializeObject(this);

                if (!GCONST.DEBUG)
                    _json = StringUtility.Compress(_json);

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

        public static NewPostManager Load()
        {
            try
            {
                string _json = string.Empty;
                string _filePath = Main.GCONST.CachePath + Main.Current.RT.Login.user.user_id + "_NP.dat";

                StreamReader _sr = File.OpenText(_filePath);
                _json = _sr.ReadToEnd();
                _sr.Close();

                if (!GCONST.DEBUG)
                    _json = StringUtility.Decompress(_json);

                NewPostManager _npm = JsonConvert.DeserializeObject<NewPostManager>(_json);

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
}