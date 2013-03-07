#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

#endregion

namespace IsolationAPI
{
    internal class AppContainer
    {
        private static string s_sThisMachine = Environment.MachineName + "\\";

        private SecurityIdentifier m_appContainerSid;
        private SecurityIdentifier m_userSid;
        private List<string> m_listUsernames = new List<string>();
        private bool m_bWasLoopbackExemptAtLastCheck;
        private string m_appContainerName;
        private string m_displayName;
        private string m_description;
        private string m_packageFullName;
        private string[] m_binaries;
        private IntPtr m_appContainerRawSid = IntPtr.Zero;

        #region Properties

        internal bool WasLoopbackExemptAtLastCheck
        {
            get { return m_bWasLoopbackExemptAtLastCheck; }
            set { m_bWasLoopbackExemptAtLastCheck = value; }
        }

        internal IntPtr AppContainerRawSid
        {
            get { return m_appContainerRawSid; }
        }

        internal string PackageFullName
        {
            get { return m_packageFullName ?? "(No Package Name)"; }
        }

        internal string AppContainerSid
        {
            get
            {
                if (null == m_appContainerSid)
                {
                    return "(No AC SID)";
                }

                return m_appContainerSid.ToString();
            }
        }

        internal string AppContainerName
        {
            get { return m_appContainerName ?? "(No AC Name)"; }
        }

        internal string DisplayName
        {
            get { return m_displayName ?? "(No DisplayName)"; }
        }

        internal string Description
        {
            get { return m_description ?? "(No Description)"; }
        }

        public static uint uiLastError { get; set; }

        #endregion

        internal string GetUsers()
        {
            if (m_listUsernames.Count < 1)
            {
                return "(No User SID)";
            }

            return string.Join("; ", m_listUsernames);
        }

        internal AppContainer(INET_FIREWALL_APP_CONTAINER ifacNative)
        {
            m_appContainerRawSid = ifacNative.appContainerSid;

            try
            {
                m_appContainerSid = new SecurityIdentifier(m_appContainerRawSid);
            }
            catch (Exception _e)
            {
                Console.WriteLine("Unexpected:" + "Failed to parse AppContainerSID\n\n" + _e.Message);
                m_appContainerSid = null;
            }

            try
            {
                m_userSid = new SecurityIdentifier(ifacNative.userSid);
                AddUser(m_userSid);
            }
            catch (Exception _e)
            {
                Console.WriteLine("Unexpected:" + "Failed to parse userSID\n\n" + _e.Message);
                m_userSid = null;
            }

            m_appContainerName = ifacNative.appContainerName;
            m_displayName = LoopbackIsolation.LoadIndirectString(ifacNative.displayName);
            m_description = LoopbackIsolation.LoadIndirectString(ifacNative.description);
            m_packageFullName = ifacNative.packageFullName;
            INET_FIREWALL_AC_BINARIES _iNetFirewallAcBinaries = ifacNative.binaries;

            if (_iNetFirewallAcBinaries.count > 0u && IntPtr.Zero != _iNetFirewallAcBinaries.binaries)
            {
                m_binaries = new string[_iNetFirewallAcBinaries.count];
                long _num = _iNetFirewallAcBinaries.binaries.ToInt64();
                int k = 0;

                while (k < _iNetFirewallAcBinaries.count)
                {
                    m_binaries[k] = Marshal.PtrToStringUni(Marshal.ReadIntPtr((IntPtr)_num));
                    _num += IntPtr.Size;
                    k++;
                }
            }
        }

        private void AddUser(SecurityIdentifier oSID)
        {
            if (null == m_userSid)
            {
                AddUser("<null>");
                return;
            }

            string _text;

            try
            {
                _text = (m_userSid.Translate(typeof(NTAccount))).ToString();

                if (_text.StartsWith(s_sThisMachine))
                {
                    _text = _text.Substring(s_sThisMachine.Length);
                }
            }
            catch
            {
                _text = oSID.Value;
            }

            AddUser(_text);
        }

        private void AddUser(string sNewUser)
        {
            m_listUsernames.Add(sNewUser);
            m_listUsernames.Sort();
        }

        internal static void ExemptFromLoopbackBlocking(AppContainer[] oACToExempt)
        {
            uint _pdwCntACs = (uint)oACToExempt.Length;
            SID_AND_ATTRIBUTES[] _appContainerSids = new SID_AND_ATTRIBUTES[_pdwCntACs];
            int k = 0;

            for (int i = 0; i < oACToExempt.Length; i++)
            {
                _appContainerSids[k] = default(SID_AND_ATTRIBUTES);
                _appContainerSids[k].Attributes = 0u;
                _appContainerSids[k].Sid = oACToExempt[i].m_appContainerRawSid;
                k++;
            }

            uint j = LoopbackIsolation.NetIsoSetAppContainerConfig(_pdwCntACs, _appContainerSids);

            if (j == 0u)
            {
                return;
            }

            if (j == 5u)
            {
                throw new UnauthorizedAccessException();
            }

            throw new Exception(string.Format("Failed to set IsolationExempt AppContainers; call returned 0x{0:X}\r\n", j));
        }

        internal static void MarkLoopbackExemptAppContainers(AppContainer[] oACs)
        {
            for (int i = 0; i < oACs.Length; i++)
            {
                oACs[i].m_bWasLoopbackExemptAtLastCheck = false;
            }

            IntPtr _appContainerSids = IntPtr.Zero;
            uint _pdwCntACs;
            uint _num = LoopbackIsolation.NetIsoGetAppContainerConfig(out _pdwCntACs, out _appContainerSids);

            if (_num == 0u)
            {
                try
                {
                    if (_pdwCntACs > 0u)
                    {
                        long _value = _appContainerSids.ToInt64();
                        int k = 0;

                        while (k < _pdwCntACs)
                        {
                            SID_AND_ATTRIBUTES sID_AND_ATTRIBUTES = (SID_AND_ATTRIBUTES)Marshal.PtrToStructure((IntPtr)_value, typeof(SID_AND_ATTRIBUTES));
                            SecurityIdentifier _securityIdentifier;

                            try
                            {
                                _securityIdentifier = new SecurityIdentifier(sID_AND_ATTRIBUTES.Sid);
                            }
                            catch (Exception _e)
                            {
                                Console.WriteLine("Unexpected:" + "Unable to convert LoopbackExempt SID to SecurityIdentifier\n\n" + _e.Message);
                                k++;
                                continue;
                            }

                            for (int j = 0; j < oACs.Length; j++)
                            {
                                if (oACs[j].m_appContainerSid == _securityIdentifier)
                                {
                                    oACs[j].m_bWasLoopbackExemptAtLastCheck = true;
                                    break;
                                }
                            }

                            _value += Marshal.SizeOf(typeof(SID_AND_ATTRIBUTES));

                            k++;
                        }
                    }

                    return;
                }
                finally
                {
                    if (_appContainerSids != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(_appContainerSids);
                    }
                }
            }

            throw new Exception(string.Format("Failed to get IsolationExempt AppContainers; call returned 0x{0:X}\r\n", _num));
        }

        internal static AppContainer[] GetAppContainers(bool bForceBinaryNames)
        {
            Dictionary<string, AppContainer> _dic = null;
            uint _pdwCntACs;
            IntPtr _ppACs;
            uint _num = LoopbackIsolation.NetIsoEnumAppContainers(out _pdwCntACs, out _ppACs, bForceBinaryNames);

            if (_num != 0u)
            {
                uiLastError = _num;
                Trace.WriteLine(string.Format("Failed to enumerate AppContainers; returned 0x{0:X}", _num));
                return null;
            }

            try
            {
                if (_pdwCntACs > 0u)
                {
                    _dic = new Dictionary<string, AppContainer>();
                    long _value = _ppACs.ToInt64();
                    int k = 0;

                    while (k < (_pdwCntACs))
                    {
                        INET_FIREWALL_APP_CONTAINER _inetFirewallAppContainer = (INET_FIREWALL_APP_CONTAINER)Marshal.PtrToStructure((IntPtr)_value, typeof(INET_FIREWALL_APP_CONTAINER));
                        AppContainer _appContainer = new AppContainer(_inetFirewallAppContainer);

                        if (_dic.ContainsKey(_appContainer.AppContainerSid))
                        {
                            _dic[_appContainer.AppContainerSid].AddUser(_appContainer.GetUsers());
                        }
                        else
                        {
                            _dic.Add(_appContainer.AppContainerSid, _appContainer);
                        }

                        _value += Marshal.SizeOf(typeof(INET_FIREWALL_APP_CONTAINER));

                        k++;
                    }
                }
            }
            finally
            {
                LoopbackIsolation.NetIsoFreeAppContainers(_ppACs);
            }

            if (_dic == null)
            {
                return null;
            }

            AppContainer[] _appContainers = new AppContainer[_dic.Count];
            _dic.Values.CopyTo(_appContainers, 0);
            return _appContainers;
        }
    }
}