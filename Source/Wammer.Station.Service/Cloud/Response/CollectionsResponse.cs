using System;
using System.Collections.Generic;

namespace Wammer.Cloud
{
	public class CollectionsResponse : CloudResponse
	{
		public List<Collection> collections { get; set; }
	}

	public class ObjectList
	{
		public string object_id { get; set; }
	}

	public class Collection
	{
		public string name { get; set; }
		public int seq_num { get; set; }
		public List<ObjectList> object_list { get; set; }
		public string creator_id { get; set; }
		public string create_time { get; set; }
		public string modify_time { get; set; }
		public string collection_id { get; set; }
		public String cover { get; set; }
		public bool hidden { get; set; }
		public bool smart { get; set; }
		public bool manual { get; set; }
	}
}