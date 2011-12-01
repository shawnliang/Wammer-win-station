using System;
using System.Collections.Generic;

namespace Waveface
{
    class Program
    {
        static void Main(string[] args)
        {
            DefaultPosts _defaultPosts = new DefaultPosts("ren.cheng@waveface.com", "123456");

            if (_defaultPosts.Login())
            {
                Console.WriteLine("Login OK!");

                //
                 _defaultPosts.PostText("[Default TextPost]:" + DateTime.Now.ToString());
                 Console.WriteLine("PostText OK!");

                //
                List<string> _photos = new List<string>();
                _photos.Add("demo1.jpg");
                //_photos.Add("demo2.jpg");
                _defaultPosts.PostPhotos("[Default Photos Post]:" + DateTime.Now.ToString(), _photos);
                Console.WriteLine("PostPhotos OK!");

                //
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
        }
    }
}
