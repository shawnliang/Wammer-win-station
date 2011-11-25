#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.Component;

#endregion

namespace Waveface
{
    public partial class PhotoView : Form
    {
        public PhotoView(List<string> files, string fileName)
        {
            InitializeComponent();

            Application.Idle += Application_Idle;

            foreach (string _file in files)
            {
                imageListView.Items.Add(_file);
            }

            imageListView.View = Manina.Windows.Forms.View.Gallery;
            imageListView.SetRenderer(new MyImageListViewRenderer());

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
            toolStripStatusLabel1.Text = string.Format("{0} Items",
                                                       imageListView.Items.Count);
        }
    }
}