
#region

using System;

#endregion

namespace Waveface.Compoment.PopupControl
{
    // Types of animation of the pop-up window.
    [Flags]
    public enum PopupAnimations
    {
        None = 0,

        // Animates the window from left to right. This flag can be used with roll or slide animation.
        LeftToRight = 0x00001,

        // Animates the window from right to left. This flag can be used with roll or slide animation.
        RightToLeft = 0x00002,

        // Animates the window from top to bottom. This flag can be used with roll or slide animation.
        TopToBottom = 0x00004,

        // Animates the window from bottom to top. This flag can be used with roll or slide animation.
        BottomToTop = 0x00008,

        // Makes the window appear to collapse inward if it is hiding or expand outward if the window is showing.
        Center = 0x00010,

        // Uses a slide animation.
        Slide = 0x40000,

        // Uses a fade effect.
        Blend = 0x80000,

        // Uses a roll animation.
        Roll = 0x100000,

        // Uses a default animation.
        SystemDefault = 0x200000,
    }
}