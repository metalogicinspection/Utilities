using System;
using System.ComponentModel;
using System.Data;
using Gen3.Data;
using Metalogic.DataUtil;
using Metaphase.Editors;

namespace Metalogic.UI.Editors
{
    [ToolboxItem(true)] 
    public class EnovaDateEdit : DevExpress.XtraEditors.DateEdit, IEnovaEdit
    {
        public EnovaDateEdit()
        {
            Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            Properties.Mask.EditMask = "yyyy-MM-dd HH:mm:ss";
            Properties.Mask.UseMaskAsDisplayFormat = true;
            Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
            Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;

            PropertiesChanged += HandlePropertyChanged;

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                CustomDisplayText += EnovaDateEdit_CustomDisplayText;
            }
        }

        private void EnovaDateEdit_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            e.DisplayText = string.Concat(QueryTableName, ".", QueryFiledName);
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

        private void HandlePropertyChanged(object sender, EventArgs e)
        {
            try
            {
                Properties.Buttons[0].Enabled = !Properties.ReadOnly;

            }
            catch (Exception)
            {
                
            }
        }

        public bool UseParentBinding { get; set; }

        public void WriteValueIntoDataSet(DataSet set)
        {
            var table = set.Tables[QueryTableName];
            DataColumn column;
            if (table == null
                || table.Rows.Count < 1
                || (column = table.Columns[QueryFiledName]) == null
                || column.DataType != typeof(DateTime))
            {
                return;
            }

            table.Rows[0][column] = EditValue ?? DBNull.Value;

            DataSet = set as MCDataSet;
        }

        public void ReadValueFromDataSet(DataSet set)
        {
            var table = set.Tables[QueryTableName];
            DataColumn column;
            if (table == null
                || table.Rows.Count < 1
                || (column = table.Columns[QueryFiledName]) == null
                || column.DataType != typeof(DateTime))
            {
                return;
            }

            var va = table.Rows[0][column];
            va = va.Equals(DBNull.Value) ? DBNull.Value : va;
            EditValue = va;

            DataSet = set as MCDataSet;
        }
        public void WriteValueIntoDataModel(DataModel model)
        {
            DataModelBindingHelper.WriteValueIntoDataModel(this, model);
        }

        public void ReadValueFromDataModel(DataModel model)
        {
            DataModelBindingHelper.ReadValueFromDataModel(this, model);
            Model = model;
        }

        public MCDataSet DataSet { get; private set; }

        public DataModel Model { get; private set; }
    }
}