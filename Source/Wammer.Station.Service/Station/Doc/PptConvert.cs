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

				object ppt = null;
				object slides = null;
				object p = null;
				Type t2 = null;
				var outputFiles = new List<string>();

				try
				{
					ppt = t.InvokeMember(
						"ActivePresentation",
						BindingFlags.Public | BindingFlags.GetProperty,
						null, o, null, null);

					t2 = ppt.GetType();
					var fullName = t2.InvokeMember(
						"FullName",
						BindingFlags.Public | BindingFlags.GetProperty,
						null, ppt, null, null).ToString();

					if (!Path.GetFileName(fullName).Equals(Path.GetFileName(file), StringComparison.CurrentCultureIgnoreCase))
						throw new Exception();

					slides = t2.InvokeMember(
						"Slides",
						BindingFlags.Public | BindingFlags.GetProperty,
						null, ppt, null, null);

					ConvertSlidsToJpg(outputPath, slides, outputFiles);
				}
				catch (Exception)
				{
					p = t.InvokeMember(
						"Presentations",
						BindingFlags.Public | BindingFlags.GetProperty,
						null, o, null, null);

					t2 = p.GetType();
					ppt = t2.InvokeMember("Open",
						BindingFlags.Public | BindingFlags.InvokeMethod,
						null, p, new object[] { file, 0, 0, 0 }, null);

					slides = t2.InvokeMember(
						"Slides",
						BindingFlags.Public | BindingFlags.GetProperty,
						null, ppt, null, null);

					ConvertSlidsToJpg(outputPath, slides, outputFiles);

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
				}
				return outputFiles;
			}

		}

		private static void ConvertSlidsToJpg(string outputPath, object slides, List<string> outputFiles)
		{
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
			}
		}
	}
}
