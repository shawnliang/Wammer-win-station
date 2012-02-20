#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NLog;
using Waveface.API.V2;
using Waveface.Component.RichEdit;
using Waveface.Configuration;

#endregion

namespace Waveface
{
    public partial class PostForm : Form
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private PostType m_postType;
        private bool m_generateWebPreview = true;
        private bool m_isFixHeight;
        private int m_fixHeight;
        private List<string> m_parsedErrorURLs = new List<string>();
        private List<string> m_parsedURLs = new List<string>();
        private WorkItem m_workItem;
        private FormattingRule m_linkFormattingRule;
        private FormSettings m_formSettings;
        private bool m_getPreviewNow;

        public NewPostItem NewPostItem { get; set; }

        public PostForm(List<string> files, PostType postType)
        {
            InitializeComponent();

            m_formSettings = new FormSettings(this);
            m_formSettings.UseLocation = true;
            m_formSettings.UseWindowState = true;
            m_formSettings.AllowMinimized = false;
            m_formSettings.SaveOnClose = true;

            CreateLinkFormattingRule();

            weblink_UI.MyParent = this;
            photo_UI.MyParent = this;
            richText_UI.MyParent = this;
            document_UI.MyParent = this;

            btnAddPhoto.Focus();

            pureTextBox.WaterMarkText = I18n.L.T("PostForm.PuretextWaterMark");

            ToSubControl(files, postType);
        }

        private void CreateLinkFormattingRule()
        {
            Format _format = new Format();
            _format.Link = true;
            _format.Font = pureTextBox.Font;
            m_linkFormattingRule = new FormattingRule(new Regex(HtmlUtility.URL_RegExp_Pattern, RegexOptions.None),
                                                      _format);
        }

        private void ToSubControl(List<string> files, PostType postType)
        {
            switch (postType)
            {
                case PostType.All:
                    toPureText_Mode();
                    break;

                case PostType.Document:
                    toDoc_Mode();
                    break;

                case PostType.Link:
                    toWebLink_Mode();
                    break;

                case PostType.Photo:
                    toPhoto_Mode(files);
                    break;

                case PostType.RichText:
                    toRichText_Mode();
                    break;

                case PostType.Text:
                    toPureText_Mode();
                    break;
            }

            m_postType = postType;
        }

        public void SetDialogResult_Yes_AndClose()
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        public void SetDialogResult_OK_AndClose()
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void PostForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //document_UI.UnloadPreviewHandler();
        }

        private void PostForm_SizeChanged(object sender, EventArgs e)
        {
            if (m_isFixHeight)
            {
                Height = m_fixHeight;
            }
        }

        private void PostForm_Resize(object sender, EventArgs e)
        {
            document_UI.ResizeUI();
        }

        private void buttonRichText_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            toRichText_Mode();
        }

        private void btnAddPhoto_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            toPhoto_Mode(null);
        }

        private void btnAddDoc_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            toDoc_Mode();
        }

        private void btnPureText_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            toPureText_Mode();
        }

        #region Mode

        private void toRichText_Mode()
        {
            m_isFixHeight = false;

            m_postType = PostType.RichText;

            multiPanel.SelectedPage = Page_RichText;

            MaximizeBox = true;
        }

        public void toPureText_Mode()
        {
            m_isFixHeight = true;

            m_postType = PostType.Text;

            multiPanel.SelectedPage = Page_P_D_W;

            panelMiddleBar.Visible = true;

            MaximizeBox = false;

            m_fixHeight = 272;
            Size = new Size(760, 272);

            //pureTextBox.Focus();
        }

        private void toWebLink_Mode()
        {
            m_isFixHeight = true;

            m_postType = PostType.Link;

            multiPanel.SelectedPage = Page_P_D_W;
            multiPanel_P_D_W.SelectedPage = Page__Link;

            panelMiddleBar.Visible = false;

            MaximizeBox = false;

            m_fixHeight = 466;
            Size = new Size(760, 466);
        }

        private void toPhoto_Mode(List<string> files)
        {
            m_isFixHeight = false;

            m_postType = PostType.Photo;

            multiPanel.SelectedPage = Page_P_D_W;
            multiPanel_P_D_W.SelectedPage = Page__Photo;

            panelMiddleBar.Visible = false;

            MaximizeBox = true;

            Size = new Size(760, 530);

            if (files == null)
            {
                photo_UI.AddPhoto();
            }
            else
            {
                photo_UI.Files = files;
            }
        }

        private void toDoc_Mode()
        {
            m_isFixHeight = false;

            m_postType = PostType.Photo;

            multiPanel.SelectedPage = Page_P_D_W;
            multiPanel_P_D_W.SelectedPage = Page__DOC;

            panelMiddleBar.Visible = false;

            MaximizeBox = true;

            Size = new Size(760, 530);
        }

        #endregion

        private void cbGenerateWebPreview_CheckedChanged(object sender, EventArgs e)
        {
            m_generateWebPreview = cbGenerateWebPreview.Checked;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            if (pureTextBox.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show(I18n.L.T("TextEmpty"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    MR_posts_new _np = Main.Current.RT.REST.Posts_New(StringUtility.RichTextBox_ReplaceNewline(pureTextBox.Text), "", "", "text");

                    if (_np == null)
                    {
                        MessageBox.Show(I18n.L.T("PostForm.PostError"), "Waveface", MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }

                    Main.Current.ShowStatuMessage(I18n.L.T("PostForm.PostSuccess"), true);
                    Main.Current.GetAllDataAsync(ShowTimelineIndexType.LocalLastRead, true);

                    SetDialogResult_Yes_AndClose();
                }
                catch (Exception _e)
                {
                    MessageBox.Show(_e.Message, "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void pureTextBox_TextChanged2(object sender, TextChanged2EventArgs args)
        {
            CreateLink();
        }

        private void CreateLink()
        {
            string _contents = pureTextBox.Text;
            FormattingInstructionCollection _instructions = new FormattingInstructionCollection();

            Format _format = new Format();
            //_format.ForeColor = Color.Black;
            //_format.Font = pureTextBox.Font;
            //_format.UnderlineFormat = new UnderlineFormat(UnderlineStyle.None, UnderlineColor.Black);
            _instructions.Add(new FormattingInstruction(0, _contents.Length, _format));

            foreach (Match _match in m_linkFormattingRule.Regex.Matches(_contents))
            {
                _instructions.Add(new FormattingInstruction(_match.Index, _match.Length, m_linkFormattingRule.Format));
            }

            pureTextBox.BatchFormat(_instructions);
        }

        #region richTextBox

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(pureTextBox.SelectedText);
            pureTextBox.SelectedText = "";
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(pureTextBox.SelectedText);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataFormats.Format _format = DataFormats.GetFormat(DataFormats.Text);

            if (pureTextBox.CanPaste(_format))
            {
                pureTextBox.Paste(_format);

                CheckWebPreview();
            }
        }

        private void contextMenuStripEdit_Opening(object sender, CancelEventArgs e)
        {
            pasteToolStripMenuItem.Enabled = Clipboard.ContainsData(DataFormats.Text);
        }

        #endregion

        #region Web Link

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            CheckWebLink_Direct(e.LinkText);
        }

        private void CheckWebLink_Direct(string text)
        {
            if (!CanGetPreview())
                return;

            lock (m_parsedURLs)
            {
                m_parsedURLs.Clear();

                m_parsedURLs.Add(text);
            }

            InvokeCheckWebPreview();
        }

        private void pureTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Space))
            {
                CheckWebPreview();
            }

            if (e.Control && e.KeyCode == Keys.V)
            {
                if (pureTextBox.CanPaste(DataFormats.GetFormat(DataFormats.Bitmap)))
                {
                    e.SuppressKeyPress = true;
                }
                else if (pureTextBox.CanPaste(DataFormats.GetFormat(DataFormats.Text)))
                {
                    pureTextBox.Paste(DataFormats.GetFormat(DataFormats.Text));
                    CheckWebPreview();
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void CheckWebPreview()
        {
            if (!CanGetPreview())
                return;

            getParsedURLs(pureTextBox.Text);

            InvokeCheckWebPreview();
        }

        private bool CanGetPreview()
        {
            return ((m_postType == PostType.Text) || (m_postType == PostType.All)) && !m_getPreviewNow;
        }

        private void ThreadMethod(object state)
        {
            m_getPreviewNow = true;

            int k = 0;

            foreach (string _url in m_parsedURLs)
            {
                if (k > 0)
                    break;

                k++;

                if (m_parsedErrorURLs.Contains(_url))
                    continue;

                showIndicator(true, _url);

                bool _isOK;
                MR_previews_get_adv _mrPreviewsGetAdv = null;

                try
                {
                    _mrPreviewsGetAdv = Main.Current.RT.REST.Preview_GetAdvancedPreview(_url);

                    _isOK = (_mrPreviewsGetAdv != null) &&
                            (_mrPreviewsGetAdv.preview != null) &&
                            (_mrPreviewsGetAdv.preview.images != null);
                }
                catch
                {
                    _isOK = false;
                }

                showIndicator(false, _url);

                if (_isOK)
                {
                    showWebLinkPreview(_mrPreviewsGetAdv);
                    break;
                }
                else
                {
                    if (!m_parsedErrorURLs.Contains(_url))
                        m_parsedErrorURLs.Add(_url);

                    showPreviewMessage(I18n.L.T("PostForm.NoWebPreview") + " " + _url, false, 5000);
                }
            }

            m_getPreviewNow = false;
        }

        private void showWebLinkPreview(MR_previews_get_adv mrPreviewsGetAdv)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { showWebLinkPreview(mrPreviewsGetAdv); }
                           ));
            }
            else
            {
                toWebLink_Mode();
                weblink_UI.ShowPreview(mrPreviewsGetAdv);
            }
        }

        private void showIndicator(bool flag, string url)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { showIndicator(flag, url); }
                           ));
            }
            else
            {
                if (flag)
                {
                    Cursor = Cursors.WaitCursor;
                    showPreviewMessage(I18n.L.T("PostForm.GetWebPreview") + " " + url, true, 0);
                }
                else
                {
                    Cursor = Cursors.Default;
                    showPreviewMessage("", false, 0);
                }
            }
        }

        private void showPreviewMessage(string message, bool showWaitingIcon, int timeout)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { showPreviewMessage(message, showWaitingIcon, timeout); }
                           ));
            }
            else
            {
                if (showWaitingIcon)
                {
                    pictureBoxWaiting.Visible = true;
                    labelPreviewMsg.Left = 34;

                    labelPreviewMsg.Text = message;

                    timerNoPreviewMsg.Enabled = false;
                }
                else
                {
                    pictureBoxWaiting.Visible = false;
                    labelPreviewMsg.Left = 12;

                    labelPreviewMsg.Text = message;

                    if (timeout == 0)
                    {
                        timerNoPreviewMsg.Enabled = false;
                    }
                    else
                    {
                        timerNoPreviewMsg.Interval = timeout;
                        timerNoPreviewMsg.Enabled = true;
                    }
                }
            }
        }

        private void timerNoPreviewMsg_Tick(object sender, EventArgs e)
        {
            showPreviewMessage("", false, 0);
        }

        private void InvokeCheckWebPreview()
        {
            if (!CanGetPreview())
                return;

            if (m_generateWebPreview)
            {
                if (m_workItem != null)
                {
                    try
                    {
                        AbortableThreadPool.Cancel(m_workItem, true);
                    }
                    catch (Exception _e)
                    {
                        Console.WriteLine(_e.Message);
                    }
                }

                m_workItem = AbortableThreadPool.QueueUserWorkItem(ThreadMethod, 0);
            }
        }

        private void getParsedURLs(string text)
        {
            lock (m_parsedURLs)
            {
                m_parsedURLs.Clear();

                string[] _strs = text.Split(new[]
                                                {
                                                    ' ', '\n', '\r'
                                                });

                foreach (string _str in _strs)
                {
                    if (FindUrls(_str) != string.Empty)
                    {
                        if (!m_parsedURLs.Contains(_str))
                            m_parsedURLs.Add(_str);
                    }
                }
            }
        }

        private string FindUrls(string input)
        {
            Regex _r1 = new Regex(HtmlUtility.URL_RegExp_Pattern, RegexOptions.None);

            MatchCollection _ms1 = _r1.Matches(input);

            foreach (Match _m in _ms1)
            {
                return _m.Value;
            }

            return "";
        }

        #endregion
    }
}