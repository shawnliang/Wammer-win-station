#region

using System.IO;
using Waveface.API.V2;

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
            return AttachmentUrlUtility.GetRedirectURL(orgURL, SessionToken, object_id, isImage);
        }

        public string attachments_getRedirectURL_Image(Attachment a, string imageType, out string url, out string fileName)
        {
            return AttachmentUrlUtility.GetRedirectURL_Image(SessionToken, a, imageType, out url, out fileName);
        }

        public string attachments_getRedirectURL_PdfCoverPage(string orgURL)
        {
            return AttachmentUrlUtility.GetRedirectURL_PdfCoverPage(orgURL, SessionToken);
        }

        #endregion

        public MR_auth_login Auth_Login(string email, string password)
        {
            MR_auth_login _login = m_service.auth_login(email, password);

            if ((_login != null) && (_login.status == "200"))
            {
                return _login;
            }
            else
            {
                return null;
            }
        }

        public MR_groups_get Groups_Get(string group_id)
        {
            MR_groups_get _groupsGet = m_service.groups_get(SessionToken, group_id);

            if ((_groupsGet != null) && (_groupsGet.status == "200"))
            {
                return _groupsGet;
            }
            else
            {
                return null;
            }
        }

        public MR_posts_new Posts_New(string text, string files, string previews, string type)
        {
            MR_posts_new _postsNew = m_service.posts_new(SessionToken, m_rt.CurrentGroupID, text, files, previews, type);

            if ((_postsNew != null) && (_postsNew.status == "200"))
            {
                return _postsNew;
            }
            else
            {
                return null;
            }
        }

        public MR_posts_newComment Posts_NewComment(string post_id, string content, string objects, string previews)
        {
            MR_posts_newComment _newComment = m_service.posts_newComment(SessionToken, m_rt.CurrentGroupID, post_id, content, objects, previews);

            if ((_newComment != null) && (_newComment.status == "200"))
            {
                return _newComment;
            }
            else
            {
                return null;
            }
        }

        public MR_posts_getSingle Posts_GetSingle(string post_id)
        {
            MR_posts_getSingle _getSingle = m_service.posts_getSingle(SessionToken, m_rt.CurrentGroupID, post_id);

            if ((_getSingle != null) && (_getSingle.status == "200"))
            {
                return _getSingle;
            }
            else
            {
                return null;
            }
        }

        public MR_previews_get_adv Preview_GetAdvancedPreview(string url)
        {
            MR_previews_get_adv _previewsGetAdv = m_service.previews_get_adv(SessionToken, url);

            if ((_previewsGetAdv != null) && (_previewsGetAdv.status == "200"))
            {
                return _previewsGetAdv;
            }
            else
            {
                return null;
            }
        }

        public MR_attachments_upload File_UploadFile(string text, string filePath, string object_id, bool isImage)
        {
            MR_attachments_upload _attachmentsUpload;
            string _resizedImageFilePath = string.Empty;

            if (isImage)
            {
                if (m_rt.IsStationOK) //如果有Station則上傳原圖, 否則就上512中圖
                {
                    _attachmentsUpload = m_service.attachments_upload(SessionToken, m_rt.CurrentGroupID, filePath, text, "", "image", "origin", object_id);
                }
                else
                {
                    _resizedImageFilePath = ImageUtility.ResizeImage(filePath, text, "512", 50);
                    _attachmentsUpload = m_service.attachments_upload(SessionToken, m_rt.CurrentGroupID, _resizedImageFilePath, text, "", "image", "medium", object_id);
                }
            }
            else
            {
                _attachmentsUpload = m_service.attachments_upload(SessionToken, m_rt.CurrentGroupID, filePath, text, "", "doc", "", "");
            }

            if ((_attachmentsUpload != null) && (_attachmentsUpload.status == "200"))
            {
                // 如果傳中圖到Cloud, 則要把原圖Cache起來, 待有Station再傳原圖
                if (_resizedImageFilePath != string.Empty)
                {
                    string _ext = ".jpg";

                    int _idx = text.IndexOf(".");

                    if (_idx != -1)
                        _ext = text.Substring(_idx);

                    string _originCacheFile = MainForm.GCONST.ImageUploadCachePath + _attachmentsUpload.object_id + _ext;
                    
                    File.Copy(filePath, _originCacheFile);
                }

                return _attachmentsUpload;
            }

            return null;
        }

        public MR_fetchfilters_list SearchFilters_List()
        {
            MR_fetchfilters_list _filtersList = m_service.fetchfilters_list(SessionToken);

            if ((_filtersList != null) && (_filtersList.status == "200"))
            {
                return _filtersList;
            }
            else
            {
                return null;
            }
        }

        public MR_fetchfilters_item FetchFilters_New(string filter_name, string filter_entity, string tag)
        {
            MR_fetchfilters_item _item = m_service.fetchfilters_new(SessionToken, filter_name, filter_entity, tag);

            if ((_item != null) && (_item.status == "200"))
            {
                return _item;
            }
            else
            {
                return null;
            }
        }

        public MR_fetchfilters_item FetchFilters_Update(string searchfilter_id, string filter_name, string filter_entity, string tag)
        {
            MR_fetchfilters_item _item = m_service.fetchfilters_update(SessionToken, searchfilter_id, filter_name, filter_entity, tag);

            if ((_item != null) && (_item.status == "200"))
            {
                return _item;
            }
            else
            {
                return null;
            }
        }

        public MR_users_findMyStation Users_findMyStation()
        {
            MR_users_findMyStation _findMyStation = m_service.users_findMyStation(SessionToken);

            if ((_findMyStation != null) && (_findMyStation.status == "200"))
            {
                return _findMyStation;
            }
            else
            {
                return null;
            }
        }

        public MR_posts_hide_ret Posts_hide(string post_id)
        {
            MR_posts_hide_ret _ret = m_service.posts_hide(SessionToken, m_rt.CurrentGroupID, post_id);

            if ((_ret != null) && (_ret.status == "200"))
            {
                return _ret;
            }
            else
            {
                return null;
            }
        }

        public MR_posts_get Posts_FetchByFilter(string filter_entity)
        {
            MR_posts_get _postsGet = m_service.posts_fetchByFilter(SessionToken, m_rt.CurrentGroupID, filter_entity);

            if ((_postsGet != null) && (_postsGet.status == "200"))
            {
                return _postsGet;
            }
            else
            {
                return null;
            }
        }

        public MR_posts_getLatest Posts_getLatest(string limit)
        {
            MR_posts_getLatest _getLatest = m_service.posts_getLatest(SessionToken, m_rt.CurrentGroupID, limit);

            if ((_getLatest != null) && (_getLatest.status == "200"))
            {
                return _getLatest;
            }
            else
            {
                return null;
            }
        }

        public MR_posts_get Posts_get(string limit, string datum, string filter)
        {
            MR_posts_get _get = m_service.posts_get(SessionToken, m_rt.CurrentGroupID, limit, datum, filter);

            if ((_get != null) && (_get.status == "200"))
            {
                return _get;
            }
            else
            {
                return null;
            }
        }
    }
}