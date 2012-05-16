#region

using Waveface.Component;

#endregion

namespace Waveface
{
    internal interface IDetailView
    {
        bool CanEdit();

        ImageButton GetMoreFonction1();
        void MoreFonction1();
    }
}