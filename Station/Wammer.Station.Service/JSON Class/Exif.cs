// JSON C# Class Generator
// http://at-my-window.blogspot.com/?page=json-class-generator

using JsonCSharpClassGenerator;
using Newtonsoft.Json.Linq;

namespace Wammer.Station.JSONClass
{

    internal class Exif
    {

        private JObject __jobject;
        public Exif(JObject obj)
        {
            this.__jobject = obj;
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int[] _yResolution;
        public int[] YResolution
        {
            get
            {
                if(_yResolution == null)
                    _yResolution = (int[])JsonClassHelper.ReadArray<int>(JsonClassHelper.GetJToken<JArray>(__jobject, "YResolution"), JsonClassHelper.ReadInteger, typeof(int[]));
                return _yResolution;
            }
        }

        public int ResolutionUnit
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "ResolutionUnit"));
            }
        }

        public int ExifImageWidth
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "ExifImageWidth"));
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int[] _focalLength;
        public int[] FocalLength
        {
            get
            {
                if(_focalLength == null)
                    _focalLength = (int[])JsonClassHelper.ReadArray<int>(JsonClassHelper.GetJToken<JArray>(__jobject, "FocalLength"), JsonClassHelper.ReadInteger, typeof(int[]));
                return _focalLength;
            }
        }

        public int ExifInteroperabilityOffset
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "ExifInteroperabilityOffset"));
            }
        }

        public string Make
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "Make"));
            }
        }

        public int ColorSpace
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "ColorSpace"));
            }
        }

        public string FlashPixVersion
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "FlashPixVersion"));
            }
        }

        public string ComponentsConfiguration
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "ComponentsConfiguration"));
            }
        }

        public int ExifOffset
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "ExifOffset"));
            }
        }

        public int ExifImageHeight
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "ExifImageHeight"));
            }
        }

        public int YCbCrPositioning
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "YCbCrPositioning"));
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int[] _xResolution;
        public int[] XResolution
        {
            get
            {
                if(_xResolution == null)
                    _xResolution = (int[])JsonClassHelper.ReadArray<int>(JsonClassHelper.GetJToken<JArray>(__jobject, "XResolution"), JsonClassHelper.ReadInteger, typeof(int[]));
                return _xResolution;
            }
        }

        public string DateTimeOriginal
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "DateTimeOriginal"));
            }
        }

        public int ISOSpeedRatings
        {
            get
            {
                return JsonClassHelper.ReadInteger(JsonClassHelper.GetJToken<JValue>(__jobject, "ISOSpeedRatings"));
            }
        }

        public string Model
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "Model"));
            }
        }

        public string ExifVersion
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "ExifVersion"));
            }
        }

        public string DateTimeDigitized
        {
            get
            {
                return JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(__jobject, "DateTimeDigitized"));
            }
        }

    }
}
