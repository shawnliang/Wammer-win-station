#region

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace App.Client
{
    public class EtchedLine : UserControl
    {
        private Container components;

        public EtchedLine()
        {
            DarkColor = SystemColors.ControlDark;
            LightColor = SystemColors.ControlLightLight;

            InitializeComponent();
        }

        [Category("Appearance")]
        public Color LightColor { get; set; }

        [Category("Appearance")]
        public Color DarkColor { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics _g = e.Graphics;
            Rectangle _rect = ClientRectangle;

            Pen _lightPen = new Pen(LightColor, 1.0F);
            Pen _darkPen = new Pen(DarkColor, 1.0F);

            if (Dock == DockStyle.Top)
            {
                int _y0 = _rect.Top;
                int _y1 = _rect.Top + 1;

                _g.DrawLine(_darkPen, _rect.Left, _y0, _rect.Right, _y0);
                _g.DrawLine(_lightPen, _rect.Left, _y1, _rect.Right, _y1);
            }
            else if (Dock == DockStyle.Bottom)
            {
                int _y0 = _rect.Bottom - 2;
                int _y1 = _rect.Bottom - 1;

                _g.DrawLine(_darkPen, _rect.Left, _y0, _rect.Right, _y0);
                _g.DrawLine(_lightPen, _rect.Left, _y1, _rect.Right, _y1);
            }

            base.OnPaint(e);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}