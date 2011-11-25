using System;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Diagnostics;

namespace Waveface
{
    public partial class TestForm : Form
    {
        private BEService2 m_service2 = new BEService2();
        private MR_auth_login m_mrAuthLogin;
        private MR_posts_getLatest m_mrPostsGetLatest;
        private MR_posts_getSingle m_mrPostsGetSingle;
        private MR_posts_new m_mrPostsNew;
        private MR_posts_get m_mrPostsGet;
        private MR_posts_newComment m_mrPostsNewComment;
        private MR_posts_getComments m_mrPostsGetComments;

        private MR_previews_get m_mrPreviewsGet;
        private MR_previews_get_adv m_mrPreviewsGetAdv;

        private MR_attachments_upload m_mrAttachmentsUpload;
        private MR_attachments_get m_attachmentsGet;
        private MR_attachments_delete m_attachmentsDelete;

        public TestForm()
        {
            InitializeComponent();
        }

        [STAThread]
        public static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new TestForm());
            }
            catch (Exception _e)
            {
                CrashReporter _errorDlg = new CrashReporter(_e);
                _errorDlg.ShowDialog();
            }
        }


        #region API

        private void DoLoginUI()
        {
            LoginForm _loginForm = new LoginForm();
            DialogResult _dr = _loginForm.ShowDialog();

            if (_dr != DialogResult.OK)
                return;

            DoLogin(_loginForm.User, _loginForm.Password);
        }

        private void DoLogin(string email, string password)
        {
            m_mrAuthLogin = m_service2.auth_login(email, password);

            if (m_mrAuthLogin == null)
            {
                MessageBox.Show("Login Error!");
                return;
            }

            if (m_mrAuthLogin.status == "200")
            {
                PropertyGrid.SelectedObject = m_mrAuthLogin;
            }
        }

        private void posts_getLatest(string session_token, string group_id, string limit)
        {
            m_mrPostsGetLatest = m_service2.posts_getLatest(session_token, group_id, limit);

            if (m_mrPostsGetLatest == null)
            {
                MessageBox.Show("PostsGetLatest Error!");
            }
            else
            {
                MessageBox.Show("PostsGetLatest Success!");

                PropertyGrid.SelectedObject = m_mrPostsGetLatest;
            }
        }

        private void posts_get(string session_token, string group_id, string limit, string datum, string filter)
        {
            m_mrPostsGet = m_service2.posts_get(session_token, group_id, limit, datum, filter);

            if (m_mrPostsGet == null)
            {
                MessageBox.Show("PostsGet Error!");
            }
            else
            {
                MessageBox.Show("PostsGet Success!");

                PropertyGrid.SelectedObject = m_mrPostsGet;
            }
        }

        private void posts_getSingle(string session_token, string group_id, string post_id)
        {
            m_mrPostsGetSingle = m_service2.posts_getSingle(session_token, group_id, post_id);

            if (m_mrPostsGetSingle == null)
            {
                MessageBox.Show("PostsGetSingle Error!");
            }
            else
            {
                MessageBox.Show("PostsGetSingle Success!");

                PropertyGrid.SelectedObject = m_mrPostsGetSingle;
            }
        }

        private void post_postsNew(string session_token, string group_id, string content, string objects, string previews, string type)
        {
            m_mrPostsNew = m_service2.posts_new(session_token, group_id, content, objects, previews, type);

            if (m_mrPostsNew == null)
            {
                MessageBox.Show("PostsNew Error!");
            }
            else
            {
                MessageBox.Show("PostsNew Success!");

                PropertyGrid.SelectedObject = m_mrPostsNew;
            }
        }

        private void post_postsNewComment(string session_token, string group_id, string post_id, string content, string objects, string previews)
        {
            m_mrPostsNewComment = m_service2.posts_newComment(session_token, group_id, post_id, content, objects, previews);

            if (m_mrPostsNewComment == null)
            {
                MessageBox.Show("PostsNewComment Error!");
            }
            else
            {
                MessageBox.Show("PostsNewComment Success!");

                PropertyGrid.SelectedObject = m_mrPostsNewComment;
            }
        }

        private void post_postsGetComments(string session_token, string group_id, string post_id)
        {
            m_mrPostsGetComments = m_service2.posts_getComments(session_token, group_id, post_id);

            if (m_mrPostsGetComments == null)
            {
                MessageBox.Show("PostsGetComments Error!");
            }
            else
            {
                MessageBox.Show("PostsGetComments Success!");

                PropertyGrid.SelectedObject = m_mrPostsGetComments;
            }
        }

        private void previews_get(string session_token, string url)
        {
            m_mrPreviewsGet = m_service2.previews_get(session_token, url);

            if (m_mrPreviewsGet == null)
            {
                MessageBox.Show("PreviewsGet Error!");
            }
            else
            {
                MessageBox.Show("PreviewsGet Success!");

                PropertyGrid.SelectedObject = m_mrPreviewsGet;
            }
        }

        private void previews_get_adv(string session_token, string url)
        {
            m_mrPreviewsGetAdv = m_service2.previews_get_adv(session_token, url);

            if (m_mrPreviewsGetAdv == null)
            {
                MessageBox.Show("PreviewsGetAdv Error!");
            }
            else
            {
                MessageBox.Show("PreviewsGetAdv Success!");

                PropertyGrid.SelectedObject = m_mrPreviewsGetAdv;
            }
        }

        private void attachments_upload(string session_token, string group_id, string fileName, string title, string description, string type, string image_meta)
        {
            m_mrAttachmentsUpload = m_service2.attachments_upload(session_token, group_id, fileName, title, description, type, image_meta, "");

            if (m_mrAttachmentsUpload == null)
            {
                MessageBox.Show("AttachmentsUpload Error!");
            }
            else
            {
                MessageBox.Show("AttachmentsUpload Success!");

                PropertyGrid.SelectedObject = m_mrAttachmentsUpload;
            }
        }

        private void attachments_get(string session_token, string object_id)
        {
            m_attachmentsGet = m_service2.attachments_get(session_token, object_id);

            if (m_attachmentsGet == null)
            {
                MessageBox.Show("AttachmentsGet Error!");
            }
            else
            {
                MessageBox.Show("AttachmentsGet Success!");

                PropertyGrid.SelectedObject = m_attachmentsGet;
            }
        }

        private void attachments_delete(string session_token, string object_id)
        {
            m_attachmentsDelete = m_service2.attachments_delete(session_token, object_id);

            if (m_attachmentsDelete == null)
            {
                MessageBox.Show("AttachmentsDelete Error!");
            }
            else
            {
                MessageBox.Show("AttachmentsDelete Success!");

                PropertyGrid.SelectedObject = m_attachmentsDelete;
            }
        }

        #endregion

        #region UI

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            //DoLoginUI();

            DoLogin("ren.cheng@waveface.com", "123456");
            //DoLogin("michael.chen@waveface.com", "michael");
            //DoLogin("steven.shen@waveface.com", "steven"); 
            //DoLogin("ben.wei@waveface.com", "ben");
        }

        private void buttonGetLatest_Click(object sender, EventArgs e)
        {
            posts_getLatest(m_mrAuthLogin.session_token, textBoxGroupID.Text, textBoxGetLatestLimit.Text);
        }

        private void buttonGetSingle_Click(object sender, EventArgs e)
        {
            posts_getSingle(m_mrAuthLogin.session_token, textBoxGroupID.Text, textBoxPostID.Text);
        }

        private void buttonPreview_Click(object sender, EventArgs e)
        {
            previews_get(m_mrAuthLogin.session_token, textBoxPreviewURL.Text);
        }

        private void buttonPreviewAdv_Click(object sender, EventArgs e)
        {
            previews_get_adv(m_mrAuthLogin.session_token, textBoxPreviewURL.Text);
        }

        private void buttonNewPost_Click(object sender, EventArgs e)
        {
            post_postsNew(m_mrAuthLogin.session_token, textBoxGroupID.Text, textBoxNewPost.Text, "", "", ""); //type 未處理
        }

        private void buttonPostGet_Click(object sender, EventArgs e)
        {
            posts_get(m_mrAuthLogin.session_token, textBoxGroupID.Text, textBoxGetLimit.Text, textBoxDatum.Text, textBoxFilter.Text);
        }

        private void buttonNewComment_Click(object sender, EventArgs e)
        {
            post_postsNewComment(m_mrAuthLogin.session_token, textBoxGroupID.Text, textBoxPostID.Text, textBoxNewPost.Text, "", "");
        }

        private void buttonGetComments_Click(object sender, EventArgs e)
        {
            post_postsGetComments(m_mrAuthLogin.session_token, textBoxGroupID.Text, textBoxPostID.Text);
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            attachments_upload(m_mrAuthLogin.session_token, textBoxGroupID.Text, textBoxImageURL.Text, "", "", "image", "origin");
        }

        private void buttonFileGet_Click(object sender, EventArgs e)
        {
            attachments_get(m_mrAuthLogin.session_token, textBoxFileGet.Text);
        }

        private void buttonFileDelete_Click(object sender, EventArgs e)
        {
            attachments_delete(m_mrAuthLogin.session_token, textBoxFileGet.Text);
        }

        #endregion
    }
}
