
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using Waveface.API.V2;

namespace Waveface.PostUI
{
    public partial class General_WebLink : UserControl
    {
        private MR_previews_get_adv m_mrPreviewsGetAdv;
        private List<OGS_Image> m_ogsImgs;
        private int m_currentPreviewImageIndex;

        public PostForm MyParent { get; set; }

        public General_WebLink()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
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
            if (richTextBox.Text.Equals(string.Empty))
            {
                MessageBox.Show("Text cannot be empty!");
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
            _og.thumbnail_url = _aog.images[m_currentPreviewImageIndex].url;
            _og.thumbnail_width = _aog.images[m_currentPreviewImageIndex].width;
            _og.thumbnail_height = _aog.images[m_currentPreviewImageIndex].height;
            _og.type = _aog.type;

            return _og;
        }

        private bool DoRealPost(string previews)
        {
            string _type = (previews != "") ? "link" : "text";

            try
            {
                MR_posts_new _np = MainForm.THIS.Post_CreateNewPost(richTextBox.Text, "", previews, _type);

                if (_np == null)
                {
                    MessageBox.Show("Post Error!");
                    return false;
                }

                MessageBox.Show("Post success!");
                return true;
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message);
                return false;
            }
        }

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            string _url = e.LinkText;

            //_url = HttpUtility.UrlEncode(_url);

            m_mrPreviewsGetAdv = MainForm.THIS.Preview_GetAdvancedPreview(_url);

            // 如果回傳Null, 則沒Preview, 也表示當下沒用Preview
            if ((m_mrPreviewsGetAdv == null) || (m_mrPreviewsGetAdv.preview.images.Count == 0))
            {
                return;
            }

            pictureBoxPreview.Image = null;

            labelTitle.Text = m_mrPreviewsGetAdv.preview.title;
            richTextBoxDescription.Text = m_mrPreviewsGetAdv.preview.description;

            buttonPrev.Enabled = false;
            buttonNext.Enabled = false;

            m_ogsImgs = m_mrPreviewsGetAdv.preview.images;

            labelPictureIndex.Text = "";

            UpdateSelectPreviewPicturesButtons(true);

            if (m_ogsImgs.Count != 0)
            {
                LoadRemotePic();
            }
        }

        #region Preview

        private void LoadRemotePic()
        {
            pictureBoxPreview.LoadAsync(m_ogsImgs[m_currentPreviewImageIndex].url);
            labelPictureIndex.Text = "[" + (m_currentPreviewImageIndex + 1) + "/" + m_ogsImgs.Count + "]";
        }

        private void pictureBoxPreview_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            UpdateSelectPreviewPicturesButtons(false);
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

            ResetUI();
        }

        private void ResetUI()
        {
            pictureBoxPreview.Image = null;
            labelTitle.Text = "";
            labelPictureIndex.Text = "";
            richTextBoxDescription.Text = "";
            buttonRemovePreview.Enabled = false;
            buttonPrev.Enabled = false;
            buttonNext.Enabled = false;
        }

        #endregion

        #region richTextBox

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(richTextBox.SelectedText);
            richTextBox.SelectedText = "";
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(richTextBox.SelectedText);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject _data = Clipboard.GetDataObject();

            if (_data.GetDataPresent(DataFormats.Text))
            {
                richTextBox.SelectedText = _data.GetData(DataFormats.Text).ToString();
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
    }
}
