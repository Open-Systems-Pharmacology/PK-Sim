using DevExpress.Utils;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Populations
{
   partial class PopulationParameterDistributionView
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
         DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel1 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
         this.chart = new UxHistogramControl();
         ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).BeginInit();
         this.SuspendLayout();
         // 
         // chart
         // 
         this.chart.CrosshairOptions.ArgumentLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(57)))), ((int)(((byte)(205)))));
         this.chart.CrosshairOptions.ValueLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(57)))), ((int)(((byte)(205)))));
         this.chart.DiagramBackColor = System.Drawing.Color.Empty;
         this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
         this.chart.Location = new System.Drawing.Point(0, 0);
         this.chart.Name = "chart";
         this.chart.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
         sideBySideBarSeriesLabel1.LineVisibility = DefaultBoolean.True;
         this.chart.SeriesTemplate.Label = sideBySideBarSeriesLabel1;
         this.chart.Size = new System.Drawing.Size(385, 387);
         this.chart.TabIndex = 1;
         // 
         // PopulationParameterDistributionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.chart);
         this.Name = "PopulationParameterDistributionView";
         this.Size = new System.Drawing.Size(385, 387);
         ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private UxHistogramControl chart;
   }
}
