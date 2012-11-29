using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Waveface.Stream.WindowsClient
{
    static class StationDB
    {
        private static MongoServer server;
        private static MongoDatabase wammerDB;

        private static Dictionary<string, MongoCollection<BsonDocument>> collections = new Dictionary<string, MongoCollection<BsonDocument>>();

        static StationDB()
        {
            server = MongoServer.Create("mongodb://127.0.0.1:10319/?safe=true");
            wammerDB = server.GetDatabase("wammer");
        }

        public static MongoCollection<BsonDocument> GetCollection(string name)
        {
            lock (collections)
            {
                if (!collections.ContainsKey(name))
                    collections.Add(name, wammerDB.GetCollection(name));

                return collections[name];
            }
        }
    }
}
