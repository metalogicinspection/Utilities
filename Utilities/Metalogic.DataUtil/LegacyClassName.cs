using System;

namespace Metalogic.DataUtil
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LegacyClassName : Attribute
    {
        public LegacyClassName(string fullClassName)
        {
            Name = fullClassName;
        }

        public LegacyClassName(string nameSpace, string className)
        {
            Name = string.Concat(nameSpace, className);
        }

        public string Name { get; private set; }


    }
}
