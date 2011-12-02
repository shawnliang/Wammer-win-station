
using System;
using System.Collections.Generic;
using System.Xml;

namespace Waveface
{
    public class DefaultPosts
    {
        private PostService m_postService;
        private bool m_loginOK;

        public static void Main(string[] args)
        {
            DefaultPosts _defaultPosts = new DefaultPosts();

            try
            {
                if (args.Length > 1)
                    _defaultPosts.AutoPost(args[0], args[1]);
                else
                    _defaultPosts.AutoPost("ren.cheng@waveface.com", "123456");
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);

                Console.ReadLine();
            }

            Console.WriteLine("Post OK!");
            Console.ReadLine();
        }

        private void AutoPost(string email, string password)
        {
            m_postService = new PostService(email, password);
            m_loginOK = m_postService.Login();

            if (m_loginOK)
            {
                Console.WriteLine("Login OK!");
            }
            else
            {
                Console.WriteLine("Login Error!");
                return;
            }

            //
            XmlDocument _doc = new XmlDocument();
            _doc.Load("DefaultPosts.xml");
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

            Console.WriteLine("Exit");
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
                _content = System.IO.File.ReadAllText(_file);
            }

            m_postService.PostText("[Default TextPost]:" + DateTime.Now.ToString() + "\n" + _content);

            Console.WriteLine("PostText OK!");
        }

        private void doDocPost(XmlNode node)
        {
            string _file = node["file"].InnerText;
            string _content = node["content"].InnerText;

            List<string> _docs = new List<string>();
            _docs.Add(_file);
            m_postService.PostDocs("[Default Docs Post]:" + DateTime.Now.ToString() + "\n" + _content, _docs);

            Console.WriteLine("PostDocs OK!");
        }

        private void doImagePost(XmlNode node)
        {
            XmlNodeList _nodes = node.SelectNodes("//files/file");

            List<string> _photos = new List<string>();

            foreach (XmlNode _node in _nodes)
            {
                _photos.Add(_node.InnerText);
            }

            string _content = node["content"].InnerText;

            m_postService.PostPhotos("[Default Photos Post]:" + DateTime.Now.ToString() + "\n" + _content, _photos);

            Console.WriteLine("PostPhotos OK!");
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
            m_postService.PostLink("[Default Docs Post]:" + DateTime.Now.ToString() + "\n" + _content, _preview);

            Console.WriteLine("PostLink OK!");
        }
    }
}
