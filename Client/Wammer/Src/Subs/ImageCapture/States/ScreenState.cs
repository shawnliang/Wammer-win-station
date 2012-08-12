#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.ImageCapture.States
{
    internal class ScreenState : BaseState
    {
        public ScreenState(Form owner) : base(owner)
        {
            owner.Cursor = Cursors.Hand;
        }

        #region ICaptureState Members

        public override void Paint(Graphics graphics)
        {
            base.Paint(graphics, new Rectangle());
        }

        public override Image Capture()
        {
            return Owner.BackgroundImage;
        }

        #endregion
    }
}