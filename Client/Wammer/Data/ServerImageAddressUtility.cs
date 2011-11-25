
using System;
using System.Drawing;
using System.Net;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;

namespace Waveface
{
    public class ServerImageAddressUtility
    {
        public static string attachments_getRedirectURL(string orgURL, string session_token, string object_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            object_id = HttpUtility.UrlEncode(object_id);

            string _url = BEService2.HostIP + orgURL;
            string _a_s = "&" + "apikey" + "=" + BEService2.APIKEY + "&" + "session_token" + "=" + session_token;

            if (orgURL == string.Empty)
            {
                _url += "/v2/attachments/view?object_id=" + object_id + _a_s + "&image_meta=medium";
            }
            else
            {
                _url += _a_s;
            }

            return _url;
        }

        public static string attachments_getRedirectURL_Image(string session_token, Attachment a, string imageType, out string url, out string fileName)
        {
            const string SMALL = "small";
            const string MEDIUM = "medium";
            const string ORIGIN = "origin";

            string object_id = a.object_id;
            string _imageType = imageType;

            session_token = HttpUtility.UrlEncode(session_token);
            object_id = HttpUtility.UrlEncode(object_id);

            string _url = BEService2.HostIP + "/v2/attachments/view?object_id=" + object_id + "&" +
                            "apikey" + "=" + BEService2.APIKEY + "&" +
                            "session_token" + "=" + session_token;

            if(imageType == SMALL)
            {
                if (a.image_meta.small != null)
                {
                    _imageType = SMALL;
                }
                else
                {
                    _imageType = MEDIUM;
                }
            }

            if (imageType == ORIGIN)
            {
                if (a.url != "")
                {
                    _imageType = ORIGIN;

                    url = _url;
                    fileName = a.object_id + "_"  + a.image_meta.medium.file_name;

                    return _imageType;
                }
                else
                {
                    _imageType = MEDIUM;
                }
            }

            url = _url + "&" + "image_meta=" + _imageType;
            fileName = a.object_id + "_" + _imageType + "_" + a.image_meta.medium.file_name;
            return _imageType;
        }

        public static bool CheckURL(string url)
        {
            //Send web request
            Uri _urlCheck = new Uri(url);
            HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(_urlCheck);
            _request.Timeout = 1500;

            HttpWebResponse _response;

            try
            {
                //get response
                _response = (HttpWebResponse)_request.GetResponse();
            }
            catch (Exception)
            {
                //403 or couldn't connect to internet
                Console.WriteLine("Could not connect to the internet");
                return false; //can't connect to url
            }

            //Return status
            return _response.StatusCode == HttpStatusCode.Found; //200 = all ok
        }
    }
}
