
using System;
using System.IO;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component.HtmlEditor;
using Waveface.Util;

namespace Waveface.PostUI
{
    public partial class RichText : UserControl
    {
        public PostForm MyParent { get; set; }

        public int HtmlEditorWidth
        {
            get
            {
                return htmlEditorControl.Width;
            }
        }

        public string HTML
        {
            get
            {
                ToXHTML();

                return htmlEditorControl.DocumentHtml;
            }
        }

        public RichText()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            RichTextPostForm _dlg = new RichTextPostForm();
            _dlg.MyParent = this;

            DialogResult _dr = _dlg.ShowDialog();

            switch (_dr)
            {
                case DialogResult.Yes:
                    MyParent.SetDialogResult_Yes_AndClose();
                    break;

                case DialogResult.OK:
                    MyParent.SetDialogResult_OK_AndClose();
                    break;
            }
        }

        public void ChangeToEditModeUI(Post post)
        {
            btnSend.Text = I18n.L.T("Update");
        }

        private void htmlEditorControl_HtmlException(object sender, HtmlExceptionEventArgs args)
        {
            // obtain the message and operation
            // concatenate the message with any inner message
            string _operation = args.Operation;
            Exception _ex = args.ExceptionObject;
            string _message = _ex.Message;

            if (_ex.InnerException != null)
            {
                if (_ex.InnerException.Message != null)
                {
                    _message = string.Format("{0}\n{1}", _message, _ex.InnerException.Message);
                }
            }

            // define the title for the internal message box
            string _title;

            if (string.IsNullOrEmpty(_operation))
            {
                _title = "Unknown Error";
            }
            else
            {
                _title = _operation + " Error";
            }

            // display the error message box
            MessageBox.Show(this, _message, _title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void htmlEditorControl_HtmlNavigation(object sender, HtmlNavigationEventArgs e)
        {
            e.Cancel = false;
        }

        private void ToXHTML()
        {
            try
            {
                String _strHtmlContent = htmlEditorControl.BodyHtml;
                String _strXhtmlContent = XMLUtil.HTML2XHTML(_strHtmlContent);

                htmlEditorControl.BodyHtml = _strXhtmlContent;
                htmlEditorControl.Focus();
            }
            catch
            { }
        }

        public string GetHtmlFile()
        {
            string _localPath = Main.GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html";
            string _content = htmlEditorControl.DocumentHtml;

            _content = _content.Replace("<HEAD>", "<HEAD><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\">");
            _content = _content.Replace(" contentEditable=true scroll=auto", "");

            using (StreamWriter _outfile = new StreamWriter(_localPath))
            {
                _outfile.Write(_content);
            }

            return _localPath;
        }
    }
}
