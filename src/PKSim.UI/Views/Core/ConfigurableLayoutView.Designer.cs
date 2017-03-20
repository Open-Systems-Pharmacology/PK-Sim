namespace PKSim.UI.Views.Core
{
   partial class ConfigurableLayoutView
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
         this.layoutMainControl = new OSPSuite.UI.Controls.UxLayoutControl(); 
         this.layoutMainGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutMainControl
         // 
         this.layoutMainControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutMainControl.Location = new System.Drawing.Point(0, 0);
         this.layoutMainControl.Name = "layoutMainControl";
         this.layoutMainControl.Root = this.layoutMainGroup;
         this.layoutMainControl.Size = new System.Drawing.Size(487, 369);
         this.layoutMainControl.TabIndex = 0;
         this.layoutMainControl.Text = "layoutMainControl";
         // 
         // layoutMainGroup
         // 
         this.layoutMainGroup.CustomizationFormText = "layoutMainGroup";
         this.layoutMainGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutMainGroup.GroupBordersVisible = false;
         this.layoutMainGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutMainGroup.Name = "layoutMainGroup";
         this.layoutMainGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutMainGroup.Size = new System.Drawing.Size(487, 369);
         this.layoutMainGroup.Text = "layoutMainGroup";
         this.layoutMainGroup.TextVisible = false;
         // 
         // ConfigurableLayoutView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutMainControl);
         this.Name = "ConfigurableLayoutView";
         this.Size = new System.Drawing.Size(487, 369);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutMainControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutMainGroup;


   }
}
