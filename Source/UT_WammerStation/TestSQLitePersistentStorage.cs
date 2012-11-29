using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SQLite;
using Wammer.Queue;

namespace UT_WammerStation
{
	class ComplexData
	{
		public int a;
		public int b;
		public string c;
		public byte[] d;

		public ComplexData()
		{
			a = 1000;
			b = 2000;
			c = "1234567";
			d = new byte[3];
			d[0] = (byte)10;
			d[1] = (byte)10;
			d[2] = (byte)10;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is ComplexData)
			{
				ComplexData rhs = (ComplexData)obj;

				if (rhs.a == a && rhs.b == b && rhs.c == c && rhs.d.Length == d.Length)
				{
					for (int i = 0; i < d.Length; i++)
						if (d[i] != rhs.d[i])
							return false;

					return true;
				}
				else
					return false;
			}
			else
				return false;
		}
	}


	[TestClass]
	public class TestSQLitePersistentStorage
	{
		private SQLitePersistentStorage storage;

		[TestInitialize]
		public void SetUp()
		{
			if (File.Exists("file.db"))
				File.Delete("file.db");

			storage = new SQLitePersistentStorage("file.db");	
		}

		[TestCleanup]
		public void TearDown()
		{
			storage.Close();
		}
		[TestMethod]
		public void TestEmptyQueue()
		{
			WMSBroker b = new WMSBroker(storage);
			WMSSession s = b.CreateSession();
			WMSQueue q1 = b.GetQueue("q1");
			Assert.AreEqual(0, q1.Count);
		}

		[TestMethod]
		public void TestSaveOneItem()
		{
			WMSBroker b = new WMSBroker(storage);
			WMSSession s = b.CreateSession();
			WMSQueue q1 = b.GetQueue("qqq");

			s.Push(q1, "1234567", true);
			s.Close();

			WMSBroker b2 = new WMSBroker(storage);
			WMSSession s2 = b2.CreateSession();
			WMSQueue q2 = b.GetQueue("qqq");
			Assert.AreEqual(1, q2.Count);
			WMSMessage m = s2.Pop(q2);
			Assert.AreEqual("1234567", (string)m.Data);
		}

		[TestMethod]
		public void TestSavedItemsAreReloadedByOrigOrder()
		{
			WMSBroker b = new WMSBroker(storage);
			WMSSession s = b.CreateSession();
			WMSQueue q1 = b.GetQueue("qqq");

			s.Push(q1, "a", true);
			s.Push(q1, "b", true);
			s.Push(q1, "c", true);
			s.Push(q1, "d", true);
			s.Push(q1, "e", true);
			s.Close();

			WMSBroker b2 = new WMSBroker(storage);
			WMSSession s2 = b2.CreateSession();
			WMSQueue q2 = b.GetQueue("qqq");
			
			Assert.AreEqual(5, q2.Count);
			Assert.AreEqual("a", (string)s2.Pop(q2).Data);
			Assert.AreEqual("b", (string)s2.Pop(q2).Data);
			Assert.AreEqual("c", (string)s2.Pop(q2).Data);
			Assert.AreEqual("d", (string)s2.Pop(q2).Data);
			Assert.AreEqual("e", (string)s2.Pop(q2).Data);
		}

		[TestMethod]
		public void TestSaveComplexData()
		{
			WMSBroker b = new WMSBroker(storage);
			WMSSession s = b.CreateSession();
			WMSQueue q1 = b.GetQueue("qqq");

			s.Push(q1, new ComplexData(), true);
			s.Close();

			WMSBroker b2 = new WMSBroker(storage);
			WMSSession s2 = b2.CreateSession();
			WMSQueue q2 = b.GetQueue("qqq");
			Assert.AreEqual(1, q2.Count);
			WMSMessage m = s2.Pop(q2);
			Assert.AreEqual(new ComplexData(), (ComplexData)m.Data);
		}

		[TestMethod]
		public void TestRemoveFromStorage()
		{
			WMSBroker b = new WMSBroker(storage);
			WMSSession s = b.CreateSession();
			WMSQueue q1 = b.GetQueue("qqq");

			s.Push(q1, new ComplexData(), true);
			WMSMessage m = s.Pop(q1);
			Assert.IsNotNull(m);

			m.Acknowledge();
			s.Close();


			WMSBroker b2 = new WMSBroker(storage);
			WMSSession s2 = b2.CreateSession();
			WMSQueue q2 = b.GetQueue("qqq");
			Assert.AreEqual(0, q2.Count);
			
		}
	}
}


