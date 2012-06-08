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
        public event ShowStationState_Delegate ShowStationState;

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private WorkItem m_workItem;
        private bool m_forceRetry;
        private int m_lastTimeout = 3000;

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
            while (true)
            {
                try
                {
                    if (Main.Current.RT.Login == null)
                    {
                        NotifyState(ConnectServiceStateType.NetworkDisconnected);

                        Delay(2);
                        continue;
                    }

                    if (Main.Current.RT.StationMode)
                    {
                        if (Main.Current.RT.REST.CheckStationAlive(WService.StationIP, m_lastTimeout))
                        {
                            Delay(10);
                            continue;
                        }
                        else
                        {
                            WService.StationIP = string.Empty;
                            Main.Current.RT.StationMode = false;

                            NotifyState(ConnectServiceStateType.Cloud);

							s_logger.Warn("Station Disappear");
                        }
                    }
                    else
                    {
                        NotifyState(ConnectServiceStateType.Cloud);

                        MR_users_findMyStation _findMyStation = Main.Current.RT.REST.Users_findMyStation();

                        s_logger.Trace("Call FindMyStation");

                        if (_findMyStation != null)
                        {
                            if (_findMyStation.stations != null)
                            {
                                //Test Local IP
                                string _ip = GetStationIP(_findMyStation.stations, false);

                                if (!string.IsNullOrEmpty(_ip))
                                {
                                    m_lastTimeout = 3000;

                                    if (Main.Current.RT.REST.CheckStationAlive(_ip, m_lastTimeout))
                                    {
                                        WService.StationIP = _ip;
                                        Main.Current.RT.StationMode = true;

                                        NotifyState(ConnectServiceStateType.Station_LocalIP);

										s_logger.Warn("Station IP:" + _ip);

                                        Delay(5);
                                        continue;
                                    }
                                }

                                //Test UPnP
                                _ip = GetStationIP(_findMyStation.stations, true);

                                m_lastTimeout = 6000;

                                if (!string.IsNullOrEmpty(_ip))
                                {
                                    if (Main.Current.RT.REST.CheckStationAlive(_ip, m_lastTimeout))
                                    {
                                        WService.StationIP = _ip;
                                        Main.Current.RT.StationMode = true;

                                        NotifyState(ConnectServiceStateType.Station_UPnP);

										s_logger.Warn("Station IP(UPnP):" + _ip);

                                        Delay(10);
                                        continue;
                                    }
                                }
                            }
                        }
                    }

                    Delay(60);
                }
                catch (Exception _e)
                {
                    NLogUtility.Exception_Warn(s_logger, _e, "ThreadMethod", "");
                }
            }
        }

        private void NotifyState(ConnectServiceStateType type)
        {
            if (ShowStationState != null)
                ShowStationState(type);
        }

        public void ForceRetry()
        {
            m_forceRetry = true;

            s_logger.Trace("ForceRetry()");
        }

        private void Delay(int sleepTime)
        {
            for (int i = 0; i < sleepTime; i++)
            {
                if (m_forceRetry)
                {
                    m_forceRetry = false;
                    break;
                }

                Thread.Sleep(1000);
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
                        {
                            if (string.IsNullOrEmpty(_station.public_location))
                                return string.Empty;
                            else
                                _ip = _station.public_location;
                        }
                        else
                        {
                            _ip = _station.location;
                        }

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