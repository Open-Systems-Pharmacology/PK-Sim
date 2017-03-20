using DevExpress.XtraBars.Docking;

namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class BoxWhiskerAnalysisResultsView
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
         this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
         this.panelCustomizationForm = new OSPSuite.UI.Controls.UxPanelControl();
         this.splitContainerPivotToChart = new DevExpress.XtraEditors.SplitContainerControl();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
         this.splitContainerControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelCustomizationForm)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerPivotToChart)).BeginInit();
         this.splitContainerPivotToChart.SuspendLayout();
         this.SuspendLayout();
         // 
         // splitContainerControl
         // 
         this.splitContainerControl.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
         this.splitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerControl.Location = new System.Drawing.Point(0, 0);
         this.splitContainerControl.Name = "splitContainerControl";
         this.splitContainerControl.Panel1.Controls.Add(this.panelCustomizationForm);
         this.splitContainerControl.Panel1.Text = "Panel1";
         this.splitContainerControl.Panel2.Controls.Add(this.splitContainerPivotToChart);
         this.splitContainerControl.Panel2.Text = "Panel2";
         this.splitContainerControl.Size = new System.Drawing.Size(647, 436);
         this.splitContainerControl.SplitterPosition = 226;
         this.splitContainerControl.TabIndex = 0;
         this.splitContainerControl.Text = "splitContainerControl1";
         // 
         // panelCustomizationForm
         // 
         this.panelCustomizationForm.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panelCustomizationForm.Location = new System.Drawing.Point(0, 0);
         this.panelCustomizationForm.Name = "panelCustomizationForm";
         this.panelCustomizationForm.Size = new System.Drawing.Size(226, 436);
         this.panelCustomizationForm.TabIndex = 0;
         // 
         // splitContainerPivotToChart
         // 
         this.splitContainerPivotToChart.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
         this.splitContainerPivotToChart.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerPivotToChart.Horizontal = false;
         this.splitContainerPivotToChart.Location = new System.Drawing.Point(0, 0);
         this.splitContainerPivotToChart.Name = "splitContainerPivotToChart";
         this.splitContainerPivotToChart.Panel1.Text = "Panel1";
         this.splitContainerPivotToChart.Panel2.Text = "Panel2";
         this.splitContainerPivotToChart.Size = new System.Drawing.Size(416, 436);
         this.splitContainerPivotToChart.SplitterPosition = 239;
         this.splitContainerPivotToChart.TabIndex = 0;
         this.splitContainerPivotToChart.Text = "splitContainerControl2";
         // 
         // BoxWhiskerPivotChartView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.splitContainerControl);
         this.Name = "BoxWhiskerAnalysisResultsView";
         this.Size = new System.Drawing.Size(647, 436);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
         this.splitContainerControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelCustomizationForm)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerPivotToChart)).EndInit();
         this.splitContainerPivotToChart.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
      private DevExpress.XtraEditors.SplitContainerControl splitContainerPivotToChart;
      private OSPSuite.UI.Controls.UxPanelControl panelCustomizationForm;



   }
}
