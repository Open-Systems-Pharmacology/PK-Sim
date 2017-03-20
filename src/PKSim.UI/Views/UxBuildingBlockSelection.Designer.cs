namespace PKSim.UI.Views
{
   partial class UxBuildingBlockSelection
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
         cleanup();
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.lblDesigner = new DevExpress.XtraEditors.LabelControl();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         this.SuspendLayout();
         // 
         // lblDesigner
         // 
         this.lblDesigner.Appearance.BackColor = System.Drawing.Color.Lime;
         this.lblDesigner.Appearance.Options.UseBackColor = true;
         this.lblDesigner.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
         this.lblDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
         this.lblDesigner.Location = new System.Drawing.Point(0, 0);
         this.lblDesigner.Name = "lblDesigner";
         this.lblDesigner.Size = new System.Drawing.Size(431, 26);
         this.lblDesigner.TabIndex = 2;
         this.lblDesigner.Text = "Label For Designer Time Only";
         // 
         // UxBuildingBlockSelection
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.lblDesigner);
         this.MaximumSize = new System.Drawing.Size(10000, 26);
         this.MinimumSize = new System.Drawing.Size(0, 26);
         this.Name = "UxBuildingBlockSelection";
         this.Size = new System.Drawing.Size(431, 26);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraEditors.LabelControl lblDesigner;


   }
}


