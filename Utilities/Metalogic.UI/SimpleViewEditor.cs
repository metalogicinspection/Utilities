using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Metalogic.DataUtil;
using Metalogic.UI.Header.GridAttributeInfo;

namespace Metalogic.UI
{
    public partial class SimpleViewEditor : UserControl
    {
        public delegate void RowCellValueChangingPostEventHandler(object sender, RowCellValueChangingPostHandleEventArgs e);

        public event RowCellValueChangingPostEventHandler RowCellValueChangingPost;


        public event RowCellValueChangingPostEventHandler NewRowAddingEvent;

        public SimpleViewEditor()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            weldView.CustomDrawCell += WeldView_CustomDrawCell;
            MinimumIndex = 0;
        }

        private void WeldView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 && !e.Column.OptionsColumn.AllowFocus)
                {
                    e.Appearance.BackColor = Color.LightGray;
                }}
            catch (Exception)
            {
                return;
            }
        }

        public bool AllowAddNewRow {
            get
            {
                return weldView.OptionsView.NewItemRowPosition != NewItemRowPosition.None;
                
            }
            set
            {
                weldView.OptionsView.NewItemRowPosition = value ? NewItemRowPosition.Bottom : NewItemRowPosition.None;
            }
        }

        private bool _allowDelete = true;
        public bool AllowDelete {
            get { return _allowDelete; }
            set
            {
                _allowDelete = value;
                if (_deleteColumn != null)
                {
                    _deleteColumn.VisibleIndex = value? weldView.Columns.Max(x => x.VisibleIndex) + 1 : -1;
                }
            }
        }
        
        public GridView View => weldView;

        private void SimpleViewEditor_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (weldView.FocusedRowHandle < 0)
            {
                return;
            }
            var row = weldView.GetDataRow(weldView.FocusedRowHandle);
            _doNotHandleCellValueChanged = true;
            weldView.BeginSort();

            var rowHandle = weldView.FocusedRowHandle;
            if (_indxColumn != null)
            {
                var index = (int) weldView.GetRowCellValue(weldView.FocusedRowHandle, _indxColumn);
                var items = Table.AsEnumerable().Where(x => (int) x[_indxColumn.FieldName]> index).OrderBy(x => (int)x[_indxColumn.FieldName]).ToList();
                foreach (DataRow dataRow in items)
                {
                    dataRow[_indxColumn.FieldName] = index++;
                }
                if (!items.Any())
                {
                    --rowHandle;
                }
            }
            row.Table.Rows.Remove(row);
            
            weldView.EndSort();
            _doNotHandleCellValueChanged = false;
            weldView.FocusedRowHandle = rowHandle;
        }

        private GridColumn _deleteColumn;
        private GridColumn _indxColumn;

        public Type MemberType { get; private set; }

        public void BindData(TableBase table)
        {
            if (MemberType == null)
            {
                weldView.AddColumns(string.Empty, table.MemberType, Visibles.Visible);


                weldView.AddDeleteColumn().ButtonClick += SimpleViewEditor_ButtonClick;
                if (!_allowDelete)
                {
                    _deleteColumn = weldView.Columns.Last();
                    _deleteColumn.Visible = false;
                }

                _indxColumn = weldView.Columns.ColumnByFieldName(IndexColumn);
                if (_indxColumn != null)
                {
                    _indxColumn.OptionsColumn.ReadOnly = true;
                }


                weldView.InitNewRow += WeldView_InitNewRow;
                MemberType = table.MemberType;

            }
            else if(table.MemberType != MemberType)
            {
                throw new Exception("Member Type not matched!");
            }
            if (Table != null)
            {
                weldView.FocusedRowChanged -= WeldView_FocusedRowChanged;
            }

            Table = table;
            weldView.BeginSort();
            gridControl1.DataSource = table;
            weldView.EndSort();
            weldView.FocusedRowChanged += WeldView_FocusedRowChanged;
            weldView.CellValueChanged += WeldView_CellValueChanged;
        }

        private bool _doNotHandleCellValueChanged = false;
        private void WeldView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (_doNotHandleCellValueChanged || RowCellValueChangingPost == null || e.RowHandle < 0)
            {
                return;
            }
            var v = this.gridControl1.FocusedView as GridView;
            if (v == null)
            {
                return;
            }

          
            var obj = v.ToObject(e.RowHandle, Table.MemberType);

            var evt = new RowCellValueChangingPostHandleEventArgs()
            {
                View = v,
                Column = e.Column,
                RowHandle = e.RowHandle,
                RowObject = obj,
            };
            RowCellValueChangingPost(v, evt);


            _doNotHandleCellValueChanged = true;
            foreach (var prpty in Table.MemberType.GetProperties().Where(x => !x.GetIndexParameters().Any()))
            {
                GridColumn col;
                var cellValue = prpty.GetValue(obj, null) ?? DBNull.Value;
                if ((col = v.Columns.ColumnByFieldName(prpty.Name)) == null
                    || e.Column == col
                    || v.GetRowCellValue(e.RowHandle, col).Equals(cellValue))
                {
                    continue;
                }
                try
                {
                    v.SetRowCellValue(e.RowHandle, col, cellValue);
                }
                catch (Exception)
                {

                }
            }

            _doNotHandleCellValueChanged = false;
        }

        private void WeldView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateWeldViewAllowFocusColumns();
        }


        private TableBase Table { get; set; }


        public bool NewRowCopyFromPreviousRow { get; set; }

        private void WeldView_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            _doNotHandleCellValueChanged = true;
            var retValue = Activator.CreateInstance(Table.MemberType);
            
            if (Table.Rows.Count >0 && NewRowCopyFromPreviousRow)
            {
                var old = Table.Rows[Table.Rows.Count - 1];
                var row = Table.NewRow();

                var imageTYpe = typeof (Bitmap);
                foreach (DataColumn column in Table.Columns)
                {
                    if (column.DataType == imageTYpe)
                    {
                        continue;
                    }
                    if ("Hash".Equals(column.ColumnName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    
                    row[column] = old[column];
                }
                retValue.LoadFieldsFromOther(row.ToObject());

            }
            
            if (NewRowAddingEvent != null)
            {
                var evt = new RowCellValueChangingPostHandleEventArgs()
                {
                    View = weldView,
                    Column = weldView.FocusedColumn,
                    RowHandle = e.RowHandle,
                    RowObject = retValue,
                };
                NewRowAddingEvent(weldView, evt);
            }


            foreach (var prpty in Table.MemberType.GetProperties().Where(x => !x.GetIndexParameters().Any()))
            {
                GridColumn col;
                var cellValue = prpty.GetValue(retValue, null);
                if ((col = weldView.Columns.ColumnByFieldName(prpty.Name)) == null
                    || weldView.FocusedColumn == col
                    || cellValue == null)
                {
                    continue;
                }
                try
                {
                    weldView.SetRowCellValue(weldView.FocusedRowHandle, col, cellValue);
                }
                catch (Exception)
                {

                }
            }

            if (_indxColumn != null)
            {
                weldView.SetRowCellValue(weldView.FocusedRowHandle, _indxColumn, Table.Rows.Count + 1 + MinimumIndex);
            }

            _doNotHandleCellValueChanged = false;
            UpdateWeldViewAllowFocusColumns();
        }

        public string InitRowFirstEnterColumn { get; set; }

        private string _indexColumnF;
        public string IndexColumn {
            get { return _indexColumnF; }
            set
            {
                _indexColumnF = value;

                _indxColumn = weldView.Columns.ColumnByFieldName(_indexColumnF);
                if (_indxColumn != null)
                {
                    _indxColumn.OptionsColumn.ReadOnly = true;
                }
            }
        }

        public int MinimumIndex { get; set; }

        private void UpdateWeldViewAllowFocusColumns()
        {
            if (string.IsNullOrWhiteSpace(InitRowFirstEnterColumn))
            {
                return;
            }
            foreach (GridColumn col in weldView.Columns)
            {
                col.OptionsColumn.AllowFocus = weldView.FocusedRowHandle >= 0 || InitRowFirstEnterColumn.Equals(col.FieldName);
            }
        }

    }
}
