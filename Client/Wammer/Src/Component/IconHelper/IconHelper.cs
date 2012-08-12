#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;

#endregion

namespace Waveface.Component.IconHelper
{
    #region IconReader

    // Provides static methods to read system icons for both folders and files.
    // [code]IconReader.GetFileIcon("c:\\general.xls");
    public class IconReader
    {
        #region FolderType enum

        // Options to specify whether folders should be in the open or closed state.
        public enum FolderType
        {
            Open = 0,
            Closed = 1
        }

        #endregion

        #region IconSize enum

        // Options to specify the size of icons to return.
        public enum IconSize
        {
            // Specify large icon - 32 pixels by 32 pixels.
            Large = 0,

            // Specify small icon - 16 pixels by 16 pixels.
            Small = 1
        }

        #endregion

        // Returns an icon for a given file - indicated by the name parameter.
        public static Icon GetFileIcon(string name, IconSize size, bool linkOverlay)
        {
            Shell32.SHFILEINFO _shfi = new Shell32.SHFILEINFO();
            uint _flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES;

            if (linkOverlay)
                _flags += Shell32.SHGFI_LINKOVERLAY;

            // Check the size specified for return.
            if (IconSize.Small == size)
            {
                _flags += Shell32.SHGFI_SMALLICON;
            }
            else
            {
                _flags += Shell32.SHGFI_LARGEICON;
            }

            Shell32.SHGetFileInfo(name,
                                  Shell32.FILE_ATTRIBUTE_NORMAL,
                                  ref _shfi,
                                  (uint)Marshal.SizeOf(_shfi),
                                  _flags);

            // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
            Icon _icon = (Icon)Icon.FromHandle(_shfi.hIcon).Clone();
            User32.DestroyIcon(_shfi.hIcon); // Cleanup
            return _icon;
        }

        // Used to access system folder icons.
        public static Icon GetFolderIcon(IconSize size, FolderType folderType)
        {
            // Need to add size check, although errors generated at present!
            uint _flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES;

            if (FolderType.Open == folderType)
            {
                _flags += Shell32.SHGFI_OPENICON;
            }

            if (IconSize.Small == size)
            {
                _flags += Shell32.SHGFI_SMALLICON;
            }
            else
            {
                _flags += Shell32.SHGFI_LARGEICON;
            }

            // Get the folder icon
            Shell32.SHFILEINFO _shfi = new Shell32.SHFILEINFO();
            Shell32.SHGetFileInfo("dummy",
                                  Shell32.FILE_ATTRIBUTE_DIRECTORY,
                                  ref _shfi,
                                  (uint)Marshal.SizeOf(_shfi),
                                  _flags);

            Icon.FromHandle(_shfi.hIcon); // Load the icon from an HICON handle

            // Now clone the icon, so that it can be successfully stored in an ImageList
            Icon _icon = (Icon)Icon.FromHandle(_shfi.hIcon).Clone();

            User32.DestroyIcon(_shfi.hIcon); // Cleanup
            return _icon;
        }
    }

    #endregion

    #region Shell32

    // Wraps necessary Shell32.dll structures and functions required to retrieve Icon Handles using SHGetFileInfo. Code
    // courtesy of MSDN Cold Rooster Consulting case study.
    // 
    // This code has been left largely untouched from that in the CRC example. The main changes have been moving
    // the icon reading code over to the IconReader type.
    public class Shell32
    {
        public const int MAX_PATH = 256;

        // Browsing for directory.
        public const uint BIF_RETURNONLYFSDIRS = 0x0001;
        public const uint BIF_DONTGOBELOWDOMAIN = 0x0002;
        public const uint BIF_STATUSTEXT = 0x0004;
        public const uint BIF_RETURNFSANCESTORS = 0x0008;
        public const uint BIF_EDITBOX = 0x0010;
        public const uint BIF_VALIDATE = 0x0020;
        public const uint BIF_NEWDIALOGSTYLE = 0x0040;
        public const uint BIF_USENEWUI = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
        public const uint BIF_BROWSEINCLUDEURLS = 0x0080;
        public const uint BIF_BROWSEFORCOMPUTER = 0x1000;
        public const uint BIF_BROWSEFORPRINTER = 0x2000;
        public const uint BIF_BROWSEINCLUDEFILES = 0x4000;
        public const uint BIF_SHAREABLE = 0x8000;

        public const uint SHGFI_ICON = 0x000000100; // get icon
        public const uint SHGFI_DISPLAYNAME = 0x000000200; // get display name
        public const uint SHGFI_TYPENAME = 0x000000400; // get type name
        public const uint SHGFI_ATTRIBUTES = 0x000000800; // get attributes
        public const uint SHGFI_ICONLOCATION = 0x000001000; // get icon location
        public const uint SHGFI_EXETYPE = 0x000002000; // return exe type
        public const uint SHGFI_SYSICONINDEX = 0x000004000; // get system icon index
        public const uint SHGFI_LINKOVERLAY = 0x000008000; // put a link overlay on icon
        public const uint SHGFI_SELECTED = 0x000010000; // show icon in selected state
        public const uint SHGFI_ATTR_SPECIFIED = 0x000020000; // get only specified attributes
        public const uint SHGFI_LARGEICON = 0x000000000; // get large icon
        public const uint SHGFI_SMALLICON = 0x000000001; // get small icon
        public const uint SHGFI_OPENICON = 0x000000002; // get open icon
        public const uint SHGFI_SHELLICONSIZE = 0x000000004; // get shell size icon
        public const uint SHGFI_PIDL = 0x000000008; // pszPath is a pidl
        public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010; // use passed dwFileAttribute
        public const uint SHGFI_ADDOVERLAYS = 0x000000020; // apply the appropriate overlays
        public const uint SHGFI_OVERLAYINDEX = 0x000000040; // Get the index of the overlay

        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags
            );

        #region Nested type: BROWSEINFO

        [StructLayout(LayoutKind.Sequential)]
        public struct BROWSEINFO
        {
            public IntPtr hwndOwner;
            public IntPtr pidlRoot;
            public IntPtr pszDisplayName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszTitle;
            public uint ulFlags;
            public IntPtr lpfn;
            public int lParam;
            public IntPtr iImage;
        }

        #endregion

        #region Nested type: ITEMIDLIST

        [StructLayout(LayoutKind.Sequential)]
        public struct ITEMIDLIST
        {
            public SHITEMID mkid;
        }

        #endregion

        #region Nested type: SHFILEINFO

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public const int NAMESIZE = 80;
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAMESIZE)]
            public string szTypeName;
        };

        #endregion

        #region Nested type: SHITEMID

        [StructLayout(LayoutKind.Sequential)]
        public struct SHITEMID
        {
            public ushort cb;
            [MarshalAs(UnmanagedType.LPArray)]
            public byte[] abID;
        }

        #endregion
    }

    #endregion

    #region User32

    // Wraps necessary functions imported from User32.dll. Code courtesy of MSDN Cold Rooster Consulting example.
    public class User32
    {
        // Provides access to function required to delete handle. This method is used internally
        // and is not required to be called separately.
        [DllImport("User32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);
    }

    #endregion
}