#region

using System.Windows.Forms;

#endregion

namespace Waveface
{
    public partial class PostArea : UserControl
    {
        string[] m_types = { "all", "text", "image", "link", "rtf", "doc" };
        private bool m_init;

        public PostsList PostsList
        {
            get { return postList; }
        }

        public PostArea()
        {
            InitializeComponent();

            comboBoxType.SelectedIndex = 0;

            m_init = true;
        }

        public void ShowTypeUI(bool flag)
        {
            labelDisplay.Visible = flag;
            comboBoxType.Visible = flag;
        }

        public string GetPostType()
        {
            return m_types[comboBoxType.SelectedIndex];
        }

        public string GetPostTypeText()
        {
            return comboBoxType.Text;
        }

        private void comboBoxType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (m_init)
                Main.Current.DoTimelineFilter(null, true);
        }

        public void ShowPostInfo(int all, int getPostCounts)
        {
            if (all == getPostCounts)
            {
                linkLabelReadMore.Visible = false;
            }
            else
            {
                linkLabelReadMore.Visible = true;
            }

            labelPostInfo.Text = "[" + (getPostCounts) + "/" + all + "]";
        }

        private void linkLabelReadMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Main.Current.FilterReadMorePost();
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;
            
            Main.Current.GetAllDataAsync(ShowTimelineIndexType.LocalLastRead, true);
        }

        public void updateRefreshUI(bool flag)
        {
            btnRefresh.Enabled = flag;
        }

        public void showRefreshUI(bool flag)
        {
            btnCreatePost.Visible = true;
            
            btnRefresh.Visible = flag;
        }

        private void btnCreatePost_Click(object sender, System.EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            Main.Current.Post();
        }
    }
}