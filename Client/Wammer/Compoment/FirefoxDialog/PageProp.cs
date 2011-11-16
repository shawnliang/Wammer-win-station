
#region

using Waveface.Component.MozBar;

#endregion

namespace Waveface.Component.FirefoxDialog
{
    public class PageProp
    {
        public int ImageIndex { get; set; }
        public string Text { get; set; }
        public PropertyPage Page { get; set; }
        public MozItem MozItem { get; set; }
    }
}