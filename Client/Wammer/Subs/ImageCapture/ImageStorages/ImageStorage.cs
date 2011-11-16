#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

#endregion

namespace Waveface.ImageCapture
{
    internal class ImageStorage
    {
        private readonly string m_root;
        private readonly object m_sync = new object();

        public ImageStorage(string root)
        {
            m_root = root;
        }

        #region IImageStorage Members

        public CaptureInfo SaveImage(Image image, ImageFormat imageFormat, string filename)
        {
            Directory.CreateDirectory(m_root); //create directory if not exist
            string _pathToSave = Path.Combine(m_root, filename);

            image.Save( _pathToSave,ImageFormat.Jpeg);

            Image _thumbnail = image.GetThumbnailImage(16, 16, () => false, IntPtr.Zero);

            return new CaptureInfo(filename, new Uri(_pathToSave), _pathToSave, _thumbnail);
        }

        public bool IsImageInStorege(CaptureInfo captureInfo)
        {
            lock (m_sync)
            {
                if ((captureInfo == null) || (captureInfo.Url == null) || string.IsNullOrEmpty(captureInfo.Path))
                    return false;

                return System.IO.File.Exists(captureInfo.Path);
            }
        }

        #endregion
    }
}