
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Waveface.Component.RichEdit
{
    public class Interop
    {
        /// <summary>
        /// Sends a message to a control. 
        /// </summary>
        /// <param name="hWnd">Window handle of the control that will receive the message</param>
        /// <param name="wMsg">The message to pass</param>
        /// <param name="wParam">The Windows WPARAM to pass</param>
        /// <param name="lParam">The Windows LPARAM to pass</param>
        /// <returns>The result of the SendMessage call</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Sends a message to a control (typesafe overload for passing integers).
        /// </summary>
        /// <param name="hWnd">Window handle of the control that will receive the message</param>
        /// <param name="wMsg">The message to pass</param>
        /// <param name="wParam">The Windows WPARAM to pass</param>
        /// <param name="lParam">The Windows LPARAM to pass</param>
        /// <returns>The result of the SendMessage call</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, ref CHARFORMAT2 format);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, ref CHARFORMAT2 format);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hWnd, int msg, ref GETTEXTEX wParam, System.Text.StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hWnd, int msg, ref GETTEXTLENGTHEX wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref CHARRANGE range);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hWnd, int msg, int wParam, TEXTRANGE range);


        /// <summary>
        /// See CHARFORMAT2 in the PlatformSDK for details
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CHARFORMAT2
        {
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public int cbSize;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public uint dwMask;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public uint dwEffects;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public int yHeight;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public int yOffset;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public int crTextColor;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public byte bCharSet;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public byte bPitchAndFamily;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;

            // CHARFORMAT2 from here onwards.

            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public short wWeight;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public short sSpacing;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public int crBackColor;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public int LCID;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public uint dwReserved;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public short sStyle;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public short wKerning;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public byte bUnderlineType;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public byte bAnimation;
            /// <summary>
            /// See CHARFORMAT2 in the PlatformSDK for details
            /// </summary>
            public byte bRevAuthor;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;

            public static CharacterRange ToManaged(CHARRANGE input)
            {
                return new CharacterRange(input.cpMin, input.cpMax - input.cpMin);
            }

            public static CHARRANGE FromManaged(CharacterRange input)
            {
                CHARRANGE result = new CHARRANGE();
                result.cpMin = input.First;
                result.cpMax = input.First + input.Length;
                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct GETTEXTEX
        {
            public int cb;
            public int flags;
            public int codepage;
            public IntPtr lpDefaultChar;
            public IntPtr lpUsedDefChar;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct GETTEXTLENGTHEX
        {
            public int flags;
            public int codepage;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class LOGFONT
        {
            public int lfHeight = 0;
            public int lfWidth = 0;
            public int lfEscapement = 0;
            public int lfOrientation = 0;
            public int lfWeight = 0;
            public byte lfItalic = 0;
            public byte lfUnderline = 0;
            public byte lfStrikeOut = 0;
            public byte lfCharSet = 0;
            public byte lfOutPrecision = 0;
            public byte lfClipPrecision = 0;
            public byte lfQuality = 0;
            public byte lfPitchAndFamily = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName = null;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class TEXTRANGE
        {
            public CHARRANGE chrg;
            public IntPtr lpstrText;
        }
    }
}