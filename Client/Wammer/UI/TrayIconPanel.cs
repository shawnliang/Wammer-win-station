#region

using System.Drawing;
using System.Windows.Forms;
using Waveface.Compoment.PopupControl;

#endregion

namespace Waveface
{
    public partial class TrayIconPanel : UserControl
    {
        public TrayIconPanel()
        {
            InitializeComponent();

            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        protected override void WndProc(ref Message m)
        {
            if ((Parent as Popup).ProcessResizing(ref m))
            {
                return;
            }

            base.WndProc(ref m);
        }
    }
}