using System.Collections.Generic;
using Waveface.API.V2;

namespace Waveface
{
    public class RunTimeData
    {
        public Dictionary<string, MR_groups_get> GroupSets { get; set; }
        public Dictionary<string, List<Post>> GroupPosts { get; set; }
        public List<User> AllUsers { get; set; }

        public void Reset()
        {
            GroupSets = new Dictionary<string, MR_groups_get>();
            GroupPosts = new Dictionary<string, List<Post>>();
            AllUsers = new List<User>();
        }
    }
}
