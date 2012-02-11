
using System.Drawing;

namespace Waveface.Component
{
    public partial class MyTaskbarNotifier : TaskbarNotifier
    {
        public Image AvatarImage
        {
            set { pictureBoxAvatar.Image = value; }
        }

        public MyTaskbarNotifier()
        {
            InitializeComponent();
        }
    }
}
