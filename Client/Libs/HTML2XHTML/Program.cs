#region

using System;
using System.Net;
using System.Xml;

#endregion

namespace Waveface.Util
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            WebClient _webClient = new WebClient();
            String _strHtmlContent = _webClient.DownloadString("http://www.codeproject.com");
            String _strXhtmlContent = XMLUtil.HTML2XHTML(_strHtmlContent);

            Console.WriteLine(_strXhtmlContent);

            Console.WriteLine("************************************************************");

            XmlDocument _xmlDoc = new XmlDocument();
            _xmlDoc.XmlResolver = new XHTMLResolver();
            _xmlDoc.LoadXml(_strXhtmlContent);

            Console.ReadKey();
        }
    }
}