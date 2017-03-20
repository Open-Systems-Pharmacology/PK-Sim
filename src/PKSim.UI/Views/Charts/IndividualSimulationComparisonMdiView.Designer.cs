namespace PKSim.UI.Views.Charts
{
   partial class IndividualSimulationComparisonMdiView
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

      #region Windows Form Designer generated code

      private void InitializeComponent()
      {
         this.panelControl = new DevExpress.XtraEditors.PanelControl();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControl)).BeginInit();
         this.SuspendLayout();
         // 
         // panelControl1
         // 
         this.panelControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panelControl.Location = new System.Drawing.Point(0, 0);
         this.panelControl.Name = "panelControl";
         this.panelControl.Size = new System.Drawing.Size(526, 428);
         this.panelControl.TabIndex = 0;
         // 
         // SummaryChartMdiView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.ClientSize = new System.Drawing.Size(526, 428);
         this.Controls.Add(this.panelControl);
         this.Name = "IndividualSimulationComparisonMdiView";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControl)).EndInit();
         this.ResumeLayout(false);

      }
      #endregion

      private DevExpress.XtraEditors.PanelControl panelControl;


   }
}