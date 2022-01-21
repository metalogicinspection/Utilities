
namespace ConsoleApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.enovaPicklistEdit1 = new Metalogic.UI.Editors.EnovaPicklistEdit();
            this.enovaPicklistEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.enovaPicklistEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // enovaPicklistEdit1
            // 
            this.enovaPicklistEdit1.Caption = "";
            this.enovaPicklistEdit1.DisplayValueMode = ((Metalogic.UI.EditorsInGrid.PicklistDisplayValueModes)((Metalogic.UI.EditorsInGrid.PicklistDisplayValueModes.Code | Metalogic.UI.EditorsInGrid.PicklistDisplayValueModes.ShortDescription)));
            this.enovaPicklistEdit1.Location = new System.Drawing.Point(293, 121);
            this.enovaPicklistEdit1.Name = "enovaPicklistEdit1";
            this.enovaPicklistEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.enovaPicklistEdit1.Properties.DisplayMember = "Code";
            this.enovaPicklistEdit1.Properties.NullText = "";
            this.enovaPicklistEdit1.Properties.PopupView = this.enovaPicklistEdit1View;
            this.enovaPicklistEdit1.QueryFiledName = "";
            this.enovaPicklistEdit1.QueryTableName = "";
            this.enovaPicklistEdit1.Size = new System.Drawing.Size(100, 20);
            this.enovaPicklistEdit1.TabIndex = 0;
            // 
            // enovaPicklistEdit1View
            // 
            this.enovaPicklistEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.enovaPicklistEdit1View.Name = "enovaPicklistEdit1View";
            this.enovaPicklistEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.enovaPicklistEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.enovaPicklistEdit1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.enovaPicklistEdit1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Metalogic.UI.Editors.EnovaPicklistEdit enovaPicklistEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView enovaPicklistEdit1View;
    }
}