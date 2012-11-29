using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
    public interface IExifExtractor
    {
        /// <summary>
        /// Extracts exif information from image file
        /// </summary>
        /// <param name="imageData"></param>
        /// <returns>exif data; null is returned if no exif is embeded or error</returns>
        exif extract(ArraySegment<byte> imageData);
    }
}
