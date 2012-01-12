#region

using System;
using System.Collections.Generic;
using System.Threading;
using NLog;
using Waveface.API.V2;

#endregion

namespace Waveface
{
    public class StationState
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private WorkItem m_workItem;

        public void Start()
        {
            m_workItem = AbortableThreadPool.QueueUserWorkItem(ThreadMethod, 0);
        }

        public WorkItemStatus AbortThread()
        {
            return AbortableThreadPool.Cancel(m_workItem, true);
        }

        private void ThreadMethod(object state)
        {
            MR_users_findMyStation _findMyStation;

            while (true)
            {
                try
                {
                    if (Main.Current.RT.Login == null)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }

                    if (Main.Current.RT.StationMode)
                    {
                        if (Main.Current.RT.REST.CheckStationAlive(WService.StationIP))
                        {
                            Thread.Sleep(15000);
                            continue;
                        }
                        else
                        {
                            WService.StationIP = string.Empty;
                            Main.Current.RT.StationMode = false;

                            s_logger.Info("Station Disappear");

                            continue;
                        }
                    }
                    else
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

                                    Thread.Sleep(15000);
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

                                    Thread.Sleep(15000);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            
                        }
                    }

                    Thread.Sleep(60000);
                    continue;
                }
                catch (Exception _e)
                {
                    NLogUtility.Exception_Warn(s_logger, _e, "ThreadMethod", "");
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
    }
}