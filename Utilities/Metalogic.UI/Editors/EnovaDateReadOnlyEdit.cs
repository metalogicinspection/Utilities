using System;
using System.Data;
using Gen3.Data;
using Metalogic.DataUtil;
using Metaphase.Editors;

namespace Metalogic.UI.Editors
{
    public class EnovaDateReadOnlyEdit : DevExpress.XtraEditors.TextEdit, IEnovaEdit
    {
        public EnovaDateReadOnlyEdit()
        {
            Properties.ReadOnly = true;
            
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