using System;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogic.DataUtil;
using Metalogic.UI.Header;

namespace Metalogic.UI.EditorsInGrid
{
    public class PickListReadOnlyInGrid : RepositoryItemTextEdit
    {
        public PickListReadOnlyInGrid()
        {
            DisplayValueMode = PicklistDisplayValueModes.Code;
            CustomDisplayText += HandleCustomDisplayText;
        }

        private void HandleCustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            var item = e.Value as PicklistItem;
            if (item == null && e.Value != null)
            {
                item = PicklistItem.GetPickListItem(PickListType, e.Value.ToString());
            }

            if (item == null)
            {
                e.DisplayText = string.Empty;
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

        public PicklistDisplayValueModes DropDownColumns { get; set; }
        
        private Type _pickListType;
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
            }
        }
    }
}