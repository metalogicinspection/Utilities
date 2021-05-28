using System;
using System.Data;
using Gen3.Data;
using Metalogic.DataUtil;
using Metaphase.Editors;

namespace Metalogic.UI.Editors
{
    public class EmpisPasswordEdit : DevExpress.XtraEditors.PopupContainerEdit, IEnovaEdit
    {

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

        public EmpisPasswordEdit()
        {

            popupContainerControl1 = new DevExpress.XtraEditors.PopupContainerControl();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            textEdit3 = new DevExpress.XtraEditors.TextEdit();
            textEdit2 = new DevExpress.XtraEditors.TextEdit();
            textEdit1 = new DevExpress.XtraEditors.TextEdit();
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            simpleButton2 = new DevExpress.XtraEditors.SimpleButton();

            // 
            // popupContainerControl1
            // 
            popupContainerControl1.Controls.Add(simpleButton2);
            popupContainerControl1.Controls.Add(simpleButton1);
            popupContainerControl1.Controls.Add(labelControl3);
            popupContainerControl1.Controls.Add(labelControl2);
            popupContainerControl1.Controls.Add(labelControl1);
            popupContainerControl1.Controls.Add(textEdit3);
            popupContainerControl1.Controls.Add(textEdit2);
            popupContainerControl1.Controls.Add(textEdit1);
            popupContainerControl1.Location = new System.Drawing.Point(13, 86);
            popupContainerControl1.Name = "popupContainerControl1";
            popupContainerControl1.Size = new System.Drawing.Size(364, 156);
            popupContainerControl1.MinimumSize = new System.Drawing.Size(364, 156);
            popupContainerControl1.MinimumSize = new System.Drawing.Size(364, 156);
            popupContainerControl1.TabIndex = 1;
            // 
            // labelControl3
            // 
            labelControl3.Location = new System.Drawing.Point(14, 79);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(126, 13);
            labelControl3.TabIndex = 5;
            labelControl3.Text = "Re-enter New Password:*";
            // 
            // labelControl2
            // 
            labelControl2.Location = new System.Drawing.Point(60, 53);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(80, 13);
            labelControl2.TabIndex = 4;
            labelControl2.Text = "New Password:*";
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(65, 27);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(75, 13);
            labelControl1.TabIndex = 3;
            labelControl1.Text = "Old Password:*";
            // 
            // textEdit3
            // 
            textEdit3.Location = new System.Drawing.Point(148, 76);
            textEdit3.Name = "textEdit3";
            textEdit3.Size = new System.Drawing.Size(188, 20);
            textEdit3.TabIndex = 2;
            // 
            // textEdit2
            // 
            textEdit2.Location = new System.Drawing.Point(148, 50);
            textEdit2.Name = "textEdit2";
            textEdit2.Size = new System.Drawing.Size(188, 20);
            textEdit2.TabIndex = 1;
            // 
            // textEdit1
            // 
            textEdit1.Location = new System.Drawing.Point(148, 24);
            textEdit1.Name = "textEdit1";
            textEdit1.Size = new System.Drawing.Size(188, 20);
            textEdit1.TabIndex = 0;
            // 
            // simpleButton1
            // 
            simpleButton1.Location = new System.Drawing.Point(79, 115);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new System.Drawing.Size(75, 23);
            simpleButton1.TabIndex = 6;
            simpleButton1.Text = "Change";
            // 
            // simpleButton2
            // 
            simpleButton2.Location = new System.Drawing.Point(182, 115);
            simpleButton2.Name = "simpleButton2";
            simpleButton2.Size = new System.Drawing.Size(75, 23);
            simpleButton2.TabIndex = 7;
            simpleButton2.Text = "Cancel";


            Properties.PasswordChar = '*';
            textEdit1.Properties.PasswordChar = '*';
            textEdit2.Properties.PasswordChar = '*';
            textEdit3.Properties.PasswordChar = '*';
            Properties.PopupControl = popupContainerControl1;
            simpleButton1.Click += SimpleButton1Click;
            simpleButton2.Click += SimpleButton2Click;

            simpleButton1.Enabled = false;
            textEdit1.EditValueChanged += editValueChanged;
            textEdit2.EditValueChanged += editValueChanged;
            textEdit3.EditValueChanged += editValueChanged;

            popupContainerControl1.PopupContainerProperties.ShowPopupCloseButton = false;
            PropertiesChanged += PropertyChanged;

            QueryPopUp += queryPop;
        }


        private DevExpress.XtraEditors.PopupContainerControl popupContainerControl1;
        private DevExpress.XtraEditors.TextEdit textEdit3;
        private DevExpress.XtraEditors.TextEdit textEdit2;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;


        private void PropertyChanged(object sender, EventArgs e)
        {
            Properties.Buttons[0].Enabled = !Properties.ReadOnly;
        }

        private void SimpleButton1Click(object sender, EventArgs e)
        {
            Parent.Focus();
            if (!textEdit2.EditValue.Equals(EditValue))
            {
                EditValue = textEdit2.EditValue;
            }
        }

        private void SimpleButton2Click(object sender, EventArgs e)
        {
            Parent.Focus();
        }

        private string _oldPassword = string.Empty;
        private void queryPop(object sender, EventArgs e)
        {
            Clear();
            if (EditValue == null )
            {
                _oldPassword = string.Empty;
            }
            else
            {
                _oldPassword = EditValue.ToString();
            }

            textEdit1.Enabled = !string.IsNullOrEmpty(_oldPassword);
        }


        private void Clear()
        {
            _doNotHandleEditValueChanged = true;

            textEdit1.EditValue = string.Empty;
            textEdit2.EditValue = string.Empty;
            textEdit3.EditValue = string.Empty;
            simpleButton1.Enabled = false;

            _doNotHandleEditValueChanged = false;
        }

        private bool _doNotHandleEditValueChanged = false;

        private void editValueChanged(object sender, EventArgs e)
        {
            if (_doNotHandleEditValueChanged)
            {
                return;
            }
            simpleButton1.Enabled = 
                textEdit1.EditValue.Equals(_oldPassword)
                && textEdit2.EditValue.Equals(textEdit3.EditValue)
                && textEdit2.EditValue.ToString().Length >= 5;
        }

        public bool UseParentBinding => false;
    }
}
