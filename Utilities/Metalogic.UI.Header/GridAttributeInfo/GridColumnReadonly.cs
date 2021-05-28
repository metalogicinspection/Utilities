using System;

namespace Metalogic.UI.Header.GridAttributeInfo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridColumnReadonly : GridViewFiedlAttributeBase
    {
        public GridColumnReadonly(bool readOnly)
        {
            ReadOnly = readOnly;
        }

        public bool ReadOnly { get; set; }
    }
}