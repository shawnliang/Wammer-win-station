
namespace Waveface
{
    public class ImageItem
    {
        public PostItemType PostItemType { get; set; }

        public string ThumbnailPath { get; set; }
        public string CloudOriginPath { get; set; }
        public string OriginPath { get; set; }
        public string MediumPath { get; set; }
        public string LocalFilePath_Origin { get; set; }
        public string LocalFilePath_Medium { get; set; }

        public int ErrorTry { get; set; }

        public ImageItem()
        {
            ThumbnailPath = string.Empty;
            CloudOriginPath = string.Empty;
            OriginPath = string.Empty;
            MediumPath = string.Empty;
            LocalFilePath_Origin = string.Empty;
            LocalFilePath_Medium = string.Empty;
        }
    }
}
