
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
         this.cbMoleculeName = new OSPSuite.UI.Controls.UxMRUEdit();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeName = new DevExpress.XtraLayout.LayoutControlItem();
         this.cbCategory = new OSPSuite.UI.Controls.UxMRUEdit();
         this.layoutItemCategory = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbCategory.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // tablePanel
         // 
         this.tablePanel.Location = new System.Drawing.Point(0, 79);
         this.tablePanel.Size = new System.Drawing.Size(577, 43);
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.cbCategory);
         this.layoutControl.Controls.Add(this.cbMoleculeName);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(577, 79);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // cbMoleculeName
         // 
         this.cbMoleculeName.Location = new System.Drawing.Point(141, 10);
         this.cbMoleculeName.Name = "cbMoleculeName";
         this.cbMoleculeName.Properties.AllowRemoveMRUItems = false;
         this.cbMoleculeName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbMoleculeName.Size = new System.Drawing.Size(425, 20);
         this.cbMoleculeName.StyleController = this.layoutControl;
         this.cbMoleculeName.TabIndex = 4;
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeName,
            this.layoutItemCategory,
            this.emptySpaceItem1});
         this.Root.Name = "Root";
         this.Root.Size = new System.Drawing.Size(577, 79);
         this.Root.TextVisible = false;
         // 
         // layoutItemMoleculeName
         // 
         this.layoutItemMoleculeName.Control = this.cbMoleculeName;
         this.layoutItemMoleculeName.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeName.Name = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.Size = new System.Drawing.Size(559, 24);
         this.layoutItemMoleculeName.TextSize = new System.Drawing.Size(120, 13);
         // 
         // cbCategory
         // 
         this.cbCategory.Location = new System.Drawing.Point(141, 34);
         this.cbCategory.Name = "cbCategory";
         this.cbCategory.Properties.AllowRemoveMRUItems = false;
         this.cbCategory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbCategory.Size = new System.Drawing.Size(425, 20);
         this.cbCategory.StyleController = this.layoutControl;
         this.cbCategory.TabIndex = 6;
         // 
         // layoutItemCategory
         // 
         this.layoutItemCategory.Control = this.cbCategory;
         this.layoutItemCategory.Location = new System.Drawing.Point(0, 24);
         this.layoutItemCategory.Name = "layoutItemCategory";
         this.layoutItemCategory.Size = new System.Drawing.Size(559, 24);
         this.layoutItemCategory.TextSize = new System.Drawing.Size(120, 13);
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 48);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(559, 15);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // RenameExpressionProfileView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "RenameExpressionProfileView";
         this.ClientSize = new System.Drawing.Size(577, 122);
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
         this.Name = "RenameExpressionProfileView";
         this.Text = "RenameExpressionProfileView";
         this.Controls.SetChildIndex(this.tablePanel, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbCategory.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private OSPSuite.UI.Controls.UxMRUEdit cbMoleculeName;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeName;
      private OSPSuite.UI.Controls.UxMRUEdit cbCategory;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCategory;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
   }
}