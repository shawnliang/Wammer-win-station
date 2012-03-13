using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace UT_WammerStation
{
	[TestClass]
	public class TestMongoPersistentStorage
	{
		private MongoDatabase db;

		[TestInitialize]
		public void SetUp()
		{
			db = MongoDatabase.Create("mongodb://127.0.0.1/wammer?safe=true");
			db.DropCollection("")
		}

		[TestMethod]
		public void TestMethod1()
		{
		}
	}
}
