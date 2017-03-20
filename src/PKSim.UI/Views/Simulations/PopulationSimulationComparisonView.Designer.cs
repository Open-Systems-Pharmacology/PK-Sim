namespace PKSim.UI.Views.Simulations
{
   partial class PopulationSimulationComparisonView
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

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.tabAnalyses = new DevExpress.XtraTab.XtraTabControl();
         ((System.ComponentModel.ISupportInitialize)(this.tabAnalyses)).BeginInit();
         this.SuspendLayout();
         // 
         // tabEditIndividual
         // 
         this.tabAnalyses.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.tabAnalyses.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.tabAnalyses.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tabAnalyses.Location = new System.Drawing.Point(0, 0);
         this.tabAnalyses.Name = "tabAnalyses";
         this.tabAnalyses.Size = new System.Drawing.Size(684, 552);
         this.tabAnalyses.TabIndex = 0;
         // 
         // EditIndividualView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "PopulationSimulationComparisonView";
         this.ClientSize = new System.Drawing.Size(684, 552);
         this.Controls.Add(this.tabAnalyses);
         this.Name = "PopulationSimulationComparisonView";
         this.Text = "PopulationSimulationComparisonView";
         ((System.ComponentModel.ISupportInitialize)(this.tabAnalyses)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraTab.XtraTabControl tabAnalyses;
   }
 
}
