﻿#region

using System.Windows.Forms;

#endregion

namespace Waveface
{
    public partial class PostArea : UserControl
    {
        public PostsList PostsList
        {
            get { return postList; }
        }

        public PostArea()
        {
            InitializeComponent();
        }

        public void RemovePost()
        {
            postList.RemovePost();
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
            // Main.Current.FilterReadMorePost();
        }
    }
}