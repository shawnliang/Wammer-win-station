using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT_WammerStation
{
	class clsA
	{
		public event EventHandler evt1;
		
		public void Do()
		{
			evt1(this, null);
		}
	}


	[TestClass]
	public class EventTest
	{
		private bool handler1IsCalled;
		private bool handler2IsCalled;

		[TestInitialize]
		public void SetUp()
		{
			handler1IsCalled = handler2IsCalled = false;
		}

		[TestMethod]
		public void TestMethod1()
		{
			clsA a = new clsA();
			a.evt1 += new EventHandler(a_evt1);
			a.evt1 += new EventHandler(a_evt2);

			a.Do();

			Assert.IsTrue(handler1IsCalled);
			Assert.IsTrue(handler2IsCalled);
		}

		void a_evt1(object sender, EventArgs e)
		{
			handler1IsCalled = true;
		}
		void a_evt2(object sender, EventArgs e)
		{
			handler2IsCalled = true;
		}

	}
}
