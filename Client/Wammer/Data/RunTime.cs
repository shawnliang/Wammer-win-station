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

        private RT_REST m_rest;
        public MR_auth_login Login { get; set; }

        public bool StopBgThread { get; set; }

        public bool StationMode { get; set; }
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

        //public Dictionary<string, MR_groups_get> GroupGetReturnSets { get; set; }
        
        private Dictionary<string, List<Post>> m_groupPosts { get; set; }
        private Dictionary<string, List<string>> m_groupHaveReadPosts { get; set; }

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

        public List<string> CurrentGroupHaveReadPosts
        {
            get
            {
                if (!m_groupHaveReadPosts.ContainsKey(CurrentGroupID))
                    m_groupHaveReadPosts[CurrentGroupID] = new List<string>();

                return m_groupHaveReadPosts[CurrentGroupID];
            }
            set { m_groupHaveReadPosts[CurrentGroupID] = value; }
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

                if (!CurrentGroupHaveReadPosts.Contains(value))
                    CurrentGroupHaveReadPosts.Add(value);

                SaveGroupLocalRead();
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

            StopBgThread = false;

            StationMode = false;
            FilterMode = false;

            CurrentGroupID = string.Empty;
            
            //GroupGetReturnSets = new Dictionary<string, MR_groups_get>();
            
            m_groupPosts = new Dictionary<string, List<Post>>();
            m_groupHaveReadPosts = new Dictionary<string, List<string>>();

            m_groupLastReadID = new Dictionary<string, string>();
            m_groupLocalLastReadID = new Dictionary<string, string>();

            FilterPosts = new List<Post>();
            CurrentFilterItem = null;
            FilterPostsAllCount = -1;
            IsFilterFirstTimeGetData = true;
            FilterTimelineMode = true;

            AllUsers = new List<User>();      
        }

        public void SetAllCurrentGroupPostHaveRead()
        {
            foreach (Post _p in CurrentGroupPosts)
            {
                if (!CurrentGroupHaveReadPosts.Contains(_p.post_id))
                    CurrentGroupHaveReadPosts.Add(_p.post_id);
            }

            SaveGroupLocalRead();
        }

        #region Last Read

        public int GetMyTimelinePosition(ShowTimelineIndexType showTimelineIndexType)
        {
            string _id = "";

            switch (showTimelineIndexType)
            {
                case ShowTimelineIndexType.Newest:
                    _id = "";
                    break;

                case ShowTimelineIndexType.LocalLastRead:
                    _id = CurrentGroupLocalLastReadID;
                    break;


                case ShowTimelineIndexType.GlobalLastRead:
                    _id = CurrentGroupLastReadID;

                    break;

                case ShowTimelineIndexType.Global_Local_LastRead_Compare:
                    _id = GetMyReadPositionID();

                    break;
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

                if (!GCONST.DEBUG)
                    _json = StringUtility.Compress(_json);

				string _filePath = Path.Combine(Main.GCONST.RunTimeDataPath, Login.user.user_id + "_RT.dat");

                using (StreamWriter _outfile = new StreamWriter(_filePath))
                {
                    _outfile.Write(_json);
                }

                SaveGroupLocalRead();
            }
            catch (Exception _e)
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
				string _filePath = Path.Combine(Main.GCONST.RunTimeDataPath, Login.user.user_id + "_RT.dat");

                StreamReader _sr = File.OpenText(_filePath);
                _json = _sr.ReadToEnd();
                _sr.Close();

                if (!GCONST.DEBUG)
                    _json = StringUtility.Decompress(_json);

                RunTime _rt = JsonConvert.DeserializeObject<RunTime>(_json);

                s_logger.Trace("LoadJSON: OK");

                return _rt;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "LoadJSON");

                return null;
            }
        }

        #endregion

        #region LocalRead

        public class LocalReadRT
        {
            public Dictionary<string, string> GroupLocalLastReadID { get; set; }
            public Dictionary<string, List<string>> GroupHaveReadPosts { get; set; }
        }

        public bool SaveGroupLocalRead()
        {
            LocalReadRT _lr = new LocalReadRT();
            _lr.GroupLocalLastReadID = m_groupLocalLastReadID;
            _lr.GroupHaveReadPosts = m_groupHaveReadPosts;

            try
            {
                string _json = JsonConvert.SerializeObject(_lr);

                if (!GCONST.DEBUG)
                    _json = StringUtility.Compress(_json);

				string _filePath = Path.Combine(Main.GCONST.RunTimeDataPath, Login.user.user_id + "_LR.dat");

                using (StreamWriter _outfile = new StreamWriter(_filePath))
                {
                    _outfile.Write(_json);
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "SaveGroupLocalRead");

                return false;
            }

            s_logger.Trace("SaveGroupLocalRead: OK");

            return true;
        }

        public void LoadGroupLocalRead()
        {
            try
            {
                string _json = string.Empty;
				string _filePath = Path.Combine(Main.GCONST.RunTimeDataPath, Login.user.user_id + "_LR.dat");

				if (!File.Exists(_filePath))
					return;

                StreamReader _sr = File.OpenText(_filePath);
                _json = _sr.ReadToEnd();
                _sr.Close();

                if (!GCONST.DEBUG)
                    _json = StringUtility.Decompress(_json);

                LocalReadRT _lr = JsonConvert.DeserializeObject<LocalReadRT>(_json);

                m_groupLocalLastReadID = _lr.GroupLocalLastReadID;
                m_groupHaveReadPosts = _lr.GroupHaveReadPosts;

                s_logger.Trace("LoadGroupLocalRead:OK");
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "LoadGroupLocalRead");
            }
        }

        #endregion
    }
}