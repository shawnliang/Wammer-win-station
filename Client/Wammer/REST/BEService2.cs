#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Spring.Http.Converters;
using Spring.Rest.Client;

#endregion

namespace Waveface.API.V2
{
    public class BEService2
    {
        public static string APIKEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";

        public const string HostIP = "http://develop.waveface.com:8080";
        public const string BaseURL = HostIP + "/v2";  //http://192.168.1.221:8082/v2 http://develop.waveface.com:8080/v2 http://192.168.1.105:8082/v2


        private RestTemplate m_rest;

        public BEService2()
        {
            m_rest = new RestTemplate();

            m_rest.MessageConverters.Add(new FormHttpMessageConverter());
            m_rest.MessageConverters.Add(new StringHttpMessageConverter());
        }

        private static string UTF8ToISO_8859_1(string str)
        {
            byte[] _utf8Bytes = Encoding.UTF8.GetBytes(str);
            byte[] _iso_8859_1_Bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), _utf8Bytes);

            return Encoding.UTF8.GetString(_iso_8859_1_Bytes);
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
                string _url = BaseURL + "/auth/signup";

                /*
                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "email" + "=" + email + "&" +
                        "password" + "=" + password + "&" +
                        "nickname" + "=" + nickname + "&" +
                        "avatar_url" + "=" + avatar_url;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);
                */

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
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
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
                string _url = BaseURL + "/auth/login";

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
            catch (Exception _e)
            {
                // MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_auth_logout auth_logout(string session_token)
        {
            MR_auth_logout _ret;

            session_token = HttpUtility.UrlEncode(session_token);

            try
            {
                string _url = BaseURL + "/auth/logout";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_auth_logout>(_r);
            }
            catch (Exception _e)
            {
                // MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        #endregion

        #region users

        public MR_users_get users_get(string session_token, string user_id)
        {
            MR_users_get _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            user_id = HttpUtility.UrlEncode(user_id);

            try
            {
                string _url = BaseURL + "/users/get";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "user_id" + "=" + user_id;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_users_get>(_r);
            }
            catch (Exception _e)
            {
                // MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_users_update users_update(string session_token, string user_id, string nickname, string avatar_url)
        {
            MR_users_update _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            user_id = HttpUtility.UrlEncode(user_id);
            nickname = HttpUtility.UrlEncode(nickname);
            avatar_url = HttpUtility.UrlEncode(avatar_url);

            try
            {
                string _url = BaseURL + "/users/update";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "user_id" + "=" + user_id + "&" +
                        "nickname" + "=" + nickname + "&" +
                        "avatar_url" + "=" + avatar_url;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_users_update>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_users_passwd users_passwd(string session_token, string old_passwd, string new_passwd)
        {
            MR_users_passwd _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            old_passwd = HttpUtility.UrlEncode(old_passwd);
            new_passwd = HttpUtility.UrlEncode(new_passwd);

            try
            {
                string _url = BaseURL + "/users/passwd";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "old_passwd" + "=" + old_passwd + "&" +
                        "new_passwd" + "=" + new_passwd;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_users_passwd>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        #endregion

        #region groups

        public MR_groups_create groups_create(string session_token, string name, string description)
        {
            MR_groups_create _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            name = HttpUtility.UrlEncode(name);
            description = HttpUtility.UrlEncode(description);

            try
            {
                string _url = BaseURL + "/groups/create";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "name" + "=" + name + "&" +
                        "description" + "=" + description;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_groups_create>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_groups_get groups_get(string session_token, string group_id)
        {
            MR_groups_get _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);

            try
            {
                string _url = BaseURL + "/groups/get";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_groups_get>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_groups_update groups_update(string session_token, string group_id, string name, string description)
        {
            MR_groups_update _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            name = HttpUtility.UrlEncode(name);
            description = HttpUtility.UrlEncode(description);

            try
            {
                string _url = BaseURL + "/groups/update";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "name" + "=" + name + "&" +
                        "description" + "=" + description;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_groups_update>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_groups_delete groups_delete(string session_token, string group_id)
        {
            MR_groups_delete _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);

            try
            {
                string _url = BaseURL + "/groups/delete";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_groups_delete>(_r);
            }
            catch (Exception _e)
            {
                return null;
            }

            return _ret;
        }

        public MR_groups_inviteUser groups_inviteUser(string session_token, string group_id, string email)
        {
            MR_groups_inviteUser _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            email = HttpUtility.UrlEncode(email);

            try
            {
                string _url = BaseURL + "/groups/inviteUser";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "email" + "=" + email;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_groups_inviteUser>(_r);
            }
            catch (Exception _e)
            {
                // MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_groups_kickUser groups_kickUser(string session_token, string group_id, string user_id)
        {
            MR_groups_kickUser _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            group_id = HttpUtility.UrlEncode(group_id);
            user_id = HttpUtility.UrlEncode(user_id);

            try
            {
                string _url = BaseURL + "/groups/kickUser";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "group_id" + "=" + group_id + "&" +
                        "user_id" + "=" + user_id;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_groups_kickUser>(_r);
            }
            catch (Exception _e)
            {
                // MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        #endregion

        #region posts

        public MR_posts_getSingle posts_getSingle(string session_token, string group_id, string post_id)
        {
            MR_posts_getSingle _ret;

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

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_posts_getSingle>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_posts_get posts_get(string session_token, string group_id, string limit, string datum, string filter)
        {
            MR_posts_get _ret;

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

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_posts_get>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_posts_getLatest posts_getLatest(string session_token, string group_id, string limit)
        {
            MR_posts_getLatest _ret;

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

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_posts_getLatest>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_posts_new posts_new(string session_token, string group_id, string content, string attachment_id_array, string preview, string type)
        {
            MR_posts_new _ret;

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

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_posts_new>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_posts_newComment posts_newComment(string session_token, string group_id, string post_id, string content, string objects, string previews)
        {
            MR_posts_newComment _ret;

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

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_posts_newComment>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_posts_getComments posts_getComments(string session_token, string group_id, string post_id)
        {
            MR_posts_getComments _ret;

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

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_posts_getComments>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        #endregion

        #region previews

        public MR_previews_get previews_get(string session_token, string url)
        {
            MR_previews_get _ret;

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

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_previews_get>(_r);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
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
                _r = UTF8ToISO_8859_1(_r);

                JsonSerializerSettings _settings = new JsonSerializerSettings();
                _settings.MissingMemberHandling = MissingMemberHandling.Ignore;

                _ret = JsonConvert.DeserializeObject<MR_previews_get_adv>(_r, _settings);
            }
            catch (Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        #endregion

        #region attachments

        public MR_attachments_upload attachments_upload(string session_token, string group_id, string fileName, string title, string description, string type, string image_meta)
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
                /*
                                _dic.Add("apikey", HttpUtility.UrlEncode(APIKEY));
                                _dic.Add("session_token", HttpUtility.UrlEncode(session_token));
                                _dic.Add("group_id", HttpUtility.UrlEncode(group_id));
                                _dic.Add("title", HttpUtility.UrlEncode(title));
                                _dic.Add("description", HttpUtility.UrlEncode(description));
                                _dic.Add("type", HttpUtility.UrlEncode(type));
                                */
                if (type == "image")
                    _dic.Add("image_meta", image_meta);

                _dic.Add("file", _data);

                string _userAgent = "Windows";
                HttpWebResponse _webResponse = MultipartFormDataPostHelper.MultipartFormDataPost(_url, _userAgent, _dic, HttpUtility.UrlEncode(new FileInfo(fileName).Name), _mimeType);

                // Process response
                StreamReader _responseReader = new StreamReader(_webResponse.GetResponseStream());
                string _r = _responseReader.ReadToEnd();
                _webResponse.Close();

                _ret = JsonConvert.DeserializeObject<MR_attachments_upload>(_r);
            }
            catch (Exception _e)
            {
                // MessageBox.Show(_e.Message);
                return null;
            }

            return _ret;
        }

        public MR_attachments_get attachments_get(string session_token, string object_id)
        {
            MR_attachments_get _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            object_id = HttpUtility.UrlEncode(object_id);

            try
            {
                string _url = BaseURL + "/attachments/get";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "object_id" + "=" + object_id;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_attachments_get>(_r);
            }
            catch (Exception _e)
            {
                return null;
            }

            return _ret;
        }

        public MR_attachments_delete attachments_delete(string session_token, string object_id)
        {
            MR_attachments_delete _ret;

            session_token = HttpUtility.UrlEncode(session_token);
            object_id = HttpUtility.UrlEncode(object_id);

            try
            {
                string _url = BaseURL + "/attachments/delete";

                _url += "?" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token + "&" +
                        "object_id" + "=" + object_id;

                string _r = m_rest.GetForObject<string>(_url);
                _r = UTF8ToISO_8859_1(_r);

                _ret = JsonConvert.DeserializeObject<MR_attachments_delete>(_r);
            }
            catch (Exception _e)
            {
                return null;
            }

            return _ret;
        }

        /*
        public void attachments_getRedirectURL(string orgURL, string session_token)
        {
            session_token = HttpUtility.UrlEncode(session_token);

            try
            {
                string _url = HostIP + orgURL;

                _url += "&" +
                        "apikey" + "=" + APIKEY + "&" +
                        "session_token" + "=" + session_token;

                byte[] _r = m_rest.GetForObject<byte[]>(_url);

                ;
                //_ret = JsonConvert.DeserializeObject<MR_attachments_delete>(_r);
            }
            catch (Exception _e)
            {
            }
        }
        */

        public string attachments_getRedirectURL(string orgURL, string session_token)
        {
            session_token = HttpUtility.UrlEncode(session_token);

            string _url = HostIP + orgURL;

            _url += "&" +
                    "apikey" + "=" + APIKEY + "&" +
                    "session_token" + "=" + session_token;

            return _url;
        }

        #endregion
    }
}