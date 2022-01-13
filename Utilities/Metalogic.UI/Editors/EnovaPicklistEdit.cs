using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using DevExpress.XtraGrid.Columns;
using Gen3.Data;
using Metalogic.DataUtil;
using Metalogic.UI.EditorsInGrid;
using Metalogic.UI.Header.GridAttributeInfo;
using Metaphase.Editors;

namespace Metalogic.UI.Editors
{
    public class PickListDropdownFilter : EventArgs
    {
        public PicklistItem[] OriginalItems { get; set; }

        public List<PicklistItem> FilteredItems { get; set; }

        public Type PicklistType { get; set; }
    }

    [ToolboxItem(true)]
    public sealed class EnovaPicklistEdit : DevExpress.XtraEditors.GridLookUpEdit, IEnovaEdit{
        public EnovaPicklistEdit()
        {
            Properties.DisplayMember = "Code";
            Properties.ValueMember = null;
            Properties.NullText = string.Empty;
            DisplayValueMode = PicklistDisplayValueModes.Code | PicklistDisplayValueModes.ShortDescription;

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            QueryPopUp += HandleQueryPopUp;
            Popup+= HandlePopup;
            CustomDisplayText += HandleCustomDisplayText;
        }

        void HandleQueryPopUp(object sender, CancelEventArgs e)
        {
            var index = 101;
            foreach (var additionalDropDownColumn in AdditionalDropDownColumns)
            {
                Properties.View.Columns.Add(new GridColumn()
                {
                    FieldName = additionalDropDownColumn,
                    Caption = additionalDropDownColumn.SplitByUpperCase(),
                    VisibleIndex = ++index
                });
            }
            foreach (GridColumn col in Properties.View.Columns)
            {
                
                if (_columnFieldToViewIndex.ContainsKey(col.FieldName))
                {
                    col.VisibleIndex = _columnFieldToViewIndex[col.FieldName];
                }

                if (AdditionalDropDownColumns.Contains(col.FieldName))
                {
                    col.VisibleIndex = ++index;
                }
                else if (col.VisibleIndex < 0)
                {

                }
                else if (col.FieldName == "Code")
                {
                    col.Visible = Test(DisplayValueMode, PicklistDisplayValueModes.Code);
                }
                else if (col.FieldName == "ShortDescription")
                {
                    col.Visible = Test(DisplayValueMode, PicklistDisplayValueModes.ShortDescription);
                }
                else if (col.FieldName == "Description")
                {
                    col.Visible = Test(DisplayValueMode, PicklistDisplayValueModes.Description);
                }
                else if (col.FieldName == "IsEnabled")
                {
                    col.Visible = false;
                }
                else
                {
                    col.Visible = Test(DisplayValueMode, PicklistDisplayValueModes.CustomFields);
                }
            }

            //var width = 0;
            //foreach (GridColumn source in Properties.View.Columns.Where(x => x.VisibleIndex > 0))
            //{
            //    width += source.VisibleWidth
            //}

            Properties.View.OptionsView.ColumnAutoWidth = false;
            Properties.View.BestFitColumns();
            Properties.View.OptionsView.ShowColumnHeaders = Properties.View.Columns.Count(col => col.Visible) > 1;
            if (FilterDropdown == null)
            {
                return;
            }

            var evt = new PickListDropdownFilter { PicklistType = PickListType, OriginalItems = _items };
            FilterDropdown(this, evt);

            if (evt.FilteredItems == null)
            {
                return;
            }

            Properties.DataSource = evt.FilteredItems;
        }
        private void HandleCustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            Properties.ValueMember = null;
            var item = e.Value as PicklistItem;
            string str;
            if (item == null &&  (str = e.Value as string)!= null)
            {
                item = PicklistItem.GetPickListItem(PickListType, str);
            }

            if (item == null)
            {
                e.DisplayText = e.Value?.ToString() ?? string.Empty;
                return;
            }
            
            switch (DisplayValueMode)
            {
                case PicklistDisplayValueModes.Description:
                    e.DisplayText = item.Description;
                    break;
                case PicklistDisplayValueModes.ShortDescription:
                    e.DisplayText = item.ShortDescription;
                    break;
                default:
                    e.DisplayText = item.Code;
                    break;
            }
        }

        public event EventHandler<PickListDropdownFilter> FilterDropdown;

        private PicklistItem[] _items;
        private void HandlePopup(object sender, EventArgs e)
        {
            Properties.View.GridControl.Parent.Width = 573;
        }

        private static bool Test(PicklistDisplayValueModes testValue, PicklistDisplayValueModes target)
        {
            return (testValue & target) == target;
        }

        public PicklistDisplayValueModes DisplayValueMode { get; set; }


        private string _queryTableName = string.Empty;
        public string QueryTableName
        {
            get
            {
                return _queryTableName;
            }
            set
            {
                _queryTableName = value.Trim();
                if (DesignMode) Text = string.Concat(QueryTableName, ".", QueryFiledName);
            }
        }

        private string _queryFiledName = string.Empty;
        public string QueryFiledName
        {
            get
            {
                return _queryFiledName;
            }
            set
            {
                _queryFiledName = value.Trim();
                if (DesignMode) Text = string.Concat(QueryTableName, ".", QueryFiledName);
            }
        }
        
        private readonly Dictionary<string, int> _columnFieldToViewIndex = new Dictionary<string, int>();

        private Type _pickListType;
        public Type PickListType {
            get { return _pickListType; }
            set
            {
                if (!typeof (PicklistItem).IsAssignableFrom(value))
                {
                    throw new Exception("Given type is not a PickListItem.");
                }
                _pickListType = value;



                int i = 0;
                foreach (var prpty in _pickListType.GetProperties().OrderBy(x => x.Name))
                {
                    if (!prpty.CanRead)
                    {
                        continue;
                    }
                    
                    GridColumnVisibleIndex indexPrpty;
                    var visibleIndex =
                        (indexPrpty = prpty.GetAttribute<GridColumnVisibleIndex>(string.Empty)) != null
                        ? indexPrpty.VisibleIndex
                        : 100 + i++;

                    //var vwidth = prpty.GetAttribute<GridColumnWidth>(viewName);
                    //if (vwidth != null)
                    //{
                    //    col.Width = vwidth.Width;
                    //}
                    _columnFieldToViewIndex[prpty.Name]= visibleIndex;
                }
                _columnFieldToViewIndex["Code"] = 0;
                _columnFieldToViewIndex["ShortDescription"] = 1;

                var items = PicklistItem.GetPickListByType(_pickListType).ToArray();
                
                Properties.DataSource = items.Where(x => x.IsEnabled).ToArray();
                _items = items;
            }
        }
        

        private string _caption = string.Empty;
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value.Trim();

                this.SetCaption();
            }
        }
        

        public bool UseParentBinding => false;
        
        public void WriteValueIntoDataModel(DataModel model)
        {
            DataModelBindingHelper.WriteValueIntoDataModel(this, model);
        }

        public void ReadValueFromDataModel(DataModel model)
        {
            DataModelBindingHelper.ReadValueFromDataModel(this, model);
            Model = model;
        }

        public DataModel Model { get; private set; }

        public List<string> AdditionalDropDownColumns = new List<string>();
    }

}
