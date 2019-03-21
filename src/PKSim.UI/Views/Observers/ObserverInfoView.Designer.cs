namespace PKSim.UI.Views.Observers
{
   partial class ObserverInfoView
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
         this.richEditControl = new DevExpress.XtraRichEdit.RichEditControl();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         this.SuspendLayout();
         // 
         // richEditControl
         // 
         this.richEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.richEditControl.Location = new System.Drawing.Point(0, 0);
         this.richEditControl.Name = "richEditControl";
         this.richEditControl.Size = new System.Drawing.Size(1224, 1171);
         this.richEditControl.TabIndex = 0;
         this.richEditControl.Text = "richEditControl";
         // 
         // ObserverInfoView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.richEditControl);
         this.Margin = new System.Windows.Forms.Padding(12);
         this.Name = "ObserverInfoView";
         this.Size = new System.Drawing.Size(1224, 1171);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraRichEdit.RichEditControl richEditControl;
   }
}
