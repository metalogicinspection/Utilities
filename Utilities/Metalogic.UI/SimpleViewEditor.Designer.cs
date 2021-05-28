namespace Metalogic.UI
{
    partial class SimpleViewEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.rsEditLocation = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsEditLength = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsEditWidth = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsEditOffset = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsEditDepth = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.DeleteDefect = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsShotTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.rsStartTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsEndTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsDeleteFilm = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsDeleteGap = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.weldView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemMemoEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.rsEditCapWdith = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsReadOnlyInchDisplay = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.rsExpTime = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemMemoEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.DeleteWeld = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DeleteDefect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsShotTextEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsStartTextEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEndTextEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsDeleteFilm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsDeleteGap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weldView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditCapWdith)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsReadOnlyInchDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsExpTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DeleteWeld)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.weldView;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.DeleteWeld,
            this.DeleteDefect,
            this.rsReadOnlyInchDisplay,
            this.rsEditLength,
            this.rsEditOffset,
            this.rsEditLocation,
            this.rsEditWidth,
            this.rsEditCapWdith,
            this.repositoryItemMemoEdit1,
            this.repositoryItemMemoEdit2,
            this.rsExpTime,
            this.rsEditDepth,
            this.rsStartTextEdit,
            this.repositoryItemTextEdit1,
            this.rsEndTextEdit,
            this.rsShotTextEdit,
            this.rsDeleteFilm,
            this.repositoryItemMemoEdit3,
            this.rsDeleteGap});
            this.gridControl1.Size = new System.Drawing.Size(798, 527);
            this.gridControl1.TabIndex = 1;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.weldView});
            // 
            // rsEditLocation
            // 
            this.rsEditLocation.AutoHeight = false;
            this.rsEditLocation.Mask.EditMask = "\\d+[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditLocation.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditLocation.Name = "rsEditLocation";
            // 
            // rsEditLength
            // 
            this.rsEditLength.AutoHeight = false;
            this.rsEditLength.Mask.EditMask = "\\d+[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditLength.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditLength.Name = "rsEditLength";
            // 
            // rsEditWidth
            // 
            this.rsEditWidth.AutoHeight = false;
            this.rsEditWidth.Mask.EditMask = "\\d+[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditWidth.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditWidth.Name = "rsEditWidth";
            // 
            // rsEditOffset
            // 
            this.rsEditOffset.AutoHeight = false;
            this.rsEditOffset.Mask.EditMask = "-?[0-9]*[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditOffset.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditOffset.Name = "rsEditOffset";
            // 
            // rsEditDepth
            // 
            this.rsEditDepth.AutoHeight = false;
            this.rsEditDepth.Mask.EditMask = "\\d+[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditDepth.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditDepth.Name = "rsEditDepth";
            // 
            // repositoryItemMemoEdit1
            // 
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            this.repositoryItemMemoEdit1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            // 
            // DeleteDefect
            // 
            this.DeleteDefect.AutoHeight = false;
            this.DeleteDefect.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.DeleteDefect.Name = "DeleteDefect";
            this.DeleteDefect.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // rsShotTextEdit
            // 
            this.rsShotTextEdit.AutoHeight = false;
            this.rsShotTextEdit.Name = "rsShotTextEdit";
            // 
            // rsStartTextEdit
            // 
            this.rsStartTextEdit.AutoHeight = false;
            this.rsStartTextEdit.Mask.EditMask = "[0-9]{1,3}(\\.[0-9][0-9]?)?";
            this.rsStartTextEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsStartTextEdit.Name = "rsStartTextEdit";
            // 
            // rsEndTextEdit
            // 
            this.rsEndTextEdit.AutoHeight = false;
            this.rsEndTextEdit.Mask.EditMask = "[0-9]{1,3}(\\.[0-9][0-9]?)?";
            this.rsEndTextEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEndTextEdit.Name = "rsEndTextEdit";
            // 
            // rsDeleteFilm
            // 
            this.rsDeleteFilm.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.rsDeleteFilm.Name = "rsDeleteFilm";
            this.rsDeleteFilm.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // rsDeleteGap
            // 
            this.rsDeleteGap.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.rsDeleteGap.Name = "rsDeleteGap";
            this.rsDeleteGap.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // weldView
            // 
            this.weldView.GridControl = this.gridControl1;
            this.weldView.Name = "weldView";
            this.weldView.OptionsCustomization.AllowGroup = false;
            this.weldView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.weldView.OptionsView.ColumnAutoWidth = false;
            this.weldView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.weldView.OptionsView.RowAutoHeight = true;
            this.weldView.OptionsView.ShowGroupPanel = false;
            // 
            // repositoryItemMemoEdit3
            // 
            this.repositoryItemMemoEdit3.Name = "repositoryItemMemoEdit3";
            this.repositoryItemMemoEdit3.ScrollBars = System.Windows.Forms.ScrollBars.None;
            // 
            // rsEditCapWdith
            // 
            this.rsEditCapWdith.AutoHeight = false;
            this.rsEditCapWdith.Mask.EditMask = "\\d+[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditCapWdith.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditCapWdith.Name = "rsEditCapWdith";
            // 
            // rsReadOnlyInchDisplay
            // 
            this.rsReadOnlyInchDisplay.AutoHeight = false;
            this.rsReadOnlyInchDisplay.Name = "rsReadOnlyInchDisplay";
            // 
            // rsExpTime
            // 
            this.rsExpTime.AutoHeight = false;
            this.rsExpTime.Name = "rsExpTime";
            // 
            // repositoryItemMemoEdit2
            // 
            this.repositoryItemMemoEdit2.Name = "repositoryItemMemoEdit2";
            this.repositoryItemMemoEdit2.ScrollBars = System.Windows.Forms.ScrollBars.None;
            // 
            // DeleteWeld
            // 
            this.DeleteWeld.AutoHeight = false;
            this.DeleteWeld.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.DeleteWeld.Name = "DeleteWeld";
            this.DeleteWeld.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // SimpleViewEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridControl1);
            this.Name = "SimpleViewEditor";
            this.Size = new System.Drawing.Size(798, 527);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DeleteDefect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsShotTextEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsStartTextEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEndTextEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsDeleteFilm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsDeleteGap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weldView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditCapWdith)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsReadOnlyInchDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsExpTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DeleteWeld)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView weldView;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit3;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rsEditCapWdith;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit rsReadOnlyInchDisplay;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit rsExpTime;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit DeleteWeld;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit DeleteDefect;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rsEditLength;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rsEditOffset;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rsEditLocation;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rsEditWidth;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rsEditDepth;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rsStartTextEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rsEndTextEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit rsShotTextEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rsDeleteFilm;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rsDeleteGap;
    }
}
