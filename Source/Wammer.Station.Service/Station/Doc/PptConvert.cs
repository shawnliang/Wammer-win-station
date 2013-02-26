using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Wammer.Station.Doc
{
	static class PptConvert
	{
		static object cs = new object();

		private static void NAR(object o)
		{
			try
			{
				while ((Marshal.ReleaseComObject(o) > 0) || (Marshal.FinalReleaseComObject(o) > 0))
				{
				}
			}
			catch
			{
			}
			finally
			{
				o = null;
			}
		}

		public static List<string> ConvertPptToJpg(string file, string outputPath)
		{
			lock (cs)
			{
				Type t = Type.GetTypeFromProgID("PowerPoint.Application");
				object o = Activator.CreateInstance(t);

				object p = t.InvokeMember(
					"Presentations",
					BindingFlags.Public | BindingFlags.GetProperty,
					null, o, null, null);

				Type t2 = p.GetType();
				object ppt = t2.InvokeMember("Open",
					BindingFlags.Public | BindingFlags.InvokeMethod,
					null, p, new object[] { file, 0, 0, 0 }, null);

				object slides = t2.InvokeMember(
				"Slides",
				BindingFlags.Public | BindingFlags.GetProperty,
				null, ppt, null, null);

				var outputFiles = new List<string>();
				var index = 0;
				foreach (object slid in (slides as IEnumerable))
				{
					++index;
					var slidType = slid.GetType();
					var slidPhotoFile = Path.Combine(outputPath, string.Format("{0}.jpg", index.ToString("d8")));
					slidType.InvokeMember("Export",
					BindingFlags.Public | BindingFlags.InvokeMethod,
					null, slid, new object[] { slidPhotoFile, "jpg" }, null);
					outputFiles.Add(slidPhotoFile);
					NAR(slid);
				}

				NAR(p);
				p = null;

				NAR(slides);
				slides = null;


				t2.InvokeMember("Close",
					BindingFlags.Public | BindingFlags.InvokeMethod,
					null, ppt, null, null);

				NAR(ppt);
				ppt = null;

				t.InvokeMember(
					"Quit",
					BindingFlags.Public | BindingFlags.InvokeMethod,
					null, o, null, null);

				NAR(o);
				o = null;

				GC.Collect();

				return outputFiles;
			}

		}
	}
}
