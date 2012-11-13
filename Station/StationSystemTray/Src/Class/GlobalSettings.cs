using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
    public class GlobalSettings
    {
        #region Const
        private const string DEFAULT_APP_ID = "WavefaceStreamApp";
        private const string DEFAULT_MUTEX_TRAY_NAME = "StationSystemTray";
        private const string DEFAULT_MESSAGE_RECEIVER_NAME = "SystemTrayMessageReceiver";
        private const string DEFAULT_CLIENT_MESSAGE_RECEIVER_NAME = "WindowsClientMessageReceiver";
        #endregion

        #region Static Var
        private static GlobalSettings _instance;
        #endregion


        #region Var
        private String _appID = DEFAULT_APP_ID;
        private String _mutexTrayName = DEFAULT_MUTEX_TRAY_NAME;
        private String _messageReceiverName = DEFAULT_MESSAGE_RECEIVER_NAME;
        private String _clientMessageReceiverName = DEFAULT_CLIENT_MESSAGE_RECEIVER_NAME;
        #endregion


        #region Public Static Property
        public static GlobalSettings Instance
        { 
            get
            {
                return _instance ?? (_instance = new GlobalSettings());
            }
        }
        #endregion


        #region Property
        public String APPID
        {
            get
            {
                return _appID;
            }
            set
            {
                _appID = value;
            }
        }

        public String MutexTrayName 
        {
            get
            {
                return _mutexTrayName;
            }
            set
            {
                _mutexTrayName = value;
            }
        }

        public String MessageReceiverName 
        {
            get
            {
                return _messageReceiverName;
            }
            set
            {
                _messageReceiverName = value;
            }
        }

        public String ClientMessageReceiverName
        {
            get
            {
                return _clientMessageReceiverName;
            }
            set
            {
                _clientMessageReceiverName = value;
            }
        }
        #endregion


        #region Constructor
        private GlobalSettings()
        {

        }
        #endregion
    }
}
