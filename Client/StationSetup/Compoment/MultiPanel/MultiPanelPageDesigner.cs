#region

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

#endregion

namespace Waveface.Component.MultiPage.Design
{
    public class MultiPanelPageDesigner : ScrollableControlDesigner
    {
        private Font m_font = new Font("Courier New", 8F, FontStyle.Bold);
        private MultiPanelPage m_page;
        private StringFormat m_rightfmt = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.DirectionRightToLeft);

        public string Text
        {
            get
            {
                return m_page.Text;
            }
            set
            {
                string _ot = m_page.Text;
                m_page.Text = value;
                IComponentChangeService _iccs = GetService(typeof (IComponentChangeService)) as IComponentChangeService;
                
                if (_iccs != null)
                {
                    MultiPanel _ytc = m_page.Parent as MultiPanel;
                
                    if (_ytc != null)
                        _ytc.Refresh();
                }
            }
        }

        protected override void OnPaintAdornments(PaintEventArgs pea)
        {
            base.OnPaintAdornments(pea);

            using (Pen _p = new Pen(SystemColors.ControlDark, 1))
            {
                _p.DashStyle = DashStyle.Dash;
                pea.Graphics.DrawRectangle(_p, 0, 0, m_page.Width - 1, m_page.Height - 1);
            }

            using (Brush _b = new SolidBrush(Color.FromArgb(100, Color.Black)))
            {
                float _fh = m_font.GetHeight(pea.Graphics);
                RectangleF _tleft = new RectangleF(0, 0, m_page.Width/2, _fh);
                RectangleF _bleft = new RectangleF(0, m_page.Height - _fh, m_page.Width/2, _fh);
                RectangleF _tright = new RectangleF(m_page.Width/2, 0, m_page.Width/2, _fh);
                RectangleF _bright = new RectangleF(m_page.Width/2, m_page.Height - _fh, m_page.Width/2, _fh);
                pea.Graphics.DrawString(m_page.Text, m_font, _b, _tleft);
                pea.Graphics.DrawString(m_page.Text, m_font, _b, _bleft);
                pea.Graphics.DrawString(m_page.Text, m_font, _b, _tright, m_rightfmt);
                pea.Graphics.DrawString(m_page.Text, m_font, _b, _bright, m_rightfmt);
            }
        }

        public override void Initialize(IComponent component)
        {
            m_page = component as MultiPanelPage;

            if (m_page == null)
                DisplayError(new Exception("You attempted to use a MultiPanelPageDesigner with a class that does not inherit from MultiPanelPage."));
            
            base.Initialize(component);
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            
            properties["Text"] = TypeDescriptor.CreateProperty(typeof (MultiPanelPageDesigner), (PropertyDescriptor) properties["Text"], new Attribute[0]);
        }
    }
}