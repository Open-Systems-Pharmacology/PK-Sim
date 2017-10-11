namespace PKSim.BatchTool.Views
{
   partial class FolderListSnapshotView
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
         _screenBinder.Dispose();
         _gridViewBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
         this.buttonClearList = new DevExpress.XtraEditors.SimpleButton();
         this.buttonExportList = new DevExpress.XtraEditors.SimpleButton();
         this.gridControl = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.buttonImportList = new DevExpress.XtraEditors.SimpleButton();
         this.buttonAdd = new DevExpress.XtraEditors.SimpleButton();
         this.buttonSelectFolder = new DevExpress.XtraEditors.ButtonEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemSelectFolder = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonAdd = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonImportList = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemGridView = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonExportList = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonClearList = new DevExpress.XtraLayout.LayoutControlItem();
         this.radioGroupExportMode = new DevExpress.XtraEditors.RadioGroup();
         this.layoutItemExportMode = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.buttonSelectFolder.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSelectFolder)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonImportList)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonExportList)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonClearList)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupExportMode.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportMode)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.radioGroupExportMode);
         this.layoutControl.Controls.Add(this.buttonClearList);
         this.layoutControl.Controls.Add(this.buttonExportList);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Controls.Add(this.buttonImportList);
         this.layoutControl.Controls.Add(this.buttonAdd);
         this.layoutControl.Controls.Add(this.buttonSelectFolder);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1036, 227, 450, 400);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(645, 406);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // buttonClearList
         // 
         this.buttonClearList.Location = new System.Drawing.Point(561, 382);
         this.buttonClearList.Name = "buttonClearList";
         this.buttonClearList.Size = new System.Drawing.Size(82, 22);
         this.buttonClearList.StyleController = this.layoutControl;
         this.buttonClearList.TabIndex = 10;
         this.buttonClearList.Text = "buttonClearList";
         // 
         // buttonExportList
         // 
         this.buttonExportList.Location = new System.Drawing.Point(468, 382);
         this.buttonExportList.Name = "buttonExportList";
         this.buttonExportList.Size = new System.Drawing.Size(89, 22);
         this.buttonExportList.StyleController = this.layoutControl;
         this.buttonExportList.TabIndex = 9;
         this.buttonExportList.Text = "buttonExportList";
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(2, 57);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(641, 321);
         this.gridControl.TabIndex = 8;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.AllowsFiltering = true;
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.GridControl = this.gridControl;
         this.gridView.MultiSelect = false;
         this.gridView.Name = "gridView";
         this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // buttonImportList
         // 
         this.buttonImportList.Location = new System.Drawing.Point(324, 382);
         this.buttonImportList.Name = "buttonImportList";
         this.buttonImportList.Size = new System.Drawing.Size(140, 22);
         this.buttonImportList.StyleController = this.layoutControl;
         this.buttonImportList.TabIndex = 7;
         this.buttonImportList.Text = "buttonImportList";
         // 
         // buttonAdd
         // 
         this.buttonAdd.Location = new System.Drawing.Point(555, 31);
         this.buttonAdd.Name = "buttonAdd";
         this.buttonAdd.Size = new System.Drawing.Size(88, 22);
         this.buttonAdd.StyleController = this.layoutControl;
         this.buttonAdd.TabIndex = 5;
         this.buttonAdd.Text = "buttonAdd";
         // 
         // buttonSelectFolder
         // 
         this.buttonSelectFolder.Location = new System.Drawing.Point(116, 31);
         this.buttonSelectFolder.Name = "buttonSelectFolder";
         this.buttonSelectFolder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.buttonSelectFolder.Size = new System.Drawing.Size(435, 20);
         this.buttonSelectFolder.StyleController = this.layoutControl;
         this.buttonSelectFolder.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSelectFolder,
            this.layoutItemButtonAdd,
            this.layoutItemButtonImportList,
            this.emptySpaceItem1,
            this.layoutItemGridView,
            this.layoutItemButtonExportList,
            this.layoutItemButtonClearList,
            this.layoutItemExportMode});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(645, 406);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemSelectFolder
         // 
         this.layoutItemSelectFolder.Control = this.buttonSelectFolder;
         this.layoutItemSelectFolder.Location = new System.Drawing.Point(0, 29);
         this.layoutItemSelectFolder.Name = "layoutItemSelectFolder";
         this.layoutItemSelectFolder.Size = new System.Drawing.Size(553, 26);
         this.layoutItemSelectFolder.TextSize = new System.Drawing.Size(111, 13);
         // 
         // layoutItemButtonAdd
         // 
         this.layoutItemButtonAdd.Control = this.buttonAdd;
         this.layoutItemButtonAdd.Location = new System.Drawing.Point(553, 29);
         this.layoutItemButtonAdd.Name = "layoutItemButtonAdd";
         this.layoutItemButtonAdd.Size = new System.Drawing.Size(92, 26);
         this.layoutItemButtonAdd.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonAdd.TextVisible = false;
         // 
         // layoutItemButtonImportList
         // 
         this.layoutItemButtonImportList.Control = this.buttonImportList;
         this.layoutItemButtonImportList.Location = new System.Drawing.Point(322, 380);
         this.layoutItemButtonImportList.Name = "layoutItemButtonImportList";
         this.layoutItemButtonImportList.Size = new System.Drawing.Size(144, 26);
         this.layoutItemButtonImportList.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonImportList.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 380);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(322, 26);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemGridView
         // 
         this.layoutItemGridView.Control = this.gridControl;
         this.layoutItemGridView.Location = new System.Drawing.Point(0, 55);
         this.layoutItemGridView.Name = "layoutItemGridView";
         this.layoutItemGridView.Size = new System.Drawing.Size(645, 325);
         this.layoutItemGridView.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemGridView.TextVisible = false;
         // 
         // layoutItemButtonExportList
         // 
         this.layoutItemButtonExportList.Control = this.buttonExportList;
         this.layoutItemButtonExportList.Location = new System.Drawing.Point(466, 380);
         this.layoutItemButtonExportList.Name = "layoutItemButtonExportList";
         this.layoutItemButtonExportList.Size = new System.Drawing.Size(93, 26);
         this.layoutItemButtonExportList.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonExportList.TextVisible = false;
         // 
         // layoutItemButtonClearList
         // 
         this.layoutItemButtonClearList.Control = this.buttonClearList;
         this.layoutItemButtonClearList.Location = new System.Drawing.Point(559, 380);
         this.layoutItemButtonClearList.Name = "layoutItemButtonClearList";
         this.layoutItemButtonClearList.Size = new System.Drawing.Size(86, 26);
         this.layoutItemButtonClearList.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonClearList.TextVisible = false;
         // 
         // radioGroupExportMode
         // 
         this.radioGroupExportMode.Location = new System.Drawing.Point(116, 2);
         this.radioGroupExportMode.Name = "radioGroupExportMode";
         this.radioGroupExportMode.Size = new System.Drawing.Size(527, 25);
         this.radioGroupExportMode.StyleController = this.layoutControl;
         this.radioGroupExportMode.TabIndex = 11;
         // 
         // layoutItemExportMode
         // 
         this.layoutItemExportMode.Control = this.radioGroupExportMode;
         this.layoutItemExportMode.Location = new System.Drawing.Point(0, 0);
         this.layoutItemExportMode.Name = "layoutItemExportMode";
         this.layoutItemExportMode.Size = new System.Drawing.Size(645, 29);
         this.layoutItemExportMode.TextSize = new System.Drawing.Size(111, 13);
         // 
         // FolderListSnapshotView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "FolderListSnapshotView";
         this.Size = new System.Drawing.Size(645, 406);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.buttonSelectFolder.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSelectFolder)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonImportList)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonExportList)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonClearList)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupExportMode.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportMode)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControl layoutControl;
      private OSPSuite.UI.Controls.UxGridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraEditors.SimpleButton buttonImportList;
      private DevExpress.XtraEditors.SimpleButton buttonAdd;
      private DevExpress.XtraEditors.ButtonEdit buttonSelectFolder;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSelectFolder;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonAdd;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonImportList;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGridView;
      private DevExpress.XtraEditors.SimpleButton buttonClearList;
      private DevExpress.XtraEditors.SimpleButton buttonExportList;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonExportList;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonClearList;
      private DevExpress.XtraEditors.RadioGroup radioGroupExportMode;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExportMode;
   }
}
