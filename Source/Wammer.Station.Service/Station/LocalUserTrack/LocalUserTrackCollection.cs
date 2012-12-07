using System;
using System.Collections.Generic;
using System.Linq;
using Wammer.Cloud;

namespace Wammer.Station.LocalUserTrack
{
	class LocalUserTrackCollection : ICollection<UserTrackDetail>
	{
		class Item
		{
			public Item(UserTrackDetail data)
			{
				if (data == null)
					throw new ArgumentNullException();

				Data = data;
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
					return true;

				if (!(obj is Item))
					return false;

				return Data.target_id.Equals((obj as Item).Data.target_id);
			}

			public override int GetHashCode()
			{
				return Data.target_id.GetHashCode();
			}

			public UserTrackDetail Data { get; set; }
		}

		private HashSet<Item> items = new HashSet<Item>();

		// This dictionary records all used usertracks by sessions.
		//
		// key: session_token
		// value: used user tracks of a session
		private Dictionary<string, HashSet<Item>> usedUserTracks = new Dictionary<string, HashSet<Item>>();

		public IEnumerable<UserTrackDetail> GetUserTracksBySession(string group_id, string session_token)
		{
			if (usedUserTracks.ContainsKey(session_token))
			{
				var used = usedUserTracks[session_token];
				var unused = items.Where(x => !used.Contains(x)).Select(x => x.Data);

				usedUserTracks[session_token] = new HashSet<Item>(items);
				return unused;
			}
			else
			{
				usedUserTracks.Add(session_token, new HashSet<Item>(items));
				return this.ToList();
			}
		}


		#region Implements ICollection<UserTrackDetail> Interface
		public void Add(UserTrackDetail data)
		{
			items.Add(new Item(data));
		}

		public void Clear()
		{
			items.Clear();
		}

		public bool Contains(UserTrackDetail data)
		{
			return items.Contains(new Item(data));
		}

		public void CopyTo(UserTrackDetail[] array, int arrayIndex)
		{
			if (array.Length - arrayIndex < items.Count)
				throw new ArgumentException("array is not large enough");

			int i = arrayIndex;
			foreach (var item in items)
			{
				array[i++] = item.Data;
			}
		}

		public int Count
		{
			get { return items.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(UserTrackDetail data)
		{
			var removeItem = new Item(data);

			foreach (var used in usedUserTracks)
			{
				used.Value.Remove(removeItem);
			}

			return items.Remove(removeItem);
		}

		public IEnumerator<UserTrackDetail> GetEnumerator()
		{
			return items.Select(x => x.Data).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return items.Select(x => x.Data).GetEnumerator();
		}
		#endregion


	}

}
