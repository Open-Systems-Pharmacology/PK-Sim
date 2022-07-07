﻿using PKSim.Assets;
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
         this.populationPKAnalysisXtraTabControl = new DevExpress.XtraTab.XtraTabControl();
         this.xtraTabPage3 = new DevExpress.XtraTab.XtraTabPage();
         this.populationPKAnalysisPanelOnCurve = new DevExpress.XtraEditors.PanelControl();
         this.populationPKAnalysisPanelOnIndividuals = new DevExpress.XtraEditors.PanelControl();
         this.xtraTabPage4 = new DevExpress.XtraTab.XtraTabPage();
         this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
         this.globalPKParametersPanelControl = new OSPSuite.UI.Controls.UxPanelControl();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.btnExportToExcel = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemExportToExcel = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutControlItemGlobalPKAnalysis = new DevExpress.XtraLayout.LayoutControlItem();
         this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisXtraTabControl)).BeginInit();
         this.populationPKAnalysisXtraTabControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelOnCurve)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelOnIndividuals)).BeginInit();
         this.xtraTabPage4.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.globalPKParametersPanelControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportToExcel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGlobalPKAnalysis)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.populationPKAnalysisXtraTabControl);
         this.layoutControl.Controls.Add(this.layoutControl1);
         this.layoutControl.Controls.Add(this.btnExportToExcel);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3693, -142, 812, 500);
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(647, 586);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // populationPKAnalysisXtraTabControl
         // 
         this.populationPKAnalysisXtraTabControl.Location = new System.Drawing.Point(2, 218);
         this.populationPKAnalysisXtraTabControl.Name = "populationPKAnalysisXtraTabControl";
         this.populationPKAnalysisXtraTabControl.SelectedTabPage = this.xtraTabPage3;
         this.populationPKAnalysisXtraTabControl.Size = new System.Drawing.Size(643, 366);
         this.populationPKAnalysisXtraTabControl.TabIndex = 0;
         this.populationPKAnalysisXtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage3,
            this.xtraTabPage4});
         // 
         // populationPKAnalysisPanelOnCurve
         // 
         this.populationPKAnalysisPanelOnCurve.AutoSize = true;
         this.populationPKAnalysisPanelOnCurve.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.populationPKAnalysisPanelOnCurve.Dock = System.Windows.Forms.DockStyle.Fill;
         this.populationPKAnalysisPanelOnCurve.Location = new System.Drawing.Point(0, 0);
         this.populationPKAnalysisPanelOnCurve.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.populationPKAnalysisPanelOnCurve.Name = "populationPKAnalysisPanel";
         this.populationPKAnalysisPanelOnCurve.Size = new System.Drawing.Size(641, 336);
         this.populationPKAnalysisPanelOnCurve.TabIndex = 6;
         // 
         // populationPKAnalysisPanelOnIndividuals
         // 
         this.populationPKAnalysisPanelOnIndividuals.AutoSize = true;
         this.populationPKAnalysisPanelOnIndividuals.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.populationPKAnalysisPanelOnIndividuals.Dock = System.Windows.Forms.DockStyle.Fill;
         this.populationPKAnalysisPanelOnIndividuals.Location = new System.Drawing.Point(0, 0);
         this.populationPKAnalysisPanelOnIndividuals.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.populationPKAnalysisPanelOnIndividuals.Name = "populationPKAnalysisPanel";
         this.populationPKAnalysisPanelOnIndividuals.Size = new System.Drawing.Size(641, 336);
         this.populationPKAnalysisPanelOnIndividuals.TabIndex = 8;
         // 
         // xtraTabPage3
         // 
         this.xtraTabPage3.Controls.Add(this.populationPKAnalysisPanelOnCurve); 
         this.xtraTabPage3.Name = "xtraTabPage3";
         this.xtraTabPage3.Size = new System.Drawing.Size(641, 336);
         this.xtraTabPage3.Text = PKSimConstants.PKAnalysis.OnCurve;
         this.xtraTabPage3.Tooltip = PKSimConstants.PKAnalysis.OnCurveTooltip;
         // 
         // xtraTabPage4
         // 
         this.xtraTabPage4.Controls.Add(this.populationPKAnalysisPanelOnIndividuals);
         this.xtraTabPage4.Name = "xtraTabPage4";
         this.xtraTabPage4.Size = new System.Drawing.Size(641, 336);
         this.xtraTabPage4.Text = PKSimConstants.PKAnalysis.OnIndividuals;
         this.xtraTabPage4.Tooltip = PKSimConstants.PKAnalysis.OnIndividualsTooltip;
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.globalPKParametersPanelControl);
         this.layoutControl1.Location = new System.Drawing.Point(210, 33);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(874, 0, 812, 500);
         this.layoutControl1.Root = this.Root;
         this.layoutControl1.Size = new System.Drawing.Size(435, 169);
         this.layoutControl1.TabIndex = 7;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // globalPKParametersPanelControl
         // 
         this.globalPKParametersPanelControl.Location = new System.Drawing.Point(2, 2);
         this.globalPKParametersPanelControl.Name = "globalPKParametersPanelControl";
         this.globalPKParametersPanelControl.Size = new System.Drawing.Size(431, 165);
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
         this.Root.Size = new System.Drawing.Size(435, 169);
         this.Root.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.globalPKParametersPanelControl;
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(435, 169);
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextVisible = false;
         // 
         // btnExportToExcel
         // 
         this.btnExportToExcel.Location = new System.Drawing.Point(325, 2);
         this.btnExportToExcel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.btnExportToExcel.Name = "btnExportToExcel";
         this.btnExportToExcel.Size = new System.Drawing.Size(320, 27);
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
            this.splitterItem1,
            this.layoutControlItem1});
         this.layoutControlGroup1.Name = "Root";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(647, 586);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemExportToExcel
         // 
         this.layoutItemExportToExcel.Control = this.btnExportToExcel;
         this.layoutItemExportToExcel.CustomizationFormText = "layoutControlItem2";
         this.layoutItemExportToExcel.Location = new System.Drawing.Point(323, 0);
         this.layoutItemExportToExcel.Name = "layoutItemExportToExcel";
         this.layoutItemExportToExcel.Size = new System.Drawing.Size(324, 31);
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
         this.emptySpaceItem.Size = new System.Drawing.Size(323, 31);
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutControlItemGlobalPKAnalysis
         // 
         this.layoutControlItemGlobalPKAnalysis.Control = this.layoutControl1;
         this.layoutControlItemGlobalPKAnalysis.Location = new System.Drawing.Point(0, 31);
         this.layoutControlItemGlobalPKAnalysis.Name = "layoutControlItemGlobalPKAnalysis";
         this.layoutControlItemGlobalPKAnalysis.Size = new System.Drawing.Size(647, 173);
         this.layoutControlItemGlobalPKAnalysis.TextSize = new System.Drawing.Size(196, 16);
         // 
         // splitterItem1
         // 
         this.splitterItem1.AllowHotTrack = true;
         this.splitterItem1.Location = new System.Drawing.Point(0, 204);
         this.splitterItem1.Name = "splitterItem1";
         this.splitterItem1.Size = new System.Drawing.Size(647, 12);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.populationPKAnalysisXtraTabControl;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 216);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(647, 370);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // PopulationPKAnalysisView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
         this.Name = "PopulationPKAnalysisView";
         this.Size = new System.Drawing.Size(647, 586);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisXtraTabControl)).EndInit();
         this.populationPKAnalysisXtraTabControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelOnCurve)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.populationPKAnalysisPanelOnIndividuals)).EndInit();
         this.xtraTabPage4.ResumeLayout(false);
         this.xtraTabPage4.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.globalPKParametersPanelControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExportToExcel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGlobalPKAnalysis)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.SimpleButton btnExportToExcel;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExportToExcel;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private DevExpress.XtraEditors.PanelControl populationPKAnalysisPanelOnCurve;
      private DevExpress.XtraEditors.PanelControl populationPKAnalysisPanelOnIndividuals;
      private DevExpress.XtraLayout.LayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemGlobalPKAnalysis;
      private DevExpress.XtraLayout.SplitterItem splitterItem1;
      private OSPSuite.UI.Controls.UxPanelControl globalPKParametersPanelControl;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraTab.XtraTabControl populationPKAnalysisXtraTabControl;
      private DevExpress.XtraTab.XtraTabPage xtraTabPage3;
      private DevExpress.XtraTab.XtraTabPage xtraTabPage4;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
   }
}
