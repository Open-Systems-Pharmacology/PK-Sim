using DevExpress.Utils;
using PKSim.UI.Views.Core;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Protocols
{
   partial class ProtocolChartView
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
         this.components = new System.ComponentModel.Container();
         DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel1 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
         this.chart = new UxChartControl();
         this._toolTipController = new DevExpress.Utils.ToolTipController(this.components);
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl(); 
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemChart = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemChart)).BeginInit();
         this.SuspendLayout();
         // 
         // chart
         // 
         this.chart.Location = new System.Drawing.Point(2, 2);
         this.chart.Name = "chart";
         this.chart.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
         sideBySideBarSeriesLabel1.LineVisibility = DevExpress.Utils.DefaultBoolean.True;
         this.chart.SeriesTemplate.Label = sideBySideBarSeriesLabel1;
         this.chart.Size = new System.Drawing.Size(603, 388);
         this.chart.TabIndex = 0;
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.chart);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(607, 392);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemChart});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(607, 392);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemChart
         // 
         this.layoutItemChart.Control = this.chart;
         this.layoutItemChart.Location = new System.Drawing.Point(0, 0);
         this.layoutItemChart.Name = "layoutItemChart";
         this.layoutItemChart.Size = new System.Drawing.Size(607, 392);
         this.layoutItemChart.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemChart.TextVisible = false;
         // 
         // ProtocolChartView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "ProtocolChartView";
         this.Size = new System.Drawing.Size(607, 392);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemChart)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private UxChartControl chart;
      private DevExpress.Utils.ToolTipController _toolTipController;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemChart;

   }
}


