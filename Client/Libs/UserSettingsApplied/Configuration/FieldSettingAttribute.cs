
#region

using System;

#endregion

namespace Waveface.Configuration
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class FieldSettingAttribute : Attribute
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }
    }
}