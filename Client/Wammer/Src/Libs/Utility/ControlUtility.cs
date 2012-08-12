
using System.Windows.Forms;

namespace Waveface
{
    public class ControlUtility
    {
        public static void DisableAllTabStop(Control container)
        {
            foreach (Control _c in container.Controls)
            {
                DisableAllTabStop(_c);

                _c.TabStop = false;
            }
        }
    }
}