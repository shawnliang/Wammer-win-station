#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Waveface.Configuration;

#endregion

namespace Waveface.DetailUI
{
    public partial class SlideShowForm : Form
    {
        private FormSettings m_formSettings;

        private int m_index;
        private int m_displayCount; //Stores how many pictures are shown in SlideShow
        private int m_fileCount;
        private List<string> m_imageFilesPath = new List<string>();

        private bool m_isHidden;
        private DateTime m_lastMouseMove;
        private bool m_loop = true;

        private TimeSpan m_maxTimeToHide = TimeSpan.FromSeconds(2);
        private bool m_playing;
        private Random m_rand = new Random(0);
        private bool m_shuffle;
        private TimeSpan m_timeSpan;

        private bool m_fast;
        private bool m_medium = true;
        private bool m_slow;

        #region Properties

        public bool Loop
        {
            get
            {
                return m_loop;
            }
            set
            {
                m_loop = value;
                loopToolStripMenuItem.Checked = value;
            }
        }

        public bool Shuffle
        {
            get
            {
                return m_shuffle;
            }
            set
            {
                m_shuffle = value;
                shuffleToolStripMenuItem.Checked = value;
            }
        }

        public bool Fast
        {
            get { return m_fast; }
            set
            {
                m_fast = value;
                fastToolStripMenuItem.Checked = value;
            }
        }

        public bool Medium
        {
            get { return m_medium; }
            set
            {
                m_medium = value;
                mediumToolStripMenuItem.Checked = value;
            }
        }

        public bool Slow
        {
            get { return m_slow; }
            set
            {
                m_slow = value;
                slowToolStripMenuItem.Checked = value;
            }
        }

        #endregion

        public SlideShowForm(List<string> imageFilesPath, int startIndex)
        {
            InitializeComponent();

            m_formSettings = new FormSettings(this);
            m_formSettings.UseSize = false;
            m_formSettings.Settings.Add(new PropertySetting("Loop", this, "Loop", true));
            m_formSettings.Settings.Add(new PropertySetting("Shuffle", this, "Shuffle", false));
            m_formSettings.Settings.Add(new PropertySetting("Fast", this, "Fast", false));
            m_formSettings.Settings.Add(new PropertySetting("Medium", this, "Medium", true));
            m_formSettings.Settings.Add(new PropertySetting("Slow", this, "Slow", false));

            m_imageFilesPath = imageFilesPath;
            m_fileCount = m_imageFilesPath.Count;

            m_index = startIndex - 1;
        }

        private void FormSlideShow_Load(object sender, EventArgs e)
        {
            //Form enters fullscreen
            FormBorderStyle = FormBorderStyle.None;
            Bounds = Screen.PrimaryScreen.Bounds;
            TopMost = true;

            //Height of picturebox becomes equal to that of form
            pictureBox.Height = Height;
            pictureBox.Width = Width;

            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            //Context menu
            pictureBox.ContextMenuStrip = contextMenuStripSlideShow;

            ShowImage();

            //Start timer
            timer.Start();
            timerCursorMove.Start();

            //Initialise playing
            m_playing = true;
        }

        private void FormSlideShow_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    exitToolStripMenuItem_Click(sender, e);
                    break;

                case Keys.Left:
                    backToolStripMenuItem_Click(sender, e);
                    break;

                case Keys.Right:
                    nextToolStripMenuItem_Click(sender, e);
                    break;

                case Keys.Space:
                    if (m_playing)
                        pauseToolStripMenuItem_Click(sender, e);
                    else
                        playToolStripMenuItem_Click(sender, e);

                    break;

                case Keys.Apps:
                    contextMenuStripSlideShow.Show();
                    break;
            }

            e.Handled = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ShowImage();
        }

        private void ShowImage()
        {
            //Code for changing picture
            if (Shuffle)
                m_index = (m_index + m_rand.Next(m_fileCount)) % m_fileCount;
            else
                m_index = (m_index + 1) % m_fileCount;

            if (Loop)
            {
                pictureBox.ImageLocation = m_imageFilesPath[m_index];
            }
            else
            {
                if (m_displayCount <= m_fileCount - 1)
                {
                    pictureBox.ImageLocation = m_imageFilesPath[m_index];
                    m_displayCount += 1;
                }
                else
                {
                    playToolStripMenuItem.Enabled = true;
                    pauseToolStripMenuItem.Enabled = false;
                    timer.Stop();
                    m_displayCount = 0;
                }
            }
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop();

            playToolStripMenuItem.Enabled = true;
            pauseToolStripMenuItem.Enabled = false;

            m_playing = false;
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Start();

            playToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = true;

            m_playing = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Show();

            Close();
        }

        private void fastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Interval = 2000;

            setSpeedValue(true, false, false);
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Interval = 4000;

            setSpeedValue(false, true, false);
        }

        private void slowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Interval = 6000;

            setSpeedValue(false, false, true);
        }

        private void setSpeedValue(bool fast, bool medium, bool slow)
        {
            m_fast = fastToolStripMenuItem.Checked = fast;
            m_medium = mediumToolStripMenuItem.Checked = medium;
            m_slow = slowToolStripMenuItem.Checked = slow;
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_index = (m_index + 1) % m_fileCount;
            pictureBox.ImageLocation = m_imageFilesPath[m_index];

            timer.Stop();

            if (m_playing)
                timer.Start();
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_index > 0)
                m_index = (m_index - 1) % m_fileCount;
            else
                m_index = m_fileCount - 1;

            pictureBox.ImageLocation = m_imageFilesPath[m_index];

            timer.Stop();

            if (m_playing)
                timer.Start();
        }

        private void loopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Loop = !Loop;
            loopToolStripMenuItem.Checked = Loop;
        }

        private void shuffleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shuffle = !Shuffle;
            shuffleToolStripMenuItem.Checked = Shuffle;
        }

        private void timerCursorMove_Tick(object sender, EventArgs e)
        {
            //Code for Show/Hide cursor
            m_timeSpan = DateTime.Now - m_lastMouseMove;

            if (m_timeSpan > m_maxTimeToHide && !m_isHidden)
            {
                Cursor.Hide();

                m_isHidden = true;
            }
        }

        private void picture_MouseMove(object sender, MouseEventArgs e)
        {
            m_lastMouseMove = DateTime.Now;

            if (m_isHidden)
            {
                Cursor.Show();

                m_isHidden = false;
            }
        }

        private void picture_MouseUp(object sender, MouseEventArgs e)
        {
            m_lastMouseMove = DateTime.Now;

            if (m_isHidden)
            {
                Cursor.Show();

                m_isHidden = false;
            }
        }

        private void SlideShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_formSettings.Save();
        }
    }
}