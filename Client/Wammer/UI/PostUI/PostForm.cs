#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Waveface.API.V2;

#endregion

namespace Waveface
{
    public partial class PostForm : Form
    {
        private PostType m_postType;
        private bool m_webLinkCheckdOneTime;

        public NewPostItem NewPostItem { get; set; }

        public PostForm(List<string> files, PostType postType)
        {
            InitializeComponent();

            general_weblink_UI.MyParent = this;
            photo_UI.MyParent = this;
            richText_UI.MyParent = this;
            document_UI.MyParent = this;

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
                    photo_UI.Files = files;
                    toPhoto_Mode();
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

        private void PostForm_Resize(object sender, EventArgs e)
        {
            document_UI.ResizeUI();
        }

        private void buttonRichText_Click(object sender, EventArgs e)
        {
            toRichText_Mode();
        }

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            doWebLink(e.LinkText);
        }

        private void doWebLink(string url)
        {
            if ((m_postType != PostType.Text) && (m_postType != PostType.All))
                return;

            toWebLink_Mode();

            general_weblink_UI.LinkClicked(url);
        }

        private void btnAddPhoto_Click(object sender, EventArgs e)
        {
            toPhoto_Mode();
        }

        private void btnAddDoc_Click(object sender, EventArgs e)
        {
            toDoc_Mode();
        }

        private void btnPureText_Click(object sender, EventArgs e)
        {
            toPureText_Mode();
        }

        private void toRichText_Mode()
        {
            m_postType = PostType.RichText;

            multiPanel.SelectedPage = Page_RichText;

            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
        }

        public void toPureText_Mode()
        {
            m_postType = PostType.Text;

            multiPanel.SelectedPage = Page_P_D_W;

            panelMiddleBar.Visible = true;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            WindowState = FormWindowState.Normal;
            MaximizeBox = false;

            Size = new Size(640, 236);

            richTextBox.Focus();
        }

        private void toWebLink_Mode()
        {
            m_postType = PostType.Link;

            multiPanel.SelectedPage = Page_P_D_W;
            multiPanel_P_D_W.SelectedPage = Page__Link;

            panelMiddleBar.Visible = false;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            WindowState = FormWindowState.Normal;
            MaximizeBox = false;

            Size = new Size(640, 440);
        }

        private void toPhoto_Mode()
        {
            m_postType = PostType.Photo;

            multiPanel.SelectedPage = Page_P_D_W;
            multiPanel_P_D_W.SelectedPage = Page__Photo;

            panelMiddleBar.Visible = false;

            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;

            Size = new Size(640, 440);
        }

        private void toDoc_Mode()
        {
            m_postType = PostType.Photo;

            multiPanel.SelectedPage = Page_P_D_W;
            multiPanel_P_D_W.SelectedPage = Page__DOC;

            panelMiddleBar.Visible = false;

            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;

            Size = new Size(640, 440);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (richTextBox.Text.Equals(string.Empty))
            {
                MessageBox.Show("Text cannot be empty!");
            }
            else
            {
                try
                {
                    MR_posts_new _np = MainForm.THIS.Post_CreateNewPost(richTextBox.Text, "", "", "text");

                    if (_np == null)
                    {
                        MessageBox.Show("Post Error!");
                    }

                    MessageBox.Show("Post success!");

                    SetDialogResult_Yes_AndClose();
                }
                catch (Exception _e)
                {
                    MessageBox.Show(_e.Message);
                }
            }
        }

        #region Web Link

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            if (m_webLinkCheckdOneTime)
                return;

            checkWebLink();
        }

        private void checkWebLink()
        {
            string _text = richTextBox.Text;

            string[] _strs = _text.Split(new[]
                                             {
                                                 ' '
                                             });

            if (_strs.Length < 2)
                return;

            int k = 0;

            foreach (string _str in _strs)
            {
                if (k > _strs.Length - 2)
                    break;


                if (FindUrls(_str) != string.Empty)
                {
                    doWebLink(_str);
                }

                k++;
            }
        }

        private string FindUrls(string input)
        {
            Regex _regx = new Regex("http(s)?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*([a-zA-Z0-9\\?\\#\\=\\/]){1})?", RegexOptions.IgnoreCase);
            //Regex _regx = new Regex( @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+" + @"\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?" +
            //             @"([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$"
            //            , RegexOptions.IgnoreCase);

            MatchCollection _mactches = _regx.Matches(input);

            foreach (Match _match in _mactches)
            {
                m_webLinkCheckdOneTime = true;

                return _match.Value;
            }


            Regex _regx2 = new Regex(@"^(www.|[a-zA-Z0-9].)[a-zA-Z0-9\-\.]+\.[a-zA-Z]*$", RegexOptions.IgnoreCase);

            MatchCollection _mactches2 = _regx2.Matches(input);

            foreach (Match _match in _mactches2)
            {
                m_webLinkCheckdOneTime = true;

                return _match.Value;
            }




            return "";
        }

        #endregion
    }
}