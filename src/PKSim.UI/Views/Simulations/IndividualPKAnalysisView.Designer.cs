using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Simulations
{
   partial class IndividualPKAnalysisView
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
         _binder.Dispose();
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl(); 
         this.panelControlIndividualAnalysis = new DevExpress.XtraEditors.PanelControl();
         this.panelControlGlobalAnalysis = new DevExpress.XtraEditors.PanelControl();
         this.btnExportToExcel = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemExportToExcel = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemGlobalPKAnalysis = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemIndividualPKAnalysis = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.splitterItem = new DevExpress.XtraLayout.SplitterItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelControlIndividualAnalysis)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControlGlobalAnalysis)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportToExcel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGlobalPKAnalysis)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividualPKAnalysis)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitterItem)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.panelControlIndividualAnalysis);
         this.layoutControl.Controls.Add(this.panelControlGlobalAnalysis);
         this.layoutControl.Controls.Add(this.btnExportToExcel);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1306, 244, 250, 350);
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(603, 392);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelControlIndividualAnalysis
         // 
         this.panelControlIndividualAnalysis.Location = new System.Drawing.Point(154, 153);
         this.panelControlIndividualAnalysis.Name = "panelControlIndividualAnalysis";
         this.panelControlIndividualAnalysis.Size = new System.Drawing.Size(447, 237);
         this.panelControlIndividualAnalysis.TabIndex = 6;
         // 
         // panelControlGlobalAnalysis
         // 
         this.panelControlGlobalAnalysis.Location = new System.Drawing.Point(154, 28);
         this.panelControlGlobalAnalysis.Name = "panelControlGlobalAnalysis";
         this.panelControlGlobalAnalysis.Size = new System.Drawing.Size(447, 116);
         this.panelControlGlobalAnalysis.TabIndex = 5;
         // 
         // btnExportToExcel
         // 
         this.btnExportToExcel.Location = new System.Drawing.Point(303, 2);
         this.btnExportToExcel.Name = "btnExportToExcel";
         this.btnExportToExcel.Size = new System.Drawing.Size(298, 22);
         this.btnExportToExcel.StyleController = this.layoutControl;
         this.btnExportToExcel.TabIndex = 4;
         this.btnExportToExcel.Text = "btnExportToExcel";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "Root";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemExportToExcel,
            this.layoutItemGlobalPKAnalysis,
            this.layoutItemIndividualPKAnalysis,
            this.emptySpaceItem1,
            this.splitterItem});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "Root";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(603, 392);
         this.layoutControlGroup1.Text = "Root";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutItemExportToExcel.Control = this.btnExportToExcel;
         this.layoutItemExportToExcel.CustomizationFormText = "layoutControlItem1";
         this.layoutItemExportToExcel.Location = new System.Drawing.Point(301, 0);
         this.layoutItemExportToExcel.Name = "layoutItemExportToExcel";
         this.layoutItemExportToExcel.Size = new System.Drawing.Size(302, 26);
         this.layoutItemExportToExcel.Text = "layoutControlItem1";
         this.layoutItemExportToExcel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemExportToExcel.TextToControlDistance = 0;
         this.layoutItemExportToExcel.TextVisible = false;
         // 
         // layoutItemGlobalPKAnalysis
         // 
         this.layoutItemGlobalPKAnalysis.Control = this.panelControlGlobalAnalysis;
         this.layoutItemGlobalPKAnalysis.CustomizationFormText = "layoutItemGlobalPKAnalysis";
         this.layoutItemGlobalPKAnalysis.Location = new System.Drawing.Point(0, 26);
         this.layoutItemGlobalPKAnalysis.Name = "layoutItemGlobalPKAnalysis";
         this.layoutItemGlobalPKAnalysis.Size = new System.Drawing.Size(603, 120);
         this.layoutItemGlobalPKAnalysis.Text = "layoutItemGlobalPKAnalysis";
         this.layoutItemGlobalPKAnalysis.TextSize = new System.Drawing.Size(149, 13);
         // 
         // layoutItemIndividualPKAnalysis
         // 
         this.layoutItemIndividualPKAnalysis.Control = this.panelControlIndividualAnalysis;
         this.layoutItemIndividualPKAnalysis.CustomizationFormText = "layoutItemIndividualPKAnalysis";
         this.layoutItemIndividualPKAnalysis.Location = new System.Drawing.Point(0, 151);
         this.layoutItemIndividualPKAnalysis.Name = "layoutItemIndividualPKAnalysis";
         this.layoutItemIndividualPKAnalysis.Size = new System.Drawing.Size(603, 241);
         this.layoutItemIndividualPKAnalysis.Text = "layoutItemIndividualPKAnalysis";
         this.layoutItemIndividualPKAnalysis.TextSize = new System.Drawing.Size(149, 13);
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(301, 26);
         this.emptySpaceItem1.Text = "emptySpaceItem1";
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // splitterItem
         // 
         this.splitterItem.AllowHotTrack = true;
         this.splitterItem.CustomizationFormText = "splitterItem";
         this.splitterItem.Location = new System.Drawing.Point(0, 146);
         this.splitterItem.Name = "splitterItem";
         this.splitterItem.Size = new System.Drawing.Size(603, 5);
         // 
         // IndividualPKAnalysisView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "IndividualPKAnalysisView";
         this.Size = new System.Drawing.Size(603, 392);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelControlIndividualAnalysis)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControlGlobalAnalysis)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportToExcel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGlobalPKAnalysis)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividualPKAnalysis)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitterItem)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl panelControlIndividualAnalysis;
      private DevExpress.XtraEditors.PanelControl panelControlGlobalAnalysis;
      private DevExpress.XtraEditors.SimpleButton btnExportToExcel;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExportToExcel;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGlobalPKAnalysis;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemIndividualPKAnalysis;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.SplitterItem splitterItem;

   }
}
