﻿#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Component.RichEdit;
using Waveface.Configuration;
using System.IO;
using Waveface.Properties;

#endregion

namespace Waveface
{
    public partial class PostForm : Form
    {
        // private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private int WEB_LINK_HEIGHT = 275;
        private PostType m_postType;
        private bool m_generateWebPreview = true;
        private List<string> m_parsedErrorURLs = new List<string>();
        private List<string> m_parsedURLs = new List<string>();
        private WorkItem m_workItem;
        private FormattingRule m_linkFormattingRule;
        private FormSettings m_formSettings;
        private bool m_getPreviewNow;
        private string m_lastPreviewURL = string.Empty;
        private Dictionary<string, string> m_oldImageFiles;
        private Dictionary<string, string> m_fileNameMapping;

        private List<string> m_existPostAddPhotos;
        private int m_existPostAddPhotosIndex;
        private string m_autoText;

        public Post Post { get; set; }
        public bool EditMode { get; set; }
        public string OldText { get; set; }
        public bool IsBackFromEditMode_Weblink { get; set; }
        public bool IsDirty { get; set; }

        private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;

        public PostForm(string autoText, List<string> files, PostType postType, Post post, bool editMode, List<string> existPostAddPhotos, int existPostAddPhotosIndex)
        {
            EditMode = editMode;
            Post = post;
            m_autoText = autoText;

            InitializeComponent();

            weblink_UI.MyParent = this;
            photo_UI.MyParent = this;
			photo_UI.PostId = (editMode) ? post.post_id : Guid.NewGuid().ToString();
            richText_UI.MyParent = this;
            document_UI.MyParent = this;

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper(false);

            m_oldImageFiles = new Dictionary<string, string>();
            m_fileNameMapping = new Dictionary<string, string>();
            m_existPostAddPhotos = existPostAddPhotos;
            m_existPostAddPhotosIndex = existPostAddPhotosIndex;

            m_formSettings = new FormSettings(this);
            m_formSettings.UseLocation = true;
            m_formSettings.UseSize = true;
            m_formSettings.UseWindowState = true;
            m_formSettings.AllowMinimized = false;
            m_formSettings.SaveOnClose = true;

            CreateLinkFormattingRule();

            btnAddPhoto.Focus();

            pureTextBox.WaterMarkText = Resources.PURE_TEXT_WATER_MARK;

            if (EditMode)
            {
                InitEditMode();
            }
            else
            {
                InitNewMode(files, postType);
            }

            IsDirty = false;

            EnablePhotoDragable(pureTextBox);
            EnablePhotoDragable(this);
        }

        private void EnablePhotoDragable(Control ctrl)
        {
            ctrl.AllowDrop = true;

            ctrl.DragDrop += panelMiddleBar_DragDrop;
            ctrl.DragEnter += panelMiddleBar_DragEnter;
            ctrl.DragOver += panelMiddleBar_DragOver;
            ctrl.DragLeave += panelMiddleBar_DragLeave;
        }

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void PostForm_Load(object sender, EventArgs e)
        {
            SetForegroundWindow(Handle);

            HackDPI();
        }

        private void HackDPI()
        {
            float _r = getDPIRatio();

            if (_r != 0)
            {
                Font _old = btnAddPhoto.Font;
                Font _new = new Font(_old.Name, _old.Size * _r, _old.Style);

                btnAddPhoto.Font = _new;
            }
        }

        private float getDPIRatio()
        {
            using (Graphics _g = CreateGraphics())
            {
                if (_g.DpiX == 120)
                    return 0.85f;
            }

            return 1;
        }

        private void InitEditMode()
        {
            List<string> _pics = new List<string>();

			Text = Resources.EDIT;

			btnSend.Text = Resources.UPDATE;

            if (Post.type == "link")
                weblink_UI.ChangeToEditModeUI(Post);

            if (Post.type == "image")
                photo_UI.ChangeToEditModeUI(Post);

            OldText = Post.content;

            pureTextBox.Text = OldText;
            pureTextBox.SelectionStart = OldText.Length;
            pureTextBox.SelectionFont = new Font("Tahoma", 11, FontStyle.Regular);

            CreateLink();

            switch (Post.type)
            {
                case "link":
                    break;

                case "image":
                    {
                        AttachmentUtility.GetAllMediumsPhotoPathsByPost(Post, m_oldImageFiles, m_fileNameMapping);

                        photo_UI.FileNameMapping = m_fileNameMapping;

                        foreach (KeyValuePair<string, string> _imgPair in m_oldImageFiles)
                        {
                            var file = _imgPair.Value;
                            if (!File.Exists(file))
                            {
                                var originalFile = m_fileNameMapping[file];
                                var extension = Path.GetExtension(originalFile);
                                var objectID = file.Substring(0, file.LastIndexOf("_"));
                                file = Path.Combine(Path.GetDirectoryName(file), objectID + extension);
                            }
                            _pics.Add(file);
                        }
                    }

                    break;
            }

            ToSubControl(_pics, getPostType(Post.type));

            if (m_existPostAddPhotos != null)
                AddTimelinePhotos();
        }

        private void AddTimelinePhotos()
        {
            switch (Post.type)
            {
                case "link":
                    break;
                case "text":
                    {
                        uiToPhotoMode();

                        photo_UI.AddPhotos(m_existPostAddPhotos.ToArray(), m_existPostAddPhotosIndex);
                    }

                    break;
                case "image":
                    {
                        photo_UI.AddPhotos(m_existPostAddPhotos.ToArray(), m_existPostAddPhotosIndex);
                    }

                    break;
            }
        }

        private void InitNewMode(List<string> files, PostType postType)
        {
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

        private PostType getPostType(string postType)
        {
            switch (postType)
            {
                case "text":
                    return PostType.Text;

                case "doc":
                    return PostType.Document;

                case "link":
                    return PostType.Link;

                case "image":
                    return PostType.Photo;

                case "rtf":

                    return PostType.RichText;
            }

            return PostType.Text;
        }

        private void ToSubControl(List<string> files, PostType postType)
        {
            switch (postType)
            {
                case PostType.All:
                case PostType.Text:
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

                    if (!EditMode)
                    {
                        if (!string.IsNullOrEmpty(m_autoText))
                        {
                            pureTextBox.Text = m_autoText;
                            pureTextBox.SelectionStart = m_autoText.Length;
                            pureTextBox.SelectionFont = new Font("Tahoma", 11, FontStyle.Regular);
                        }
                    }

                    break;

                case PostType.RichText:
                    toRichText_Mode();
                    break;
            }

            m_postType = postType;
        }

        public void SetDialogResult_Yes_AndClose()
        {
            IsDirty = false;

            DialogResult = DialogResult.Yes;
            Close();
        }

        public void SetDialogResult_OK_AndClose()
        {
            IsDirty = false;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void PostForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //document_UI.UnloadPreviewHandler();

            if (IsDirty)
            {
                DialogResult _dr = MessageBox.Show(Resources.DISCARD_EDIT_POST, EditMode ? Resources.CANCEL_EDIT_TITLE : Resources.CANCEL_POST_TITLE, MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Warning);

                if (_dr != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void PostForm_SizeChanged(object sender, EventArgs e)
        {
            if (panelSubArea.Visible)
            {
                if (m_postType == PostType.Link)
                {
                    panelPureTextArea.Height = ClientSize.Height - WEB_LINK_HEIGHT;
                }
            }
            else
            {
                panelPureTextArea.Height = ClientSize.Height;
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
            m_postType = PostType.RichText;

            multiPanel.SelectedPage = Page_RichText;
        }

        public void toPureText_Mode()
        {
            m_postType = PostType.Text;

            multiPanel.SelectedPage = Page_P_D_W;

            RelayoutUI(true);
        }

        private void toWebLink_Mode()
        {
            m_postType = PostType.Link;

            multiPanel.SelectedPage = Page_P_D_W;
            multiPanel_P_D_W.SelectedPage = Page__Link;

            RelayoutUI(false);
        }

        private void toPhoto_Mode(List<string> files)
        {
            uiToPhotoMode();

            if (!EditMode || IsBackFromEditMode_Weblink)
            {
                if (files == null)
                {
                    photo_UI.AddPhoto();
                }
                else
                {
                    photo_UI.AddNewPostPhotoFiles(files);
                }
            }
            else
            {
                if (files == null)
                {
                    photo_UI.AddPhoto();
                }
                else
                {
                    photo_UI.AddEditModePhotoFiles(files, Post);
                }
            }
        }

        private void uiToPhotoMode()
        {
            m_postType = PostType.Photo;

            multiPanel.SelectedPage = Page_P_D_W;
            multiPanel_P_D_W.SelectedPage = Page__Photo;

            RelayoutUI(false);
        }

        private void toDoc_Mode()
        {
            m_postType = PostType.Photo;

            multiPanel.SelectedPage = Page_P_D_W;
            multiPanel_P_D_W.SelectedPage = Page__DOC;

            RelayoutUI(false);
        }

        private void RelayoutUI(bool toTextType)
        {
            int _d = 22;

            panelMiddleBar.Visible = toTextType;
            panelSubArea.Visible = !toTextType;

            splitter.Visible = !toTextType;

            if (m_postType == PostType.Link)
                splitter.Visible = false;

            if (toTextType)
            {
                panelPureTextArea.Height = ClientSize.Height;
            }
            else
            {
                if (m_postType == PostType.Link)
                {
                    panelPureTextArea.Height = ClientSize.Height - WEB_LINK_HEIGHT;
                }
                else
                {
                    panelPureTextArea.Height = ClientSize.Height / 2;
                }
            }

            pureTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            if (toTextType)
            {
                pureTextBox.Height = panelPureTextArea.Height - panelMiddleBar.Height - _d;
            }
            else
            {
                pureTextBox.Height = panelPureTextArea.Height - _d;
            }

            pureTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
        }

        #endregion

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            if (pureTextBox.Text.Trim().Equals(string.Empty))
            {
				MessageBox.Show(Resources.EMPTY_CONTENT, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (EditMode)
            {
                if (!pureTextBox.Text.Trim().Equals(OldText) || (Post.type != "text"))
                {
                    Dictionary<string, string> _params = new Dictionary<string, string>();
                    _params.Add("content", StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(pureTextBox.Text, 80000)));
                    _params.Add("type", "text");

                    Main.Current.PostUpdate(Post, _params);
                }

                SetDialogResult_Yes_AndClose();
            }
            else
            {
                try
                {
                    MR_posts_new _np =
                        Main.Current.RT.REST.Posts_New("", StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(pureTextBox.Text, 80000)),
                                                       "", "", "text", "");

                    if (_np == null)
                    {
						MessageBox.Show(Resources.POST_ERROR, "Stream", MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }

					Main.Current.ShowStatuMessage(Resources.POST_SUCCESS, true);
                    Main.Current.ReloadAllData();

                    SetDialogResult_Yes_AndClose();
                }
                catch (Exception _e)
                {
                    MessageBox.Show(_e.Message, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void pureTextBox_TextChanged2(object sender, TextChanged2EventArgs args)
        {
            CreateLink();

            IsDirty = true;
        }

        private void CreateLink()
        {
            string _contents = pureTextBox.Text;
            FormattingInstructionCollection _instructions = new FormattingInstructionCollection();

            Format _format = new Format();
            _instructions.Add(new FormattingInstruction(0, _contents.Length, _format));

            foreach (Match _match in m_linkFormattingRule.Regex.Matches(_contents))
            {
                _instructions.Add(new FormattingInstruction(_match.Index, _match.Length, m_linkFormattingRule.Format));
            }

            pureTextBox.BatchFormat(_instructions);
        }

        public void BackFromEditMode_Weblink()
        {
            IsBackFromEditMode_Weblink = true;

            btnAddPhoto.Visible = true;
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
                if (k > 0) //Only First Item
                    break;

                k++;

                if (_url == m_lastPreviewURL)
                    break;

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

                    m_lastPreviewURL = _url;

                    break;
                }
                else
                {
                    if (!m_parsedErrorURLs.Contains(_url))
                        m_parsedErrorURLs.Add(_url);

					showPreviewMessage(Resources.NO_WEB_PREVIEW + " " + _url, false, 5000);
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
					showPreviewMessage(Resources.GET_WEB_PREVIEW + " " + url, true, 0);
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

        #region panelMiddleBar DragDrop

        private void panelMiddleBar_DragDrop(object sender, DragEventArgs e)
        {
            List<string> _pics = m_dragDropClipboardHelper.Drag_Drop(e);

            if (_pics != null)
            {
                uiToPhotoMode();

                photo_UI.AddPhotos(_pics.ToArray(), 0);
            }

            FlashWindow.Stop(this);
        }

        private void panelMiddleBar_DragEnter(object sender, DragEventArgs e)
        {
            FlashWindow.Start(this);

            m_dragDropClipboardHelper.Drag_Enter(e, false);
        }

        private void panelMiddleBar_DragLeave(object sender, EventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Leave();

            FlashWindow.Stop(this);
        }

        private void panelMiddleBar_DragOver(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Over(e, false);
        }

        #endregion
    }
}