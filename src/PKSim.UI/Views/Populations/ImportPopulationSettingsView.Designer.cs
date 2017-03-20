namespace PKSim.UI.Views.Populations
{
   partial class ImportPopulationSettingsView
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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.btnAddFile = new DevExpress.XtraEditors.SimpleButton();
         this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
         this.gridControl = new PKSim.UI.Views.Core.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.tbLog = new DevExpress.XtraEditors.MemoEdit();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.panelIndividualSelection = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemIndividual = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupImportPopulation = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonAdd = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
         this.splitContainer.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbLog.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelIndividualSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividual)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupImportPopulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.btnAddFile);
         this.layoutControl.Controls.Add(this.splitContainer);
         this.layoutControl.Controls.Add(this.lblDescription);
         this.layoutControl.Controls.Add(this.panelIndividualSelection);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(511, 605);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // btnAddFile
         // 
         this.btnAddFile.Location = new System.Drawing.Point(257, 59);
         this.btnAddFile.Name = "btnAddFile";
         this.btnAddFile.Size = new System.Drawing.Size(242, 22);
         this.btnAddFile.StyleController = this.layoutControl;
         this.btnAddFile.TabIndex = 8;
         this.btnAddFile.Text = "btnAddFile";
         // 
         // splitContainer
         // 
         this.splitContainer.Horizontal = false;
         this.splitContainer.Location = new System.Drawing.Point(12, 85);
         this.splitContainer.Name = "splitContainer";
         this.splitContainer.Panel1.Controls.Add(this.gridControl);
         this.splitContainer.Panel1.Text = "Panel1";
         this.splitContainer.Panel2.Controls.Add(this.tbLog);
         this.splitContainer.Panel2.Text = "Panel2";
         this.splitContainer.Size = new System.Drawing.Size(487, 508);
         this.splitContainer.SplitterPosition = 244;
         this.splitContainer.TabIndex = 7;
         this.splitContainer.Text = "splitContainerControl1";
         // 
         // gridControl
         // 
         this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.gridControl.Location = new System.Drawing.Point(0, 0);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(487, 244);
         this.gridControl.TabIndex = 0;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.GridControl = this.gridControl;
         this.gridView.Name = "gridView";
         // 
         // tbLog
         // 
         this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tbLog.Location = new System.Drawing.Point(0, 0);
         this.tbLog.Name = "tbLog";
         this.tbLog.Size = new System.Drawing.Size(487, 259);
         this.tbLog.TabIndex = 0;
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(12, 42);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.layoutControl;
         this.lblDescription.TabIndex = 5;
         this.lblDescription.Text = "lblDescription";
         // 
         // panelIndividualSelection
         // 
         this.panelIndividualSelection.Location = new System.Drawing.Point(113, 12);
         this.panelIndividualSelection.Name = "panelIndividualSelection";
         this.panelIndividualSelection.Size = new System.Drawing.Size(386, 26);
         this.panelIndividualSelection.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemIndividual,
            this.layoutItemDescription,
            this.layoutGroupImportPopulation});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(511, 605);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemIndividual
         // 
         this.layoutItemIndividual.Control = this.panelIndividualSelection;
         this.layoutItemIndividual.CustomizationFormText = "layoutItemIndividual";
         this.layoutItemIndividual.Location = new System.Drawing.Point(0, 0);
         this.layoutItemIndividual.MaxSize = new System.Drawing.Size(0, 30);
         this.layoutItemIndividual.MinSize = new System.Drawing.Size(205, 30);
         this.layoutItemIndividual.Name = "layoutItemIndividual";
         this.layoutItemIndividual.Size = new System.Drawing.Size(491, 30);
         this.layoutItemIndividual.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemIndividual.Text = "layoutItemIndividual";
         this.layoutItemIndividual.TextSize = new System.Drawing.Size(98, 13);
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.CustomizationFormText = "layoutControlItem1";
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 30);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(491, 17);
         this.layoutItemDescription.Text = "layoutItemDescription";
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextToControlDistance = 0;
         this.layoutItemDescription.TextVisible = false;
         // 
         // layoutGroupImportPopulation
         // 
         this.layoutGroupImportPopulation.CustomizationFormText = "layoutGroupImportPopulation";
         this.layoutGroupImportPopulation.GroupBordersVisible = false;
         this.layoutGroupImportPopulation.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutItemButtonAdd,
            this.emptySpaceItem});
         this.layoutGroupImportPopulation.Location = new System.Drawing.Point(0, 47);
         this.layoutGroupImportPopulation.Name = "layoutGroupImportPopulation";
         this.layoutGroupImportPopulation.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutGroupImportPopulation.Size = new System.Drawing.Size(491, 538);
         this.layoutGroupImportPopulation.Text = "layoutGroupImportPopulation";
         this.layoutGroupImportPopulation.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.splitContainer;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 26);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(491, 512);
         this.layoutControlItem2.Text = "layoutControlItem2";
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextToControlDistance = 0;
         this.layoutControlItem2.TextVisible = false;
         // 
         // layoutItemButtonAdd
         // 
         this.layoutItemButtonAdd.Control = this.btnAddFile;
         this.layoutItemButtonAdd.CustomizationFormText = "layoutControlItem3";
         this.layoutItemButtonAdd.Location = new System.Drawing.Point(245, 0);
         this.layoutItemButtonAdd.Name = "layoutControlItem3";
         this.layoutItemButtonAdd.Size = new System.Drawing.Size(246, 26);
         this.layoutItemButtonAdd.Text = "layoutControlItem3";
         this.layoutItemButtonAdd.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonAdd.TextToControlDistance = 0;
         this.layoutItemButtonAdd.TextVisible = false;
         // 
         // emptySpaceItem
         // 
         this.emptySpaceItem.AllowHotTrack = false;
         this.emptySpaceItem.CustomizationFormText = "emptySpaceItem";
         this.emptySpaceItem.Location = new System.Drawing.Point(0, 0);
         this.emptySpaceItem.Name = "emptySpaceItem";
         this.emptySpaceItem.Size = new System.Drawing.Size(245, 26);
         this.emptySpaceItem.Text = "emptySpaceItem";
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // ImportPopulationSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "ImportPopulationSettingsView";
         this.Size = new System.Drawing.Size(511, 605);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
         this.splitContainer.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbLog.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelIndividualSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividual)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupImportPopulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.PanelControl panelIndividualSelection;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemIndividual;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private DevExpress.XtraEditors.SplitContainerControl splitContainer;
      private PKSim.UI.Views.Core.UxGridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraEditors.SimpleButton btnAddFile;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonAdd;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private DevExpress.XtraEditors.MemoEdit tbLog;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupImportPopulation;
   }
}
