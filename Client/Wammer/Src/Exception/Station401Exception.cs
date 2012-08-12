#region

using System;

#endregion

namespace Waveface
{
    public class Station401Exception : Exception
    {
        public string ErrorMessage { get; set; }

        public Station401Exception()
        {
            ErrorMessage = base.Message;
        }

        public Station401Exception(string msg)
        {
            ErrorMessage = msg;
        }
    }
}