
#region

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.FirefoxDialog
{
    public class PropertyPage : UserControl
    {
        private Container components;

        public bool IsInit { get; set; }

        public PropertyPage()
        {
            InitComponent();
        }

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

        #region Component Designer generated code

        private void InitComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion

        #region Overridables

        public new virtual string Text
        {
            get
            {
                return GetType().Name;
            }
        }

        public virtual Image Image
        {
            get
            {
                return null;
            }
        }

        public virtual void OnInit()
        {
            IsInit = true;
        }

        public virtual void OnSetActive()
        {
        }

        public virtual void OnApply()
        {
        }

        #endregion
    }
}