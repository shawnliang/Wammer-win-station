#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface;
#endregion

namespace StationSetup
{
    public class PostService
    {
        private const string PREVIEW = "{\"provider_url\":\"[URL]\",\"provider_name\":\"[TITLE]\",\"title\":\"[TITLE]\",\"url\":\"[URL]\",\"description\":\"[DESCRIPTION]\",\"thumbnail_url\":\"[IMAGE_URL]\",\"type\":\"html\",\"thumbnail_width\":\"[IMAGE_W]\",\"thumbnail_height\":\"[IMAGE_H]\"}";

        private BEService2 m_serviceV2;
        private MR_auth_login m_login;
        public static GCONST GCONST = new GCONST();
        private RunTimeData RT;

        private string m_email;
        private string m_password;

        private string SessionToken
        {
            get { return m_login.session_token; }
        }

        public PostService(string email, string password)
        {
            RT = new RunTimeData();

            m_email = email;
            m_password = password;
        }

        #region Login

        public bool Login()
        {
            m_serviceV2 = new BEService2();

            m_login = m_serviceV2.auth_login(m_email, m_password);

            if (m_login == null)
                return false;

            if (m_login.status != "200")
                return false;

            RT.Reset();

            CheckStation(m_login.stations);

            //êË®≠Áæ§Á
            RT.CurrentGroupID = m_login.groups[0].group_id;

            return true;
        }

        private void CheckStation(List<Station> stations)
        {
            if (stations != null)
            {
                foreach (Station _station in stations)
                {
                    if (_station.status == "connected")
                    {
                        string _ip = _station.location;

                        if (_ip.EndsWith("/"))
                            _ip = _ip.Substring(0, _ip.Length - 1);

                        BEService2.StationIP = _ip;

                        RT.IsStationOK = true;

                        return;
                    }
                }
            }

            RT.IsStationOK = false;
        }

        #endregion

        #region Post Text

        public void PostText(string text)
        {
            MR_posts_new _np = Post_CreateNewPost(text, "", "", "text");
            if (_np == null)
                throw new Exception("create default text post failure");
        }

        #endregion

        #region Post Link

        public void PostLink(string text, string previews)
        {
            MR_posts_new _np = Post_CreateNewPost(text, "", previews, "link");

            if (_np == null)
                throw new Exception("create default web link post failure");
        }

        public string GetPreview(string url, string title, string description, string imageURL, string image_w, string image_h)
        {
            string _s = PREVIEW;

            _s = _s.Replace("[URL]", url);
            _s = _s.Replace("[TITLE]", title);
            _s = _s.Replace("[DESCRIPTION]", description);
            _s = _s.Replace("[IMAGE_URL]", imageURL);
            _s = _s.Replace("[IMAGE_W]", image_w);
            _s = _s.Replace("[IMAGE_H]", image_h);

            return _s;
        }

        #endregion

        #region Post Photo

        public void PostPhotos(string text, List<string> images)
        {
            string _files = postImages(images);

            MR_posts_new _np = Post_CreateNewPost(text, _files, "", "image");

            if (_np == null)
                throw new Exception("create default photo post failure");
        }

        private string postImages(List<string> images)
        {
            string _ids = "[";

            foreach (string _file in images)
            {
                try
                {
                    MR_attachments_upload _uf = File_UploadFile(new FileInfo(_file).Name, _file, "", true);

                    if (_uf == null)
                    {
                        return "";
                    }

                    _ids += "\"" + _uf.object_id + "\"" + ",";
                }
                catch
                {
                    return "";
                }
            }

            _ids = _ids.Substring(0, _ids.Length - 1); // ªÊÄÂæå‰,"
            _ids += "]";

            return (_ids);
        }

        #endregion

        #region Post Doc

        public void PostDocs(string text, List<string> docs)
        {
            string _files = doPostDocs(docs);

            MR_posts_new _np = Post_CreateNewPost(text, _files, "", "doc");

            if (_np == null)
                throw new Exception("create default doc post failure");
        }

        private string doPostDocs(List<string> docs)
        {
            string _ids = "[";

            foreach (string _path in docs)
            {
                try
                {
                    MR_attachments_upload _uf = File_UploadFile(new FileInfo(_path).Name, _path, "", false);

                    if (_uf == null)
                    {
                        return "";
                    }

                    _ids += "\"" + _uf.object_id + "\"" + ",";
                }
                catch
                {
                    return "";
                }
            }

            _ids = _ids.Substring(0, _ids.Length - 1); // ªÊÄÂæå‰,"
            _ids += "]";

            return (_ids);
        }

        #endregion

        #region API

        public string attachments_getRedirectURL(string orgURL, string object_id, bool isImage)
        {
            return AttachmentUrlUtility.GetRedirectURL(orgURL, SessionToken, object_id, isImage);
        }

        public string attachments_getRedirectURL_Image(Attachment a, string imageType, out string url,
                                                       out string fileName)
        {
            return AttachmentUrlUtility.GetRedirectURL_Image(SessionToken, a, imageType, out url,
                                                                              out fileName);
        }

        public MR_posts_new Post_CreateNewPost(string text, string files, string previews, string type)
        {
            MR_posts_new _postsNew = m_serviceV2.posts_new(SessionToken, RT.CurrentGroupID, text, files, previews, type);

            if ((_postsNew != null) && (_postsNew.status == "200"))
            {
                return _postsNew;
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
                if (RT.IsStationOK) //Â¶ÇÊâStationá‰≥Â ¶ÂÂ∞±‰512‰∏≠Â
                {
                    _attachmentsUpload = m_serviceV2.attachments_upload(SessionToken, RT.CurrentGroupID, filePath, text,
                                                                        "", "image", "origin", object_id);
                }
                else
                {
                    _resizedImageFilePath = ResizeImage(filePath, text, "512", 50);
                    _attachmentsUpload = m_serviceV2.attachments_upload(SessionToken, RT.CurrentGroupID,
                                                                        _resizedImageFilePath, text, "", "image",
                                                                        "medium", object_id);
                }
            }
            else
            {
                _attachmentsUpload = m_serviceV2.attachments_upload(SessionToken, RT.CurrentGroupID, filePath, text, "",
                                                                    "doc", "", "");
            }

            if ((_attachmentsUpload != null) && (_attachmentsUpload.status == "200"))
            {
                // Â¶ÇÊ≥‰∏≠ñÂà∞Cloud, áËäÂñCacheËµ∑‰, ÂæÖÊStation®ÂÇ≥üÂ
                if (_resizedImageFilePath != string.Empty)
                {
                    string _ext = ".jpg";

                    int _idx = text.IndexOf(".");

                    if (_idx != -1)
                        _ext = text.Substring(_idx);

                    string _originCacheFile = GCONST.ImageUploadCachePath + _attachmentsUpload.object_id + _ext;
                    File.Copy(filePath, _originCacheFile);
                }

                return _attachmentsUpload;
            }

            return null;
        }

        #endregion

        #region Resize Image

        public static string ResizeImage(string orgImageFilePath, string fileName, string resizeRatio, int ratio)
        {
            string _resize = resizeRatio;

            if (_resize.Equals(string.Empty) || _resize.Equals("100%"))
                return orgImageFilePath;

            int _longestSide = int.Parse(_resize);

            try
            {
                Bitmap _bmp = ResizeImage_Impl(orgImageFilePath, _longestSide);

                if (_bmp == null)
                    return orgImageFilePath;

                if ((_bmp.Height < _longestSide) && (_bmp.Width < _longestSide))
                    return orgImageFilePath;

                string _newPath = GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" +
                                  fileName;


                // Create parameters
                EncoderParameters _params = new EncoderParameters(1);

                // Set quality (100)
                _params.Param[0] = new EncoderParameter(Encoder.Quality, ratio);

                // Create encoder info
                ImageCodecInfo _codec = null;

                foreach (ImageCodecInfo _codectemp in ImageCodecInfo.GetImageDecoders())
                {
                    if (_codectemp.MimeType == FileUtility.GetMimeType(new FileInfo(fileName)))
                    {
                        _codec = _codectemp;
                        break;
                    }
                }

                // Check
                if (_codec == null)
                    throw new Exception("Codec not found!");

                _bmp.Save(_newPath, _codec, _params);

                return _newPath;
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message);
            }

            return orgImageFilePath;
        }

        private static Bitmap ResizeImage_Impl(string imagePath, int longestSide)
        {
            Bitmap _newImage = null;
            Bitmap _image = null;

            try
            {
                _image = new Bitmap(imagePath);

                float _scale = (_image.Width > _image.Height
                                    ? (longestSide)/((float) _image.Width)
                                    : (longestSide)/((float) _image.Height));

                int _width = (int) (_image.Width*_scale);
                int _height = (int) (_image.Height*_scale);
                _newImage = new Bitmap(_width, _height);

                Graphics _g = Graphics.FromImage(_newImage);
                _g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                _g.DrawImage(_image, new Rectangle(0, 0, _width, _height), 0, 0, _image.Width, _image.Height,
                             GraphicsUnit.Pixel);
                _g.Dispose();
            }
            catch
            {
            }
            finally
            {
                if ((_image != null))
                    _image.Dispose();
            }

            return _newImage;
        }

        #endregion
    }
}
