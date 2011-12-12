#region

using System.Collections.Generic;
using Newtonsoft.Json;
using Waveface.API.V2;
using Waveface.FilterUI;

#endregion

namespace Waveface
{
    public class RunTime
    {
        #region System

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
        private Dictionary<string, string> m_groupLastRead { get; set; }

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

        public string CurrentGroupLastRead
        {
            get
            {
                if (!m_groupLastRead.ContainsKey(CurrentGroupID))
                    m_groupLastRead[CurrentGroupID] = string.Empty;

                return m_groupLastRead[CurrentGroupID];
            }
            set { m_groupLastRead[CurrentGroupID] = value; }
        }

        #endregion

        public List<User> AllUsers { get; set; }

        public RunTime()
        {
            m_rest = new RT_REST(this);
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
            m_groupLastRead = new Dictionary<string, string>();

            FilterPosts = new List<Post>();
            CurrentFilterItem = null;
            FilterPostsAllCount = -1;
            IsFilterFirstTimeGetData = true;
            FilterTimelineMode = true;

            AllUsers = new List<User>();
        }

        public bool SaveJSON()
        {
            try
            {
                string _str = JsonConvert.SerializeObject(this);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public int GetCurrentGroupLastReadPosition()
        {
            for (int i = 0; i < CurrentGroupPosts.Count; i++)
            {
                if (CurrentGroupPosts[i].post_id == CurrentGroupLastRead)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}