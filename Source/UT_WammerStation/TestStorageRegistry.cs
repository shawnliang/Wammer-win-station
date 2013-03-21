using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waveface.Stream.Core;

namespace UT_WammerStation
{
	[TestClass]
	public class TestStorageRegistry
	{
		[TestMethod]
		public void saveAndQuery()
		{
			StorageRegistry.Save("user_id1", "path");

			string location = StorageRegistry.QueryStorageLocation("user_id1");
			Assert.AreEqual("path", location);
		}

		[TestMethod]
		public void saveOverride()
		{
			StorageRegistry.Save("user_id1", "path");
			StorageRegistry.Save("user_id1", "overriden");

			string location = StorageRegistry.QueryStorageLocation("user_id1");
			Assert.AreEqual("overriden", location);
		}

		[TestMethod]
		public void testClearAll()
		{
			StorageRegistry.Save("user1", "path1");
			StorageRegistry.Save("user2", "path2");

			StorageRegistry.ClearAll();
			StorageRegistry.ClearAll();
			Assert.AreEqual(null, StorageRegistry.QueryStorageLocation("user1"));
		}

		[TestMethod]
		public void testRemove()
		{
			StorageRegistry.Save("user1", "p1");
			StorageRegistry.Save("user2", "p2");

			StorageRegistry.Remove("user1");
			Assert.AreEqual(null, StorageRegistry.QueryStorageLocation("user1"));
			Assert.AreEqual("p2", StorageRegistry.QueryStorageLocation("user2"));
		}
	}
}
