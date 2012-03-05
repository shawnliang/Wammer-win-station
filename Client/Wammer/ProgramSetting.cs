using System.Security.Cryptography;
using System.Configuration;
using System.Text;
using System;


namespace Waveface
{
    //public class ProgramSetting: ApplicationSettingsBase
    //{
    //    private static byte[] secret = { 1, 2, 3, 4, 5, 6 };

    //    [UserScopedSetting]
    //    [DefaultSettingValue("")]
    //    public string StationToken
    //    {
    //        get { return (string)this["StationToken"]; }
    //        set { this["StationToken"] = value; }
    //    }

    //    [UserScopedSetting]
    //    [DefaultSettingValue("false")]
    //    public bool RememberPassword
    //    {
    //        get { return (bool)this["RememberPassword"]; }
    //        set { this["RememberPassword"] = value; }
    //    }

    //    [UserScopedSetting]
    //    [DefaultSettingValue("")]
    //    public string Email
    //    {
    //        get { 
    //            string ret = (string)this["Email"];
    //            if (string.IsNullOrEmpty(ret))
    //            {
    //                return (string)StationRegHelper.GetValue("driver", "");
    //            }
    //            return ret;
    //        }
    //        set { this["Email"] = value; }
    //    }

    //    [UserScopedSetting]
    //    [DefaultSettingValue("")]
    //    public string Password
    //    {
    //        get { return (string)this["Password"]; }
    //        set { this["Password"] = value; }
    //    }

    //    [UserScopedSetting]
    //    [DefaultSettingValue("")]
    //    public string EncryptedPassword
    //    {
    //        get {
    //            if (string.IsNullOrEmpty((string)this["EncryptedPassword"]))
    //                return (string)this["EncryptedPassword"];
    //            else
    //                return Encoding.Default.GetString(ProtectedData.Unprotect(Convert.FromBase64String((string)this["EncryptedPassword"]), secret, DataProtectionScope.CurrentUser));
    //        }
    //        set {
    //            byte[] bpasswd = ProtectedData.Protect(Encoding.Default.GetBytes(value), secret, DataProtectionScope.CurrentUser);
    //            this["EncryptedPassword"] = Convert.ToBase64String(bpasswd);
    //        }
    //    }

    //    [UserScopedSetting]
    //    [DefaultSettingValue("false")]
    //    public bool IsUpgraded
    //    {
    //        get { return (bool)this["IsUpgraded"]; }
    //        set { this["IsUpgraded"] = value; }
    //    }
    //}

    public enum QuitOption
    {
        Logout,
        QuitProgram
    }
}
