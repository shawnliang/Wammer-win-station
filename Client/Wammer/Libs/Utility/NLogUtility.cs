
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

        public static void WebException(Logger logger, WebException e, string message)
        {
            string _s = "[" + message + "]" + "\n" +
                        GetExceptionMessage(e) + "\n";

            _s += "[Status]:" + e.Status.ToString() + "\n";

            if (e.InnerException != null)
                _s = _s + "[InnerException]\n" + GetExceptionMessage(e.InnerException) + "\n";

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
