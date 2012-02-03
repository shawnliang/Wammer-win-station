#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NLog;
using Waveface.API.V2;

#endregion

namespace Waveface
{
    public partial class PostForm : Form
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private PostType m_postType;
        private bool m_isFixHeight;
        private int m_fixHeight;
        private List<string> m_parsedErrorURLs = new List<string>();
        private Dictionary<string, bool> m_parsedURLs = new Dictionary<string, bool>();
        private WorkItem m_workItem;

        public NewPostItem NewPostItem { get; set; }

        public PostForm(List<string> files, PostType postType)
        {
            InitializeComponent();

            weblink_UI.MyParent = this;
            photo_UI.MyParent = this;
            richText_UI.MyParent = this;
            document_UI.MyParent = this;

            btnAddPhoto.Focus();

            pureTextBox.WaterMarkText = I18n.L.T("PostForm.PuretextWaterMark");

            ToSubControl(files, postType);
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

            m_fixHeight = 268;
            Size = new Size(720, 268);

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

            m_fixHeight = 460;
            Size = new Size(720, 460);
        }

        private void toPhoto_Mode(List<string> files)
        {
            m_isFixHeight = false;

            m_postType = PostType.Photo;

            multiPanel.SelectedPage = Page_P_D_W;
            multiPanel_P_D_W.SelectedPage = Page__Photo;

            panelMiddleBar.Visible = false;

            MaximizeBox = true;

            Size = new Size(720, 530);

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

            Size = new Size(720, 530);
        }

        #endregion

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
                    MR_posts_new _np = Main.Current.RT.REST.Posts_New(pureTextBox.Text, "", "", "text");

                    if (_np == null)
                    {
                        MessageBox.Show(I18n.L.T("PostForm.PostError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            IDataObject _data = Clipboard.GetDataObject();

            if (_data.GetDataPresent(DataFormats.Text))
            {
                pureTextBox.SelectedText = _data.GetData(DataFormats.Text).ToString();
            }
        }

        private void contextMenuStripEdit_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Clipboard.ContainsData(DataFormats.Text))
            {
                pasteToolStripMenuItem.Enabled = false;
            }
            else
            {
                pasteToolStripMenuItem.Enabled = true;
            }
        }

        #endregion

        #region Web Link

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            lock (m_parsedURLs)
            {
                m_parsedURLs.Clear();

                m_parsedURLs.Add(e.LinkText, true);
            }

            checkWebLink();
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            if (m_postType == PostType.Link)
                return;

            getParsedURLs(pureTextBox.Text);

            checkWebLink();
        }

        private void ThreadMethod(object state)
        {
            foreach (string _url in m_parsedURLs.Keys)
            {
                if (m_parsedErrorURLs.Contains(_url))
                    continue;

                if (!m_parsedURLs[_url]) //!force
                {
                    if ((m_postType != PostType.Text) && (m_postType != PostType.All))
                        continue;
                }

                MR_previews_get_adv _mrPreviewsGetAdv = Main.Current.RT.REST.Preview_GetAdvancedPreview(_url);
                bool _isOK = (_mrPreviewsGetAdv != null) && (_mrPreviewsGetAdv.preview.images.Count != 0);

                if (_isOK)
                {
                    showWebLinkPreview(_mrPreviewsGetAdv);
                    break;
                }
                else
                {
                    if (!m_parsedErrorURLs.Contains(_url))
                        m_parsedErrorURLs.Add(_url);
                }
            }
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

        private void checkWebLink()
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

        private void getParsedURLs(string text)
        {
            lock (m_parsedURLs)
            {
                m_parsedURLs.Clear();


                string[] _strs = text.Split(new[]
                                             {
                                                 ' '
                                             });

                //if (_strs.Length < 2)
                //    return;

                int k = 0;

                foreach (string _str in _strs)
                {
                    if (k > _strs.Length - 2)
                        break;

                    if (FindUrls(_str) != string.Empty)
                    {
                        if (!m_parsedURLs.ContainsKey(_str))
                            m_parsedURLs.Add(_str, false);
                    }

                    k++;
                }
            }
        }

        private string FindUrls(string input)
        {
            Regex _r1 = new Regex(@"((www\.|(http|https)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])", RegexOptions.IgnoreCase);

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