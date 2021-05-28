using System;
using System.Linq;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Metalogic.DataUtil;

namespace Metalogic.UI
{
    public static class GridViewExtentions
    {
        public static void HideCompletely(this GridColumn col)
        {
            col.VisibleIndex = -1;
            col.ShowUnboundExpressionMenu = false;
            col.OptionsColumn.ShowInCustomizationForm = false;
        }
        
        internal static object ToObject(this GridView gridview, int rowHandle, Type rqT)
        {
            object retValue;

            if (typeof (PicklistItem).IsAssignableFrom(rqT))
            {
                retValue = PicklistItem.GetPickListByType(rqT)
                    .FirstOrDefault(x => x.Code == gridview.GetRowCellValue(rowHandle, "Code").ToString());
                if (retValue != null)
                {
                    return retValue;
                }
            }

            retValue = Activator.CreateInstance(rqT);

            foreach (var prpty in rqT.GetProperties().Where(x => x.CanWrite && !x.GetIndexParameters().Any()))
            {
                var gridCOl = gridview.Columns.ColumnByFieldName(prpty.Name);

                if (gridCOl == null)
                {
                    continue;
                }
                var val = gridview.GetRowCellValue(rowHandle, gridCOl);
                val = DBNull.Value == val ? null : val;
                prpty.SetValue(retValue, val, null);
            }
            return retValue;
        }
    }
}
