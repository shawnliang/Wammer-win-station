
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
    }
}
