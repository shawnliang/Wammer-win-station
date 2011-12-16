
using System;
using NLog;

namespace Waveface
{
    public class NLogUtility
    {
        public static void Exception(Logger logger, Exception e, string message)
        {
            string _s = "[" + message + "]" + "\n" +
                        GetExceptionMessage(e) + "\n";

            if(e.InnerException != null)
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
