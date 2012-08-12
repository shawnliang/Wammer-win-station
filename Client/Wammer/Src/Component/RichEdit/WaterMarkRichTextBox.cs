#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.RichEdit
{
    public class WaterMarkRichTextBox : RichTextBox
    {
        private Font m_oldFont;
        private Boolean m_waterMarkTextEnabled;
        private Color m_waterMarkColor = Color.Gray;
        private string m_waterMarkText = "Water Mark";
        private bool m_imeComposition;

        #region Properties

        public Color WaterMarkColor
        {
            get { return m_waterMarkColor; }
            set
            {
                m_waterMarkColor = value;

                Invalidate();
            }
        }

        public string WaterMarkText
        {
            get { return m_waterMarkText; }
            set
            {
                m_waterMarkText = value;

                Invalidate();
            }
        }

        #endregion

        public WaterMarkRichTextBox()
        {
            TextChanged += WaterMark_Toggel;
            LostFocus += WaterMark_Toggel;
            FontChanged += WaterMark_FontChanged;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            WaterMark_Toggel(null, null);
            
            // Hack... Orz
            Text = " ";
            Application.DoEvents();
            Text = "";
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_IME_STARTCOMPOSITION = 0x010D;
            const int WM_IME_ENDCOMPOSITION = 0x010E;
            const int WM_IME_COMPOSITION = 0x010F;

            try
            {
                switch (m.Msg)
                {
                    case WM_IME_STARTCOMPOSITION:
                        m_imeComposition = true;

                        WaterMark_Toggel(null, null);
                        break;

                    case WM_IME_ENDCOMPOSITION:
                        m_imeComposition = false;

                        WaterMark_Toggel(null, null);
                        break;

                    case WM_IME_COMPOSITION:
                        if (m.LParam.ToInt32() > 2048)
                        {
                            // 組完中文字
                        }

                        break;
                }

                base.WndProc(ref m);
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
            }
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            Font _drawFont = new Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);

            SolidBrush _drawBrush = new SolidBrush(WaterMarkColor);

            args.Graphics.DrawString((m_waterMarkTextEnabled ? WaterMarkText : Text), _drawFont, _drawBrush,
                                     new PointF(0.0F, 0.0F));

            base.OnPaint(args);
        }

        private void WaterMark_Toggel(object sender, EventArgs args)
        {
            if ((Text.Length > 0) || m_imeComposition)
            {
                m_waterMarkTextEnabled = false;

                SetStyle(ControlStyles.UserPaint, false);

                //Return back oldFont if existed
                if (m_oldFont != null)
                    Font = new Font(m_oldFont.FontFamily, m_oldFont.Size, m_oldFont.Style, m_oldFont.Unit);
            }
            else
            {
                m_waterMarkTextEnabled = true;

                // Save current font until returning the UserPaint style to false
                m_oldFont = new Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);

                //Enable OnPaint event handler
                SetStyle(ControlStyles.UserPaint, true);

                Refresh();
            }
        }

        private void WaterMark_FontChanged(object sender, EventArgs args)
        {
            if (m_waterMarkTextEnabled)
            {
                m_oldFont = new Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit);

                Refresh();
            }
        }
    }
}