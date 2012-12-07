using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.Doc;
using System.IO;

namespace UT_WammerStation.Doc
{
	[TestClass]
	public class GeneratePptPreviewTest
	{
		[TestInitialize]
		public void Setup()
		{
			if (Directory.Exists("preview"))
				Directory.Delete("preview", true);

			Directory.CreateDirectory("preview");
		}

		[TestCleanup]
		public void TearDown()
		{
			if (Directory.Exists("preview"))
				Directory.Delete("preview", true);
		}

		[TestMethod]
		[Ignore]
		public void GeneratePptPreviews()
		{
			var previews = DocumentChangeMonitorUtil.GeneratePowerPointPreviews(@"ppt.pptx", "preview").ToList();
			Assert.AreEqual(4, previews.Count);

			Assert.AreEqual(@"preview\00000001.jpg", previews[0]);
			Assert.AreEqual(@"preview\00000002.jpg", previews[1]);
			Assert.AreEqual(@"preview\00000003.jpg", previews[2]);
			Assert.AreEqual(@"preview\00000004.jpg", previews[3]);
		}

		[TestMethod]
		public void GeneratePDFPreviews()
		{
			var previews = DocumentChangeMonitorUtil.GeneratePdfPreviews(@"pdf.pdf", "preview").ToList();
			Assert.AreEqual(4, previews.Count);

			Assert.AreEqual(@"preview\00000001.jpg", previews[0]);
			Assert.AreEqual(@"preview\00000002.jpg", previews[1]);
			Assert.AreEqual(@"preview\00000003.jpg", previews[2]);
			Assert.AreEqual(@"preview\00000004.jpg", previews[3]);
		}
	}
}
