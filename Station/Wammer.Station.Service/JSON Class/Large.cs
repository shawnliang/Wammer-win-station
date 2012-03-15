// JSON C# Class Generator
// http://at-my-window.blogspot.com/?page=json-class-generator

using JsonCSharpClassGenerator;
using Newtonsoft.Json.Linq;

namespace Wammer.Station.JSONClass
{

    internal class Large
    {

        private JObject __jobject;
        public Large(JObject obj)
        {
            this.__jobject = obj;
        }

        public string Url
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "url"));
            }
        }

        public int Height
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "height"));
            }
        }

        public int Width
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "width"));
            }
        }

        public int ModifyTime
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "modify_time"));
            }
        }

        public int FileSize
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "file_size"));
            }
        }

        public string MimeType
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "mime_type"));
            }
        }

        public string Md5
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "md5"));
            }
        }

    }
}
