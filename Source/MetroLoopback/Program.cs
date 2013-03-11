#region

using System;
using System.Collections.Generic;
using IsolationAPI;

#endregion

namespace MyEnableMetroLoopBack
{
    internal class Program
    {
        private static void Main(string[] args)
        {
			if (args.Length < 1)
				return;

			try
			{
				AppContainer[] _appContainers = AppContainer.GetAppContainers(false);

				if (_appContainers == null)
				{
					// Unable to enumerate AppContainers.
					return;
				}

				AppContainer.MarkLoopbackExemptAppContainers(_appContainers);

				int _index = -1;

				for (int i = 0; i < _appContainers.Length; i++)
				{
					Console.WriteLine(i + ": " + _appContainers[i].Description); // _appContainers[i].PackageFullName

					if (_appContainers[i].Description == args[0])
					{
						_index = i;
					}
				}

				if (_index != -1)
				{
					List<AppContainer> _list = new List<AppContainer>();
					_list.Add(_appContainers[_index]);

					try
					{
						AppContainer.ExemptFromLoopbackBlocking(_list.ToArray());

						Console.WriteLine("Loopback exemption configuration successfully updated.");
					}
					catch
					{
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(string.Format("Unable to allow {0} to loopback: {1}", args[0], e.Message));
				Environment.Exit(-1);
			}
        }
    }
}