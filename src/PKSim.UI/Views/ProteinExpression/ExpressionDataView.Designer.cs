using DevExpress.Utils;

namespace PKSim.UI.Views.ProteinExpression
{
   partial class ExpressionDataView
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
         DevExpress.XtraCharts.XYDiagram xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
         DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
         DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel1 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
         DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series();
         DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel2 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
         DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel3 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
         this.pgrdExpressionData = new DevExpress.XtraPivotGrid.PivotGridControl();
         this.splExpressionData = new DevExpress.XtraEditors.SplitterControl();
         this.chrtExpressionData = new DevExpress.XtraCharts.ChartControl();
         ((System.ComponentModel.ISupportInitialize)(this.pgrdExpressionData)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chrtExpressionData)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(series2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel3)).BeginInit();
         this.SuspendLayout();
         // 
         // pgrdExpressionData
         // 
         this.pgrdExpressionData.Cursor = System.Windows.Forms.Cursors.Default;
         this.pgrdExpressionData.Dock = System.Windows.Forms.DockStyle.Top;
         this.pgrdExpressionData.Location = new System.Drawing.Point(0, 0);
         this.pgrdExpressionData.Name = "pgrdExpressionData";
         this.pgrdExpressionData.Size = new System.Drawing.Size(343, 168);
         this.pgrdExpressionData.TabIndex = 0;
         // 
         // splExpressionData
         // 
         this.splExpressionData.Dock = System.Windows.Forms.DockStyle.Top;
         this.splExpressionData.Location = new System.Drawing.Point(0, 168);
         this.splExpressionData.Name = "splExpressionData";
         this.splExpressionData.Size = new System.Drawing.Size(343, 6);
         this.splExpressionData.TabIndex = 1;
         this.splExpressionData.TabStop = false;
         // 
         // chrtExpressionData
         // 
         xyDiagram1.AxisX.WholeRange.AutoSideMargins = true;
         xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
         xyDiagram1.AxisY.WholeRange.AutoSideMargins = true;
         xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
         this.chrtExpressionData.Diagram = xyDiagram1;
         this.chrtExpressionData.Dock = System.Windows.Forms.DockStyle.Fill;
         this.chrtExpressionData.Location = new System.Drawing.Point(0, 174);
         this.chrtExpressionData.Name = "chrtExpressionData";
         sideBySideBarSeriesLabel1.LineVisibility = DefaultBoolean.True;
         series1.Label = sideBySideBarSeriesLabel1;
         series1.Name = "Series 1";
         sideBySideBarSeriesLabel2.LineVisibility = DefaultBoolean.True;
         series2.Label = sideBySideBarSeriesLabel2;
         series2.Name = "Series 2";
         this.chrtExpressionData.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1,
        series2};
         sideBySideBarSeriesLabel3.LineVisibility = DefaultBoolean.True;
         this.chrtExpressionData.SeriesTemplate.Label = sideBySideBarSeriesLabel3;
         this.chrtExpressionData.Size = new System.Drawing.Size(343, 139);
         this.chrtExpressionData.TabIndex = 2;
         // 
         // ExpressionDataView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.chrtExpressionData);
         this.Controls.Add(this.splExpressionData);
         this.Controls.Add(this.pgrdExpressionData);
         this.Name = "ExpressionDataView";
         this.Size = new System.Drawing.Size(343, 313);
         ((System.ComponentModel.ISupportInitialize)(this.pgrdExpressionData)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(series2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chrtExpressionData)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraPivotGrid.PivotGridControl pgrdExpressionData;
      private DevExpress.XtraEditors.SplitterControl splExpressionData;
      private DevExpress.XtraCharts.ChartControl chrtExpressionData;


   }
}
