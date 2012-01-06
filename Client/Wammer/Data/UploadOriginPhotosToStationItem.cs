
namespace Waveface
{
    public class UploadOriginPhotosToStationItem
    {
        public string FilePath_OID { get; set; }
        public string FilePath_REAL { get; set; }
        public string ObjectID { get; set; }

        public UploadOriginPhotosToStationItem()
        {
            FilePath_OID = string.Empty;
            FilePath_REAL = string.Empty;
            ObjectID = string.Empty;
        }
    }
}
