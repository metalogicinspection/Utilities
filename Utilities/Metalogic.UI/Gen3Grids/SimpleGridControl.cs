using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Gen3.Data;
using Metalogic.UI.Header.GridAttributeInfo;
using System.Threading;
using Metalogic.UI;

namespace Gen3.UI.Grids
{

    [ToolboxItem(true)]
    public partial class SimpleGridControl : UserControl
    {
        public delegate void RowCellValueChangingPostEventHandler(object sender, RowCellValueChangingPostHandleEventArgs e);

        public event RowCellValueChangingPostEventHandler RowCellValueChangingPost;


        public event RowCellValueChangingPostEventHandler NewRowAddingEvent;

        public SimpleGridControl()
        {
            InitializeComponent();
            
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
            var rowHandle = weldView.FocusedRowHandle;
            var index = weldView.GetDataSourceRowIndex(weldView.FocusedRowHandle);
            if (index < 0)
            {
                return;
            }

            _doNotHandleCellValueChanged = true;
            weldView.BeginSort();
            
            if (_indexPropertyInfo != null && Table.Count > 1)
            {
                for (var i = index + 1; i < Table.Count; ++i)
                {
                    _indexPropertyInfo.SetValue(Table[i], i  + MinimumIndex);
                }
            }
            Table.Remove(Table[index]);

            weldView.EndSort();
            _doNotHandleCellValueChanged = false;

            if (Table.Count > 1)
            {
                weldView.FocusedRowHandle = rowHandle;
            }
        }

        private GridColumn _deleteColumn;
        private GridColumn _indxColumn;
        private PropertyInfo _indexPropertyInfo;
        

        public void BindData(IGen3DataList table)
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
            _indexPropertyInfo = table.MemberType.GetProperties()
             .FirstOrDefault(x => x.CanWrite &&
             x.CanRead &&
             x.PropertyType == typeof(int) &&
             x.Name.Equals(_indxColumn?.FieldName));

            weldView.InitNewRow += WeldView_InitNewRow;
            

            Table = table;
            weldView.BeginSort();
            gridControl1.DataSource = table;
            weldView.EndSort();
            weldView.CellValueChanged += WeldView_CellValueChanged;
            weldView.CellValueChanging += View_CellValueChanging;
        }

        private bool _skipEditValuePost = false;
        private void View_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (RowCellValueChangingPost == null)
            {
                return;
            }
            var v = gridControl1.FocusedView as GridView;
            if (v == null)
            {
                return;
            }
            if (e.RowHandle < 0)
            {
                return;
            }

            var prpty = Table.MemberType.GetProperty(e.Column.FieldName);
            if (prpty == null)
            {
                return;
            }

            if (prpty.PropertyType == typeof(string))
            {
                return;
            }



            var dataSource = v.DataSource as IBindingList;
            if (dataSource == null)
            {
                return;
            }

            var obj = v.GetFocusedRow();
            if (obj == null)
            {
                return;
            }

            try
            {
                prpty.SetValue(obj, e.Value);
            }
            catch (Exception)
            {
                return;
            }
            _skipEditValuePost = true;


            var evt = new RowCellValueChangingPostHandleEventArgs()
            {
                View = v,
                Column = e.Column,
                RowHandle = e.RowHandle,
                RowObject = obj,
                NewRow = e.RowHandle < 0
            };
            new Thread(() =>
            {
                try
                {
                    Thread.Sleep(new TimeSpan(0, 0, 0, 0, 100));
                    if (IsDisposed)
                    {
                        return;
                    }
                    BeginInvoke(new Action(() =>
                    {
                        gridControl1.BeginUpdate();
                        RowCellValueChangingPost(v, evt);
                        gridControl1.EndUpdate();
                    }));
                }
                catch (Exception)
                {
                }
            }).Start();
        }

        private void WeldView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (_skipEditValuePost)
            {
                _skipEditValuePost = false;
                return;
            }
            if (RowCellValueChangingPost == null)
            {
                return;
            }
            var v = gridControl1.FocusedView as GridView;


            var dataSource = v?.DataSource as IBindingList;
            if (dataSource == null)
            {
                return;
            }

            var obj = v.GetFocusedRow();

            var evt = new RowCellValueChangingPostHandleEventArgs()
            {
                View = v,
                Column = e.Column,
                RowHandle = e.RowHandle,
                RowObject = obj,
                NewRow = e.RowHandle < 0
            };

            new Thread(() =>
            {
                try
                {
                    Thread.Sleep(new TimeSpan(0, 0, 0, 0, 100));
                    if (IsDisposed)
                    {
                        return;
                    }
                    BeginInvoke(new Action(() =>
                    {
                        gridControl1.BeginUpdate();
                        RowCellValueChangingPost(v, evt);
                        gridControl1.EndUpdate();
                    }));
                }
                catch (Exception)
                {
                }

            }).Start();

        }

        private bool _doNotHandleCellValueChanged = false;
       
        

        private IGen3DataList Table { get; set; }


        public bool NewRowCopyFromPreviousRow { get; set; }

        private void WeldView_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            _doNotHandleCellValueChanged = true;
            
            var newObj = Table[Table.Count - 1];
            

            _indexPropertyInfo?.SetValue(newObj, Table.Count + MinimumIndex);

            var hashPrpty = Table.MemberType.GetProperties()
                         .FirstOrDefault(x => x.CanWrite &&
                         x.CanRead &&
                         x.PropertyType == typeof(Guid) &&
                         x.Name.Equals("Hash", StringComparison.InvariantCultureIgnoreCase));

            if (Table.Count > 1 && NewRowCopyFromPreviousRow)
            {
                var preObj = Table[Table.Count - 2];
                var properties = Table.MemberType.GetProperties()
                    .Where(x => x.CanWrite &&
                                x.CanRead && 
                                x.PropertyType != typeof(Bitmap) &&
                                !x.Name.Equals(_indxColumn?.FieldName) &&
                                !x.Name.Equals(View.FocusedColumn.FieldName) && 
                                x != hashPrpty)
                    .ToList();

                foreach (var curPrpty in properties)
                {
                    var val = curPrpty.GetValue(preObj);
                    curPrpty.SetValue(newObj, val);
                }
            }

            if (Guid.Empty.Equals(hashPrpty?.GetValue(newObj)))
            {
                hashPrpty?.SetValue(newObj, Guid.NewGuid());
            }

            if (NewRowAddingEvent != null)
            {
                var evt = new RowCellValueChangingPostHandleEventArgs()
                {
                    View = weldView,
                    Column = weldView.FocusedColumn,
                    RowHandle = e.RowHandle,
                    RowObject = newObj,
                    NewRow = e.RowHandle < 0
                };
                NewRowAddingEvent(weldView, evt);
            }

            


            _doNotHandleCellValueChanged = false;
        }

        public string InitRowFirstEnterColumn { get; set; }

        private string _indexColumnF;
        public string IndexColumn {
            get { return _indexColumnF; }
            set
            {
                _indexColumnF = value;

                _indxColumn = weldView.Columns.ColumnByFieldName(_indexColumnF);

                _indexPropertyInfo = Table?.MemberType.GetProperties()
                             .FirstOrDefault(x => x.CanWrite &&
                             x.CanRead &&
                             x.PropertyType == typeof(int) &&
                             x.Name.Equals(_indxColumn?.FieldName));


                if (_indxColumn != null)
                {
                    _indxColumn.OptionsColumn.ReadOnly = true;
                }
            }
        }

        public int MinimumIndex { get; set; }
        

    }
}
