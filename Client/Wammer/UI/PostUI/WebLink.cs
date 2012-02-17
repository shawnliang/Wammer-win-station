
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
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

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
            if (MyParent.pureTextBox.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show(I18n.L.T("TextEmpty"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
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
            _og.description = _aog.description;
            _og.provider_name = _aog.provider_name;
            _og.provider_url = _aog.provider_url;
            _og.url = _aog.url;
            _og.title = _aog.title;

            if (!cbNoThumbnail.Checked)
            {
                if (_aog.images.Count != 0)
                {
                    _og.thumbnail_url = _aog.images[m_currentPreviewImageIndex].url;
                    _og.thumbnail_width = _aog.images[m_currentPreviewImageIndex].width;
                    _og.thumbnail_height = _aog.images[m_currentPreviewImageIndex].height;
                }
            }

            _og.type = _aog.type;

            return _og;
        }

        private bool DoRealPost(string previews)
        {
            string _type = (previews != "") ? "link" : "text";

            try
            {
                MR_posts_new _np = Main.Current.RT.REST.Posts_New(MyParent.pureTextBox.Text, "", previews, _type);

                if (_np == null)
                {
                    MessageBox.Show(I18n.L.T("PostForm.PostError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                Main.Current.ShowStatuMessage(I18n.L.T("PostForm.PostSuccess"), true);
                Main.Current.GetAllDataAsync(ShowTimelineIndexType.LocalLastRead, true);

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
            labelSummary.Text = "網路預覽來自 - " + m_mrPreviewsGetAdv.preview.url;

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
            m_mrPreviewsGetAdv = null;

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
                if (flag)
                {
                    Cursor = Cursors.WaitCursor;
                }
                else
                {
                    Cursor = Cursors.Default;
                }

                cbNoThumbnail.Refresh(); //HACK
            }
        }

        private void General_WebLink_Resize(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(243, 242, 238); //Hack
        }

        private void cbNoThumbnail_CheckedChanged(object sender, EventArgs e)
        {
            pictureBoxPreview.Visible = !cbNoThumbnail.Checked;
        }
    }
}
