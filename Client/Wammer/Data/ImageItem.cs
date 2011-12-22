
namespace Waveface
{
    public class ImageItem
    {
        public PostItemType PostItemType { get; set; }

        public string ThumbnailPath { get; set; }
        public string OriginPath { get; set; }
        public string MediumPath { get; set; }
        public string LocalFilePath { get; set; }
        public string LocalFilePath2 { get; set; }

        public int ErrorTry { get; set; }

        public ImageItem()
        {
            ThumbnailPath = string.Empty;
            OriginPath = string.Empty;
            MediumPath = string.Empty;
            LocalFilePath = string.Empty;
            LocalFilePath2 = string.Empty;
        }
    }
}
