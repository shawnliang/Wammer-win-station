using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Waveface.Stream.WindowsClient
{
	public class MessageReceiver : IDisposable
    {
        #region Struct
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct WNDCLASS
        {
            public uint style;
            public WndProcDelegate lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszClassName;
        }
        #endregion


        #region Delegate
        delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam); 
        #endregion


        #region DllImport
        /// <summary>
        /// Registers the class W.
        /// </summary>
        /// <param name="lpWndClass">The lp WND class.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt16 RegisterClassW(
            [In] ref WNDCLASS lpWndClass
        );

        /// <summary>
        /// Creates the window ex W.
        /// </summary>
        /// <param name="dwExStyle">The dw ex style.</param>
        /// <param name="lpClassName">Name of the lp class.</param>
        /// <param name="lpWindowName">Name of the lp window.</param>
        /// <param name="dwStyle">The dw style.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="nWidth">Width of the n.</param>
        /// <param name="nHeight">Height of the n.</param>
        /// <param name="hWndParent">The h WND parent.</param>
        /// <param name="hMenu">The h menu.</param>
        /// <param name="hInstance">The h instance.</param>
        /// <param name="lpParam">The lp param.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CreateWindowExW(
           UInt32 dwExStyle,
           [MarshalAs(UnmanagedType.LPWStr)]
       string lpClassName,
           [MarshalAs(UnmanagedType.LPWStr)]
       string lpWindowName,
           UInt32 dwStyle,
           Int32 x,
           Int32 y,
           Int32 nWidth,
           Int32 nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam
        );

        /// <summary>
        /// Defs the window proc W.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr DefWindowProcW(
            IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam
        );

        /// <summary>
        /// Destroys the window.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyWindow(
            IntPtr hWnd
        );
        #endregion


        #region Const
        private const int ERROR_CLASS_ALREADY_EXISTS = 1410; 
        #endregion


		#region Readonly Static Var
		private static readonly WndProcDelegate m_WndProc = CustomWndProc;
		#endregion


		#region Static Var
		private static Dictionary<IntPtr, MessageReceiver> _receiverPool;
        #endregion


        #region Private Static Property
        /// <summary>
        /// Gets the m_ receiver pool.
        /// </summary>
        /// <value>
        /// The m_ receiver pool.
        /// </value>
        public static Dictionary<IntPtr, MessageReceiver> m_ReceiverPool 
        {
            get
            {
                return _receiverPool ?? (_receiverPool = new Dictionary<IntPtr, MessageReceiver>());
            }
        }
        #endregion


        #region Private Property
        /// <summary>
        /// Gets or sets a value indicating whether [m_ disposed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [m_ disposed]; otherwise, <c>false</c>.
        /// </value>
        private bool m_Disposed { get; set; }

        /// <summary>
        /// Gets or sets the M_HWND.
        /// </summary>
        /// <value>
        /// The M_HWND.
        /// </value>
        private IntPtr m_Hwnd { get; set; }
        #endregion

        #region Event
        public event EventHandler<MessageEventArgs> WndProc; 
        #endregion


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceiver" /> class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="title">The title.</param>
        /// <exception cref="System.Exception">Could not register window class</exception>
        public MessageReceiver(string className, string title)
        {
            if (string.IsNullOrEmpty(className))
                className = Guid.NewGuid().ToString();

            // Create WNDCLASS
            var wndClass = new WNDCLASS() 
            {
                lpszClassName = className,
				lpfnWndProc = m_WndProc
            };


            UInt16 class_atom = RegisterClassW(ref wndClass);

            int last_error = Marshal.GetLastWin32Error();

            if (class_atom == 0 && last_error != ERROR_CLASS_ALREADY_EXISTS)
            {
                throw new System.Exception("Could not register window class");
            }

            // Create window
            m_Hwnd = CreateWindowExW(
                0,
                className,
                title,
                0,
                0,
                0,
                0,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            if (m_Hwnd == IntPtr.Zero)
                return;

            m_ReceiverPool[m_Hwnd] = this;
        }
        #endregion


        #region Private Static Method
        /// <summary>
        /// Customs the WND proc.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <returns></returns>
        private static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (m_ReceiverPool.ContainsKey(hWnd))
                m_ReceiverPool[hWnd].OnWndProc(new MessageEventArgs(msg, wParam, lParam));

            return DefWindowProcW(hWnd, msg, wParam, lParam);
        }
        #endregion


        #region Private Method
        private void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                }

                // Dispose unmanaged resources
                if (m_Hwnd != IntPtr.Zero)
                {
                    m_ReceiverPool.Remove(m_Hwnd);
                    DestroyWindow(m_Hwnd);
                    m_Hwnd = IntPtr.Zero;
                }
                m_Disposed = true;
            }
        }
        #endregion


        #region Protected Method
        /// <summary>
        /// Raises the <see cref="E:WndProc" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MessageEventArgs" /> instance containing the event data.</param>
        protected void OnWndProc(MessageEventArgs e)
        {
            if (WndProc == null)
                return;
            WndProc(this, e);
        }
        #endregion


        #region Public Method
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
	}
}
