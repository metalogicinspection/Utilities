using System;
using System.ComponentModel;
using System.Data;
using Gen3.Data;
using Metalogic.DataUtil;
using Metaphase.Editors;

namespace Metalogic.UI.Editors
{
    [ToolboxItem(true)] 
    public class EnovaMemoEdit : DevExpress.XtraEditors.MemoEdit, IEnovaEdit
    {
        public EnovaMemoEdit()
        {
            Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            
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