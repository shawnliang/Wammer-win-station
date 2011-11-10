using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public class AtomicDictionary<TKey, TValue>
	{
		private Dictionary<TKey, TValue> map = new Dictionary<TKey, TValue>();

		public void Add(TKey key, TValue value)
		{
			lock (map)
			{
				map.Add(key, value);
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				lock (map)
				{
					return map[key];
				}
			}

			set
			{
				lock (map)
				{
					map[key] = value;
				}
			}
		}

		public Dictionary<TKey, TValue> GetAll()
		{
			lock (map)
			{
				return new Dictionary<TKey, TValue>(map);
			}
		}
	}
}
