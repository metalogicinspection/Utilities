namespace Gen3.UI.Grids
{
    partial class SimpleGridControl
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
            this.components = new System.ComponentModel.Container();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.weldView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.DeleteWeld = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.DeleteDefect = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsReadOnlyInchDisplay = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.rsEditLength = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsEditOffset = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsEditLocation = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsEditWidth = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsEditCapWdith = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.repositoryItemMemoEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.rsExpTime = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.rsEditDepth = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsStartTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.rsEndTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.rsShotTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.rsDeleteFilm = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemMemoEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.rsDeleteGap = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weldView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DeleteWeld)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DeleteDefect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsReadOnlyInchDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditCapWdith)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsExpTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsStartTextEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEndTextEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsShotTextEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsDeleteFilm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsDeleteGap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
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
            // weldView
            // 
            this.weldView.GridControl = this.gridControl1;
            this.weldView.Name = "weldView";
            this.weldView.OptionsCustomization.AllowGroup = false;
            this.weldView.OptionsDetail.EnableMasterViewMode = false;
            this.weldView.OptionsNavigation.AutoFocusNewRow = true;
            this.weldView.OptionsView.ColumnAutoWidth = false;
            this.weldView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.weldView.OptionsView.RowAutoHeight = true;
            this.weldView.OptionsView.ShowGroupPanel = false;
            // 
            // DeleteWeld
            // 
            this.DeleteWeld.AutoHeight = false;
            this.DeleteWeld.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.DeleteWeld.Name = "DeleteWeld";
            this.DeleteWeld.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // DeleteDefect
            // 
            this.DeleteDefect.AutoHeight = false;
            this.DeleteDefect.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.DeleteDefect.Name = "DeleteDefect";
            this.DeleteDefect.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // rsReadOnlyInchDisplay
            // 
            this.rsReadOnlyInchDisplay.AutoHeight = false;
            this.rsReadOnlyInchDisplay.Name = "rsReadOnlyInchDisplay";
            // 
            // rsEditLength
            // 
            this.rsEditLength.AutoHeight = false;
            this.rsEditLength.Mask.EditMask = "\\d+[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditLength.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditLength.Name = "rsEditLength";
            // 
            // rsEditOffset
            // 
            this.rsEditOffset.AutoHeight = false;
            this.rsEditOffset.Mask.EditMask = "-?[0-9]*[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditOffset.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditOffset.Name = "rsEditOffset";
            // 
            // rsEditLocation
            // 
            this.rsEditLocation.AutoHeight = false;
            this.rsEditLocation.Mask.EditMask = "\\d+[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditLocation.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditLocation.Name = "rsEditLocation";
            // 
            // rsEditWidth
            // 
            this.rsEditWidth.AutoHeight = false;
            this.rsEditWidth.Mask.EditMask = "\\d+[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditWidth.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditWidth.Name = "rsEditWidth";
            // 
            // rsEditCapWdith
            // 
            this.rsEditCapWdith.AutoHeight = false;
            this.rsEditCapWdith.Mask.EditMask = "\\d+[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditCapWdith.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditCapWdith.Name = "rsEditCapWdith";
            // 
            // repositoryItemMemoEdit1
            // 
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            this.repositoryItemMemoEdit1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            // 
            // repositoryItemMemoEdit2
            // 
            this.repositoryItemMemoEdit2.Name = "repositoryItemMemoEdit2";
            this.repositoryItemMemoEdit2.ScrollBars = System.Windows.Forms.ScrollBars.None;
            // 
            // rsExpTime
            // 
            this.rsExpTime.AutoHeight = false;
            this.rsExpTime.Name = "rsExpTime";
            // 
            // rsEditDepth
            // 
            this.rsEditDepth.AutoHeight = false;
            this.rsEditDepth.Mask.EditMask = "\\d+[.]?[0-9]?[0-9]?[0-9]?";
            this.rsEditDepth.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEditDepth.Name = "rsEditDepth";
            // 
            // rsStartTextEdit
            // 
            this.rsStartTextEdit.AutoHeight = false;
            this.rsStartTextEdit.Mask.EditMask = "[0-9]{1,3}(\\.[0-9][0-9]?)?";
            this.rsStartTextEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsStartTextEdit.Name = "rsStartTextEdit";
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // rsEndTextEdit
            // 
            this.rsEndTextEdit.AutoHeight = false;
            this.rsEndTextEdit.Mask.EditMask = "[0-9]{1,3}(\\.[0-9][0-9]?)?";
            this.rsEndTextEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.rsEndTextEdit.Name = "rsEndTextEdit";
            // 
            // rsShotTextEdit
            // 
            this.rsShotTextEdit.AutoHeight = false;
            this.rsShotTextEdit.Name = "rsShotTextEdit";
            // 
            // rsDeleteFilm
            // 
            this.rsDeleteFilm.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.rsDeleteFilm.Name = "rsDeleteFilm";
            this.rsDeleteFilm.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // repositoryItemMemoEdit3
            // 
            this.repositoryItemMemoEdit3.Name = "repositoryItemMemoEdit3";
            this.repositoryItemMemoEdit3.ScrollBars = System.Windows.Forms.ScrollBars.None;
            // 
            // rsDeleteGap
            // 
            this.rsDeleteGap.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.rsDeleteGap.Name = "rsDeleteGap";
            this.rsDeleteGap.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // SimpleGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridControl1);
            this.Name = "SimpleGridControl";
            this.Size = new System.Drawing.Size(798, 527);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weldView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DeleteWeld)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DeleteDefect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsReadOnlyInchDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditCapWdith)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsExpTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEditDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsStartTextEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsEndTextEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsShotTextEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsDeleteFilm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rsDeleteGap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
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
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
    }
}
