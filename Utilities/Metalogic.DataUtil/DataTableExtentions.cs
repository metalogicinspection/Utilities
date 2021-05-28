using System.Data;

namespace Metalogic.DataUtil
{
    public static class DataTableExtentions
    {
        public static DataRow GetFirstRow(this DataTable tbl)
        {
            return tbl.Rows.Count == 0 ? null : tbl.Rows[0];
        }
    }
}
