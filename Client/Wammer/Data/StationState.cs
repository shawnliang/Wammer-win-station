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
            MR_users_findMyStation _findMyStation;

            while (!m_exit)
            {
                try
                {
                    if (Main.Current.RT.Login == null)
                    {
                        Thread.Sleep(3000);
                        continue;
                    }

                    if (!Main.Current.RT.StationMode) //Staion不在
                    {
                        _findMyStation = Main.Current.RT.REST.Users_findMyStation();

                        if (_findMyStation != null)
                        {
                            //Test UPnP
                            string _ip = GetStationIP(_findMyStation.stations, true);

                            if (!string.IsNullOrEmpty(_ip))
                            {
                                if (Main.Current.RT.REST.CheckStationAlive(_ip))
                                {
                                    WService.StationIP = _ip;
                                    Main.Current.RT.StationMode = true;

                                    s_logger.Info("Station IP(UPnP):" + _ip);

                                    Thread.Sleep(30000);
                                    continue;
                                }
                            }

                            //Test Local IP
                            _ip = GetStationIP(_findMyStation.stations, false);

                            if (!string.IsNullOrEmpty(_ip))
                            {
                                if (Main.Current.RT.REST.CheckStationAlive(_ip))
                                {
                                    WService.StationIP = _ip;
                                    Main.Current.RT.StationMode = true;

                                    s_logger.Info("Station IP:" + _ip);

                                    Thread.Sleep(30000);
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!Main.Current.RT.REST.CheckStationAlive(WService.StationIP))
                        {
                            WService.StationIP = string.Empty;
                            Main.Current.RT.StationMode = false;

                            continue;
                        }
                    }

                    Thread.Sleep(30000);
                    continue;
                }
                catch (Exception _e)
                {
                    NLogUtility.Exception(s_logger, _e, "ThreadMethod");
                }
            }
        }

        private string GetStationIP(List<Station> stations, bool UPnP)
        {
            string _ip = string.Empty;

            if (stations != null)
            {
                foreach (Station _station in stations)
                {
                    if (_station.status == "connected")
                    {
                        if (UPnP)
                            _ip = _station.public_location;
                        else
                            _ip = _station.location;

                        if (_ip.EndsWith("/"))
                            _ip = _ip.Substring(0, _ip.Length - 1);

                        return _ip;
                    }
                }
            }

            return string.Empty;
        }

        public void Exit()
        {
            m_exit = true;
        }
    }
}