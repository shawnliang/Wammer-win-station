#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
#endregion

namespace StationSetup
{
    public class DefaultPosts
    {
        private PostService m_postService;
        private bool m_loginOK;

        public void AutoPost(string email, string password)
        {
            try
            {
                m_postService = new PostService(email, password);
                m_loginOK = m_postService.Login();

                if (!m_loginOK)
                    throw new Exception("Login failure");

                XmlDocument _doc = new XmlDocument();

                string culture = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation", "Culture", null);
                if (culture == null)
                    culture = "en-US";

                _doc.Load(string.Format(@"DefaultResources\DefaultPosts.{0}.xml", culture));

                XmlElement _root = _doc.DocumentElement;
                XmlNodeList _nodes = _root.SelectNodes("//posts/post");

                foreach (XmlNode _node in _nodes)
                {
                    string _type = _node.Attributes[0].Value;

                    switch (_type)
                    {
                        case "text":
                            doTextPost(_node);
                            break;

                        case "link":
                            doLinkPost(_node);
                            break;

                        case "image":
                            doImagePost(_node);
                            break;

                        case "doc":
                            doDocPost(_node);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to generate default posts.\r\n" + e, "Waveface",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void doTextPost(XmlNode node)
        {
            string _content = string.Empty;

            string _file = node["file"].InnerText;

            if (_file == string.Empty)
            {
                _content = node["content"].InnerText;
            }
            else
            {
                _content = File.ReadAllText(_file);
            }

            m_postService.PostText(_content);
        }

        private void doDocPost(XmlNode node)
        {
            string _file = node["file"].InnerText;
            string _content = node["content"].InnerText;

            List<string> _docs = new List<string>();
            _docs.Add(_file);
            m_postService.PostDocs(_content, _docs);
        }

        private void doImagePost(XmlNode node)
        {
            XmlNodeList _nodes = node.SelectNodes("files/file");

            List<string> _photos = new List<string>();

            foreach (XmlNode _node in _nodes)
            {
                _photos.Add(_node.InnerText);
            }

            string _content = node["content"].InnerText;

            m_postService.PostPhotos(_content, _photos);
        }

        private void doLinkPost(XmlNode node)
        {
            string _content = node["content"].InnerText;
            string _url = node["url"].InnerText;
            string _title = node["title"].InnerText;
            string _description = node["description"].InnerText;
            string _image_url = node["image_url"].InnerText;
            string _image_w = node["image_w"].InnerText;
            string _image_h = node["image_h"].InnerText;

            string _preview = m_postService.GetPreview(_url, _title, _description, _image_url, _image_w, _image_h);
            m_postService.PostLink(_content, _preview);
        }
    }
}