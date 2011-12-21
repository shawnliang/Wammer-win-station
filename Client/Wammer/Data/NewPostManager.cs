#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NLog;
using Newtonsoft.Json;
using Waveface.API.V2;

#endregion

namespace Waveface
{
    public class NewPostManager
    {
        public delegate void UpdateUI_Delegate(int percent, string text);

        public delegate void ShowMessage_Delegate(string text);

        public event UpdateUI_Delegate UpdateUI;
        public event ShowMessage_Delegate ShowMessage;

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private static NewPostManager s_newPostManager;
        private bool m_startUpload;

        public List<NewPostItem> Items { get; set; }

        public static NewPostManager Current
        {
            get
            {
                if (s_newPostManager == null)
                {
                    s_newPostManager = Load() ?? new NewPostManager();
                }

                return s_newPostManager;
            }
        }

        public NewPostManager()
        {
            Items = new List<NewPostItem>();

            ThreadPool.QueueUserWorkItem(state => { BatchPostThreadMethod(); });
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

        private void BatchPostThreadMethod()
        {
            if (ShowMessage != null)
                ShowMessage("Drag & Drop here");

            Thread.Sleep(3000);

            m_startUpload = true;

            while (true)
            {
                if (ShowMessage != null)
                    ShowMessage("Drag & Drop here");

                NewPostItem _newPost;

                lock (Current)
                {
                    if (Current.Items.Count > 0)
                        _newPost = Current.Items[Current.Items.Count - 1];
                    else
                        _newPost = null;
                }

                if (_newPost != null)
                {
                    if (ShowMessage != null)
                        ShowMessage("");

                    if (m_startUpload)
                    {
                        NewPostItem _retItem = BatchPhotoPost(_newPost);

                        if (_retItem.PostOK)
                        {
                            lock (Current)
                            {
                                Current.Remove(_newPost);
                            }
                        }
                        else
                        {
                            lock (Current)
                            {
                                Current.Save();
                            }
                        }
                    }
                }

                Thread.Sleep(1000);
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
                if (m_startUpload)
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
                        }
                        catch (Exception _e)
                        {
                            NLogUtility.Exception(s_logger, _e, "BatchPhotoPost:File_UploadFile");
                            newPost.PostOK = false;
                            return newPost;
                        }
                    }

                    _count++;

                    int _counts = newPost.Files.Count;

                    if (UpdateUI != null)
                        UpdateUI(_count*100/_counts, string.Format("Uploading {0} of {1} photos", _count, _counts));

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

        #region IO

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

        public static NewPostManager Load()
        {
            try
            {
                string _json = string.Empty;
                string _filePath = Main.GCONST.CachePath + Main.Current.RT.Login.user.user_id + "_NP.dat";

                StreamReader _sr = File.OpenText(_filePath);
                _json = _sr.ReadToEnd();
                _sr.Close();

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