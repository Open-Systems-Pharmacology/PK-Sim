namespace PKSim.UI.Views.Observers
{
   partial class ImportObserversView
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
         this.panelObservedDataDetails = new DevExpress.XtraEditors.PanelControl();
         this.gridControl = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.btnAddFile = new DevExpress.XtraEditors.SimpleButton();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemAddFile = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemGrid = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelObservedDataDetails)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddFile)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.panelObservedDataDetails);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Controls.Add(this.btnAddFile);
         this.layoutControl.Controls.Add(this.lblDescription);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(1136, 1300);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl";
         // 
         // panelObservedDataDetails
         // 
         this.panelObservedDataDetails.Location = new System.Drawing.Point(4, 573);
         this.panelObservedDataDetails.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.panelObservedDataDetails.Name = "panelObservedDataDetails";
         this.panelObservedDataDetails.Size = new System.Drawing.Size(1128, 723);
         this.panelObservedDataDetails.TabIndex = 9;
         // 
         // gridControl
         // 
         this.gridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
         this.gridControl.Location = new System.Drawing.Point(4, 89);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(1128, 476);
         this.gridControl.TabIndex = 8;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.AllowsFiltering = true;
         this.gridView.DetailHeight = 673;
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.FixedLineWidth = 4;
         this.gridView.GridControl = this.gridControl;
         this.gridView.MultiSelect = false;
         this.gridView.Name = "gridView";
         this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // btnAddFile
         // 
         this.btnAddFile.Location = new System.Drawing.Point(572, 37);
         this.btnAddFile.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.btnAddFile.Name = "btnAddFile";
         this.btnAddFile.Size = new System.Drawing.Size(560, 44);
         this.btnAddFile.StyleController = this.layoutControl;
         this.btnAddFile.TabIndex = 6;
         this.btnAddFile.Text = "btnAddFile";
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(4, 4);
         this.lblDescription.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(126, 25);
         this.lblDescription.StyleController = this.layoutControl;
         this.lblDescription.TabIndex = 4;
         this.lblDescription.Text = "lblDescription";
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemDescription,
            this.layoutItemAddFile,
            this.emptySpaceItem2,
            this.layoutItemGrid,
            this.layoutControlItem1});
         this.Root.Name = "Root";
         this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.Root.Size = new System.Drawing.Size(1136, 1300);
         this.Root.TextVisible = false;
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(1136, 33);
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextVisible = false;
         // 
         // layoutItemAddFile
         // 
         this.layoutItemAddFile.Control = this.btnAddFile;
         this.layoutItemAddFile.Location = new System.Drawing.Point(568, 33);
         this.layoutItemAddFile.Name = "layoutItemAddFile";
         this.layoutItemAddFile.Size = new System.Drawing.Size(568, 52);
         this.layoutItemAddFile.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemAddFile.TextVisible = false;
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.Location = new System.Drawing.Point(0, 33);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(568, 52);
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemGrid
         // 
         this.layoutItemGrid.Control = this.gridControl;
         this.layoutItemGrid.Location = new System.Drawing.Point(0, 85);
         this.layoutItemGrid.Name = "layoutItemGrid";
         this.layoutItemGrid.Size = new System.Drawing.Size(1136, 484);
         this.layoutItemGrid.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemGrid.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.panelObservedDataDetails;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 569);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(1136, 731);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // ImportObserversView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
         this.Name = "ImportObserversView";
         this.Size = new System.Drawing.Size(1136, 1300);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelObservedDataDetails)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddFile)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private OSPSuite.UI.Controls.UxGridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraEditors.SimpleButton btnAddFile;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemAddFile;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGrid;
      private DevExpress.XtraEditors.PanelControl panelObservedDataDetails;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
   }
}
