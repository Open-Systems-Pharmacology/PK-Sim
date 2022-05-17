namespace PKSim.UI.Views.Simulations
{
   partial class ImportSimulationResultsView
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
         _gridViewBinder.Dispose();
         _screenBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.tbLog = new DevExpress.XtraEditors.MemoEdit();
         this.btnAddFile = new DevExpress.XtraEditors.SimpleButton();
         this.btnImport = new DevExpress.XtraEditors.SimpleButton();
         this.gridControl = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemButtonImport = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItemButtonImport = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutGroupImportFolder = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupImportSingleFiles = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonAdd = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItemButtonAddFile = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemLog = new DevExpress.XtraLayout.LayoutControlItem();
         this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
         this.btnBrowseForFolder = new DevExpress.XtraEditors.ButtonEdit();
         this.layoutItemButtonBrowse = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbLog.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonImport)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemButtonImport)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupImportFolder)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupImportSingleFiles)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemButtonAddFile)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLog)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnBrowseForFolder.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonBrowse)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.btnBrowseForFolder);
         this.layoutControl.Controls.Add(this.tbLog);
         this.layoutControl.Controls.Add(this.btnAddFile);
         this.layoutControl.Controls.Add(this.btnImport);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(867, 295, 250, 350);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(587, 598);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl1";
         // 
         // tbLog
         // 
         this.tbLog.Location = new System.Drawing.Point(24, 354);
         this.tbLog.Name = "tbLog";
         this.tbLog.Size = new System.Drawing.Size(539, 194);
         this.tbLog.StyleController = this.layoutControl;
         this.tbLog.TabIndex = 12;
         // 
         // btnAddFile
         // 
         this.btnAddFile.Location = new System.Drawing.Point(296, 110);
         this.btnAddFile.Name = "btnAddFile";
         this.btnAddFile.Size = new System.Drawing.Size(267, 22);
         this.btnAddFile.StyleController = this.layoutControl;
         this.btnAddFile.TabIndex = 11;
         this.btnAddFile.Text = "btnAddFile";
         // 
         // btnImport
         // 
         this.btnImport.Location = new System.Drawing.Point(296, 564);
         this.btnImport.Name = "btnImport";
         this.btnImport.Size = new System.Drawing.Size(279, 22);
         this.btnImport.StyleController = this.layoutControl;
         this.btnImport.TabIndex = 10;
         this.btnImport.Text = "btnImport";
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(24, 136);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(539, 209);
         this.gridControl.TabIndex = 8;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.GridControl = this.gridControl;
         this.gridView.Name = "gridView";
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemButtonImport,
            this.emptySpaceItemButtonImport,
            this.layoutGroupImportFolder,
            this.layoutGroupImportSingleFiles});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.Size = new System.Drawing.Size(587, 598);
         this.layoutControlGroup.Text = "Root";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemButtonImport
         // 
         this.layoutItemButtonImport.Control = this.btnImport;
         this.layoutItemButtonImport.CustomizationFormText = "layoutControlItem5";
         this.layoutItemButtonImport.Location = new System.Drawing.Point(284, 552);
         this.layoutItemButtonImport.Name = "layoutControlItem5";
         this.layoutItemButtonImport.Size = new System.Drawing.Size(283, 26);
         this.layoutItemButtonImport.Text = "layoutControlItem5";
         this.layoutItemButtonImport.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonImport.TextToControlDistance = 0;
         this.layoutItemButtonImport.TextVisible = false;
         // 
         // emptySpaceItemButtonImport
         // 
         this.emptySpaceItemButtonImport.AllowHotTrack = false;
         this.emptySpaceItemButtonImport.CustomizationFormText = "emptySpaceItem";
         this.emptySpaceItemButtonImport.Location = new System.Drawing.Point(0, 552);
         this.emptySpaceItemButtonImport.Name = "emptySpaceItemButtonImport";
         this.emptySpaceItemButtonImport.Size = new System.Drawing.Size(284, 26);
         this.emptySpaceItemButtonImport.Text = "emptySpaceItemButtonImport";
         this.emptySpaceItemButtonImport.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutGroupImportFolder
         // 
         this.layoutGroupImportFolder.CustomizationFormText = "layoutGroupImportFolder";
         this.layoutGroupImportFolder.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemButtonBrowse});
         this.layoutGroupImportFolder.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupImportFolder.Name = "layoutGroupImportFolder";
         this.layoutGroupImportFolder.Size = new System.Drawing.Size(567, 67);
         this.layoutGroupImportFolder.Text = "layoutGroupImportFolder";
         // 
         // layoutGroupImportSingleFiles
         // 
         this.layoutGroupImportSingleFiles.CustomizationFormText = "layoutGroupImportSingleFiles";
         this.layoutGroupImportSingleFiles.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4,
            this.layoutItemButtonAdd,
            this.emptySpaceItemButtonAddFile,
            this.layoutItemLog,
            this.splitterItem1});
         this.layoutGroupImportSingleFiles.Location = new System.Drawing.Point(0, 67);
         this.layoutGroupImportSingleFiles.Name = "layoutGroupImportSingleFiles";
         this.layoutGroupImportSingleFiles.Size = new System.Drawing.Size(567, 485);
         this.layoutGroupImportSingleFiles.Text = "layoutGroupImportSingleFiles";
         // 
         // layoutControlItem4
         // 
         this.layoutControlItem4.Control = this.gridControl;
         this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
         this.layoutControlItem4.Location = new System.Drawing.Point(0, 26);
         this.layoutControlItem4.Name = "layoutControlItem4";
         this.layoutControlItem4.Size = new System.Drawing.Size(543, 213);
         this.layoutControlItem4.Text = "layoutControlItem4";
         this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem4.TextToControlDistance = 0;
         this.layoutControlItem4.TextVisible = false;
         // 
         // layoutItemButtonAdd
         // 
         this.layoutItemButtonAdd.Control = this.btnAddFile;
         this.layoutItemButtonAdd.CustomizationFormText = "layoutControlItem1";
         this.layoutItemButtonAdd.Location = new System.Drawing.Point(272, 0);
         this.layoutItemButtonAdd.Name = "layoutControlItem1";
         this.layoutItemButtonAdd.Size = new System.Drawing.Size(271, 26);
         this.layoutItemButtonAdd.Text = "layoutControlItem1";
         this.layoutItemButtonAdd.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonAdd.TextToControlDistance = 0;
         this.layoutItemButtonAdd.TextVisible = false;
         // 
         // emptySpaceItemButtonAddFile
         // 
         this.emptySpaceItemButtonAddFile.AllowHotTrack = false;
         this.emptySpaceItemButtonAddFile.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItemButtonAddFile.Location = new System.Drawing.Point(0, 0);
         this.emptySpaceItemButtonAddFile.Name = "emptySpaceItemButtonAddFile";
         this.emptySpaceItemButtonAddFile.Size = new System.Drawing.Size(272, 26);
         this.emptySpaceItemButtonAddFile.Text = "emptySpaceItemButtonAddFile";
         this.emptySpaceItemButtonAddFile.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemLog
         // 
         this.layoutItemLog.Control = this.tbLog;
         this.layoutItemLog.CustomizationFormText = "layoutItemLog";
         this.layoutItemLog.Location = new System.Drawing.Point(0, 244);
         this.layoutItemLog.Name = "layoutItemLog";
         this.layoutItemLog.Size = new System.Drawing.Size(543, 198);
         this.layoutItemLog.Text = "layoutItemLog";
         this.layoutItemLog.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLog.TextToControlDistance = 0;
         this.layoutItemLog.TextVisible = false;
         // 
         // splitterItem1
         // 
         this.splitterItem1.AllowHotTrack = true;
         this.splitterItem1.CustomizationFormText = "splitterItem1";
         this.splitterItem1.Location = new System.Drawing.Point(0, 239);
         this.splitterItem1.Name = "splitterItem1";
         this.splitterItem1.Size = new System.Drawing.Size(543, 5);
         // 
         // btnBrowseForFolder
         // 
         this.btnBrowseForFolder.Location = new System.Drawing.Point(24, 43);
         this.btnBrowseForFolder.Name = "btnBrowseForFolder";
         this.btnBrowseForFolder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.btnBrowseForFolder.Size = new System.Drawing.Size(539, 20);
         this.btnBrowseForFolder.StyleController = this.layoutControl;
         this.btnBrowseForFolder.TabIndex = 13;
         // 
         // layoutItemButtonBrowse
         // 
         this.layoutItemButtonBrowse.Control = this.btnBrowseForFolder;
         this.layoutItemButtonBrowse.CustomizationFormText = "layoutItemButtonBrowse";
         this.layoutItemButtonBrowse.Location = new System.Drawing.Point(0, 0);
         this.layoutItemButtonBrowse.Name = "layoutItemButtonBrowse";
         this.layoutItemButtonBrowse.Size = new System.Drawing.Size(543, 24);
         this.layoutItemButtonBrowse.Text = "layoutItemButtonBrowse";
         this.layoutItemButtonBrowse.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonBrowse.TextToControlDistance = 0;
         this.layoutItemButtonBrowse.TextVisible = false;
         // 
         // ImportSimulationResultsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "ImportSimulationResultsView";
         this.ClientSize = new System.Drawing.Size(587, 644);
         this.Controls.Add(this.layoutControl);
         this.Name = "ImportSimulationResultsView";
         this.Text = "ImportSimulationResultsView";
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tbLog.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonImport)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemButtonImport)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupImportFolder)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupImportSingleFiles)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemButtonAddFile)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLog)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnBrowseForFolder.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonBrowse)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.SimpleButton btnImport;
      private DevExpress.XtraGrid.GridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonImport;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItemButtonImport;
      private DevExpress.XtraEditors.SimpleButton btnAddFile;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupImportFolder;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupImportSingleFiles;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonAdd;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItemButtonAddFile;
      private DevExpress.XtraEditors.MemoEdit tbLog;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLog;
      private DevExpress.XtraLayout.SplitterItem splitterItem1;
      private DevExpress.XtraEditors.ButtonEdit btnBrowseForFolder;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonBrowse;
   }
}