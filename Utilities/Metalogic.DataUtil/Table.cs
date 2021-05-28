using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace Metalogic.DataUtil
{
    public class TableBase : DataTable
    {
        public TableBase()
        {
            RowChanged += HandleRowChanged;
            RowDeleting += HandleRowDeleting;
            TableClearing += HandleTableClearing;
        }

        public TableBase(Type type) : this()
        {
            MemberType = type;
        }

        public static TableBase CreateTableByType<T>() where T : class
        {
            return new TableBase(typeof (T));
        }

        public static TableBase CreateTableByItem(object item)
        {
            var retValue = new TableBase();
            retValue.AddNewRow(item);
            return retValue;
        }

        public static TableBase CreateTableByItems<T>(IEnumerable<T> items) where T : class
        {
            var retValue = new TableBase(typeof (T));
            foreach (var item in items)
            {
                retValue.AddNewRow(item);

            }
            return retValue;
        }

        public static TableBase CreateTableByItems<T>(DataTable table) where T : class
        {
            var retValue = new TableBase(typeof (T));
            foreach (DataRow item in table.Rows)
            {
                var obj = item.ToObject(retValue.MemberType);
                retValue.AddNewRow(obj);
            }
            return retValue;
        }

        private void HandleTableClearing(object sender, DataTableClearEventArgs e)
        {
            FocusedRow = null;
        }

        private void HandleRowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete)
            {
                return;
            }

            if (_focusedRow != e.Row)
            {
                return;
            }

            if (Rows.Count < 2)
            {
                FocusedRow = null;
            }
            else
            {
                FocusedRow = Rows[(Rows.IndexOf(FocusedRow) + 1)%Rows.Count];
            }
        }

        private void HandleRowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Add)
            {
                return;
            }

            if (_focusedRow == null)
            {
                FocusedRow = e.Row;
            }
        }

        public void RemoveRow(DataRow row)
        {
            Rows.Remove(row);
        }

        private Type _memberType = null;

        public Type MemberType
        {
            get { return _memberType; }
            set
            {
                if (value == null)
                {
                    throw new Exception("Set MemberType Failed. value can not be null.");
                }
                if (_memberType != null)
                {
                    throw new Exception("Set MemberType Failed. MemberType Already Set.");
                }
                _memberType = value;
                if (string.IsNullOrEmpty(TableName))
                {
                    TableName = MemberType.Name;
                }
                foreach (var prpty in MemberType.GetProperties())
                {
                    if (Columns[prpty.Name] != null)
                    {
                        return;
                    }
                    Columns.Add(new DataColumn(prpty.Name, prpty.PropertyType));
                }
            }
        }

        public DataRow AddNewRow(object item, int index = -1)
        {
            if (item == null)
            {
                throw new Exception("item can not be null");
            }

            if (MemberType == null)
            {
                MemberType = item.GetType();
            }

            var itemType = item.GetType();
            if (itemType != MemberType && !MemberType.IsAssignableFrom(itemType))
            {
                throw new Exception("item is not an object of class: " + MemberType.Name);
            }

            var row = NewRow();
            foreach (var prpty in MemberType.GetProperties().Where(x => x.CanRead))
            {
                var val = prpty.GetValue(item, null);

                row[prpty.Name] = val ?? DBNull.Value;
            }

            if (index < 0)
            {
                Rows.Add(row);
            }
            else
            {
                Rows.InsertAt(row, index);
            }

            return row;
        }

        public event EventHandler<FocusedRowChangedEventArgs> FocusedRowChanged;

        public class FocusedRowChangedEventArgs : EventArgs
        {
            public DataRow PreviousFocusedRow { get; set; }

            public DataRow CurrentFocusedRow { get; set; }
        }

        private bool _update = false;

        public void BeginUpdateData()
        {
            _update = true;
        }

        public void EndUpdateData()
        {
            _update = false;
            if (FocusedRowChanged != null)
            {
                FocusedRowChanged(this,
                    new FocusedRowChangedEventArgs() {PreviousFocusedRow = null, CurrentFocusedRow = _focusedRow});
            }
        }

        private DataRow _focusedRow;

        public DataRow FocusedRow
        {
            get { return _focusedRow; }
            set
            {
                if (value != null)
                {
                    if (value.Table != this)
                    {
                        throw new Exception("Row does not belong to this table.");
                    }
                    if (Rows.IndexOf(value) < 0)
                    {
                        throw new Exception("Row is not added to Rows");
                    }
                }

                if (_focusedRow == value)
                {
                    return;
                }

                var oldValue = _focusedRow;
                _focusedRow = value;

                if (FocusedRowChanged != null && !_update)
                {
                    FocusedRowChanged(this,
                        new FocusedRowChangedEventArgs()
                        {
                            PreviousFocusedRow = oldValue,
                            CurrentFocusedRow = _focusedRow
                        });
                }
            }
        }

        public override DataTable Clone()
        {
            if (MemberType == null)
            {
                throw new Exception("Clone Table Failed: MemberType can not be null.");
            }

            var retValue = new TableBase(MemberType);
            return retValue;
        }

        public object GetFirstRowAsObject()
        {
            if (Rows.Count == 0)
            {
                return null;
            }
            try
            {
                return Rows[0].ToObject(MemberType);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public List<DataTableChangedItem> Diff(TableBase other, List<string> ignoreColumnsNames = null)
        {

            var retValue = new List<DataTableChangedItem>();


            if (Columns.Count != other.Columns.Count)
            {
                throw new Exception("Data Columns Count are not the same");
            }

            var columns = Columns.Cast<DataColumn>().ToList();
            if (columns.Any(col => other.Columns[col.ColumnName] == null))
            {
                throw new Exception("Data Columns are not the same");
            }

            columns = columns.Where(x => x.DataType != typeof(byte[])).ToList();
            if (ignoreColumnsNames != null)
            {
                columns = columns.Where(x => !ignoreColumnsNames.Contains(x.ColumnName)).ToList();
            }

            columns = columns
                        .Join(MemberType.GetProperties()
                        .Where(x => x.CanWrite),
                            l => l.ColumnName,
                            r => r.Name,
                            (l, r) => l)
                        .ToList();

            
            var minCount = Math.Min(Rows.Count, other.Rows.Count);
            for (var i = 0; i < minCount; ++i)
            {
                var curRow = Rows[i];
                var preRow = other.Rows[i];
                foreach (var dataColumn in columns)
                {
                    var curValue = curRow[dataColumn];
                    var previousValue = preRow[dataColumn.ColumnName];
                    if (dataColumn.DataType == typeof (string))
                    {
                        if (curValue == DBNull.Value)
                        {
                            curValue = string.Empty;
                        }
                        if (previousValue == DBNull.Value)
                        {
                            previousValue = string.Empty;
                        }
                    }

                    if (!curValue.Equals(previousValue))
                    {
                        retValue.Add(new DataTableRowCellChanged { Column = dataColumn.ColumnName, NewState = curRow, NewValue = curRow[dataColumn.ColumnName], OldValue = preRow[dataColumn.ColumnName], OldState = preRow });
                    }
                }
            }

            for (var i = minCount; i < Rows.Count; ++i){
                var curRow = Rows[i];
                retValue.Add(new DataTableRowAdded {NewState = curRow, OldState = null });
            }

            for (var i = minCount; i < other.Rows.Count; ++i)
            {
                var oldRow = other.Rows[i];
                retValue.Add(new DataTableRowDeleted { NewState = null, OldState = oldRow });
            }

            return retValue;
        }
    }
}
