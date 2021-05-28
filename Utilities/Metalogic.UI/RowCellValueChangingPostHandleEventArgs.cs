using System;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Metalogic.UI
{
    public class RowCellValueChangingPostHandleEventArgs : EventArgs
    {
        public GridView View { get; internal set; }
        public int RowHandle { get; internal set; }
        public GridColumn Column { get; internal set; }
        public object RowObject { get; internal set; }
    }
}