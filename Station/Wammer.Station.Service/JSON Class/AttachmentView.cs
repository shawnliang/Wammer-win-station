// JSON C# Class Generator
// http://at-my-window.blogspot.com/?page=json-class-generator

using JsonCSharpClassGenerator;
using Newtonsoft.Json.Linq;

namespace Wammer.Station.JSONClass
{

    internal class AttachmentView
    {

        public AttachmentView(string json)
         : this(JObject.Parse(json))
        {
        }

        private JObject __jobject;
        public AttachmentView(JObject obj)
        {
            this.__jobject = obj;
        }

        public int Status
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "status"));
            }
        }

        public string SessionToken
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "session_token"));
            }
        }

        public string SessionExpires
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "session_expires"));
            }
        }

        public string RedirectTo
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "redirect_to"));
            }
        }

        public int ApiRetCode
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "api_ret_code"));
            }
        }

        public string ApiRetMessage
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "api_ret_message"));
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Debug _debug;
        public Debug Debug
        {
            get
            {
                if(_debug == null)
                    _debug = JsonClassHelper.ReadStronglyTypedObject<Debug>(JsonClassHelper.GetJToken<JObject>(__jobject, "debug"));
                return _debug;
            }
        }

        public string FileName
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "file_name"));
            }
        }

        public string MetaStatus
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "meta_status"));
            }
        }

        public int Height
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "height"));
            }
        }

        public int FileSize
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "file_size"));
            }
        }

        public int Loc
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "loc"));
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Exif _exif;
        public Exif Exif
        {
            get
            {
                if(_exif == null)
                    _exif = JsonClassHelper.ReadStronglyTypedObject<Exif>(JsonClassHelper.GetJToken<JObject>(__jobject, "exif"));
                return _exif;
            }
        }

        public string Title
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "title"));
            }
        }

        public string ObjectId
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "object_id"));
            }
        }

        public int Width
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "width"));
            }
        }

        public bool DefaultPost
        {
            get
            {
                return JsonClassHelper.ReadBoolean(JsonClassHelper.GetJToken<JValue>(__jobject, "default_post"));
            }
        }

        public string Hidden
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "hidden"));
            }
        }

        public string DeviceId
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "device_id"));
            }
        }

        public string Type
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "type"));
            }
        }

        public string MimeType
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "mime_type"));
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Gps _gps;
        public Gps Gps
        {
            get
            {
                if(_gps == null)
                    _gps = JsonClassHelper.ReadStronglyTypedObject<Gps>(JsonClassHelper.GetJToken<JObject>(__jobject, "gps"));
                return _gps;
            }
        }

        public int MetaTime
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "meta_time"));
            }
        }

        public string Description
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "description"));
            }
        }

        public string CodeName
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "code_name"));
            }
        }

        public string Md5
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "md5"));
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Places _places;
        public Places Places
        {
            get
            {
                if(_places == null)
                    _places = JsonClassHelper.ReadStronglyTypedObject<Places>(JsonClassHelper.GetJToken<JObject>(__jobject, "places"));
                return _places;
            }
        }

        public string Url
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "url"));
            }
        }

        public string CreatorId
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "creator_id"));
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private ImageMeta _imageMeta;
        public ImageMeta ImageMeta
        {
            get
            {
                if(_imageMeta == null)
                    _imageMeta = JsonClassHelper.ReadStronglyTypedObject<ImageMeta>(JsonClassHelper.GetJToken<JObject>(__jobject, "image_meta"));
                return _imageMeta;
            }
        }

        public int ModifyTime
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "modify_time"));
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private object[] _faces;
        public object[] Faces
        {
            get
            {
                if(_faces == null)
                    _faces = (object[])JsonClassHelper.ReadArray<object>(JsonClassHelper.GetJToken<JArray>(__jobject, "faces"), JsonClassHelper.ReadObject, typeof(object[]));
                return _faces;
            }
        }

        public string GroupId
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "group_id"));
            }
        }

    }
}
