
using System;
using System.Collections.Generic;
using System.Xml;

namespace Waveface
{
    class Program
    {
        private static DefaultPosts s_defaultPosts;
        private static bool s_loginOK;

        static void Main(string[] args)
        {
            s_defaultPosts = new DefaultPosts("ren.cheng@waveface.com", "123456");
            s_loginOK = s_defaultPosts.Login();

            if (s_loginOK)
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
                        //doTextPost(_node);
                        break;

                    case "link":
                        doLinkPost(_node);
                        break;

                    case "image":
                        //doImagePost(_node);
                        break;

                    case "doc":
                        doDocPost(_node);
                        break;
                }
            }


            /*
            DefaultPosts _defaultPosts = new DefaultPosts("ren.cheng@waveface.com", "123456");

            if (_defaultPosts.Login())
            {
                Console.WriteLine("Login OK!");

                _defaultPosts.PostText("[Default TextPost]:" + DateTime.Now.ToString());
                Console.WriteLine("PostText OK!");


                //
                string _preview = _defaultPosts.GetPreview("http://www.apple.com/", "蘋果電腦", "這是預設Post, 以Apple網站為範例",
                                                           "http://images.apple.com/home/images/hero.jpg", 579, 595);
                _defaultPosts.PostLink("[Default WebLink Post]:" + DateTime.Now.ToString(), _preview);
                Console.WriteLine("PostLink OK!");

                //
                List<string> _photos = new List<string>();
                _photos.Add("demo1.jpg");
                _photos.Add("demo2.jpg");
                _defaultPosts.PostPhotos("[Default Photos Post]:" + DateTime.Now.ToString(), _photos);
                Console.WriteLine("PostPhotos OK!");

                List<string> _docs = new List<string>();
                _docs.Add("demo.pdf");
                _defaultPosts.PostDocs("[Default Docs Post]:" + DateTime.Now.ToString(), _docs);
                Console.WriteLine("PostDocs OK!");
            }
            else
            {
                Console.WriteLine("Error");
            }

            Console.WriteLine("Exit");
            */
        }

        private static void doTextPost(XmlNode node)
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

            s_defaultPosts.PostText("[Default TextPost]:" + DateTime.Now.ToString() + "\n" + _content);

            Console.WriteLine("PostText OK!");
        }

        private static void doDocPost(XmlNode node)
        {
            string _file = node["file"].InnerText;
            string _content = node["content"].InnerText;

            List<string> _docs = new List<string>();
            _docs.Add(_file);
            s_defaultPosts.PostDocs("[Default Docs Post]:" + DateTime.Now.ToString() + "\n" + _content, _docs);
            
            Console.WriteLine("PostDocs OK!");
        }

        private static void doImagePost(XmlNode node)
        {
            XmlNodeList _nodes = node.SelectNodes("//files/file");

            List<string> _photos = new List<string>();

            foreach (XmlNode _node in _nodes)
            {
                _photos.Add(_node.InnerText);
            }

            string _content = node["content"].InnerText;

            s_defaultPosts.PostPhotos("[Default Photos Post]:" + DateTime.Now.ToString() + "\n" + _content, _photos);

            Console.WriteLine("PostPhotos OK!");
        }

        private static void doLinkPost(XmlNode node)
        {

        }
    }
}
