#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using NLog;
using Newtonsoft.Json;
using System.Windows.Forms;

#endregion

namespace Waveface.API.V2
{
    public class WService
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        public static string APIKEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";
        public const string DEF_BASE_URL = "https://develop.waveface.com/v2/"; // https://api.waveface.com/v2/

        #region Properties

#if DEBUG
        public static string StationIP
        {
            get { return "http://127.0.0.1:9981"; }
            set { }
        }
#else
        public static string StationIP { get; set; }
#endif

        public static string CloudIP
        {
            get
            {
                //return "https://develop.waveface.com";
                //return "https://api.waveface.com";

                Uri _url = new Uri(CloudBaseURL);

                return (_url.Scheme + "://" + _url.Host + ":" + _url.Port);
            }
        }

        public static string HostIP
        {
            get { return string.IsNullOrEmpty(StationIP) ? CloudIP : StationIP; }
        }

        public static string BaseURL
        {
            get { return HostIP + "/v2"; }
        }

        public static string BaseURLForGroupUserAuth
        {
            get { return CloudIP + "/v2"; }
        }

        public static string CloudBaseURL
        {
            get { return (string)StationRegHelper.GetValue("cloudBaseURL", DEF_BASE_URL); }
        }

        public static string WebURL
        {
            get
            {
                if (CloudBaseURL.Contains("api.waveface.com"))
                    return "https://waveface.com";
                else if (CloudBaseURL.Contains("develop.waveface.com"))
                    return "https://devweb.waveface.com";
                else if (CloudBaseURL.Contains("staging.waveface.com"))
                    return "http://staging.waveface.com";
                else
                    return "https://waveface.com";
            }
        }

        public static string UpdateURL
        {
            get
            {
                if (CloudBaseURL.Contains("develop.waveface.com"))
                    return "https://develop.waveface.com";
                else
                    return WebURL;
            }
        }
        #endregion

        #region Misc

        private T HttpGetObject<T>(string _url)
        {
            HttpWebRequest _req = (HttpWebRequest)WebRequest.Create(_url);
            _req.Timeout = 30000;
            _req.Headers.Set("Content-Encoding", "UTF-8");
            _req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            _req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            WebResponse _resp = _req.GetResponse();
            StreamReader _sr = new StreamReader(_resp.GetResponseStream());
            string _r = _sr.ReadToEnd().Trim();

            JsonSerializerSettings _settings = new JsonSerializerSettings();
            _settings.MissingMemberHandling = MissingMemberHandling.Ignore;

            return JsonConvert.DeserializeObject<T>(_r, _settings);
        }

        public string HttpGet(string _url, int timeout, bool useWarnLog)
        {
            try
            {
                HttpWebRequest _req = (HttpWebRequest)WebRequest.Create(_url);
                _req.Timeout = timeout;
                _req.Headers.Set("Content-Encoding", "UTF-8");
                _req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                _req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                WebResponse _resp = _req.GetResponse();
                StreamReader _sr = new StreamReader(_resp.GetResponseStream());
                return _sr.ReadToEnd().Trim();
            }
            catch (WebException _e)
            {
                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    return "OK";
                }

                NLogUtility.WebException(s_logger, _e, "HttpGet", useWarnLog);

                return null;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "HttpGet");

                return null;
            }
        }

        #endregion

        #region auth

        public MR_auth_signup auth_signup(string email, string password, string nickname, string avatar_url)
        {
            MR_auth_signup _ret;

            email = email.Replace("@", "%40");
            password = HttpUtility.UrlEncode(password);
            nickname = HttpUtility.UrlEncode(nickname);
            avatar_url = HttpUtility.UrlEncode(avatar_url);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/auth/signup";

                string _parms =
                    "apikey" + "=" + APIKEY + "&" +
                    "email" + "=" + email + "&" +
                    "password" + "=" + password + "&" +
                    "nickname" + "=" + nickname + "&" +
                    "avatar_url" + "=" + avatar_url;

                WebPostHelper _webPos = new WebPostHelper();
                bool _isOK = _webPos.doPost(_url, _parms, null);

                if (!_isOK)
                    return null;

                string _r = _webPos.getContent();

                _ret = JsonConvert.DeserializeObject<MR_auth_signup>(_r);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "auth_signup", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "auth_signup");

                throw;
            }

            return _ret;
        }

        public MR_auth_login auth_login(string email, string password)
        {
            email = HttpUtility.UrlEncode(email);
            password = HttpUtility.UrlEncode(password);

            MR_auth_login _ret;

            try
            {
                string _deviceInfo = string.Empty;

                // Hack
                if(Environment.GetCommandLineArgs().Length == 1)
                {
                    string _device_id = HttpUtility.UrlEncode(StationRegHelper.GetValue("stationId", string.Empty).ToString());
                    string _device_name = HttpUtility.UrlEncode(Environment.MachineName);

                    _deviceInfo = "device_id=" + _device_id + "&device_name=" + _device_name + "&";
                }

                string _url = BaseURL + "/auth/login";

                string _parms = "apikey" + "=" + APIKEY + "&" +
                                "email" + "=" + email + "&" +
                                _deviceInfo +
                                "password" + "=" + password;

                WebPostHelper _webPos = new WebPostHelper();
                bool _isOK = _webPos.doPost(_url, _parms, null);

                if (!_isOK)
                    return null;

                string _r = _webPos.getContent();

                _ret = JsonConvert.DeserializeObject<MR_auth_login>(_r);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "auth_login", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "auth_login");

                throw;
            }

            return _ret;
        }

        public MR_auth_logout auth_logout(string session_token)
        {
            session_token = HttpUtility.UrlEncode(session_token);

            try
            {
                string _url = BaseURL + "/auth/logout";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token;

                return HttpGetObject<MR_auth_logout>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "auth_logout", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "auth_logout");

                throw;
            }
        }

        #endregion

        #region users

        public MR_users_get users_get(string session_token, string user_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            user_id = HttpUtility.UrlEncode(user_id);

            try
            {
                string _url = BaseURL + "/users/get";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "user_id" + "=" + user_id;

                return HttpGetObject<MR_users_get>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "users_get", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "users_get");

                throw;
            }
        }

        public MR_FB_Disconnect SNSDisconnect(string session_token, string sns)
        {
            session_token = HttpUtility.UrlEncode(session_token);

            try
            {
                string _url = BaseURL + "/users/SNSDisconnect";

                _url += "?" +
                        "apikey=" + APIKEY + "&" +
                        "session_token=" + session_token + "&" +
                        "sns=" + sns + "&" +
                        "purge_all= no";

                return HttpGetObject<MR_FB_Disconnect>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "SNSDisconnect", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "SNSDisconnect");

                throw;
            }
        }

        public MR_users_update users_update(string session_token, string user_id, string nickname, string avatar_url)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            user_id = HttpUtility.UrlEncode(user_id);
            nickname = HttpUtility.UrlEncode(nickname);
            avatar_url = HttpUtility.UrlEncode(avatar_url);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/users/update";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "user_id" + "=" + user_id + "&" +
                        "nickname" + "=" + nickname + "&" +
                        "avatar_url" + "=" + avatar_url;

                return HttpGetObject<MR_users_update>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "users_update", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "users_update");

                throw;
            }
        }

        public MR_users_update users_update(string session_token, string user_id, Boolean subscribed)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            user_id = HttpUtility.UrlEncode(user_id);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/users/update";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "user_id" + "=" + user_id + "&" +
                        "subscribed" + "=" + (subscribed ? "yes" : "no");

                return HttpGetObject<MR_users_update>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "users_update", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "users_update");

                throw;
            }
        }

        public MR_users_passwd users_passwd(string session_token, string old_passwd, string new_passwd)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            old_passwd = HttpUtility.UrlEncode(old_passwd);
            new_passwd = HttpUtility.UrlEncode(new_passwd);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/users/passwd";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "old_passwd" + "=" + old_passwd + "&" +
                        "new_passwd" + "=" + new_passwd;

                return HttpGetObject<MR_users_passwd>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "users_passwd", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "users_passwd");

                throw;
            }
        }

        public MR_users_findMyStation users_findMyStation(string session_token)
        {
            session_token = HttpUtility.UrlEncode(session_token);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/users/findMyStation";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token;

                return HttpGetObject<MR_users_findMyStation>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "users_findMyStation", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "users_findMyStation");

                throw;
            }
        }

        public void pingMyStation(string session_token)
        {
            string _url = BaseURLForGroupUserAuth + "/users/pingMyStation";

            _url += "?" +
                    "apikey" + "=" + APIKEY + "&" +
                    "session_token" + "=" + HttpUtility.UrlEncode(session_token);

            General_R response = HttpGetObject<General_R>(_url);

            if (response == null || !response.status.Equals("200") || !response.api_ret_code.Equals("0"))
                throw new Exception("pingMyStation request to cloud is not success");
        }

        #endregion

        #region groups

        public MR_groups_create groups_create(string session_token, string name, string description)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            name = HttpUtility.UrlEncode(name);
            description = HttpUtility.UrlEncode(description);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/groups/create";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "name" + "=" + name + "&" +
                        "description" + "=" + description;

                return HttpGetObject<MR_groups_create>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "groups_create", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "groups_create");

                throw;
            }
        }

        public MR_groups_get groups_get(string session_token, string group_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/groups/get";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id;

                return HttpGetObject<MR_groups_get>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "groups_get", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "groups_get");

                throw;
            }
        }

        public MR_groups_update groups_update(string session_token, string group_id, string name, string description)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            name = HttpUtility.UrlEncode(name);
            description = HttpUtility.UrlEncode(description);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/groups/update";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "name" + "=" + name + "&" +
                        "description" + "=" + description;

                return HttpGetObject<MR_groups_update>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "groups_update", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "groups_update");

                throw;
            }
        }

        public MR_groups_delete groups_delete(string session_token, string group_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/groups/delete";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id;

                return HttpGetObject<MR_groups_delete>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "groups_delete", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "groups_delete");

                throw;
            }
        }

        public MR_groups_inviteUser groups_inviteUser(string session_token, string group_id, string email)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            email = HttpUtility.UrlEncode(email);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/groups/inviteUser";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "email" + "=" + email;

                return HttpGetObject<MR_groups_inviteUser>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "groups_inviteUser", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "groups_inviteUser");

                throw;
            }
        }

        public MR_groups_kickUser groups_kickUser(string session_token, string group_id, string user_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            user_id = HttpUtility.UrlEncode(user_id);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/groups/kickUser";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "user_id" + "=" + user_id;

                return HttpGetObject<MR_groups_kickUser>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "groups_kickUser", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "groups_kickUser");

                throw;
            }
        }

        #endregion

        #region posts

        public MR_posts_getSingle posts_getSingle(string session_token, string group_id, string post_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            post_id = HttpUtility.UrlEncode(post_id);

            try
            {
                string _url = BaseURL + "/posts/getSingle";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "post_id" + "=" + post_id;

                return HttpGetObject<MR_posts_getSingle>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "posts_getSingle", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_getSingle");

                throw;
            }
        }

        public MR_posts_get posts_get(string session_token, string group_id, string limit, string datum, string filter)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            limit = HttpUtility.UrlEncode(limit);
            datum = HttpUtility.UrlEncode(datum);
            filter = HttpUtility.UrlEncode(filter);

            try
            {
                string _url = BaseURL + "/posts/get";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "limit" + "=" + limit + "&" +
                        "datum" + "=" + datum + "&" +
                        "filter" + "=" + filter;

                return HttpGetObject<MR_posts_get>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "posts_get", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_get");

                throw;
            }
        }

        public MR_posts_getLatest posts_getLatest(string session_token, string group_id, string limit)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            limit = HttpUtility.UrlEncode(limit);

            try
            {
                string _url = BaseURL + "/posts/getLatest";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "limit" + "=" + limit;

                return HttpGetObject<MR_posts_getLatest>(_url);
            }
            catch (WebException _e)
            {
                ThrowProperException(_e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_getLatest");

                throw;
            }
        }

        private static void ThrowProperException(WebException _e, string methodName)
        {
            NLogUtility.WebException(s_logger, _e, methodName, false);

            if (_e.Status == WebExceptionStatus.ProtocolError)
            {
                HttpWebResponse _res = (HttpWebResponse)_e.Response;

                if (_res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Station401Exception();
                }
                else
                {
                    using (var reader = new StreamReader(_res.GetResponseStream(), Encoding.UTF8))
                    {
                        var jsonObj = JsonConvert.DeserializeObject<General_R>(reader.ReadToEnd());
                        if (jsonObj.api_ret_code == "999")
                            throw new VersionNotSupportedException("Need to upgrade to latest version in order to proceed");
                        else if (jsonObj.api_ret_code == "45060")
                            throw new ChangeLogsPurgedException();
                    }
                }
            }

            throw _e;
        }

        public MR_posts_new posts_new(string session_token, string post_id, string group_id, string content, string attachment_id_array,
                                      string preview, string type, string coverAttach)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            post_id = HttpUtility.UrlEncode(post_id);
            group_id = HttpUtility.UrlEncode(group_id);
            content = HttpUtility.UrlEncode(content);
            attachment_id_array = HttpUtility.UrlEncode(attachment_id_array);
            preview = HttpUtility.UrlEncode(preview);

            try
            {
                string _url = BaseURL + "/posts/new";

                string _parms =
                    "apikey" + "=" + APIKEY + "&" +
                    "session_token" + "=" + session_token + "&" +
                    "content" + "=" + content + "&" +
                    "type" + "=" + type + "&";

                if (post_id != string.Empty)
                    _parms += "post_id" + "=" + post_id + "&";

                if (attachment_id_array != string.Empty)
                    _parms += "attachment_id_array" + "=" + attachment_id_array + "&";

                if (preview != string.Empty)
                    _parms += "preview" + "=" + preview + "&";               

                if (type == "image")
                {
                    if(coverAttach != string.Empty)
                    {
                        _parms += "cover_attach" + "=" + coverAttach + "&";
                    }
                }

                _parms += "group_id" + "=" + group_id;


                WebPostHelper _webPos = new WebPostHelper();
                bool _isOK = _webPos.doPost(_url, _parms, null);

                if (!_isOK)
                    return null;

                string _r = _webPos.getContent();

                MR_posts_new _ret = JsonConvert.DeserializeObject<MR_posts_new>(_r);

                return _ret;
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "posts_new", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_new");

                throw;
            }
        }

        public MR_posts_newComment posts_newComment(string session_token, string group_id, string post_id,
                                                    string content, string objects, string previews)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            post_id = HttpUtility.UrlEncode(post_id);
            content = HttpUtility.UrlEncode(content);
            objects = HttpUtility.UrlEncode(objects);
            previews = HttpUtility.UrlEncode(previews);

            try
            {
                string _url = BaseURL + "/posts/newComment";

                //_url += "?" +
                //        "apikey" + "=" + APIKEY + "&" +
                //        "session_token" + "=" + session_token + "&" +
                //        "group_id" + "=" + group_id + "&" +
                //        "post_id" + "=" + post_id + "&" +
                //        "content" + "=" + content + "&" +
                //        "objects" + "=" + objects + "&" +
                //        "previews" + "=" + previews;

                var _parms = "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "post_id" + "=" + post_id + "&" +
                        "content" + "=" + content + "&" +
                        "objects" + "=" + objects + "&" +
                        "previews" + "=" + previews;

                WebPostHelper _webPos = new WebPostHelper();
                bool _isOK = _webPos.doPost(_url, _parms, null);

                if (!_isOK)
                    return null;

                string _r = _webPos.getContent();

                return JsonConvert.DeserializeObject<MR_posts_newComment>(_r);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "posts_newComment", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_newComment");

                throw;
            }
        }

        public MR_posts_getComments posts_getComments(string session_token, string group_id, string post_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            post_id = HttpUtility.UrlEncode(post_id);

            try
            {
                string _url = BaseURL + "/posts/getComments";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "post_id" + "=" + post_id;

                return HttpGetObject<MR_posts_getComments>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "posts_getComments", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_getComments");

                throw;
            }
        }

        public MR_posts_update posts_update(string session_token, string group_id, string post_id, string last_update_time, Dictionary<string, string> OptionalParams)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            post_id = HttpUtility.UrlEncode(post_id);
            last_update_time = HttpUtility.UrlEncode(last_update_time);

            try
            {
                string _url = BaseURL + "/posts/update";

                var _parms = string.Empty;

                foreach (KeyValuePair<string, string> _pair in OptionalParams)
                {
                    _parms += _pair.Key + "=" + HttpUtility.UrlEncode(_pair.Value) + "&";
                }

                _parms += "apikey" + "=" + APIKEY + "&" +
                       "session_token" + "=" + session_token + "&" +
                       "group_id" + "=" + group_id + "&" +
                       "last_update_time" + "=" + last_update_time + "&" +
                       "post_id" + "=" + post_id;

                WebPostHelper _webPos = new WebPostHelper();
                bool _isOK = _webPos.doPost(_url, _parms, null);

                if (!_isOK)
                    return null;

                string _r = _webPos.getContent();

                return JsonConvert.DeserializeObject<MR_posts_update>(_r);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "posts_update", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_update");

                throw;
            }
        }


        public MR_posts_get posts_fetchByFilter(string session_token, string group_id, string filter_entity)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            filter_entity = HttpUtility.UrlEncode(filter_entity);

            try
            {
                string _url = BaseURL + "/posts/fetchByFilter";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "filter_entity" + "=" + filter_entity;

                return HttpGetObject<MR_posts_get>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "posts_fetchByFilter", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_fetchByFilter");

                throw;
            }
        }

        public MR_posts_get posts_fetchByFilter_2(string session_token, string group_id, string post_id_list)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            post_id_list = HttpUtility.UrlEncode(post_id_list);

            try
            {
                string _url = BaseURL + "/posts/fetchByFilter";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "post_id_list" + "=" + post_id_list;

                return HttpGetObject<MR_posts_get>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "posts_fetchByFilter", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_fetchByFilter");

                throw;
            }
        }

        public MR_posts_hide_ret posts_hide(string session_token, string group_id, string post_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            post_id = HttpUtility.UrlEncode(post_id);

            try
            {
                string _url = BaseURL + "/posts/hide";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "post_id" + "=" + post_id;

                return HttpGetObject<MR_posts_hide_ret>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "posts_hide", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_hide");

                throw;
            }
        }

        public MR_posts_hide_ret posts_unhide(string session_token, string group_id, string post_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            post_id = HttpUtility.UrlEncode(post_id);

            try
            {
                string _url = BaseURL + "/posts/unhide";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "post_id" + "=" + post_id;

                return HttpGetObject<MR_posts_hide_ret>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "posts_unhide", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "posts_unhide");

                throw;
            }
        }

        #endregion

        #region previews

        public MR_previews_get previews_get(string session_token, string url)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            url = HttpUtility.UrlEncode(url);

            try
            {
                string _url = BaseURL + "/previews/get";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "url" + "=" + url + "&" +
                        "adv" + "=" + "false";

                return HttpGetObject<MR_previews_get>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "previews_get", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "previews_get");

                throw;
            }
        }

        public MR_previews_get_adv previews_get_adv(string session_token, string url)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            url = HttpUtility.UrlEncode(url);

            try
            {
                string _url = BaseURL + "/previews/get";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "url" + "=" + url + "&" +
                        "adv" + "=" + "true";

                return HttpGetObject<MR_previews_get_adv>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "previews_get_adv", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "previews_get_adv");

                throw;
            }
        }

        #endregion

        #region attachments

        public MR_attachments_upload attachments_upload(string session_token, string group_id, string fileName,
                                                        string title, string description, string type, string image_meta,
                                                        string object_id, string post_id)
        {
            MR_attachments_upload _ret;

            try
            {
                string _url = BaseURL + "/attachments/upload";

                string _mimeType = FileUtility.GetMimeType(new FileInfo(fileName));

                byte[] _data = FileUtility.ConvertFileToByteArray(fileName);

                Dictionary<string, object> _dic = new Dictionary<string, object>();
                _dic.Add("apikey", APIKEY);
                _dic.Add("session_token", session_token);
                _dic.Add("group_id", group_id);
                _dic.Add("title", title);

                //if (description == string.Empty)
                //   description = title;

                _dic.Add("description", description);
                _dic.Add("type", type);

                if (type == "image")
                    _dic.Add("image_meta", image_meta);

                if (object_id != string.Empty)
                    _dic.Add("object_id", object_id);

                if (post_id != string.Empty)
                    _dic.Add("post_id", post_id);

                _dic.Add("file", _data);

                string _userAgent = "Windows";

                string _fileName = Path.GetFileName(fileName);

                HttpWebResponse _webResponse = MultipartFormDataPostHelper.MultipartFormDataPost(_url, _userAgent, _dic,
                                                                                                 _fileName, _mimeType);


                // Process response
                StreamReader _responseReader = new StreamReader(_webResponse.GetResponseStream());
                string _r = _responseReader.ReadToEnd();
                _webResponse.Close();

                _ret = JsonConvert.DeserializeObject<MR_attachments_upload>(_r);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "attachments_upload", true);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)_e.Response;

                    if (res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "attachments_upload");

                throw;
            }

            return _ret;
        }

        public MR_attachments_get attachments_get(string session_token, string object_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            object_id = HttpUtility.UrlEncode(object_id);

            try
            {
                string _url = BaseURL + "/attachments/get";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "object_id" + "=" + object_id;

                return HttpGetObject<MR_attachments_get>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "attachments_get", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "attachments_get");

                throw;
            }
        }

        public MR_attachments_delete attachments_delete(string session_token, string object_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            object_id = HttpUtility.UrlEncode(object_id);

            try
            {
                string _url = BaseURL + "/attachments/delete";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "object_id" + "=" + object_id;

                return HttpGetObject<MR_attachments_delete>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "attachments_delete", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "attachments_delete");

                throw;
            }
        }

        //
        // http://api.waveface.com/v2/attachments/hide
        //

        //
        // http://api.waveface.com/v2/attachments/unhide
        //

        #endregion

        #region footprints

        public MR_footprints_LastScan footprints_getLastScan(string session_token, string group_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);

            try
            {
                string _url = BaseURL + "/footprints/getLastScan";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id;

                return HttpGetObject<MR_footprints_LastScan>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "footprints_getLastScan", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "footprints_getLastScan");

                throw;
            }
        }

        public MR_footprints_LastScan footprints_setLastScan(string session_token, string group_id, string post_id)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            post_id = HttpUtility.UrlEncode(post_id);

            try
            {
                string _url = BaseURL + "/footprints/setLastScan";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "post_id" + "=" + post_id;

                return HttpGetObject<MR_footprints_LastScan>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "footprints_setLastScan", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "footprints_setLastScan");

                throw;
            }
        }

        public MR_footprints_LastRead footprints_getLastRead(string session_token, string group_id, string post_id_array)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            post_id_array = HttpUtility.UrlEncode(post_id_array);

            try
            {
                string _url = BaseURL + "/footprints/getLastRead";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "post_id_array" + "=" + post_id_array;

                return HttpGetObject<MR_footprints_LastRead>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "footprints_getLastRead", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                    throw new Station401Exception();

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "footprints_getLastRead");

                throw;
            }
        }

        public MR_footprints_LastRead footprints_setLastRead(string session_token, string group_id,
                                                             string last_read_input_array)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            last_read_input_array = HttpUtility.UrlEncode(last_read_input_array);

            try
            {
                string _url = BaseURL + "/footprints/setLastRead";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "last_read_input_array" + "=" + last_read_input_array;

                return HttpGetObject<MR_footprints_LastRead>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "footprints_setLastRead", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "footprints_setLastRead");

                throw;
            }
        }

        #endregion

        #region fetchfilters

        public MR_fetchfilters_item fetchfilters_new(string session_token, string filter_name, string filter_entity,
                                                     string tag)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            filter_name = HttpUtility.UrlEncode(filter_name);
            filter_entity = HttpUtility.UrlEncode(filter_entity);
            tag = HttpUtility.UrlEncode(tag);

            try
            {
                string _url = BaseURL + "/fetchfilters/new";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "filter_name" + "=" + filter_name + "&" +
                        "filter_entity" + "=" + filter_entity + "&" +
                        "tag" + "=" + tag;

                return HttpGetObject<MR_fetchfilters_item>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "fetchfilters_new", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "fetchfilters_new");

                throw;
            }
        }

        public MR_fetchfilters_item fetchfilters_update(string session_token, string filter_id, string filter_name,
                                                        string filter_entity, string tag)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            filter_id = HttpUtility.UrlEncode(filter_id);
            filter_name = HttpUtility.UrlEncode(filter_name);
            filter_entity = HttpUtility.UrlEncode(filter_entity);
            tag = HttpUtility.UrlEncode(tag);

            try
            {
                string _url = BaseURL + "/fetchfilters/update";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "filter_id" + "=" + filter_id + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "filter_name" + "=" + filter_name + "&" +
                        "filter_entity" + "=" + filter_entity + "&" +
                        "tag" + "=" + tag;

                return HttpGetObject<MR_fetchfilters_item>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "fetchfilters_update", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "fetchfilters_update");

                throw;
            }
        }

        //public MR_fetchfilters_list fetchfilters_list(string session_token)
        //{
        //    session_token = HttpUtility.UrlEncode(session_token);

        //    try
        //    {
        //        string _url = BaseURL + "/fetchfilters/list";

        //        _url += "?" +
        //                "apikey" + "=" + APIKEY + "&" +
        //                "session_token" + "=" + session_token;

        //        return HttpGetObject<MR_fetchfilters_list>(_url);
        //    }
        //    catch (WebException _e)
        //    {
        //        NLogUtility.WebException(s_logger, _e, "fetchfilters_list", false);

        //        if (_e.Status == WebExceptionStatus.ProtocolError)
        //        {
        //            HttpWebResponse _res = (HttpWebResponse)_e.Response;

        //            if (_res.StatusCode == HttpStatusCode.Unauthorized)
        //                throw new Station401Exception();
        //        }

        //        throw;
        //    }
        //    catch (Exception _e)
        //    {
        //        NLogUtility.Exception(s_logger, _e, "fetchfilters_list");

        //        throw;
        //    }
        //}

        #endregion

        #region storages

        public MR_storages_usage storages_usage(string session_token)
        {
            try
            {
                string _url = BaseURLForGroupUserAuth + "/storages/usage";
                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + HttpUtility.UrlEncode(session_token);

                return HttpGetObject<MR_storages_usage>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "storages_usage", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "storages_usage");

                throw;
            }
        }

        #endregion

        #region station status/login/logout/remove owner

        public MR_station_status GetStationStatus(string session_token)
        {
            try
            {
                string _url = BaseURL + "/station/status/get";
                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + HttpUtility.UrlEncode(session_token);

                return HttpGetObject<MR_station_status>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "station_status", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "station_status");

                throw;
            }
        }

        public static string LoginStation(string email, string password)
        {
            //email = email.Replace("@", "%40");
            email = HttpUtility.UrlEncode(email);
            password = HttpUtility.UrlEncode(password);

            try
            {
                using (WebClient agent = new WebClient())
                {
                    string url =
                        string.Format("http://127.0.0.1:9989/v2/station/online?email={0}&password={1}&apikey={2}",
                                      email, password, APIKEY);

                    byte[] resp = agent.DownloadData(url);
                    string respText = Encoding.UTF8.GetString(resp);

                    General_R r = JsonConvert.DeserializeObject<General_R>(respText);

                    return r.session_token;
                }
            }
            catch (WebException e)
            {
                NLogUtility.WebException(s_logger, e, "LoginStation", false);

                if (e.Status == WebExceptionStatus.ConnectFailure)
                {
                    throw new StationServiceDownException("Station service down?");
                }

                string msg = ExtractApiRetMsg(e);

                if (!string.IsNullOrEmpty(msg))
                {
                    throw new Exception(msg);
                }

                throw;
            }
        }

        public static void LogoutStation(string session_token)
        {
            try
            {
                using (WebClient agent = new WebClient())
                {
                    string url = string.Format("http://127.0.0.1:9989/v2/station/offline?session_token={0}&apikey={1}",
                                               HttpUtility.UrlEncode(session_token), APIKEY);

                    agent.DownloadData(url);
                }
            }
            catch (WebException e)
            {
                NLogUtility.WebException(s_logger, e, "LogoutStation", false);

                string msg = ExtractApiRetMsg(e);

                if (!string.IsNullOrEmpty(msg))
                    throw new Exception(msg);

                throw;
            }
        }

        public static string ExtractApiRetMsg(WebException e)
        {
            HttpWebResponse res = (HttpWebResponse)e.Response;

            if (res != null)
            {
                using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                {
                    string resText = reader.ReadToEnd();
                    General_R r = JsonConvert.DeserializeObject<General_R>(resText);

                    if (res.StatusCode == HttpStatusCode.ServiceUnavailable)
                        throw new ServiceUnavailableException(r.api_ret_message);

                    return r.api_ret_message;
                }
            }

            return string.Empty;
        }

        public static void RemoveOwner(string email, string password, string token)
        {
            using (WebClient agent = new WebClient())
            {
                string url =
                    string.Format(
                        "http://127.0.0.1:9989/v2/station/drivers/remove?session_token={0}&apikey={1}&email={2}&password={3}",
                        HttpUtility.UrlEncode(token),
                        APIKEY,
                        HttpUtility.UrlEncode(email),
                        HttpUtility.UrlEncode(password));

                agent.DownloadData(url);
            }
        }

        #endregion

        #region cloudstorage

        public MR_cloudstorage_list cloudstorage_list(string session_token)
        {
            try
            {
                string _url = BaseURL + "/cloudstorage/list";
                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + HttpUtility.UrlEncode(session_token);

                MR_cloudstorage_list res = HttpGetObject<MR_cloudstorage_list>(_url);

                bool dropboxConnected = false;

                foreach (CloudStorage cloudstorage in res.cloudstorages)
                {
                    if (cloudstorage.type == "dropbox")
                    {
                        dropboxConnected = true;
                    }
                }

                if (!dropboxConnected && dropboxInstalled())
                {
                    res.cloudstorages.Add(new CloudStorage { type = "dropbox", connected = false, quota = 0, used = 0 });
                }

                return res;
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "cloudstorage_list", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "cloudstorage_list");

                throw;
            }
        }

        public MR_cloudstorage_dropbox_oauth cloudstorage_dropbox_oauth(string session_token)
        {
            try
            {
                string _url = BaseURL + "/cloudstorage/dropbox/oauth";
                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + HttpUtility.UrlEncode(session_token);

                return HttpGetObject<MR_cloudstorage_dropbox_oauth>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "cloudstorage_dropbox_oauth", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "cloudstorage_dropbox_oauth");

                throw;
            }
        }

        public MR_cloudstorage_dropbox_connect cloudstorage_dropbox_connect(string session_token, long quota)
        {
            try
            {
                string _url = BaseURL + "/cloudstorage/dropbox/connect";
                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + HttpUtility.UrlEncode(session_token) + "&" +
                        "quota" + "=" + quota.ToString() + "&" +
                        "folder" + "=" + HttpUtility.UrlEncode(getSyncFolder());

                return HttpGetObject<MR_cloudstorage_dropbox_connect>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "cloudstorage_dropbox_connect", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "cloudstorage_dropbox_connect");

                throw;
            }
        }

        public MR_cloudstorage_dropbox_update cloudstorage_dropbox_update(string session_token, long quota)
        {
            try
            {
                string _url = BaseURL + "/cloudstorage/dropbox/update";
                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + HttpUtility.UrlEncode(session_token) + "&" +
                        "quota" + "=" + quota.ToString();

                return HttpGetObject<MR_cloudstorage_dropbox_update>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "cloudstorage_dropbox_update", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "cloudstorage_dropbox_update");

                throw;
            }
        }

        public MR_cloudstorage_dropbox_disconnect cloudstorage_dropbox_disconnect(string session_token)
        {
            try
            {
                string _url = BaseURL + "/cloudstorage/dropbox/disconnect";
                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + HttpUtility.UrlEncode(session_token);

                return HttpGetObject<MR_cloudstorage_dropbox_disconnect>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "cloudstorage_dropbox_disconnect", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "cloudstorage_dropbox_disconnect");

                throw;
            }
        }

        private bool dropboxInstalled()
        {
            string hostDb = @"Dropbox\host.db";
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string hostDbPath = Path.Combine(appData, hostDb);

            return File.Exists(hostDbPath);
        }

        private string getSyncFolder()
        {
            string hostDb = @"Dropbox\host.db";
            string syncFolder = @"Waveface";
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string hostDbPath = Path.Combine(appData, hostDb);

            if (!File.Exists(hostDbPath))
                return string.Empty;

            using (FileStream fs = new FileStream(hostDbPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    var ignore = reader.ReadLine();
                    string line = reader.ReadLine().Trim();
                    byte[] data = Convert.FromBase64String(line);

                    string syncFolderPath = Path.Combine(Encoding.UTF8.GetString(data), syncFolder);
                    return syncFolderPath;
                }
            }
        }

        #endregion

        #region usertracks

        public MR_usertracks_get usertracks_get(string session_token, string group_id, string since)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            since = HttpUtility.UrlEncode(since);

            try
            {
                string _url = BaseURL + "/usertracks/get";

                _url += "?" +
                       "apikey" + "=" + APIKEY + "&" +
                       "session_token" + "=" + session_token + "&" +
                       "group_id" + "=" + group_id + "&" +
                       "since" + "=" + since + "&" +
                       "include_entities=true";

                return HttpGetObject<MR_usertracks_get>(_url);
            }
            catch (WebException _e)
            {
                NLogUtility.WebException(s_logger, _e, "usertracks_get", false);

                if (_e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse _res = (HttpWebResponse)_e.Response;

                    if (_res.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Station401Exception();
                }

                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "usertracks_get");

                throw;
            }
        }

        #endregion


        #region changelogs
        public MR_changelogs_get changelogs_get(string session_token, string group_id, int since)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);

            try
            {
                string _url = BaseURL + "/changelogs/get";

                _url += "?" +
                       "apikey" + "=" + APIKEY + "&" +
                       "session_token" + "=" + session_token + "&" +
                       "group_id" + "=" + group_id + "&" +
                       "since" + "=" + since + "&" +
                       "include_entities=true";

                return HttpGetObject<MR_changelogs_get>(_url);
            }
            catch (WebException _e)
            {
                ThrowProperException(_e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "changelogs_get");

                throw;
            }
        }
        #endregion
    }

    #region ServiceUnavailableException

    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string msg)
            : base(msg)
        {
        }
    }

    #endregion

    #region Station Exception

    public class StationServiceDownException : Exception
    {
        public StationServiceDownException(string msg)
            : base(msg)
        {
        }
    }

    #endregion
}
