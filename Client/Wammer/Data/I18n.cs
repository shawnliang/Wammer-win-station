using System.Windows.Forms;
using Waveface.Localization;

namespace Waveface
{
    public class I18n
    {
        private static Localizer s_localizer;

        public static Localizer L
        {
            get { return s_localizer; }
            set { s_localizer = value; }
        }

        static I18n()
        {
            s_localizer = new Localizer();
            s_localizer.WItemsFullPath = Application.StartupPath + "\\WavefaceML.xml";
            s_localizer.CurrentCulture = CultureManager.ApplicationUICulture;
        }
    }
}
