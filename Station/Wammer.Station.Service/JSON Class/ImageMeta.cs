// JSON C# Class Generator
// http://at-my-window.blogspot.com/?page=json-class-generator

using System;
using Newtonsoft.Json.Linq;
using JsonCSharpClassGenerator;

namespace Wammer.Station.JSONClass
{

    internal class ImageMeta
    {

        private JObject __jobject;
        public ImageMeta(JObject obj)
        {
            this.__jobject = obj;
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Large _large;
        public Large Large
        {
            get
            {
                if(_large == null)
                    _large = JsonClassHelper.ReadStronglyTypedObject<Large>(JsonClassHelper.GetJToken<JObject>(__jobject, "large"));
                return _large;
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Small _small;
        public Small Small
        {
            get
            {
                if(_small == null)
                    _small = JsonClassHelper.ReadStronglyTypedObject<Small>(JsonClassHelper.GetJToken<JObject>(__jobject, "small"));
                return _small;
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Medium _medium;
        public Medium Medium
        {
            get
            {
                if(_medium == null)
                    _medium = JsonClassHelper.ReadStronglyTypedObject<Medium>(JsonClassHelper.GetJToken<JObject>(__jobject, "medium"));
                return _medium;
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Square _square;
        public Square Square
        {
            get
            {
                if(_square == null)
                    _square = JsonClassHelper.ReadStronglyTypedObject<Square>(JsonClassHelper.GetJToken<JObject>(__jobject, "square"));
                return _square;
            }
        }

    }
}
