#region

using System;

#endregion

namespace Waveface.Component.HtmlEditor
{

    # region Application delegate definitions

    // Define delegate for raising an editor exception
    public delegate void HtmlExceptionEventHandler(object sender, HtmlExceptionEventArgs e);

    // Define delegate for handling navigation events
    public delegate void HtmlNavigationEventHandler(object sender, HtmlNavigationEventArgs e);

    // delegate declarations required for the find and replace dialog
    internal delegate void FindReplaceResetDelegate();

    internal delegate bool FindFirstDelegate(string findText, bool matchWhole, bool matchCase);

    internal delegate bool FindNextDelegate(string findText, bool matchWhole, bool matchCase);

    internal delegate bool FindReplaceOneDelegate(string findText, string replaceText, bool matchWhole, bool matchCase);

    internal delegate int FindReplaceAllDelegate(string findText, string replaceText, bool matchWhole, bool matchCase);

    #endregion

    #region Navigation Event Arguments

    // on a user initiated navigation create an event with the following EventArgs
    // user can set the cancel property to cancel the navigation
    public class HtmlNavigationEventArgs : EventArgs
    {
        //private variables
        private string m_url = string.Empty;
        private bool m_cancel;

        // define url property get
        public string Url
        {
            get { return m_url; }
        }

        // define the cancel property
        // also allows a set operation
        public bool Cancel
        {
            get { return m_cancel; }
            set { m_cancel = value; }
        }

        public HtmlNavigationEventArgs(string url)
        {
            m_url = url;
        }
    }

    #endregion

    #region HtmlException defintion and Event Arguments

    //Exception class for HtmlEditor
    public class HtmlEditorException : ApplicationException
    {
        private string m_operationName;

        // property for the operation name
        public string Operation
        {
            get { return m_operationName; }
            set { m_operationName = value; }
        }

        public HtmlEditorException()
        {
            m_operationName = string.Empty;
        }

        // Constructor accepting a single string message
        public HtmlEditorException(string message) : base(message)
        {
            m_operationName = string.Empty;
        }

        // Constructor accepting a string message and an inner exception
        public HtmlEditorException(string message, Exception inner) : base(message, inner)
        {
            m_operationName = string.Empty;
        }

        // Constructor accepting a single string message and an operation name
        public HtmlEditorException(string message, string operation) : base(message)
        {
            m_operationName = operation;
        }

        // Constructor accepting a string message an operation and an inner exception
        public HtmlEditorException(string message, string m_operationName, Exception inner) : base(message, inner)
        {
        }
    }

    #endregion

    #region HtmlExceptionEventArgs

    // if capturing an exception internally throw an event with the following EventArgs
    public class HtmlExceptionEventArgs : EventArgs
    {
        //private variables
        private string m_operation;
        private Exception m_exception;

        // define operation name property get
        public string Operation
        {
            get { return m_operation; }
        }

        // define the exception property get
        public Exception ExceptionObject
        {
            get { return m_exception; }
        }

        // constructor for event args
        public HtmlExceptionEventArgs(string operation, Exception exception)
        {
            m_operation = operation;
            m_exception = exception;
        }
    }

    #endregion
}