using System;
using System.Drawing;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogic.DataUtil;
using Metalogic.UI.Editors;
using Metalogic.UI.Header;

namespace Metalogic.UI.EditorsInGrid
{
    public class PickListEditInGrid : RepositoryItemLookUpEdit
    {
        public PickListEditInGrid()
        {
            DisplayMember = "Code";
            NullText = string.Empty; DropdownColumnsModes = PicklistDisplayValueModes.Code;
            PopupFormSize = new Size(500, 400);
            CustomDisplayText += HandleCustomDisplayText;
            ShowHeader = false;
            QueryPopUp += HandleQueryPopUp;
        }


        public event EventHandler<PickListDropdownFilter> FilterDropdown;

        void HandleQueryPopUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
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
            (sender as LookUpEdit).Properties.DataSource = evt.FilteredItems;
        }


        private void HandleCustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            if (e.Value == null)
            {
                e.DisplayText = string.Empty;
                return;
            }
            var valueStr = e.Value as string;
            var item = valueStr != null 
                ? PicklistItem.GetPickListItem(PickListType, valueStr) 
                : e.Value as PicklistItem;
            
            if (item == null )
            {
                e.DisplayText = valueStr?? string.Empty;
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

        public PicklistDisplayValueModes DisplayValueMode { get; set; }

        private PicklistDisplayValueModes _DropdownColumnsModes = PicklistDisplayValueModes.Code;
        public PicklistDisplayValueModes DropdownColumnsModes
        {
            get { return _DropdownColumnsModes; }
            set
            {
                _DropdownColumnsModes = value;
                Columns.Clear();
                if (value.HasFlag(PicklistDisplayValueModes.Code))
                {
                    Columns.Add(new LookUpColumnInfo("Code"));
                }
                if (value.HasFlag(PicklistDisplayValueModes.ShortDescription))
                {
                    Columns.Add(new LookUpColumnInfo("Description"));
                }
                if (value.HasFlag(PicklistDisplayValueModes.Description))
                {
                    Columns.Add(new LookUpColumnInfo("Detail"));
                }
            }
        }


        private Type _pickListType;
        private PicklistItem[] _items;
        public Type PickListType
        {
            get { return _pickListType; }
            set
            {
                if (!typeof(PicklistItem).IsAssignableFrom(value))
                {
                    throw new Exception("Given type is not a PickListItem.");
                }
                _pickListType = value;
                _items = PicklistItem.GetPickListByType(_pickListType).Where(x => x.IsEnabled).ToArray();
                DataSource = _items;
            }
        }

    }
}
