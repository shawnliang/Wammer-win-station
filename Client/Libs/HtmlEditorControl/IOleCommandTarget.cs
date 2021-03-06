#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace Waveface.Component.COM
{
    #region Enumeratons Constants and Struct Defintions

    // Information for the values of these constants is derived from the follwoing header files
    // MsHtmdid.h
    // MsHtmcid.h

    // a class defined for constant defintions realting to COM interop
    internal class CommandGroup
    {
        public static Guid CGID_MSHTML = new Guid("de4ba900-59ca-11cf-9592-444553540000");
    }

    // HRESULT codes for COM Interop
    // basic values and those used are defined
    internal struct HRESULT
    {
        public const int S_OK = 0;
        public const int S_FALSE = 1;

        public const int E_PENDING = unchecked((int) 0x8000000A);

        public const int E_NOTIMPL = unchecked((int) 0x80004001);
        public const int E_NOINTERFACE = unchecked((int) 0x80004002);
        public const int E_POINTER = unchecked((int) 0x80004003);
        public const int E_ABORT = unchecked((int) 0x80004004);
        public const int E_FAIL = unchecked((int) 0x80004005);

        public const int E_ACCESSDENIED = unchecked((int) 0x80070006);
        public const int E_HANDLE = unchecked((int) 0x80070006);
        public const int E_INVALIDARG = unchecked((int) 0x80070057);

        public const int E_UNEXPECTED = unchecked((int) 0x8000FFFF);
    }

    // Command options for the web browser ExecWB and Exec commands
    internal enum CommandOption
    {
        OLECMDEXECOPT_DODEFAULT = 0,
        OLECMDEXECOPT_PROMPTUSER = 1,
        OLECMDEXECOPT_DONTPROMPTUSER = 2,
        OLECMDEXECOPT_SHOWHELP = 3
    }

    // Options that can be defined that affect the browser operations
    // Set through the use of the Exec command
    internal enum CommandId
    {
        IDM_NOACTIVATENORMALOLECONTROLS = 2332,
        IDM_NOACTIVATEDESIGNTIMECONTROLS = 2333,
        IDM_NOACTIVATEJAVAAPPLETS = 2334,
        IDM_NOFIXUPURLSONPASTE = 2335
    }

    // OLECMD struct
    [ComVisible(true), StructLayout(LayoutKind.Sequential)]
    internal struct OLECMD
    {
        private int cmdID;
        private int cmdf;
    }

    #endregion
}

namespace Waveface.Component.COM.IOleCommandTarget
{
    #region COM Interop Interface Definitions

    // The implementations uses the PreserveSig attribute to prevent exception throwing
    // thus allowing the caller to analyze the HRESULT.
    // To revert to exception throwing remove the attribue and change the return type to void 

    // IOleCommandTarget interface 
    [ComVisible(true), ComImport, Guid("b722bccb-4e68-101b-a2bc-00aa00404770"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOleCommandTarget
    {
        [PreserveSig]
        int QueryStatus(
            ref Guid pguidCmdGroup,
            uint cCmds,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] OLECMD prgCmds,
            ref IntPtr pCmdText);

        [PreserveSig]
        int Exec(
            ref Guid pguidCmdGroup,
            uint nCmdID,
            uint nCmdExecOpt,
            ref object pvaIn,
            ref object pvaOut);
    }

    #endregion
}