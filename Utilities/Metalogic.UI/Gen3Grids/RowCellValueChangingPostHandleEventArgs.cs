using System;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Gen3.Data;

namespace Gen3.UI.Grids
{
    public class RowCellValueChangingPostHandleEventArgs : EventArgs
    {
        public GridView View { get; internal set; }
        public int RowHandle { get; internal set; }
        public GridColumn Column { get; internal set; }
        public object RowObject { get; internal set; }

        public IGen3DataList DataList { get; internal set; }

        public bool NewRow { get; internal set; }
    }
}