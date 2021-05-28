using System;

namespace Metalogic.UI.Header.GridAttributeInfo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridColumnVisibleIndex : GridViewFiedlAttributeBase
    {
        public GridColumnVisibleIndex(string viewName, int visibleIndex)
        {
            ViewName = viewName;
            VisibleIndex = visibleIndex;
        }
        public GridColumnVisibleIndex(int visibleIndex)
            : this(string.Empty, visibleIndex)
        {
        }

        public int VisibleIndex { get; set; }
    }
}