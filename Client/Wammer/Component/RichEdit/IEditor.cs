
#region

using System.Drawing;

#endregion

namespace Waveface.Component.RichEdit
{
    internal interface IEditor
    {
        CharacterRange SelectionRange { get; }
        string Text { get; }
        int TextLength { get; }

        string GetTextRange(CharacterRange range);
    }
}