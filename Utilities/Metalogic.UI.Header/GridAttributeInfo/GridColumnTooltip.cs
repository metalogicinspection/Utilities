using System;

namespace Metalogic.UI.Header.GridAttributeInfo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridColumnTooltip : GridViewFiedlAttributeBase
    {
        public string Tooltip { get; set; }

        public GridColumnTooltip(string tooltip)
        {
            Tooltip = tooltip;
        }      
    }
}