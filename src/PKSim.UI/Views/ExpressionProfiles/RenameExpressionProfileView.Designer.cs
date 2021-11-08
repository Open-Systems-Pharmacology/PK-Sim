
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
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).BeginInit();
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
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(465, 12);
         this.btnCancel.Size = new System.Drawing.Size(96, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(349, 12);
         this.btnOk.Size = new System.Drawing.Size(112, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 69);
         this.layoutControlBase.Size = new System.Drawing.Size(573, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(165, 22);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(573, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(337, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(116, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(453, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(100, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(169, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(168, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(169, 26);
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.tbCategory);
         this.layoutControl.Controls.Add(this.cbMoleculeName);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(573, 69);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // tbCategory
         // 
         this.tbCategory.Location = new System.Drawing.Point(135, 36);
         this.tbCategory.Name = "tbCategory";
         this.tbCategory.Size = new System.Drawing.Size(426, 20);
         this.tbCategory.StyleController = this.layoutControl;
         this.tbCategory.TabIndex = 5;
         // 
         // cbMoleculeName
         // 
         this.cbMoleculeName.Location = new System.Drawing.Point(135, 12);
         this.cbMoleculeName.Name = "cbMoleculeName";
         this.cbMoleculeName.Properties.AllowRemoveMRUItems = false;
         this.cbMoleculeName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbMoleculeName.Size = new System.Drawing.Size(426, 20);
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
         this.Root.Size = new System.Drawing.Size(573, 69);
         this.Root.TextVisible = false;
         // 
         // layoutItemMoleculeName
         // 
         this.layoutItemMoleculeName.Control = this.cbMoleculeName;
         this.layoutItemMoleculeName.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeName.Name = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.Size = new System.Drawing.Size(553, 24);
         this.layoutItemMoleculeName.TextSize = new System.Drawing.Size(120, 13);
         // 
         // layoutItemCategory
         // 
         this.layoutItemCategory.Control = this.tbCategory;
         this.layoutItemCategory.Location = new System.Drawing.Point(0, 24);
         this.layoutItemCategory.Name = "layoutItemCategory";
         this.layoutItemCategory.Size = new System.Drawing.Size(553, 25);
         this.layoutItemCategory.TextSize = new System.Drawing.Size(120, 13);
         // 
         // RenameExpressionProfileView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "RenameExpressionProfileView";
         this.ClientSize = new System.Drawing.Size(573, 115);
         this.Controls.Add(this.layoutControl);
         this.Name = "RenameExpressionProfileView";
         this.Text = "RenameExpressionProfileView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).EndInit();
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