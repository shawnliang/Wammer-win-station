
#region

using System.Drawing;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    internal partial class TaskBar
    {
        public static Rectangle GetTaskBarRectangle()
        {
            Point _location = GetTaskBarLocation();
            Size _size = GetTaskBarSize();

            return Rectangle.FromLTRB(_location.X, _location.Y,
                                      _location.X + _size.Width, _location.Y + _size.Height);
        }
    }
}