namespace PKSim.UI.Views.Core
{
   partial class BuildingBlockWizardView
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
         _buildingBlockBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.tbName = new DevExpress.XtraEditors.TextEdit();
         this.layoutItemName = new DevExpress.XtraLayout.LayoutControlItem();
         this.tabControl = new DevExpress.XtraTab.XtraTabControl();
         this.layoutItemTabControl = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTabControl)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.tabControl);
         this.layoutControl.Controls.Add(this.tbName);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(510, 404);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl1";
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemName,
            this.layoutItemTabControl});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(510, 404);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // tbName
         // 
         this.tbName.Location = new System.Drawing.Point(95, 12);
         this.tbName.Name = "tbName";
         this.tbName.Size = new System.Drawing.Size(403, 20);
         this.tbName.StyleController = this.layoutControl;
         this.tbName.TabIndex = 4;
         // 
         // layoutItemName
         // 
         this.layoutItemName.Control = this.tbName;
         this.layoutItemName.CustomizationFormText = "layoutItemName";
         this.layoutItemName.Location = new System.Drawing.Point(0, 0);
         this.layoutItemName.Name = "layoutItemName";
         this.layoutItemName.Size = new System.Drawing.Size(490, 24);
         this.layoutItemName.Text = "layoutItemName";
         this.layoutItemName.TextSize = new System.Drawing.Size(79, 13);
         // 
         // tabControl
         // 
         this.tabControl.Location = new System.Drawing.Point(10, 34);
         this.tabControl.Name = "tabControl";
         this.tabControl.Size = new System.Drawing.Size(490, 360);
         this.tabControl.TabIndex = 5;
         // 
         // layoutItemTabControl
         // 
         this.layoutItemTabControl.Control = this.tabControl;
         this.layoutItemTabControl.CustomizationFormText = "layoutItemTabControl";
         this.layoutItemTabControl.Location = new System.Drawing.Point(0, 24);
         this.layoutItemTabControl.Name = "layoutItemTabControl";
         this.layoutItemTabControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemTabControl.Size = new System.Drawing.Size(490, 360);
         this.layoutItemTabControl.Text = "layoutItemTabControl";
         this.layoutItemTabControl.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemTabControl.TextToControlDistance = 0;
         this.layoutItemTabControl.TextVisible = false;
         // 
         // BuildingBlockWizardView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "BuildingBlockWizardView";
         this.ClientSize = new System.Drawing.Size(510, 450);
         this.Controls.Add(this.layoutControl);
         this.Name = "BuildingBlockWizardView";
         this.Text = "BuildingBlockWizardView";
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTabControl)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraTab.XtraTabControl tabControl;
      private DevExpress.XtraEditors.TextEdit tbName;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTabControl;
   }
}