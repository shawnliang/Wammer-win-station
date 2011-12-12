#region

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Waveface.API.V2;
using Waveface.FilterUI;

#endregion

namespace Waveface
{
    public class RunTime
    {
        private RT_REST m_rest;

        public bool IsTimelineFilter { get; set; }
        public Dictionary<string, MR_groups_get> GroupSets { get; set; }
        public Dictionary<string, List<Post>> GroupPosts { get; set; }
        public Dictionary<string, List<Post>> GroupAllPosts { get; set; }
        public Dictionary<string, int> GroupPostsAllCount { get; set; }

        public List<User> AllUsers { get; set; }
        public List<Post> FilterPosts { get; set; }
        public FilterItem CurrentFilterItem { get; set; }
        public bool IsAllPostMode { get; set; }
        public string CurrentGroupID { get; set; }
        public int FilterPostsAllCount { get; set; }
        public bool IsFirstTimeGetData { get; set; }
        public bool StationMode { get; set; }
        public bool OnlineMode { get; set; }
        public MR_auth_login Login { get; set; }

        public RT_REST REST
        {
            get
            {
                return m_rest;
            }
        }

        public bool LoginOK
        {
            get
            {
                return Login != null;
            }
        }

        public List<Post> CurrentGroupPosts
        {
            get
            {
                if (!GroupPosts.ContainsKey(CurrentGroupID))
                    GroupPosts[CurrentGroupID] = new List<Post>();

                return GroupPosts[CurrentGroupID];
            }
            set { GroupPosts[CurrentGroupID] = value; }
        }

        public int CurrentGroupPostsAllCount
        {
            get
            {
                if (!GroupPostsAllCount.ContainsKey(CurrentGroupID))
                    GroupPostsAllCount[CurrentGroupID] = -1;

                return GroupPostsAllCount[CurrentGroupID];
            }
            set { GroupPostsAllCount[CurrentGroupID] = value; }
        }

        public List<Post> CurrentPosts
        {
            get
            {
                if (IsAllPostMode)
                    return CurrentGroupPosts;
                else
                    return FilterPosts;
            }
            set
            {
                if (IsAllPostMode)
                    CurrentGroupPosts = value;
                else
                    FilterPosts = value;
            }
        }

        public int CurrentPostsAllCount
        {
            get
            {
                if (IsAllPostMode)
                    return CurrentGroupPostsAllCount;
                else
                    return FilterPostsAllCount;
            }
            set
            {
                if (IsAllPostMode)
                    CurrentGroupPostsAllCount = value;
                else
                    FilterPostsAllCount = value;
            }
        }

        public RunTime()
        {
            m_rest = new RT_REST(this);
        }

        public void Reset()
        {
            GroupSets = new Dictionary<string, MR_groups_get>();
            GroupPosts = new Dictionary<string, List<Post>>();
            GroupAllPosts = new Dictionary<string, List<Post>>();
            GroupPostsAllCount = new Dictionary<string, int>();

            AllUsers = new List<User>();
            FilterPosts = new List<Post>();
            CurrentFilterItem = null;
            IsAllPostMode = true;
            CurrentGroupID = string.Empty;
            IsTimelineFilter = true;
            FilterPostsAllCount = -1;
            IsFirstTimeGetData = true;
            StationMode = false;
            OnlineMode = false;
            Login = null;
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
    }
}