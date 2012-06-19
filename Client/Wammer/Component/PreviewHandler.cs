
#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using Microsoft.Win32;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

#endregion

namespace Waveface.Component
{
    public class PreviewHandlerHost : Control
    {
        private object m_currentPreviewHandler;
        private Guid m_currentPreviewHandlerGUID;
        private Stream m_currentPreviewHandlerStream;
        private string m_errorMessage;

        public PreviewHandlerHost()
        {
            m_currentPreviewHandlerGUID = Guid.Empty;
            BackColor = Color.White;
            Size = new Size(320, 240);

            // display default error message (no file)
            ErrorMessage = "No file loaded.\n(Only available on Windows Vista and newer operating systems)";

            // enable transparency
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        private string ErrorMessage
        {
            set
            {
                m_errorMessage = value;
                Invalidate(); // repaint the control
            }
        }

        // Gets or sets the background colour of this PreviewHandlerHost.
        [DefaultValue("White")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        // Releases the unmanaged resources used by the PreviewHandlerHost and optionally releases the managed resources.
        protected override void Dispose(bool disposing)
        {
            UnloadPreviewHandler();

            try
            {
                if (m_currentPreviewHandler != null)
                {
                    Marshal.FinalReleaseComObject(m_currentPreviewHandler);
                    m_currentPreviewHandler = null;
                }
            }
            catch
            {}

            base.Dispose(disposing);
        }

        private Guid GetPreviewHandlerGUID(string filename)
        {
            // open the registry key corresponding to the file extension
            RegistryKey _ext = Registry.ClassesRoot.OpenSubKey(Path.GetExtension(filename));

            if (_ext != null)
            {
                // open the key that indicates the GUID of the preview handler type
                RegistryKey _test = _ext.OpenSubKey("shellex\\{8895b1c6-b41f-4c1c-a562-0d564250836f}");

                if (_test != null)
                    return new Guid(Convert.ToString(_test.GetValue(null)));

                // sometimes preview handlers are declared on key for the class
                string _className = Convert.ToString(_ext.GetValue(null));

                if (_className != null)
                {
                    _test = Registry.ClassesRoot.OpenSubKey(_className + "\\shellex\\{8895b1c6-b41f-4c1c-a562-0d564250836f}");

                    if (_test != null)
                        return new Guid(Convert.ToString(_test.GetValue(null)));
                }
            }

            return Guid.Empty;
        }

        // Paints the error message text on the PreviewHandlerHost control.
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_errorMessage != String.Empty)
            {
                // paint the error message
                TextRenderer.DrawText(
                    e.Graphics,
                    m_errorMessage,
                    Font,
                    ClientRectangle,
                    ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                    );
            }
        }

        // Resizes the hosted preview handler when this PreviewHandlerHost is resized.
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (m_currentPreviewHandler is IPreviewHandler)
            {
                try
                {
                    // update the preview handler's bounds to match the control's
                    Rectangle _r = ClientRectangle;

                    //@Ren ((IPreviewHandler) m_currentPreviewHandler).SetWindow(Handle, ref _r);
                    ((IPreviewHandler)m_currentPreviewHandler).SetRect(ref _r);
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.Message);
                }
            }
        }

        // Opens the specified file using the appropriate preview handler and displays the result in this PreviewHandlerHost.
        public bool Open(string filename)
        {
            UnloadPreviewHandler();

            if (String.IsNullOrEmpty(filename))
            {
                ErrorMessage = "No file loaded.";
                return false;
            }

            // 目前64bit PDF Reader 有問題
            if (isWin64AndPDF(filename))
            {
                ErrorMessage = "No file loaded.";
                return false;
            }

            // try to get GUID for the preview handler
            Guid _guid = GetPreviewHandlerGUID(filename);
            ErrorMessage = "";

            if (_guid != Guid.Empty)
            {
                try
                {
                    if (_guid != m_currentPreviewHandlerGUID)
                    {
                        m_currentPreviewHandlerGUID = _guid;

                        // need to instantiate a different COM type (file format has changed)
                        if (m_currentPreviewHandler != null)
                            Marshal.FinalReleaseComObject(m_currentPreviewHandler);

                        // use reflection to instantiate the preview handler type
                        Type _comType = Type.GetTypeFromCLSID(m_currentPreviewHandlerGUID);
                        m_currentPreviewHandler = Activator.CreateInstance(_comType);
                    }

                    if (m_currentPreviewHandler is IInitializeWithFile)
                    {
                        // some handlers accept a filename
                        ((IInitializeWithFile)m_currentPreviewHandler).Initialize(filename, 0);
                    }
                    else if (m_currentPreviewHandler is IInitializeWithStream)
                    {
                        if (File.Exists(filename))
                        {
                            // other handlers want an IStream (in this case, a file stream)
                            m_currentPreviewHandlerStream = File.Open(filename, FileMode.Open);
                            StreamWrapper stream = new StreamWrapper(m_currentPreviewHandlerStream);
                            ((IInitializeWithStream)m_currentPreviewHandler).Initialize(stream, 0);
                        }
                        else
                        {
                            ErrorMessage = "File not found.";
                        }
                    }

                    if (m_currentPreviewHandler is IPreviewHandler)
                    {
                        // bind the preview handler to the control's bounds and preview the content
                        Rectangle _r = ClientRectangle;
                        ((IPreviewHandler)m_currentPreviewHandler).SetWindow(Handle, ref _r);
                        ((IPreviewHandler)m_currentPreviewHandler).DoPreview();

                        return true;
                    }
                }
                catch (Exception _ex)
                {
                    ErrorMessage = "Preview could not be generated.\n"; // +_ex.Message;
                }
            }
            else
            {
                ErrorMessage = "No preview available.";
            }

            return false;
        }

        private bool isWin64AndPDF(string filename)
        {
            if(".pdf" == Path.GetExtension(filename).ToLower())
            {
                if (OsUtility.Is64BitOperatingSystem)
                    return true;
            }

            return false;
        }

        // Opens the specified stream using the preview handler COM type with the provided GUID and displays the result in this PreviewHandlerHost.
        public bool Open(Stream stream, Guid previewHandler)
        {
            UnloadPreviewHandler();

            if (stream == null)
            {
                ErrorMessage = "No file loaded.";
                return false;
            }

            ErrorMessage = "";

            if (previewHandler != Guid.Empty)
            {
                try
                {
                    if (previewHandler != m_currentPreviewHandlerGUID)
                    {
                        m_currentPreviewHandlerGUID = previewHandler;

                        // need to instantiate a different COM type (file format has changed)
                        if (m_currentPreviewHandler != null)
                            Marshal.FinalReleaseComObject(m_currentPreviewHandler);

                        // use reflection to instantiate the preview handler type
                        Type _comType = Type.GetTypeFromCLSID(m_currentPreviewHandlerGUID);
                        m_currentPreviewHandler = Activator.CreateInstance(_comType);
                    }

                    if (m_currentPreviewHandler is IInitializeWithStream)
                    {
                        // must wrap the stream to provide compatibility with IStream
                        m_currentPreviewHandlerStream = stream;
                        StreamWrapper _wrapped = new StreamWrapper(m_currentPreviewHandlerStream);
                        ((IInitializeWithStream)m_currentPreviewHandler).Initialize(_wrapped, 0);
                    }

                    if (m_currentPreviewHandler is IPreviewHandler)
                    {
                        // bind the preview handler to the control's bounds and preview the content
                        Rectangle _r = ClientRectangle;
                        ((IPreviewHandler)m_currentPreviewHandler).SetWindow(Handle, ref _r);
                        ((IPreviewHandler)m_currentPreviewHandler).DoPreview();

                        return true;
                    }
                }
                catch (Exception _ex)
                {
                    ErrorMessage = "Preview could not be generated.\n"; // +_ex.Message;
                }
            }
            else
            {
                ErrorMessage = "No preview available.";
            }

            return false;
        }

        // Unloads the preview handler hosted in this PreviewHandlerHost and closes the file stream.
        public void UnloadPreviewHandler()
        {
            try
            {
                if (m_currentPreviewHandler is IPreviewHandler)
                {
                    // explicitly unload the content
                    ((IPreviewHandler) m_currentPreviewHandler).Unload();
                }

                if (m_currentPreviewHandlerStream != null)
                {
                    m_currentPreviewHandlerStream.Close();
                    m_currentPreviewHandlerStream = null;
                }
            }
            catch
            {}
        }
    }

    #region COM Interop

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("8895b1c6-b41f-4c1c-a562-0d564250836f")]
    internal interface IPreviewHandler
    {
        void SetWindow(IntPtr hwnd, ref Rectangle rect);
        void SetRect(ref Rectangle rect);
        void DoPreview();
        void Unload();
        void SetFocus();
        void QueryFocus(out IntPtr phwnd);

        [PreserveSig]
        uint TranslateAccelerator(ref Message pmsg);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("b7d14566-0509-4cce-a71f-0a554233bd9b")]
    internal interface IInitializeWithFile
    {
        void Initialize([MarshalAs(UnmanagedType.LPWStr)] string pszFilePath, uint grfMode);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("b824b49d-22ac-4161-ac8a-9916e8fa3f7f")]
    internal interface IInitializeWithStream
    {
        void Initialize(IStream pstream, uint grfMode);
    }

    #region StreamWrapper

    // Provides a bare-bones implementation of System.Runtime.InteropServices.IStream that wraps an System.IO.Stream.
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    internal class StreamWrapper : IStream
    {
        private Stream m_inner;

        // Initialises a new instance of the StreamWrapper class, using the specified System.IO.Stream.
        public StreamWrapper(Stream inner)
        {
            m_inner = inner;
        }

        #region IStream Members

        // This operation is not supported.
        public void Clone(out IStream ppstm)
        {
            throw new NotSupportedException();
        }

        // This operation is not supported.
        public void Commit(int grfCommitFlags)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            throw new NotSupportedException();
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotSupportedException();
        }

        // Reads a sequence of bytes from the underlying System.IO.Stream.
        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            long _bytesRead = m_inner.Read(pv, 0, cb);

            if (pcbRead != IntPtr.Zero)
                Marshal.WriteInt64(pcbRead, _bytesRead);
        }

        public void Revert()
        {
            throw new NotSupportedException();
        }

        // Advances the stream to the specified position.
        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            long _pos = m_inner.Seek(dlibMove, (SeekOrigin)dwOrigin);

            if (plibNewPosition != IntPtr.Zero)
                Marshal.WriteInt64(plibNewPosition, _pos);
        }

        public void SetSize(long libNewSize)
        {
            throw new NotSupportedException();
        }

        // Returns details about the stream, including its length, type and name.
        public void Stat(out STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new STATSTG();
            pstatstg.cbSize = m_inner.Length;
            pstatstg.type = 2; // stream type
            pstatstg.pwcsName = (m_inner is FileStream) ? ((FileStream)m_inner).Name : String.Empty;
        }

        // This operation is not supported.
        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotSupportedException();
        }

        // Writes a sequence of bytes to the underlying System.IO.Stream.
        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            m_inner.Write(pv, 0, cb);

            if (pcbWritten != IntPtr.Zero)
                Marshal.WriteInt64(pcbWritten, cb);
        }

        #endregion
    }

    #endregion

    #endregion
}