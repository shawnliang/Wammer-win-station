using System;
using System.Drawing;

namespace Waveface.ImageCapture
{
  [Serializable]
  public class CaptureInfo
  {
    public string Name { get; private set; }
    public Uri Url { get; private set; }
    public string Path { get; private set; }
    public Image Image { get; private set; }

    public CaptureInfo(string name, Uri url, string path, Image image) 
    {
      Name = name;
      Url = url;
      Path = path;
      Image = image;
    }
  }
}
