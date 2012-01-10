#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NLog;
using Newtonsoft.Json;
using Waveface.API.V2;

#endregion

namespace Waveface
{
    public class StationState
    {
        private static StationState s_current;
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private bool m_exit;

        #region Properties

        public static StationState Current
        {
            get
            {
                if (s_current == null)
                {
                    s_current = new StationState();
                }

                return s_current;
            }
            set { s_current = value; }
        }

        #endregion


        public void Start()
        {
            ThreadPool.QueueUserWorkItem(state => { ThreadMethod(); });
        }

        private void ThreadMethod()
        {
            MR_users_findMyStation _findMyStation = null;

            while (!m_exit)
            {
                if (Main.Current.RT.Login == null)
                {
                    Thread.Sleep(2000);
                    continue;
                }

                _findMyStation = Main.Current.RT.REST.Users_findMyStation(); 

                if(_findMyStation == null)
                {
                    Thread.Sleep(2000);
                    continue;
                }





                Thread.Sleep(2000);
            }
        }

        public void Exit()
        {
            m_exit = true;
        }
    }
}