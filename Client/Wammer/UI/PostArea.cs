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

            if (flag)
            {
                labelDisplay.Visible = true;
                comboBoxType.Visible = true;
            }
            else
            {
                labelDisplay.Visible = false;
                comboBoxType.Visible = false;
            }
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
                MainForm.THIS.DoTimelineFilter(null, true);
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
            MainForm.THIS.ReadMorePost();
        }

        public void ShowNewPostInfo(int count)
        {
            btnNewPost.Visible = count > 0;

            btnNewPost.Text = count + " new " + ((count > 1) ? "posts" : "post");
        }

        private void btnNewPost_Click(object sender, System.EventArgs e)
        {

        }

        /*
        private void cmbTimeline_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            FilterItem _filterItem = (FilterItem) comboBoxTimeline.SelectedItem;

            MainForm.THIS.DoTimelineFilter(_filterItem, true);
        }
        */

        public void ShowTimelineComboBox(bool visible)
        {
            //comboBoxTimeline.Visible = visible;
        }
    }
}