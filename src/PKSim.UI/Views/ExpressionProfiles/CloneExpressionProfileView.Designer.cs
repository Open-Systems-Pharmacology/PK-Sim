
namespace PKSim.UI.Views.ExpressionProfiles
{
   partial class CloneExpressionProfileView
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
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.tbCategory = new DevExpress.XtraEditors.TextEdit();
         this.cbMoleculeName = new OSPSuite.UI.Controls.UxMRUEdit();
         this.layoutItemMoleculeName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCategory = new DevExpress.XtraLayout.LayoutControlItem();
         this.cbSpecies = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.layoutItemSpecies = new DevExpress.XtraLayout.LayoutControlItem();
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
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbCategory.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(455, 12);
         this.btnCancel.Size = new System.Drawing.Size(93, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(341, 12);
         this.btnOk.Size = new System.Drawing.Size(110, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 93);
         this.layoutControlBase.Size = new System.Drawing.Size(560, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(160, 22);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(560, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(329, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(114, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(443, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(97, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(164, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(165, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(164, 26);
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.cbSpecies);
         this.layoutControl.Controls.Add(this.cbMoleculeName);
         this.layoutControl.Controls.Add(this.tbCategory);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(560, 93);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeName,
            this.layoutItemCategory,
            this.layoutItemSpecies});
         this.Root.Name = "Root";
         this.Root.Size = new System.Drawing.Size(560, 93);
         this.Root.TextVisible = false;
         // 
         // tbCategory
         // 
         this.tbCategory.Location = new System.Drawing.Point(144, 60);
         this.tbCategory.Name = "tbCategory";
         this.tbCategory.Size = new System.Drawing.Size(404, 20);
         this.tbCategory.StyleController = this.layoutControl;
         this.tbCategory.TabIndex = 5;
         // 
         // cbMoleculeName
         // 
         this.cbMoleculeName.Location = new System.Drawing.Point(144, 36);
         this.cbMoleculeName.Name = "cbMoleculeName";
         this.cbMoleculeName.Properties.AllowRemoveMRUItems = false;
         this.cbMoleculeName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbMoleculeName.Size = new System.Drawing.Size(404, 20);
         this.cbMoleculeName.StyleController = this.layoutControl;
         this.cbMoleculeName.TabIndex = 4;
         // 
         // layoutItemMoleculeName
         // 
         this.layoutItemMoleculeName.Control = this.cbMoleculeName;
         this.layoutItemMoleculeName.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
         this.layoutItemMoleculeName.CustomizationFormText = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.Location = new System.Drawing.Point(0, 24);
         this.layoutItemMoleculeName.Name = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.Size = new System.Drawing.Size(540, 24);
         this.layoutItemMoleculeName.Text = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.TextSize = new System.Drawing.Size(120, 13);
         // 
         // layoutItemCategory
         // 
         this.layoutItemCategory.Control = this.tbCategory;
         this.layoutItemCategory.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
         this.layoutItemCategory.CustomizationFormText = "layoutItemCategory";
         this.layoutItemCategory.Location = new System.Drawing.Point(0, 48);
         this.layoutItemCategory.Name = "layoutItemCategory";
         this.layoutItemCategory.Size = new System.Drawing.Size(540, 25);
         this.layoutItemCategory.Text = "layoutItemCategory";
         this.layoutItemCategory.TextSize = new System.Drawing.Size(120, 13);
         // 
         // cbSpecies
         // 
         this.cbSpecies.Location = new System.Drawing.Point(144, 12);
         this.cbSpecies.Name = "cbSpecies";
         this.cbSpecies.Properties.AllowMouseWheel = false;
         this.cbSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSpecies.Size = new System.Drawing.Size(404, 20);
         this.cbSpecies.StyleController = this.layoutControl;
         this.cbSpecies.TabIndex = 6;
         // 
         // layoutItemSpecies
         // 
         this.layoutItemSpecies.Control = this.cbSpecies;
         this.layoutItemSpecies.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSpecies.Name = "layoutItemSpecies";
         this.layoutItemSpecies.Size = new System.Drawing.Size(540, 24);
         this.layoutItemSpecies.TextSize = new System.Drawing.Size(120, 13);
         // 
         // CloneExpressionProfileView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "CloneExpressionProfileView";
         this.ClientSize = new System.Drawing.Size(560, 139);
         this.Controls.Add(this.layoutControl);
         this.Name = "CloneExpressionProfileView";
         this.Text = "CloneExpressionProfileView";
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
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbCategory.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).EndInit();
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
   }
}