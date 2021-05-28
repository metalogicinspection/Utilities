using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Metalogic.DataUtil
{
    public abstract class DataTableChangedItem
    {
        public DataRow NewState { get; internal set; }
        public DataRow OldState { get; internal set; }

        public object NewValue { get; internal set; }
        public object OldValue { get; internal set; }

    }

    public class DataTableRowCellChanged : DataTableChangedItem
    {
        public string Column { get; internal set; }
    }

    public class DataTableRowAdded : DataTableChangedItem
    {
    }
    
    public class DataTableRowDeleted : DataTableChangedItem
    {
    }


    public static class DataSetExtentions
    {
        public static void Import(this DataTable target, DataTable source)
        {
            var columns = target.Columns.Cast<DataColumn>();
            DataColumn targetColumn;
            if (columns.Any(srcColumn => (targetColumn = source.Columns[srcColumn.ColumnName]) == null || targetColumn.DataType != srcColumn.DataType))
            {
                throw new Exception("Data table columns structure not match");
            }
            target.Rows.Clear();


            var tblAsMcTable = target as TableBase;
            var otrTblAsMcTable = source as TableBase;
            if (tblAsMcTable != null && otrTblAsMcTable != null)
            {
                foreach (DataRow otrRow in otrTblAsMcTable.Rows)
                {
                    var obj = otrRow.ToObject();
                    tblAsMcTable.AddNewRow(obj);
                }
            }
            else
            {
                foreach (DataRow otrRow in source.Rows)
                {
                    target.ImportRow(otrRow);
                }
            }
            target.AcceptChanges();
        }
        

        public static TableBase GetTableByTemplateType<T>(this DataSet set) where T : class
        {
            var t = typeof (T);
            return set.Tables.OfType<TableBase>().FirstOrDefault(tbl => tbl.MemberType == t);
        }

        public static TableBase GetTableByTemplateType(this DataSet set, Type type)
        {
            return set.Tables.OfType<TableBase>().FirstOrDefault(tbl => tbl.MemberType == type);
        }

        public static object ToObject(this DataRow row)
        {
            var tbl = row.Table as TableBase;
            if (tbl == null)
            {
                throw new Exception("Template Type do not match data table's tempalte type");
            }

            object retValue;

            if (typeof(PicklistItem).IsAssignableFrom(tbl.MemberType))
            {
                retValue = PicklistItem.GetPickListByType(tbl.MemberType)
                            .FirstOrDefault(x => x.Code == row["Code"].ToString());
                if (retValue != null)
                {
                    return retValue;
                }
            }

            retValue = Activator.CreateInstance(tbl.MemberType);

            foreach (var prpty in tbl.MemberType.GetProperties().Where(x => x.CanWrite && !x.GetIndexParameters().Any()))
            {
                DataColumn col;
                object val;
                if ((col = tbl.Columns[prpty.Name]) != null && (val = row[col]) != DBNull.Value)
                {
                    try
                    {
                        prpty.SetValue(retValue, val, null);
                    }
                    catch (Exception)
                    {

                    }
                }

            }
            return retValue;
        }

        /// <summary>
        /// Convert this row to the template object then load fields value from it
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static void UpdateFields(this DataRow row)
        {
            var item = ToObject(row);
            if (item == null)
            {
                return;
            }

            var itemType = item.GetType();
            foreach (var prpty in itemType.GetProperties().Where(x => x.CanRead))
            {
                var val = prpty.GetValue(item, null);

                row[prpty.Name] = val ?? DBNull.Value;
            }
        }

        /// <returns></returns>
        /// <summary>
        /// load fields value from template object then
        /// </summary>
        /// <param name="row"></param>
        /// <param name="info"></param>
        public static void UpdateFields(this DataRow row, object info)
        {
            if (info == null)
            {
                return;
            }
            var itemType = info.GetType();
            foreach (var prpty in itemType.GetProperties().Where(x => x.CanRead))
            {
                if (row.Table.Columns[prpty.Name] == null)
                {
                    continue;
                }
                var val = prpty.GetValue(info, null);
                row[prpty.Name] = val ?? DBNull.Value;
            }
        }

        public static object ToObject(this DataRow row, Type type)
        {
            object retValue;

            if (typeof(PicklistItem).IsAssignableFrom(type))
            {
                retValue = PicklistItem.GetPickListByType(type)
                            .FirstOrDefault(x => x.Code == row["Code"].ToString());
                if (retValue != null)
                {
                    return retValue;
                }
            }

            retValue = Activator.CreateInstance(type);

            foreach (var prpty in type.GetProperties().Where(x => x.CanWrite && !x.GetIndexParameters().Any()))
            {
                var col = row.Table.Columns[prpty.Name];
                object val;
                if (col == null)
                {
                    prpty.GetCustomAttributes(typeof (LegacyPropertyName), true)
                        .OfType<LegacyPropertyName>()
                        .Any(x => (col = row.Table.Columns[x.Name]) != null);
                }

                if (col == null || (val = row[col]) == DBNull.Value)
                {
                    if (!prpty.PropertyType.IsSubclassOf(typeof (PicklistItem)))
                    {
                        continue;
                    }
                    val = null;
                }
                if (prpty.PropertyType == typeof (string))
                {
                    if (col.DataType != typeof (string))
                    {
                        val = val.ToString();
                    }
                }
                else if (prpty.PropertyType == typeof (bool))
                {
                    if (col.DataType != typeof (bool))
                    {
                        bool tpValue;
                        if (bool.TryParse(val.ToString(), out tpValue))
                        {
                            val = tpValue;
                        }
                    }
                }

                try
                {
                    prpty.SetValue(retValue, val, null);
                }
                catch (Exception)
                {

                }
            }
            return retValue;
        }

        public static T ToObject<T>(this DataRow row) where T : class
        {
            var tbl = row.Table as TableBase;
            if (tbl == null)
            {
                throw new Exception("Datarow is not belongs to MCTable");
            }

            var rqT = typeof(T);
            if (!(tbl.MemberType.IsAssignableFrom(rqT) || rqT.IsAssignableFrom(tbl.MemberType)))
            {
                throw new Exception("Template Type do not match data table's tempalte type");
            }


            T retValue;

            if (typeof (PicklistItem).IsAssignableFrom(tbl.MemberType))
            {
                retValue = PicklistItem.GetPickListByType(tbl.MemberType)
                            .FirstOrDefault(x => x.Code == row["Code"].ToString()) as T;
                if (retValue != null)
                {
                    return retValue;
                }
            }

            retValue = Activator.CreateInstance(rqT) as T;

            var prptys = !tbl.MemberType.IsAssignableFrom(rqT) ? rqT.GetProperties() : tbl.MemberType.GetProperties();

            foreach (var prpty in prptys.Where(x => x.CanWrite && !x.GetIndexParameters().Any()))
            {
                var col = tbl.Columns[prpty.Name];
                object val;
                if (col == null)
                {
                    prpty.GetCustomAttributes(typeof(LegacyPropertyName), true)
                        .OfType<LegacyPropertyName>()
                        .Any(x => (col = row.Table.Columns[x.Name]) != null);
                }
                if (col != null && (val = row[col]) != DBNull.Value)
                {
                    prpty.SetValue(retValue, val, null);
                }

            }
            return retValue;
        }

        public static bool IsStringWhiteSpaceOrEmpty(this DataRow row, string column)
        {
            DataColumn columnC;
            if ((columnC = row.Table.Columns[column]) == null || columnC.DataType != typeof(string))
            {
                throw new Exception(string.Concat(column, " does not belongs to table or it is not a string type"));
            }
            return row[columnC] != DBNull.Value && string.IsNullOrWhiteSpace(row[columnC].ToString());
        }

        public static void AddDelta(this DataRow row, string column, TimeSpan delta)
        {
            DataColumn columnC;
            if ((columnC = row.Table.Columns[column]) == null)
            {
                throw new Exception(string.Concat(column, " does not belongs to table or it is not a string type"));
            }
            if (row[columnC] == DBNull.Value)
            {
                return;
            }
            var date = (DateTime)row[columnC];
            row[columnC] = date.Add(delta);
        }


        public static void LoadFieldsFromOther(this object target, object other, List<string> ignoreColumns = null)
        {
            var ot = other.GetType();
            var tt = target.GetType();


            var matchedFields = ot.GetProperties().Join(tt.GetProperties(), 
                otP => otP.Name, 
                ttP => ttP.Name, 
                (otP, ttP) => new {TargetField = ttP, OtherField = otP })
                .Where(x => x.OtherField.CanRead 
                        && x.TargetField.CanWrite
                        && !x.TargetField.GetIndexParameters().Any()
                        && !x.OtherField.GetIndexParameters().Any()
                        && x.TargetField.PropertyType.IsAssignableFrom(x.OtherField.PropertyType));

            foreach (var pair in matchedFields)
            {
                if (ignoreColumns?.Contains(pair.OtherField.Name) ?? false)
                {
                    continue;
                }
                var val = pair.OtherField.GetValue(other, null);
                pair.TargetField.SetValue(target,val,null);
            }
        }

        public static Queue<T> ToQueue<T>(this List<T> list) where T : class
        {
            return new Queue<T>(list);
        }




        /// <summary>
        /// convert a string list into 1 string
        /// Example:
        /// {"a", "a", "b", "c", "b"} => "a; b; c"
        /// </summary>
        /// <param name="sList"></param>
        /// <returns></returns>
        public static string ToSemicolonString(this IEnumerable<string> sList)
        {
            var iList = sList.Distinct().ToList();
            if (iList.Count == 1)
            {
                return iList[0];
            }
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < iList.Count; ++i)
            {
                var curValue = iList[i];
                stringBuilder.Append(curValue);
                if (i != iList.Count - 1)
                {
                    stringBuilder.Append("; ");
                }
            }
            return stringBuilder.ToString();
        }
    }
}
