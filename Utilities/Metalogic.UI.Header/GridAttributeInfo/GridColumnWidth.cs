using System;

namespace Metalogic.UI.Header.GridAttributeInfo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridColumnWidth : GridViewFiedlAttributeBase
    {
        public GridColumnWidth(string viewName, int width)
        {
            ViewName = viewName;
            Width = width;
        }

        public GridColumnWidth(int width) : this(string.Empty, width)
        {
        }


        public int Width { get; set; }
    }
}