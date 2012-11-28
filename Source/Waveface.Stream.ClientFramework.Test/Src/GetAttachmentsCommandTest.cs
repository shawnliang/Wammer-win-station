using Waveface.Stream.ClientFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Waveface.Stream.ClientFramework.Test
{
    
    
    /// <summary>
    ///This is a test class for GetAttachmentsCommandTest and is intended
    ///to contain all GetAttachmentsCommandTest Unit Tests
    ///</summary>
	[TestClass()]
    public class GetAttachmentsCommandTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for Execute
		///</summary>
		[TestMethod()]
		[Ignore]
		public void ExecuteTest()
		{
            GetAttachmentsCommand target = new GetAttachmentsCommand(); // TODO: Initialize to an appropriate value
            Dictionary<string, object> parameters = new Dictionary<string, object>();

			target.Execute(parameters);

			Assert.IsNotNull(target.Value);
		}
	}
}
