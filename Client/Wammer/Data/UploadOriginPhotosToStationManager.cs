#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NLog;
using Newtonsoft.Json;
using Waveface.API.V2;

#endregion

namespace Waveface
{
    public class UploadOriginPhotosToStationManager
    {
        private static UploadOriginPhotosToStationManager s_current;
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private bool m_exit;
        private List<UploadOriginPhotosToStationItem> m_items;

        #region Properties

        public static UploadOriginPhotosToStationManager Current
        {
            get
            {
                if (s_current == null)
                {
                    s_current = Load() ?? new UploadOriginPhotosToStationManager();
                }

                return s_current;
            }
            set { s_current = value; }
        }

        #endregion

        public UploadOriginPhotosToStationManager()
        {
            m_items = new List<UploadOriginPhotosToStationItem>();
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(state => { ThreadMethod(); });
        }

        private void ThreadMethod()
        {
            UploadOriginPhotosToStationItem _item = null;

            while (!m_exit)
            {
                Thread.Sleep(2500);

                while (true)
                {
                    lock (Current)
                    {
                        if (m_items.Count == 0)
                        {
                            break;
                        }
                        else
                        {
                            _item = m_items[0];
                        }
                    }

                    if (!Main.Current.RT.StationMode)
                        break;

                    try
                    {
                        File.Copy(_item.FilePath_OID, _item.FilePath_REAL, true);

                        FileName _fileName = new FileName(_item.FilePath_REAL);

                        MR_attachments_upload _uf = Main.Current.RT.REST.File_UploadFile(_fileName.Name, _item.FilePath_REAL,
                                                                                         _item.ObjectID, true);

                        if (_uf == null)
                        {
                            break;
                        }


                        lock (Current)
                        {
                            m_items.Remove(_item);
                        }
                            
                        File.Delete(_item.FilePath_REAL);
                        File.Delete(_item.FilePath_OID);

                        s_logger.Trace("UploadOriginPhotosToStation:" + _item.FilePath_REAL);
                    }
                    catch (Exception _e)
                    {
                        NLogUtility.Exception(s_logger, _e, "ThreadMethod");
                        break;
                    }

                    Thread.Sleep(1);
                }
            }
        }

        public void Add(string filePath_OID, string filePath_REAL, string object_id)
        {
            UploadOriginPhotosToStationItem _item = new UploadOriginPhotosToStationItem();
            _item.FilePath_OID = filePath_OID;
            _item.FilePath_REAL = filePath_REAL;
            _item.ObjectID = object_id;

            lock (Current)
            {
                m_items.Add(_item);
            }
        }

        public void Exit()
        {
            m_exit = true;
        }

        public static UploadOriginPhotosToStationManager Load()
        {
            try
            {
                string _json = string.Empty;
                string _filePath = Main.GCONST.CachePath + Main.Current.RT.Login.user.user_id + "_UO.dat";

                StreamReader _sr = File.OpenText(_filePath);
                _json = _sr.ReadToEnd();
                _sr.Close();

                if (!GCONST.DEBUG)
                    _json = StringUtility.Decompress(_json);

                UploadOriginPhotosToStationManager _npm = JsonConvert.DeserializeObject<UploadOriginPhotosToStationManager>(_json);

                s_logger.Trace("Load:OK");

                return _npm;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "Load");
            }

            return null;
        }

        public bool Save()
        {
            try
            {
                string _json = JsonConvert.SerializeObject(this);

                if (!GCONST.DEBUG)
                    _json = StringUtility.Compress(_json);

                string _filePath = Main.GCONST.CachePath + Main.Current.RT.Login.user.user_id + "_UO.dat";

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
    }
}