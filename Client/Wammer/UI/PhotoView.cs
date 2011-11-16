#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Manina.Windows.Forms;

#endregion

namespace Waveface
{
    public partial class PhotoView : Form
    {
        public PhotoView(List<string> files, string fileName)
        {
            InitializeComponent();

            // Setup the background worker
            Application.Idle += Application_Idle;

            //imageListView1.AllowDuplicateFileNames = true;
            //imageListView1.SetRenderer(new ImageListViewRenderers.DefaultRenderer());

            foreach (string _file in files)
            {
                imageListView.Items.Add(_file);
            }

            imageListView.View = Manina.Windows.Forms.View.Gallery;

            foreach (ImageListViewItem _item in imageListView.Items)
            {
                if (fileName == _item.FileName)
                {
                    _item.Selected = true;
                    return;
                }
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            thumbnailsToolStripButton.Checked = (imageListView.View == Manina.Windows.Forms.View.Thumbnails);
            galleryToolStripButton.Checked = (imageListView.View == Manina.Windows.Forms.View.Gallery);
            paneToolStripButton.Checked = (imageListView.View == Manina.Windows.Forms.View.Pane);

            toolStripStatusLabel1.Text = string.Format("{0} Items",
                                                       imageListView.Items.Count);

            groupAscendingToolStripMenuItem.Checked = imageListView.GroupOrder == SortOrder.Ascending;
            groupDescendingToolStripMenuItem.Checked = imageListView.GroupOrder == SortOrder.Descending;
            sortAscendingToolStripMenuItem.Checked = imageListView.SortOrder == SortOrder.Ascending;
            sortDescendingToolStripMenuItem.Checked = imageListView.SortOrder == SortOrder.Descending;
        }

        private void thumbnailsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView.View = Manina.Windows.Forms.View.Thumbnails;
        }

        private void galleryToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView.View = Manina.Windows.Forms.View.Gallery;
        }

        private void paneToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView.View = Manina.Windows.Forms.View.Pane;
        }

        private void groupAscendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.GroupOrder = SortOrder.Ascending;
        }

        private void sortAscendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.SortOrder = SortOrder.Ascending;
        }

        private void groupDescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.GroupOrder = SortOrder.Descending;
        }

        private void sortDescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.SortOrder = SortOrder.Descending;
        }
    }
}