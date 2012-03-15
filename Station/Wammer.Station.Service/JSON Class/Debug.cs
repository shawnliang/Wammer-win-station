// JSON C# Class Generator
// http://at-my-window.blogspot.com/?page=json-class-generator

using System;
using Newtonsoft.Json.Linq;
using JsonCSharpClassGenerator;

namespace Wammer.Station.JSONClass
{

    internal class Debug
    {

        private JObject __jobject;
        public Debug(JObject obj)
        {
            this.__jobject = obj;
        }

        public string ConnectionId
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "connection_id"));
            }
        }

    }
}
