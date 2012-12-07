using System;

namespace Waveface.Stream.WindowsClient
{
	public class WizardParameters : System.Collections.Specialized.NameObjectCollectionBase
	{
		public void Set(string key, object value)
		{
			if (value == null)
				throw new ArgumentNullException("value cannot be null");

			if (BaseGet(key) != null)
				BaseSet(key, value);
			else
				BaseAdd(key, value);
		}

		public object Get(string key)
		{
			return BaseGet(key);
		}
	}
}
