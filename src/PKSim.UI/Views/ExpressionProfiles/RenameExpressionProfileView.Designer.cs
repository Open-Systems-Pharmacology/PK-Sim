
namespace PKSim.UI.Views.ExpressionProfiles
{
   partial class RenameExpressionProfileView
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.tbCategory = new DevExpress.XtraEditors.TextEdit();
         this.cbMoleculeName = new OSPSuite.UI.Controls.UxMRUEdit();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCategory = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbCategory.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.tbCategory);
         this.layoutControl.Controls.Add(this.cbMoleculeName);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(668, 84);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // tbCategory
         // 
         this.tbCategory.Location = new System.Drawing.Point(171, 40);
         this.tbCategory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.tbCategory.Name = "tbCategory";
         this.tbCategory.Size = new System.Drawing.Size(483, 22);
         this.tbCategory.StyleController = this.layoutControl;
         this.tbCategory.TabIndex = 5;
         // 
         // cbMoleculeName
         // 
         this.cbMoleculeName.Location = new System.Drawing.Point(171, 14);
         this.cbMoleculeName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.cbMoleculeName.Name = "cbMoleculeName";
         this.cbMoleculeName.Properties.AllowRemoveMRUItems = false;
         this.cbMoleculeName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbMoleculeName.Size = new System.Drawing.Size(483, 22);
         this.cbMoleculeName.StyleController = this.layoutControl;
         this.cbMoleculeName.TabIndex = 4;
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeName,
            this.layoutItemCategory});
         this.Root.Name = "Root";
         this.Root.Size = new System.Drawing.Size(668, 84);
         this.Root.TextVisible = false;
         // 
         // layoutItemMoleculeName
         // 
         this.layoutItemMoleculeName.Control = this.cbMoleculeName;
         this.layoutItemMoleculeName.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeName.Name = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.Size = new System.Drawing.Size(644, 26);
         this.layoutItemMoleculeName.TextSize = new System.Drawing.Size(143, 16);
         // 
         // layoutItemCategory
         // 
         this.layoutItemCategory.Control = this.tbCategory;
         this.layoutItemCategory.Location = new System.Drawing.Point(0, 26);
         this.layoutItemCategory.Name = "layoutItemCategory";
         this.layoutItemCategory.Size = new System.Drawing.Size(644, 34);
         this.layoutItemCategory.TextSize = new System.Drawing.Size(143, 16);
         // 
         // RenameExpressionProfileView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "RenameExpressionProfileView";
         this.ClientSize = new System.Drawing.Size(668, 141);
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.Name = "RenameExpressionProfileView";
         this.Text = "RenameExpressionProfileView";
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tbCategory.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.TextEdit tbCategory;
      private OSPSuite.UI.Controls.UxMRUEdit cbMoleculeName;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCategory;
   }
}