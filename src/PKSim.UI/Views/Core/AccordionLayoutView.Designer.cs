namespace PKSim.UI.Views.Core
{
   partial class AccordionLayoutView
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
         this.emptySpaceGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.SuspendLayout();
         // 
         // emptySpaceGroup
         // 
         this.emptySpaceGroup.CustomizationFormText = "layoutControlGroup1";
         this.emptySpaceGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.emptySpaceGroup.GroupBordersVisible = false;
         this.emptySpaceGroup.Name = "emptySpaceGroup";
         this.emptySpaceGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.emptySpaceGroup.Size = new System.Drawing.Size(175, 185);
         this.emptySpaceGroup.Text = "layoutControlGroup";
         this.emptySpaceGroup.TextVisible = false;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(4);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(749, 164, 520, 350);
         this.layoutControl.Root = this.emptySpaceGroup;
         this.layoutControl.Size = new System.Drawing.Size(175, 185);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // AccordionLayoutView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
         this.Name = "AccordionLayoutView";
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup emptySpaceGroup;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
   }
}
