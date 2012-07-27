
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using Waveface.API.V2;

namespace Waveface.PostUI
{
    public partial class WebLink : UserControl
    {
        private MR_previews_get_adv m_mrPreviewsGetAdv;
        private List<OGS_Image> m_ogsImgs;
        private int m_currentPreviewImageIndex;

        public PostForm MyParent { get; set; }

        public WebLink()
        {
            InitializeComponent();

            HackDPI();
        }

        private void HackDPI()
        {
            float _r = getDPIRatio();

            if (_r != 0)
            {
                Font _old = buttonRemovePreview.Font;
                Font _new = new Font(_old.Name, _old.Size * _r, _old.Style);

                buttonRemovePreview.Font = _new;
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

        public void ChangeToEditModeUI(Post post)
        {
            btnSend.Text = I18n.L.T("Update");

            buttonPrev.Visible = false;
            buttonNext.Visible = false;
            labelPictureIndex.Visible = false;
            cbNoThumbnail.Visible = false;

            if (post.type == "link")
            {
                if (Main.Current.CheckNetworkStatus())
                {
                    if (string.IsNullOrEmpty(post.preview.thumbnail_url))
                    {
                        panelContent.Left = 0;
                        panelContent.Width = panelPreview.Width - 8;
                        panelSelectPicture.Visible = false;
                    }
                    else
                    {
                        pictureBoxPreview.LoadAsync(post.preview.thumbnail_url);
                    }
                }

                labelTitle.Text = post.preview.title.Trim();
                labelProvider.Text = post.preview.provider_display;
                richTextBoxDescription.Text = post.preview.description.Trim();
                labelSummary.Text = I18n.L.T("WebLink.ComeFrom") + " " + post.preview.url;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            if (MyParent.EditMode)
            {
                if (m_mrPreviewsGetAdv == null)
                {
                    if (!MyParent.pureTextBox.Text.Trim().Equals(MyParent.OldText))
                    {
                        Dictionary<string, string> _params = new Dictionary<string, string>();
                        _params.Add("content", StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(MyParent.pureTextBox.Text, 80000)));

                        Main.Current.PostUpdate(MyParent.Post, _params);
                    }
                }
                else
                {
                    Preview_OpenGraph _openGraph = CreateOpenGraph();
                    string previews = JsonConvert.SerializeObject(_openGraph);
                    string type = (previews != "") ? "link" : "text";

                    Dictionary<string, string> _params = new Dictionary<string, string>();
                    _params.Add("content", StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(MyParent.pureTextBox.Text, 80000)));

                    _params.Add("type", type);

                    if (previews != null)
                        _params.Add("preview", previews);

                    Main.Current.PostUpdate(MyParent.Post, _params);
                }

                MyParent.SetDialogResult_Yes_AndClose();
            }
            else
            {
                //[1] 有Preview
                if (m_mrPreviewsGetAdv != null)
                {
                    try
                    {
                        Preview_OpenGraph _openGraph = CreateOpenGraph();
                        string _og = JsonConvert.SerializeObject(_openGraph);

                        if (DoRealPost(_og))
                        {
                            MyParent.SetDialogResult_Yes_AndClose();
                        }

                        return;
                    }
                    catch (Exception _e)
                    {
                        MessageBox.Show(_e.Message);
                        return;
                    }
                }

                //[2] 單純文字
                if (DoRealPost(""))
                {
                    MyParent.SetDialogResult_Yes_AndClose();
                }
            }
        }

        private Preview_OpenGraph CreateOpenGraph()
        {
            Preview_AdvancedOpenGraph _aog = m_mrPreviewsGetAdv.preview;

            Preview_OpenGraph _og = new Preview_OpenGraph();
            _og.type = _aog.type;
            _og.description = _aog.description;
            _og.provider_display = _aog.provider_display;
            _og.url = _aog.url;
            _og.title = _aog.title;
            _og.thumbnail_url = "";

            if (!cbNoThumbnail.Checked)
            {
                if (_aog.images.Count != 0)
                {
                    _og.thumbnail_url = _aog.images[m_currentPreviewImageIndex].url;
                }
            }

            if (string.IsNullOrEmpty(_aog.favicon_url))
                _og.favicon_url = "";
            else
                _og.favicon_url = _aog.favicon_url;

            if (_aog.images != null)
                _og.images = _aog.images;

            return _og;
        }

        private bool DoRealPost(string previews)
        {
            string _type = (previews != "") ? "link" : "text";

            try
            {
                MR_posts_new _np = Main.Current.RT.REST.Posts_New("", StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(MyParent.pureTextBox.Text, 80000)), "", previews, _type, "");

                if (_np == null)
                {
                    MessageBox.Show(I18n.L.T("PostForm.PostError"), "Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                Main.Current.ShowStatuMessage(I18n.L.T("PostForm.PostSuccess"), true);
                Main.Current.ReloadAllData();

                return true;
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message);
                return false;
            }
        }

        public void ShowPreview(MR_previews_get_adv mrPreviewsGetAdv)
        {
            m_mrPreviewsGetAdv = mrPreviewsGetAdv;

            ResetUI();

            pictureBoxPreview.Image = null;

            labelTitle.Text = m_mrPreviewsGetAdv.preview.title.Trim();
            labelProvider.Text = m_mrPreviewsGetAdv.preview.provider_display;
            richTextBoxDescription.Text = m_mrPreviewsGetAdv.preview.description.Trim();
            labelSummary.Text = I18n.L.T("WebLink.ComeFrom") + " " + m_mrPreviewsGetAdv.preview.url;

            buttonPrev.Enabled = false;
            buttonNext.Enabled = false;

            m_ogsImgs = m_mrPreviewsGetAdv.preview.images;

            labelPictureIndex.Text = "";

            UpdateSelectPreviewPicturesButtons(true);

            if (m_ogsImgs.Count == 0)
            {
                panelContent.Left = 0;
                panelContent.Width = panelPreview.Width - 8;
                panelSelectPicture.Visible = false;
            }
            else
            {
                panelContent.Left = 196;
                panelContent.Width = Width - panelSelectPicture.Width - 32;
                panelSelectPicture.Visible = true;

                LoadRemotePic();
            }
        }

        #region Preview

        private void LoadRemotePic()
        {
            showIndicator(true);

            pictureBoxPreview.LoadAsync(m_ogsImgs[m_currentPreviewImageIndex].url);
            labelPictureIndex.Text = "[" + (m_currentPreviewImageIndex + 1) + "/" + m_ogsImgs.Count + "]";
        }

        private void pictureBoxPreview_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            UpdateSelectPreviewPicturesButtons(false);

            showIndicator(false);
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            m_currentPreviewImageIndex--;

            UpdateSelectPreviewPicturesButtons(true);

            LoadRemotePic();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            m_currentPreviewImageIndex++;

            UpdateSelectPreviewPicturesButtons(true);

            LoadRemotePic();
        }

        private void UpdateSelectPreviewPicturesButtons(bool isLock)
        {
            buttonRemovePreview.Enabled = true;

            if (isLock)
            {
                buttonPrev.Enabled = false;
                buttonNext.Enabled = false;
            }
            else
            {
                buttonPrev.Enabled = m_currentPreviewImageIndex > 0;
                buttonNext.Enabled = m_currentPreviewImageIndex < (m_ogsImgs.Count - 1);
            }
        }

        private void buttonRemovePreview_Click(object sender, EventArgs e)
        {
            MyParent.IsDirty = true;

            m_mrPreviewsGetAdv = null;

            if (MyParent.EditMode)
            {
                buttonPrev.Visible = true;
                buttonNext.Visible = true;
                labelPictureIndex.Visible = true;
                cbNoThumbnail.Visible = true;

                MyParent.BackFromEditMode_Weblink();
            }

            MyParent.toPureText_Mode();
        }

        private void ResetUI()
        {
            cbNoThumbnail.Checked = false;
            pictureBoxPreview.Image = null;
            labelTitle.Text = "";
            labelPictureIndex.Text = "";
            richTextBoxDescription.Text = "";
            buttonPrev.Enabled = false;
            buttonNext.Enabled = false;

            m_currentPreviewImageIndex = 0;

            if (m_ogsImgs != null)
                m_ogsImgs.Clear();
        }

        #endregion

        private void showIndicator(bool flag)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { showIndicator(flag); }
                           ));
            }
            else
            {
                Cursor = flag ? Cursors.WaitCursor : Cursors.Default;

                cbNoThumbnail.Refresh(); //HACK
            }
        }

        private void General_WebLink_Resize(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(226,226,226); //Hack
        }

        private void cbNoThumbnail_CheckedChanged(object sender, EventArgs e)
        {
            pictureBoxPreview.Visible = !cbNoThumbnail.Checked;
        }
    }
}