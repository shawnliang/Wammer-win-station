﻿
using System;
using System.Net;
using NLog;

namespace Waveface
{
    public class NLogUtility
    {
        public static void Exception(Logger logger, Exception e, string message)
        {
            string _s = "[" + message + "]" + "\n" +
                        GetExceptionMessage(e) + "\n";

            if (e.InnerException != null)
                _s = _s + "[InnerException]\n" + GetExceptionMessage(e.InnerException) + "\n";

            logger.Error(_s);
        }

        public static void Exception_Warn(Logger logger, Exception e, string message, string message2)
        {
            string _s = "[" + message + "]" + "\n" + message2 + "\n" +
                        GetExceptionMessage(e) + "\n";

            if (e.InnerException != null)
                _s = _s + "[InnerException]\n" + GetExceptionMessage(e.InnerException) + "\n";

            logger.Warn(_s);
        }

        public static void WebException(Logger logger, WebException e, string message, bool useWarnLog)
        {
            string _s = "[" + message + "]" + "\n" +
                        GetExceptionMessage(e) + "\n";

            HttpWebResponse _res = (HttpWebResponse)e.Response;

            if (_res != null)
            {
                _s += "[StatusCode]:" + _res.StatusCode.ToString() + "\n";
                _s += "[StatusDescription]:" + _res.StatusDescription + "\n";
            }

            if (e.InnerException != null)
                _s = _s + "[InnerException]\n" + GetExceptionMessage(e.InnerException) + "\n";

            if (useWarnLog)
                logger.Warn(_s);
            else
                logger.Error(_s);
        }

        private static string GetExceptionMessage(Exception e)
        {
            return
                "[Message]:" + e.Message + "\n" +
                "[Source]:" + e.Source + "\n" +
                "[StackTrace]:" + e.StackTrace;
        }
    }
}
