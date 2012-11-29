using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
    [BsonIgnoreExtraElements]
    public class Person
    {
        [BsonIgnoreIfNull]
        public string name { get; set; }

        [BsonIgnoreIfNull]
        public string avatar { get; set; }
    }
}
