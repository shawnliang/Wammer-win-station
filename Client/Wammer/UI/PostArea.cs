#region

using System.Collections.Generic;
using System.Windows.Forms;
using Waveface.API.V2;

#endregion

namespace Waveface
{
    public partial class PostArea : UserControl
    {
        public PostsList PostsList
        {
            get { return postList; }
        }

        public PostArea()
        {
            InitializeComponent();

            comboBoxType.SelectedIndex = 0;
        }

        public void ShowTypeUI(bool flag)
        {
            if(flag)
            {
                labelDisplay.Visible = true;
                comboBoxType.Visible = true;
            }
            else
            {
                labelDisplay.Visible = false;
                comboBoxType.Visible = false;                
            }
        }
    }
}