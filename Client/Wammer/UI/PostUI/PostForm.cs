#region

using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace Waveface
{
    public partial class PostForm : Form
    {
        private NewPostItem m_newPostItem;

        public NewPostItem NewPostItem
        {
            get { return m_newPostItem; }
            set { m_newPostItem = value; }
        }

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
                    break;

                case PostType.Document:
                    break;

                case PostType.Link:
                    break;

                case PostType.Photo:
                    photo_UI.Files = files;
                    tabControl.SelectedIndex = 1;
                    break;

                case PostType.RichText:
                    break;

                case PostType.Text:
                    break;
            }
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
    }
}