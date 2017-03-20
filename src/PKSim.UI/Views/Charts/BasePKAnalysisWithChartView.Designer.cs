namespace PKSim.UI.Views.Charts
{
   abstract partial class BasePKAnalysisWithChartView
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
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.btnSwithPKAnalysisPlot = new DevExpress.XtraEditors.SimpleButton();
         this.analysisPanel = new DevExpress.XtraEditors.PanelControl();
         this.chartPanel = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.chartControlItem = new DevExpress.XtraLayout.LayoutControlItem();
         this.analysisControlItem = new DevExpress.XtraLayout.LayoutControlItem();
         this.buttonControlItem = new DevExpress.XtraLayout.LayoutControlItem();
         this.viewSplitter = new DevExpress.XtraLayout.SplitterItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.analysisPanel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chartPanel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chartControlItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.analysisControlItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.buttonControlItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.viewSplitter)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.btnSwithPKAnalysisPlot);
         this.layoutControl.Controls.Add(this.analysisPanel);
         this.layoutControl.Controls.Add(this.chartPanel);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(597, 533);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // btnSwithPKAnalysisPlot
         // 
         this.btnSwithPKAnalysisPlot.Location = new System.Drawing.Point(2, 509);
         this.btnSwithPKAnalysisPlot.Name = "btnSwithPKAnalysisPlot";
         this.btnSwithPKAnalysisPlot.Size = new System.Drawing.Size(593, 22);
         this.btnSwithPKAnalysisPlot.StyleController = this.layoutControl;
         this.btnSwithPKAnalysisPlot.TabIndex = 0;
         this.btnSwithPKAnalysisPlot.Text = "btnSwithPKAnalysisPlot";
         // 
         // analysisPanel
         // 
         this.analysisPanel.Location = new System.Drawing.Point(2, 2);
         this.analysisPanel.Name = "analysisPanel";
         this.analysisPanel.Size = new System.Drawing.Size(196, 503);
         this.analysisPanel.TabIndex = 0;
         // 
         // chartPanel
         // 
         this.chartPanel.Location = new System.Drawing.Point(207, 2);
         this.chartPanel.Margin = new System.Windows.Forms.Padding(0);
         this.chartPanel.Name = "chartPanel";
         this.chartPanel.Size = new System.Drawing.Size(388, 503);
         this.chartPanel.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.chartControlItem,
            this.analysisControlItem,
            this.buttonControlItem,
            this.viewSplitter});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(597, 533);
         this.layoutControlGroup.Text = "Root";
         this.layoutControlGroup.TextVisible = false;
         // 
         // chartControlItem
         // 
         this.chartControlItem.Control = this.chartPanel;
         this.chartControlItem.CustomizationFormText = "layoutControlItem1";
         this.chartControlItem.Location = new System.Drawing.Point(205, 0);
         this.chartControlItem.Name = "chartControlItem";
         this.chartControlItem.Size = new System.Drawing.Size(392, 507);
         this.chartControlItem.Text = "chartControlItem";
         this.chartControlItem.TextSize = new System.Drawing.Size(0, 0);
         this.chartControlItem.TextToControlDistance = 0;
         this.chartControlItem.TextVisible = false;
         // 
         // analysisControlItem
         // 
         this.analysisControlItem.Control = this.analysisPanel;
         this.analysisControlItem.CustomizationFormText = "analysisControlItem";
         this.analysisControlItem.Location = new System.Drawing.Point(0, 0);
         this.analysisControlItem.Name = "analysisControlItem";
         this.analysisControlItem.Size = new System.Drawing.Size(200, 507);
         this.analysisControlItem.Text = "analysisControlItem";
         this.analysisControlItem.TextSize = new System.Drawing.Size(0, 0);
         this.analysisControlItem.TextToControlDistance = 0;
         this.analysisControlItem.TextVisible = false;
         // 
         // buttonControlItem
         // 
         this.buttonControlItem.Control = this.btnSwithPKAnalysisPlot;
         this.buttonControlItem.CustomizationFormText = "buttonControlItem";
         this.buttonControlItem.Location = new System.Drawing.Point(0, 507);
         this.buttonControlItem.Name = "buttonControlItem";
         this.buttonControlItem.Size = new System.Drawing.Size(597, 26);
         this.buttonControlItem.Text = "buttonControlItem";
         this.buttonControlItem.TextSize = new System.Drawing.Size(0, 0);
         this.buttonControlItem.TextToControlDistance = 0;
         this.buttonControlItem.TextVisible = false;
         // 
         // splitterItem1
         // 
         this.viewSplitter.AllowHotTrack = true;
         this.viewSplitter.CustomizationFormText = "splitterItem1";
         this.viewSplitter.Location = new System.Drawing.Point(200, 0);
         this.viewSplitter.Name = "viewSplitter";
         this.viewSplitter.Size = new System.Drawing.Size(5, 507);
         // 
         // SimulationChartView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "SimulationTimeProfileChartView";
         this.Size = new System.Drawing.Size(597, 533);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.analysisPanel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chartPanel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chartControlItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.analysisControlItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.buttonControlItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.viewSplitter)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      protected DevExpress.XtraEditors.SimpleButton btnSwithPKAnalysisPlot;
      private DevExpress.XtraEditors.PanelControl chartPanel;
      private DevExpress.XtraLayout.LayoutControlItem chartControlItem;
      private DevExpress.XtraEditors.PanelControl analysisPanel;
      private DevExpress.XtraLayout.LayoutControlItem analysisControlItem;
      private DevExpress.XtraLayout.LayoutControlItem buttonControlItem;
      private DevExpress.XtraLayout.SplitterItem viewSplitter;


   }
}


