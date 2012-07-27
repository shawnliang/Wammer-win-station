﻿#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface
{
    public partial class PostArea : UserControl
    {
        private string m_dateText = "";
        private Font m_font;

        public PostsList PostsList
        {
            get
            {
                postList.MyParent = this;

                return postList;
            }
        }

        public PostArea()
        {
            InitializeComponent();

            m_dateText = "";
            m_font = new Font("Tahoma", 9);
        }

        public void RefreshTimelineUI()
        {
            postList.RefreshTimelineUI();
        }

        public void RemovePost()
        {
            postList.RemovePost();
        }

        public void SetDateText(string text)
        {
            m_dateText = text;

            Refresh();
        }

        public void SetDateTextFont(Font font)
        {
            if (m_font != font)
                m_font = font;
        }

        private void panelTimeBar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Properties.Resources.timebar, 0, 0);

            if (m_font != null)
            {
                using (Brush _brush = new SolidBrush(Color.FromArgb(57, 80, 85)))
                {
                    e.Graphics.DrawString(m_dateText, m_font, _brush, 4, 2);
                }
            }
        }
    }
}