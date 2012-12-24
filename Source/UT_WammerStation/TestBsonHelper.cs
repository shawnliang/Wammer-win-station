using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using System.Collections.Generic;
using Wammer.Utility;
using Waveface.Stream.Model;


namespace UT_WammerStation
{
	public class MyClass2
	{
		public List<MyClass3> Array { get; set; }
	}
	public class MyClass3
	{
		public string Data { get; set; }
	}

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
			Attachment a = new Attachment
			{
				type = AttachmentType.image,
				object_id = "1234567890",
				image_meta = new ImageProperty
				{
					small = new ThumbnailInfo
					{
						url = "http://localhost/"
					}
				}
			};

			Attachment b = new Attachment
			{
				type = AttachmentType.image,
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


		[TestMethod]
		public void TestBsonListSerialization()
		{
			MyClass2 a = new MyClass2
			{
				Array = new List<MyClass3>()
			};

			a.Array.Add(new MyClass3 { Data = "123" });
			a.Array.Add(new MyClass3 { Data = "456" });

			BsonDocument doc = a.ToBsonDocument();
			Assert.IsTrue(doc["Array"].IsBsonArray);
			Assert.AreEqual("123", doc["Array"].AsBsonArray[0].AsBsonDocument["Data"].AsString);
			Assert.AreEqual("456", doc["Array"].AsBsonArray[1].AsBsonDocument["Data"].AsString);
			Assert.AreEqual(2, doc["Array"].AsBsonArray.Count);
		}
	}
}
