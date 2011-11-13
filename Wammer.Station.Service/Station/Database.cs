using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public class Database
	{
		public static MongoDB.Driver.MongoServer mongodb;

		static Database()
		{
			mongodb = MongoDB.Driver.MongoServer.Create(
									string.Format("mongodb://localhost:{0}/?safe=true",
									StationRegistry.GetValue("dbPort", 10319))); // TODO: Remove Hard code
		}
	}
}
