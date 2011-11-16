using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wammer.Station;

namespace Wammer.Model
{
	public class Database
	{
		public static MongoDB.Driver.MongoServer mongodb;
		public static MongoDB.Driver.MongoDatabase wammer;

		static Database()
		{
			mongodb = MongoDB.Driver.MongoServer.Create(
									string.Format("mongodb://localhost:{0}/?safe=true",
									StationRegistry.GetValue("dbPort", 10319))); // TODO: Remove Hard code
			wammer = mongodb.GetDatabase("wammer");
		}
	}
}
