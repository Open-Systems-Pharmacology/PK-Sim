namespace PKSim.UI.Views.Individuals
{
   partial class EditIndividualView
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
         this.tabEditIndividual = new DevExpress.XtraTab.XtraTabControl();
         ((System.ComponentModel.ISupportInitialize)(this.tabEditIndividual)).BeginInit();
         this.SuspendLayout();
         // 
         // tabEditIndividual
         // 
         this.tabEditIndividual.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.tabEditIndividual.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.tabEditIndividual.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tabEditIndividual.Location = new System.Drawing.Point(0, 0);
         this.tabEditIndividual.Name = "tabEditIndividual";
         this.tabEditIndividual.Size = new System.Drawing.Size(684, 552);
         this.tabEditIndividual.TabIndex = 0;
         // 
         // EditIndividualView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "EditIndividualView";
         this.ClientSize = new System.Drawing.Size(684, 552);
         this.Controls.Add(this.tabEditIndividual);
         this.Name = "EditIndividualView";
         this.Text = "EditIndividualView";
         ((System.ComponentModel.ISupportInitialize)(this.tabEditIndividual)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraTab.XtraTabControl tabEditIndividual;
   }
}