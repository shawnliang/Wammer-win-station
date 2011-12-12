#region

using System.Runtime.InteropServices;

#endregion

namespace Waveface.DragDropLib
{
    [ComImport]
    [Guid("4657278A-411B-11d2-839A-00C04FD918D0")]
    public class DragDropHelper
    {
    }

    //
    //  Drag Sample Code
    //
    //
    //private void bt_MouseDown(object sender, MouseEventArgs e)
    //{
    //    Bitmap bmp = new Bitmap(100, 100, PixelFormat.Format32bppArgb);
    //    using (Graphics g = Graphics.FromImage(bmp))
    //    {
    //        g.Clear(Color.Magenta);
    //        using (Pen pen = new Pen(Color.Blue, 4f))
    //        {
    //            g.DrawEllipse(pen, 20, 20, 60, 60);
    //        }
    //    }
    //
    //    DataObject data = new DataObject(new Waveface.DragDropLib.DataObject());
    //
    //    ShDragImage shdi = new ShDragImage();
    //    Win32Size size;
    //    size.cx = bmp.Width;
    //    size.cy = bmp.Height;
    //    shdi.sizeDragImage = size;
    //    Point p = e.Location;
    //    Win32Point wpt;
    //    wpt.x = p.X;
    //    wpt.y = p.Y;
    //    shdi.ptOffset = wpt;
    //    shdi.hbmpDragImage = bmp.GetHbitmap();
    //    shdi.crColorKey = Color.Magenta.ToArgb();
    //
    //    IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();
    //    sourceHelper.InitializeFromBitmap(ref shdi, data);
    //
    //    DoDragDrop(data, DragDropEffects.Copy);
    //}
}