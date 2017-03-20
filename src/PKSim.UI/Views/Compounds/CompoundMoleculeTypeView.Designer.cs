namespace PKSim.UI.Views.Compounds
{
   partial class CompoundMoleculeTypeView
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
         _screenBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.chkIsSmallMolecule = new OSPSuite.UI.Controls.UxCheckEdit();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkIsSmallMolecule.Properties)).BeginInit();
         this.SuspendLayout();
         // 
         // chkIsSmallMolecule
         // 
         this.chkIsSmallMolecule.AllowClicksOutsideControlArea = false;
         this.chkIsSmallMolecule.Dock = System.Windows.Forms.DockStyle.Fill;
         this.chkIsSmallMolecule.Location = new System.Drawing.Point(0, 0);
         this.chkIsSmallMolecule.Name = "chkIsSmallMolecule";
         this.chkIsSmallMolecule.Properties.Caption = "";
         this.chkIsSmallMolecule.Size = new System.Drawing.Size(406, 19);
         this.chkIsSmallMolecule.TabIndex = 5;
         // 
         // CompoundMoleculeTypeView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.chkIsSmallMolecule);
         this.Name = "CompoundMoleculeTypeView";
         this.Size = new System.Drawing.Size(406, 23);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkIsSmallMolecule.Properties)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxCheckEdit chkIsSmallMolecule;


   }
}
