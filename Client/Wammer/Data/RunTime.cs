#region

using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using Newtonsoft.Json;
using Waveface.API.V2;
using Waveface.FilterUI;

#endregion

namespace Waveface
{
    public class RunTime
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        #region System

        private string RUN_TIME_FILE = "Waveface System.dat";
        private string LAST_READ_FILE = "Waveface Last Read.dat";

        private RT_REST m_rest;
        public MR_auth_login Login { get; set; }

        public bool StationMode { get; set; }
        public bool OnlineMode { get; set; }
        public bool FilterMode { get; set; }

        public RT_REST REST
        {
            get { return m_rest; }
        }

        public bool LoginOK
        {
            get { return Login != null; }
        }

        #endregion

        #region Filter

        public bool IsFilterFirstTimeGetData { get; set; }
        public bool FilterTimelineMode { get; set; }
        public List<Post> FilterPosts { get; set; }
        public FilterItem CurrentFilterItem { get; set; }
        public int FilterPostsAllCount { get; set; }

        #endregion

        #region Group

        public string CurrentGroupID { get; set; }

        public Dictionary<string, MR_groups_get> GroupGetReturnSets { get; set; }
        private Dictionary<string, List<Post>> m_groupPosts { get; set; }

        public List<Post> CurrentGroupPosts
        {
            get
            {
                if (!m_groupPosts.ContainsKey(CurrentGroupID))
                    m_groupPosts[CurrentGroupID] = new List<Post>();

                return m_groupPosts[CurrentGroupID];
            }
            set { m_groupPosts[CurrentGroupID] = value; }
        }

        #endregion

        #region LastRead

        private Dictionary<string, string> m_groupLastReadID { get; set; }
        private Dictionary<string, string> m_groupLocalLastReadID { get; set; }

        public string CurrentGroupLastReadID
        {
            get
            {
                if (!m_groupLastReadID.ContainsKey(CurrentGroupID))
                    m_groupLastReadID[CurrentGroupID] = string.Empty;

                return m_groupLastReadID[CurrentGroupID];
            }
            set
            {
                m_groupLastReadID[CurrentGroupID] = value;
            }
        }

        public string CurrentGroupLocalLastReadID
        {
            get
            {
                if (!m_groupLocalLastReadID.ContainsKey(CurrentGroupID))
                    m_groupLocalLastReadID[CurrentGroupID] = string.Empty;

                return m_groupLocalLastReadID[CurrentGroupID];
            }
            set
            {
                m_groupLocalLastReadID[CurrentGroupID] = value;

                SaveGroupLocalLastRead();
            }
        }

        #endregion

        public List<User> AllUsers { get; set; }

        public RunTime()
        {
            m_rest = new RT_REST(this);
            Reset();
        }

        public void Reset()
        {
            Login = null;
            StationMode = false;
            OnlineMode = false;
            FilterMode = false;

            CurrentGroupID = string.Empty;
            GroupGetReturnSets = new Dictionary<string, MR_groups_get>();
            m_groupPosts = new Dictionary<string, List<Post>>();

            m_groupLastReadID = new Dictionary<string, string>();
            m_groupLocalLastReadID = new Dictionary<string, string>();

            FilterPosts = new List<Post>();
            CurrentFilterItem = null;
            FilterPostsAllCount = -1;
            IsFilterFirstTimeGetData = true;
            FilterTimelineMode = true;

            AllUsers = new List<User>();

            LoadGroupLocalLastRead();
        }

        #region Last Read

        public int GetMyTimelinePosition(bool keepTimelineIndex)
        {
            string _id;

            if (keepTimelineIndex)
            {
                _id = CurrentGroupLocalLastReadID;
            }
            else
            {
                _id = ""; 
                //_id = GetMyReadPositionID();
            }

            if (_id != string.Empty)
            {
                for (int i = 0; i < CurrentGroupPosts.Count; i++)
                {
                    if (CurrentGroupPosts[i].post_id == _id)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        public string GetMyReadPositionID()
        {
            if (CurrentGroupLastReadID == string.Empty)
            {
                s_logger.Trace("GetMyReadPositionID:CurrentGroupLocalLastReadID = " + CurrentGroupLocalLastReadID);

                return CurrentGroupLocalLastReadID;
            }
            else
            {
                int _lastRead = -1;
                int _localLastRead = -1;

                for (int i = 0; i < CurrentGroupPosts.Count; i++)
                {
                    Post _p = CurrentGroupPosts[i];

                    if (_p.post_id == CurrentGroupLastReadID)
                        _lastRead = i;

                    if (_p.post_id == CurrentGroupLocalLastReadID)
                        _localLastRead = i;
                }

                if (_lastRead == -1)
                {
                    s_logger.Trace("GetMyReadPositionID:CurrentGroupLocalLastReadID = " + CurrentGroupLocalLastReadID);

                    return CurrentGroupLocalLastReadID;
                }

                if (_localLastRead == -1)
                {
                    s_logger.Trace("GetMyReadPositionID:Empty");

                    return string.Empty;
                }

                if (_lastRead < _localLastRead)
                {
                    s_logger.Trace("GetMyReadPositionID:CurrentGroupLocalLastReadID = " + CurrentGroupLocalLastReadID);

                    return CurrentGroupLocalLastReadID;
                }
                else
                {
                    s_logger.Trace("GetMyReadPositionID:CurrentGroupLastReadID = " + CurrentGroupLastReadID);

                    return CurrentGroupLastReadID;
                }
            }
        }

        public void SetCurrentGroupLocalLastRead(Post post)
        {
            CurrentGroupLocalLastReadID = post.post_id;
        }

        public void SetCurrentGroupLastRead(LastScan lastScan)
        {
            CurrentGroupLastReadID = lastScan.post_id;
        }

        #endregion

        #region IO

        public bool SaveJSON()
        {
            try
            {
                string _json = JsonConvert.SerializeObject(this);

                string _filePath = Main.GCONST.CachePath + RUN_TIME_FILE;

                using (StreamWriter _outfile = new StreamWriter(_filePath))
                {
                    _outfile.Write(_json);
                }

                SaveGroupLocalLastRead();
            }
            catch(Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "SaveJSON");

                return false;
            }

            s_logger.Trace("SaveJSON: OK");

            return true;
        }

        public RunTime LoadJSON()
        {
            try
            {
                string _json = string.Empty;
                string _filePath = Main.GCONST.CachePath + RUN_TIME_FILE;

                StreamReader _sr = File.OpenText(_filePath);
                _json = _sr.ReadToEnd();
                _sr.Close();

                RunTime _rt = JsonConvert.DeserializeObject<RunTime>(_json);

                s_logger.Trace("LoadJSON: OK");

                return _rt;
            }
            catch(Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "LoadJSON");

                return null;
            }
        }

        #endregion

        #region LocalLastRead

        public class LocalLastReadRT
        {
            public Dictionary<string, string> GroupLocalLastReadID { get; set; }
        }

        public bool SaveGroupLocalLastRead()
        {
            LocalLastReadRT _lr = new LocalLastReadRT();
            _lr.GroupLocalLastReadID = m_groupLocalLastReadID;

            try
            {
                string _json = JsonConvert.SerializeObject(_lr);

                string _filePath = Main.GCONST.CachePath + LAST_READ_FILE;

                using (StreamWriter _outfile = new StreamWriter(_filePath))
                {
                    _outfile.Write(_json);
                }
            }
            catch(Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "SaveGroupLocalLastRead");

                return false;
            }

            s_logger.Trace("SaveGroupLocalLastRead: OK");

            return true;
        }

        public void LoadGroupLocalLastRead()
        {
            try
            {
                string _json = string.Empty;
                string _filePath = Main.GCONST.CachePath + LAST_READ_FILE;

                StreamReader _sr = File.OpenText(_filePath);
                _json = _sr.ReadToEnd();
                _sr.Close();

                LocalLastReadRT _lr = JsonConvert.DeserializeObject<LocalLastReadRT>(_json);

                m_groupLocalLastReadID = _lr.GroupLocalLastReadID;

                s_logger.Trace("LoadGroupLocalLastRead:OK");
            }
            catch(Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "LoadGroupLocalLastRead");
            }
        }

        #endregion
    }
}