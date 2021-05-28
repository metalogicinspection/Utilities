using System;
using System.ComponentModel;
using System.Data;
using Gen3.Data;
using Metalogic.DataUtil;
using Metaphase.Editors;

namespace Metalogic.UI.Editors
{
    [ToolboxItem(true)] 
    public class EnovaPictureEdit : DevExpress.XtraEditors.PictureEdit, IEnovaEdit
    {
        public EnovaPictureEdit()
        {
            ParentChanged += HandleParentChanged;
            
        }

        private void HandleParentChanged(object sender, EventArgs e)
        {
            this.SetCaption();
        }
        
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
                if (DesignMode)
                    Text = string.Concat(QueryTableName, ".", QueryFiledName);
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
                if (DesignMode)
                Text = string.Concat(QueryTableName, ".", QueryFiledName);
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
    }
}