using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Wammer.Utility;

namespace UT_WammerStation
{
	[TestClass]
	public class TestStreamHelper
	{
		private bool copyDoneIsCalled;


		[TestInitialize]
		public void Setup()
		{
			copyDoneIsCalled = false;
		}

		[TestMethod]
		public void TestCopy()
		{
			FileStream from = File.OpenRead("penguins.jpg");
			FileStream to = File.Open("dest.jpg", FileMode.Create);

			IAsyncResult result = StreamHelper.BeginCopy(from, to, CopyDone, from);
			result.AsyncWaitHandle.WaitOne();

			Assert.IsTrue(copyDoneIsCalled);

			Assert.AreEqual(to.Length, from.Length);
			from.Position = to.Position = 0;

			for (int i = 0; i < to.Length; i++)
				Assert.AreEqual(to.ReadByte(), from.ReadByte());
		}

		private void CopyDone(IAsyncResult ar)
		{
			copyDoneIsCalled = true;
			StreamHelper.EndCopy(ar);
		}
	}
}
