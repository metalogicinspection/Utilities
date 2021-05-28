using System;

namespace Metalogic.DataUtil
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class LegacyPropertyName : Attribute
    {
        public LegacyPropertyName(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }
    }
}
