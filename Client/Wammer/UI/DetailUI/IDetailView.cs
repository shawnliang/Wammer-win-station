using System.Collections.Generic;
using System.Windows.Forms;

namespace Waveface
{
    interface IDetailView
    {
        bool CanEdit();

        List<ToolStripMenuItem> GetMoreMenuItems();
    }
}
