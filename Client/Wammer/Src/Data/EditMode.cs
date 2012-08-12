
namespace Waveface
{
    public enum EditModePhotoType
    {
        NewPostOrigin,
        NewPostNewAdd,
        EditModeOrigin,
        EditModeNewAdd
    }

    public class EditModeImageListViewItemTag
    {
        public EditModePhotoType AddPhotoType { get; set; }
        public string ObjectID { get; set; }
        public bool IsCoverImage_UI { get; set; }
    }

    public class DetailViewImageListViewItemTag
    {
        public string Index { get; set; }
        public bool IsCoverImage { get; set; }
    }
}
