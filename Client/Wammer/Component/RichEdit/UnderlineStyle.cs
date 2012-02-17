
namespace Waveface.Component.RichEdit
{
    public enum UnderlineStyle
    {
        // No underlining
        None = RichEditConstants.CFU_UNDERLINENONE,

        // Normal underlining
        Normal = RichEditConstants.CFU_UNDERLINE,

        // Word underlining. Equivalent to Normal. 
        Word = RichEditConstants.CFU_UNDERLINEWORD,

        // Double underlining
        Double = RichEditConstants.CFU_UNDERLINEDOUBLE,

        // Dotted underlining
        Dotted = RichEditConstants.CFU_UNDERLINEDOTTED,

        // Dashed underlining
        Dash = RichEditConstants.CFU_UNDERLINEDASH,

        // Underlining consiting of alternating dashes and dots
        DashDot = RichEditConstants.CFU_UNDERLINEDASHDOT,

        // Underlining consisting of a dash followed by two dots
        DashDotDot = RichEditConstants.CFU_UNDERLINEDASHDOTDOT,

        // Wavy underlining
        Wave = RichEditConstants.CFU_UNDERLINEWAVE,

        // Underlining consisting of a single thick line
        Thick = RichEditConstants.CFU_UNDERLINETHICK,

        // Not supported - equivalent to Normal.
        Hairline = RichEditConstants.CFU_UNDERLINEHAIRLINE,

        // Not supported - equivalent to Normal.
        DoubleWave = RichEditConstants.CFU_UNDERLINEDOUBLEWAVE,

        // Not supported - equivalent to Normal.
        HeavyWave = RichEditConstants.CFU_UNDERLINEHEAVYWAVE,

        // Not supported - equivalent to Normal.
        LongDash = RichEditConstants.CFU_UNDERLINELONGDASH,

        // Not supported - equivalent to Normal.
        ThickDash = RichEditConstants.CFU_UNDERLINETHICKDASH,

        // Not supported - equivalent to Normal.
        ThickDashDot = RichEditConstants.CFU_UNDERLINETHICKDASHDOT,

        // Not supported - equivalent to Normal.
        ThickDashDotDot = RichEditConstants.CFU_UNDERLINETHICKDASHDOTDOT,

        // Not supported - equivalent to Normal.
        ThickDotted = RichEditConstants.CFU_UNDERLINETHICKDOTTED,

        // Not supported - equivalent to Normal.
        ThickLongDash = RichEditConstants.CFU_UNDERLINETHICKLONGDASH
    }
}