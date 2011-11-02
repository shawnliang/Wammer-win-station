using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Bson;
using MongoDB.Driver;
using Wammer.Utility;

using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization;

namespace UT_WammerStation
{
	[TestClass]
	public class TestBsonHelper
	{
		//static MongoServer mongodb;

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			//mongodb = MongoServer.Create("mongodb://localhost:10319/?safe=true");
		}

		[TestMethod]
		public void TestSimpleMerge()
		{
			BsonDocument a = new BsonDocument{
						{"a", 1},
						{"b", 2},
			};

			BsonDocument b = new BsonDocument{
						{"d", 3},
						{"e", 4},
			};

			BsonDocument res = a.Merge(b);
			Assert.IsTrue(res["a"] == 1);
			Assert.IsTrue(res["b"] == 2);
			Assert.IsTrue(res["d"] == 3);
			Assert.IsTrue(res["e"] == 4);
			Assert.AreEqual(4, res.ElementCount);
			
		}

		[TestMethod]
		public void TestMerge_Override()
		{
			BsonDocument a = new BsonDocument{
						{"a", 1},
						{"b", 2},
			};

			BsonDocument b = new BsonDocument{
						{"a", 3},
						{"c", 4},
			};

			BsonDocument res = a.Merge(b, true);
			Assert.IsTrue(res["a"] == 3);
			Assert.IsTrue(res["b"] == 2);
			Assert.IsTrue(res["c"] == 4);
			Assert.AreEqual(3, res.ElementCount);
		}

		[TestMethod]
		public void TestMerge_Complex()
		{
			BsonDocument a = new BsonDocument{
						{"a", 1},
						{"b", 2},
			};

			BsonDocument b = new BsonDocument{
						{"b", new BsonDocument { {"aa", 10}, {"bb", 20 }}},
			};

			BsonDocument res = a.Merge(b, true);
			Assert.IsTrue(res["a"] == 1);
			Assert.IsTrue(res["b"].Equals(new BsonDocument { { "aa", 10 }, { "bb", 20 } }));
			Assert.AreEqual(2, res.ElementCount);
		}

		[TestMethod]
		public void TestMerge_NestedComplex()
		{
			BsonDocument a = new BsonDocument{
						{"a", 1},
						{"b", new BsonDocument { {"a", 10}, {"b", 20 }}},
			};

			BsonDocument b = new BsonDocument{
						{"b", new BsonDocument { {"aa", 10}, {"bb", 20 }}},
			};

			BsonDocument res = a.DeepMerge(b);
			Assert.IsTrue(res["a"] == 1);
			Assert.IsTrue(res["b"].Equals(new BsonDocument { 
				{ "a", 10},
				{ "b", 20},
				{ "aa", 10 }, 
				{ "bb", 20 },
				})
				);
			Assert.AreEqual(2, res.ElementCount);
		}

		[TestMethod]
		public void TestMerge_NestedComplex2()
		{
			BsonDocument a = new BsonDocument{
						{"a", 1},
						{"b", new BsonDocument { {"a", 10}, {"b", 20 }}},
			};

			BsonDocument b = new BsonDocument{
						{"b", 2},
			};

			BsonDocument res = a.DeepMerge(b);
			Assert.IsTrue(res["a"] == 1);
			Assert.IsTrue(res["b"] == 2);
			Assert.AreEqual(2, res.ElementCount);
			Assert.AreEqual(res, a);
		}

		[TestMethod]
		public void testMixedMerge()
		{
			Wammer.Cloud.Attachment a = new Wammer.Cloud.Attachment
			{
				type = Wammer.Cloud.AttachmentType.image,
				object_id = "1234567890",
				image_meta = new Wammer.Cloud.ImageProperty
				{
					small = new Wammer.Cloud.ThumbnailInfo
					{
						url = "http://localhost/"
					}
				}
			};

			Wammer.Cloud.Attachment b = new Wammer.Cloud.Attachment
			{
				type = Wammer.Cloud.AttachmentType.image,
				title = "this is title",
				description = "this is description"
			};

			BsonDocument doc = b.ToBsonDocument();
			BsonDocument update = a.ToBsonDocument();
			doc.DeepMerge(update);

			string bb = update.ToString();
			
			Assert.AreEqual("this is title", doc["title"].AsString);
			Assert.AreEqual("this is description", doc["description"].AsString);
			Assert.AreEqual("http://localhost/", doc["image_meta"]
				.AsBsonDocument["small"].AsBsonDocument["url"].AsString);

			Assert.AreEqual("1234567890", doc["_id"].AsString);
		}
	}
}
