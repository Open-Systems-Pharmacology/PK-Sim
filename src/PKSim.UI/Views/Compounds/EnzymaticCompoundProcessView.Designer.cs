namespace PKSim.UI.Views.Compounds
{
   partial class EnzymaticCompoundProcessView
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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl(); 
         this.mainLayoutGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.panelControl = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.mainLayoutGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.panelControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(700, 177, 589, 350);
         this.layoutControl.Root = this.mainLayoutGroup;
         this.layoutControl.Size = new System.Drawing.Size(474, 152);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // mainLayoutGroup
         // 
         this.mainLayoutGroup.CustomizationFormText = "compoundProcessGroup";
         this.mainLayoutGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.mainLayoutGroup.GroupBordersVisible = false;
         this.mainLayoutGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem});
         this.mainLayoutGroup.Location = new System.Drawing.Point(0, 0);
         this.mainLayoutGroup.Name = "mainLayoutGroup";
         this.mainLayoutGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.mainLayoutGroup.Size = new System.Drawing.Size(474, 152);
         this.mainLayoutGroup.Text = "mainLayoutGroup";
         this.mainLayoutGroup.TextVisible = false;
         // 
         // panelControl
         // 
         this.panelControl.Location = new System.Drawing.Point(2, 2);
         this.panelControl.Name = "panelControl";
         this.panelControl.Size = new System.Drawing.Size(470, 148);
         this.panelControl.TabIndex = 4;
         // 
         // layoutControlItem
         // 
         this.layoutControlItem.Control = this.panelControl;
         this.layoutControlItem.CustomizationFormText = "layoutControlItem";
         this.layoutControlItem.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem.Name = "layoutControlItem";
         this.layoutControlItem.Size = new System.Drawing.Size(474, 152);
         this.layoutControlItem.Text = "layoutControlItem";
         this.layoutControlItem.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem.TextToControlDistance = 0;
         this.layoutControlItem.TextVisible = false;
         // 
         // EnzymaticCompoundProcessView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "EnzymaticCompoundProcessView";
         this.Size = new System.Drawing.Size(474, 152);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.mainLayoutGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup mainLayoutGroup;
      private DevExpress.XtraEditors.PanelControl panelControl;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem;
   }
}
