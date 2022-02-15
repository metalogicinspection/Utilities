using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Gen3.Data;
using Metalogic.UI.Header.GridAttributeInfo;
using DevExpress.XtraGrid.Columns;
using Metalogic.UI;

namespace Gen3.UI.Grids
{
    public partial class MasterDetailGridControl : UserControl
    {
        public delegate void RowCellValueChangingPostEventHandler(object sender, RowCellValueChangingPostHandleEventArgs e);

        public event RowCellValueChangingPostEventHandler RowCellValueChangingPost;

        public event RowCellValueChangingPostEventHandler NewRowAddingEvent;

        public event RowCellValueChangingPostEventHandler RowDeletingEvent;

        public MasterDetailGridControl()
        {
            InitializeComponent();
            
            Settings = new List<MasterDetailViewSetting>();
           

        }

        public GridView GetView(Type memberType)
        {
            var masterDetailViewSetting = Settings.FirstOrDefault(x => x.MemberType == memberType);
            return masterDetailViewSetting?.View;
        }

        public List<MasterDetailViewSetting> Settings { get; private set; } 

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

        public GridView MainView => weldView;

        public void ExportToXlsx(string filePath)
        {
            gridControl1.ExportToXlsx(filePath);
        }

        public GridView FocusedView => gridControl1.FocusedView as GridView;

        private void SimpleViewEditor_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            var v = this.gridControl1.FocusedView as GridView;
            if (v == null)
            {
                return;
            }


            var rowHandle = v.FocusedRowHandle;
            var index = v.GetDataSourceRowIndex(rowHandle);
            if (index < 0)
            {
                return;
            }
            
            var dataSource = v.DataSource as IBindingListWithMemberType;
            if (dataSource == null)
            {
                return;
            }


            var setting = Settings.First(x => x.MemberType == dataSource.MemberType);
            gridControl1.BeginUpdate();

            var curFocusedItem = dataSource[index];

            if (setting.IndexPropertyInfo != null && dataSource.Count > 1)
            {
                for (var i = index + 1; i < dataSource.Count; ++i)
                {
                    setting.IndexPropertyInfo.SetValue(dataSource[i], i);
                }
            }

            foreach (var item in setting.DataList.FindAllOffsprings(curFocusedItem).Reverse())
            {
                item.DataList.Remove(item.Item);
            }

            var evt = new RowCellValueChangingPostHandleEventArgs()
            {
                View = v,
                Column = null,
                RowHandle = rowHandle,
                RowObject = curFocusedItem,
                NewRow = false,
            };

            RowDeletingEvent?.Invoke(this, evt);
            dataSource.Remove(curFocusedItem);
            

            gridControl1.EndUpdate();


            if (setting.DataList.Count > 1)
            {
                v.FocusedRowHandle = rowHandle;
            }

        }
        

        public void BindData(IGen3DataList primaryList)
        {
            AddView(primaryList, null);
            var v = gridControl1.FocusedView as GridView;
            if (v == null)
            {
                return;
            }
            v.BeginSort();
            gridControl1.DataSource = primaryList;
            Table = primaryList;
            v.EndSort();
        }

        private void AddView(IGen3DataList table, DRelation parentRelaiton)
        {
            var setting = Settings.First(x => x.MemberType == table.MemberType);
            setting.View = parentRelaiton == null? weldView : new GridView();
            setting.View.OptionsCustomization.AllowGroup = false;
            setting.View.OptionsDetail.AllowExpandEmptyDetails = true;
            setting.View.OptionsView.ColumnAutoWidth = false;
            setting.View.OptionsView.RowAutoHeight = true;
            setting.View.OptionsView.ShowGroupPanel = false;
            setting.View.Name = table.MemberType.FullName;

            setting.View.OptionsView.NewItemRowPosition = setting.AllowAddNewRow
                ? NewItemRowPosition.Bottom
                : NewItemRowPosition.None;

            setting.View.AddColumns(setting.ColumnsViewName, table.MemberType, Visibles.Visible);
            if (setting.AllowDetailButton && table.Relations.Length > 0)
            {
                setting.View.AddDetailsButton().ButtonClick += MasterDetailViewEditor_ButtonClick;
            }


            var indexColumn = setting.View.Columns.ColumnByFieldName(setting.IndexColumnName);
            if (indexColumn != null)
            {
                indexColumn.OptionsColumn.ReadOnly = true;
            }
            setting.IndexPropertyInfo = table.MemberType.GetProperties()
             .FirstOrDefault(x => x.CanWrite &&
             x.CanRead &&
             x.PropertyType == typeof(int) &&
             x.Name.Equals(indexColumn?.FieldName));
            setting.IndexColumn = setting.IndexPropertyInfo!= null ? indexColumn : null;

            setting.HashPropertyInfo = table.MemberType.GetProperties()
                        .FirstOrDefault(x => x.CanWrite &&
                        x.CanRead &&
                        x.PropertyType == typeof(Guid) &&
                        x.Name.Equals("Hash", StringComparison.InvariantCultureIgnoreCase));
            setting.HashColumn = setting.HashPropertyInfo != null ? setting.View.Columns.ColumnByFieldName("Hash") : null;

            if (setting.HashColumn != null)
            {
                setting.HashColumn.OptionsColumn.ReadOnly = true;
                setting.HashColumn.VisibleIndex = -1;
            }

            if (setting.AllowDelete)
            {
                setting.View.AddDeleteColumn().ButtonClick += SimpleViewEditor_ButtonClick;
                setting.DeleteColumn = setting.View.Columns.Last();
                setting.DeleteColumn.VisibleIndex = setting.View.Columns.Max(x => x.VisibleIndex) + 1;
            }

            setting.View.CustomDrawCell += WeldView_CustomDrawCell;
            setting.View.InitNewRow += WeldView_InitNewRow;
            if (!string.IsNullOrWhiteSpace(setting.InitRowFirstEnterColumnName))
            {
                setting.View.FocusedRowChanged += WeldView_FocusedRowChanged;
                setting.View.MasterRowExpanded += WeldView_MasterRowExpanded;
            }

            setting.View.CellValueChanging += View_CellValueChanging; ;
            setting.View.CellValueChanged += WeldView_CellValueChanged;
            setting.DataList = table;

            if (setting.View != weldView)
            {
                gridControl1.ViewCollection.Add(setting.View);

                var gridLevelNode1 = new GridLevelNode
                {
                    LevelTemplate = setting.View,
                    RelationName = parentRelaiton == null ? string.Empty : parentRelaiton.RelationName
                };
                gridControl1.LevelTree.Nodes.Add(gridLevelNode1);

            }
            

            foreach (var relation in table.Relations)
            {
                AddView(relation.ChildList, relation);
            }
        }

        private bool _skipEditValuePost = false;

        public bool PauseRowCellValueChangingPost { get; set; } = false;
        private void View_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (RowCellValueChangingPost == null)
            {
                return;
            }

            if (PauseRowCellValueChangingPost)
            {
                return;
            }
            var v = (sender as GridView)??gridControl1.FocusedView as GridView;
            if (v == null)
            {
                Console.WriteLine("View not found");
                return;
            }
            if (e.RowHandle < 0)
            {
                Console.WriteLine("Row is empty");
                return;
            }
            var setting = Settings.FirstOrDefault(x => x.View.Name == v.Name);
            if (setting == null)
            {
                Console.WriteLine("setting not found");
                return;
            }

            var prpty = setting.MemberType.GetProperty(e.Column.FieldName);
            if (prpty == null){

                Console.WriteLine("property not found");
                return;
            }

            if(prpty.PropertyType ==  typeof(string))
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
                Console.WriteLine("focused row not found");
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
                        _skipEditValuePost = false;
                    }));
                }
                catch (Exception)
                {
                }
               
            }).Start();
        }

        private void MasterDetailViewEditor_ButtonClick(object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            var focusedView = gridControl1.FocusedView as GridView;
            if (focusedView == null)
            {
                return;
            }
            if (focusedView.FocusedRowHandle < 0)
            {
                return;
            }
            if (focusedView.GetMasterRowExpanded(focusedView.FocusedRowHandle))
            {
                focusedView.CollapseMasterRow(focusedView.FocusedRowHandle);
                return;
            }
            focusedView.ExpandMasterRow(focusedView.FocusedRowHandle);

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
            var v = (sender as GridView) ?? gridControl1.FocusedView as GridView;
            if (v == null)
            {
                return;
            }

            var setting = Settings.FirstOrDefault(x => x.View.Name == v.Name);
            if (setting == null)
            {
                return;
            }

            var dataSource = v.DataSource as IBindingList;
            if (dataSource == null)
            {
                return;
            }


            var obj = v.GetFocusedRow();
            
            var evt = new RowCellValueChangingPostHandleEventArgs()
            {
                View = v,Column = e.Column, RowHandle = e.RowHandle, RowObject = obj, NewRow = e.RowHandle < 0
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
        
        
        
        private IGen3DataList Table { get; set; }

        private void WeldView_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            var v = gridControl1.FocusedView as GridView;
            if (v == null)
            {
                return;
            }
            var setting = Settings.FirstOrDefault(x => x.View.Name == v.Name);
            if (setting == null)
            {
                return;
            }

            var dataSource = v.DataSource as IBindingListWithMemberType;
            if (dataSource == null)
            {
                return;
            }

            var newObj = dataSource[dataSource.Count - 1];


            setting.IndexPropertyInfo?.SetValue(newObj, dataSource.Count);

            
            if (dataSource.Count > 1 && setting.NewRowCopyFromPreviousRow)
            {
                var hashPrpty = dataSource.MemberType.GetProperties()
                         .FirstOrDefault(x => x.CanWrite &&
                         x.CanRead &&
                         x.PropertyType == typeof(Guid) &&
                         x.Name.Equals("Hash", StringComparison.InvariantCultureIgnoreCase));


                var preObj = dataSource[dataSource.Count - 2];
                var properties = dataSource.MemberType.GetProperties()
                    .Where(x => x.CanWrite &&
                                x.CanRead &&
                                x.PropertyType != typeof(Bitmap) &&
                                x!= setting.IndexPropertyInfo &&
                                !x.Name.Equals(v.FocusedColumn.FieldName) &&
                                x != setting.HashPropertyInfo && 
                                x!= hashPrpty)
                    .ToList();

                foreach (var curPrpty in properties)
                {
                    var val = curPrpty.GetValue(preObj);
                    curPrpty.SetValue(newObj, val);
                }
            }

            if (Guid.Empty.Equals(setting.HashPropertyInfo?.GetValue(newObj)))
            {
                setting.HashPropertyInfo?.SetValue(newObj, Guid.NewGuid());
            }
           
            if (NewRowAddingEvent != null)
            {
                var evt = new RowCellValueChangingPostHandleEventArgs()
                {
                    View = v,
                    Column = v.FocusedColumn,
                    RowHandle = e.RowHandle,
                    RowObject = newObj,
                    NewRow = true,
                    DataList = setting.DataList
                };
                NewRowAddingEvent(weldView, evt);
            }
            if (!string.IsNullOrWhiteSpace(setting.InitRowFirstEnterColumnName))
            {
                UpdateWeldViewAllowFocusColumns(v);
            }
        }

        private void WeldView_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            UpdateWeldViewAllowFocusColumns(weldView.GetDetailView(e.RowHandle, e.RelationIndex) as GridView);
        }

        private void WeldView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateWeldViewAllowFocusColumns(sender as GridView);
        }


        public void BeginUpdate() => gridControl1.BeginUpdate();
        public void EndUpdate() => gridControl1.EndUpdate();

        //public  int GetMasterRowHandle(GridView curDefectView, out int dataRelation)
        //{
        //    var curDetailTable = curDefectView.DataSource as IItemSpecificChildrenList;
        //    var i = 0;
        //    for (; i < curDetailTable.DataSet.Relations.Count; ++i)
        //    {
        //        if (curDetailTable.DataSet.Relations[i].ChildTable == curDetailTable)
        //        {
        //            break;
        //        }
        //    }
        //    dataRelation = i;
        //    for (i = 0; i < weldView.RowCount; ++i)
        //    {
        //        if (weldView.GetMasterRowExpanded(i))
        //        {
        //            for (int k = 0; k < 4; ++k)
        //            {
        //                if (weldView.GetDetailView(i, k) == curDefectView)
        //                {
        //                    return i;
        //                }
        //            }
        //        }

        //    }
        //    dataRelation = -1;
        //    return -1;
        //}
        

        private void UpdateWeldViewAllowFocusColumns(GridView v)
        {
            if (v == null)
            {
                return;
            }
            var setting = Settings.FirstOrDefault(x => v.Name.Equals(x.View?.Name));
            if (string.IsNullOrWhiteSpace(setting?.InitRowFirstEnterColumnName))
            {
                return;
            }
            foreach (GridColumn col in v.Columns)
            {
                col.OptionsColumn.AllowFocus = v.FocusedRowHandle >= 0 || setting.InitRowFirstEnterColumnName.Equals(col.FieldName);
            }
        }

        public void RefreshData()
        {
            foreach (var view in gridControl1.ViewCollection.OfType<GridView>())
            {
                view.BeginUpdate();
                
                view.EndUpdate();
            }
        }

    }
}
