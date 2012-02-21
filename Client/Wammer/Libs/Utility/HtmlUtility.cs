#region

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace Waveface
{
    public class HtmlUtility
    {
        public static string URL_RegExp_Pattern =
            "(?i)\\b((?:https?://|www\\d{0,3}[.]|[a-z0-9.\\-]+[.][a-z]{2,4}/)(?:[^\\s()<>]+|\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\))+(?:\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)|[^\\s`!()\\[\\]{};:'\".,<>?«»“”‘’]))";

        public static string TrimScript(string htmlDocText)
        {
            string _bodyText;
            string trimJavascript = "<script type=\"text/javascript\">(.*?)</script>";
            Regex _regexTrimJs = new Regex(trimJavascript);
            _bodyText = _regexTrimJs.Replace(htmlDocText, "");
            return _bodyText;
        }

        public static string MakeLink(string txt, List<string> clickableUrl)
        {
            Regex _r = new Regex(URL_RegExp_Pattern, RegexOptions.None);

            MatchCollection _ms1 = _r.Matches(txt);

            foreach (Match _m in _ms1)
            {
                if (!clickableUrl.Contains(_m.Value))
                {
                    clickableUrl.Add(_m.Value);
                    clickableUrl.Add("http://" + _m.Value);
                    clickableUrl.Add("https://" + _m.Value);
                }
            }

            return _r.Replace(txt, "<a href=\"$1\" target=\"&#95;blank\">$1</a>").Replace("href=\"www", "href=\"http://www").Replace("href=\"WWW", "href=\"http://www");
        }

        public static string RemoveClassTag(string html)
        {
            string _html = html;

            int _idxS = 0;
            int _idxE1 = 0;
            int _idxE2 = 0;
            string _buf;

            while (true)
            {
                try
                {
                    _idxS = _html.IndexOf("class=", StringComparison.OrdinalIgnoreCase);

                    if (_idxS == -1)
                        break;

                    _buf = _html.Substring(_idxS + "class=".Length);

                    _idxE1 = _buf.IndexOf(">", StringComparison.OrdinalIgnoreCase);
                    _idxE2 = _buf.IndexOf(" ", StringComparison.OrdinalIgnoreCase);

                    _html = _html.Substring(0, _idxS - 1) + _buf.Substring(Math.Min(_idxE1, _idxE2));
                }
                catch
                {
                    return html;
                }
            }

            return _html;
        }

        public static string RemoveStyleTag1(string source)
        {
            string _html = source;

            int _idxS;
            int _idxE;
            string _buf;

            while (true)
            {
                try
                {
                    _idxS = _html.IndexOf("style=\"", StringComparison.OrdinalIgnoreCase);

                    if (_idxS == -1)
                        break;

                    _buf = _html.Substring(_idxS + "style=\"".Length);

                    _idxE = _buf.IndexOf("\"", StringComparison.OrdinalIgnoreCase);

                    _html = _html.Substring(0, _idxS - 1) + _buf.Substring(_idxE + 1);
                }
                catch
                {
                    return source;
                }
            }

            return _html;
        }

        public static string RemoveStyleTag2(string source)
        {
            string _ret = source;

            _ret = Regex.Replace(_ret,
                                 @"<( )*style([^>])*>", "<style>",
                                 RegexOptions.IgnoreCase);

            _ret = Regex.Replace(_ret,
                                 @"(<( )*(/)( )*style( )*>)", "</style>",
                                 RegexOptions.IgnoreCase);

            _ret = Regex.Replace(_ret,
                                 "(<style>).*(</style>)", string.Empty,
                                 RegexOptions.IgnoreCase);

            return _ret;
        }

        public static string StripHTML(string source)
        {
            try
            {
                string _ret;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                _ret = source.Replace("\r", " ");

                // Replace line breaks with space
                // because browsers inserts space
                _ret = _ret.Replace("\n", " ");

                // Remove step-formatting
                _ret = _ret.Replace("\t", string.Empty);

                // Remove repeating spaces because browsers ignore them
                _ret = Regex.Replace(_ret, @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                _ret = Regex.Replace(_ret,
                                     @"<( )*head([^>])*>", "<head>",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"(<( )*(/)( )*head( )*>)", "</head>",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     "(<head>).*(</head>)", string.Empty,
                                     RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                _ret = Regex.Replace(_ret,
                                     @"<( )*script([^>])*>", "<script>",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"(<( )*(/)( )*script( )*>)", "</script>",
                                     RegexOptions.IgnoreCase);

                _ret = Regex.Replace(_ret,
                                     @"(<script>).*(</script>)", string.Empty,
                                     RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                _ret = Regex.Replace(_ret,
                                     @"<( )*style([^>])*>", "<style>",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"(<( )*(/)( )*style( )*>)", "</style>",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     "(<style>).*(</style>)", string.Empty,
                                     RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                _ret = Regex.Replace(_ret,
                                     @"<( )*td([^>])*>", "\t",
                                     RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                _ret = Regex.Replace(_ret,
                                     @"<( )*br( )*>", "\r",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"<( )*li( )*>", "\r",
                                     RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                _ret = Regex.Replace(_ret,
                                     @"<( )*div([^>])*>", "\r\r",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"<( )*tr([^>])*>", "\r\r",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"<( )*p([^>])*>", "\r\r",
                                     RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                _ret = Regex.Replace(_ret,
                                     @"<[^>]*>", string.Empty,
                                     RegexOptions.IgnoreCase);

                // replace special characters:
                _ret = Regex.Replace(_ret,
                                     @" ", " ",
                                     RegexOptions.IgnoreCase);

                _ret = Regex.Replace(_ret,
                                     @"&bull;", " * ",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"&lsaquo;", "<",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"&rsaquo;", ">",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"&trade;", "(tm)",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"&frasl;", "/",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"&lt;", "<",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"&gt;", ">",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"&copy;", "(c)",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     @"&reg;", "(r)",
                                     RegexOptions.IgnoreCase);

                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                _ret = Regex.Replace(_ret,
                                     @"&(.{2,6});", string.Empty,
                                     RegexOptions.IgnoreCase);

                // make line breaking consistent
                _ret = _ret.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                _ret = Regex.Replace(_ret,
                                     "(\r)( )+(\r)", "\r\r",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     "(\t)( )+(\t)", "\t\t",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     "(\t)( )+(\r)", "\t\r",
                                     RegexOptions.IgnoreCase);
                _ret = Regex.Replace(_ret,
                                     "(\r)( )+(\t)", "\r\t",
                                     RegexOptions.IgnoreCase);

                // Remove redundant tabs
                _ret = Regex.Replace(_ret,
                                     "(\r)(\t)+(\r)", "\r\r",
                                     RegexOptions.IgnoreCase);

                // Remove multiple tabs following a line break with just one tab
                _ret = Regex.Replace(_ret,
                                     "(\r)(\t)+", "\r\t",
                                     RegexOptions.IgnoreCase);

                // Initial replacement target string for line breaks
                string _breaks = "\r\r\r";

                // Initial replacement target string for tabs
                string _tabs = "\t\t\t\t\t";

                for (int _index = 0; _index < _ret.Length; _index++)
                {
                    _ret = _ret.Replace(_breaks, "\r\r");
                    _ret = _ret.Replace(_tabs, "\t\t\t\t");
                    _breaks = _breaks + "\r";
                    _tabs = _tabs + "\t";
                }

                // That's it.
                return _ret;
            }
            catch
            {
                return source;
            }
        }

        public static List<string> FetchImagesPath(string html)
        {
            string _htmlData = html;

            List<string> _imageList = new List<string>();

            if (_htmlData != string.Empty)
            {
                string _imageHtmlCode = "<img";
                string _imageSrcCode = @"src=""";

                int _index = _htmlData.IndexOf(_imageHtmlCode, StringComparison.OrdinalIgnoreCase);

                while (_index != -1)
                {
                    //Remove previous data
                    _htmlData = _htmlData.Substring(_index);

                    //Find the location of the two quotes that mark the image's location
                    int _brackedEnd = _htmlData.IndexOf(">", StringComparison.OrdinalIgnoreCase); //make sure data will be inside img tag
                    int _start = _htmlData.IndexOf(_imageSrcCode, StringComparison.OrdinalIgnoreCase) + _imageSrcCode.Length;
                    int _end = _htmlData.IndexOf("\"", _start + 1, StringComparison.OrdinalIgnoreCase);

                    //Extract the line
                    if ((_end > _start) && (_start < _brackedEnd))
                    {
                        string _loc = _htmlData.Substring(_start, _end - _start);

                        if (!_imageList.Contains(_loc))
                            _imageList.Add(_loc);
                    }

                    //Move index to next image location
                    if (_imageHtmlCode.Length < _htmlData.Length)
                        _index = _htmlData.IndexOf(_imageHtmlCode, _imageHtmlCode.Length, StringComparison.OrdinalIgnoreCase);
                    else
                        _index = -1;
                }
            }

            return _imageList;
        }
    }
}