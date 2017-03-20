namespace PKSim.UI.Views.Parameters
{
   partial class TableParameterView
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
         this.layoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl();
         this.btnImportPoints = new DevExpress.XtraEditors.SimpleButton();
         this.gridValuePoints = new PKSim.UI.Views.Core.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.lblImportDescription = new DevExpress.XtraEditors.LabelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemTable = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemImportPoints = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.btnAddPoint = new DevExpress.XtraEditors.SimpleButton();
         this.layoutItemButtonAddPoint = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridValuePoints)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTable)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemImportPoints)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAddPoint)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.btnAddPoint);
         this.layoutControl1.Controls.Add(this.btnImportPoints);
         this.layoutControl1.Controls.Add(this.gridValuePoints);
         this.layoutControl1.Controls.Add(this.lblImportDescription);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(471, 470);
         this.layoutControl1.TabIndex = 0;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // btnImportPoints
         // 
         this.btnImportPoints.Location = new System.Drawing.Point(237, 2);
         this.btnImportPoints.Name = "btnImportPoints";
         this.btnImportPoints.Size = new System.Drawing.Size(88, 22);
         this.btnImportPoints.StyleController = this.layoutControl1;
         this.btnImportPoints.TabIndex = 5;
         this.btnImportPoints.Text = "btnImportPoints";
         // 
         // gridValuePoints
         // 
         this.gridValuePoints.Location = new System.Drawing.Point(2, 45);
         this.gridValuePoints.MainView = this.gridView;
         this.gridValuePoints.Name = "gridValuePoints";
         this.gridValuePoints.Size = new System.Drawing.Size(467, 423);
         this.gridValuePoints.TabIndex = 4;
         this.gridValuePoints.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridViewValuePoints
         // 
         this.gridView.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.GridControl = this.gridValuePoints;
         this.gridView.Name = "gridViewValuePoints";
         this.gridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridView.OptionsNavigation.AutoFocusNewRow = true;
         this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // lblImportDescription
         // 
         this.lblImportDescription.Location = new System.Drawing.Point(2, 28);
         this.lblImportDescription.Name = "lblImportDescription";
         this.lblImportDescription.Size = new System.Drawing.Size(95, 13);
         this.lblImportDescription.StyleController = this.layoutControl1;
         this.lblImportDescription.TabIndex = 6;
         this.lblImportDescription.Text = "lblImportDescription";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemTable,
            this.layoutItemButtonAddPoint,
            this.emptySpaceItem2,
            this.layoutItemImportPoints,
            this.layoutItemDescription});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(471, 470);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemTable
         // 
         this.layoutItemTable.Control = this.gridValuePoints;
         this.layoutItemTable.CustomizationFormText = "layoutControlItem1";
         this.layoutItemTable.Location = new System.Drawing.Point(0, 43);
         this.layoutItemTable.Name = "layoutItemTable";
         this.layoutItemTable.Size = new System.Drawing.Size(471, 427);
         this.layoutItemTable.Text = "layoutItemTable";
         this.layoutItemTable.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemTable.TextToControlDistance = 0;
         this.layoutItemTable.TextVisible = false;
         // 
         // layoutItemImportPoints
         // 
         this.layoutItemImportPoints.Control = this.btnImportPoints;
         this.layoutItemImportPoints.CustomizationFormText = "layoutControlItem2";
         this.layoutItemImportPoints.Location = new System.Drawing.Point(235, 0);
         this.layoutItemImportPoints.Name = "layoutItemImportPoints";
         this.layoutItemImportPoints.Size = new System.Drawing.Size(92, 26);
         this.layoutItemImportPoints.Text = "layoutItemImportPoints";
         this.layoutItemImportPoints.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemImportPoints.TextToControlDistance = 0;
         this.layoutItemImportPoints.TextVisible = false;
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblImportDescription;
         this.layoutItemDescription.CustomizationFormText = "layoutControlItem3";
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 26);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(471, 17);
         this.layoutItemDescription.Text = "layoutItemDescription";
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextToControlDistance = 0;
         this.layoutItemDescription.TextVisible = false;
         // 
         // btnAddPoint
         // 
         this.btnAddPoint.Location = new System.Drawing.Point(329, 2);
         this.btnAddPoint.Name = "btnAddPoint";
         this.btnAddPoint.Size = new System.Drawing.Size(140, 22);
         this.btnAddPoint.StyleController = this.layoutControl1;
         this.btnAddPoint.TabIndex = 7;
         this.btnAddPoint.Text = "btnAddPoint";
         // 
         // layoutItemButtonAddPoint
         // 
         this.layoutItemButtonAddPoint.Control = this.btnAddPoint;
         this.layoutItemButtonAddPoint.CustomizationFormText = "layoutItemButtonAddPoint";
         this.layoutItemButtonAddPoint.Location = new System.Drawing.Point(327, 0);
         this.layoutItemButtonAddPoint.Name = "layoutItemButtonAddPoint";
         this.layoutItemButtonAddPoint.Size = new System.Drawing.Size(144, 26);
         this.layoutItemButtonAddPoint.Text = "layoutItemButtonAddPoint";
         this.layoutItemButtonAddPoint.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonAddPoint.TextToControlDistance = 0;
         this.layoutItemButtonAddPoint.TextVisible = false;
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
         this.emptySpaceItem2.Location = new System.Drawing.Point(0, 0);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(235, 26);
         this.emptySpaceItem2.Text = "emptySpaceItem2";
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // TableParameterView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl1);
         this.Name = "TableParameterView";
         this.Size = new System.Drawing.Size(471, 470);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridValuePoints)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTable)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemImportPoints)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAddPoint)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl1;
      protected DevExpress.XtraEditors.LabelControl lblImportDescription;
      protected DevExpress.XtraEditors.SimpleButton btnImportPoints;
      private PKSim.UI.Views.Core.UxGridControl gridValuePoints;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTable;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemImportPoints;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      protected DevExpress.XtraEditors.SimpleButton btnAddPoint;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonAddPoint;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
   }
}
