#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Component
{
    public class AutoScrollPanel : Panel
    {
        protected override Point ScrollToControl(Control activeControl)
        {
            // When the user clicks on the webbrowser, .NET tries to scroll to 
            // the control. Since it's the only control in the panel it will 
            // scroll up. This little hack prevents that.
            return DisplayRectangle.Location;
        }
    }
}