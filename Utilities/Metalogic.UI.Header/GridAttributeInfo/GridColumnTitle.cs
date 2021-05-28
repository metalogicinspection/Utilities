using System;

namespace Metalogic.UI.Header.GridAttributeInfo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridColumnTitle : GridViewFiedlAttributeBase
    {
        public string Title { get; set; }

        public GridColumnTitle(string viewName, string title)
        {
            ViewName = viewName;
            Title = title;
        }

        public GridColumnTitle(string title)
        {
            ViewName = string.Empty;
            Title = title;
        }
        
    }
}