﻿﻿#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface
{
    public partial class PostArea : UserControl
    {      
        //private bool m_init;

        public PostsList PostsList
        {
            get { return postList; }
        }

        public PostArea()
        {
            InitializeComponent();

            //comboBoxType.SelectedIndex = 0;

            //m_init = true;

            show_labelStatus(false);
        }

        public void ShowTypeUI(bool flag)
        {
            labelDisplay.Visible = flag;
            comboBoxType.Visible = flag;
        }

        public string GetPostTypeText()
        {
            return comboBoxType.Text;
        }

        private void comboBoxType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //if (m_init)
            //    Main.Current.DoTimelineFilter(null, true);
        }

        public void ShowPostInforPanel(bool flag)
        {
            panelButtom.Visible = flag;
        }

        public void ShowPostInfor(int all, int getPostCounts)
        {
            ShowPostInforPanel(true);

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

        public void ShowStatusText(string msg)
        {
            set_labelStatus_SizeLocation();

            labelStatus.Text = msg;

            show_labelStatus(msg != "");
        }

        private void show_labelStatus(bool flag)
        {
            labelStatus.Font = new Font("Arial", 11, FontStyle.Bold);

            labelStatus.Visible = flag;

            btnCreatePost.Visible = !flag;
            btnRefresh.Visible = !flag;

            if (flag)
            {
                labelStatus.BringToFront();
            }
            else
            {
                labelStatus.SendToBack();
            }
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

        private void PostArea_Resize(object sender, System.EventArgs e)
        {
            set_labelStatus_SizeLocation();
        }

        private void set_labelStatus_SizeLocation()
        {
            labelStatus.Size = new Size(panelTop.Size.Width - 4, panelTop.Size.Height - 3);
            labelStatus.Location = new Point(2, 2);
        }
    }
}