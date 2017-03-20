namespace PKSim.UI.Views.Populations
{
   partial class EditRandomPopulationView
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
         this.tabEditPopulation = new DevExpress.XtraTab.XtraTabControl();
         ((System.ComponentModel.ISupportInitialize)(this.tabEditPopulation)).BeginInit();
         this.SuspendLayout();
         // 
         // tabEditPopulation
         // 
         this.tabEditPopulation.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tabEditPopulation.Location = new System.Drawing.Point(0, 0);
         this.tabEditPopulation.Name = "tabEditPopulation";
         this.tabEditPopulation.Size = new System.Drawing.Size(425, 408);
         this.tabEditPopulation.TabIndex = 0;
         // 
         // EditRandomPopulationView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(425, 408);
         this.Controls.Add(this.tabEditPopulation);
         this.Name = "EditRandomPopulationView";
         this.Text = "EditRandomPopulationView";
         ((System.ComponentModel.ISupportInitialize)(this.tabEditPopulation)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraTab.XtraTabControl tabEditPopulation;
   }
}