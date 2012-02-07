#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Compoment
{
    public class AutoScrollPanel : Panel
    {
        private Point m_scrollLocation;

        public AutoScrollPanel()
        {
            Enter += PanelNoScrollOnFocus_Enter;
            Leave += PanelNoScrollOnFocus_Leave;
        }

        private void PanelNoScrollOnFocus_Enter(object sender, EventArgs e)
        {
            // Set the scroll location back when the control regains focus.
            HorizontalScroll.Value = m_scrollLocation.X;
            VerticalScroll.Value = m_scrollLocation.Y;
        }

        private void PanelNoScrollOnFocus_Leave(object sender, EventArgs e)
        {
            // Remember the scroll location when the control loses focus.
            m_scrollLocation.X = HorizontalScroll.Value;
            m_scrollLocation.Y = VerticalScroll.Value;
        }

        protected override Point ScrollToControl(Control activeControl)
        {
            // When the user clicks on the webbrowser, .NET tries to scroll to 
            //  the control. Since it's the only control in the panel it will 
            //  scroll up. This little hack prevents that.
            return DisplayRectangle.Location;
        }
    }
}