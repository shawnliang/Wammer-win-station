using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		HashSet<Item> items = new HashSet<Item>();

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
			return items.Remove(new Item(data));
		}

		public IEnumerator<UserTrackDetail> GetEnumerator()
		{
			return items.Select(x => x.Data).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return items.Select(x => x.Data).GetEnumerator();
		}
	}

}
