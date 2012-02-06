
using System.Windows.Forms;

namespace Waveface
{
    public class ControlUtility
    {
        public static void DisableAllTabStop(Control container)
        {
            foreach (Control c in container.Controls)
            {
                DisableAllTabStop(c);

                c.TabStop = false;
            }
        }
    }
}