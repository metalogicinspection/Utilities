using System;
using System.Reflection;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Gen3.Data;

namespace Gen3.UI.Grids
{
    public class MasterDetailViewSetting
    {
        public MasterDetailViewSetting()
        {
            AllowAddNewRow = true;
            AllowDelete = true;
        }

        public Type MemberType { get; set; }
        public bool AllowAddNewRow { get; set; }

        public bool AllowDetailButton { get; set; }

        public bool AllowDelete { get; set; }

        public string IndexColumnName { get; set; }

        public string InitRowFirstEnterColumnName { get; set; }

        internal GridColumn IndexColumn { get; set; }

        internal PropertyInfo IndexPropertyInfo { get; set; }

        internal PropertyInfo HashPropertyInfo { get; set; }

        internal GridColumn HashColumn { get; set; }


        internal GridColumn DeleteColumn { get; set; }
        internal GridView View { get; set; }

        public IGen3DataList DataList { get; set; }

        public bool NewRowCopyFromPreviousRow { get; set; }
    }
}