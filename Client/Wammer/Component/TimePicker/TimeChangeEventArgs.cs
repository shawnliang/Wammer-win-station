#region

using System;

#endregion

namespace Waveface.Component.TimePickerEx
{
    public class TimeChangeEventArgs : EventArgs
    {
        public TimeSpan OldSelectedTime { get; private set; }
        public TimeSpan MyProperty { get; private set; }

        public TimeChangeEventArgs(TimeSpan sOldSelectedTime, TimeSpan sNewSelectedTime)
        {
            OldSelectedTime = sOldSelectedTime;
            MyProperty = sNewSelectedTime;
        }
    }
}