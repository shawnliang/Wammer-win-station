#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Waveface.ImageCapture.States;
using Waveface.ImageCapture.Utils;

#endregion

namespace Waveface.ImageCapture
{
    public enum ShotType
    {
        General,
        Region,
        Window,
        Screen
    }

    public partial class CaptureForm : Form
    {
        private readonly ICaptureState m_state;
        private Point m_lastLocation;
       
        public Image Image { get; private set; }

        public CaptureForm(ShotType shotType)
        {
            InitializeComponent();

            //this code must be here, you must set beckground before form takes focus
            ScreenCapture _sc = new ScreenCapture();
            BackgroundImageLayout = ImageLayout.None;
            BackgroundImage = _sc.CaptureRectangle(GetScreenSize());

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            switch (shotType)
            {
                case ShotType.General:
                    m_state = new GeneralState(this);
                    break;
                case ShotType.Region:
                    m_state = new RegionState(this);
                    break;
                case ShotType.Window:
                    m_state = new WindowState(this);
                    break;
                case ShotType.Screen:
                    m_state = new ScreenState(this);
                    break;
            }
        }

        private void ShotForm_Load(object sender, EventArgs e)
        {
            Rectangle _rect = GetScreenSize();

            Location = _rect.Location;
            Size = _rect.Size;
        }

        private static Rectangle GetScreenSize()
        {
            Rectangle _rect = new Rectangle();

            foreach (Screen _screen in Screen.AllScreens)
            {
                _rect.X = Math.Min(_rect.X, _screen.Bounds.X);
                _rect.Y = Math.Min(_rect.Y, _screen.Bounds.Y);
                _rect.Width += Math.Abs(_screen.Bounds.Width);
                _rect.Height += Math.Abs(_screen.Bounds.Height);
            }

            return _rect;
        }

        private void ShotForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                m_state.End();
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            m_state.Start(e.Location);
        }

        private void ShotForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                Close();
                return;
            }

            m_state.End();
            Image = m_state.Capture();

            if (Image == null)
                DialogResult = DialogResult.Cancel;

            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //base.OnMouseMove(e);
            if (m_lastLocation == e.Location)
                return;

            m_lastLocation = e.Location;
            m_state.Update(e.Location);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;

            m_state.Paint(e.Graphics);
        }

        private void ShotForm_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void ShotForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                m_state.End();
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }
}