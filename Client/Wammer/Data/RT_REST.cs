
using System.IO;
using Waveface.API.V2;

namespace Waveface
{
    public class RT_REST
    {
        private WService m_serviceV2;
        private RunTimeData m_runTimeData;

        public WService Service
        {
            get
            {
                return m_serviceV2;
            }
        }

        public string SessionToken
        {
            get { return m_runTimeData.Login.session_token; }
        }

        public RT_REST(RunTimeData runTimeData)
        {
            m_runTimeData = runTimeData;

            m_serviceV2 = new WService();
        }

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

        public MR_posts_new Post_CreateNewPost(string text, string files, string previews, string type)
        {
            MR_posts_new _postsNew = m_serviceV2.posts_new(SessionToken, m_runTimeData.CurrentGroupID, text, files, previews, type);

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
            MR_posts_newComment _newComment = m_serviceV2.posts_newComment(SessionToken, m_runTimeData.CurrentGroupID, post_id, content, objects, previews);

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
            MR_posts_getSingle _getSingle = m_serviceV2.posts_getSingle(SessionToken, m_runTimeData.CurrentGroupID, post_id);

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
            MR_previews_get_adv _previewsGetAdv = m_serviceV2.previews_get_adv(SessionToken, url);

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
                if (m_runTimeData.IsStationOK) //如果有Station則上傳原圖, 否則就上512中圖
                {
                    _attachmentsUpload = m_serviceV2.attachments_upload(SessionToken, m_runTimeData.CurrentGroupID, filePath, text, "", "image", "origin", object_id);
                }
                else
                {
                    _resizedImageFilePath = ImageUtility.ResizeImage(filePath, text, "512", 50);
                    _attachmentsUpload = m_serviceV2.attachments_upload(SessionToken, m_runTimeData.CurrentGroupID, _resizedImageFilePath, text, "", "image", "medium", object_id);
                }
            }
            else
            {
                _attachmentsUpload = m_serviceV2.attachments_upload(SessionToken, m_runTimeData.CurrentGroupID, filePath, text, "", "doc", "", "");
            }

            if ((_attachmentsUpload != null) && (_attachmentsUpload.status == "200"))
            {
                // 如果傳中圖到Cloud, 則要把原圖Cache起來, 待有Station在傳原圖
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

        public MR_posts_get Posts_FetchByFilter(string filter_entity)
        {
            MR_posts_get _postsGet = m_serviceV2.posts_fetchByFilter(SessionToken, m_runTimeData.CurrentGroupID, filter_entity);

            if ((_postsGet != null) && (_postsGet.status == "200"))
            {
                return _postsGet;
            }
            else
            {
                return null;
            }
        }

        public MR_fetchfilters_list SearchFilters_List()
        {
            MR_fetchfilters_list _filtersList = m_serviceV2.fetchfilters_list(SessionToken);

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
            MR_fetchfilters_item _item = m_serviceV2.fetchfilters_new(SessionToken, filter_name, filter_entity, tag);

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
            MR_fetchfilters_item _item = m_serviceV2.fetchfilters_update(SessionToken, searchfilter_id, filter_name, filter_entity, tag);

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
            MR_users_findMyStation _findMyStation = m_serviceV2.users_findMyStation(SessionToken);

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
            MR_posts_hide_ret _ret = m_serviceV2.posts_hide(SessionToken, m_runTimeData.CurrentGroupID, post_id);

            if ((_ret != null) && (_ret.status == "200"))
            {
                return _ret;
            }
            else
            {
                return null;
            }
        }
    }
}
