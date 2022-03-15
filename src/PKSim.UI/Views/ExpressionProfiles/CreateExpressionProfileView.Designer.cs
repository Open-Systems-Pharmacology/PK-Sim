
namespace PKSim.UI.Views.ExpressionProfiles
{
   partial class CreateExpressionProfileView
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
         this.labelCategoryDescription = new DevExpress.XtraEditors.LabelControl();
         this.cbMoleculeName = new OSPSuite.UI.Controls.UxMRUEdit();
         this.tbCategory = new DevExpress.XtraEditors.TextEdit();
         this.cbSpecies = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCategory = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSpecies = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
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
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbCategory.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(536, 8);
         this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
         this.btnCancel.Size = new System.Drawing.Size(115, 27);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(400, 8);
         this.btnOk.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
         this.btnOk.Size = new System.Drawing.Size(134, 27);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 138);
         this.layoutControlBase.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
         this.layoutControlBase.Size = new System.Drawing.Size(659, 57);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Location = new System.Drawing.Point(8, 8);
         this.btnExtra.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
         this.btnExtra.Size = new System.Drawing.Size(195, 27);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(659, 57);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(392, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(136, 43);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(528, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(117, 43);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(197, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(195, 43);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(197, 43);
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.labelCategoryDescription);
         this.layoutControl.Controls.Add(this.cbMoleculeName);
         this.layoutControl.Controls.Add(this.tbCategory);
         this.layoutControl.Controls.Add(this.cbSpecies);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(659, 138);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // labelCategoryDescription
         // 
         this.labelCategoryDescription.Location = new System.Drawing.Point(6, 79);
         this.labelCategoryDescription.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.labelCategoryDescription.Name = "labelCategoryDescription";
         this.labelCategoryDescription.Size = new System.Drawing.Size(141, 16);
         this.labelCategoryDescription.StyleController = this.layoutControl;
         this.labelCategoryDescription.TabIndex = 7;
         this.labelCategoryDescription.Text = "labelCategoryDescription";
         // 
         // cbMoleculeName
         // 
         this.cbMoleculeName.Location = new System.Drawing.Point(155, 31);
         this.cbMoleculeName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.cbMoleculeName.Name = "cbMoleculeName";
         this.cbMoleculeName.Properties.AllowRemoveMRUItems = false;
         this.cbMoleculeName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbMoleculeName.Size = new System.Drawing.Size(498, 22);
         this.cbMoleculeName.StyleController = this.layoutControl;
         this.cbMoleculeName.TabIndex = 4;
         // 
         // tbCategory
         // 
         this.tbCategory.Location = new System.Drawing.Point(155, 55);
         this.tbCategory.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.tbCategory.Name = "tbCategory";
         this.tbCategory.Size = new System.Drawing.Size(498, 22);
         this.tbCategory.StyleController = this.layoutControl;
         this.tbCategory.TabIndex = 5;
         // 
         // cbSpecies
         // 
         this.cbSpecies.Location = new System.Drawing.Point(155, 7);
         this.cbSpecies.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.cbSpecies.Name = "cbSpecies";
         this.cbSpecies.Properties.AllowMouseWheel = false;
         this.cbSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSpecies.Size = new System.Drawing.Size(498, 22);
         this.cbSpecies.StyleController = this.layoutControl;
         this.cbSpecies.TabIndex = 6;
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeName,
            this.layoutItemCategory,
            this.layoutItemSpecies,
            this.layoutControlItem1});
         this.Root.Name = "Root";
         this.Root.Size = new System.Drawing.Size(659, 138);
         this.Root.TextVisible = false;
         // 
         // layoutItemMoleculeName
         // 
         this.layoutItemMoleculeName.Control = this.cbMoleculeName;
         this.layoutItemMoleculeName.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
         this.layoutItemMoleculeName.CustomizationFormText = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.Location = new System.Drawing.Point(0, 24);
         this.layoutItemMoleculeName.Name = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.Size = new System.Drawing.Size(649, 24);
         this.layoutItemMoleculeName.TextSize = new System.Drawing.Size(143, 16);
         // 
         // layoutItemCategory
         // 
         this.layoutItemCategory.Control = this.tbCategory;
         this.layoutItemCategory.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
         this.layoutItemCategory.CustomizationFormText = "layoutItemCategory";
         this.layoutItemCategory.Location = new System.Drawing.Point(0, 48);
         this.layoutItemCategory.Name = "layoutItemCategory";
         this.layoutItemCategory.Size = new System.Drawing.Size(649, 24);
         this.layoutItemCategory.TextSize = new System.Drawing.Size(143, 16);
         // 
         // layoutItemSpecies
         // 
         this.layoutItemSpecies.Control = this.cbSpecies;
         this.layoutItemSpecies.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSpecies.Name = "layoutItemSpecies";
         this.layoutItemSpecies.Size = new System.Drawing.Size(649, 24);
         this.layoutItemSpecies.TextSize = new System.Drawing.Size(143, 16);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.labelCategoryDescription;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 72);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(649, 54);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // CreateExpressionProfileView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "CreateExpressionProfileView";
         this.ClientSize = new System.Drawing.Size(659, 195);
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.Name = "CreateExpressionProfileView";
         this.Text = "CreateExpressionProfileView";
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
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbCategory.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private OSPSuite.UI.Controls.UxMRUEdit cbMoleculeName;
      private DevExpress.XtraEditors.TextEdit tbCategory;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCategory;
      private Core.UxImageComboBoxEdit cbSpecies;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSpecies;
      private DevExpress.XtraEditors.LabelControl labelCategoryDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
   }
}