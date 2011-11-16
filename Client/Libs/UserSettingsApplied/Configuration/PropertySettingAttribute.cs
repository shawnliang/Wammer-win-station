
#region

using System;

#endregion

namespace Waveface.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertySettingAttribute : Attribute
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }
    }
}