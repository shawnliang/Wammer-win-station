using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace UT_WammerStation
{
	interface Animal
	{

	}

	[Serializable]
	class Cat : Animal
	{
	}

	[Serializable]
	class Dog : Animal
	{

	}

	[Serializable]
	class Lion : Cat
	{

	}

	[TestClass]
	public class TestPersistentQueue
	{
		[TestMethod]
		public void TestPolymophism()
		{
			Animal dog = new Dog();
			Animal lion = new Lion();
			Animal cat = new Cat();

			MemoryStream m = new MemoryStream();
			BinaryFormatter s = new BinaryFormatter();
			s.Serialize(m, lion);
			m.Position = 0;

			Animal lion2 = s.Deserialize(m) as Animal;
			Assert.IsTrue(lion2 is Lion);
		}
	}
}
