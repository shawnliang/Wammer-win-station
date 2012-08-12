#region

using System.Drawing;
using System.Windows.Forms;
using Waveface.Component.PopupControl;
using Waveface.Component;
using Waveface.Component.RichEdit;

#endregion

namespace Waveface
{
    public partial class OldImagesPopupPanel : UserControl
    {
        public OldImagesPopupPanel()
        {
            InitializeComponent();

            MinimumSize = Size;
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