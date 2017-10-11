namespace PKSim.BatchTool.Views
{
   partial class SingleFolderSnapshotView
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
         this.btnOutputFolderSelection = new DevExpress.XtraEditors.ButtonEdit();
         this.btnInputFolderSelection = new DevExpress.XtraEditors.ButtonEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemInputFolder = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemOutputFolder = new DevExpress.XtraLayout.LayoutControlItem();
         this.radioGroupExportMode = new DevExpress.XtraEditors.RadioGroup();
         this.layoutItemExportMode = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutGroupProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.btnOutputFolderSelection.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnInputFolderSelection.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemInputFolder)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOutputFolder)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupExportMode.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportMode)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupProperties)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.radioGroupExportMode);
         this.layoutControl.Controls.Add(this.btnOutputFolderSelection);
         this.layoutControl.Controls.Add(this.btnInputFolderSelection);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(713, 476);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // btnOutputFolderSelection
         // 
         this.btnOutputFolderSelection.Location = new System.Drawing.Point(121, 55);
         this.btnOutputFolderSelection.Name = "btnOutputFolderSelection";
         this.btnOutputFolderSelection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.btnOutputFolderSelection.Size = new System.Drawing.Size(590, 20);
         this.btnOutputFolderSelection.StyleController = this.layoutControl;
         this.btnOutputFolderSelection.TabIndex = 5;
         // 
         // btnInputFolderSelection
         // 
         this.btnInputFolderSelection.Location = new System.Drawing.Point(121, 31);
         this.btnInputFolderSelection.Name = "btnInputFolderSelection";
         this.btnInputFolderSelection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.btnInputFolderSelection.Size = new System.Drawing.Size(590, 20);
         this.btnInputFolderSelection.StyleController = this.layoutControl;
         this.btnInputFolderSelection.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem1,
            this.layoutGroupProperties});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(713, 476);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemInputFolder
         // 
         this.layoutItemInputFolder.Control = this.btnInputFolderSelection;
         this.layoutItemInputFolder.Location = new System.Drawing.Point(0, 29);
         this.layoutItemInputFolder.Name = "layoutItemInputFolder";
         this.layoutItemInputFolder.Size = new System.Drawing.Size(713, 24);
         this.layoutItemInputFolder.TextSize = new System.Drawing.Size(116, 13);
         // 
         // layoutItemOutputFolder
         // 
         this.layoutItemOutputFolder.Control = this.btnOutputFolderSelection;
         this.layoutItemOutputFolder.Location = new System.Drawing.Point(0, 53);
         this.layoutItemOutputFolder.Name = "layoutItemOutputFolder";
         this.layoutItemOutputFolder.Size = new System.Drawing.Size(713, 24);
         this.layoutItemOutputFolder.TextSize = new System.Drawing.Size(116, 13);
         // 
         // radioGroupExportMode
         // 
         this.radioGroupExportMode.Location = new System.Drawing.Point(121, 2);
         this.radioGroupExportMode.Name = "radioGroupExportMode";
         this.radioGroupExportMode.Size = new System.Drawing.Size(590, 25);
         this.radioGroupExportMode.StyleController = this.layoutControl;
         this.radioGroupExportMode.TabIndex = 6;
         // 
         // layoutItemExportMode
         // 
         this.layoutItemExportMode.Control = this.radioGroupExportMode;
         this.layoutItemExportMode.Location = new System.Drawing.Point(0, 0);
         this.layoutItemExportMode.Name = "layoutItemExportMode";
         this.layoutItemExportMode.Size = new System.Drawing.Size(713, 29);
         this.layoutItemExportMode.TextSize = new System.Drawing.Size(116, 13);
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 77);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(713, 399);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutGroupProperties
         // 
         this.layoutGroupProperties.GroupBordersVisible = false;
         this.layoutGroupProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemExportMode,
            this.layoutItemInputFolder,
            this.layoutItemOutputFolder});
         this.layoutGroupProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupProperties.Name = "layoutGroupProperties";
         this.layoutGroupProperties.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutGroupProperties.Size = new System.Drawing.Size(713, 77);
         // 
         // SingleFolderSnapshotView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "SingleFolderSnapshotView";
         this.Size = new System.Drawing.Size(713, 476);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.btnOutputFolderSelection.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnInputFolderSelection.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemInputFolder)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOutputFolder)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupExportMode.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportMode)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupProperties)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControl layoutControl;
      private DevExpress.XtraEditors.ButtonEdit btnOutputFolderSelection;
      private DevExpress.XtraEditors.ButtonEdit btnInputFolderSelection;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemInputFolder;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOutputFolder;
      private DevExpress.XtraEditors.RadioGroup radioGroupExportMode;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExportMode;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupProperties;
   }
}
