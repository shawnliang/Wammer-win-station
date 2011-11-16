#region

using System.Drawing;

#endregion

namespace Waveface.ImageCapture.States
{
    internal interface ICaptureState
    {
        void Paint(Graphics graphics);
        void Start(Point location);
        void Update(Point location);
        void End();
        Image Capture();
    }
}