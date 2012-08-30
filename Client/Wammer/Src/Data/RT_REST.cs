#region

using System.Collections.Generic;
using System.IO;
using Waveface.API.V2;
using System;
using Waveface.Libs.StationDB;
using System.Diagnostics;
using System.Web;
using Newtonsoft.Json;

#endregion

namespace Waveface
{
    public class RT_REST
    {
        private RunTime m_rt;
        private WService m_service;

        #region Properties

        public WService Service
        {
            get
            {
                return m_service;
            }
        }

		[JsonIgnore]
        public string SessionToken
        {
            get
            {
                return m_rt.Login.session_token;
            }
        }

        public bool IsNetworkAvailable { get; set; }

        #endregion

        public RT_REST(RunTime rt)
        {
            m_rt = rt;

            m_service = new WService();
        }

        #region RedirectURL

        public string attachments_getRedirectURL(string orgURL, string object_id, bool isImage)
        {
            return AttachmentUtility.GetRedirectURL(orgURL, SessionToken, object_id, isImage);
        }

        public string attachments_getRedirectURL_Image(Attachment a, string imageType, out string url, out string fileName, bool forceCloud)
        {
            return AttachmentUtility.GetRedirectURL_Image(SessionToken, a, imageType, out url, out fileName, forceCloud);
        }

        public string attachments_getRedirectURL_PdfCoverPage(string orgURL)
        {
            return AttachmentUtility.GetRedirectURL_PdfCoverPage(orgURL, SessionToken);
        }

        public string attachments_getImageURL(string object_id, string type)
        {
			DebugInfo.ShowMethod();

            if (type == "origin")
                return string.Format("{0}/v2/attachments/view?object_id={1}&apikey={2}&session_token={3}",
					WService.HostIP, object_id, HttpUtility.UrlEncode(WService.APIKEY), HttpUtility.UrlEncode(SessionToken));
            else
                return string.Format("{0}/v2/attachments/view?object_id={1}&apikey={2}&session_token={3}&image_meta={4}",
					WService.HostIP, object_id, HttpUtility.UrlEncode(WService.APIKEY), HttpUtility.UrlEncode(SessionToken), type);
        }

        private string attachments_getSavedFileName(string object_id, string type)
        {
            if (type == "origin")
                throw new InvalidOperationException("origin is not supported");
            else
                return string.Format("{0}_{1}.dat", object_id, type);
        }

        public string attachments_getThumbnailFilePath(string object_id, string type)
        {
            return Path.Combine(Main.GCONST.ImageCachePath, attachments_getSavedFileName(object_id, type));
        }

        public string attachments_getOriginFilePath(string object_id)
        {
            var orig_file_name = AttachmentCollection.QueryFileName(object_id);
            if (string.IsNullOrEmpty(orig_file_name))
                return string.Empty;

            var extension = Path.GetExtension(orig_file_name);
            return Path.Combine(Main.GCONST.ImageCachePath, object_id + extension);
        }

        #endregion

        #region Station

        public bool CheckStationAlive(string ip, int timeout)
        {
            if (!IsNetworkAvailable)
                return false;

            string _url = ip + "/v2/availability/ping/";

            string _ret = m_service.HttpGet(_url, timeout, true);

            return (_ret != null);
        }

        #endregion


        public MR_auth_login Auth_Login(string email, string password)
        {
            //if (!IsNetworkAvailable)
            //    return null;

            MR_auth_login _ret = null;

            try
            {
                _ret = m_service.auth_login(email, password);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public void Auth_Logout(string session)
        {
            try
            {
                m_service.auth_logout(session);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }
        }

        public MR_groups_get Groups_Get(string group_id)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_groups_get _ret = null;

            try
            {
                _ret = m_service.groups_get(SessionToken, group_id);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_posts_new Posts_New(string post_id, string text, string files, string previews, string type, string coverAttach)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_posts_new _ret = null;

            try
            {
                _ret = m_service.posts_new(SessionToken, post_id, m_rt.CurrentGroupID, text, files, previews, type, coverAttach);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_posts_newComment Posts_NewComment(string post_id, string content, string objects, string previews)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_posts_newComment _ret = null;

            try
            {
                _ret = m_service.posts_newComment(SessionToken, m_rt.CurrentGroupID, post_id, content, objects, previews);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_posts_update Posts_update(string post_id, string last_update_time, Dictionary<string, string> OptionalParams)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_posts_update _ret = null;

            try
            {
                _ret = m_service.posts_update(SessionToken, m_rt.CurrentGroupID, post_id, last_update_time, OptionalParams);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_posts_getSingle Posts_GetSingle(string post_id)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_posts_getSingle _ret = null;

            try
            {
                _ret = m_service.posts_getSingle(SessionToken, m_rt.CurrentGroupID, post_id);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }



        public MR_previews_get_adv Preview_GetAdvancedPreview(string url)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_previews_get_adv _ret = null;

            try
            {
                _ret = m_service.previews_get_adv(SessionToken, url);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }
            catch
            {
                return null;
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_attachments_upload File_UploadFile(string text, string filePath, string object_id, bool isImage, string post_id)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_attachments_upload _ret;
            string _resizedImageFilePath = string.Empty;

            if (isImage)
            {
                if (m_rt.StationMode) //如果有Station則上傳原圖, 否則就上1024中圖 (512 - > 1024)
                {
                    _ret = m_service.attachments_upload(SessionToken, m_rt.CurrentGroupID, filePath, text, "", "image", "origin", object_id, post_id);
                }
                else
                {
                    _resizedImageFilePath = ImageUtility.ResizeImage(filePath, text, "1024", 100); //512 -> 1024

                    _ret = m_service.attachments_upload(SessionToken, m_rt.CurrentGroupID, _resizedImageFilePath, text, "", "image", "medium", object_id, post_id);
                }
            }
            else
            {
                _ret = m_service.attachments_upload(SessionToken, m_rt.CurrentGroupID, filePath, text, "", "doc", "", "", post_id);
            }

            if (_ret != null)
            {
                if (_ret.status == "401")
                    throw new Station401Exception();

                if (_ret.status == "200")
                {
                    // 如果傳中圖到Cloud, 則要把原圖Cache起來, 待有Station再傳原圖
                    if (_resizedImageFilePath != string.Empty)
                    {
                        string _ext = ".jpg";

                        int _idx = text.IndexOf(".");

                        if (_idx != -1)
                            _ext = text.Substring(_idx);

                        string _originCacheFile_OID = Main.GCONST.ImageUploadCachePath + _ret.object_id + _ext;
                        string _originCacheFile_REAL = Main.GCONST.ImageUploadCachePath + text;

                        File.Copy(filePath, _originCacheFile_OID, true);

                        Main.Current.UploadOriginPhotosToStationManager.Add(_originCacheFile_OID, _originCacheFile_REAL, _ret.object_id, post_id);
                    }

                    return _ret;
                }
            }

            return null;
        }

        public MR_fetchfilters_list SearchFilters_List()
        {
            if (!IsNetworkAvailable)
                return null;

            MR_fetchfilters_list _ret = null;

            //try
            //{
            //    _ret = m_service.fetchfilters_list(SessionToken);
            //}
            //catch (Station401Exception _e)
            //{
            //    Main.Current.Station401ExceptionHandler(_e.Message);
            //}

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_fetchfilters_item FetchFilters_New(string filter_name, string filter_entity, string tag)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_fetchfilters_item _ret = null;

            try
            {
                _ret = m_service.fetchfilters_new(SessionToken, filter_name, filter_entity, tag);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_fetchfilters_item FetchFilters_Update(string searchfilter_id, string filter_name, string filter_entity, string tag)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_fetchfilters_item _ret = null;

            try
            {
                _ret = m_service.fetchfilters_update(SessionToken, searchfilter_id, filter_name, filter_entity, tag);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_users_findMyStation Users_findMyStation()
        {
            if (!IsNetworkAvailable)
                return null;

            MR_users_findMyStation _ret = null;

            try
            {
                _ret = m_service.users_findMyStation(SessionToken);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_posts_hide_ret Posts_hide(string post_id)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_posts_hide_ret _ret = null;

            try
            {
                _ret = m_service.posts_hide(SessionToken, m_rt.CurrentGroupID, post_id);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_posts_get Posts_FetchByFilter(string filter_entity)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_posts_get _ret = null;

            try
            {
                _ret = m_service.posts_fetchByFilter(SessionToken, m_rt.CurrentGroupID, filter_entity);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_posts_get Posts_FetchByFilter_2(string post_id_list)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_posts_get _ret = null;

            try
            {
                _ret = m_service.posts_fetchByFilter_2(SessionToken, m_rt.CurrentGroupID, post_id_list);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_posts_getLatest Posts_getLatest(string limit)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_posts_getLatest _ret = null;

            try
            {
                _ret = m_service.posts_getLatest(SessionToken, m_rt.CurrentGroupID, limit);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_posts_get Posts_get(string limit, string datum, string filter)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_posts_get _ret = null;

            try
            {
                _ret = m_service.posts_get(SessionToken, m_rt.CurrentGroupID, limit, datum, filter);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public LastScan Footprints_getLastScan()
        {
            if (!IsNetworkAvailable)
                return null;

            MR_footprints_LastScan _ret = null;

            try
            {
                _ret = m_service.footprints_getLastScan(SessionToken, m_rt.CurrentGroupID);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret.last_scan;
            }

            return null;
        }

        public string Footprints_setLastScan(string post_id)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_footprints_LastScan _ret = null;

            try
            {
                _ret = m_service.footprints_setLastScan(SessionToken, m_rt.CurrentGroupID, post_id);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret.last_scan.post_id;
            }

            return null;
        }

        public MR_storages_usage Storages_Usage()
        {
            if (!IsNetworkAvailable)
                return null;

            MR_storages_usage _ret = null;

            try
            {
                _ret = m_service.storages_usage(SessionToken);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_station_status GetStationStatus()
        {
            if (!IsNetworkAvailable)
                return null;

            MR_station_status _ret = null;

            try
            {
                _ret = m_service.GetStationStatus(SessionToken);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_usertracks_get usertracks_get(string since)
        {
            if (!IsNetworkAvailable)
                return null;

            MR_usertracks_get _ret = null;

            try
            {
                _ret = m_service.usertracks_get(SessionToken, m_rt.CurrentGroupID, since);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }

        public MR_changelogs_get changelogs_get(int since)
        {
            MR_changelogs_get _ret = null;

            try
            {
                _ret = m_service.changelogs_get(SessionToken, m_rt.CurrentGroupID, since);
            }
            catch (Station401Exception _e)
            {
                Main.Current.Station401ExceptionHandler(_e.Message);
            }

            if (_ret != null)
            {
                if (_ret.status == "200")
                    return _ret;
            }

            return null;
        }
    }
}