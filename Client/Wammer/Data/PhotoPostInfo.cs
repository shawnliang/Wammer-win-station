#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NLog;
using Newtonsoft.Json;
using Waveface.API.V2;
using System.Net;

#endregion

namespace Waveface
{
    class PhotoPostInfo
    {
        public string post_id { get; set; }
		/// <summary>
		/// key: object_id, value: source file path
		/// </summary>
        public Dictionary<string, string> sources { get; set; }

        public PhotoPostInfo()
        {
            sources = new Dictionary<string, string>();
        }
    }

}