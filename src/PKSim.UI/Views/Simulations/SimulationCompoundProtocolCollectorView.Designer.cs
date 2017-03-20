using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Simulations
{
   partial class SimulationCompoundProtocolCollectorView
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
         this.layoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl(); 
         this.uxHintPanel = new UxHintPanel();
         this.panelChartView = new DevExpress.XtraEditors.PanelControl();
         this.panelCollectorView = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemCollector = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemChart = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemWarning = new DevExpress.XtraLayout.LayoutControlItem();
         this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelChartView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelCollectorView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCollector)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemChart)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.uxHintPanel);
         this.layoutControl1.Controls.Add(this.panelChartView);
         this.layoutControl1.Controls.Add(this.panelCollectorView);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(663, 214, 250, 350);
         this.layoutControl1.Root = this.layoutControlGroup;
         this.layoutControl1.Size = new System.Drawing.Size(432, 434);
         this.layoutControl1.TabIndex = 0;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // uxHintPanel
         // 
         this.uxHintPanel.Location = new System.Drawing.Point(12, 12);
         this.uxHintPanel.MaximumSize = new System.Drawing.Size(1000000, 40);
         this.uxHintPanel.MinimumSize = new System.Drawing.Size(200, 40);
         this.uxHintPanel.Name = "uxHintPanel";
         this.uxHintPanel.NoteText = "";
         this.uxHintPanel.Size = new System.Drawing.Size(408, 40);
         this.uxHintPanel.TabIndex = 6;
         // 
         // panelChartView
         // 
         this.panelChartView.Location = new System.Drawing.Point(109, 210);
         this.panelChartView.Name = "panelChartView";
         this.panelChartView.Size = new System.Drawing.Size(311, 212);
         this.panelChartView.TabIndex = 5;
         // 
         // panelCollectorView
         // 
         this.panelCollectorView.Location = new System.Drawing.Point(107, 54);
         this.panelCollectorView.Name = "panelCollectorView";
         this.panelCollectorView.Size = new System.Drawing.Size(315, 149);
         this.panelCollectorView.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemCollector,
            this.layoutItemChart,
            this.layoutItemWarning,
            this.splitterItem1});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.Size = new System.Drawing.Size(432, 434);
         this.layoutControlGroup.Text = "Root";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemCollector
         // 
         this.layoutItemCollector.Control = this.panelCollectorView;
         this.layoutItemCollector.CustomizationFormText = "layoutItemCollector";
         this.layoutItemCollector.Location = new System.Drawing.Point(0, 44);
         this.layoutItemCollector.Name = "layoutItemCollector";
         this.layoutItemCollector.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemCollector.Size = new System.Drawing.Size(412, 149);
         this.layoutItemCollector.Text = "layoutItemCollector";
         this.layoutItemCollector.TextSize = new System.Drawing.Size(94, 13);
         // 
         // layoutItemChart
         // 
         this.layoutItemChart.Control = this.panelChartView;
         this.layoutItemChart.CustomizationFormText = "layoutItemChart";
         this.layoutItemChart.Location = new System.Drawing.Point(0, 198);
         this.layoutItemChart.Name = "layoutItemChart";
         this.layoutItemChart.Size = new System.Drawing.Size(412, 216);
         this.layoutItemChart.Text = "layoutItemChart";
         this.layoutItemChart.TextSize = new System.Drawing.Size(94, 13);
         // 
         // layoutItemWarning
         // 
         this.layoutItemWarning.Control = this.uxHintPanel;
         this.layoutItemWarning.CustomizationFormText = "layoutItemWarning";
         this.layoutItemWarning.Location = new System.Drawing.Point(0, 0);
         this.layoutItemWarning.Name = "layoutItemWarning";
         this.layoutItemWarning.Size = new System.Drawing.Size(412, 44);
         this.layoutItemWarning.Text = "layoutItemWarning";
         this.layoutItemWarning.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemWarning.TextToControlDistance = 0;
         this.layoutItemWarning.TextVisible = false;
         // 
         // splitterItem1
         // 
         this.splitterItem1.AllowHotTrack = true;
         this.splitterItem1.CustomizationFormText = "splitterItem1";
         this.splitterItem1.Location = new System.Drawing.Point(0, 193);
         this.splitterItem1.Name = "splitterItem1";
         this.splitterItem1.Size = new System.Drawing.Size(412, 5);
         // 
         // SimulationCompoundProtocolCollectorView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl1);
         this.Name = "SimulationCompoundProtocolCollectorView";
         this.Size = new System.Drawing.Size(432, 434);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelChartView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelCollectorView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCollector)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemChart)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.PanelControl panelChartView;
      private DevExpress.XtraEditors.PanelControl panelCollectorView;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCollector;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemChart;
      private UxHintPanel uxHintPanel;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemWarning;
      private DevExpress.XtraLayout.SplitterItem splitterItem1;
   }
}
