#region

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Spring.Http.Converters;
using Spring.Rest.Client;

#endregion

namespace Waveface.API.V2
{
    public class WService
    {
        public const string CloundIP = "https://develop.waveface.com"; //http://develop.waveface.com:8080
        public static string APIKEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";

        private RestTemplate m_rest;

        public static string HostIP
        {
            get
            {
                if (string.IsNullOrEmpty(StationIP))
                {
                    return CloundIP;
                }
                else
                {
                    return StationIP;
                }
            }
        }

        public static string BaseURL
        {
            get { return HostIP + "/v2"; }
        }

        public static string BaseURLForGroupUserAuth
        {
            get { return CloundIP + "/v2"; }
        }

        public static string StationIP { get; set; }

        public WService()
        {
            m_rest = new RestTemplate();

            m_rest.MessageConverters.Add(new FormHttpMessageConverter());
            m_rest.MessageConverters.Add(new StringHttpMessageConverter());
        }

        private T HttpGetObject<T>(string _url)
        {
            string _r = m_rest.GetForObject<string>(_url);
            _r = StringUtility.UTF8ToISO_8859_1(_r);
            return JsonConvert.DeserializeObject<T>(_r);
        }

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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();
                    
                throw;
            }
            catch
            {
                throw;
            }

            return _ret;
        }

        public MR_auth_login auth_login(string email, string password)
        {
            email = email.Replace("@", "%40");
            password = HttpUtility.UrlEncode(password);

            MR_auth_login _ret;

            try
            {
                string _url = BaseURLForGroupUserAuth + "/auth/login";

                string _parms = "apikey" + "=" + APIKEY + "&" +
                                "email" + "=" + email + "&" +
                                "password" + "=" + password;

                WebPostHelper _webPos = new WebPostHelper();
                bool _isOK = _webPos.doPost(_url, _parms, null);

                if (!_isOK)
                    return null;

                string _r = _webPos.getContent();

                _ret = JsonConvert.DeserializeObject<MR_auth_login>(_r);
            }
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }

            return _ret;
        }

        public MR_auth_logout auth_logout(string session_token)
        {
            session_token = HttpUtility.UrlEncode(session_token);

            try
            {
                string _url = BaseURLForGroupUserAuth + "/auth/logout";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token;

                return HttpGetObject<MR_auth_logout>(_url);
            }
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
                string _url = BaseURLForGroupUserAuth + "/users/get";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "user_id" + "=" + user_id;

                return HttpGetObject<MR_users_get>(_url);
            }
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }
        }

        public MR_posts_new posts_new(string session_token, string group_id, string content, string attachment_id_array, string preview, string type)
        {
            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            content = HttpUtility.UrlEncode(content);
            attachment_id_array = HttpUtility.UrlEncode(attachment_id_array);
            preview = HttpUtility.UrlEncode(preview);

            try
            {
                string _url = BaseURL + "/posts/new";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "content" + "=" + content + "&" +
                        "type" + "=" + type + "&";

                if (attachment_id_array != string.Empty)
                    _url += "attachment_id_array" + "=" + attachment_id_array + "&";

                if (preview != string.Empty)
                    _url += "preview" + "=" + preview + "&";

                _url += "group_id" + "=" + group_id;

                return HttpGetObject<MR_posts_new>(_url);
            }
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }
        }

        public MR_posts_newComment posts_newComment(string session_token, string group_id, string post_id, string content, string objects, string previews)
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

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "post_id" + "=" + post_id + "&" +
                        "content" + "=" + content + "&" +
                        "objects" + "=" + objects + "&" +
                        "previews" + "=" + previews;

                return HttpGetObject<MR_posts_newComment>(_url);
            }
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }
        }

        public MR_previews_get_adv previews_get_adv(string session_token, string url)
        {
            MR_previews_get_adv _ret;

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

                string _r = m_rest.GetForObject<string>(_url);
                _r = StringUtility.UTF8ToISO_8859_1(_r);

                JsonSerializerSettings _settings = new JsonSerializerSettings();
                _settings.MissingMemberHandling = MissingMemberHandling.Ignore;

                _ret = JsonConvert.DeserializeObject<MR_previews_get_adv>(_r, _settings);
            }
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }

            return _ret;
        }

        #endregion

        #region attachments

        public MR_attachments_upload attachments_upload(string session_token, string group_id, string fileName, string title, string description, string type, string image_meta, string object_id)
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
                _dic.Add("description", description);
                _dic.Add("type", type);

                if (type == "image")
                    _dic.Add("image_meta", image_meta);

                if (object_id != string.Empty)
                    _dic.Add("object_id", object_id);

                _dic.Add("file", _data);

                string _userAgent = "Windows";
                HttpWebResponse _webResponse = MultipartFormDataPostHelper.MultipartFormDataPost(_url, _userAgent, _dic, new FileInfo(fileName).Name, _mimeType); //HttpUtility.UrlEncode(new FileInfo(fileName).Name)

                // Process response
                StreamReader _responseReader = new StreamReader(_webResponse.GetResponseStream());
                string _r = _responseReader.ReadToEnd();
                _webResponse.Close();

                _ret = JsonConvert.DeserializeObject<MR_attachments_upload>(_r);
            }
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();
                else
                    throw;
            }
            catch
            {
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }
        }

        public MR_footprints_LastRead footprints_setLastRead(string session_token, string group_id, string last_read_input_array)
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region fetchfilters

        public MR_fetchfilters_item fetchfilters_new(string session_token, string filter_name, string filter_entity, string tag)
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }
        }

        public MR_fetchfilters_item fetchfilters_update(string session_token, string filter_id, string filter_name, string filter_entity, string tag)
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
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }
        }

        public MR_fetchfilters_list fetchfilters_list(string session_token)
        {
            session_token = HttpUtility.UrlEncode(session_token);

            try
            {
                string _url = BaseURL + "/fetchfilters/list";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token;

                return HttpGetObject<MR_fetchfilters_list>(_url);
            }
            catch (HttpResponseException _e)
            {
                if (_e.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Station401Exception();

                throw;
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}