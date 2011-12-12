#region

using System.Collections.Generic;
using System.Windows.Forms;
using Waveface.FilterUI;

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

        public void FillTimelineComboBox(List<FilterItem> list)
        {
            /*
            comboBoxTimeline.Items.Clear();

            foreach (FilterItem _item in list)
            {
                comboBoxTimeline.Items.Add(_item);
            }

            comboBoxTimeline.SelectedIndex = 0;
            */
        }

        public void ShowTypeUI(bool flag)
        {
            panelButtom.Visible = true;

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
            Main.Current.ReadMorePost();
        }
    }
}