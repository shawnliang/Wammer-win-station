using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface
{
	public static class EnumerableExtender
	{
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this  IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				var elementValue = keySelector(element);
				if (seenKeys.Add(elementValue))
				{
					yield return element;
				}
			}
		}
	}
}
