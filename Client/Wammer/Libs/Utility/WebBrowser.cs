#region

using System;
using Microsoft.Win32;

#endregion

namespace Waveface
{
    public class WebBrowserUtility
    {
        // method for retrieving the users default web browser
        public static string GetSystemDefaultBrowser()
        {
            string _name = string.Empty;
            RegistryKey _regKey = null;

            try
            {
                //set the registry key we want to open
                _regKey = Registry.ClassesRoot.OpenSubKey("HTTP\\shell\\open\\command", false);

                //get rid of the enclosing quotes
                _name = _regKey.GetValue(null).ToString().ToLower().Replace("" + (char)34, "");

                //check to see if the value ends with .exe (this way we can remove any command line arguments)
                if (!_name.EndsWith("exe"))
                    //get rid of all command line arguments (anything after the .exe must go)
                    _name = _name.Substring(0, _name.LastIndexOf(".exe") + 4);
            }
            catch
            {
            }
            finally
            {
                //check and see if the key is still open, if so then close it
                if (_regKey != null)
                    _regKey.Close();
            }

            return _name;
        }
    }
}