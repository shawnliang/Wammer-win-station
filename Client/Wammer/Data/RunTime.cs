#region

using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using Newtonsoft.Json;
using Waveface.API.V2;
using Waveface.FilterUI;
using System.Windows.Forms;

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

            FilterPosts = new List<Post>();
            CurrentFilterItem = null;
            FilterPostsAllCount = -1;
            IsFilterFirstTimeGetData = true;
            FilterTimelineMode = true;

            AllUsers = new List<User>();
        }

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
    }
}