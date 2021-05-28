using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Metalogic.DataUtil;
using Metalogic.UI.EditorsInGrid;
using Metalogic.UI.Header.GridAttributeInfo;

namespace Metalogic.UI
{
    public static class Tools
    {
        public static int GetMasterRowHandle(this GridView detailGridView, GridView masterGridview, int relationIndex = 0)
        {
            var rowHandle = -1;
            for (var i = 0; i < masterGridview.RowCount; ++i)
            {
                if (masterGridview.GetMasterRowExpanded(i) &&
                    (masterGridview.GetDetailView(i, relationIndex) == detailGridView || masterGridview.GetDetailView(i, 1) == detailGridView || masterGridview.GetDetailView(i, 2) == detailGridView))
                {
                    rowHandle = i;
                }
            }
            return rowHandle;
        }

        public static RepositoryItemButtonEdit AddDeleteColumn(this GridView gridview)
        {
            var col = new GridColumn() {Caption = "Delete"};
            col.VisibleIndex = gridview.Columns.Max(x => x.VisibleIndex) + 1;
            var edit = new RepositoryItemButtonEdit();
            edit.Buttons[0].Kind = ButtonPredefines.Delete;
            edit.TextEditStyle = TextEditStyles.HideTextEditor;
            col.ColumnEdit = edit;
            gridview.Columns.Add(col);
            return edit;
        }

        public static RepositoryItemButtonEdit AddDetailsButton(this GridView gridview)
        {
            var col = new GridColumn
            {
                Caption = "Details",
                VisibleIndex = gridview.Columns.Max(x => x.VisibleIndex) + 1
            };
            var edit = new RepositoryItemButtonEdit();
            edit.Buttons[0].Kind = ButtonPredefines.Ellipsis;
            edit.TextEditStyle = TextEditStyles.HideTextEditor;
            col.ColumnEdit = edit;
            gridview.Columns.Add(col);
            return edit;
        }

        public static void AddColumns(this GridView gridview, string viewName, Type memberType, Visibles fieldAttributeNotFoundMode)
        {
            var cols = new List<GridColumn>();

            int i = 0;
            foreach (var prpty in memberType.GetProperties().OrderBy(x => x.Name))
            {
                if (!prpty.CanRead)
                {
                    continue;
                }
                
                GridColumnVisibleIndex indexPrpty;
                var visibleIndex = (indexPrpty = prpty.GetAttribute<GridColumnVisibleIndex>(viewName)) != null
                    ? indexPrpty.VisibleIndex
                    : 100 + i++;

                var col = new GridColumn
                {
                    Caption = prpty.Name.SplitByUpperCase(),
                    Visible = visibleIndex >= 0,
                    VisibleIndex = visibleIndex,
                    FieldName = prpty.Name,
                    FilterMode = ColumnFilterMode.DisplayText,
                    SortMode = prpty.PropertyType.IsSubclassOf(typeof(PicklistItem)) 
                                ? ColumnSortMode.Custom : ColumnSortMode.Value
                };


                var vTitle = prpty.GetAttribute<GridColumnTitle>(viewName);
                if (vTitle != null)
                {
                    col.Caption = vTitle.Title;
                }

                var vwidth = prpty.GetAttribute<GridColumnWidth>(viewName);
                if (vwidth != null)
                {
                    col.Width = vwidth.Width;
                }

                var vReadOnly = prpty.GetAttribute<GridColumnReadonly>(viewName);
                if (!prpty.CanWrite ||  prpty.PropertyType == typeof (Guid))
                {
                    col.OptionsColumn.ReadOnly = true;
                }
                else if (vReadOnly != null)
                {
                    col.OptionsColumn.ReadOnly = vReadOnly.ReadOnly;
                }


                if (prpty.PropertyType.IsSubclassOf(typeof(PicklistItem)) && col.ColumnEdit == null)
                {
                    if (col.OptionsColumn.ReadOnly)
                    {
                        col.ColumnEdit = new PickListReadOnlyInGrid { PickListType = prpty.PropertyType };
                    }
                    else
                    {
                        col.ColumnEdit = new PickListEditInGrid() { PickListType = prpty.PropertyType, ValueMember = null };
                    }
                }

                cols.Add(col);
            }

            var ary = cols.OrderBy(x => x.VisibleIndex).ToArray();

            gridview.Columns.AddRange(ary);
            try
            {
                gridview.BestFitColumns();
            }
            catch (Exception)
            {
                
            }
        }
        
    }
}
