using PKSim.Assets;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Simulations
{
   partial class PopulationPKAnalysisView
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
         _screenBinder.Dispose();
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.labelControlGlobalPKAnalysisDescription = new DevExpress.XtraEditors.LabelControl();
         this.populationPKAnalysisXtraTabControl = new DevExpress.XtraTab.XtraTabControl();
         this.pageOnCurves = new DevExpress.XtraTab.XtraTabPage();
         this.populationPKAnalysisPanelOnCurve = new DevExpress.XtraEditors.PanelControl();
         this.pageOnIndividuals = new DevExpress.XtraTab.XtraTabPage();
         this.populationPKAnalysisPanelOnIndividuals = new DevExpress.XtraEditors.PanelControl();
         this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
         this.globalPKParametersPanelControl = new OSPSuite.UI.Controls.UxPanelControl();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.btnExportToExcel = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemExportToExcel = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutControlItemGlobalPKAnalysis = new DevExpress.XtraLayout.LayoutControlItem();
         this.splitter = new DevExpress.XtraLayout.SplitterItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItemGlobalPKAnalysisDescription = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisXtraTabControl)).BeginInit();
         this.populationPKAnalysisXtraTabControl.SuspendLayout();
         this.pageOnCurves.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelOnCurve)).BeginInit();
         this.pageOnIndividuals.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelOnIndividuals)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.globalPKParametersPanelControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportToExcel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGlobalPKAnalysis)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGlobalPKAnalysisDescription)).BeginInit();
         this.SuspendLayout();
         // 
         // labelControlGlobalPKAnalysisDescription
         // 
         this.labelControlGlobalPKAnalysisDescription.Location = new System.Drawing.Point(2, 28);
         this.labelControlGlobalPKAnalysisDescription.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.labelControlGlobalPKAnalysisDescription.Name = "labelControlGlobalPKAnalysisDescription";
         this.labelControlGlobalPKAnalysisDescription.Size = new System.Drawing.Size(190, 13);
         this.labelControlGlobalPKAnalysisDescription.StyleController = this.layoutControl;
         this.labelControlGlobalPKAnalysisDescription.TabIndex = 8;
         this.labelControlGlobalPKAnalysisDescription.Text = "labelControlGlobalPKAnalysisDescription";
         // 
         // populationPKAnalysisXtraTabControl
         // 
         this.populationPKAnalysisXtraTabControl.Location = new System.Drawing.Point(2, 177);
         this.populationPKAnalysisXtraTabControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.populationPKAnalysisXtraTabControl.Name = "populationPKAnalysisXtraTabControl";
         this.populationPKAnalysisXtraTabControl.SelectedTabPage = this.pageOnCurves;
         this.populationPKAnalysisXtraTabControl.Size = new System.Drawing.Size(551, 297);
         this.populationPKAnalysisXtraTabControl.TabIndex = 0;
         this.populationPKAnalysisXtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.pageOnCurves,
            this.pageOnIndividuals});
         // 
         // pageOnCurves
         // 
         this.pageOnCurves.Controls.Add(this.populationPKAnalysisPanelOnCurve);
         this.pageOnCurves.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.pageOnCurves.Name = "pageOnCurves";
         this.pageOnCurves.Size = new System.Drawing.Size(549, 272);
         this.pageOnCurves.Text = "pageOnCurves";
         // 
         // populationPKAnalysisPanelOnCurve
         // 
         this.populationPKAnalysisPanelOnCurve.AutoSize = true;
         this.populationPKAnalysisPanelOnCurve.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.populationPKAnalysisPanelOnCurve.Dock = System.Windows.Forms.DockStyle.Fill;
         this.populationPKAnalysisPanelOnCurve.Location = new System.Drawing.Point(0, 0);
         this.populationPKAnalysisPanelOnCurve.Name = "populationPKAnalysisPanelOnCurve";
         this.populationPKAnalysisPanelOnCurve.Size = new System.Drawing.Size(549, 272);
         this.populationPKAnalysisPanelOnCurve.TabIndex = 6;
         // 
         // pageOnIndividuals
         // 
         this.pageOnIndividuals.Controls.Add(this.populationPKAnalysisPanelOnIndividuals);
         this.pageOnIndividuals.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.pageOnIndividuals.Name = "pageOnIndividuals";
         this.pageOnIndividuals.Size = new System.Drawing.Size(549, 272);
         this.pageOnIndividuals.Text = "pageOnIndividuals";
         // 
         // populationPKAnalysisPanelOnIndividuals
         // 
         this.populationPKAnalysisPanelOnIndividuals.AutoSize = true;
         this.populationPKAnalysisPanelOnIndividuals.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.populationPKAnalysisPanelOnIndividuals.Dock = System.Windows.Forms.DockStyle.Fill;
         this.populationPKAnalysisPanelOnIndividuals.Location = new System.Drawing.Point(0, 0);
         this.populationPKAnalysisPanelOnIndividuals.Name = "populationPKAnalysisPanelOnIndividuals";
         this.populationPKAnalysisPanelOnIndividuals.Size = new System.Drawing.Size(549, 272);
         this.populationPKAnalysisPanelOnIndividuals.TabIndex = 8;
         // 
         // layoutControl1
         // 
         this.layoutControl.Controls.Add(this.globalPKParametersPanelControl);
         this.layoutControl.Location = new System.Drawing.Point(177, 43);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(874, 0, 812, 500);
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(378, 122);
         this.layoutControl.TabIndex = 7;
         this.layoutControl.Text = "layoutControl1";
         // 
         // globalPKParametersPanelControl
         // 
         this.globalPKParametersPanelControl.Location = new System.Drawing.Point(2, 2);
         this.globalPKParametersPanelControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.globalPKParametersPanelControl.Name = "globalPKParametersPanelControl";
         this.globalPKParametersPanelControl.Size = new System.Drawing.Size(374, 118);
         this.globalPKParametersPanelControl.TabIndex = 4;
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2});
         this.Root.Name = "Root";
         this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.Root.Size = new System.Drawing.Size(378, 122);
         this.Root.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.globalPKParametersPanelControl;
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(378, 122);
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextVisible = false;
         // 
         // btnExportToExcel
         // 
         this.btnExportToExcel.Location = new System.Drawing.Point(279, 2);
         this.btnExportToExcel.Name = "btnExportToExcel";
         this.btnExportToExcel.Size = new System.Drawing.Size(274, 22);
         this.btnExportToExcel.StyleController = this.layoutControl;
         this.btnExportToExcel.TabIndex = 5;
         this.btnExportToExcel.Text = "btnExportToExcel";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemExportToExcel,
            this.emptySpaceItem,
            this.layoutControlItemGlobalPKAnalysis,
            this.splitter,
            this.layoutControlItem1,
            this.layoutControlItemGlobalPKAnalysisDescription});
         this.layoutControlGroup1.Name = "Root";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(555, 476);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemExportToExcel
         // 
         this.layoutItemExportToExcel.Control = this.btnExportToExcel;
         this.layoutItemExportToExcel.CustomizationFormText = "layoutControlItem2";
         this.layoutItemExportToExcel.Location = new System.Drawing.Point(277, 0);
         this.layoutItemExportToExcel.Name = "layoutItemExportToExcel";
         this.layoutItemExportToExcel.Size = new System.Drawing.Size(278, 26);
         this.layoutItemExportToExcel.Text = "layoutControlItem2";
         this.layoutItemExportToExcel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemExportToExcel.TextVisible = false;
         // 
         // emptySpaceItem
         // 
         this.emptySpaceItem.AllowHotTrack = false;
         this.emptySpaceItem.CustomizationFormText = "emptySpaceItem";
         this.emptySpaceItem.Location = new System.Drawing.Point(0, 0);
         this.emptySpaceItem.Name = "emptySpaceItem";
         this.emptySpaceItem.Size = new System.Drawing.Size(277, 26);
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutControlItemGlobalPKAnalysis
         // 
         this.layoutControlItemGlobalPKAnalysis.Control = this.layoutControl;
         this.layoutControlItemGlobalPKAnalysis.Location = new System.Drawing.Point(0, 43);
         this.layoutControlItemGlobalPKAnalysis.Name = "layoutControlItemGlobalPKAnalysis";
         this.layoutControlItemGlobalPKAnalysis.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlItemGlobalPKAnalysis.Size = new System.Drawing.Size(555, 122);
         this.layoutControlItemGlobalPKAnalysis.TextSize = new System.Drawing.Size(167, 13);
         // 
         // splitter
         // 
         this.splitter.AllowHotTrack = true;
         this.splitter.Location = new System.Drawing.Point(0, 165);
         this.splitter.Name = "splitter";
         this.splitter.Size = new System.Drawing.Size(555, 10);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.populationPKAnalysisXtraTabControl;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 175);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(555, 301);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutControlItemGlobalPKAnalysisDescription
         // 
         this.layoutControlItemGlobalPKAnalysisDescription.Control = this.labelControlGlobalPKAnalysisDescription;
         this.layoutControlItemGlobalPKAnalysisDescription.Location = new System.Drawing.Point(0, 26);
         this.layoutControlItemGlobalPKAnalysisDescription.Name = "layoutControlItemGlobalPKAnalysisDescription";
         this.layoutControlItemGlobalPKAnalysisDescription.Size = new System.Drawing.Size(555, 17);
         this.layoutControlItemGlobalPKAnalysisDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItemGlobalPKAnalysisDescription.TextVisible = false;
         // 
         // PopulationPKAnalysisView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.Name = "PopulationPKAnalysisView";
         this.Size = new System.Drawing.Size(555, 476);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisXtraTabControl)).EndInit();
         this.populationPKAnalysisXtraTabControl.ResumeLayout(false);
         this.pageOnCurves.ResumeLayout(false);
         this.pageOnCurves.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelOnCurve)).EndInit();
         this.pageOnIndividuals.ResumeLayout(false);
         this.pageOnIndividuals.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelOnIndividuals)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.globalPKParametersPanelControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportToExcel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGlobalPKAnalysis)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGlobalPKAnalysisDescription)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.SimpleButton btnExportToExcel;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExportToExcel;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private DevExpress.XtraEditors.PanelControl populationPKAnalysisPanelOnCurve;
      private DevExpress.XtraEditors.PanelControl populationPKAnalysisPanelOnIndividuals;
      private DevExpress.XtraLayout.LayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemGlobalPKAnalysis;
      private DevExpress.XtraLayout.SplitterItem splitter;
      private OSPSuite.UI.Controls.UxPanelControl globalPKParametersPanelControl;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraTab.XtraTabControl populationPKAnalysisXtraTabControl;
      private DevExpress.XtraTab.XtraTabPage pageOnCurves;
      private DevExpress.XtraTab.XtraTabPage pageOnIndividuals;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraEditors.LabelControl labelControlGlobalPKAnalysisDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemGlobalPKAnalysisDescription;
   }
}
