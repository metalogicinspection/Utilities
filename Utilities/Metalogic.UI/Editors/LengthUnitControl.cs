using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using Gen3.Data;
using Metalogic.DataUtil;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Utils;

namespace Metalogic.UI.Editors
{
    public class LengthUnitDropdownFilter : EventArgs
    {
        public LengthUnits[] OriginalItems { get; set; }

        public List<LengthUnits> FilteredItems { get; set; }
    }

    public partial class LengthUnitControl : UserControl, IDataBindingFromParentControl , IMetadataControl
    {
        private readonly LengthUnits[] _items;
        public LengthUnitControl()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            spinEdit1.EditValue = 0d;
            spinEdit1.EditValueChanged += SpinEdit1_EditValueChanged;

            _items = PicklistItem.GetPickList<LengthUnits>().ToArray();
            gridLookUpEdit1.Properties.DataSource = _items;

            gridLookUpEdit1.Properties.DisplayMember = "Code";

            gridLookUpEdit1.EditValue = LengthUnits.MM;

            this.spinEdit1.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.spinEdit1.Properties.Mask.EditMask = LengthUnits.MM.Mask;

            gridLookUpEdit1.EditValueChanged += GridLookUpEdit1_EditValueChanged;
            gridLookUpEdit1.QueryPopUp += GridLookUpEdit1_QueryPopUp;

        }

        private void GridLookUpEdit1_QueryPopUp(object sender, CancelEventArgs e)
        {
            foreach (GridColumn col in gridLookUpEdit1.Properties.View.Columns)
            {
                if (!col.FieldName.Equals("Code"))
                {
                    col.Visible = false;
                }
            }

            if (FilterDropdown == null)
            {
                return;
            }

            var evt = new LengthUnitDropdownFilter {OriginalItems = _items };
            FilterDropdown(this, evt);

            if (evt.FilteredItems == null)
            {
                return;
            }
            gridLookUpEdit1.Properties.DataSource = evt.FilteredItems;
        }

        private void GridLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            this.spinEdit1.Properties.Mask.EditMask = DisplayLengthUnit.Mask;
            UpdateSpinEditValue();

            EditValueChanged?.Invoke(this, new EventArgs { });
        }

        private void SpinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (_doNotHandle )
            {
                return;
            }
            
            _value = Convert.ToDouble(spinEdit1.EditValue) * DisplayLengthUnit.GetConversionFactor(ValueLengthUnit);

            EditValueChanged?.Invoke(this, new EventArgs {});
        }


        private LengthUnits _valueLengthUnit = LengthUnits.M;
        public LengthUnits ValueLengthUnit
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return null;
                }

                var t = new StackTrace();
                if (t.GetFrame(1).GetMethod().Name == "InitializeComponent")
                {
                    return null;
                }


                return _valueLengthUnit;
            }
            set
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return;
                }

                var t = new StackTrace();
                if (t.GetFrame(1).GetMethod().Name == "InitializeComponent")
                {
                    return;
                }

                if (value == null)
                {
                    return;
                }
                _doNotHandle = true;

                _value = _value*_valueLengthUnit.GetConversionFactor(value);
                _valueLengthUnit = value;

                _doNotHandle = false;
            }
        }

        
        public LengthUnits DisplayLengthUnit
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return null;
                }
                var t = new StackTrace();
                if (t.GetFrame(1).GetMethod().Name == "InitializeComponent")
                {
                    return null;
                }
                return gridLookUpEdit1?.EditValue as LengthUnits;
            }
            set
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return;
                }

                var t = new StackTrace();
                if (t.GetFrame(1).GetMethod().Name == "InitializeComponent")
                {
                    return;
                }

                if (value == null || DisplayLengthUnit == value)
                {
                    return;
                }

                _doNotHandle = true;

                gridLookUpEdit1.EditValue = value;
                this.spinEdit1.Properties.Mask.EditMask = DisplayLengthUnit.Mask;
                UpdateSpinEditValue();

                _doNotHandle = false;
            }
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

        private double _value = 0d;

        public object EditValue
        {
            get
            {
                return _value;
            }
            set
            {
                if (this.DesignMode)
                {
                    return;
                }

                var stackTrace = new StackTrace();
                var method = stackTrace.GetFrame(1).GetMethod();
                if (method.ReflectedType == null)
                {
                    return;
                }
                if ("InitializeComponent".Equals(method.Name))
                {
                    return;
                }
                var dValue = value as double?;
                var fValue = value as float?;
                if (dValue != null)
                {
                    _value = dValue.Value;
                }
                else if (fValue != null)
                {
                    _value = fValue.Value;
                }
                else
                {
                    return;}

                UpdateSpinEditValue();

            }
        }
        
        public void WriteValueIntoDataSet(DataSet set)
        {
            var table = set.Tables[QueryTableName];
            DataColumn column;
            if (table == null 
                || table.Rows.Count < 1 
                || (column = table.Columns[QueryFiledName]) == null
                || column.DataType != typeof(double))
            {
                return;
            }

            table.Rows[0][column] = _value;


            var lengthUnitColumn = table.Columns[QueryDisplayUnitFieldName];
            var unit = gridLookUpEdit1.EditValue as LengthUnits;
            if (lengthUnitColumn != null && unit != null)
            {
                table.Rows[0][lengthUnitColumn] = unit;
            }

            DataSet = set as MCDataSet;
        }

        public void ReadValueFromDataSet(DataSet set)
        {
            var table = set.Tables[QueryTableName];
            DataColumn column;
            if (table == null
                || table.Rows.Count < 1
                || (column = table.Columns[QueryFiledName]) == null
                || column.DataType != typeof(double))
            {
                return;
            }

            var va = table.Rows[0][column];
            if (va == DBNull.Value)
            {
                return;
            }

            var lengthUnitColumn = table.Columns[QueryDisplayUnitFieldName];
            LengthUnits unit;
            if (lengthUnitColumn != null
                && (unit = table.Rows[0][lengthUnitColumn] as LengthUnits) != null)
            {
                _doNotHandle = true;

                gridLookUpEdit1.EditValue = unit;
                spinEdit1.Properties.Mask.EditMask = DisplayLengthUnit.Mask;
                _doNotHandle = false;
            }



            _value = (double)va;
            UpdateSpinEditValue();
            DataSet = set as MCDataSet;
        }

        public void WriteValueIntoDataModel(DataModel model)
        {
            var list = model.GetListByName(QueryTableName);
            if (list == null || list.Count < 1)
            {
                return;
            }
            var prpty = list.MemberType.GetProperty(QueryFiledName);
            prpty?.SetValue(list[0], _value);

            if (prpty == null)
            {
                return;
            }

            var lengthUnitPrpty = list.MemberType.GetProperty(QueryDisplayUnitFieldName);
            var unit = gridLookUpEdit1.EditValue as LengthUnits;
            if (lengthUnitPrpty == null || unit == null)
            {
                return;
            }

            lengthUnitPrpty.SetValue(list[0], unit);
        }

        public void ReadValueFromDataModel(DataModel model)
        {
            var list = model.GetListByName(QueryTableName);
            if (list == null || list.Count < 1)
            {
                return;
            }

            var prpty = list.MemberType.GetProperty(QueryFiledName);
            if (prpty == null)
            {
                return;
            }

            var lengthUnitPrpty = list.MemberType.GetProperty(QueryDisplayUnitFieldName);
            LengthUnits unit;
            if (lengthUnitPrpty != null
                && (unit = lengthUnitPrpty.GetValue(list[0]) as LengthUnits) != null)
            {
                _doNotHandle = true;

                gridLookUpEdit1.EditValue = unit;
                spinEdit1.Properties.Mask.EditMask = DisplayLengthUnit.Mask;
                _doNotHandle = false;
            }

            _value = (double)prpty.GetValue(list[0]);
            UpdateSpinEditValue();
        }

        public void RemoveArrows()
        {
            spinEdit1.Properties.Buttons[0].Visible = false;
            spinEdit1.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
        }

        private bool _doNotHandle = false;
        private void UpdateSpinEditValue()
        {
            var showValue = _value * ValueLengthUnit.GetConversionFactor(DisplayLengthUnit);

            _doNotHandle = true;
            spinEdit1.EditValue = showValue;
            _doNotHandle = false;
        }

        public event System.EventHandler EditValueChanged;

        public bool UseParentBinding => false;


        public MCDataSet DataSet { get; private set; }

        public DataModel Model { get; private set; }

        public string QueryDisplayUnitFieldName { get; set; }
        public bool ReadOnly
        {
            get
            {
                return spinEdit1.ReadOnly;
            }
            set
            {
                spinEdit1.ReadOnly = value;
                
            }
        }

        public event EventHandler<LengthUnitDropdownFilter> FilterDropdown;
    }

}
