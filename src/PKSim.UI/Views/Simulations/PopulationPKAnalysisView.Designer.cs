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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.labelControlGlobalPKAnalysisDescription = new DevExpress.XtraEditors.LabelControl();
         this.populationPKAnalysisTabControl = new DevExpress.XtraTab.XtraTabControl();
         this.pageAggregatedPKValues = new DevExpress.XtraTab.XtraTabPage();
         this.populationPKAnalysisPanelAggregatedPKValues = new DevExpress.XtraEditors.PanelControl();
         this.pageIndividualPKValues = new DevExpress.XtraTab.XtraTabPage();
         this.populationPKAnalysisPanelIndividualPKValues = new DevExpress.XtraEditors.PanelControl();
         this.btnExportToExcel = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemExportToExcel = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         this.splitter = new DevExpress.XtraLayout.SplitterItem();
         this.layoutItemPopulationPKAnalysisTab = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItemGlobalPKAnalysisDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItemGlobalPKAnalysis = new DevExpress.XtraLayout.LayoutControlItem();
         this.globalPKParametersPanelControl = new OSPSuite.UI.Controls.UxPanelControl();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisTabControl)).BeginInit();
         this.populationPKAnalysisTabControl.SuspendLayout();
         this.pageAggregatedPKValues.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelAggregatedPKValues)).BeginInit();
         this.pageIndividualPKValues.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelIndividualPKValues)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportToExcel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulationPKAnalysisTab)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGlobalPKAnalysisDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGlobalPKAnalysis)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.globalPKParametersPanelControl)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.globalPKParametersPanelControl);
         this.layoutControl.Controls.Add(this.labelControlGlobalPKAnalysisDescription);
         this.layoutControl.Controls.Add(this.populationPKAnalysisTabControl);
         this.layoutControl.Controls.Add(this.btnExportToExcel);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3693, -142, 812, 500);
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(555, 476);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
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
         // populationPKAnalysisTabControl
         // 
         this.populationPKAnalysisTabControl.Location = new System.Drawing.Point(2, 177);
         this.populationPKAnalysisTabControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.populationPKAnalysisTabControl.Name = "populationPKAnalysisTabControl";
         this.populationPKAnalysisTabControl.SelectedTabPage = this.pageIndividualPKValues;
         this.populationPKAnalysisTabControl.Size = new System.Drawing.Size(551, 297);
         this.populationPKAnalysisTabControl.TabIndex = 0;
         this.populationPKAnalysisTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.pageIndividualPKValues,
            this.pageAggregatedPKValues,
         });
         // 
         // pageOnCurves
         // 
         this.pageAggregatedPKValues.Controls.Add(this.populationPKAnalysisPanelAggregatedPKValues);
         this.pageAggregatedPKValues.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.pageAggregatedPKValues.Name = "pageAggregatedPKValues";
         this.pageAggregatedPKValues.Size = new System.Drawing.Size(549, 272);
         this.pageAggregatedPKValues.Text = "pageAggregatedPKValues";
         // 
         // populationPKAnalysisPanelAggregatedPKValues
         // 
         this.populationPKAnalysisPanelAggregatedPKValues.AutoSize = true;
         this.populationPKAnalysisPanelAggregatedPKValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.populationPKAnalysisPanelAggregatedPKValues.Dock = System.Windows.Forms.DockStyle.Fill;
         this.populationPKAnalysisPanelAggregatedPKValues.Location = new System.Drawing.Point(0, 0);
         this.populationPKAnalysisPanelAggregatedPKValues.Name = "populationPKAnalysisPanelAggregatedPKValues";
         this.populationPKAnalysisPanelAggregatedPKValues.Size = new System.Drawing.Size(549, 272);
         this.populationPKAnalysisPanelAggregatedPKValues.TabIndex = 6;
         // 
         // pageOnIndividuals
         // 
         this.pageIndividualPKValues.Controls.Add(this.populationPKAnalysisPanelIndividualPKValues);
         this.pageIndividualPKValues.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.pageIndividualPKValues.Name = "pageIndividualPKValues";
         this.pageIndividualPKValues.Size = new System.Drawing.Size(549, 272);
         this.pageIndividualPKValues.Text = "pageOnIndividuals";
         // 
         // populationPKAnalysisPanelIndividualPKValues
         // 
         this.populationPKAnalysisPanelIndividualPKValues.AutoSize = true;
         this.populationPKAnalysisPanelIndividualPKValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.populationPKAnalysisPanelIndividualPKValues.Dock = System.Windows.Forms.DockStyle.Fill;
         this.populationPKAnalysisPanelIndividualPKValues.Location = new System.Drawing.Point(0, 0);
         this.populationPKAnalysisPanelIndividualPKValues.Name = "populationPKAnalysisPanelIndividualPKValues";
         this.populationPKAnalysisPanelIndividualPKValues.Size = new System.Drawing.Size(549, 272);
         this.populationPKAnalysisPanelIndividualPKValues.TabIndex = 8;
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
            this.splitter,
            this.layoutItemPopulationPKAnalysisTab,
            this.layoutControlItemGlobalPKAnalysisDescription,
            this.layoutControlItemGlobalPKAnalysis});
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
         // splitter
         // 
         this.splitter.AllowHotTrack = true;
         this.splitter.Location = new System.Drawing.Point(0, 165);
         this.splitter.Name = "splitter";
         this.splitter.Size = new System.Drawing.Size(555, 10);
         // 
         // layoutItemPopulationPKAnalysisTab
         // 
         this.layoutItemPopulationPKAnalysisTab.Control = this.populationPKAnalysisTabControl;
         this.layoutItemPopulationPKAnalysisTab.Location = new System.Drawing.Point(0, 175);
         this.layoutItemPopulationPKAnalysisTab.Name = "layoutItemPopulationPKAnalysisTab";
         this.layoutItemPopulationPKAnalysisTab.Size = new System.Drawing.Size(555, 301);
         this.layoutItemPopulationPKAnalysisTab.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemPopulationPKAnalysisTab.TextVisible = false;
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
         // layoutControlItemGlobalPKAnalysis
         // 
         this.layoutControlItemGlobalPKAnalysis.Control = this.globalPKParametersPanelControl;
         this.layoutControlItemGlobalPKAnalysis.Location = new System.Drawing.Point(0, 43);
         this.layoutControlItemGlobalPKAnalysis.Name = "layoutControlItemGlobalPKAnalysis";
         this.layoutControlItemGlobalPKAnalysis.Size = new System.Drawing.Size(555, 122);
         this.layoutControlItemGlobalPKAnalysis.TextSize = new System.Drawing.Size(167, 13);
         // 
         // globalPKParametersPanelControl
         // 
         this.globalPKParametersPanelControl.Location = new System.Drawing.Point(181, 45);
         this.globalPKParametersPanelControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.globalPKParametersPanelControl.Name = "globalPKParametersPanelControl";
         this.globalPKParametersPanelControl.Size = new System.Drawing.Size(372, 118);
         this.globalPKParametersPanelControl.TabIndex = 4;
         // 
         // PopulationPKAnalysisView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.Name = "PopulationPKAnalysisView";
         this.Size = new System.Drawing.Size(555, 476);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         this.layoutControl.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisTabControl)).EndInit();
         this.populationPKAnalysisTabControl.ResumeLayout(false);
         this.pageAggregatedPKValues.ResumeLayout(false);
         this.pageAggregatedPKValues.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelAggregatedPKValues)).EndInit();
         this.pageIndividualPKValues.ResumeLayout(false);
         this.pageIndividualPKValues.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelIndividualPKValues)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportToExcel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulationPKAnalysisTab)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGlobalPKAnalysisDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGlobalPKAnalysis)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.globalPKParametersPanelControl)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.SimpleButton btnExportToExcel;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExportToExcel;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private DevExpress.XtraEditors.PanelControl populationPKAnalysisPanelAggregatedPKValues;
      private DevExpress.XtraEditors.PanelControl populationPKAnalysisPanelIndividualPKValues;
      private DevExpress.XtraLayout.SplitterItem splitter;
      private DevExpress.XtraTab.XtraTabControl populationPKAnalysisTabControl;
      private DevExpress.XtraTab.XtraTabPage pageAggregatedPKValues;
      private DevExpress.XtraTab.XtraTabPage pageIndividualPKValues;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPopulationPKAnalysisTab;
      private DevExpress.XtraEditors.LabelControl labelControlGlobalPKAnalysisDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemGlobalPKAnalysisDescription;
      private OSPSuite.UI.Controls.UxPanelControl globalPKParametersPanelControl;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemGlobalPKAnalysis;
   }
}